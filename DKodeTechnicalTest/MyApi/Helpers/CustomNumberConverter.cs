using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace MyApi.Helpers
{
    public class CustomNumberConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            text = text.Trim().Replace(".", "").Replace(",", ".");
            return decimal.Parse(text, CultureInfo.InvariantCulture);
        }

    }
}
