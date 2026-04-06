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

    // ── Aggregate window function tests ─────────────────────────────────
    //
    // Aggregate functions (SUM, AVG, COUNT, MIN, MAX) with OVER produce
    // results that depend on the frame clause — unlike ranking functions.
    // A running total (SUM with ROWS UNBOUNDED PRECEDING TO CURRENT ROW)
    // gives a different value per row than a full-partition SUM.
    //
    // Seed data (ordered by Price ASC):
    //   Price:  10, 15, 20, 20, 25, 30, 35, 40, 45, 50
    //   Running total: 10, 25, 45, 65, 90, 120, 155, 195, 240, 290

    [TestMethod]
    public async Task Sum_WithRowsFrame_ProducesRunningTotal()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                RunningTotal = WindowFunction.Sum(o.Price,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
            })
            .OrderBy(x => x.RunningTotal);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "SUM");
        StringAssert.Contains(sql, "ROWS BETWEEN");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // Running total must increase monotonically
        for (var i = 1; i < results.Count; i++)
            Assert.IsTrue(results[i].RunningTotal >= results[i - 1].RunningTotal,
                $"Running total must be non-decreasing: {results[i].RunningTotal} < {results[i - 1].RunningTotal}");

        // First row = smallest price (10), last row = sum of all (290)
        Assert.AreEqual(10.0, results[0].RunningTotal);
        Assert.AreEqual(290.0, results[^1].RunningTotal);
    }

    [TestMethod]
    public async Task Sum_WithPartitionAndFrame_ResetsPerGroup()
    {
        // Customer 1 prices (ascending): 15, 20, 20, 35, 50 → running totals: 15, 35, 55, 90, 140
        // Customer 2 prices (ascending): 10, 25, 30, 40, 45 → running totals: 10, 35, 65, 105, 150
        var query = Context.Orders
            .Select(o => new
            {
                o.CustomerId,
                o.Price,
                RunningTotal = WindowFunction.Sum(o.Price,
                    Window.PartitionBy(o.CustomerId)
                          .OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.RunningTotal);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        var c1 = results.Where(r => r.CustomerId == 1).ToList();
        var c2 = results.Where(r => r.CustomerId == 2).ToList();

        // Each partition's running total starts at the first price
        Assert.AreEqual(c1[0].Price, c1[0].RunningTotal);
        Assert.AreEqual(c2[0].Price, c2[0].RunningTotal);

        // Each partition's last running total = sum of all prices in that partition
        Assert.AreEqual(140.0, c1[^1].RunningTotal); // 15+20+20+35+50
        Assert.AreEqual(150.0, c2[^1].RunningTotal); // 10+25+30+40+45
    }

    [TestMethod]
    public async Task Average_WithSlidingWindow_ProducesMovingAverage()
    {
        // 3-row sliding window average (1 preceding, current, 1 following)
        // differs from a full-partition average
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                MovingAvg = WindowFunction.Average(o.Price,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.Preceding(1), WindowFrameBound.Following(1))),
            })
            .OrderBy(x => x.Price);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "AVG");
        StringAssert.Contains(sql, "1 PRECEDING");
        StringAssert.Contains(sql, "1 FOLLOWING");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row (price=10): avg of [10, 15] = 12.5 (only 1 following, no preceding)
        // Middle rows have 3-element windows
        // Not all values should be the same (unlike full-partition avg which would be 29.0)
        var distinctAvgs = results.Select(r => r.MovingAvg).Distinct().Count();
        Assert.IsTrue(distinctAvgs > 1, "Moving average should produce varying values per row");
    }

    [TestMethod]
    public async Task Count_Star_WithPartition_ProducesRunningCount()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                RunningCount = WindowFunction.Count(
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Id)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.Id);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "COUNT(*)");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // Running count per customer: 1, 2, 3, 4, 5 for each partition
        var c1 = results.Where(r => r.CustomerId == 1).Select(r => r.RunningCount).ToList();
        var c2 = results.Where(r => r.CustomerId == 2).Select(r => r.RunningCount).ToList();

        CollectionAssert.AreEqual(new long[] { 1, 2, 3, 4, 5 }, c1);
        CollectionAssert.AreEqual(new long[] { 1, 2, 3, 4, 5 }, c2);
    }

    [TestMethod]
    public async Task Min_WithPartition_ReturnsRunningMin()
    {
        // Running MIN per customer, ordered by Id (insertion order).
        // Customer 1 prices by Id: 50, 20, 20, 15, 35 → running min: 50, 20, 20, 15, 15
        // Customer 2 prices by Id: 10, 30, 40, 25, 45 → running min: 10, 10, 10, 10, 10
        var query = Context.Orders
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                o.Price,
                RunningMin = WindowFunction.Min(o.Price,
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Id)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.Id);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // Running min is non-increasing within each partition
        foreach (var group in results.GroupBy(r => r.CustomerId))
        {
            var mins = group.OrderBy(r => r.Id).Select(r => r.RunningMin).ToList();
            for (var i = 1; i < mins.Count; i++)
                Assert.IsTrue(mins[i] <= mins[i - 1],
                    $"Running MIN must be non-increasing: {mins[i]} > {mins[i - 1]}");
        }
    }

    [TestMethod]
    public async Task Max_WithRowsFrame_ReturnsRunningMax()
    {
        // Ordered by Price ASC → running MAX = current row's price (each new row is the max so far)
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                RunningMax = WindowFunction.Max(o.Price,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
            })
            .OrderBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // When ordered ascending, running MAX equals the current price
        for (var i = 0; i < results.Count; i++)
            Assert.AreEqual(results[i].Price, results[i].RunningMax,
                $"Running MAX should equal current price when ordered ascending, at index {i}");
    }

    [TestMethod]
    public async Task MultipleAggregates_InSameSelect()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Id,
                o.Price,
                RunningSum = WindowFunction.Sum(o.Price,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
                RunningCount = WindowFunction.Count(
                    Window.OrderBy(o.Price)),
                RunningMax = WindowFunction.Max(o.Price,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
            })
            .OrderBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // Last row: sum = 290, count = 10, max = 50
        Assert.AreEqual(290.0, results[^1].RunningSum);
        Assert.AreEqual(10, results[^1].RunningCount);
        Assert.AreEqual(50.0, results[^1].RunningMax);
    }

    // ── Navigation function tests (LAG / LEAD) ──────────────────────────
    //
    // LAG/LEAD access a row at a fixed offset from the current row.
    // They do NOT support frame clauses (SQL standard forbids it).
    //
    // Ordered by Price ASC: 10, 15, 20, 20, 25, 30, 35, 40, 45, 50

    [TestMethod]
    public async Task Lag_Default_ReturnsPreviousRowPrice()
    {
        // Cast to (double?) so SQL NULL is distinguishable from 0.0
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                PrevPrice = (double?)WindowFunction.Lag(o.Price,
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "LAG");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row has no previous → null
        Assert.IsNull(results[0].PrevPrice, "First row should have no previous value");
        // Second row's LAG = first row's price
        Assert.AreEqual(10.0, results[1].PrevPrice);
    }

    [TestMethod]
    public async Task Lag_WithOffset_ReturnsCorrectRow()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                Prev2 = (double?)WindowFunction.Lag(o.Price, 2,
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First two rows have no row 2 back → null
        Assert.IsNull(results[0].Prev2);
        Assert.IsNull(results[1].Prev2);
        // Third row (price=20) looks back 2 → price=10
        Assert.AreEqual(10.0, results[2].Prev2);
    }

    [TestMethod]
    public async Task Lag_WithDefault_ReturnsDefaultWhenNoRow()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                PrevOrZero = WindowFunction.Lag(o.Price, 1, 0.0,
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row has no previous → default (0.0)
        Assert.AreEqual(0.0, results[0].PrevOrZero);
        // Second row → previous price (10)
        Assert.AreEqual(10.0, results[1].PrevOrZero);
    }

    [TestMethod]
    public async Task Lead_Default_ReturnsNextRowPrice()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                NextPrice = (double?)WindowFunction.Lead(o.Price,
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "LEAD");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // Last row has no next → null
        Assert.IsNull(results[^1].NextPrice, "Last row should have no next value");
        // First row's LEAD = second row's price (15)
        Assert.AreEqual(15.0, results[0].NextPrice);
    }

    [TestMethod]
    public async Task Lead_WithOffsetAndDefault_ReturnsCorrectValues()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                Next2OrNeg1 = WindowFunction.Lead(o.Price, 2, -1.0,
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row (price=10) looks ahead 2 → price=20
        Assert.AreEqual(20.0, results[0].Next2OrNeg1);
        // Last two rows have no row 2 ahead → default (-1.0)
        Assert.AreEqual(-1.0, results[^1].Next2OrNeg1);
        Assert.AreEqual(-1.0, results[^2].Next2OrNeg1);
    }

    [TestMethod]
    public async Task Lag_WithPartition_ResetsPerGroup()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.CustomerId,
                o.Price,
                PrevInGroup = (double?)WindowFunction.Lag(o.Price,
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Price)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row of each customer partition has no previous → null
        var c1 = results.Where(r => r.CustomerId == 1).ToList();
        var c2 = results.Where(r => r.CustomerId == 2).ToList();

        Assert.IsNull(c1[0].PrevInGroup);
        Assert.IsNull(c2[0].PrevInGroup);
        // Second row of each partition → first row's price
        Assert.AreEqual(c1[0].Price, c1[1].PrevInGroup);
        Assert.AreEqual(c2[0].Price, c2[1].PrevInGroup);
    }

    // ── FIRST_VALUE / LAST_VALUE ─────────────────────────────────────────

    [TestMethod]
    public async Task FirstValue_ReturnsFirstPriceInPartition()
    {
        // Ordered by Price ASC: 10, 15, 20, 20, 25, 30, 35, 40, 45, 50
        // FIRST_VALUE with default frame → first row's price = 10 for every row
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                First = WindowFunction.FirstValue(o.Price,
                    Window.OrderBy(o.Price)),
            });

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "FIRST_VALUE");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);
        Assert.IsTrue(results.All(r => r.First == 10.0),
            "FIRST_VALUE should return the lowest price (10) for all rows");
    }

    [TestMethod]
    public async Task LastValue_WithUnboundedFrame_ReturnsLastPriceInPartition()
    {
        // Without an explicit frame, LAST_VALUE returns the current row (useless).
        // With ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING → true last value.
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                Last = WindowFunction.LastValue(o.Price,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.UnboundedFollowing)),
            });

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "LAST_VALUE");
        StringAssert.Contains(sql, "UNBOUNDED FOLLOWING");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);
        Assert.IsTrue(results.All(r => r.Last == 50.0),
            "LAST_VALUE with unbounded frame should return the highest price (50) for all rows");
    }

    [TestMethod]
    public async Task FirstValue_WithPartition_ReturnsFirstPerGroup()
    {
        // Customer 1 prices (ascending): 15, 20, 20, 35, 50 → first = 15
        // Customer 2 prices (ascending): 10, 25, 30, 40, 45 → first = 10
        var query = Context.Orders
            .Select(o => new
            {
                o.CustomerId,
                o.Price,
                FirstInGroup = WindowFunction.FirstValue(o.Price,
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Price)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        var c1 = results.Where(r => r.CustomerId == 1).ToList();
        var c2 = results.Where(r => r.CustomerId == 2).ToList();

        Assert.IsTrue(c1.All(r => r.FirstInGroup == 15.0));
        Assert.IsTrue(c2.All(r => r.FirstInGroup == 10.0));
    }

    // ── PERCENT_RANK ─────────────────────────────────────────────────────

    [TestMethod]
    public async Task PercentRank_ReturnsBetweenZeroAndOne()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                Pct = WindowFunction.PercentRank(
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "PERCENT_RANK");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row always has PERCENT_RANK = 0.0
        Assert.AreEqual(0.0, results[0].Pct);
        // Last row always has PERCENT_RANK = 1.0 (when no ties at the end)
        Assert.AreEqual(1.0, results[^1].Pct);
        // All values in [0.0, 1.0]
        Assert.IsTrue(results.All(r => r.Pct >= 0.0 && r.Pct <= 1.0));
    }

    // ── CUME_DIST ────────────────────────────────────────────────────────

    [TestMethod]
    public async Task CumeDist_ReturnsBetweenZeroAndOne()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                Cume = WindowFunction.CumeDist(
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "CUME_DIST");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // CUME_DIST: last row = 1.0, all values in (0.0, 1.0]
        Assert.AreEqual(1.0, results[^1].Cume);
        Assert.IsTrue(results.All(r => r.Cume > 0.0 && r.Cume <= 1.0));
        // Unlike PERCENT_RANK, first row's CUME_DIST > 0.0
        Assert.IsTrue(results[0].Cume > 0.0);
    }

    // ── NTH_VALUE ────────────────────────────────────────────────────────

    [TestMethod]
    public async Task NthValue_ReturnsValueAtPosition()
    {
        // Ordered by Price ASC: 10, 15, 20, 20, 25, 30, 35, 40, 45, 50
        // NTH_VALUE(Price, 3) with unbounded frame → 3rd value = 20 for all rows
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                Third = WindowFunction.NthValue(o.Price, 3,
                    Window.OrderBy(o.Price)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.UnboundedFollowing)),
            });

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "NTH_VALUE");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);
        Assert.IsTrue(results.All(r => r.Third == 20.0),
            "NTH_VALUE(Price, 3) should return the 3rd price (20) for all rows with unbounded frame");
    }

    // ── Coverage gap tests ───────────────────────────────────────────────

    [TestMethod]
    public async Task Count_Expression_CountsNonNullOnly()
    {
        // Tag is null for all seeded rows, so COUNT(Tag) should be 0 everywhere,
        // while COUNT(*) returns the running count. This proves COUNT(expr) only
        // counts non-null values.
        var query = Context.Orders
            .Select(o => new
            {
                o.Id,
                TagCount = WindowFunction.Count(o.Tag, Window.OrderBy(o.Id)),
                StarCount = WindowFunction.Count(Window.OrderBy(o.Id)),
            })
            .OrderBy(x => x.Id);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // COUNT(Tag) = 0 for every row (all Tags are null)
        Assert.IsTrue(results.All(r => r.TagCount == 0),
            "COUNT(Tag) should be 0 when all Tags are null");
        // COUNT(*) = running count 1..10
        Assert.AreEqual(10, results[^1].StarCount);
    }

    [TestMethod]
    public async Task Average_IntColumn_ReturnsDouble()
    {
        // Quantity is int — the int→double overload should be selected.
        // Ordered by Quantity ASC: 1,1,1,1,2,2,2,3,3,5
        var query = Context.Orders
            .Select(o => new
            {
                o.Quantity,
                AvgQty = WindowFunction.Average(o.Quantity,
                    Window.OrderBy(o.Quantity)
                          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
            })
            .OrderBy(x => x.Quantity);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First row: AVG of [1] = 1.0
        Assert.AreEqual(1.0, results[0].AvgQty);
        // Last row: AVG of all quantities = (1+1+1+1+2+2+2+3+3+5)/10 = 2.1
        Assert.AreEqual(2.1, results[^1].AvgQty, 0.01);
    }

    [TestMethod]
    public async Task Sum_WithoutFrame_UsesDefaultFrame()
    {
        // Sum with OrderedWindowDefinition only (no explicit frame).
        // SQL default frame with ORDER BY = RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW.
        // This is a running total — same behavior as the explicit frame test.
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                RunningTotal = WindowFunction.Sum(o.Price,
                    Window.OrderBy(o.Price)),
            })
            .OrderBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // First = 10, last = 290 (same as explicit ROWS UNBOUNDED PRECEDING TO CURRENT ROW)
        // Note: with RANGE default, tied prices (20, 20) get the same running total
        Assert.AreEqual(290.0, results[^1].RunningTotal);
        // Running total must be non-decreasing
        for (var i = 1; i < results.Count; i++)
            Assert.IsTrue(results[i].RunningTotal >= results[i - 1].RunningTotal);
    }

    [TestMethod]
    public async Task Sum_WithRangeFrame_ProducesRangeBasedTotal()
    {
        // RANGE frame groups rows by value, not by position.
        // With RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW,
        // tied prices (20, 20) both see the sum up to and including both 20s.
        var query = Context.Orders
            .Select(o => new
            {
                o.Price,
                RangeTotal = WindowFunction.Sum(o.Price,
                    Window.OrderBy(o.Price)
                          .RangeBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)),
            })
            .OrderBy(x => x.Price);

        var sql = query.ToQueryString();
        StringAssert.Contains(sql, "RANGE BETWEEN");

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // The two price-20 rows should have the same RANGE-based running total
        // (RANGE includes all rows with the same ORDER BY value)
        var price20Totals = results.Where(r => r.Price == 20).Select(r => r.RangeTotal).Distinct().ToList();
        Assert.AreEqual(1, price20Totals.Count,
            "RANGE frame should give tied prices the same running total");
    }

    [TestMethod]
    public async Task Lead_WithPartition_ResetsPerGroup()
    {
        var query = Context.Orders
            .Select(o => new
            {
                o.CustomerId,
                o.Price,
                NextInGroup = (double?)WindowFunction.Lead(o.Price,
                    Window.PartitionBy(o.CustomerId).OrderBy(o.Price)),
            })
            .OrderBy(x => x.CustomerId)
            .ThenBy(x => x.Price);

        var results = await query.ToListAsync();
        Assert.AreEqual(10, results.Count);

        // Last row of each customer partition has no next → null
        var c1 = results.Where(r => r.CustomerId == 1).ToList();
        var c2 = results.Where(r => r.CustomerId == 2).ToList();

        Assert.IsNull(c1[^1].NextInGroup, "Last row of customer 1 should have no next");
        Assert.IsNull(c2[^1].NextInGroup, "Last row of customer 2 should have no next");
        // First row of each partition → second row's price
        Assert.AreEqual(c1[1].Price, c1[0].NextInGroup);
        Assert.AreEqual(c2[1].Price, c2[0].NextInGroup);
    }
}
