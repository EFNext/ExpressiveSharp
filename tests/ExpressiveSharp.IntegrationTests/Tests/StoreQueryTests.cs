using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

/// <summary>
/// Store-domain queries that combine multiple ExpressiveSharp features in a
/// single query, compiled to delegates and run in-memory.
/// </summary>
[TestClass]
public class StoreQueryTests
{
    private readonly ExpressionCompileRunner _runner = new();

    [TestInitialize]
    public void Seed()
    {
        _runner.Seed(SeedData.Addresses, SeedData.Customers, SeedData.Orders, SeedData.LineItems);
    }

    [TestMethod]
    public void FilterByCustomerName_ProjectTotal()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expandedName = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expandedName.Parameters[0];
        var predicate = Expression.Lambda<Func<Order, bool>>(
            Expression.Equal(expandedName.Body, Expression.Constant("Bob", typeof(string))),
            param);

        var filtered = _runner.Where(predicate);
        Assert.AreEqual(1, filtered.Count);
        Assert.AreEqual(2, filtered[0].Id);

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var totals = _runner.Select<Order, double>(expandedTotal);
        var bobTotal = totals.ElementAt(1); // Order 2 is Bob's
        Assert.AreEqual(1500.0, bobTotal);
    }

    [TestMethod]
    public void OrderByGrade_SelectTotal()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var results = _runner.OrderBySelect<Order, string, double>(expandedGrade, expandedTotal);

        // Alphabetical: Budget(Order 3: 30), Premium(Order 1: 240), Standard(Order 2: 1500, Order 4: 250)
        Assert.AreEqual(30.0, results[0]);
        Assert.AreEqual(240.0, results[1]);
    }

    [TestMethod]
    public void ProjectToOrderDto_ForAllOrders()
    {
        Expression<Func<Order, OrderDto>> expr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
        var expanded = (Expression<Func<Order, OrderDto>>)expr.ExpandExpressives();

        var results = _runner.Select<Order, OrderDto>(expanded);

        Assert.AreEqual(4, results.Count);

        var rush = results.Single(r => r.Id == 1);
        Assert.AreEqual("RUSH", rush.Description);
        Assert.AreEqual(240.0, rush.Total);

        var noTag = results.Single(r => r.Id == 3);
        Assert.AreEqual("N/A", noTag.Description);
        Assert.AreEqual(30.0, noTag.Total);
    }

    [TestMethod]
    public void FilterByStatus_SelectCategory()
    {
        Expression<Func<Order, string>> categoryExpr = o => o.GetCategory();
        var expandedCategory = (Expression<Func<Order, string>>)categoryExpr.ExpandExpressives();

        Expression<Func<Order, bool>> predicate = o => o.Status == OrderStatus.Pending;

        var filtered = _runner.Where(predicate);
        Assert.AreEqual(2, filtered.Count);

        var categories = _runner.Select<Order, string>(expandedCategory);
        Assert.IsTrue(categories.Contains("Bulk"));
        Assert.IsTrue(categories.Contains("Regular"));
    }

    [TestMethod]
    public void OrderByGrade_ThenByTotal_CompoundSort()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = _runner.OrderByThenBySelect<Order, string, double, int>(
            expandedGrade, expandedTotal, idExpr);

        // Budget(30): [3], Premium(240): [1], Standard(250,1500): [4,2]
        Assert.AreEqual(3, results[0]);
        Assert.AreEqual(1, results[1]);
        Assert.AreEqual(4, results[2]);
        Assert.AreEqual(2, results[3]);
    }

    [TestMethod]
    public void GroupByStatus_CountOrders()
    {
        Expression<Func<Order, OrderStatus>> statusExpr = o => o.Status;

        var results = _runner.GroupByCount<Order, OrderStatus>(statusExpr);

        var dict = results.ToDictionary(kv => kv.Key, kv => kv.Value);
        Assert.AreEqual(2, dict[OrderStatus.Pending]);
        Assert.AreEqual(1, dict[OrderStatus.Approved]);
        Assert.AreEqual(1, dict[OrderStatus.Rejected]);
    }
}
