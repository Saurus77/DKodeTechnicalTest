namespace MyApi.Helpers
{
    // Class to provide utility method for parallel processing of data batches
    // Increases speed of data reading and parsing
    public static class ParallelProcessing
    {
        /// <summary>
        /// Processes items collection batches in parallel
        /// Allows for controll of concurrency - how many batches are processed at the same time
        /// </summary>
        /// <typeparam name="T">Generic type for auto result handling</typeparam>
        /// <param name="batch">A collection of items to process</param>
        /// <param name="processBatch">Async function to process each batch chunk - takes list, returns tasks</param>
        /// <param name="parallelOptions">Options for parallel execution - max concurrent processes</param>
        /// <returns>Task for async data processing</returns>
        public static async Task ProcessBatchWithParallelism<T>(
            List<T> batch,
            Func<List<T>, Task> processBatch,
            ParallelOptions parallelOptions)
        {
            // Calculate chung size by dividing total items (chunk count) by max concurrent processing
            var chunks = batch.Chunk(batch.Count / parallelOptions.MaxDegreeOfParallelism);

            //Process chung using async parallel "foreach"
            //Params:
            //- chunks: collection of data to process
            //- parallelOptions: config - max concurrent processing
            //- async: async delegate to process chunks
            //  - chunk: data collection
            //  - ct: cancellation token

            await Parallel.ForEachAsync(chunks, parallelOptions, async (chunk, ct) =>
            {
                // Convert chunk to list for processing - create list object after enumeration
                // Execute processing
                await processBatch(chunk.ToList());
            });
        }
    }
}
