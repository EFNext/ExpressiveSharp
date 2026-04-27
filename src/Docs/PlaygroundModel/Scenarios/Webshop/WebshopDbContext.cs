using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

public class WebshopDbContext : DbContext
{
    public ExpressiveDbSet<Customer> Customers => this.ExpressiveSet<Customer>();
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
    public ExpressiveDbSet<LineItem> LineItems => this.ExpressiveSet<LineItem>();
    public ExpressiveDbSet<Product> Products => this.ExpressiveSet<Product>();

    public WebshopDbContext(DbContextOptions<WebshopDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>().HasKey(c => c.Id);

        b.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId);
        });

        b.Entity<LineItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.HasOne(i => i.Order).WithMany(o => o.Items).HasForeignKey(i => i.OrderId);
            e.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId);
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);
        });

        b.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.ListPrice).HasPrecision(18, 2);
        });
    }
}
