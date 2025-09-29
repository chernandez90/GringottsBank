using GringottsBankAPI.Controllers;
using GringottsBankAPI.DTOs;
using GringottsBankAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GringottsBankAPI.Tests.Controllers
{
    public class AccountTransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly AccountTransactionsController _controller;

        public AccountTransactionsControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new AccountTransactionsController(_mockTransactionService.Object);
        }

        [Fact]
        public async Task GetAllTransactions_ShouldReturnOkResult()
        {
            // Arrange
            var expectedTransactions = new List<TransactionResponseDto>
            {
                new TransactionResponseDto
                {
                    Id = 1,
                    AccountId = 1,
                    AccountType = "Savings",
                    TransactionType = "Deposit",
                    Amount = 500m,
                    Balance = 500m,
                    TransactionDate = DateTime.UtcNow
                },
                new TransactionResponseDto
                {
                    Id = 2,
                    AccountId = 1,
                    AccountType = "Savings",
                    TransactionType = "Deposit",
                    Amount = 300m,
                    Balance = 800m,
                    TransactionDate = DateTime.UtcNow
                }
            };

            _mockTransactionService
                .Setup(service => service.GetAllTransactionsAsync())
                .ReturnsAsync(expectedTransactions);

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransactions = Assert.IsAssignableFrom<IEnumerable<TransactionResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedTransactions.Count());
        }

        [Fact]
        public async Task GetTransactionById_WithExistingId_ShouldReturnOkResult()
        {
            // Arrange
            var expectedTransaction = new TransactionResponseDto
            {
                Id = 1,
                AccountId = 1,
                AccountType = "Savings",
                TransactionType = "Deposit",
                Amount = 500m,
                Balance = 500m,
                TransactionDate = DateTime.UtcNow
            };

            _mockTransactionService
                .Setup(service => service.GetTransactionByIdAsync(1))
                .ReturnsAsync(expectedTransaction);

            // Act
            var result = await _controller.GetTransactionById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransaction = Assert.IsType<TransactionResponseDto>(okResult.Value);
            Assert.Equal(1, returnedTransaction.Id);
        }

        [Fact]
        public async Task GetTransactionById_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            _mockTransactionService
                .Setup(service => service.GetTransactionByIdAsync(999))
                .ReturnsAsync((TransactionResponseDto)null);

            // Act
            var result = await _controller.GetTransactionById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTransaction_WithValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                AccountId = 1,
                TransactionType = "Deposit",
                Amount = 500m
            };

            var transactionResponse = new TransactionResponseDto
            {
                Id = 3,
                AccountId = 1,
                AccountType = "Savings",
                TransactionType = "Deposit",
                Amount = 500m,
                Balance = 1300m,
                TransactionDate = DateTime.UtcNow
            };

            _mockTransactionService
                .Setup(service => service.ProcessTransactionAsync(It.IsAny<TransactionDto>()))
                .ReturnsAsync((transactionResponse, null));

            // Act
            var result = await _controller.CreateTransaction(transactionDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetTransactionById", createdAtActionResult.ActionName);
            var returnedTransaction = Assert.IsType<TransactionResponseDto>(createdAtActionResult.Value);
            Assert.Equal(3, returnedTransaction.Id);
        }

        [Fact]
        public async Task CreateTransaction_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                AccountId = 1,
                TransactionType = "Withdrawal",
                Amount = 5000m // Exceeds balance
            };

            _mockTransactionService
                .Setup(service => service.ProcessTransactionAsync(It.IsAny<TransactionDto>()))
                .ReturnsAsync((null, "Insufficient funds for withdrawal."));

            // Act
            var result = await _controller.CreateTransaction(transactionDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task TransferFunds_WithValidData_ShouldReturnOkResult()
        {
            // Arrange
            var transferDto = new TransferDto
            {
                FromAccountId = 2,
                ToAccountId = 1,
                Amount = 500m
            };

            var transactionResponses = new List<TransactionResponseDto>
            {
                new TransactionResponseDto
                {
                    Id = 4,
                    AccountId = 2,
                    AccountType = "Checking",
                    TransactionType = "Transfer Out",
                    Amount = 500m,
                    Balance = 2000m,
                    TransactionDate = DateTime.UtcNow
                },
                new TransactionResponseDto
                {
                    Id = 5,
                    AccountId = 1,
                    AccountType = "Savings",
                    TransactionType = "Transfer In",
                    Amount = 500m,
                    Balance = 1500m,
                    TransactionDate = DateTime.UtcNow
                }
            };

            _mockTransactionService
                .Setup(service => service.ProcessTransferAsync(It.IsAny<TransferDto>()))
                .ReturnsAsync((transactionResponses, null));

            // Act
            var result = await _controller.TransferFunds(transferDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTransactions = Assert.IsAssignableFrom<IEnumerable<TransactionResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedTransactions.Count());
        }

        [Fact]
        public async Task TransferFunds_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var transferDto = new TransferDto
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 5000m // Exceeds balance
            };

            _mockTransactionService
                .Setup(service => service.ProcessTransferAsync(It.IsAny<TransferDto>()))
                .ReturnsAsync((new List<TransactionResponseDto>(), "Insufficient funds for transfer."));

            // Act
            var result = await _controller.TransferFunds(transferDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
