using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

/// <summary>
/// Verifies the <see cref="ExpressiveDbSetDiscoveryConvention"/> and
/// <see cref="ExpressiveDbSetTableNameConvention"/> against real schema
/// creation and query execution. These are convention-plugin tests that
/// don't depend on provider-specific SQL, so they run on SQLite only.
/// </summary>
[TestClass]
public class ExpressiveDbSetDiscoveryTests
{
    private SqliteConnection _connection = null!;

    [TestInitialize]
    public void Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    [TestCleanup]
    public void Cleanup() => _connection.Dispose();

    private DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;

    private DbContextOptions<TContext> CreateOptionsReversed<TContext>() where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseExpressives()
            .UseSqlite(_connection)
            .Options;

    [TestMethod]
    public void ExpressiveOnlyContext_EntityIsDiscoveredInModel()
    {
        using var ctx = new ExpressiveOnlyContext(CreateOptions<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem));
        Assert.IsNotNull(entityType,
            "Entity type should be discovered from ExpressiveDbSet property");
    }

    [TestMethod]
    public void ExpressiveOnlyContext_TableNameMatchesPropertyName()
    {
        using var ctx = new ExpressiveOnlyContext(CreateOptions<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        var tableName = ctx.Model.FindEntityType(typeof(DiscoveryItem))!.GetTableName();
        Assert.AreEqual("Items", tableName);
    }

    [TestMethod]
    public async Task ExpressiveOnlyContext_QueryExecutesAgainstDiscoveredTable()
    {
        using var ctx = new ExpressiveOnlyContext(CreateOptions<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        ctx.Items.Add(new DiscoveryItem { Name = "Widget" });
        ctx.Items.Add(new DiscoveryItem { Name = "Gadget" });
        await ctx.SaveChangesAsync();

        var items = await ctx.Items.Where(i => i.Name == "Widget").ToListAsync();
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("Widget", items[0].Name);
    }

    [TestMethod]
    public void ExpressiveOnlyContext_UseExpressivesBeforeProvider_EntityStillDiscovered()
    {
        // Verify the deferred decoration path: UseExpressives() called before UseSqlite()
        using var ctx = new ExpressiveOnlyContext(CreateOptionsReversed<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem));
        Assert.IsNotNull(entityType);
    }

    [TestMethod]
    public void MixedContext_DbSetTableNameWinsOverExpressiveDbSet()
    {
        using var ctx = new MixedDbSetContext(CreateOptions<MixedDbSetContext>());
        ctx.Database.EnsureCreated();

        var tableName = ctx.Model.FindEntityType(typeof(DiscoveryItem))!.GetTableName();
        Assert.AreEqual("Items", tableName,
            "When both DbSet and ExpressiveDbSet exist, the DbSet table name should be used");
    }

    // ── Test-local models ───────────────────────────────────────────────

    public class DiscoveryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class ExpressiveOnlyContext : DbContext
    {
        public ExpressiveDbSet<DiscoveryItem> Items => this.ExpressiveSet<DiscoveryItem>();
        public ExpressiveOnlyContext(DbContextOptions<ExpressiveOnlyContext> options) : base(options) { }
    }

    public class MixedDbSetContext : DbContext
    {
        public DbSet<DiscoveryItem> Items => Set<DiscoveryItem>();
        public ExpressiveDbSet<DiscoveryItem> ExpressiveItems => this.ExpressiveSet<DiscoveryItem>();
        public MixedDbSetContext(DbContextOptions<MixedDbSetContext> options) : base(options) { }
    }
}
