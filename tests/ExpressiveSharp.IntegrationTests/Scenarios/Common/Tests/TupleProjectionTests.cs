using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class TupleProjectionTests : StoreTestBase
{
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
