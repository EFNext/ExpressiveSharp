using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.EntityFrameworkCore.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests;

[TestClass]
public class IncludeThenIncludeTests
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

    [TestMethod]
    public void Include_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.Include(o => o.Customer);

        Assert.IsInstanceOfType<IIncludableRewritableQueryable<Order, Customer?>>(result);
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void StringInclude_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.Include("Customer");

        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void MultipleIncludes_PreservesChain()
    {
        using var ctx = CreateContext();

        // Second Include should resolve to our overload (IRewritableQueryable is more specific)
        var result = ctx.Orders
            .Include(o => o.Customer)
            .Include(o => o.Customer);

        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public async Task Include_ThenWhere_ExecutesCorrectly()
    {
        using var ctx = CreateContext();
        ctx.Customers.Add(new Customer { Id = 1, Name = "Alice" });
        ctx.Orders.Add(new Order { Id = 1, Price = 100, Quantity = 1, CustomerId = 1 });
        await ctx.SaveChangesAsync();

        var results = await ctx.Orders
            .Include(o => o.Customer)
            .Where(o => o.Customer!.Name == "Alice")
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.IsNotNull(results[0].Customer);
        Assert.AreEqual("Alice", results[0].Customer!.Name);
    }
}
