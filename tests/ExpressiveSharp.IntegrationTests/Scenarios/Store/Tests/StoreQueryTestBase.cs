using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Tests;

public abstract class StoreQueryTestBase : StoreTestBase
{
    [TestMethod]
    public async Task FilterByCustomerName_ProjectTotal()
    {
        // Combines null-conditional (CustomerName) + arithmetic (Total)
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expandedName = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expandedName.Parameters[0];
        var predicate = Expression.Lambda<Func<Order, bool>>(
            Expression.Equal(expandedName.Body, Expression.Constant("Bob", typeof(string))),
            param);

        var filtered = await Runner.WhereAsync(predicate);
        Assert.AreEqual(1, filtered.Count);
        Assert.AreEqual(2, filtered[0].Id);

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var totals = await Runner.SelectAsync<Order, double>(expandedTotal);
        var bobTotal = totals.ElementAt(1); // Order 2 is Bob's
        Assert.AreEqual(1500.0, bobTotal);
    }

    [TestMethod]
    public async Task OrderByGrade_SelectTotal()
    {
        // Combines switch expression (GetGrade) + arithmetic (Total)
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var results = await Runner.OrderBySelectAsync<Order, string, double>(
            expandedGrade, expandedTotal);

        // Alphabetical: Budget(Order 3: 30), Premium(Order 1: 240), Standard(Order 2: 1500, Order 4: 250)
        Assert.AreEqual(30.0, results[0]);
        Assert.AreEqual(240.0, results[1]);
    }

    [TestMethod]
    public async Task ProjectToOrderDto_ForAllOrders()
    {
        // Constructor projection with computed Total
        Expression<Func<Order, OrderDto>> expr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
        var expanded = (Expression<Func<Order, OrderDto>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, OrderDto>(expanded);

        Assert.AreEqual(4, results.Count);

        var rush = results.Single(r => r.Id == 1);
        Assert.AreEqual("RUSH", rush.Description);
        Assert.AreEqual(240.0, rush.Total);

        var noTag = results.Single(r => r.Id == 3);
        Assert.AreEqual("N/A", noTag.Description);
        Assert.AreEqual(30.0, noTag.Total);
    }

    [TestMethod]
    public async Task FilterByStatus_SelectCategory()
    {
        // Enum expansion (StatusDescription) + block body (GetCategory)
        Expression<Func<Order, string>> categoryExpr = o => o.GetCategory();
        var expandedCategory = (Expression<Func<Order, string>>)categoryExpr.ExpandExpressives();

        // Filter to Pending orders only (Orders 2 and 4)
        Expression<Func<Order, bool>> predicate = o => o.Status == OrderStatus.Pending;

        var filtered = await Runner.WhereAsync(predicate);
        Assert.AreEqual(2, filtered.Count);

        var categories = await Runner.SelectAsync<Order, string>(expandedCategory);
        // Order 2: Bulk, Order 4: Regular (all 4 orders projected, but we verified filter works)
        Assert.IsTrue(categories.Contains("Bulk"));
        Assert.IsTrue(categories.Contains("Regular"));
    }

    [TestMethod]
    public async Task OrderByGrade_ThenByTotal_CompoundSort()
    {
        // Compound sort: primary by GetGrade (switch), secondary by Total (arithmetic)
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderByThenBySelectAsync<Order, string, double, int>(
            expandedGrade, expandedTotal, idExpr);

        // Budget(30): [3], Premium(240): [1], Standard(250,1500): [4,2]
        Assert.AreEqual(3, results[0]);
        Assert.AreEqual(1, results[1]);
        Assert.AreEqual(4, results[2]);
        Assert.AreEqual(2, results[3]);
    }

    [TestMethod]
    public async Task GroupByStatus_CountOrders()
    {
        // GroupBy on a plain enum column + count
        Expression<Func<Order, OrderStatus>> statusExpr = o => o.Status;

        var results = await Runner.GroupByCountAsync<Order, OrderStatus>(statusExpr);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(2, dict[OrderStatus.Pending]);   // Orders 2, 4
        Assert.AreEqual(1, dict[OrderStatus.Approved]);   // Order 1
        Assert.AreEqual(1, dict[OrderStatus.Rejected]);   // Order 3
    }
}
