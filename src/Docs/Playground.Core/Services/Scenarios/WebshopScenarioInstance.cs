using ExpressiveSharp;
using ExpressiveSharp.Docs.PlaygroundModel.Webshop;
using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;

public sealed class WebshopScenarioInstance : IScenarioInstance
{
    private readonly WebshopDbContext _sqlite;
    private WebshopDbContext? _postgres;

    public WebshopScenarioInstance()
    {
        _sqlite = new WebshopDbContext(
            new DbContextOptionsBuilder<WebshopDbContext>()
                .UseSqlite("Data Source=:memory:")
                .UseExpressives()
                // Disable EF Core's process-wide internal service provider cache.
                // Without this, failing queries in one sample can cache bad
                // compiled-query state that breaks later samples with seemingly
                // unrelated errors (e.g., "'System.Object' cannot be used for
                // return type 'System.String'").
                .EnableServiceProviderCaching(false)
                .Options);
    }

    public IWebshopQueryRoots SqliteRoots => new DbContextRoots(_sqlite);

    public IWebshopQueryRoots PostgresRoots
        => new DbContextRoots(_postgres ??= BuildPostgresContext());

    private static WebshopDbContext BuildPostgresContext() =>
        new(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseNpgsql("Host=localhost;Database=playground;Username=postgres;Password=postgres")
            .UseExpressives()
            .EnableServiceProviderCaching(false)
            .Options);

    public object QueryArgument => SqliteRoots;

    public async ValueTask DisposeAsync()
    {
        await _sqlite.DisposeAsync();
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    // Adapts a WebshopDbContext to IWebshopQueryRoots.
    private sealed class DbContextRoots : IWebshopQueryRoots
    {
        private readonly WebshopDbContext _ctx;
        public DbContextRoots(WebshopDbContext ctx) { _ctx = ctx; }
        public IExpressiveQueryable<Customer> Customers => _ctx.Customers;
        public IExpressiveQueryable<Order> Orders => _ctx.Orders;
        public IExpressiveQueryable<Product> Products => _ctx.Products;
        public IExpressiveQueryable<LineItem> LineItems => _ctx.LineItems;
    }
}
