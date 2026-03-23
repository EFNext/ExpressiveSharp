using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class BlockBodyTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_GetCategory_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string>(expanded);

        // Order 1: 2*10=20, not > 100 → Regular
        // Order 2: 20*10=200, > 100 → Bulk
        // Order 3: 3*10=30, not > 100 → Regular
        // Order 4: 5*10=50, not > 100 → Regular
        CollectionAssert.AreEquivalent(
            new[] { "Regular", "Bulk", "Regular", "Regular" },
            results);
    }
}
