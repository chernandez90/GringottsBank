namespace GringottsBankAPI.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; } = string.Empty; // e.g., Savings, Checking
    }
}
