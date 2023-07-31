using CsvHelper.Configuration.Attributes;
using TestCaseDotnet.Enums;
using TestCaseDotnet.Helpers;

namespace TestCaseDotnet.Dtos;

public class TransactionDto
{
    public int TransactionId { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public string ClientName { get; set; }
    public decimal Amount { get; set; }
}
