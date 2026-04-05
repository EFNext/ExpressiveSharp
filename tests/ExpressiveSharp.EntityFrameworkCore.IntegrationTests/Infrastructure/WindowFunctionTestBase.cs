using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests for window functions (ROW_NUMBER, RANK, DENSE_RANK, NTILE,
/// indexed Select). These exercise the
/// <see cref="RelationalExpressivePlugin"/> translators and the indexed-Select
/// rewriter end-to-end against a real database.
/// </summary>
public abstract class WindowFunctionTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public async Task SeedWindowData()
    {
        // Two customers, 10 orders with ties in the price column
        Context.Customers.AddRange(
            new Customer { Id = 1, Name = "Alice" },
            new Customer { Id = 2, Name = "Bob" });
        await Context.SaveChangesAsync();

        Context.Orders.AddRange(
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
        await Context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task RowNumber_ReturnsCorrectSequentialNumbers()
    {
        var results = await Context.Orders
            .Select(o => new
            {
                o.Id,
                o.Price,
                RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.RowNum)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        for (var i = 0; i < results.Count; i++)
        {
            Assert.AreEqual(i + 1, results[i].RowNum,
                $"Expected RowNum {i + 1} at index {i}, got {results[i].RowNum}");
        }

        for (var i = 1; i < results.Count; i++)
        {
            Assert.IsTrue(results[i].Price >= results[i - 1].Price,
                $"Expected non-decreasing prices, but {results[i].Price} < {results[i - 1].Price}");
        }
    }

    [TestMethod]
    public async Task Rank_WithTies_ReturnsGaps()
    {
        // Sorted prices: 10, 15, 20, 20, 25, 30, 35, 40, 45, 50
        // RANK:          1,  2,  3,  3,  5,  6,  7,  8,  9, 10  (gap after the tie)
        var results = await Context.Orders
            .Select(o => new
            {
                o.Price,
                PriceRank = WindowFunction.Rank(Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.PriceRank)
            .ThenBy(x => x.Price)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);

        // The two price-20 rows share rank 3
        var tiedRanks = results.Where(r => r.Price == 20).Select(r => r.PriceRank).ToList();
        Assert.AreEqual(2, tiedRanks.Count);
        Assert.AreEqual(3, tiedRanks[0]);
        Assert.AreEqual(3, tiedRanks[1]);

        // Rank 4 is skipped (gap), rank 5 goes to the next distinct price
        Assert.IsFalse(results.Any(r => r.PriceRank == 4), "Rank 4 should be skipped after a tie");
        var price25Rank = results.Single(r => r.Price == 25).PriceRank;
        Assert.AreEqual(5, price25Rank);
    }

    [TestMethod]
    public async Task DenseRank_WithTies_ReturnsNoGaps()
    {
        var results = await Context.Orders
            .Select(o => new
            {
                o.Price,
                Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Dense)
            .ToListAsync();

        // 9 distinct prices in the 10-row dataset (20 appears twice).
        // Highest DENSE_RANK should equal 9.
        Assert.AreEqual(9, results.Max(r => r.Dense));

        // The two price-20 rows share the same dense rank.
        var tiedDense = results.Where(r => r.Price == 20).Select(r => r.Dense).Distinct().ToList();
        Assert.AreEqual(1, tiedDense.Count);
    }

    [TestMethod]
    public async Task Ntile_DistributesIntoBuckets()
    {
        var results = await Context.Orders
            .Select(o => new
            {
                o.Id,
                Quartile = WindowFunction.Ntile(4, Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Quartile)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        Assert.IsTrue(results.All(r => r.Quartile >= 1 && r.Quartile <= 4),
            "All quartile values should be between 1 and 4");

        // NTILE(4) over 10 rows gives 3, 3, 2, 2
        var bucketCounts = results
            .GroupBy(r => r.Quartile)
            .OrderBy(g => g.Key)
            .Select(g => g.Count())
            .ToList();
        Assert.AreEqual(4, bucketCounts.Count);
        Assert.AreEqual(3, bucketCounts[0]);
        Assert.AreEqual(3, bucketCounts[1]);
        Assert.AreEqual(2, bucketCounts[2]);
        Assert.AreEqual(2, bucketCounts[3]);
    }

    [TestMethod]
    public async Task RowNumber_WithPartitionBy_ResetsPerGroup()
    {
        var results = await Context.Orders
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                o.Price,
                RowNum = WindowFunction.RowNumber(
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Price)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.RowNum)
            .ToListAsync();

        var customer1Rows = results.Where(r => r.CustomerId == 1).ToList();
        var customer2Rows = results.Where(r => r.CustomerId == 2).ToList();

        Assert.AreEqual(1, customer1Rows.First().RowNum);
        Assert.AreEqual(1, customer2Rows.First().RowNum);

        for (var i = 0; i < customer1Rows.Count; i++)
            Assert.AreEqual(i + 1, customer1Rows[i].RowNum);
        for (var i = 0; i < customer2Rows.Count; i++)
            Assert.AreEqual(i + 1, customer2Rows[i].RowNum);
    }

    [TestMethod]
    public async Task IndexedSelect_ReturnsZeroBasedIndices()
    {
        // ExpressiveDbSet allows the two-argument Select(o, index) overload
        // which the RewriteIndexedSelectToRowNumber transformer rewrites to
        // (long)ROW_NUMBER() - 1.
        var results = await Context.ExpressiveOrders
            .Select((o, index) => new { o.Id, Position = index })
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        var positions = results.Select(r => r.Position).OrderBy(p => p).ToList();
        CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, positions);
    }

    [TestMethod]
    public async Task MultipleWindowFunctions_InSameSelect()
    {
        var results = await Context.Orders
            .Select(o => new
            {
                o.Id,
                RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Price)),
                Dense = WindowFunction.DenseRank(Window.OrderBy(o.Price)),
                Quartile = WindowFunction.Ntile(4, Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.RowNum)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);
        // First row: row 1, dense 1, quartile 1
        Assert.AreEqual(1, results[0].RowNum);
        Assert.AreEqual(1, results[0].Dense);
        Assert.AreEqual(1, results[0].Quartile);
        // Last row: row 10, dense 9 (one duplicate), quartile 4
        Assert.AreEqual(10, results[9].RowNum);
        Assert.AreEqual(9, results[9].Dense);
        Assert.AreEqual(4, results[9].Quartile);
    }

    // ── Interaction: window function over an [Expressive] projection ───
    //
    // RowNumber over o.Total (which is [Expressive] => Price * Quantity).
    // The ExpressiveQueryCompiler must expand Total inside the OrderBy
    // argument of the window spec BEFORE the window translator translates
    // it to SQL.

    [TestMethod]
    public async Task RowNumber_OverExpressiveTotal_UsesExpandedExpression()
    {
        // Totals (Price * Quantity) for the seeded orders:
        //   1: 50*1 =  50       6: 40*2 =  80
        //   2: 20*2 =  40       7: 15*1 =  15
        //   3: 10*3 =  30       8: 25*3 =  75
        //   4: 30*1 =  30       9: 35*2 =  70
        //   5: 20*5 = 100      10: 45*1 =  45
        //
        // Sorted ascending: 15(7), 30(3), 30(4), 40(2), 45(10), 50(1),
        //                   70(9), 75(8), 80(6), 100(5)
        var results = await Context.Orders
            .Select(o => new
            {
                o.Id,
                Total = o.Total,
                RowNum = WindowFunction.RowNumber(Window.OrderBy(o.Total)),
            })
            .OrderBy(x => x.RowNum)
            .ToListAsync();

        Assert.AreEqual(10, results.Count);

        // First row has the smallest total (15, order 7)
        Assert.AreEqual(7, results[0].Id);
        Assert.AreEqual(15.0, results[0].Total);
        Assert.AreEqual(1, results[0].RowNum);

        // Last row has the largest total (100, order 5)
        Assert.AreEqual(5, results[9].Id);
        Assert.AreEqual(100.0, results[9].Total);
        Assert.AreEqual(10, results[9].RowNum);

        // Totals must be non-decreasing along the row-number ordering
        for (var i = 1; i < results.Count; i++)
            Assert.IsTrue(results[i].Total >= results[i - 1].Total);
    }
}
