using CsvHelper.Configuration;
using CsvHelper;

namespace TestCaseDotnet.Helpers;

public class CurrencyDecimalConverter : CsvHelper.TypeConversion.DecimalConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        // Remove currency symbols, commas, or any other non-numeric characters
        text = text.Replace("$", "").Replace(",", "");
        return base.ConvertFromString(text, row, memberMapData);
    }
}
