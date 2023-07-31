using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;
using TestCaseDotnet.Data;
using TestCaseDotnet.Entities;

namespace TestCaseDotnet.Services;

public class TransactionsService
{
    private TestCaseContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TransactionsService(TestCaseContext context,
        ILogger<TransactionsService> logger,
        UserManager<User> userManager, 
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetCurrentUserIdAsync()
    {
        var currentUser = _httpContextAccessor.HttpContext?.User;
        if (currentUser != null && currentUser.Identity.IsAuthenticated)
        {
            var user = await _userManager.FindByEmailAsync(currentUser.Identity.Name);
            return user?.Id;
        }

        return null;
    }

    public string ToCsv(List<Transaction> transactions)
    {
        StringBuilder csvData = new StringBuilder();

        csvData.AppendLine("TransactionId,Status,Type,ClientName,Amount");

        foreach (Transaction transaction in transactions)
        {
            csvData.AppendLine($"{transaction.TransactionId},{transaction.Status},{transaction.Type},{transaction.ClientName},${transaction.Amount}");
        }

        return csvData.ToString();
    }

    public async Task<List<Transaction>> Filter(Specification spec)
    {
        string sqlQuery = "SELECT * FROM \"Transactions\" WHERE 1=1";
        List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

        if (spec.TransactionTypes != null && spec.TransactionTypes.Count > 0)
        {
            string typeConditions = string.Join(",", spec.TransactionTypes.Select(t => (int)t));
            sqlQuery += $" AND \"Type\" IN ({typeConditions})";
        }

        if (!string.IsNullOrEmpty(spec.ClientName))
        {
            sqlQuery += " AND \"ClientName\" = @clientName";
            parameters.Add(new NpgsqlParameter("clientName", spec.ClientName));
        }

        if (spec.TransactionStatus.HasValue)
        {
            sqlQuery += " AND \"Status\" = @status";
            parameters.Add(new NpgsqlParameter("status", (int)spec.TransactionStatus.Value));
        }

        sqlQuery += " AND \"UserId\" = @userId";

        parameters.Add(new NpgsqlParameter("userId", await GetCurrentUserIdAsync()));

        return _context.Transactions.FromSqlRaw(sqlQuery, parameters.ToArray()).ToList();
    }
}
