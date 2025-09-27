namespace GringottsBankAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountHolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
