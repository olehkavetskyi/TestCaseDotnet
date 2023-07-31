using TestCaseDotnet.Data;
using TestCaseDotnet.Entities;
using TestCaseDotnet.Enums;
using TestCaseDotnet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace TestCaseDotnet.Tests.ServicesTests;

public class TransactionsServiceTests
{
    private TransactionsService CreateTransactionsService(TestCaseContext context)
    {
        var mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null, null, null, null, null, null, null, null
        );

        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

        return new TransactionsService(context, Mock.Of<ILogger<TransactionsService>>(), mockUserManager.Object, mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task GetCurrentUserIdAsync_Should_Return_Null_When_User_Is_Not_Authenticated()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestCaseContext>()
            .UseInMemoryDatabase(databaseName: "GetCurrentUserIdAsync_Database")
            .Options;

        using (var context = new TestCaseContext(options))
        {
            var service = CreateTransactionsService(context);

            // Act
            var userId = await service.GetCurrentUserIdAsync();

            // Assert
            Assert.Null(userId);
        }
    }

    [Fact]
    public void ToCsv_Should_Convert_Transactions_To_CsvFormat()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction { TransactionId = 1, Status = TransactionStatus.Completed, Type = TransactionType.Withdrawal, ClientName = "John Doe", Amount = 100.00m },
            new Transaction { TransactionId = 2, Status = TransactionStatus.Pending, Type = TransactionType.Withdrawal, ClientName = "Jane Smith", Amount = 50.00m }
        };

        var service = new TransactionsService(null, null, null, null);

        // Act
        var csvData = service.ToCsv(transactions);

        // Assert
        var expectedCsvData = "TransactionId,Status,Type,ClientName,Amount\r\n" +
                                "1,Completed,Withdrawal,John Doe,$100.00\r\n" +
                                "2,Pending,Withdrawal,Jane Smith,$50.00\r\n";
        Assert.Equal(expectedCsvData, csvData);
    }
}


