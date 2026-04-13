using ExpressiveSharp.Docs.PlaygroundModel.Webshop;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.Docs.Prerenderer;

// Single shared Cosmos container; every entity lives in it and is
// discriminated by $type. Samples access each concrete type via
// `DbSet<WebshopEntity>.OfType<T>()`, so every query stays rooted at
// one entity type from Cosmos's perspective — sidestepping the
// "Root entity X already being referenced" failure.
internal sealed class WebshopCosmosDbContext : WebshopDbContext
{
    public WebshopCosmosDbContext(DbContextOptions<WebshopDbContext> options) : base(options) { }

    public DbSet<WebshopEntity> Entities => Set<WebshopEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<WebshopEntity>(e =>
        {
            e.ToContainer("webshop");
            e.HasKey(x => x.Id);
            e.HasPartitionKey(x => x.Id);
            e.HasDiscriminator<string>("$type")
                .HasValue<Customer>("Customer")
                .HasValue<Order>("Order")
                .HasValue<Product>("Product")
                .HasValue<LineItem>("LineItem");
        });

        b.Entity<Order>(e => e.Ignore(o => o.Customer));

        b.Entity<LineItem>(e =>
        {
            e.Ignore(i => i.Order);
            e.Ignore(i => i.Product);
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);
        });

        b.Entity<Product>(e => e.Property(p => p.ListPrice).HasPrecision(18, 2));
    }
}
