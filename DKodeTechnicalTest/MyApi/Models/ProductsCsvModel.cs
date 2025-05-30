using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace MyApi.Models
{
    public class ProductsCsvModel
    {
        public long ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string ReferenceNumber { get; set; }
        public string EAN { get; set; }
        public bool CanBeReturned { get; set; }
        public string ProducerName { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public string ChildCategory { get; set; }
        public bool IsWire { get; set; }
        public string Shipping { get; set; }
        public string PackageSize { get; set; }
        public bool Available { get; set; }
        public int LogisticHeight { get; set; }
        public int LogisticWidth { get; set; }
        public int LogisticLength { get; set; }
        public float LogisticWeight { get; set; }
        public bool IsVendor { get; set; }
        public bool AvailableInParcelLocker { get; set; }
        public string DefaultImage { get; set; }
    }

    public sealed class ProductCsvMap : ClassMap<ProductsCsvModel>
    {
        public ProductCsvMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.SKU).Name("SKU");
            Map(m => m.Name).Name("Name");
            Map(m => m.ReferenceNumber).Name("ReferenceNumber");
            Map(m => m.EAN).Name("EAN");
            Map(m => m.CanBeReturned).Name("CanBeReturned")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
            Map(m => m.ProducerName).Name("ProducerName");
            Map(m => m.MainCategory).Name("MainCategory");
            Map(m => m.SubCategory).Name("SubCategory");
            Map(m => m.ChildCategory).Name("ChildCategory");
            Map(m => m.IsWire).Name("IsWire")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
            Map(m => m.Shipping).Name("Shipping");
            Map(m => m.PackageSize).Name("PackageSize");
            Map(m => m.Available).Name("Available");


            Map(m => m.LogisticHeight).Name("LogisticHeight");
            Map(m => m.LogisticWidth).Name("LogisticWidth");
            Map(m => m.LogisticLength).Name("LogtisticLenght");
            Map(m => m.LogisticWeight).Name("LogisticWeight");
            Map(m => m.IsVendor).Name("IsVendor")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
            Map(m => m.AvailableInParcelLocker).Name("AvailableInParcelLocker")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
            Map(m => m.DefaultImage).Name("DefaultImage")
                .TypeConverterOption.BooleanValues(true, true, "true", "1")
                .TypeConverterOption.BooleanValues(false, true, "false", "0");
        }
    }
}
