using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Category { get; set; }
    public string Tag { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
}

public class ExecuteUpdateTestDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public ExpressiveDbSet<Product> ExpressiveProducts => this.ExpressiveSet<Product>();

    public ExecuteUpdateTestDbContext(DbContextOptions<ExecuteUpdateTestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
