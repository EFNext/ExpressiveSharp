using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class ArithmeticTests : StoreTestBase
{
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
}
