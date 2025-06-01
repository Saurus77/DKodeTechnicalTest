using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MyApi.Helpers;

namespace MyApi.Services
{
    /// <summary>
    /// Class for a service processing CSV files with automatic delimiter detecion and parallel batch processing.
    /// </summary>
    public class PrepCsvService
    {
        /// <summary>
        /// Parses CSV file with automatic header and delimiter detection
        /// </summary>
        /// <typeparam name="T">Generic type for auto type handling</typeparam>
        /// <typeparam name="TMap">Classmap type for csv config</typeparam>
        /// <param name="filePath">Path to the csv file</param>
        /// <param name="processBatch">Callback to process each batch</param>
        /// <param name="batchSize">Number of records per patch - def set to 200k</param>
        /// <param name="maxParallelDegree">Max parallel batch processing - def set to 4</param>
        public async Task ParseCsvFileWithAutoDecectDelimiterAsync<T, TMap>(
            string filePath,
            Func<List<T>,Task> processBatch,
            int batchSize = 200_000,
            int maxParallelDegree = 4)
            where TMap : ClassMap<T>
        {
            try
            {
                // Attempt to parse with headers enabled
                await PrepCsv<T, TMap>(filePath, true, processBatch, batchSize, maxParallelDegree);
            }
            catch (HeaderValidationException)
            {
                // On failure fallback and attempt with headers disabled
                await PrepCsv<T, TMap>(filePath, false, processBatch, batchSize, maxParallelDegree);
            }
        }

        /// <summary>
        /// CSV processing method with delimiter detection
        /// </summary>
        /// <typeparam name="T">Generic type for auto type handling</typeparam>
        /// <typeparam name="TMap">Classmap type for csv config</typeparam>
        /// <param name="filePath">Path to the csv file</param>
        /// <param name="hasHeaders">Flag to check is csv has headers</param>
        /// <param name="processBatch">Callback to process each batch</param>
        /// <param name="batchSize">Number of records per patch - def set to 200k</param>
        /// <param name="maxParallelDegree">Max parallel batch processing - def set to 4</param>
        public async Task PrepCsv<T, TMap>(
            string filePath,
            bool hasHeaders,
            Func<List<T>, Task> processBatch,
            int batchSize = 200_000,
            int maxParallelDegree = 4)
            where TMap : ClassMap<T>
        {
            // Set of delimiters to check with
            string[] possibleDelimiters = new[] { ",", ";", "\t" };
            
            // Read first line to check
            string firstLine;
            using (var reader = new StreamReader(filePath, encoding: Encoding.UTF8))
            {
                firstLine = await reader.ReadLineAsync() ?? "";
            }

            // Assign result with most likely delimiter based on first line
            string delimiter = DetectedDelimiter(firstLine, possibleDelimiters);

            // Configure csv reader settings
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                BadDataFound = null, // Ignore bad data
                TrimOptions = TrimOptions.Trim, // Trim whitespace
                MissingFieldFound = null, // Ignore missing fields
                HasHeaderRecord = hasHeaders, // Use headers as sepcified by a flag
                Quote = '"', // Standard quote character
                ReadingExceptionOccurred = Ex => { return false; }, // Continue on errors
            };

            // Initialize batch collection
            var batch = new List<T>(batchSize);

            // Configure parallel processing options
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelDegree
            };

            // Process csv file
            using (var output = new StreamReader(filePath, encoding: Encoding.UTF8))
            using (var csv = new CsvReader(output, config))
            {
                // Register class mapping
                csv.Context.RegisterClassMap<TMap>();

                // Process records asynchronously
                await foreach (var record in csv.GetRecordsAsync<T>())
                {
                    batch.Add(record);

                    // Process batch when size threschold is reached
                    if (batch.Count >= batchSize)
                    {
                        // Process batches parallel
                        await ParallelProcessing.ProcessBatchWithParallelism(batch, processBatch, parallelOptions);
                        batch.Clear();
                        GC.Collect(); // Explicit garabge collector call - deliberate cleanup after large batches processing
                    }
                }

                // Process remaining records in last batch if there are any leftovers
                if (batch.Count > 0)
                {
                    await ParallelProcessing.ProcessBatchWithParallelism(batch, processBatch, parallelOptions);
                }
            }
        }

        /// <summary>
        /// Derects the most likely delimiter used in CSV line
        /// </summary>
        /// <param name="line">A line to check with - preferably first line</param>
        /// <param name="delimiters">Possible delimiters to check</param>
        /// <returns>Most probable delimiter based on amount of occuriencies</returns>
        private string DetectedDelimiter(string line, string[] delimiters)
        {
            // Choose probable delimiter by counting how many times each of possible delimiters occure in a provided line.
            return delimiters.Select(d => new { Delimiter = d, Count = line.Count(c => c == d[0]) }).OrderByDescending(x => x.Count).First().Delimiter;
        }
    }

}
