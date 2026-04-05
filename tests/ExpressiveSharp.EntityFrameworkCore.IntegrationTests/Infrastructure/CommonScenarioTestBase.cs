using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Scenario tests exercising individual ExpressiveSharp features (arithmetic,
/// switch expressions, pattern matching, null-conditional chains, loops,
/// tuples, constructor projections, etc.) against a real EF Core provider.
/// Runs against any provider (relational or Cosmos).
/// </summary>
public abstract class CommonScenarioTestBase : EFCoreTestBase
{
    [TestInitialize]
    public virtual Task SeedStoreData() => Context.SeedStoreAsync();

    // ── Arithmetic ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_Total_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        // Order 1: 120*2=240, Order 2: 75*20=1500, Order 3: 10*3=30, Order 4: 50*5=250
        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public async Task Where_TotalGreaterThan100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    [TestMethod]
    public async Task Where_NoMatch_ReturnsEmpty()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(10000.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task OrderByDescending_Total_ReturnsSortedDescending()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Context.Set<Order>().OrderByDescending(expanded).Select(idExpr).ToListAsync();

        // Descending by Total: 1500(2), 250(4), 240(1), 30(3)
        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }

    // ── Block Body ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_GetCategory_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(
            new[] { "Regular", "Bulk", "Regular", "Regular" },
            results);
    }

    // ── Checked Arithmetic ──────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_CheckedTotal_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => o.CheckedTotal;
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public virtual async Task Where_CheckedTotalGreaterThan100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> totalExpr = o => o.CheckedTotal;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    // ── Collection Expression ───────────────────────────────────────────────

    [TestMethod]
    public virtual async Task Select_PriceBreakpoints_ReturnsArrayLiteral()
    {
        Expression<Func<Order, int[]>> expr = o => o.PriceBreakpoints;
        var expanded = (Expression<Func<Order, int[]>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        Assert.AreEqual(4, results.Count);
        foreach (var breakpoints in results)
        {
            CollectionAssert.AreEqual(new[] { 10, 50, 100 }, breakpoints);
        }
    }

    // ── Constructor Projection ──────────────────────────────────────────────

    [TestMethod]
    public async Task Select_OrderDto_ProjectsCorrectly()
    {
        Expression<Func<Order, OrderDto>> expr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
        var expanded = (Expression<Func<Order, OrderDto>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        Assert.AreEqual(4, results.Count);

        var order1 = results.Single(r => r.Id == 1);
        Assert.AreEqual("RUSH", order1.Description);
        Assert.AreEqual(240.0, order1.Total);

        var order3 = results.Single(r => r.Id == 3);
        Assert.AreEqual("N/A", order3.Description);
        Assert.AreEqual(30.0, order3.Total);
    }

    // ── Enum Expansion ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_StatusDescription_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(
            new[] { "Order approved", "Awaiting processing", "Order rejected", "Awaiting processing" },
            results);
    }

    [TestMethod]
    public virtual async Task GroupBy_StatusDescription_CountsCorrectly()
    {
        Expression<Func<Order, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .GroupBy(expanded)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToListAsync();

        var dict = results.ToDictionary(r => r.Key, r => r.Count);
        Assert.AreEqual(2, dict["Awaiting processing"]);  // Pending × 2
        Assert.AreEqual(1, dict["Order approved"]);
        Assert.AreEqual(1, dict["Order rejected"]);
    }

    // ── ExpressiveFor Mapping ───────────────────────────────────────────────

    [TestMethod]
    public async Task Select_ClampedPrice_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 100.0, 75.0, 20.0, 50.0 }, results);
    }

    [TestMethod]
    public async Task Where_ClampedPriceEquals100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> clampExpr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)clampExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task Select_DiscountedPrice_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => PricingUtils.ApplyDiscount(o.Price, 10.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        var expected = new[] { 108.0, 67.5, 9.0, 45.0 };
        Assert.AreEqual(expected.Length, results.Count);
        foreach (var exp in expected)
            Assert.IsTrue(results.Any(r => Math.Abs(r - exp) < 0.001), $"Expected {exp} in results");
    }

    // ── ExpressiveFor on instance method ────────────────────────────────────

    [TestMethod]
    public async Task Select_InstanceMethod_ViaExpressiveFor_ReturnsCorrectValues()
    {
        // DisplayFormatter.Wrap is an instance method mapped via
        // [ExpressiveFor]. The formatter is captured from outer scope
        // (like a DI-injected service would be).
        var formatter = new DisplayFormatter("<", ">");

        Expression<Func<Order, string>> expr = o => formatter.Wrap(o.Tag ?? "none");
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        CollectionAssert.AreEqual(
            new[] { "<RUSH>", "<STD>", "<none>", "<SPECIAL>" },
            results);
    }

    [TestMethod]
    public async Task Select_InstanceProperty_ViaExpressiveFor_ReturnsConstant()
    {
        // DisplayFormatter.Label is an instance property mapped via
        // [ExpressiveFor]. The property value is captured from outer scope
        // (Prefix/Suffix are captured) — the expanded expression becomes a
        // constant string interpolation evaluated per-row.
        var formatter = new DisplayFormatter("A", "B");

        Expression<Func<Order, string>> expr = o => formatter.Label + ":" + o.Id;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        CollectionAssert.AreEqual(
            new[] { "[A/B]:1", "[A/B]:2", "[A/B]:3", "[A/B]:4" },
            results);
    }

    // ── Loop Tests ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_ItemCount_ReturnsCorrectCounts()
    {
        Expression<Func<Order, int>> expr = o => o.ItemCount();
        var expanded = (Expression<Func<Order, int>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 2, 1, 0, 1 }, results);
    }

    [TestMethod]
    public async Task Select_ItemTotal_ReturnsCorrectTotals()
    {
        Expression<Func<Order, double>> expr = o => o.ItemTotal();
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 175.0, 500.0, 0.0, 50.0 }, results);
    }

    [TestMethod]
    public async Task Select_HasExpensiveItems_ReturnsCorrectFlags()
    {
        Expression<Func<Order, bool>> expr = o => o.HasExpensiveItems();
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { true, true, false, false }, results);
    }

    [TestMethod]
    public async Task Select_AllItemsAffordable_ReturnsCorrectFlags()
    {
        Expression<Func<Order, bool>> expr = o => o.AllItemsAffordable();
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { true, true, true, true }, results);
    }

    [TestMethod]
    public async Task Select_ItemTotalForExpensive_ReturnsCorrectTotals()
    {
        Expression<Func<Order, double>> expr = o => o.ItemTotalForExpensive();
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 150.0, 500.0, 0.0, 0.0 }, results);
    }

    // ── Null Conditional ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_CustomerName_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "Alice", "Bob", null, null }, results);
    }

    [TestMethod]
    public async Task Select_TagLength_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, int?>> expr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new int?[] { 4, 3, null, 7 }, results);
    }

    [TestMethod]
    public async Task Where_CustomerNameEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("Alice", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task Where_CustomerNameIsNull_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant(null, typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 3, 4 }, ids);
    }

    [TestMethod]
    public virtual async Task OrderBy_TagLength_NullsAppearFirst()
    {
        // NULL sort order is provider-specific: SQLite/SQL Server/MySQL
        // put NULLs first on ASC, PostgreSQL puts them last. This test
        // asserts the SQL Server / SQLite / MySQL convention; Postgres
        // overrides with its own expectation.
        Expression<Func<Order, int?>> tagLenExpr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)tagLenExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Context.Set<Order>().OrderBy(expanded).Select(idExpr).ToListAsync();

        Assert.AreEqual(4, results.Count);
        Assert.AreEqual(3, results[0]); // Order 3 has null Tag → sorts first
    }

    // ── Nullable Chain ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_CustomerCountry_TwoLevelChain()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "US", "UK", null, null }, results);
    }

    [TestMethod]
    public async Task Where_CustomerCountryEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> countryExpr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)countryExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("US", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Context.Set<Order>().Where(predicate).ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public virtual async Task Select_CustomerCity_ViaCustomerExpressive()
    {
        // Customer.City is [Expressive] => Customer?.Address?.City
        var expr = ExpressionPolyfill.Create((Customer c) => c.Address != null ? c.Address.City : null);

        var results = await Context.Set<Customer>().Select(expr).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "New York", "London", null }, results);
    }

    // ── Pattern Matching ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Where_IsPattern_NullCheck()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag != null);

        var results = await Context.Set<Order>().Where(expr).ToListAsync();

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    [TestMethod]
    public async Task Select_ConditionalWithComparison_ReturnsCorrectValues()
    {
        var expr = ExpressionPolyfill.Create((Order o) =>
            o.Quantity >= 20 ? "High Volume"
            : o.Quantity >= 5 ? "Medium Volume"
            : "Low Volume");

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        CollectionAssert.AreEquivalent(
            new[] { "Low Volume", "High Volume", "Low Volume", "Medium Volume" },
            results);
    }

    [TestMethod]
    public async Task Where_CompoundCondition_FiltersCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50 && o.Tag != null);

        var results = await Context.Set<Order>().Where(expr).ToListAsync();

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public async Task Select_NestedTernary_ReturnsCorrectValues()
    {
        var expr = ExpressionPolyfill.Create((Order o) =>
            o.Status == OrderStatus.Rejected ? "Rejected"
            : o.Price >= 100 ? "Premium Active"
            : "Standard Active");

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        CollectionAssert.AreEquivalent(
            new[] { "Premium Active", "Standard Active", "Rejected", "Standard Active" },
            results);
    }

    // ── Polyfill Pathway ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Polyfill_SimpleCondition_FiltersCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50);

        var results = await Context.Set<Order>().Where(expr).ToListAsync();

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public async Task Polyfill_Arithmetic_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price * o.Quantity);

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public async Task Polyfill_NullConditional_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Customer != null ? o.Customer.Name : null);

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "Alice", "Bob", null, null }, results);
    }

    [TestMethod]
    public async Task Polyfill_NullCoalescing_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag ?? "N/A");

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "RUSH", "STD", "N/A", "SPECIAL" }, results);
    }

    // ── String Operations ───────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_Summary_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.Summary;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                "Order #1: RUSH",
                "Order #2: STD",
                "Order #3: N/A",
                "Order #4: SPECIAL",
            },
            results);
    }

    [TestMethod]
    public async Task Select_SummaryConcat_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.SummaryConcat;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                "Order #1: RUSH",
                "Order #2: STD",
                "Order #3: N/A",
                "Order #4: SPECIAL",
            },
            results);
    }

    // ── Switch Expression ───────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_GetGrade_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetGrade();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "Premium", "Standard", "Budget", "Standard" }, results);
    }

    [TestMethod]
    public async Task OrderBy_GetGrade_ReturnsSorted()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Context.Set<Order>().OrderBy(expandedGrade).Select(idExpr).ToListAsync();

        // Alphabetical: Budget(3), Premium(1), Standard(2,4)
        Assert.AreEqual(3, results[0]);
        Assert.AreEqual(1, results[1]);
    }

    [TestMethod]
    public async Task OrderByDescending_GetGrade_ReturnsSortedDescending()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Context.Set<Order>().OrderByDescending(expandedGrade).Select(idExpr).ToListAsync();

        Assert.AreEqual(3, results.Last());
        Assert.AreEqual(1, results[^2]);
    }

    [TestMethod]
    public virtual async Task GroupBy_GetGrade_CountsCorrectly()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .GroupBy(expandedGrade)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToListAsync();

        var dict = results.ToDictionary(r => r.Key, r => r.Count);
        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual(1, dict["Premium"]);
        Assert.AreEqual(2, dict["Standard"]);
        Assert.AreEqual(1, dict["Budget"]);
    }

    // ── Tuple Binary ────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_IsPriceQuantityMatch_ReturnsCorrectValues()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityMatch;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { false, false, false, true }, results);
    }

    [TestMethod]
    public async Task Select_IsPriceQuantityDifferent_ReturnsCorrectValues()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityDifferent;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { true, true, true, false }, results);
    }

    [TestMethod]
    public virtual async Task Where_IsPriceQuantityMatch_FiltersCorrectly()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityMatch;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Where(expanded).ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(4, results[0].Id);
    }

    // ── Tuple Projection ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_InlineTuple_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Price));

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        Assert.AreEqual(4, results.Count);
        Assert.IsTrue(results.Contains((1, 120.0)));
        Assert.IsTrue(results.Contains((3, 10.0)));
    }

    [TestMethod]
    public async Task Select_TupleWithExpressiveMember_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Price * o.Quantity));

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        Assert.IsTrue(results.Contains((1, 240.0)));
        Assert.IsTrue(results.Contains((2, 1500.0)));
        Assert.IsTrue(results.Contains((3, 30.0)));
        Assert.IsTrue(results.Contains((4, 250.0)));
    }

    [TestMethod]
    public async Task Select_TupleWithNullable_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Tag ?? "none"));

        var results = await Context.Set<Order>().Select(expr).ToListAsync();

        Assert.IsTrue(results.Contains((1, "RUSH")));
        Assert.IsTrue(results.Contains((3, "none")));
    }
}
