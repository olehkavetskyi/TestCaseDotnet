using CsvHelper.Configuration.Attributes;
using TestCaseDotnet.Enums;
using TestCaseDotnet.Helpers;

namespace TestCaseDotnet.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public string ClientName { get; set; }
    [TypeConverter(typeof(CurrencyDecimalConverter))]
    public decimal Amount { get; set; }
    [Ignore]
    public string? UserId { get; set; }
}
