using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class SampleDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
    public ExpressiveDbSet<Customer> Customers => this.ExpressiveSet<Customer>();

    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e => e.HasKey(c => c.Id));

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);
        });
    }
}
