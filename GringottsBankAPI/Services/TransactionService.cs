using GringottsBankAPI.DTOs;
using GringottsBankAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace GringottsBankAPI.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponseDto>> GetAllTransactionsAsync();
        Task<TransactionResponseDto?> GetTransactionByIdAsync(int id);
        Task<IEnumerable<TransactionResponseDto>> GetTransactionsByAccountIdAsync(int accountId);
        Task<(TransactionResponseDto? Transaction, string? ErrorMessage)> ProcessTransactionAsync(TransactionDto transactionDto);
        Task<(IEnumerable<TransactionResponseDto> Transactions, string? ErrorMessage)> ProcessTransferAsync(TransferDto transferDto);
    }
    public class TransactionService : ITransactionService
    {
        private readonly BankDbContext _context;
        public TransactionService(BankDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TransactionResponseDto>> GetAllTransactionsAsync()
        {
            var transactions = await _context.Transactions
                            .Include(t => t.Account)
                            .ToListAsync();

            return transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                AccountType = t.Account?.Type ?? string.Empty,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                Balance = t.Balance,
                TransactionDate = t.TransactionDate
            });
        }

        public async Task<TransactionResponseDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return null;

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                AccountType = transaction.Account?.Type ?? string.Empty,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Balance = transaction.Balance,
                TransactionDate = transaction.TransactionDate
            };
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByAccountIdAsync(int accountId)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                AccountType = t.Account?.Type ?? string.Empty,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                Balance = t.Balance,
                TransactionDate = t.TransactionDate
            });
        }
        public async Task<(TransactionResponseDto? Transaction, string? ErrorMessage)> ProcessTransactionAsync(TransactionDto transactionDto)
        {
            if (transactionDto.Amount <= 0)
            {
                return (null, "Transaction amount must be greater than zero.");
            }

            // Find the account
            var account = await _context.Accounts.FindAsync(transactionDto.AccountId);
            if (account == null)
            {
                return (null, $"Account with ID {transactionDto.AccountId} not found.");
            }
            // Process transaction based on type
            decimal newBalance = account.Balance;
            switch (transactionDto.TransactionType.ToLower())
            {
                case "deposit":
                    newBalance += transactionDto.Amount;
                    break;
                case "withdrawal":
                    if (account.Balance < transactionDto.Amount)
                    {
                        return (null, "Insufficient funds for withdrawal.");
                    }
                    newBalance -= transactionDto.Amount;
                    break;
                default:
                    return (null, "Invalid transaction type. Use 'Deposit' or 'Withdrawal'.");
            }
            // Update account balance
            account.Balance = newBalance;

            // Create transaction record
            var transaction = new Transaction
            {
                AccountId = transactionDto.AccountId,
                TransactionType = transactionDto.TransactionType,
                Amount = transactionDto.Amount,
                Balance = newBalance,
                TransactionDate = DateTime.UtcNow,
                Account = account
            };

            // Save changes
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var responseDto = new TransactionResponseDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                AccountType = account.Type,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Balance = transaction.Balance,
                TransactionDate = transaction.TransactionDate
            };
            return (responseDto, null);
        }
        public async Task<(IEnumerable<TransactionResponseDto> Transactions, string? ErrorMessage)> ProcessTransferAsync(TransferDto transferDto)
        {
            if (transferDto.Amount <= 0)
            {
                return (Array.Empty<TransactionResponseDto>(), "Transfer amount must be greater than zero.");
            }

            // Validate accounts exist
            var fromAccount = await _context.Accounts.FindAsync(transferDto.FromAccountId);
            if (fromAccount == null)
            {
                return (Array.Empty<TransactionResponseDto>(), $"Source account with ID {transferDto.FromAccountId} not found.");
            }

            var toAccount = await _context.Accounts.FindAsync(transferDto.ToAccountId);
            if (toAccount == null)
            {
                return (Array.Empty<TransactionResponseDto>(), $"Destination account with ID {transferDto.ToAccountId} not found.");
            }

            // Verify sufficient funds
            if (fromAccount.Balance < transferDto.Amount)
            {
                return (Array.Empty<TransactionResponseDto>(), "Insufficient funds for transfer.");
            }

            try
            {
                // Update account balances
                fromAccount.Balance -= transferDto.Amount;
                toAccount.Balance += transferDto.Amount;

                // Create withdrawal transaction record
                var withdrawalTransaction = new Transaction
                {
                    AccountId = transferDto.FromAccountId,
                    TransactionType = "Transfer Out",
                    Amount = transferDto.Amount,
                    Balance = fromAccount.Balance,
                    TransactionDate = DateTime.UtcNow,
                    Account = fromAccount
                };

                // Create deposit transaction record
                var depositTransaction = new Transaction
                {
                    AccountId = transferDto.ToAccountId,
                    TransactionType = "Transfer In",
                    Amount = transferDto.Amount,
                    Balance = toAccount.Balance,
                    TransactionDate = DateTime.UtcNow,
                    Account = toAccount
                };

                // Add both transactions and save changes
                _context.Transactions.Add(withdrawalTransaction);
                _context.Transactions.Add(depositTransaction);
                await _context.SaveChangesAsync();

                // Create response DTOs
                var responseTransactions = new List<TransactionResponseDto>
                {
                    new TransactionResponseDto
                    {
                        Id = withdrawalTransaction.Id,
                        AccountId = withdrawalTransaction.AccountId,
                        AccountType = fromAccount.Type,
                        TransactionType = withdrawalTransaction.TransactionType,
                        Amount = withdrawalTransaction.Amount,
                        Balance = withdrawalTransaction.Balance,
                        TransactionDate = withdrawalTransaction.TransactionDate
                    },
                    new TransactionResponseDto
                    {
                        Id = depositTransaction.Id,
                        AccountId = depositTransaction.AccountId,
                        AccountType = toAccount.Type,
                        TransactionType = depositTransaction.TransactionType,
                        Amount = depositTransaction.Amount,
                        Balance = depositTransaction.Balance,
                        TransactionDate = depositTransaction.TransactionDate
                    }
                };

                return (responseTransactions, null);
            }
            catch (Exception ex)
            {
                // With in-memory DB we can't roll back, but at least we report the error
                return (Array.Empty<TransactionResponseDto>(), $"Transfer failed: {ex.Message}");
            }
        }
    }
}
