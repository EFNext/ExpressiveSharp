using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests;

/// <summary>
/// Standalone DbContext used by query filter expansion tests. Has two global
/// query filters that reference <c>[Expressive]</c> members of different
/// shapes:
/// <list type="bullet">
///   <item><c>Order</c>: expression-bodied <c>o.Total &gt; 0</c> — the
///     <see cref="ExpressiveExpandQueryFiltersConvention"/> must expand it.</item>
///   <item><c>Customer</c>: block-bodied <c>c.HasValidEmail()</c> — exercises
///     the <c>FlattenBlockExpressions</c> transformer <b>inside</b> the filter
///     expansion path.</item>
/// </list>
/// </summary>
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
            // Block-body [Expressive] in a query filter — the
            // FlattenBlockExpressions transformer must run during filter
            // expansion to inline the if/else.
            entity.HasQueryFilter(c => c.HasValidEmail());
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
            // Expression-bodied [Expressive] (Total => Price * Quantity).
            entity.HasQueryFilter(o => o.Total > 0);
        });
    }
}
