namespace GringottsBankAPI.DTOs
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountType { get; set; } = string.Empty; // Include relevant account info directly
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; } = 0;
        public DateTime TransactionDate { get; set; }
    }
}
