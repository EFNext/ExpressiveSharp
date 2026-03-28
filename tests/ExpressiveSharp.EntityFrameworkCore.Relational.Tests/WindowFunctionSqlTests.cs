using ExpressiveSharp.EntityFrameworkCore.Relational.Tests.Infrastructure;
using ExpressiveSharp.EntityFrameworkCore.Relational.Tests.Models;
using ExpressiveSharp.EntityFrameworkCore.Relational.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using VerifyMSTest;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Tests;

/// <summary>
/// Verifies the exact SQL generated for window functions across multiple database providers.
/// Uses Verify snapshot testing — each derived class produces its own <c>.verified.txt</c> files.
/// No real database connection is required; only <c>ToQueryString()</c> is used.
/// </summary>
public abstract class WindowFunctionSqlTests : VerifyBase
{
    protected abstract WindowTestDbContext CreateContext();

    [TestMethod]
    public Task RowNumber_WithOrderBy()
    {
        using var ctx = CreateContext();
        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price))
        }).ToQueryString();
        return Verify(sql);
    }

    [TestMethod]
    public Task Rank_WithPartitionAndOrder()
    {
        using var ctx = CreateContext();
        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            PriceRank = WindowFunction.Rank(
                Window.PartitionBy(o.CustomerId).OrderByDescending(o.Price))
        }).ToQueryString();
        return Verify(sql);
    }

    [TestMethod]
    public Task DenseRank_WithOrderBy()
    {
        using var ctx = CreateContext();
        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price))
        }).ToQueryString();
        return Verify(sql);
    }

    [TestMethod]
    public Task Ntile_WithBuckets()
    {
        using var ctx = CreateContext();
        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            Quartile = WindowFunction.Ntile(4, Window.OrderBy(o.Price))
        }).ToQueryString();
        return Verify(sql);
    }

    [TestMethod]
    public Task MultipleWindowFunctions_InSameSelect()
    {
        using var ctx = CreateContext();
        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price)),
            PriceRank = WindowFunction.Rank(Window.OrderBy(o.Price)),
            Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price))
        }).ToQueryString();
        return Verify(sql);
    }

    [TestMethod]
    public Task IndexedSelect()
    {
        using var ctx = CreateContext();
        var sql = ctx.ExpressiveOrders
            .Select((o, index) => new { o.Id, Position = index })
            .ToQueryString();
        return Verify(sql);
    }
}

[TestClass]
public class WindowFunctionSqlTests_Sqlite : WindowFunctionSqlTests
{
    protected override WindowTestDbContext CreateContext() =>
        TestProvider.CreateSqliteContext();
}

[TestClass]
public class WindowFunctionSqlTests_SqlServer : WindowFunctionSqlTests
{
    protected override WindowTestDbContext CreateContext() =>
        TestProvider.CreateSqlServerContext();
}

[TestClass]
public class WindowFunctionSqlTests_Npgsql : WindowFunctionSqlTests
{
    protected override WindowTestDbContext CreateContext() =>
        TestProvider.CreateNpgsqlContext();
}
