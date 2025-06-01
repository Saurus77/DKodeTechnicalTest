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
    /// <summary>
    /// Class for service responsible for downloading, processing and importing data from cvs files
    /// </summary>
    public class CsvDataService
    {
        // Http client for downloading files
        private readonly HttpClient _httpClient;

        // Repository for warehouse data operations
        private readonly IWarehouseRepository _warehouseRepository;

        // Helper service for CSV parsing
        private readonly PrepCsvService _prepCsvService;

        /// <summary>
        /// Initializes new instance of CsvDataService
        /// </summary>
        /// <param name="httpClientFactory">Factory for creating Http clients</param>
        /// <param name="warehouseRepository">Warehouse data repository</param>
        /// <param name="prepCsvService">CSV parsing service</param>
        public CsvDataService(IHttpClientFactory httpClientFactory, IWarehouseRepository warehouseRepository, PrepCsvService prepCsvService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _warehouseRepository = warehouseRepository;
            _prepCsvService = prepCsvService;
        }


        /// <summary>
        /// Downloads CSV file from a specified url and saves it to a temporary location
        /// </summary>
        /// <param name="url">Url of the CSV to download from</param>
        /// <param name="localFileName">Name for a temporary file</param>
        /// <returns>Path to the temporarly downaloaded file</returns>
        public async Task<string> DownloadCsvAsync(string url, string localFileName)
        {
            // Downloads file contents
            var response = await _httpClient.GetAsync(url);

            // Throws on failure
            response.EnsureSuccessStatusCode();

            // Creates temporary file path
            var filePath = Path.Combine(Path.GetTempPath(), localFileName);

            // Saves the downloaded content to a file
            using(var fs = new FileStream(filePath, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
            return filePath;
        }




        /// <summary>
        /// Processes the products CSV file from specified url
        /// </summary>
        /// <param name="url">Url of products csv file</param>
        /// <returns></returns>
        public async Task ProcessProductsCsvAsync(string url)
        {
            // Download cvs file
            var filePath = await DownloadCsvAsync(url, "Products.csv");

            // Tracking processed records
            int processedCount = 0;

            // Local function processing each batch of records
            async Task ProcessBatch(List<ProductCsvModel> batch)
            {
                // Transform FullCategory into sub categories
                foreach (var record in batch)
                {
                    if (!string.IsNullOrEmpty(record.FullCategory))
                    {
                        // Split by " | "
                        // If there is 1 part -> Main
                        // 2 parts -> Sub
                        // 3 or more parts -> Child
                        var parts = record.FullCategory.Split('|', StringSplitOptions.TrimEntries);
                        record.MainCategory = parts.Length > 0 ? parts[0] : null;
                        record.SubCategory = parts.Length > 1 ? parts[1] : null;
                        record.ChildCategory = parts.Length > 2 ? parts[2] : null;
                    }
                }

                // Filter records by wire status and shippping before insertion
                var recordsToInsert = batch.Where(p => !p.IsWire && p.Shipping.Contains("24h"));

                // Insert/Update filtered records
                await _warehouseRepository.BulkInsertOrUpdateProductsAsync(recordsToInsert);

                // Update counter
                processedCount += batch.Count;

            }

            // Parse csv file using prep service
            await _prepCsvService.ParseCsvFileWithAutoDecectDelimiterAsync<ProductCsvModel, ProductCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }


        /// <summary>
        /// Processes the inventory CSV file from specified url
        /// </summary>
        /// <param name="url">Url of inventory csv file</param>
        /// <returns></returns>
        public async Task ProcessInventoryCsvAsync(string url)
        {
            // Download cvs file
            var filePath = await DownloadCsvAsync(url, "Inventory.csv");

            // Tracking processed records
            int processedCount = 0;

            // Local function processing each batch of records
            async Task ProcessBatch(List<InventoryCsvModel> batch)
            {
                // Pattern for shipping filtering
                var pattern = new[] { "24h", "Wysyłka w 24h" };

                // Filter records with shipping methods
                // - not null/empty, matches pattern
                var recordsToInsert = batch.Where(i => !string.IsNullOrEmpty(i.Shipping) && pattern.Any(p => i.Shipping.Contains(p)));

                // Insert/Update filtered records
                await _warehouseRepository.BulkInsertOrUpdateInventoryAsync(recordsToInsert);

                // Update counter
                processedCount += batch.Count;

            }

            // Parse csv file using prep service
            await _prepCsvService.ParseCsvFileWithAutoDecectDelimiterAsync<InventoryCsvModel, InventoryCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }


        /// <summary>
        /// Processes the prices CSV file from specified url
        /// </summary>
        /// <param name="url">Url of prices csv file</param>
        /// <returns></returns>
        public async Task ProcessPricesCsvAsync(string url)
        {
            // Download cvs file
            var filePath = await DownloadCsvAsync(url, "Prices.csv");

            // Tracking processed records
            int processedCount = 0;

            // Local function processing each batch of records
            async Task ProcessBatch(List<PricesCsvModel> batch)
            {
                // Insert/Update filtered records
                await _warehouseRepository.BulkInsertOrUpdatePricesAsync(batch);

                // Update counter
                processedCount += batch.Count;

            }

            // Parse csv file using prep service
            await _prepCsvService.ParseCsvFileWithAutoDecectDelimiterAsync<PricesCsvModel, PricesCsvMap>(
                filePath: filePath,
                processBatch: ProcessBatch
                );
        }
    }
}
