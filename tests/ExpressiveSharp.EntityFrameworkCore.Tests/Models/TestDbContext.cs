using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests.Models;

public class TestDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => Set<Order>().AsExpressiveDbSet();
    public DbSet<Customer> Customers => Set<Customer>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
