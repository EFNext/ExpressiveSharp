using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests for <see cref="IExpressiveQueryable{T}"/> async terminal
/// methods, multi-lambda operators, chain continuity, and null-conditional
/// syntax in delegate lambdas. These exercise the polyfill interceptor path
/// that rewrites delegate-based LINQ calls into expression trees.
/// </summary>
public abstract class AsyncQueryableTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public Task SeedStoreData() => Context.SeedStoreAsync();

    // ── Async terminal methods with predicates ─────────────────────────

    [TestMethod]
    public async Task AnyAsync_WithPredicate_Executes()
    {
        var result = await Context.Orders.AnyAsync(o => o.Price > 100);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task AllAsync_WithPredicate_Executes()
    {
        var result = await Context.Orders.AllAsync(o => o.Price > 0);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CountAsync_WithPredicate_Executes()
    {
        var result = await Context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public async Task LongCountAsync_WithPredicate_Executes()
    {
        var result = await Context.Orders.LongCountAsync(o => o.Price > 0);
        Assert.AreEqual(4L, result);
    }

    [TestMethod]
    public async Task FirstAsync_WithPredicate_Executes()
    {
        var result = await Context.Orders.FirstAsync(o => o.Price > 100);
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_WithPredicate_ReturnsNullWhenNoMatch()
    {
        var result = await Context.Orders.FirstOrDefaultAsync(o => o.Price > 9999);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SingleAsync_WithPredicate_Executes()
    {
        var result = await Context.Orders.SingleAsync(o => o.Id == 2);
        Assert.AreEqual(75, result.Price);
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_WithPredicate_ReturnsNullWhenNoMatch()
    {
        var result = await Context.Orders.SingleOrDefaultAsync(o => o.Price > 9999);
        Assert.IsNull(result);
    }

    // ── Async aggregation with selectors ───────────────────────────────

    [TestMethod]
    public async Task SumAsync_WithSelector_Executes()
    {
        var result = await Context.Orders.SumAsync(o => o.Quantity);
        Assert.AreEqual(30, result); // 2 + 20 + 3 + 5
    }

    [TestMethod]
    public async Task MinAsync_WithSelector_Executes()
    {
        var result = await Context.Orders.MinAsync(o => o.Price);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public async Task MaxAsync_WithSelector_Executes()
    {
        var result = await Context.Orders.MaxAsync(o => o.Price);
        Assert.AreEqual(120, result);
    }

    [TestMethod]
    public async Task AverageAsync_WithSelector_Executes()
    {
        var result = await Context.Orders.AverageAsync(o => o.Price);
        // (120 + 75 + 10 + 50) / 4 = 63.75
        Assert.AreEqual(63.75, result, 0.001);
    }

    // ── Chain continuity + async execution ─────────────────────────────

    [TestMethod]
    public async Task AsNoTracking_Where_ToListAsync_ExecutesCorrectly()
    {
        var results = await Context.Orders
            .AsNoTracking()
            .Where(o => o.Price > 50)
            .ToListAsync();

        Assert.AreEqual(2, results.Count); // 120 and 75
    }

    [TestMethod]
    public async Task Include_Where_ToListAsync_LoadsNavigation()
    {
        var results = await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId != null)
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        Assert.IsTrue(results.All(o => o.Customer != null));
    }

    [TestMethod]
    public async Task TagWith_Where_FirstAsync_ExecutesCorrectly()
    {
        var result = await Context.Orders
            .TagWith("integration test")
            .FirstAsync(o => o.Id == 1);

        Assert.AreEqual(120, result.Price);
    }

    // ── Multi-lambda methods ───────────────────────────────────────────

    [TestMethod]
    public async Task GroupBy_WithElementSelector_Executes()
    {
        var results = await Context.Orders
            .GroupBy(o => o.Status, o => o.Price)
            .Select(g => new { Status = g.Key, Total = g.Sum() })
            .ToListAsync();

        Assert.AreEqual(3, results.Count); // Pending, Approved, Rejected
    }

    [TestMethod]
    public async Task Join_Executes()
    {
        var results = await Context.Orders
            .Join(Context.Customers,
                  o => o.CustomerId,
                  c => c.Id,
                  (o, c) => c.Name)
            .ToListAsync();

        // 3 orders match a customer (order 3 has CustomerId = null)
        Assert.AreEqual(3, results.Count);
        CollectionAssert.Contains(results, "Alice");
        CollectionAssert.Contains(results, "Bob");
    }

    // ── Passthrough chain-continuity ───────────────────────────────────

    [TestMethod]
    public async Task Take_Skip_Where_ToListAsync_ExecutesCorrectly()
    {
        var results = await Context.Orders
            .OrderBy(o => o.Id)
            .Take(3)
            .Skip(1)
            .ToListAsync();

        Assert.AreEqual(2, results.Count);
    }

    // ── Null-conditional in async context ──────────────────────────────

    [TestMethod]
    public async Task NullConditional_InWhere_ToListAsync_ExecutesCorrectly()
    {
        var results = await Context.ExpressiveOrders
            .Where(o => o.Customer?.Name == "Alice")
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task NullConditional_InFirstOrDefaultAsync_Predicate_ExecutesCorrectly()
    {
        var result = await Context.ExpressiveOrders
            .FirstOrDefaultAsync(o => o.Customer?.Name == "Bob");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result!.Id);
    }

    // ── SelectMany / GroupJoin / DistinctBy / TakeWhile / Contains ──────
    //
    // These were previously uncovered — the polyfill interceptor rewrites
    // each delegate-based call into an expression tree before EF Core sees
    // it. If any of these break per-provider we want to know.

    [TestMethod]
    public async Task SelectMany_WithCollection_Executes()
    {
        var lineItems = await Context.Orders
            .SelectMany(o => o.Items)
            .OrderBy(li => li.Id)
            .ToListAsync();

        // Order 1 has 2 items (ids 1, 2), Order 2 has 1 (id 3), Order 3 has 0, Order 4 has 1 (id 4)
        Assert.AreEqual(4, lineItems.Count);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, lineItems.Select(li => li.Id).ToList());
    }

    [TestMethod]
    public async Task SelectMany_WithResultSelector_Executes()
    {
        var results = await Context.Orders
            .SelectMany(o => o.Items, (o, i) => new { o.Id, ProductName = i.ProductName })
            .OrderBy(x => x.Id).ThenBy(x => x.ProductName)
            .ToListAsync();

        Assert.AreEqual(4, results.Count);
        Assert.IsTrue(results.Any(r => r.Id == 1 && r.ProductName == "Widget"));
        Assert.IsTrue(results.Any(r => r.Id == 1 && r.ProductName == "Gadget"));
    }

    [TestMethod]
    public async Task GroupJoin_Executes()
    {
        var results = await Context.Customers
            .GroupJoin(
                Context.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { c.Name, Count = orders.Count() })
            .OrderBy(x => x.Name)
            .ToListAsync();

        // Alice (1 order), Bob (1), NullName (1) — orders 3 with null customer are excluded
        Assert.AreEqual(3, results.Count);
        var alice = results.Single(r => r.Name == "Alice");
        Assert.AreEqual(1, alice.Count);
    }

    // Note: DistinctBy, TakeWhile, SkipWhile are intentionally NOT tested here.
    // EF Core does not translate them to SQL (documented limitation). The
    // polyfill interceptor correctly wraps them, but EF Core throws at query
    // compilation — a round-trip through the interceptor would just verify
    // that EF Core fails as expected, which isn't useful integration coverage.

    [TestMethod]
    public async Task ContainsAsync_WithValue_Executes()
    {
        var ids = Context.Orders.Select(o => o.Id);

        var containsTwo = await ids.ContainsAsync(2);
        var containsHundred = await ids.ContainsAsync(100);

        Assert.IsTrue(containsTwo);
        Assert.IsFalse(containsHundred);
    }

    [TestMethod]
    public async Task ElementAtAsync_Executes()
    {
        var ordered = Context.Orders.OrderBy(o => o.Id);

        var second = await ordered.ElementAtAsync(1);

        Assert.AreEqual(2, second.Id);
    }

    [TestMethod]
    public async Task ElementAtOrDefaultAsync_OutOfRange_ReturnsNull()
    {
        var result = await Context.Orders.OrderBy(o => o.Id).ElementAtOrDefaultAsync(99);

        Assert.IsNull(result);
    }
}
