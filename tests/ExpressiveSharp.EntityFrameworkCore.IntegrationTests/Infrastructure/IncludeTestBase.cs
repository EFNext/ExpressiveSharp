using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests for Include/ThenInclude chain continuity via
/// <see cref="IIncludableRewritableQueryable{TEntity, TProperty}"/>. Verifies
/// that navigation loading works end-to-end after <c>Include</c> on a
/// rewritable queryable.
/// </summary>
public abstract class IncludeTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public Task SeedStoreData() => Context.SeedStoreAsync();

    [TestMethod]
    public async Task Include_LoadsRelatedCustomer()
    {
        var results = await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == 1)
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.IsNotNull(results[0].Customer);
        Assert.AreEqual("Alice", results[0].Customer!.Name);
    }

    [TestMethod]
    public async Task Include_ThenInclude_LoadsTwoLevelNavigation()
    {
        var results = await Context.Orders
            .Include(o => o.Customer)
            .ThenInclude(c => c!.Address)
            .Where(o => o.CustomerId == 1)
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        var customer = results[0].Customer;
        Assert.IsNotNull(customer);
        Assert.IsNotNull(customer!.Address);
        Assert.AreEqual("New York", customer.Address!.City);
    }

    [TestMethod]
    public async Task StringInclude_LoadsNavigation()
    {
        var results = await Context.Orders
            .Include("Customer")
            .Where(o => o.CustomerId == 2)
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.IsNotNull(results[0].Customer);
        Assert.AreEqual("Bob", results[0].Customer!.Name);
    }

    [TestMethod]
    public async Task Include_ThenWhere_OnNavigationProperty_ExecutesCorrectly()
    {
        var results = await Context.Orders
            .Include(o => o.Customer)
            .Where(o => o.Customer!.Name == "Alice")
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task IgnoreQueryFilters_Executes()
    {
        // No query filters on IntegrationTestDbContext, but calling IgnoreQueryFilters
        // should still return a valid executable chain.
        var results = await Context.Orders
            .IgnoreQueryFilters()
            .ToListAsync();

        Assert.AreEqual(4, results.Count);
    }

    [TestMethod]
    public async Task AsNoTracking_Include_Executes()
    {
        var results = await Context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Where(o => o.CustomerId != null)
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        Assert.IsTrue(results.All(o => o.Customer != null));
    }

    [TestMethod]
    public async Task ChainedModifiers_Include_Execute()
    {
        var results = await Context.Orders
            .AsNoTracking()
            .TagWith("include chain test")
            .IgnoreAutoIncludes()
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == 1)
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.IsNotNull(results[0].Customer);
    }
}
