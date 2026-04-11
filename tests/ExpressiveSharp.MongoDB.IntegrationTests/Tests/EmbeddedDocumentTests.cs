using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies expressive features work with MongoDB's embedded document model.
/// </summary>
[TestClass]
public class EmbeddedDocumentTests : MongoTestBase
{
    [TestMethod]
    public async Task ExpressiveMember_OnEmbeddedDocument_Expands()
    {
        // CustomerName is [Expressive] => Customer?.Name
        // Customer is an embedded document in MongoDB
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Query
            .Where(o => o.Customer != null)
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        Assert.AreEqual(3, results.Count);
        Assert.AreEqual("Alice", results[0]);
        Assert.AreEqual("Bob", results[1]);
        Assert.IsNull(results[2]); // Customer exists but Name is null
    }

    [TestMethod]
    public async Task NullConditional_ThroughEmbeddedDocument_Works()
    {
        // CustomerCountry is [Expressive] => Customer?.Address?.Country
        // Two levels of embedded document navigation
        Expression<Func<Order, string?>> expr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Query
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        Assert.AreEqual("US", results[0]);  // Order 1: Alice → New York, US
        Assert.AreEqual("UK", results[1]);  // Order 2: Bob → London, UK
        Assert.IsNull(results[2]);          // Order 3: no customer
        Assert.IsNull(results[3]);          // Order 4: customer has no address
    }

    [TestMethod]
    public async Task Query_EmbeddedCustomer_ByName()
    {
        var results = await Query
            .Where(o => o.Customer != null && o.Customer.Name == "Alice")
            .Select(o => o.Id)
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0]);
    }

    [TestMethod]
    public async Task Query_EmbeddedAddress_ByCity()
    {
        var results = await Query
            .Where(o => o.Customer != null && o.Customer.Address != null && o.Customer.Address.City == "London")
            .Select(o => o.Id)
            .ToListAsync();

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(2, results[0]);
    }
}
