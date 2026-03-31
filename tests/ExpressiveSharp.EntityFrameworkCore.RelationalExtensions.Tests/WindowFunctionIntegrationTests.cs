using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests.Models;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests;

/// <summary>
/// End-to-end integration tests that seed data into SQLite, execute window function
/// queries, and verify actual result values. These require a real database connection.
/// </summary>
[TestClass]
public class WindowFunctionIntegrationTests
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

    private WindowTestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        var ctx = new WindowTestDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private WindowTestDbContext CreateContextReversedOrder()
    {
        // UseExpressives() before UseSqlite() — simulates AddSqlite optionsAction ordering
        var options = new DbContextOptionsBuilder<WindowTestDbContext>()
            .UseExpressives(o => o.UseRelationalExtensions())
            .UseSqlite(_connection)
            .Options;
        var ctx = new WindowTestDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static void SeedTestData(WindowTestDbContext ctx)
    {
        ctx.Customers.AddRange(
            new Customer { Id = 1, Name = "Alice" },
            new Customer { Id = 2, Name = "Bob" });
        ctx.SaveChanges();

        // Prices: 50, 20, 10, 30, 20, 40, 15, 25, 35, 45 (note duplicates for tie-testing)
        ctx.Orders.AddRange(
            new Order { Id = 1, Price = 50, Quantity = 1, CustomerId = 1 },
            new Order { Id = 2, Price = 20, Quantity = 2, CustomerId = 1 },
            new Order { Id = 3, Price = 10, Quantity = 3, CustomerId = 2 },
            new Order { Id = 4, Price = 30, Quantity = 1, CustomerId = 2 },
            new Order { Id = 5, Price = 20, Quantity = 5, CustomerId = 1 },
            new Order { Id = 6, Price = 40, Quantity = 2, CustomerId = 2 },
            new Order { Id = 7, Price = 15, Quantity = 1, CustomerId = 1 },
            new Order { Id = 8, Price = 25, Quantity = 3, CustomerId = 2 },
            new Order { Id = 9, Price = 35, Quantity = 2, CustomerId = 1 },
            new Order { Id = 10, Price = 45, Quantity = 1, CustomerId = 2 });
        ctx.SaveChanges();
    }

    private static void SeedTieData(WindowTestDbContext ctx)
    {
        ctx.Customers.Add(new Customer { Id = 1, Name = "Alice" });
        ctx.SaveChanges();
        ctx.Orders.AddRange(
            new Order { Id = 1, Price = 10, Quantity = 1, CustomerId = 1 },
            new Order { Id = 2, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 3, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 4, Price = 30, Quantity = 1, CustomerId = 1 });
        ctx.SaveChanges();
    }

    [TestMethod]
    public async Task RowNumber_ReturnsCorrectSequentialNumbers()
    {
        using var ctx = CreateContext();
        SeedTestData(ctx);

        var results = await ctx.Orders
            .Select(o => new
            {
                o.Id,
                o.Price,
                RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price))
            })
            .OrderBy(x => x.RowNum)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        for (var i = 0; i < results.Count; i++)
        {
            Assert.AreEqual(i + 1, results[i].RowNum,
                $"Expected RowNum {i + 1} at index {i}, got {results[i].RowNum}");
        }

        // Prices should be non-decreasing
        for (var i = 1; i < results.Count; i++)
        {
            Assert.IsTrue(results[i].Price >= results[i - 1].Price,
                $"Expected non-decreasing prices, but {results[i].Price} < {results[i - 1].Price}");
        }
    }

    [TestMethod]
    public async Task Rank_WithTies_ReturnsGaps()
    {
        using var ctx = CreateContext();
        SeedTieData(ctx);

        var results = await ctx.Orders
            .Select(o => new
            {
                o.Price,
                PriceRank = WindowFunction.Rank(Window.OrderBy(o.Price))
            })
            .OrderBy(x => x.PriceRank)
            .ToListAsync();

        // RANK with ties: 1, 2, 2, 4 (gap after ties)
        Assert.AreEqual(4, results.Count);
        Assert.AreEqual(1, results[0].PriceRank);
        Assert.AreEqual(2, results[1].PriceRank);
        Assert.AreEqual(2, results[2].PriceRank);
        Assert.AreEqual(4, results[3].PriceRank);
    }

    [TestMethod]
    public async Task DenseRank_WithTies_ReturnsNoGaps()
    {
        using var ctx = CreateContext();
        SeedTieData(ctx);

        var results = await ctx.Orders
            .Select(o => new
            {
                o.Price,
                Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price))
            })
            .OrderBy(x => x.Dense)
            .ThenBy(x => x.Price)
            .ToListAsync();

        // DENSE_RANK with ties: 1, 2, 2, 3 (no gaps)
        Assert.AreEqual(4, results.Count);
        Assert.AreEqual(1, results[0].Dense);
        Assert.AreEqual(2, results[1].Dense);
        Assert.AreEqual(2, results[2].Dense);
        Assert.AreEqual(3, results[3].Dense);
    }

    [TestMethod]
    public async Task Ntile_DistributesIntoBuckets()
    {
        using var ctx = CreateContext();
        SeedTestData(ctx);

        var results = await ctx.Orders
            .Select(o => new
            {
                o.Id,
                Quartile = WindowFunction.Ntile(4, Window.OrderBy(o.Price))
            })
            .OrderBy(x => x.Quartile)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        Assert.IsTrue(results.All(r => r.Quartile >= 1 && r.Quartile <= 4),
            "All quartile values should be between 1 and 4");

        // NTILE(4) over 10 rows gives 3, 3, 2, 2
        var bucketCounts = results.GroupBy(r => r.Quartile).OrderBy(g => g.Key).Select(g => g.Count()).ToList();
        Assert.AreEqual(4, bucketCounts.Count, "Should have exactly 4 buckets");
        Assert.AreEqual(3, bucketCounts[0], "Bucket 1 should have 3 rows");
        Assert.AreEqual(3, bucketCounts[1], "Bucket 2 should have 3 rows");
        Assert.AreEqual(2, bucketCounts[2], "Bucket 3 should have 2 rows");
        Assert.AreEqual(2, bucketCounts[3], "Bucket 4 should have 2 rows");
    }

    [TestMethod]
    public async Task RowNumber_WithPartitionBy_ResetsPerGroup()
    {
        using var ctx = CreateContext();
        SeedTestData(ctx);

        var results = await ctx.Orders
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                o.Price,
                RowNum = WindowFunction.RowNumber(
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Price))
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.RowNum)
            .ToListAsync();

        var customer1Rows = results.Where(r => r.CustomerId == 1).ToList();
        var customer2Rows = results.Where(r => r.CustomerId == 2).ToList();

        Assert.AreEqual(1, customer1Rows.First().RowNum, "Customer 1 should start at 1");
        Assert.AreEqual(1, customer2Rows.First().RowNum, "Customer 2 should start at 1");

        for (var i = 0; i < customer1Rows.Count; i++)
            Assert.AreEqual(i + 1, customer1Rows[i].RowNum);
        for (var i = 0; i < customer2Rows.Count; i++)
            Assert.AreEqual(i + 1, customer2Rows[i].RowNum);
    }

    [TestMethod]
    public async Task IndexedSelect_ReturnsZeroBasedIndices()
    {
        using var ctx = CreateContext();
        ctx.Customers.Add(new Customer { Id = 1, Name = "Alice" });
        ctx.SaveChanges();
        ctx.Orders.AddRange(
            new Order { Id = 1, Price = 10, Quantity = 1, CustomerId = 1 },
            new Order { Id = 2, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 3, Price = 30, Quantity = 1, CustomerId = 1 });
        ctx.SaveChanges();

        var results = await ctx.ExpressiveOrders
            .Select((o, index) => new { o.Id, Position = index })
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        var positions = results.Select(r => r.Position).OrderBy(p => p).ToList();
        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, positions);
    }

    // ── Reversed ordering tests (UseExpressives before UseSqlite) ─────

    [TestMethod]
    public async Task RowNumber_BeforeProvider_ReturnsCorrectSequentialNumbers()
    {
        using var ctx = CreateContextReversedOrder();
        SeedTestData(ctx);

        var results = await ctx.Orders
            .Select(o => new
            {
                o.Id,
                o.Price,
                RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price))
            })
            .OrderBy(x => x.RowNum)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        for (var i = 0; i < results.Count; i++)
        {
            Assert.AreEqual(i + 1, results[i].RowNum,
                $"Expected RowNum {i + 1} at index {i}, got {results[i].RowNum}");
        }
    }
}
