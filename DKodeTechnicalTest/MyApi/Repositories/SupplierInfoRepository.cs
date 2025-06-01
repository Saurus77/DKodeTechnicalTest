using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.RegularExpressions;
using Dapper;
using MyApi.Dtos;

namespace MyApi.Repositories
{
    /// <summary>
    /// Class as implementation of ISupplierInfoRepository
    /// Handles data operations related to supplier info
    /// </summary>
    public class SupplierInfoRepository : ISupplierInfoRepository
    {
        // Field to store db connection string
        private readonly string _connectionString = "";

        public SupplierInfoRepository(IConfiguration configuration)
        {
            // Assigns connections string form appsettings.json to pvt field
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Retrieves supplier info from database
        /// </summary>
        /// <param name="supplierName">Name of the supplier to query</param>
        /// <returns>Collection of SupplierInfoDto objects containing supplier info data - or empty if supplier not found</returns>
        public async Task<IEnumerable<SupplierInfoDto>> GetSupplierInfoDtoAsync(string supplierName)
        {
            // Assign sql query to a field
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

            // Creates and amanages sql connection
            // Asnychronous resource cleanup by await using
            await using var connection = new SqlConnection(_connectionString);
         
            // Opens data base connection asnychronously
            await connection.OpenAsync();

            // Execute query using dapper queryasync method
            return await connection.QueryAsync<SupplierInfoDto>(sql, new { SupplierName = supplierName });
          
           
         
        }
    }
}

