using TestCaseDotnet.Enums;

namespace TestCaseDotnet.Entities;

public class Specification
{
    public List<TransactionType>? TransactionTypes { get; set; }
    public TransactionStatus? TransactionStatus { get; set; } 
    public string? ClientName { get; set; }
}
