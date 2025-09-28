namespace GringottsBankAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // e.g., "Deposit", "Withdrawal"
        public decimal Amount { get; set; }
        public decimal Balance { get; set; } = 0;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public Account? Account { get; set; }
    }
}
