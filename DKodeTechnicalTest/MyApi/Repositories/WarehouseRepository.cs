using System.Data;
using System.Data.SqlClient;
using Dapper;
using MyApi.Models;

namespace MyApi.Repositories
{
    /// <summary>
    /// Class for bulk operation on big volume data using SQL Server and Dapper
    /// </summary>
    public class WarehouseRepository : IWarehouseRepository
    {
        // Field to store db connection string
        private readonly string _connectionString;

        public WarehouseRepository(IConfiguration configuration)
        {
            // Assigns connections string form appsettings.json to pvt field
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Performs bulk insert or update operations for product data
        /// </summary>
        /// <param name="productsCsvModel">Collection of product records from cvs</param>
        /// <returns>Task as async operation</returns>
        public async Task BulkInsertOrUpdateProductsAsync(IEnumerable<ProductCsvModel> productsCsvModel)
        {
            // Creates and amanages sql connection
            // Asnychronous resource cleanup by await using
            await using var connection = new SqlConnection(_connectionString);

            // Opens data base connection asnychronously
            await connection.OpenAsync();

            // Begins transaction
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Process data in batches
                foreach (var batch in productsCsvModel.Chunk(10000))
                {
                    // Execute stored procedure for each batch
                    await connection.ExecuteAsync("dbo.usp_UpsertProducts", batch.Select(item => new
                        {
                            item.ID,
                            item.SKU,
                            item.Name,
                            item.EAN,
                            item.ProducerName,
                            item.MainCategory,
                            item.SubCategory,
                            item.ChildCategory,
                            item.IsWire,
                            item.Shipping,
                            item.Available,
                            item.IsVendor,
                            item.DefaultImage
                        }),
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                       
                }
                // Commit transaction if all batches are ok
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback on any failure
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Performs bulk insert or update operations for inventory data
        /// </summary>
        /// <param name="inventoryCsvModel">Collection of product records from cvs</param>
        /// <returns>Task as async operation</returns>
        public async Task BulkInsertOrUpdateInventoryAsync(IEnumerable<InventoryCsvModel> inventoryCsvModel)
        {
            // Creates and amanages sql connection
            // Asnychronous resource cleanup by await using
            await using var connection = new SqlConnection(_connectionString);
            
            // Opens data base connection asnychronously
            await connection.OpenAsync();

            // Begins transaction
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Process data in batches
                foreach (var batch in inventoryCsvModel.Chunk(10000))
                {
                    // Execute stored procedure for each batch
                    await connection.ExecuteAsync("dbo.usp_UpsertInventory", batch.Select(item => new
                    {
                        item.ProductID,
                        item.SKU,
                        item.StockQuantity,
                        item.Shipping
                    }),
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);

                }
                // Commit transaction if all batches are ok
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback on any failure
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Performs bulk insert or update operations for inventory data
        /// </summary>
        /// <param name="pricesCsvModel">Collection of product records from cvs</param>
        /// <returns>Task as async operation</returns>
        public async Task BulkInsertOrUpdatePricesAsync(IEnumerable<PricesCsvModel> pricesCsvModel)
        {
            // Creates and amanages sql connection
            // Asnychronous resource cleanup by await using
            await using var connection = new SqlConnection(_connectionString);

            // Opens data base connection asnychronously
            await connection.OpenAsync();

            // Begins transaction
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Process data in batches
                foreach (var batch in pricesCsvModel.Chunk(10000))
                {
                    // Execute stored procedure for each batch
                    await connection.ExecuteAsync("dbo.usp_UpsertPrices", batch.Select(item => new
                    {
                        item.ProductID,
                        item.SKU,
                        item.DiscountNetPrice,
                        item.LogisticDiscountNetPrice,
                    }),
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);

                }
                // Commit transaction if all batches are ok
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback on any failure
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}