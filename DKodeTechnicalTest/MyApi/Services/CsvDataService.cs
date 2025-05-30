using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MyApi.Services
{
    public class CsvDataService
    {
        private readonly HttpClient _httpClient;

        public CsvDataService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
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

        public async Task<List<T>> ParseCsvFileAsync<T, TMap>(string filePath)
            where TMap : ClassMap<T>
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, culture: CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMap>();
            var records = csv.GetRecords<T>().ToList();
            return records;
        }
    }
}
