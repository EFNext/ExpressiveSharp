using System.Linq.Expressions;
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
    public void UseExpressives_ExpandsExpressivePropertyInQuery()
    {
        using var ctx = CreateContext();

        // Query using an [Expressive] property — should auto-expand without manual ExpandExpressives()
        var query = ctx.Set<Order>().Select(o => o.Total);
        var sql = query.ToQueryString();

        // The SQL should contain the arithmetic expansion (Price * Quantity), not "Total"
        Assert.IsTrue(sql.Contains("*"), $"Expected multiplication in SQL, got: {sql}");
    }

    [TestMethod]
    public void UseExpressives_ExpandsExpressiveMethodInQuery()
    {
        using var ctx = CreateContext();

        var query = ctx.Set<Order>().Select(o => o.GetGrade());
        var sql = query.ToQueryString();

        // Switch expression should expand to CASE WHEN or IIF pattern
        Assert.IsTrue(
            sql.Contains("WHEN", StringComparison.OrdinalIgnoreCase) ||
            sql.Contains("IIF", StringComparison.OrdinalIgnoreCase) ||
            sql.Contains("CASE", StringComparison.OrdinalIgnoreCase),
            $"Expected conditional in SQL, got: {sql}");
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

    [TestMethod]
    public void UseExpressives_ExpandsConstructorProjection()
    {
        using var ctx = CreateContext();

        // new OrderDto(id, desc, total) with [Expressive] constructor
        // should expand to MemberInit: new OrderDto { Id = ..., Description = ..., Total = ... }
        var query = ctx.Set<Order>().Select(o => new OrderDto(o.Id, "test", o.Total));
        var sql = query.ToQueryString();

        // Should contain the expanded Total (Price * Quantity)
        Assert.IsTrue(sql.Contains("*"), $"Expected multiplication in SQL, got: {sql}");
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
}
