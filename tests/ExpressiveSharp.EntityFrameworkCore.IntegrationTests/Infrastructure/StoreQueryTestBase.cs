using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

public abstract class StoreQueryTestBase : EFCoreTestBase
{
    [TestInitialize]
    public virtual Task SeedStoreData() => Context.SeedStoreAsync();

    [TestMethod]
    public async Task FilterByCustomerName_ProjectTotal()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expandedName = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expandedName.Parameters[0];
        var predicate = Expression.Lambda<Func<Order, bool>>(
            Expression.Equal(expandedName.Body, Expression.Constant("Bob", typeof(string))),
            param);

        var filtered = await Context.Set<Order>().Where(predicate).ToListAsync();
        Assert.AreEqual(1, filtered.Count);
        Assert.AreEqual(2, filtered[0].Id);

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var totals = await Context.Set<Order>().Select(expandedTotal).ToListAsync();
        var bobTotal = totals.ElementAt(1); // Order 2 is Bob's
        Assert.AreEqual(1500.0, bobTotal);
    }

    [TestMethod]
    public async Task OrderByGrade_SelectTotal()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .OrderBy(expandedGrade)
            .Select(expandedTotal)
            .ToListAsync();

        // Alphabetical: Budget(Order 3: 30), Premium(Order 1: 240), Standard(Order 2: 1500, Order 4: 250)
        Assert.AreEqual(30.0, results[0]);
        Assert.AreEqual(240.0, results[1]);
    }

    [TestMethod]
    public async Task ProjectToOrderDto_ForAllOrders()
    {
        Expression<Func<Order, OrderDto>> expr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
        var expanded = (Expression<Func<Order, OrderDto>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>().Select(expanded).ToListAsync();

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
        Expression<Func<Order, string>> categoryExpr = o => o.GetCategory();
        var expandedCategory = (Expression<Func<Order, string>>)categoryExpr.ExpandExpressives();

        Expression<Func<Order, bool>> predicate = o => o.Status == OrderStatus.Pending;

        var filtered = await Context.Set<Order>().Where(predicate).ToListAsync();
        Assert.AreEqual(2, filtered.Count);

        var categories = await Context.Set<Order>().Select(expandedCategory).ToListAsync();
        Assert.IsTrue(categories.Contains("Bulk"));
        Assert.IsTrue(categories.Contains("Regular"));
    }

    [TestMethod]
    public async Task OrderByGrade_ThenByTotal_CompoundSort()
    {
        Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
        var expandedGrade = (Expression<Func<Order, string>>)gradeExpr.ExpandExpressives();

        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Context.Set<Order>()
            .OrderBy(expandedGrade)
            .ThenBy(expandedTotal)
            .Select(idExpr)
            .ToListAsync();

        // Budget(30): [3], Premium(240): [1], Standard(250,1500): [4,2]
        Assert.AreEqual(3, results[0]);
        Assert.AreEqual(1, results[1]);
        Assert.AreEqual(4, results[2]);
        Assert.AreEqual(2, results[3]);
    }

    [TestMethod]
    public virtual async Task GroupByStatus_CountOrders()
    {
        Expression<Func<Order, OrderStatus>> statusExpr = o => o.Status;

        var results = await Context.Set<Order>()
            .GroupBy(statusExpr)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToListAsync();

        var dict = results.ToDictionary(r => r.Key, r => r.Count);
        Assert.AreEqual(2, dict[OrderStatus.Pending]);
        Assert.AreEqual(1, dict[OrderStatus.Approved]);
        Assert.AreEqual(1, dict[OrderStatus.Rejected]);
    }

    [TestMethod]
    public async Task NullConditional_WithExpressiveFor_ComposedInProjection()
    {
        // Combines [ExpressiveFor] on PricingUtils.Clamp with [Expressive]
        // null-conditional CustomerName.
        Expression<Func<Order, string>> expr = o =>
            PricingUtils.Clamp(o.Price, 20.0, 100.0).ToString() + ":" + (o.CustomerName ?? "NONE");

        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        Assert.AreEqual(4, results.Count);
        // Order 1: Price=120→clamp=100, Customer=Alice            → "100:Alice"
        // Order 2: Price=75→clamp=75,  Customer=Bob               → "75:Bob"
        // Order 3: Price=10→clamp=20,  Customer=null              → "20:NONE"
        // Order 4: Price=50→clamp=50,  Customer with null name    → "50:NONE"
        Assert.IsTrue(results[0].StartsWith("100") && results[0].EndsWith(":Alice"));
        Assert.IsTrue(results[1].StartsWith("75") && results[1].EndsWith(":Bob"));
        Assert.IsTrue(results[2].StartsWith("20") && results[2].EndsWith(":NONE"));
        Assert.IsTrue(results[3].StartsWith("50") && results[3].EndsWith(":NONE"));
    }

    [TestMethod]
    public async Task CapturedVariable_PlusExpressive_Total_FiltersCorrectly()
    {
        // Both rewrite mechanisms (interceptor + query compiler) must cooperate
        // in a single query.
        var minTotal = 200.0;
        Expression<Func<Order, bool>> baseExpr = o => o.Total > minTotal;
        var expanded = (Expression<Func<Order, bool>>)baseExpr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .Where(expanded)
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        // Totals: 240, 1500, 30, 250 → > 200 keeps 1, 2, 4
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task ExpressiveFor_Inside_ExpressiveMember_ComposesCorrectly()
    {
        // Verifies the resolver handles [ExpressiveFor] targets and [Expressive]
        // members in the same expression tree.
        Expression<Func<Order, double>> expr = o =>
            PricingUtils.Clamp(o.Total, 0.0, 200.0);

        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Context.Set<Order>()
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Totals: 240, 1500, 30, 250 → clamped to [0, 200]: 200, 200, 30, 200
        CollectionAssert.AreEqual(new[] { 200.0, 200.0, 30.0, 200.0 }, results);
    }

    [TestMethod]
    public async Task VirtualExpressiveMember_FiltersInDatabase()
    {
        // Regression for the reverted "bad commit": a virtual [Expressive] member must expand
        // (using its static/declared body) and translate to SQL. The polymorphism gate used to
        // skip expansion, so LineItem.IsExpensive reached the provider untranslated and threw.
        // Compare against the database's own non-expressive predicate so the assertion is
        // independent of the seed details.
        Expression<Func<LineItem, bool>> expr = li => li.IsExpensive;
        var expanded = (Expression<Func<LineItem, bool>>)expr.ExpandExpressives();

        var expected = await Context.Set<LineItem>().Where(li => li.UnitPrice > 40).CountAsync();
        Assert.IsTrue(expected > 0, "Seed should contain at least one expensive line item.");

        var actual = await Context.Set<LineItem>().Where(expanded).CountAsync();

        Assert.AreEqual(expected, actual);
    }
}
