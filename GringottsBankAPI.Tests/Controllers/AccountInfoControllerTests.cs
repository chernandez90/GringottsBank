using GringottsBankAPI.Controllers;
using GringottsBankAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GringottsBankAPI.Tests.Controllers
{
    public class AccountInfoControllerTests
    {
        private readonly BankDbContext _context;
        private readonly AccountInfoController _controller;

        public AccountInfoControllerTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<BankDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new BankDbContext(options);

            // Seed test data
            _context.Accounts.AddRange(
                new Account { Id = 1, Type = "Savings", Balance = 1000m },
                new Account { Id = 2, Type = "Checking", Balance = 2500m }
            );
            _context.SaveChanges();

            // Create the controller with the test database context
            _controller = new AccountInfoController(_context);
        }

        [Fact]
        public async Task GetAllAccounts_ShouldReturnAllAccounts()
        {
            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var accounts = Assert.IsAssignableFrom<IEnumerable<Account>>(okResult.Value);
            Assert.Equal(2, accounts.Count());
        }

        [Fact]
        public async Task GetAccountBalance_WithValidId_ShouldReturnAccount()
        {
            // Act
            var result = await _controller.GetAccountBalance(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var account = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(1, account.Id);
            Assert.Equal("Savings", account.Type);
            Assert.Equal(1000m, account.Balance);
        }
    }
}
