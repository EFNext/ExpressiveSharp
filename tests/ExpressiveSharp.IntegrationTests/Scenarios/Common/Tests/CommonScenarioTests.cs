using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class CommonScenarioTests : StoreTestBase
{
    // ── Arithmetic ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_Total_ReturnsCorrectValues()
    {
        Expression<Func<Order, double>> expr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, double>(expanded);

        // Order 1: 120*2=240, Order 2: 75*20=1500, Order 3: 10*3=30, Order 4: 50*5=250
        CollectionAssert.AreEquivalent(
            new[] { 240.0, 1500.0, 30.0, 250.0 },
            results);
    }

    [TestMethod]
    public async Task Where_TotalGreaterThan100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

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

        var results = await Runner.WhereAsync(predicate);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task OrderByDescending_Total_ReturnsSortedDescending()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderByDescendingSelectAsync<Order, double, int>(expanded, idExpr);

        // Descending by Total: 1500(2), 250(4), 240(1), 30(3)
        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }

    // ── Block Body ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_GetCategory_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string>(expanded);

        // Order 1: 2*10=20, not > 100 → Regular
        // Order 2: 20*10=200, > 100 → Bulk
        // Order 3: 3*10=30, not > 100 → Regular
        // Order 4: 5*10=50, not > 100 → Regular
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

        var results = await Runner.SelectAsync<Order, double>(expanded);

        // Same as Total: 120*2=240, 75*20=1500, 10*3=30, 50*5=250
        CollectionAssert.AreEquivalent(
            new[] { 240.0, 1500.0, 30.0, 250.0 },
            results);
    }

    [TestMethod]
    public virtual async Task Where_CheckedTotalGreaterThan100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> totalExpr = o => o.CheckedTotal;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.GreaterThan(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    // ── Collection Expression ───────────────────────────────────────────────

    [TestMethod]
    public virtual async Task Select_PriceBreakpoints_ReturnsArrayLiteral()
    {
        Expression<Func<Order, int[]>> expr = o => o.PriceBreakpoints;
        var expanded = (Expression<Func<Order, int[]>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, int[]>(expanded);

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

        var results = await Runner.SelectAsync<Order, OrderDto>(expanded);

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

        var results = await Runner.SelectAsync<Order, string>(expanded);

        // Order 1: Approved, Order 2: Pending, Order 3: Rejected, Order 4: Pending
        CollectionAssert.AreEquivalent(
            new[] { "Order approved", "Awaiting processing", "Order rejected", "Awaiting processing" },
            results);
    }

    [TestMethod]
    public virtual async Task GroupBy_StatusDescription_CountsCorrectly()
    {
        Expression<Func<Order, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Runner.GroupByCountAsync<Order, string>(expanded);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(2, dict["Awaiting processing"]);  // Pending × 2
        Assert.AreEqual(1, dict["Order approved"]);
        Assert.AreEqual(1, dict["Order rejected"]);
    }

    // ── ExpressiveFor Mapping ───────────────────────────────────────────────

    [TestMethod]
    public async Task Select_ClampedPrice_ReturnsCorrectValues()
    {
        // PricingUtils.Clamp(price, 20, 100) → clamps each order's price to [20, 100]
        Expression<Func<Order, double>> expr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, double>(expanded);

        // Order 1: Clamp(120, 20, 100) = 100
        // Order 2: Clamp(75, 20, 100)  = 75
        // Order 3: Clamp(10, 20, 100)  = 20   (below minimum)
        // Order 4: Clamp(50, 20, 100)  = 50
        CollectionAssert.AreEquivalent(
            new[] { 100.0, 75.0, 20.0, 50.0 },
            results);
    }

    [TestMethod]
    public async Task Where_ClampedPriceEquals100_FiltersCorrectly()
    {
        Expression<Func<Order, double>> clampExpr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)clampExpr.ExpandExpressives();

        // Filter: orders whose clamped price equals 100 (only Order 1 with Price=120)
        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant(100.0));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task Select_DiscountedPrice_ReturnsCorrectValues()
    {
        // 10% discount on each order's price
        Expression<Func<Order, double>> expr = o => PricingUtils.ApplyDiscount(o.Price, 10.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, double>(expanded);

        // Order 1: 120 * 0.9 = 108
        // Order 2: 75 * 0.9  = 67.5
        // Order 3: 10 * 0.9  = 9
        // Order 4: 50 * 0.9  = 45
        var expected = new[] { 108.0, 67.5, 9.0, 45.0 };
        Assert.AreEqual(expected.Length, results.Count);
        foreach (var exp in expected)
            Assert.IsTrue(results.Any(r => Math.Abs(r - exp) < 0.001),
                $"Expected {exp} in results");
    }

    // ── Loop Tests ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_ItemCount_ReturnsCorrectCounts()
    {
        Expression<Func<Order, int>> expr = o => o.ItemCount();
        var expanded = (Expression<Func<Order, int>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, int>(expanded);

        // Order 1: 2 items (Widget, Gadget), Order 2: 1 item, Order 3: 0 items, Order 4: 1 item
        CollectionAssert.AreEquivalent(new[] { 2, 1, 0, 1 }, results);
    }

    [TestMethod]
    public async Task Select_ItemTotal_ReturnsCorrectTotals()
    {
        Expression<Func<Order, double>> expr = o => o.ItemTotal();
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, double>(expanded);

        // Order 1: 50*3 + 25*1 = 175, Order 2: 50*10 = 500, Order 3: 0 (no items), Order 4: 10*5 = 50
        CollectionAssert.AreEquivalent(new[] { 175.0, 500.0, 0.0, 50.0 }, results);
    }

    [TestMethod]
    public async Task Select_HasExpensiveItems_ReturnsCorrectFlags()
    {
        Expression<Func<Order, bool>> expr = o => o.HasExpensiveItems();
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, bool>(expanded);

        // Order 1: Widget 50.0 > 40 → true, Order 2: Widget 50.0 > 40 → true,
        // Order 3: no items → false, Order 4: Gizmo 10.0 ≤ 40 → false
        CollectionAssert.AreEquivalent(new[] { true, true, false, false }, results);
    }

    [TestMethod]
    public async Task Select_AllItemsAffordable_ReturnsCorrectFlags()
    {
        Expression<Func<Order, bool>> expr = o => o.AllItemsAffordable();
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, bool>(expanded);

        // All items have UnitPrice <= 100, so All returns true for all orders
        // Order 3 has no items — All() on empty returns true
        CollectionAssert.AreEquivalent(new[] { true, true, true, true }, results);
    }

    [TestMethod]
    public async Task Select_ItemTotalForExpensive_ReturnsCorrectTotals()
    {
        Expression<Func<Order, double>> expr = o => o.ItemTotalForExpensive();
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, double>(expanded);

        // Order 1: Widget 50.0 > 40 → 50*3=150 (Gadget 25.0 ≤ 40 skipped)
        // Order 2: Widget 50.0 > 40 → 50*10=500
        // Order 3: no items → 0
        // Order 4: Gizmo 10.0 ≤ 40 → 0
        CollectionAssert.AreEquivalent(new[] { 150.0, 500.0, 0.0, 0.0 }, results);
    }

    // ── Null Conditional ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_CustomerName_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string?>(expanded);

        // Order 1: Alice, Order 2: Bob, Order 3: null (no customer), Order 4: null (customer.Name is null)
        CollectionAssert.AreEquivalent(
            new[] { "Alice", "Bob", null, null },
            results);
    }

    [TestMethod]
    public async Task Select_TagLength_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, int?>> expr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, int?>(expanded);

        // Order 1: "RUSH".Length=4, Order 2: "STD".Length=3, Order 3: null, Order 4: "SPECIAL".Length=7
        CollectionAssert.AreEquivalent(
            new int?[] { 4, 3, null, 7 },
            results);
    }

    [TestMethod]
    public async Task Where_CustomerNameEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("Alice", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

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

        var results = await Runner.WhereAsync(predicate);

        // Order 3: no customer, Order 4: customer with null Name
        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 3, 4 }, ids);
    }

    [TestMethod]
    public async Task OrderBy_TagLength_NullsAppearFirst()
    {
        Expression<Func<Order, int?>> tagLenExpr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)tagLenExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderBySelectAsync<Order, int?, int>(expanded, idExpr);

        // null sorts first, then 3 ("STD"), 4 ("RUSH"), 7 ("SPECIAL")
        Assert.AreEqual(3, results[0]); // Order 3 has null Tag
    }

    // ── Nullable Chain ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_CustomerCountry_TwoLevelChain()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string?>(expanded);

        // Order 1: Customer 1 → Address 1 → "US"
        // Order 2: Customer 2 → Address 2 → "UK"
        // Order 3: no customer → null
        // Order 4: Customer 3 → no address → null
        CollectionAssert.AreEquivalent(
            new[] { "US", "UK", null, null },
            results);
    }

    [TestMethod]
    public async Task Where_CustomerCountryEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> countryExpr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)countryExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("US", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public virtual async Task Select_CustomerCity_ViaCustomerExpressive()
    {
        // Test Customer.City which is Customer?.Address?.City (single-level on Customer)
        var expr = ExpressionPolyfill.Create((Customer c) => c.Address != null ? c.Address.City : null);

        var results = await Runner.SelectAsync<Customer, string?>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "New York", "London", null },
            results);
    }

    // ── Pattern Matching ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Where_IsPattern_NullCheck()
    {
        // Pattern: o.Tag is not null
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag != null);

        var results = await Runner.WhereAsync(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        // Orders 1, 2, 4 have non-null tags; Order 3 has null
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, ids);
    }

    [TestMethod]
    public async Task Select_ConditionalWithComparison_ReturnsCorrectValues()
    {
        // Relational pattern via ternary: discount tier based on Quantity
        var expr = ExpressionPolyfill.Create((Order o) =>
            o.Quantity >= 20 ? "High Volume"
            : o.Quantity >= 5 ? "Medium Volume"
            : "Low Volume");

        var results = await Runner.SelectAsync<Order, string>(expr);

        // Order 1: Qty=2 → Low, Order 2: Qty=20 → High, Order 3: Qty=3 → Low, Order 4: Qty=5 → Medium
        CollectionAssert.AreEquivalent(
            new[] { "Low Volume", "High Volume", "Low Volume", "Medium Volume" },
            results);
    }

    [TestMethod]
    public async Task Where_CompoundCondition_FiltersCorrectly()
    {
        // Compound: Price > 50 AND Tag is not null
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50 && o.Tag != null);

        var results = await Runner.WhereAsync(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        // Order 1: 120 > 50 && "RUSH" ✓, Order 2: 75 > 50 && "STD" ✓, Order 4: 50 > 50 ✗
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public async Task Select_NestedTernary_ReturnsCorrectValues()
    {
        // Nested ternary: classify by status and price
        var expr = ExpressionPolyfill.Create((Order o) =>
            o.Status == OrderStatus.Rejected ? "Rejected"
            : o.Price >= 100 ? "Premium Active"
            : "Standard Active");

        var results = await Runner.SelectAsync<Order, string>(expr);

        // Order 1: Approved, 120 → "Premium Active"
        // Order 2: Pending, 75 → "Standard Active"
        // Order 3: Rejected → "Rejected"
        // Order 4: Pending, 50 → "Standard Active"
        CollectionAssert.AreEquivalent(
            new[] { "Premium Active", "Standard Active", "Rejected", "Standard Active" },
            results);
    }

    // ── Polyfill Pathway ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Polyfill_SimpleCondition_FiltersCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50);

        var results = await Runner.WhereAsync(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        // Order 1: 120 > 50 ✓, Order 2: 75 > 50 ✓, Order 3: 10 > 50 ✗, Order 4: 50 > 50 ✗
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public async Task Polyfill_Arithmetic_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price * o.Quantity);

        var results = await Runner.SelectAsync<Order, double>(expr);

        CollectionAssert.AreEquivalent(
            new[] { 240.0, 1500.0, 30.0, 250.0 },
            results);
    }

    [TestMethod]
    public async Task Polyfill_NullConditional_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Customer != null ? o.Customer.Name : null);

        var results = await Runner.SelectAsync<Order, string?>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "Alice", "Bob", null, null },
            results);
    }

    [TestMethod]
    public async Task Polyfill_NullCoalescing_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag ?? "N/A");

        var results = await Runner.SelectAsync<Order, string>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "RUSH", "STD", "N/A", "SPECIAL" },
            results);
    }

    // ── String Operations ───────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_Summary_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.Summary;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string>(expanded);

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

        var results = await Runner.SelectAsync<Order, string>(expanded);

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

        var results = await Runner.SelectAsync<Order, string>(expanded);

        // Order 1: 120 >= 100 → Premium, Order 2: 75 >= 50 → Standard,
        // Order 3: 10 < 50 → Budget, Order 4: 50 >= 50 → Standard
        CollectionAssert.AreEquivalent(
            new[] { "Premium", "Standard", "Budget", "Standard" },
            results);
    }

    [TestMethod]
    public async Task OrderBy_GetGrade_ReturnsSorted()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderBySelectAsync<Order, string, int>(expandedGrade, idExpr);

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

        var results = await Runner.OrderByDescendingSelectAsync<Order, string, int>(expandedGrade, idExpr);

        // Reverse alphabetical: Standard(2,4), Premium(1), Budget(3)
        Assert.AreEqual(3, results.Last());
        Assert.AreEqual(1, results[^2]);
    }

    [TestMethod]
    public virtual async Task GroupBy_GetGrade_CountsCorrectly()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        var results = await Runner.GroupByCountAsync<Order, string>(expandedGrade);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
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

        var results = await Runner.SelectAsync<Order, bool>(expanded);

        // Order 1: (120,2)==(50,5) false, Order 2: (75,20)==(50,5) false,
        // Order 3: (10,3)==(50,5) false, Order 4: (50,5)==(50,5) true
        CollectionAssert.AreEquivalent(
            new[] { false, false, false, true },
            results);
    }

    [TestMethod]
    public async Task Select_IsPriceQuantityDifferent_ReturnsCorrectValues()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityDifferent;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, bool>(expanded);

        // Opposite of IsPriceQuantityMatch
        CollectionAssert.AreEquivalent(
            new[] { true, true, true, false },
            results);
    }

    [TestMethod]
    public virtual async Task Where_IsPriceQuantityMatch_FiltersCorrectly()
    {
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityMatch;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Runner.WhereAsync(expanded);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(4, results[0].Id);
    }

    // ── Tuple Projection ────────────────────────────────────────────────────

    [TestMethod]
    public async Task Select_InlineTuple_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Price));

        var results = await Runner.SelectAsync<Order, (int, double)>(expr);

        Assert.AreEqual(4, results.Count);
        Assert.IsTrue(results.Contains((1, 120.0)));
        Assert.IsTrue(results.Contains((3, 10.0)));
    }

    [TestMethod]
    public async Task Select_TupleWithExpressiveMember_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Price * o.Quantity));

        var results = await Runner.SelectAsync<Order, (int, double)>(expr);

        Assert.IsTrue(results.Contains((1, 240.0)));
        Assert.IsTrue(results.Contains((2, 1500.0)));
        Assert.IsTrue(results.Contains((3, 30.0)));
        Assert.IsTrue(results.Contains((4, 250.0)));
    }

    [TestMethod]
    public async Task Select_TupleWithNullable_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => (o.Id, o.Tag ?? "none"));

        var results = await Runner.SelectAsync<Order, (int, string)>(expr);

        Assert.IsTrue(results.Contains((1, "RUSH")));
        Assert.IsTrue(results.Contains((3, "none")));
    }
}
