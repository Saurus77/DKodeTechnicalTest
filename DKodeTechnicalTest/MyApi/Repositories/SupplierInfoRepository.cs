using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.RegularExpressions;
using Dapper;
using MyApi.Dtos;

namespace MyApi.Repositories
{
    public class SupplierInfoRepository : ISupplierInfoRepository
    {
        private readonly string _connectionString;

        public SupplierInfoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<SupplierInfoDto>> GetSupplierInfoDtoAsync(string supplierName)
        {
            const string sql = @"
             SELECT
                 prod.ProducerName AS SupplierName,
                 prod.MainCategory AS MainCategory,
                 prod.SubCategory AS SubCategory,
                 SUM(inv.StockQuantity) AS TotalStockQuantity,
                 CAST(AVG(price.LogisticDiscountNetPrice) AS DECIMAL(12, 2)) AS AverageDiscountedPrice,
                 CAST(SUM(inv.StockQuantity * price.DiscountNetPrice) AS DECIMAL (18, 2)) AS TotalStockValue,
                 CASE
                     WHEN prod.IsVendor = 'false' THEN 'Supplier'
                     ELSE 'Warehouse'
                 END AS ShippedBy

                 FROM Products prod
                 JOIN Inventory inv ON prod.SKU = inv.SKU
                 JOIN Prices price ON prod.SKU = price.SKU
                 WHERE prod.ProducerName = @SupplierName
                 GROUP BY prod.ProducerName, prod.MainCategory, prod.SubCategory, prod.IsVendor";

            using var connection = new SqlConnection(_connectionString);
         
            await connection.OpenAsync();

            return await connection.QueryAsync<SupplierInfoDto>(sql, new { SupplierName = supplierName });
          
           
         
        }
    }
}

