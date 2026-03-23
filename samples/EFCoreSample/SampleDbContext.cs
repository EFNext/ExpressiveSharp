using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SampleDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => Set<Order>().AsExpressiveDbSet();
    public ExpressiveDbSet<Customer> Customers => Set<Customer>().AsExpressiveDbSet();

    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

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
