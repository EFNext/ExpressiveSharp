using ExpressiveSharp.EntityFrameworkCore.Relational.Tests.Models;
using ExpressiveSharp.EntityFrameworkCore.Relational.WindowFunctions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Tests;

[TestClass]
public class WindowFunctionTests
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

    private void SeedTestData(WindowTestDbContext ctx)
    {
        ctx.Customers.AddRange(
            new Customer { Id = 1, Name = "Alice" },
            new Customer { Id = 2, Name = "Bob" });
        ctx.SaveChanges();

        // Prices: 10, 20, 20, 30, 50 (note duplicates for tie-testing)
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

    // ── SQL translation tests ──────────────────────────────────────────
    // Assert exact SQL output so the test is self-documenting and any
    // change produces a clear diff.

    private static void AssertSql(string expected, string actual)
    {
        // Normalize whitespace for comparison — EF Core may use \r\n or \n
        var normalizedExpected = expected.ReplaceLineEndings("\n").Trim();
        var normalizedActual = actual.ReplaceLineEndings("\n").Trim();
        Assert.AreEqual(normalizedExpected, normalizedActual);
    }

    [TestMethod]
    public void RowNumber_WithOrderBy_GeneratesCorrectSql()
    {
        using var ctx = CreateContext();

        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price))
        }).ToQueryString();

        AssertSql("""
            SELECT "o"."Id", ROW_NUMBER() OVER(ORDER BY "o"."Price") AS "RowNum"
            FROM "Orders" AS "o"
            """, sql);
    }

    [TestMethod]
    public void Rank_WithPartitionAndOrder_GeneratesCorrectSql()
    {
        using var ctx = CreateContext();

        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            PriceRank = WindowFunction.Rank(
                Window.PartitionBy(o.CustomerId).OrderByDescending(o.Price))
        }).ToQueryString();

        AssertSql("""
            SELECT "o"."Id", RANK() OVER(PARTITION BY "o"."CustomerId" ORDER BY "o"."Price" DESC) AS "PriceRank"
            FROM "Orders" AS "o"
            """, sql);
    }

    [TestMethod]
    public void DenseRank_GeneratesCorrectSql()
    {
        using var ctx = CreateContext();

        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price))
        }).ToQueryString();

        AssertSql("""
            SELECT "o"."Id", DENSE_RANK() OVER(ORDER BY "o"."Price" ASC) AS "Dense"
            FROM "Orders" AS "o"
            """, sql);
    }

    [TestMethod]
    public void Ntile_GeneratesCorrectSql()
    {
        using var ctx = CreateContext();

        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            Quartile = WindowFunction.Ntile(4, Window.OrderBy(o.Price))
        }).ToQueryString();

        AssertSql("""
            SELECT "o"."Id", NTILE(4) OVER(ORDER BY "o"."Price" ASC) AS "Quartile"
            FROM "Orders" AS "o"
            """, sql);
    }

    [TestMethod]
    public void MultipleWindowFunctions_InSameSelect_GeneratesCorrectSql()
    {
        using var ctx = CreateContext();

        var sql = ctx.Orders.Select(o => new
        {
            o.Id,
            RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price)),
            PriceRank = WindowFunction.Rank(Window.OrderBy(o.Price)),
            Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price))
        }).ToQueryString();

        AssertSql("""
            SELECT "o"."Id", ROW_NUMBER() OVER(ORDER BY "o"."Price") AS "RowNum", RANK() OVER(ORDER BY "o"."Price" ASC) AS "PriceRank", DENSE_RANK() OVER(ORDER BY "o"."Price" ASC) AS "Dense"
            FROM "Orders" AS "o"
            """, sql);
    }

    // ── Integration tests (seed data, execute, verify values) ────────

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
        // ROW_NUMBER should assign 1-10 ordered by price
        for (var i = 0; i < results.Count; i++)
        {
            Assert.AreEqual(i + 1, results[i].RowNum,
                $"Expected RowNum {i + 1} for result at index {i}, got {results[i].RowNum}");
        }

        // Verify ordering: prices should be non-decreasing
        for (var i = 1; i < results.Count; i++)
        {
            Assert.IsTrue(results[i].Price >= results[i - 1].Price,
                $"Expected non-decreasing prices, but {results[i].Price} < {results[i - 1].Price}");
        }
    }

    [TestMethod]
    public async Task Rank_WithTies_ReturnsCorrectRanks()
    {
        using var ctx = CreateContext();
        // Seed just a few orders with known tie prices
        ctx.Customers.Add(new Customer { Id = 1, Name = "Alice" });
        ctx.SaveChanges();
        ctx.Orders.AddRange(
            new Order { Id = 1, Price = 10, Quantity = 1, CustomerId = 1 },
            new Order { Id = 2, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 3, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 4, Price = 30, Quantity = 1, CustomerId = 1 });
        ctx.SaveChanges();

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
    public async Task DenseRank_WithTies_ReturnsConsecutiveRanks()
    {
        using var ctx = CreateContext();
        ctx.Customers.Add(new Customer { Id = 1, Name = "Alice" });
        ctx.SaveChanges();
        ctx.Orders.AddRange(
            new Order { Id = 1, Price = 10, Quantity = 1, CustomerId = 1 },
            new Order { Id = 2, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 3, Price = 20, Quantity = 1, CustomerId = 1 },
            new Order { Id = 4, Price = 30, Quantity = 1, CustomerId = 1 });
        ctx.SaveChanges();

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
        // NTILE(4) with 10 rows: buckets of 3,3,2,2
        Assert.IsTrue(results.All(r => r.Quartile >= 1 && r.Quartile <= 4),
            "All quartile values should be between 1 and 4");
        // Count per bucket
        var bucketCounts = results.GroupBy(r => r.Quartile).OrderBy(g => g.Key).Select(g => g.Count()).ToList();
        Assert.AreEqual(4, bucketCounts.Count, "Should have exactly 4 buckets");
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

        // Each customer partition should have row numbers starting from 1
        var customer1Rows = results.Where(r => r.CustomerId == 1).ToList();
        var customer2Rows = results.Where(r => r.CustomerId == 2).ToList();

        Assert.AreEqual(1, customer1Rows.First().RowNum, "Customer 1 should start at 1");
        Assert.AreEqual(1, customer2Rows.First().RowNum, "Customer 2 should start at 1");

        // Row numbers should be sequential within each partition
        for (var i = 0; i < customer1Rows.Count; i++)
            Assert.AreEqual(i + 1, customer1Rows[i].RowNum);
        for (var i = 0; i < customer2Rows.Count; i++)
            Assert.AreEqual(i + 1, customer2Rows[i].RowNum);
    }

    // ── Indexed Select tests ─────────────────────────────────────────

    [TestMethod]
    public void IndexedSelect_GeneratesRowNumberSql()
    {
        using var ctx = CreateContext();

        var sql = ctx.ExpressiveOrders
            .Select((o, index) => new { o.Id, Position = index })
            .ToQueryString();

        AssertSql("""
            SELECT "o"."Id", CAST(ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) - 1 AS INTEGER) AS "Position"
            FROM "Orders" AS "o"
            """, sql);
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
        // All positions should be 0, 1, or 2 (0-based)
        var positions = results.Select(r => r.Position).OrderBy(p => p).ToList();
        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, positions);
    }
}
