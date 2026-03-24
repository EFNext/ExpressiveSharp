using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class TupleBinaryTests : StoreTestBase
{
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
}
