using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MyApi.Helpers;

namespace MyApi.Services
{
    public class PrepCsvService
    {
        public async Task ParseCsvFileWithAutoDecectDelimiterAsync<T, TMap>(
            string filePath,
            Func<List<T>,Task> processBatch,
            int batchSize = 200_000,
            int maxParallelDegree = 4)
            where TMap : ClassMap<T>
        {
            try
            {
                await PrepCsv<T, TMap>(filePath, true, processBatch, batchSize, maxParallelDegree);
            }
            catch (HeaderValidationException)
            {
                await PrepCsv<T, TMap>(filePath, false, processBatch, batchSize, maxParallelDegree);
            }
        }

        public async Task PrepCsv<T, TMap>(
            string filePath,
            bool hasHeaders,
            Func<List<T>, Task> processBatch,
            int batchSize = 200_000,
            int maxParallelDegree = 4)
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
                HasHeaderRecord = hasHeaders,
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

                    if (batch.Count >= batchSize)
                    {
                        await ParallelProcessing.ProcessBatchWithParallelism(batch, processBatch, parallelOptions);
                        batch.Clear();
                        GC.Collect();
                    }
                }

                if (batch.Count > 0)
                {
                    await ParallelProcessing.ProcessBatchWithParallelism(batch, processBatch, parallelOptions);
                }
            }
        }
        private string DetectedDelimiter(string line, string[] delimiters)
        {
            return delimiters.Select(d => new { Delimiter = d, Count = line.Count(c => c == d[0]) }).OrderByDescending(x => x.Count).First().Delimiter;
        }
    }

}
