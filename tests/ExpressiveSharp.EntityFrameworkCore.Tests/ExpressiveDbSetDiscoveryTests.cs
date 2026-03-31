using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests;

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
    public void Cleanup()
    {
        _connection.Dispose();
    }

    [TestMethod]
    public void ExpressiveDbSet_EntityIsDiscoveredInModel()
    {
        var options = new DbContextOptionsBuilder<ExpressiveOnlyContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        using var ctx = new ExpressiveOnlyContext(options);
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem));
        Assert.IsNotNull(entityType,
            "Entity type should be discovered from ExpressiveDbSet property");
    }

    [TestMethod]
    public void ExpressiveDbSet_TableNameMatchesPropertyName()
    {
        var options = new DbContextOptionsBuilder<ExpressiveOnlyContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        using var ctx = new ExpressiveOnlyContext(options);
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem))!;
        var tableName = entityType.GetTableName();
        Assert.AreEqual("Items", tableName,
            "Table name should match the ExpressiveDbSet property name");
    }

    [TestMethod]
    public void ExpressiveDbSet_QueryExecutesSuccessfully()
    {
        var options = new DbContextOptionsBuilder<ExpressiveOnlyContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        using var ctx = new ExpressiveOnlyContext(options);
        ctx.Database.EnsureCreated();

        ctx.Items.Add(new DiscoveryItem { Name = "Widget" });
        ctx.Items.Add(new DiscoveryItem { Name = "Gadget" });
        ctx.SaveChanges();

        var items = ctx.Items.Where(i => i.Name == "Widget").ToList();
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("Widget", items[0].Name);
    }

    [TestMethod]
    public void ExpressiveDbSet_ReversedOrder_EntityIsDiscovered()
    {
        var options = new DbContextOptionsBuilder<ExpressiveOnlyContext>()
            .UseExpressives()
            .UseSqlite(_connection)
            .Options;
        using var ctx = new ExpressiveOnlyContext(options);
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem));
        Assert.IsNotNull(entityType,
            "Entity type should be discovered (UseExpressives before UseSqlite)");
    }

    [TestMethod]
    public void MixedContext_DbSetTableNameWins()
    {
        var options = new DbContextOptionsBuilder<MixedDbSetContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        using var ctx = new MixedDbSetContext(options);
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem))!;
        var tableName = entityType.GetTableName();
        Assert.AreEqual("Items", tableName,
            "When both DbSet and ExpressiveDbSet exist, the DbSet table name should be used");
    }

    [TestMethod]
    public void ExpressiveDbSet_ToQueryStringContainsCorrectTableName()
    {
        var options = new DbContextOptionsBuilder<ExpressiveOnlyContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        using var ctx = new ExpressiveOnlyContext(options);
        ctx.Database.EnsureCreated();

        var sql = ctx.Items.Where(i => i.Name == "test").ToQueryString();
        Assert.IsTrue(sql.Contains("Items", StringComparison.OrdinalIgnoreCase),
            $"Expected 'Items' table name in SQL, got: {sql}");
    }
}

// ── Test models (top-level to avoid nested-type issues) ──────────────

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
