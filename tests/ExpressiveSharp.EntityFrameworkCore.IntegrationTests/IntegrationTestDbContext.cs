using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests;

public class IntegrationTestDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Delegate-based DbSet for tests that exercise the polyfill interceptor
    /// (e.g. <c>?.</c>, indexed Select with WindowFunction.RowNumber).
    /// </summary>
    public ExpressiveDbSet<Order> ExpressiveOrders => Set<Order>().AsExpressiveDbSet();

    /// <summary>
    /// Delegate-based DbSet for ExecuteUpdate tests with modern C# syntax.
    /// </summary>
    public ExpressiveDbSet<Product> ExpressiveProducts => Set<Product>().AsExpressiveDbSet();

    public IntegrationTestDbContext(DbContextOptions<IntegrationTestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasOne(e => e.Address)
                .WithMany()
                .HasForeignKey(e => e.AddressId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(e => e.OrderId);
        });

        modelBuilder.Entity<LineItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}
