using Microsoft.EntityFrameworkCore;
namespace GringottsBankAPI.Models
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Establish link between Account and Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships if needed
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = 1, Balance = 1000.00m, Type = "Savings" },
                new Account { Id = 2, Balance = 2500.50m, Type = "Checking" }
                );
        }
    }
}
