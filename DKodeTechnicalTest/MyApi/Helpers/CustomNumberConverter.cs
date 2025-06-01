using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace MyApi.Helpers
{
    // Class for custom type converter for handling numeric types - mostly prices
    public class CustomNumberConverter : DefaultTypeConverter
    {

        /// <summary>
        /// Converts string value from csv file to a decimal number
        /// Handles data formatting
        /// </summary>
        /// <param name="text">String value from csv file</param>
        /// <param name="row">Unused - method directly applied to necessary fields</param>
        /// <param name="memberMapData">Data mapping</param>
        /// <returns>Modified text as decimal number</returns>
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            // Return 0 if string is null
            if (string.IsNullOrWhiteSpace(text)) return 0;

            // Trim text from white spaces, change values: 1.2345,00 -> 12345,00 -> 12345.00
            text = text.Trim().Replace(".", "").Replace(",", ".");

            // Return modified text as parsed to decimal
            return decimal.Parse(text, CultureInfo.InvariantCulture);
        }

    }
}
