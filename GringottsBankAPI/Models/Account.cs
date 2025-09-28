namespace GringottsBankAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; } // e.g., Savings, Checking
        
    }
}
