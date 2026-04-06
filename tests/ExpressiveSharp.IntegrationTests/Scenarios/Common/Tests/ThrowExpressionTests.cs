using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class ThrowExpressionTests : StoreTestBase
{
    [TestMethod]
    public async Task Where_SafeTag_FiltersCorrectly()
    {
        Expression<Func<Order, string>> expr = o => o.SafeTag;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        // Build predicate: o => o.Tag != null && o.SafeTag == "RUSH"
        // AndAlso short-circuits so SafeTag is not evaluated for null-Tag orders
        var param = expanded.Parameters[0];
        var tagNotNull = Expression.NotEqual(
            Expression.Property(param, nameof(Order.Tag)),
            Expression.Constant(null, typeof(string)));
        var safeTagEquals = Expression.Equal(expanded.Body, Expression.Constant("RUSH"));
        var body = Expression.AndAlso(tagNotNull, safeTagEquals);
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public void Select_SafeTag_ThrowsForNullTag()
    {
        Expression<Func<Order, string>> expr = o => o.SafeTag;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var compiled = expanded.Compile();
        var order = new Order { Tag = null };
        Assert.ThrowsExactly<InvalidOperationException>(() => compiled(order));
    }
}
