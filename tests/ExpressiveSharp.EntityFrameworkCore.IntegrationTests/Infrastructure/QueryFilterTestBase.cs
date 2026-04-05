using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests for the <see cref="ExpressiveExpandQueryFiltersConvention"/>.
/// Uses a dedicated <see cref="QueryFilterTestDbContext"/> with two global
/// query filters — one expression-bodied (<c>o.Total &gt; 0</c>) and one
/// block-bodied (<c>c.HasValidEmail()</c>) — to verify that both the
/// standard convention and the <c>FlattenBlockExpressions</c> transformer
/// run during filter expansion at model finalization.
/// </summary>
public abstract class QueryFilterTestBase : EFCoreRelationalTestBase<QueryFilterTestDbContext>
{
    [TestInitialize]
    public async Task SeedQueryFilterData()
    {
        // Customers 1-2 have valid emails (pass HasValidEmail filter).
        // Customer 3 has a null email (filtered out).
        Context.Customers.AddRange(
            new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" },
            new Customer { Id = 2, Name = "Bob", Email = "bob@example.com" },
            new Customer { Id = 3, Name = "NullMail", Email = null });
        await Context.SaveChangesAsync();

        // Orders 1-3 have positive totals (pass Total > 0 filter).
        // Order 4 has zero total (filtered out).
        Context.Orders.AddRange(
            new Order { Id = 1, Price = 100, Quantity = 2, CustomerId = 1 },  // Total = 200 ✓
            new Order { Id = 2, Price = 50, Quantity = 1, CustomerId = 2 },   // Total = 50 ✓
            new Order { Id = 3, Price = 25, Quantity = 4, CustomerId = 1 },   // Total = 100 ✓
            new Order { Id = 4, Price = 0, Quantity = 0, CustomerId = 2 });   // Total = 0 ✗
        await Context.SaveChangesAsync();
    }

    // ── Expression-bodied [Expressive] filter (Order.Total > 0) ────────

    [TestMethod]
    public async Task ExpressionBodyFilter_FiltersOutZeroTotals()
    {
        var ids = await Context.Orders
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ids);
    }

    [TestMethod]
    public async Task ExpressionBodyFilter_IgnoreQueryFilters_ReturnsAll()
    {
        var count = await Context.Orders
            .IgnoreQueryFilters()
            .CountAsync();

        Assert.AreEqual(4, count);
    }

    [TestMethod]
    public async Task ExpressionBodyFilter_CountAsync_RespectsFilter()
    {
        var count = await Context.Orders.CountAsync();
        Assert.AreEqual(3, count);
    }

    // ── Block-body [Expressive] filter (Customer.HasValidEmail()) ──────

    [TestMethod]
    public async Task BlockBodyFilter_FiltersOutNullEmails()
    {
        // HasValidEmail() has an if/else that the FlattenBlockExpressions
        // transformer must inline during filter expansion. Customers 1 and 2
        // have emails, customer 3 has null.
        var ids = await Context.Customers
            .OrderBy(c => c.Id)
            .Select(c => c.Id)
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public async Task BlockBodyFilter_IgnoreQueryFilters_ReturnsAll()
    {
        var count = await Context.Customers
            .IgnoreQueryFilters()
            .CountAsync();

        Assert.AreEqual(3, count);
    }

    [TestMethod]
    public async Task BlockBodyFilter_CountAsync_RespectsFilter()
    {
        var count = await Context.Customers.CountAsync();
        Assert.AreEqual(2, count);
    }
}
