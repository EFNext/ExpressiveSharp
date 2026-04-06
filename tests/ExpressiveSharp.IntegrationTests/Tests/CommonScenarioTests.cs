using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

/// <summary>
/// Scenario tests that compile expanded expression trees to delegates and run
/// them against seeded in-memory collections. Validates every feature
/// (arithmetic, switch expressions, pattern matching, null-conditional chains,
/// loops, tuples, constructor projections, etc.) at runtime without touching
/// any ORM.
///
/// The parallel EF Core integration tests live in
/// <c>ExpressiveSharp.EntityFrameworkCore.IntegrationTests</c>.
/// </summary>
[TestClass]
public class CommonScenarioTests
{
    private readonly ExpressionCompileRunner _runner = new();

    [TestInitialize]
    public void Seed()
    {
        _runner.Seed(SeedData.Addresses, SeedData.Customers, SeedData.Orders, SeedData.LineItems);
    }

    // ── Arithmetic ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Select_Total_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, double>(expanded);

        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public void Where_TotalGreaterThan100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    [TestMethod]
    public void Where_NoMatch_ReturnsEmpty()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(10000.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void OrderByDescending_Total_ReturnsSortedDescending()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = _runner.OrderByDescendingSelect<Order, double, int>(expanded, idExpr);

        // Descending by Total: 1500(2), 250(4), 240(1), 30(3)
        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }

    // ── Block Body ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Select_GetCategory_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string>(expanded);

        CollectionAssert.AreEquivalent(
            new[] { "Regular", "Bulk", "Regular", "Regular" },
            results);
    }

    // ── Checked Arithmetic ──────────────────────────────────────────────────

    [TestMethod]
    public void Select_CheckedTotal_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => o.CheckedTotal;
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, double>(expanded);

        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public void Where_CheckedTotalGreaterThan100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> totalExpr = o => o.CheckedTotal;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    // ── Collection Expression ───────────────────────────────────────────────

    [TestMethod]
    public void Select_PriceBreakpoints_ReturnsArrayLiteral()
    {
        Expression<Func<Order, int[]>> expr = o => o.PriceBreakpoints;
        var expanded = (Expression<Func<Order, int[]>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, int[]>(expanded);

        Assert.AreEqual(4, results.Count);
        foreach (var breakpoints in results)
        {
            CollectionAssert.AreEqual(new[] { 10, 50, 100 }, breakpoints);
        }
    }

    // ── Constructor Projection ──────────────────────────────────────────────

    [TestMethod]
    public void Select_OrderDto_ProjectsCorrectly()
    {
        Expression<Func<Order, OrderDto>> expr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
        var expanded = (Expression<Func<Order, OrderDto>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, OrderDto>(expanded);

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
    public void Select_StatusDescription_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string>(expanded);

        CollectionAssert.AreEquivalent(
            new[] { "Order approved", "Awaiting processing", "Order rejected", "Awaiting processing" },
            results);
    }

    [TestMethod]
    public void GroupBy_StatusDescription_CountsCorrectly()
    {
        Expression<Func<Order, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.GroupByCount<Order, string>(expanded);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(2, dict["Awaiting processing"]);
        Assert.AreEqual(1, dict["Order approved"]);
        Assert.AreEqual(1, dict["Order rejected"]);
    }

    // ── ExpressiveFor Mapping ───────────────────────────────────────────────

    [TestMethod]
    public void Select_ClampedPrice_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, double>(expanded);

        CollectionAssert.AreEquivalent(new[] { 100.0, 75.0, 20.0, 50.0 }, results);
    }

    [TestMethod]
    public void Where_ClampedPriceEquals100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> clampExpr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)clampExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public void Select_DiscountedPrice_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => PricingUtils.ApplyDiscount(o.Price, 10.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, double>(expanded);

        var expected = new[] { 108.0, 67.5, 9.0, 45.0 };
        Assert.AreEqual(expected.Length, results.Count);
        foreach (var exp in expected)
            Assert.IsTrue(results.Any(r => Math.Abs(r - exp) < 0.001), $"Expected {exp} in results");
    }

    // ── ExpressiveFor on instance method ────────────────────────────────────

    [TestMethod]
    public void Select_InstanceMethod_ViaExpressiveFor_ReturnsCorrectValues()
    {
        var formatter = new DisplayFormatter("<", ">");

        Expression<Func<Order, string>> expr = o => formatter.Wrap(o.Tag ?? "none");
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.OrderBySelect<Order, int, string>(o => o.Id, expanded);

        CollectionAssert.AreEqual(
            new[] { "<RUSH>", "<STD>", "<none>", "<SPECIAL>" },
            results);
    }

    [TestMethod]
    public void Select_InstanceProperty_ViaExpressiveFor_ReturnsConstant()
    {
        var formatter = new DisplayFormatter("A", "B");

        Expression<Func<Order, string>> expr = o => formatter.Label + ":" + o.Id;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.OrderBySelect<Order, int, string>(o => o.Id, expanded);

        CollectionAssert.AreEqual(
            new[] { "[A/B]:1", "[A/B]:2", "[A/B]:3", "[A/B]:4" },
            results);
    }

    // ── Loop Tests ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Select_ItemCount_ReturnsCorrectCounts()
    {
        Expression<Func<Order, int>> expr = o => o.ItemCount();
        var expanded = (Expression<Func<Order, int>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, int>(expanded);

        CollectionAssert.AreEquivalent(new[] { 2, 1, 0, 1 }, results);
    }

    [TestMethod]
    public void Select_ItemTotal_ReturnsCorrectTotals()
    {
        Expression<Func<Order, double>> expr = o => o.ItemTotal();
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, double>(expanded);

        CollectionAssert.AreEquivalent(new[] { 175.0, 500.0, 0.0, 50.0 }, results);
    }

    [TestMethod]
    public void Select_HasExpensiveItems_ReturnsCorrectFlags()
    {
        Expression<Func<Order, bool>> expr = o => o.HasExpensiveItems();
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, bool>(expanded);

        CollectionAssert.AreEquivalent(new[] { true, true, false, false }, results);
    }

    [TestMethod]
    public void Select_AllItemsAffordable_ReturnsCorrectFlags()
    {
        Expression<Func<Order, bool>> expr = o => o.AllItemsAffordable();
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, bool>(expanded);

        CollectionAssert.AreEquivalent(new[] { true, true, true, true }, results);
    }

    [TestMethod]
    public void Select_ItemTotalForExpensive_ReturnsCorrectTotals()
    {
        Expression<Func<Order, double>> expr = o => o.ItemTotalForExpensive();
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, double>(expanded);

        CollectionAssert.AreEquivalent(new[] { 150.0, 500.0, 0.0, 0.0 }, results);
    }

    // ── Null Conditional ────────────────────────────────────────────────────

    [TestMethod]
    public void Select_CustomerName_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string?>(expanded);

        CollectionAssert.AreEquivalent(new[] { "Alice", "Bob", null, null }, results);
    }

    [TestMethod]
    public void Select_TagLength_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, int?>> expr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, int?>(expanded);

        CollectionAssert.AreEquivalent(new int?[] { 4, 3, null, 7 }, results);
    }

    [TestMethod]
    public void Where_CustomerNameEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("Alice", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public void Where_CustomerNameIsNull_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant(null, typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 3, 4 }, ids);
    }

    [TestMethod]
    public void OrderBy_TagLength_NullsAppearFirst()
    {
        Expression<Func<Order, int?>> tagLenExpr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)tagLenExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = _runner.OrderBySelect<Order, int?, int>(expanded, idExpr);

        Assert.AreEqual(3, results[0]); // Order 3 has null Tag
    }

    // ── Nullable Chain ──────────────────────────────────────────────────────

    [TestMethod]
    public void Select_CustomerCountry_TwoLevelChain()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string?>(expanded);

        CollectionAssert.AreEquivalent(new[] { "US", "UK", null, null }, results);
    }

    [TestMethod]
    public void Where_CustomerCountryEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> countryExpr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)countryExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("US", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = _runner.Where(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public void Select_CustomerCity_ViaCustomerExpressive()
    {
        var expr = ExpressionPolyfill.Create((Customer c) => c.Address != null ? c.Address.City : null);

        var results = _runner.Select<Customer, string?>(expr);

        CollectionAssert.AreEquivalent(new[] { "New York", "London", null }, results);
    }

    // ── Pattern Matching ────────────────────────────────────────────────────

    [TestMethod]
    public void Where_IsPattern_NullCheck()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag != null);

        var results = _runner.Where(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    [TestMethod]
    public void Select_ConditionalWithComparison_ReturnsCorrectValues()
    {
        var expr = ExpressionPolyfill.Create((Order o) =>
            o.Quantity >= 20 ? "High Volume"
            : o.Quantity >= 5 ? "Medium Volume"
            : "Low Volume");

        var results = _runner.Select<Order, string>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "Low Volume", "High Volume", "Low Volume", "Medium Volume" },
            results);
    }

    [TestMethod]
    public void Where_CompoundCondition_FiltersCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50 && o.Tag != null);

        var results = _runner.Where(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public void Select_NestedTernary_ReturnsCorrectValues()
    {
        var expr = ExpressionPolyfill.Create((Order o) =>
            o.Status == OrderStatus.Rejected ? "Rejected"
            : o.Price >= 100 ? "Premium Active"
            : "Standard Active");

        var results = _runner.Select<Order, string>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "Premium Active", "Standard Active", "Rejected", "Standard Active" },
            results);
    }

    // ── Polyfill Pathway ────────────────────────────────────────────────────

    [TestMethod]
    public void Polyfill_SimpleCondition_FiltersCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50);

        var results = _runner.Where(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public void Polyfill_Arithmetic_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price * o.Quantity);

        var results = _runner.Select<Order, double>(expr);

        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public void Polyfill_NullConditional_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Customer != null ? o.Customer.Name : null);

        var results = _runner.Select<Order, string?>(expr);

        CollectionAssert.AreEquivalent(new[] { "Alice", "Bob", null, null }, results);
    }

    [TestMethod]
    public void Polyfill_NullCoalescing_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag ?? "N/A");

        var results = _runner.Select<Order, string>(expr);

        CollectionAssert.AreEquivalent(new[] { "RUSH", "STD", "N/A", "SPECIAL" }, results);
    }

    // ── String Operations ───────────────────────────────────────────────────

    [TestMethod]
    public void Select_Summary_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.Summary;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string>(expanded);

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
    public void Select_SummaryConcat_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.SummaryConcat;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string>(expanded);

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
    public void Select_GetGrade_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetGrade();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, string>(expanded);

        CollectionAssert.AreEquivalent(new[] { "Premium", "Standard", "Budget", "Standard" }, results);
    }

    [TestMethod]
    public void OrderBy_GetGrade_ReturnsSorted()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = _runner.OrderBySelect<Order, string, int>(expandedGrade, idExpr);

        Assert.AreEqual(3, results[0]);
        Assert.AreEqual(1, results[1]);
    }

    [TestMethod]
    public void OrderByDescending_GetGrade_ReturnsSortedDescending()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = _runner.OrderByDescendingSelect<Order, string, int>(expandedGrade, idExpr);

        Assert.AreEqual(3, results.Last());
        Assert.AreEqual(1, results[^2]);
    }

    [TestMethod]
    public void GroupBy_GetGrade_CountsCorrectly()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        var results = _runner.GroupByCount<Order, string>(expandedGrade);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual(1, dict["Premium"]);
        Assert.AreEqual(2, dict["Standard"]);
        Assert.AreEqual(1, dict["Budget"]);
    }

    // ── Tuple Binary ────────────────────────────────────────────────────────

    [TestMethod]
    public void Select_IsPriceQuantityMatch_ReturnsCorrectValues()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityMatch;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, bool>(expanded);

        CollectionAssert.AreEquivalent(new[] { false, false, false, true }, results);
    }

    [TestMethod]
    public void Select_IsPriceQuantityDifferent_ReturnsCorrectValues()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityDifferent;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, bool>(expanded);

        CollectionAssert.AreEquivalent(new[] { true, true, true, false }, results);
    }

    [TestMethod]
    public void Where_IsPriceQuantityMatch_FiltersCorrectly()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityMatch;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = _runner.Where(expanded);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(4, results[0].Id);
    }

    // ── Tuple Projection ────────────────────────────────────────────────────

    [TestMethod]
    public void Select_InlineTuple_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Price));

        var results = _runner.Select<Order, (int, double)>(expr);

        Assert.AreEqual(4, results.Count);
        Assert.IsTrue(results.Contains((1, 120.0)));
        Assert.IsTrue(results.Contains((3, 10.0)));
    }

    [TestMethod]
    public void Select_TupleWithExpressiveMember_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Price * o.Quantity));

        var results = _runner.Select<Order, (int, double)>(expr);

        Assert.IsTrue(results.Contains((1, 240.0)));
        Assert.IsTrue(results.Contains((2, 1500.0)));
        Assert.IsTrue(results.Contains((3, 30.0)));
        Assert.IsTrue(results.Contains((4, 250.0)));
    }

    [TestMethod]
    public void Select_TupleWithNullable_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Tag ?? "none"));

        var results = _runner.Select<Order, (int, string)>(expr);

        Assert.IsTrue(results.Contains((1, "RUSH")));
        Assert.IsTrue(results.Contains((3, "none")));
    }

    // ── C# Language Feature Coverage ────────────────────────────────────
    //
    // These features are documented as supported by the generator but have
    // no integration coverage elsewhere. Tests live in the ExpressionCompile
    // path because most of them (with expressions, dictionary initializers)
    // don't SQL-translate.

    [TestMethod]
    public void WithExpression_OnRecord_ReturnsModifiedInstance()
    {
        var template = new PriceInfo(0, 1.0);
        var expr = ExpressionPolyfill.Create((Order o) =>
            (template with { BasePrice = o.Price, Multiplier = 0.9 }).Final);

        var results = _runner.Select<Order, double>(expr);

        // Price * 0.9: 120*0.9=108, 75*0.9=67.5, 10*0.9=9, 50*0.9=45
        var expected = new[] { 108.0, 67.5, 9.0, 45.0 };
        Assert.AreEqual(expected.Length, results.Count);
        foreach (var exp in expected)
            Assert.IsTrue(results.Any(r => Math.Abs(r - exp) < 0.001), $"Expected {exp}");
    }

    // Generator gaps discovered during this test expansion (documented in
    // limitations.md as "Supported", but don't work in practice today):
    //
    //   * List patterns   [_, _, _] / [var x, ..]
    //       EXP0008 at compile time — "unsupported operation (ListPattern)"
    //   * Index-from-end  arr[^1]
    //       Runtime: "Argument for array index must be of type Int32" — the
    //       generator emits Expression.ArrayIndex with a System.Index operand
    //       instead of converting to int.
    //   * Range operator  arr[1..]
    //       Same issue as Index-from-end.
    //   * Dictionary indexer initializer  new Dictionary { ["k"] = v }
    //       Runtime: ArgumentNullException "member cannot be null" — generator
    //       hits a null member lookup when emitting the collection initializer.
    //
    // These are genuine generator bugs, not test gaps. A follow-up PR should
    // either fix the generator or remove these from the supported list in
    // limitations.md. For now, only `with` expressions are tested since
    // they work correctly.
}
