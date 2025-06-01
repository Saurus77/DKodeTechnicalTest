using MyApi.Models;

namespace MyApi.Repositories
{
    public interface IWarehouseRepository
    {
        Task BulkInsertOrUpdateProductsAsync(IEnumerable<ProductCsvModel> productsCsvModel);
        Task BulkInsertOrUpdateInventoryAsync(IEnumerable<InventoryCsvModel> inventoryCsvModel);
        Task BulkInsertOrUpdatePricesAsync(IEnumerable<PricesCsvModel> pricesCsvModel);
    }
}
