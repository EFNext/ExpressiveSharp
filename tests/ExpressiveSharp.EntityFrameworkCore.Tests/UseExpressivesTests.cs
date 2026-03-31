using ExpressiveSharp.EntityFrameworkCore.Tests.Models;
using ExpressiveSharp.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests;

[TestClass]
public class UseExpressivesTests
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

    private TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        var ctx = new TestDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private TestDbContext CreateContextReversedOrder()
    {
        // UseExpressives() before UseSqlite() — simulates AddSqlite optionsAction ordering
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseExpressives()
            .UseSqlite(_connection)
            .Options;
        var ctx = new TestDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private TestDbContextWithQueryFilter CreateContextWithQueryFilter()
    {
        var options = new DbContextOptionsBuilder<TestDbContextWithQueryFilter>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;
        var ctx = new TestDbContextWithQueryFilter(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private TestDbContextWithQueryFilter CreateContextWithQueryFilterReversedOrder()
    {
        var options = new DbContextOptionsBuilder<TestDbContextWithQueryFilter>()
            .UseExpressives()
            .UseSqlite(_connection)
            .Options;
        var ctx = new TestDbContextWithQueryFilter(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [TestMethod]
    public void UseExpressives_MarksExpressivePropertiesAsUnmapped()
    {
        using var ctx = CreateContext();
        var model = ctx.Model;
        var orderEntity = model.FindEntityType(typeof(Order))!;

        // [Expressive] properties should not be mapped
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.Total)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.CustomerName)));

        // Regular properties should still be mapped
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Price)));
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Quantity)));
    }

    [TestMethod]
    public void UseExpressives_AppliesNullConditionalTransformer()
    {
        using var ctx = CreateContext();

        // CustomerName has [Expressive(RemoveNullConditionalPatterns = true)]
        // The null-check ternary should be stripped by the transformer
        var query = ctx.Set<Order>().Select(o => o.CustomerName);
        var sql = query.ToQueryString();

        // Should translate without error (the transformer removes the null-check pattern)
        Assert.IsNotNull(sql);
    }

    [TestMethod]
    public void UseExpressives_AppliesFlattenBlockTransformer()
    {
        using var ctx = CreateContext();

        // GetCategory() is block-bodied — the FlattenBlockExpressions transformer
        // should inline the local variable so EF Core can translate it
        var query = ctx.Set<Order>().Select(o => o.GetCategory());
        var sql = query.ToQueryString();

        Assert.IsTrue(
            sql.Contains("WHEN", StringComparison.OrdinalIgnoreCase) ||
            sql.Contains("IIF", StringComparison.OrdinalIgnoreCase) ||
            sql.Contains("CASE", StringComparison.OrdinalIgnoreCase),
            $"Expected conditional in SQL, got: {sql}");
    }

    [TestMethod]
    public void UseExpressives_ExpandsQueryFilters()
    {
        using var ctx = CreateContextWithQueryFilter();

        // The query filter uses o.Total > 0, which references an [Expressive] property.
        // The convention should expand it so EF Core can translate it.
        var query = ctx.Orders.ToQueryString();

        // Should contain the expanded filter (Price * Quantity > 0)
        Assert.IsTrue(query.Contains("*"),
            $"Expected multiplication in query filter SQL, got: {query}");
    }

    // ── ExpressiveDbSet tests ──────────────────────────────────────────
    // TestDbContext.Orders is ExpressiveDbSet<Order> (via AsExpressiveDbSet()),
    // so delegate lambdas with ?. work directly — no .WithExpressionRewrite() needed.

    [TestMethod]
    public void ExpressiveDbSet_NullConditionalInWhere()
    {
        using var ctx = CreateContext();

        // ?. in a delegate lambda — normally CS8072 in Expression<Func<>>,
        // but ExpressiveDbSet accepts delegates and the interceptor rewrites them
        var query = ctx.Orders.Where(o => o.Customer?.Name == "Alice");
        var sql = query.ToQueryString();

        Assert.IsTrue(sql.Contains("Alice", StringComparison.OrdinalIgnoreCase),
            $"Expected 'Alice' in SQL, got: {sql}");
    }

    [TestMethod]
    public void ExpressiveDbSet_NullConditionalInSelect()
    {
        using var ctx = CreateContext();

        var query = ctx.Orders.Select(o => o.Customer?.Name);
        var sql = query.ToQueryString();

        Assert.IsTrue(sql.Contains("Name", StringComparison.OrdinalIgnoreCase),
            $"Expected 'Name' column in SQL, got: {sql}");
    }

    [TestMethod]
    public void ExpressiveDbSet_NullConditionalWithExpressiveExpansion()
    {
        using var ctx = CreateContext();

        // Combines both: ?. (polyfill interceptor) + [Expressive] property (query compiler)
        var query = ctx.Orders
            .Where(o => o.Customer?.Name != null)
            .Select(o => o.Total);
        var sql = query.ToQueryString();

        Assert.IsTrue(sql.Contains("*"), $"Expected multiplication in SQL, got: {sql}");
    }

    [TestMethod]
    public void ExpressiveDbSet_GroupByWithNullConditional()
    {
        using var ctx = CreateContext();

        var query = ctx.Orders.GroupBy(o => o.Customer?.Name);
        var sql = query.ToQueryString();

        Assert.IsTrue(sql.Contains("Name", StringComparison.OrdinalIgnoreCase),
            $"Expected 'Name' in SQL, got: {sql}");
    }

    // ── Reversed ordering tests (UseExpressives before UseSqlite) ─────

    [TestMethod]
    public void UseExpressives_BeforeProvider_MarksExpressivePropertiesAsUnmapped()
    {
        using var ctx = CreateContextReversedOrder();
        var model = ctx.Model;
        var orderEntity = model.FindEntityType(typeof(Order))!;

        Assert.IsNull(orderEntity.FindProperty(nameof(Order.Total)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.CustomerName)));
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Price)));
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Quantity)));
    }

    [TestMethod]
    public void UseExpressives_BeforeProvider_ExpandsExpressiveProperties()
    {
        using var ctx = CreateContextReversedOrder();

        var query = ctx.Set<Order>()
            .Where(o => o.Customer != null)
            .Select(o => o.Total);
        var sql = query.ToQueryString();

        Assert.IsTrue(sql.Contains("*"),
            $"Expected multiplication in SQL, got: {sql}");
    }

    [TestMethod]
    public void UseExpressives_BeforeProvider_AppliesNullConditionalTransformer()
    {
        using var ctx = CreateContextReversedOrder();

        var query = ctx.Set<Order>().Select(o => o.CustomerName);
        var sql = query.ToQueryString();

        Assert.IsNotNull(sql);
    }

    [TestMethod]
    public void UseExpressives_BeforeProvider_AppliesFlattenBlockTransformer()
    {
        using var ctx = CreateContextReversedOrder();

        var query = ctx.Set<Order>().Select(o => o.GetCategory());
        var sql = query.ToQueryString();

        Assert.IsTrue(
            sql.Contains("WHEN", StringComparison.OrdinalIgnoreCase) ||
            sql.Contains("IIF", StringComparison.OrdinalIgnoreCase) ||
            sql.Contains("CASE", StringComparison.OrdinalIgnoreCase),
            $"Expected conditional in SQL, got: {sql}");
    }

    [TestMethod]
    public void UseExpressives_BeforeProvider_ExpandsQueryFilters()
    {
        using var ctx = CreateContextWithQueryFilterReversedOrder();

        var query = ctx.Orders.ToQueryString();

        Assert.IsTrue(query.Contains("*"),
            $"Expected multiplication in query filter SQL, got: {query}");
    }
}
