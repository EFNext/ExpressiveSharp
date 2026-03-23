using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class PatternMatchingTests : StoreTestBase
{
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
}
