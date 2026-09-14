using BankFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace BankFlow.Data;

public class BankFlowDbContext : DbContext
{
    public BankFlowDbContext(DbContextOptions<BankFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Ensure decimal precision handling if migrating to SQL Server later
        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasColumnType("decimal(18,2)");
            
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");
    }
}
