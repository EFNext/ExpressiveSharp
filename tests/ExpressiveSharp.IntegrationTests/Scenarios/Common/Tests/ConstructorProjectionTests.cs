using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class ConstructorProjectionTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_OrderDto_ProjectsCorrectly()
    {
        Expression<Func<Order, OrderDto>> expr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
        var expanded = (Expression<Func<Order, OrderDto>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, OrderDto>(expanded);

        Assert.AreEqual(4, results.Count);

        var order1 = results.Single(r => r.Id == 1);
        Assert.AreEqual("RUSH", order1.Description);
        Assert.AreEqual(240.0, order1.Total);

        var order3 = results.Single(r => r.Id == 3);
        Assert.AreEqual("N/A", order3.Description);
        Assert.AreEqual(30.0, order3.Total);
    }
}
