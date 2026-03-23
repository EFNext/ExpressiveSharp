using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.FunctionalTests.Models;

public class TestDbContext : DbContext
{
    public DbSet<TestOrder> Orders => Set<TestOrder>();
    public DbSet<TestCustomer> Customers => Set<TestCustomer>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
        });

        modelBuilder.Entity<TestCustomer>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
