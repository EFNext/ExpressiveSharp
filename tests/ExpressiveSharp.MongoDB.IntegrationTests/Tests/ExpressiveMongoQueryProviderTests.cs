using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies that <see cref="Infrastructure.ExpressiveMongoQueryProvider"/> correctly
/// expands <c>[Expressive]</c> members before MongoDB's LINQ provider processes the query.
/// </summary>
[TestClass]
public class ExpressiveMongoQueryProviderTests : MongoTestBase
{
    [TestMethod]
    public async Task Select_ExpressiveMember_ExpandsBeforeMongoTranslation()
    {
        // Total is [Expressive] => Price * Quantity
        // Provider should expand before MongoDB sees it.
        // Use the collection-based AsExpressive so the ExpressiveMongoQueryProvider is
        // in the chain — otherwise the core wrapper forwards to MongoDB's raw provider,
        // which has no way to expand [Expressive] members on a lambda Select.
        var results = await Orders.AsExpressive()
            .Select(o => o.Total)
            .ToListAsync();

        // Order 1: 120*2=240, Order 2: 75*20=1500, Order 3: 10*3=30, Order 4: 50*5=250
        CollectionAssert.AreEquivalent(new[] { 240.0, 1500.0, 30.0, 250.0 }, results);
    }

    [TestMethod]
    public async Task Where_ExpressiveMember_ExpandsInPredicate()
    {
        Expression<Func<Order, bool>> predicate = o => o.Total > 200;
        var expanded = (Expression<Func<Order, bool>>)predicate.ExpandExpressives();

        var results = await Orders.AsQueryable()
            .AsExpressive()
            .Where(expanded)
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        // Totals: 240, 1500, 30, 250 → >200 keeps orders 1, 2, 4
        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task OrderBy_ExpressiveMember_ExpandsInSort()
    {
        Expression<Func<Order, double>> totalExpr = o => o.Total;
        var expanded = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();

        var results = await Orders.AsQueryable()
            .AsExpressive()
            .OrderBy(expanded)
            .Select(o => o.Id)
            .ToListAsync();

        // Totals: 30(3), 240(1), 250(4), 1500(2)
        CollectionAssert.AreEqual(new[] { 3, 1, 4, 2 }, results);
    }

    [TestMethod]
    public async Task ExpressiveFor_StaticMethod_ExpandsCorrectly()
    {
        Expression<Func<Order, double>> expr = o => PricingUtils.Clamp(o.Price, 20.0, 100.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Orders.AsQueryable()
            .AsExpressive()
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Prices: 120→100, 75→75, 10→20, 50→50
        CollectionAssert.AreEqual(new[] { 100.0, 75.0, 20.0, 50.0 }, results);
    }

    [TestMethod]
    public async Task NestedExpressive_ExpandsRecursively()
    {
        // PricingUtils.Clamp is [ExpressiveFor], Total is [Expressive]
        Expression<Func<Order, double>> expr = o => PricingUtils.Clamp(o.Total, 0.0, 200.0);
        var expanded = (Expression<Func<Order, double>>)expr.ExpandExpressives();

        var results = await Orders.AsQueryable()
            .AsExpressive()
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Totals: 240→200, 1500→200, 30→30, 250→200
        CollectionAssert.AreEqual(new[] { 200.0, 200.0, 30.0, 200.0 }, results);
    }

    [TestMethod]
    public async Task CapturedVariable_WithExpressive_BothResolve()
    {
        var minTotal = 200.0;
        Expression<Func<Order, bool>> expr = o => o.Total > minTotal;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Orders.AsQueryable()
            .AsExpressive()
            .Where(expanded)
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, results);
    }
}
