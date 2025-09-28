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
    }
}
