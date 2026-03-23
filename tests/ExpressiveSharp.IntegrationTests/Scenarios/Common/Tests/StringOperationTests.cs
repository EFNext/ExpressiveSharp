using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class StringOperationTests : StoreTestBase
{
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
}
