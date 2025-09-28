namespace GringottsBankAPI.DTOs
{
    public class TransactionDto
    {
        public int AccountId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // e.g., "Deposit", "Withdrawal"
        public decimal Amount { get; set; }
    }
}
