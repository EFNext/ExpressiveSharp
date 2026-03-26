using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

/// <summary>
/// Demonstrates the core value of [ExpressiveFor]: using external utility methods
/// in LINQ expression trees (including EF Core queries) that would normally fail
/// because the provider has no built-in translation for them.
///
/// <see cref="PricingUtils"/> is a stand-in for any third-party or BCL utility class.
/// Without [ExpressiveFor], calling <c>PricingUtils.Clamp(o.Price, 20, 100)</c> in an
/// EF Core query would throw "could not be translated". With the mapping in
/// <c>PricingUtilsMappings</c>, the call is replaced with an expression tree
/// (<c>value &lt; min ? min : (value &gt; max ? max : value)</c>) that EF Core translates to SQL.
/// </summary>
public abstract class ExpressiveForMappingTests : StoreTestBase
{
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
}
