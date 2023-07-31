using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestCaseDotnet.Entities;
using System.Security.Claims;
using TestCaseDotnet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Npgsql;
using TestCaseDotnet.Services;
using TestCaseDotnet.Dtos;
using TestCaseDotnet.Enums;
using System.Globalization;

namespace TestCaseDotnet.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private TestCaseContext _context;
    private TransactionsService _transactionsService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger _logger;

    public TransactionsController(TestCaseContext context, 
        UserManager<User> userManager, 
        ILogger<TransactionsController> logger, 
        TransactionsService transactionsService)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _transactionsService = transactionsService;
    }

    [HttpPost]
    public async Task<ActionResult> Import(IFormFile file)
    {
        if (file == null)
        {
            return BadRequest("No file or empty file provided.");
        }

        List<Transaction> transactions = new List<Transaction>();

        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            while (csv.Read())
            {
                try
                {
                    var transaction = csv.GetRecord<Transaction>();

                    transaction.UserId =  _userManager.FindByEmailAsync(User?.Identity?.Name)?.Result?.Id;

                    transactions.Add(transaction);
                }
                catch (CsvHelperException ex)
                {
                    throw;
                }
            }
        }

        await _context.AddRangeAsync(transactions);

        await _context.SaveChangesAsync();

        return Ok("Transactions imported successfully.");
    }

    [HttpPost("filter")]
    public async Task<ActionResult> FilterTransactions(Specification spec)
    {
        List<Transaction> filteredTransactions = await _transactionsService.Filter(spec);
        List<TransactionDto> transactionDtos = new();

        foreach (var transaction in filteredTransactions)
        {
            TransactionDto transactionDto = new();

            transactionDto.Amount = transaction.Amount;
            transactionDto.TransactionId = transaction.TransactionId;
            transactionDto.Status = transaction.Status;
            transactionDto.ClientName = transaction.ClientName;
            transactionDto.Type = transaction.Type;
            
            transactionDtos.Add(transactionDto);
        }


        return Ok(transactionDtos);
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateValue(int id, TransactionStatus status)
    {
        Transaction existingTransaction = _context.Transactions.Find(id);

        if (existingTransaction == null)
        {
            return NotFound();
        }
        
        existingTransaction.Status = status;

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("export")]
    public async Task<ActionResult> ExportCsv(Specification spec)
    {
        List<Transaction> filteredTransactions = await _transactionsService.Filter(spec);

        string csvData = _transactionsService.ToCsv(filteredTransactions);

        byte[] fileBytes = Encoding.UTF8.GetBytes(csvData);

        return new FileContentResult(fileBytes, "text/csv")
        {
            FileDownloadName = "transactions.csv"
        };
    }
}
