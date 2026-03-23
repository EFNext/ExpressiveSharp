using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class EnumExpansionTests : StoreTestBase
{
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
    public async Task GroupBy_StatusDescription_CountsCorrectly()
    {
        Expression<Func<Order, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Runner.GroupByCountAsync<Order, string>(expanded);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(2, dict["Awaiting processing"]);  // Pending × 2
        Assert.AreEqual(1, dict["Order approved"]);
        Assert.AreEqual(1, dict["Order rejected"]);
    }
}
