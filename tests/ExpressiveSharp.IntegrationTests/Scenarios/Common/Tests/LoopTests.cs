using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class LoopTests : StoreTestBase
{
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
}
