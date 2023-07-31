using Microsoft.AspNetCore.Identity;

namespace TestCaseDotnet.Entities;

public class User : IdentityUser
{
    public List<Transaction> Transactions { get; set; } = new();
}
