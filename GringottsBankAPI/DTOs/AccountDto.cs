namespace GringottsBankAPI.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string AccountHolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Type { get; set; } = string.Empty; // e.g., Savings, Checking
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
