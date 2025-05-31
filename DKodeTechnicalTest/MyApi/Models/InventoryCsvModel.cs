using System.ComponentModel;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using MyApi.Helpers;

namespace MyApi.Models
{
    public class InventoryCsvModel
    {
  
        public int ProductID { get; set; }   
        public string SKU { get; set; }  
        public string Unit {  get; set; }
        public int StockQuantity { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerRefNum { get; set; }
        public string Shipping { get; set; }
        public float ShippingCost { get; set; }

    }

    public sealed class InventoryCsvMap : ClassMap<InventoryCsvModel>
    {
        public InventoryCsvMap()
        {
            Map(m => m.ProductID).Name("product_id");
            Map(m => m.SKU).Name("sku");
            Map(m => m.Unit).Name("unit");
            Map(m => m.StockQuantity).Name("qty").Default(0).TypeConverter<CustomNumberConverter>();
            Map(m => m.ManufacturerName).Name("manufacturer_name");
            Map(m => m.ManufacturerRefNum).Name("manufacturer_ref_num");
            Map(m => m.Shipping).Name("shipping");
            Map(m => m.ShippingCost).Name("shipping_cost").Default(0);
        }
    }
}
