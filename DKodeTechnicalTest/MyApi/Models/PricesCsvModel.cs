using System.ComponentModel;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using MyApi.Helpers;

namespace MyApi.Models
{
    /// <summary>
    /// Class to represent prices table data model
    /// </summary>
    public class PricesCsvModel
    {

        public string ProductID { get; set; }
        public string SKU { get; set; }
        public decimal DiscountNetPrice { get; set; }
        public decimal LogisticDiscountNetPrice { get; set; }

    }

    /// <summary>
    /// Class to define mapping between model fields and actual csv columns
    /// </summary>
    public sealed class PricesCsvMap : ClassMap<PricesCsvModel>
    {
        public PricesCsvMap()
        {
            Map(m => m.ProductID).Index(0);
            Map(m => m.SKU).Index(1);
            Map(m => m.DiscountNetPrice)
                .TypeConverter<CustomNumberConverter>()
                .Index(2);
            Map(m => m.LogisticDiscountNetPrice)
                .TypeConverter<CustomNumberConverter>()
                .Index(3);
        }
    }
}
