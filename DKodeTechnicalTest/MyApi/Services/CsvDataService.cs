using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
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
        private readonly PrepCsvService _prepCsvService;

        public CsvDataService(IHttpClientFactory httpClientFactory, IWarehouseRepository warehouseRepository, PrepCsvService prepCsvService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _warehouseRepository = warehouseRepository;
            _prepCsvService = prepCsvService;
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

 
            await _prepCsvService.ParseCsvFileWithAutoDecectDelimiterAsync<ProductCsvModel, ProductCsvMap>(
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

            await _prepCsvService.ParseCsvFileWithAutoDecectDelimiterAsync<InventoryCsvModel, InventoryCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }


        public async Task ProcessPricesCsvAsync(string url)
        {
            var filePath = await DownloadCsvAsync(url, "Prices.csv");
            int processedCount = 0;

            async Task ProcessBatch(List<PricesCsvModel> batch)
            {
                await _warehouseRepository.BulkInsertOrUpdatePricesAsync(batch);

                processedCount += batch.Count;

            }

            await _prepCsvService.ParseCsvFileWithAutoDecectDelimiterAsync<PricesCsvModel, PricesCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }
    }
}
