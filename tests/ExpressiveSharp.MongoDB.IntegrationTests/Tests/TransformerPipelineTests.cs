using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using ExpressiveSharp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies the default transformer pipeline produces expressions that
/// MongoDB's LINQ provider can translate.
/// </summary>
[TestClass]
public class TransformerPipelineTests : MongoTestBase
{
    [TestMethod]
    public async Task NullConditionalPatterns_FlattenedForMongo()
    {
        // CustomerName is [Expressive] => Customer?.Name
        // RemoveNullConditionalPatterns should flatten the null-conditional pattern
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Query
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        Assert.AreEqual("Alice", results[0]);
        Assert.AreEqual("Bob", results[1]);
        Assert.IsNull(results[2]); // Order 3: no customer
        Assert.IsNull(results[3]); // Order 4: customer with null name
    }

    [TestMethod]
    public async Task BlockExpressions_Flattened()
    {
        // GetCategory is [Expressive(AllowBlockBody = true)] with if/else
        Expression<Func<Order, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Query
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Order 1: Qty*10=20 → Regular, Order 2: Qty*10=200 → Bulk
        // Order 3: Qty*10=30 → Regular, Order 4: Qty*10=50 → Regular
        Assert.AreEqual("Regular", results[0]);
        Assert.AreEqual("Bulk", results[1]);
        Assert.AreEqual("Regular", results[2]);
        Assert.AreEqual("Regular", results[3]);
    }

    [TestMethod]
    public async Task ThrowExpressions_ReplacedWithDefault()
    {
        // SafeTag => Tag ?? throw ... — ReplaceThrowWithDefault should replace throw with default
        Expression<Func<Order, string>> expr = o => o.SafeTag;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Query
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        Assert.AreEqual("RUSH", results[0]);
        Assert.AreEqual("STD", results[1]);
        Assert.IsNull(results[2]); // Tag is null → throw replaced with default(string) = null
        Assert.AreEqual("SPECIAL", results[3]);
    }

    [TestMethod]
    public async Task CustomOptions_OverrideDefaults()
    {
        // Use custom options with no transformers — raw expansion only
        var customOptions = new ExpressiveOptions();
        var customQuery = MongoDB.Extensions.MongoExpressiveExtensions.AsExpressive(
            Orders, customOptions);

        // Simple query should still work (no transformers needed for basic arithmetic)
        var results = await customQuery
            .Select(o => o.Price * 2)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 240.0, 150.0, 20.0, 100.0 }, results);
    }
}
