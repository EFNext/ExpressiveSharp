using ExpressiveSharp.EntityFrameworkCore.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Tests;

[TestClass]
public class RewritableQueryableEfCoreExtensionsTests
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
    public void AsNoTracking_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var source = ctx.Orders;
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(source);

        var result = source.AsNoTracking();
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void AsNoTrackingWithIdentityResolution_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.AsNoTrackingWithIdentityResolution();
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void AsTracking_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.AsTracking();
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void IgnoreAutoIncludes_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.IgnoreAutoIncludes();
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void IgnoreQueryFilters_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.IgnoreQueryFilters();
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void TagWith_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders.TagWith("test tag");
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }

    [TestMethod]
    public void ChainedEfCoreModifiers_PreservesRewritableQueryable()
    {
        using var ctx = CreateContext();
        var result = ctx.Orders
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .TagWith("chained test");
        Assert.IsInstanceOfType<IRewritableQueryable<Order>>(result);
    }
}
