namespace MyApi.Helpers
{
    public static class ParallelProcessing
    {
        public static async Task ProcessBatchWithParallelism<T>(
            List<T> batch,
            Func<List<T>, Task> processBatch,
            ParallelOptions parallelOptions)
        {
            // Split batch into smaller chunks for parallel processing
            var chunks = batch.Chunk(batch.Count / parallelOptions.MaxDegreeOfParallelism);

            await Parallel.ForEachAsync(chunks, parallelOptions, async (chunk, ct) =>
            {
                await processBatch(chunk.ToList());
            });
        }
    }
}
