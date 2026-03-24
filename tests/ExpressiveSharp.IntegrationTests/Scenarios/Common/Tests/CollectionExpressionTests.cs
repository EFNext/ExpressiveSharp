using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class CollectionExpressionTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_PriceBreakpoints_ReturnsArrayLiteral()
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
}
