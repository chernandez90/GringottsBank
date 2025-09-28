namespace GringottsBankAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public required string Type { get; set; } // e.g., Savings, Checking
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
