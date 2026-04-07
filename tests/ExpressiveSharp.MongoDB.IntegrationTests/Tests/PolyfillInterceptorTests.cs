using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies that delegate-based lambdas (modern C# syntax) are rewritten to
/// expression trees by the source generator and correctly forwarded through
/// the MongoDB provider.
/// </summary>
[TestClass]
public class PolyfillInterceptorTests : MongoTestBase
{
    [TestMethod]
    public async Task Where_DelegateLambda_InterceptedAndTranslated()
    {
        // Delegate lambda with null-conditional — should be intercepted by source generator
        var results = await Query
            .Where(o => o.Price > 50)
            .Select(o => o.Id)
            .ToListAsync();

        // Orders with Price > 50: 1 (120), 2 (75), 4 (50 is not > 50)
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, results);
    }

    [TestMethod]
    public async Task Select_DelegateLambda_InterceptedAndTranslated()
    {
        var results = await Query
            .Select(o => o.Price * 2)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 240.0, 150.0, 20.0, 100.0 }, results);
    }

    [TestMethod]
    public async Task OrderBy_DelegateLambda_InterceptedAndTranslated()
    {
        var results = await Query
            .OrderBy(o => o.Price)
            .Select(o => o.Id)
            .ToListAsync();

        // Prices: 10(3), 50(4), 75(2), 120(1)
        CollectionAssert.AreEqual(new[] { 3, 4, 2, 1 }, results);
    }

    [TestMethod]
    public async Task NullConditional_InDelegateLambda_TranslatesToMongo()
    {
        // This uses ?. which requires source generator interception
        var results = await Query
            .Where(o => o.Tag?.Length > 0)
            .Select(o => o.Tag)
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        CollectionAssert.AreEquivalent(new[] { "RUSH", "STD", "SPECIAL" }, results);
    }

    [TestMethod]
    public async Task Compound_Where_Select_OrderBy_Works()
    {
        var results = await Query
            .Where(o => o.Status == OrderStatus.Pending)
            .OrderBy(o => o.Price)
            .Select(o => o.Id)
            .ToListAsync();

        // Pending orders: 2 (75), 4 (50) → sorted by price: 4, 2
        CollectionAssert.AreEqual(new[] { 4, 2 }, results);
    }
}
