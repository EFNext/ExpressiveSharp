using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class ThrowExpressionTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_SafeTag_ReturnsValueForNonNullTags()
    {
        Expression<Func<Order, string>> expr = o => o.SafeTag;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        // Compile and invoke directly — non-null Tag should return the value
        var compiled = expanded.Compile();
        var order = new Order { Tag = "RUSH" };
        Assert.AreEqual("RUSH", compiled(order));
    }

    [TestMethod]
    public async Task Select_SafeTag_ThrowsForNullTag()
    {
        Expression<Func<Order, string>> expr = o => o.SafeTag;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var compiled = expanded.Compile();
        var order = new Order { Tag = null };
        Assert.ThrowsExactly<InvalidOperationException>(() => compiled(order));
    }
}
