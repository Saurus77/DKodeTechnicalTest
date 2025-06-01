using System.Data;
using System.Data.SqlClient;
using Dapper;
using MyApi.Models;

namespace MyApi.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly string _connectionString;

        public WarehouseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task BulkInsertOrUpdateProductsAsync(IEnumerable<ProductCsvModel> productsCsvModel)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var batch in productsCsvModel.Chunk(10000))
                {
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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task BulkInsertOrUpdateInventoryAsync(IEnumerable<InventoryCsvModel> inventoryCsvModel)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var batch in inventoryCsvModel.Chunk(10000))
                {
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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task BulkInsertOrUpdatePricesAsync(IEnumerable<PricesCsvModel> pricesCsvModel)
        {

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var batch in pricesCsvModel.Chunk(10000))
                {
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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}