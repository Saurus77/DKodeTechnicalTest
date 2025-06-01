using CsvHelper.Configuration;

namespace MyApi.Models
{
    public class ProductCsvModel
    {
        public string ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string EAN { get; set; }
        public string ProducerName { get; set; }


        public string FullCategory {  get; set; }
        public string? MainCategory { get; set; }
        public string? SubCategory { get; set; }
        public string? ChildCategory { get; set; }


        public bool IsWire { get; set; }
        public string Shipping { get; set; }
        public bool Available { get; set; }
        public bool IsVendor { get; set; }
        public string DefaultImage { get; set; }
    }

    public sealed class ProductCsvMap : ClassMap<ProductCsvModel>
    {
        public ProductCsvMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.SKU).Name("SKU");
            Map(m => m.Name).Name("name").Default("");
            Map(m => m.EAN).Name("EAN").Default("");
            Map(m => m.ProducerName).Name("producer_name").Default("");
            Map(m => m.FullCategory).Name("category").Default("");
            Map(m => m.IsWire).Name("is_wire")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
            Map(m => m.Shipping).Name("shipping").Default("");
            Map(m => m.Available).Name("available")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
            Map(m => m.IsVendor).Name("is_vendor");
            Map(m => m.DefaultImage).Name("default_image").Default("");
        }
    }
}
