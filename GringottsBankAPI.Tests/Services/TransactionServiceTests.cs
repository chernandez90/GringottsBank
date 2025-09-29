using GringottsBankAPI.DTOs;
using GringottsBankAPI.Models;
using GringottsBankAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace GringottsBankAPI.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly BankDbContext _context;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<BankDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BankDbContext(options);

            // Seed test data
            SeedDatabase();

            // Create the service with the test database context
            _transactionService = new TransactionService(_context);
        }
        private void SeedDatabase()
        {
            // Add test accounts
            _context.Accounts.AddRange(
                new Account { Id = 1, Type = "Savings", Balance = 1000m },
                new Account { Id = 2, Type = "Checking", Balance = 2500m }
            );

            // Add test transactions
            _context.Transactions.AddRange(
                new Transaction
                {
                    Id = 1,
                    AccountId = 1,
                    TransactionType = "Deposit",
                    Amount = 500m,
                    Balance = 500m,
                    TransactionDate = DateTime.UtcNow.AddDays(-5)
                },
                new Transaction
                {
                    Id = 2,
                    AccountId = 1,
                    TransactionType = "Deposit",
                    Amount = 500m,
                    Balance = 1000m,
                    TransactionDate = DateTime.UtcNow.AddDays(-2)
                }
            );

            _context.SaveChanges();
        }
        [Fact]
        public async Task GetAllTransactionsAsync_ShouldReturnAllTransactions()
        {
            // Act
            var result = await _transactionService.GetAllTransactionsAsync();

            // Assert
            var transactions = result.ToList();
            Assert.Equal(2, transactions.Count);
            Assert.Contains(transactions, t => t.Id == 1);
            Assert.Contains(transactions, t => t.Id == 2);
        }
        [Fact]
        public async Task GetTransactionByIdAsync_WithValidId_ShouldReturnTransaction()
        {
            // Act
            var result = await _transactionService.GetTransactionByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.AccountId);
            Assert.Equal("Deposit", result.TransactionType);
            Assert.Equal(500m, result.Amount);
        }
        [Fact]
        public async Task GetTransactionByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _transactionService.GetTransactionByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_ShouldReturnAccountTransactions()
        {
            // Act
            var result = await _transactionService.GetTransactionsByAccountIdAsync(1);

            // Assert
            var transactions = result.ToList();
            Assert.Equal(2, transactions.Count);
            Assert.All(transactions, t => Assert.Equal(1, t.AccountId));
        }

        [Fact]
        public async Task ProcessTransactionAsync_Deposit_ShouldIncreaseBalance()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                AccountId = 1,
                TransactionType = "Deposit",
                Amount = 500m
            };

            // Act
            var (transaction, errorMessage) = await _transactionService.ProcessTransactionAsync(transactionDto);

            // Assert
            Assert.Null(errorMessage);
            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.AccountId);
            Assert.Equal("Deposit", transaction.TransactionType);
            Assert.Equal(500m, transaction.Amount);

            // Verify account balance was updated
            var account = await _context.Accounts.FindAsync(1);
            Assert.Equal(1500m, account.Balance); // Original 1000m + 500m deposit
        }
        [Fact]
        public async Task ProcessTransactionAsync_Withdrawal_WithSufficientFunds_ShouldDecreaseBalance()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                AccountId = 1,
                TransactionType = "Withdrawal",
                Amount = 300m
            };

            // Act
            var (transaction, errorMessage) = await _transactionService.ProcessTransactionAsync(transactionDto);

            // Assert
            Assert.Null(errorMessage);
            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.AccountId);
            Assert.Equal("Withdrawal", transaction.TransactionType);
            Assert.Equal(300m, transaction.Amount);

            // Verify account balance was updated
            var account = await _context.Accounts.FindAsync(1);
            Assert.Equal(700m, account.Balance); // Original 1000m - 300m withdrawal
        }
        [Fact]
        public async Task ProcessTransactionAsync_Withdrawal_WithInsufficientFunds_ShouldReturnError()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                AccountId = 1,
                TransactionType = "Withdrawal",
                Amount = 1500m // More than the 1000m balance
            };

            // Act
            var (transaction, errorMessage) = await _transactionService.ProcessTransactionAsync(transactionDto);

            // Assert
            Assert.NotNull(errorMessage);
            Assert.Contains("Insufficient funds", errorMessage);
            Assert.Null(transaction);

            // Verify account balance was not changed
            var account = await _context.Accounts.FindAsync(1);
            Assert.Equal(1000m, account.Balance); // Original balance unchanged
        }
        [Fact]
        public async Task ProcessTransferAsync_WithSufficientFunds_ShouldTransferMoney()
        {
            // Arrange
            var transferDto = new TransferDto
            {
                FromAccountId = 2, // 2500m balance
                ToAccountId = 1,   // 1000m balance
                Amount = 500m
            };

            // Act
            var (transactions, errorMessage) = await _transactionService.ProcessTransferAsync(transferDto);

            // Assert
            Assert.Null(errorMessage);
            Assert.NotNull(transactions);
            Assert.Equal(2, transactions.Count());

            // Verify account balances were updated
            var fromAccount = await _context.Accounts.FindAsync(2);
            var toAccount = await _context.Accounts.FindAsync(1);
            Assert.Equal(2000m, fromAccount.Balance); // Original 2500m - 500m transfer
            Assert.Equal(1500m, toAccount.Balance);   // Original 1000m + 500m transfer
        }

        [Fact]
        public async Task ProcessTransferAsync_WithInsufficientFunds_ShouldReturnError()
        {
            // Arrange
            var transferDto = new TransferDto
            {
                FromAccountId = 1, // 1000m balance
                ToAccountId = 2,
                Amount = 1500m    // More than the balance
            };

            // Act
            var (transactions, errorMessage) = await _transactionService.ProcessTransferAsync(transferDto);

            // Assert
            Assert.NotNull(errorMessage);
            Assert.Contains("Insufficient funds", errorMessage);
            Assert.Empty(transactions);

            // Verify account balances were not changed
            var fromAccount = await _context.Accounts.FindAsync(1);
            var toAccount = await _context.Accounts.FindAsync(2);
            Assert.Equal(1000m, fromAccount.Balance); // Original balance unchanged
            Assert.Equal(2500m, toAccount.Balance);   // Original balance unchanged
        }
    }
}
