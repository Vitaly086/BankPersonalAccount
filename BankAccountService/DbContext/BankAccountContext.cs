using BankAccountService.Entity;
using Microsoft.EntityFrameworkCore;

namespace BankAccountService.DbContext;

public class BankAccountContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Account> Accounts { get; set; }

    public BankAccountContext(DbContextOptions<BankAccountContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Accounts)
            .WithOne(a => a.Client)
            .IsRequired()
            .HasForeignKey(a => a.ClientId);
    }
}