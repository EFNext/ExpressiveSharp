using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class CheckedArithmeticTests : StoreTestBase
{
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
}
