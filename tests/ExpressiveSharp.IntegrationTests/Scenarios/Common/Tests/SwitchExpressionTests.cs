using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class SwitchExpressionTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_GetGrade_ReturnsCorrectValues()
    {
        Expression<Func<Order, string>> expr = o => o.GetGrade();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string>(expanded);

        // Order 1: 120 >= 100 → Premium, Order 2: 75 >= 50 → Standard,
        // Order 3: 10 < 50 → Budget, Order 4: 50 >= 50 → Standard
        CollectionAssert.AreEquivalent(
            new[] { "Premium", "Standard", "Budget", "Standard" },
            results);
    }

    [TestMethod]
    public async Task OrderBy_GetGrade_ReturnsSorted()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderBySelectAsync<Order, string, int>(expandedGrade, idExpr);

        // Alphabetical: Budget(3), Premium(1), Standard(2,4)
        Assert.AreEqual(3, results[0]);
        Assert.AreEqual(1, results[1]);
    }

    [TestMethod]
    public async Task OrderByDescending_GetGrade_ReturnsSortedDescending()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderByDescendingSelectAsync<Order, string, int>(expandedGrade, idExpr);

        // Reverse alphabetical: Standard(2,4), Premium(1), Budget(3)
        Assert.AreEqual(3, results.Last());
        Assert.AreEqual(1, results[^2]);
    }

    [TestMethod]
    public async Task GroupBy_GetGrade_CountsCorrectly()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        var results = await Runner.GroupByCountAsync<Order, string>(expandedGrade);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual(1, dict["Premium"]);
        Assert.AreEqual(2, dict["Standard"]);
        Assert.AreEqual(1, dict["Budget"]);
    }
}
