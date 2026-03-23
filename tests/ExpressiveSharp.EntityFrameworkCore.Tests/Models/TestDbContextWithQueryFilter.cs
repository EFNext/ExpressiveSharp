using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests.Models;

public class TestDbContextWithQueryFilter : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();

    public TestDbContextWithQueryFilter(DbContextOptions<TestDbContextWithQueryFilter> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
            // Global query filter using an [Expressive] property
            entity.HasQueryFilter(o => o.Total > 0);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
