using ExpressiveSharp.EntityFrameworkCore.Tests.Models;
using ExpressiveSharp.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests;

/// <summary>
/// Integration tests that exercise <see cref="IRewritableQueryable{T}"/> delegate-based stubs
/// against a real EF Core SQLite provider — async terminal methods, multi-lambda operators,
/// chain continuity, and null-conditional syntax.
/// </summary>
[TestClass]
public class RewritableQueryableAsyncTests
{
    private SqliteConnection _connection = null!;

    [TestInitialize]
    public async Task Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var ctx = CreateContext();
        ctx.Customers.AddRange(
            new Customer { Id = 1, Name = "Alice" },
            new Customer { Id = 2, Name = "Bob" },
            new Customer { Id = 3, Name = null });
        ctx.Orders.AddRange(
            new Order { Id = 1, Price = 120, Quantity = 2, CustomerId = 1, Status = OrderStatus.Approved },
            new Order { Id = 2, Price = 75, Quantity = 20, CustomerId = 2, Status = OrderStatus.Pending },
            new Order { Id = 3, Price = 10, Quantity = 3, CustomerId = null, Status = OrderStatus.Pending },
            new Order { Id = 4, Price = 50, Quantity = 5, CustomerId = 3, Status = OrderStatus.Approved });
        await ctx.SaveChangesAsync();
    }

    [TestCleanup]
    public void Cleanup() => _connection.Dispose();

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

    // ── 1. Async terminal methods with predicates ────────────────────────

    [TestMethod]
    public async Task AnyAsync_WithPredicate_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.AnyAsync(o => o.Price > 100);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task AllAsync_WithPredicate_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.AllAsync(o => o.Price > 0);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CountAsync_WithPredicate_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public async Task FirstAsync_WithPredicate_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.FirstAsync(o => o.Price > 100);
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_WithPredicate_ReturnsNullWhenNoMatch()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.FirstOrDefaultAsync(o => o.Price > 9999);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SingleAsync_WithPredicate_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.SingleAsync(o => o.Id == 2);
        Assert.AreEqual(75, result.Price);
    }

    // ── 2. Async aggregation with selectors ──────────────────────────────

    [TestMethod]
    public async Task SumAsync_WithSelector_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.SumAsync(o => o.Quantity);
        Assert.AreEqual(30, result); // 2 + 20 + 3 + 5
    }

    [TestMethod]
    public async Task MinAsync_WithSelector_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.MinAsync(o => o.Price);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public async Task MaxAsync_WithSelector_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders.MaxAsync(o => o.Price);
        Assert.AreEqual(120, result);
    }

    // ── 3. Chain continuity + async execution ────────────────────────────

    [TestMethod]
    public async Task AsNoTracking_Where_ToListAsync_ExecutesCorrectly()
    {
        using var ctx = CreateContext();
        var results = await ctx.Orders
            .AsNoTracking()
            .Where(o => o.Price > 50)
            .ToListAsync();

        Assert.AreEqual(2, results.Count); // 120 and 75
    }

    [TestMethod]
    public async Task Include_Where_ToListAsync_LoadsNavigation()
    {
        using var ctx = CreateContext();
        var results = await ctx.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId != null)
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        Assert.IsTrue(results.All(o => o.Customer != null));
    }

    [TestMethod]
    public async Task TagWith_Where_FirstAsync_ExecutesCorrectly()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders
            .TagWith("integration test")
            .FirstAsync(o => o.Id == 1);

        Assert.AreEqual(120, result.Price);
    }

    // ── 4. Multi-lambda methods ──────────────────────────────────────────

    [TestMethod]
    public async Task GroupBy_WithElementSelector_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var results = await ctx.Orders
            .GroupBy(o => o.Status, o => o.Price)
            .Select(g => new { Status = g.Key, Total = g.Sum() })
            .ToListAsync();

        Assert.AreEqual(2, results.Count); // Approved and Pending
    }

    [TestMethod]
    public async Task Join_ExecutesAgainstEfCore()
    {
        using var ctx = CreateContext();
        var results = await ctx.Orders
            .Join(ctx.Customers,
                  o => o.CustomerId,
                  c => c.Id,
                  (o, c) => c.Name)
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        CollectionAssert.Contains(results, "Alice");
        CollectionAssert.Contains(results, "Bob");
    }

    // ── 5. Passthrough chain-continuity ─────────────────────────────────

    [TestMethod]
    public async Task Take_Skip_Where_ToListAsync_ExecutesCorrectly()
    {
        using var ctx = CreateContext();
        var results = await ctx.Orders
            .OrderBy(o => o.Id)
            .Take(3)
            .Skip(1)
            .ToListAsync();

        Assert.AreEqual(2, results.Count);
    }

    // ── 7. Null-conditional in async context ─────────────────────────────

    [TestMethod]
    public async Task NullConditional_InWhere_ToListAsync_ExecutesCorrectly()
    {
        using var ctx = CreateContext();
        var results = await ctx.Orders
            .Where(o => o.Customer?.Name == "Alice")
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task NullConditional_InFirstAsync_Predicate_ExecutesCorrectly()
    {
        using var ctx = CreateContext();
        var result = await ctx.Orders
            .FirstOrDefaultAsync(o => o.Customer?.Name == "Bob");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result!.Id);
    }
}
