using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TestCaseDotnet.Controllers;
using TestCaseDotnet.Data;
using TestCaseDotnet.Entities;
using TestCaseDotnet.Enums;
using TestCaseDotnet.Services;

namespace TestCaseDotnet.Tests.ControllersTests;

public class TransactionsControllerTests
{
    private readonly TestCaseContext _context;
    private readonly TransactionsService _transactionsService;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        var options = new DbContextOptionsBuilder<TestCaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TestCaseContext(options);

        var loggerMock = new Mock<ILogger<TransactionsService>>();
        var userManagerMock = MockUserManager<User>();

        _transactionsService = new TransactionsService(_context, loggerMock.Object, userManagerMock.Object, null);

        var userManagerControllerMock = MockUserManager<User>();
        var loggerControllerMock = new Mock<ILogger<TransactionsController>>();

        _controller = new TransactionsController(_context, userManagerControllerMock.Object, loggerControllerMock.Object, _transactionsService);
    }

    private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Import_ShouldReturnBadRequest_WhenFileIsNull()
    {
        // Arrange
        var controller = new TransactionsController(_context, null, Mock.Of<ILogger<TransactionsController>>(), _transactionsService);

        // Act
        var result = await controller.Import(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file or empty file provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateValue_ShouldReturnNotFound_WhenTransactionDoesNotExist()
    {
        // Arrange
        int nonExistentTransactionId = 999;
        var statusToUpdate = TransactionStatus.Completed;
        var controller = new TransactionsController(_context, null, Mock.Of<ILogger<TransactionsController>>(), _transactionsService);

        // Act
        var result = await controller.UpdateValue(nonExistentTransactionId, statusToUpdate);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateValue_ShouldNotUpdateStatus_WhenTransactionExistsAndStatusIsSame()
    {
        // Arrange
        var testTransaction = new Transaction
        {
            TransactionId = 1,
            Amount = 100.0M,
            Status = TransactionStatus.Completed,
            ClientName = "Test Client",
            Type = TransactionType.Withdrawal
        };

        _context.Transactions.Add(testTransaction);
        _context.SaveChanges();

        var transactionIdToUpdate = 1;
        var statusToUpdate = TransactionStatus.Completed;
        var controller = new TransactionsController(_context, null, Mock.Of<ILogger<TransactionsController>>(), _transactionsService);

        // Act
        var result = await controller.UpdateValue(transactionIdToUpdate, statusToUpdate);

        // Assert
        Assert.IsType<OkResult>(result);

        var updatedTransaction = _context.Transactions.Find(transactionIdToUpdate);
        Assert.NotNull(updatedTransaction);
        Assert.Equal(testTransaction.Status, updatedTransaction.Status);
    }

}


