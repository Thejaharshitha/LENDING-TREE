using System.Data.Entity;

namespace Account.Models
{
    public class AccountContext : DbContext
    {
        public AccountContext() : base("name=conn")
        {
        }

        public DbSet<Register> Registers { get; set; }
        public DbSet<AgentRegister> AgentRegisters { get; set; }
        public DbSet<Loan> Loans { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Register>().Property(r => r.UserId).IsRequired();
        //}
    }
}