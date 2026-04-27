using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests;

// Two global query filters reference [Expressive] members of different shapes:
// Order has expression-bodied (Total > 0); Customer has block-bodied
// (HasValidEmail()) which exercises FlattenBlockExpressions inside filter expansion.
public class QueryFilterTestDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();

    public QueryFilterTestDbContext(DbContextOptions<QueryFilterTestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasQueryFilter(c => c.HasValidEmail());
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
            entity.HasQueryFilter(o => o.Total > 0);
        });
    }
}
