using System.ComponentModel;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using MyApi.Helpers;

namespace MyApi.Models
{
    public class InventoryCsvModel
    {
  
        public string ProductID { get; set; }
        public string SKU { get; set; }
        public decimal StockQuantity { get; set; }
        public string Shipping { get; set; }

    }

    public sealed class InventoryCsvMap : ClassMap<InventoryCsvModel>
    {
        public InventoryCsvMap()
        {
            Map(m => m.ProductID).Name("product_id");
            Map(m => m.SKU).Name("sku");
            Map(m => m.StockQuantity).Name("qty")
                .TypeConverter<CustomNumberConverter>()
                .Default(0);
            Map(m => m.Shipping).Name("shipping");
        }
    }
}
