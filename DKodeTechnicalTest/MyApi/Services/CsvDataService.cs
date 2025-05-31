using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MyApi.Helpers;
using MyApi.Models;
using MyApi.Repositories;

namespace MyApi.Services
{
    public class CsvDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IWarehouseRepository _warehouseRepository;

        public CsvDataService(IHttpClientFactory httpClientFactory, IWarehouseRepository warehouseRepository)
        {
            _httpClient = httpClientFactory.CreateClient();
            _warehouseRepository = warehouseRepository;
        }

        public async Task<string> DownloadCsvAsync(string url, string localFileName)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var filePath = Path.Combine(Path.GetTempPath(), localFileName);

            using(var fs = new FileStream(filePath, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
            return filePath;
        }


        public async Task ParseCsvFileWithAutoDecectDelimiterAsync<T, TMap>(string filePath, Func<List<T>, Task> processBatch, int batchSize = 200_000, int maxParallelDegree = 4)
            where TMap : ClassMap<T>
        {
            string[] possibleDelimiters = new[] { ",", ";", "\t" };

            string firstLine;

            using (var reader = new StreamReader(filePath, encoding: Encoding.UTF8))
            {
                firstLine = await reader.ReadLineAsync() ?? "";
            }

            string delimiter = DetectedDelimiter(firstLine, possibleDelimiters);



            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HasHeaderRecord = true,
                Quote = '"',
                ReadingExceptionOccurred = Ex => { return false; },
            };


            var batch = new List<T>(batchSize);
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelDegree
            };

            using (var output = new StreamReader(filePath, encoding: Encoding.UTF8)) 
            using (var csv = new CsvReader(output, config))
            {
                csv.Context.RegisterClassMap<TMap>();

                await foreach (var record in csv.GetRecordsAsync<T>())
                {
                    batch.Add(record);

                    if(batch.Count >= batchSize)
                    {
                        await ParallelProcessing.ProcessBatchWithParallelism(batch, processBatch, parallelOptions);
                        batch.Clear();
                        GC.Collect();
                    }
                }

                if(batch.Count > 0)
                {
                    await ParallelProcessing.ProcessBatchWithParallelism(batch, processBatch, parallelOptions);
                }
            }
        }

        public async Task ProcessProductsCsvAsync(string url)
        {
            var filePath = await DownloadCsvAsync(url, "Products.csv");
            int processedCount = 0;

            async Task ProcessBatch(List<ProductCsvModel> batch)
            {
                foreach (var record in batch)
                {
                    if (!string.IsNullOrEmpty(record.FullCategory))
                    {
                        var parts = record.FullCategory.Split('|', StringSplitOptions.TrimEntries);
                        record.MainCategory = parts.Length > 0 ? parts[0] : null;
                        record.SubCategory = parts.Length > 1 ? parts[1] : null;
                        record.ChildCategory = parts.Length > 2 ? parts[2] : null;
                    }
                }

                var recordsToInsert = batch.Where(p => !p.IsWire && p.Shipping.Contains("24h"));

                await _warehouseRepository.BulkInsertOrUpdateProductsAsync(recordsToInsert);

                processedCount += batch.Count;

            }

            await ParseCsvFileWithAutoDecectDelimiterAsync<ProductCsvModel, ProductCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }


        public async Task ProcessInventoryCsvAsync(string url)
        {
            var filePath = await DownloadCsvAsync(url, "Inventory.csv");
            int processedCount = 0;

            async Task ProcessBatch(List<InventoryCsvModel> batch)
            {
                var pattern = new[] { "24h", "Wysyłka w 24h" };

                var recordsToInsert = batch.Where(i => !string.IsNullOrEmpty(i.Shipping) && pattern.Any(p => i.Shipping.Contains(p)));

                await _warehouseRepository.BulkInsertOrUpdateInventoryAsync(recordsToInsert);

                processedCount += batch.Count;

            }

            await ParseCsvFileWithAutoDecectDelimiterAsync<InventoryCsvModel, InventoryCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }

        private string DetectedDelimiter(string line, string[] delimiters)
        {
            return delimiters.Select(d => new { Delimiter = d, Count = line.Count(c => c == d[0]) }).OrderByDescending(x => x.Count).First().Delimiter;
        }
    }
}
