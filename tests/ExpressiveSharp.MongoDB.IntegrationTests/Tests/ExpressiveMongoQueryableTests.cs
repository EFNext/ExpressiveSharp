using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.Extensions;
using ExpressiveSharp.MongoDB.Infrastructure;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

[TestClass]
public class ExpressiveMongoQueryableTests : MongoTestBase
{
    [TestMethod]
    public void AsExpressive_ReturnsIExpressiveMongoQueryable()
    {
        var result = Orders.AsExpressive();
        Assert.IsInstanceOfType<IExpressiveMongoQueryable<Order>>(result);
    }

    [TestMethod]
    public void AsExpressive_FromCollection_CreatesQueryable()
    {
        var result = Orders.AsExpressive();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IExpressiveMongoQueryable<Order>>(result);
    }

    [TestMethod]
    public void Provider_IsExpressiveMongoQueryProvider()
    {
        var result = Orders.AsExpressive();
        Assert.IsInstanceOfType<ExpressiveMongoQueryProvider>(result.Provider);
    }

    [TestMethod]
    public void Expression_MatchesUnderlyingQueryable()
    {
        var baseline = Orders.AsQueryable();
        var expressive = Orders.AsExpressive();

        Assert.AreEqual(baseline.Expression.NodeType, expressive.Expression.NodeType);
    }

    [TestMethod]
    public async Task ChainedOperations_MaintainWrapperType()
    {
        var filtered = Query.Where(o => o.Price > 50);
        Assert.IsInstanceOfType<ExpressiveMongoQueryProvider>(filtered.Provider);

        var results = await MongoQueryable.ToListAsync(filtered);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task ToCursorAsync_WorksThroughWrapper()
    {
        using var cursor = await MongoQueryable.ToCursorAsync(Query);
        var results = new List<Order>();
        while (await cursor.MoveNextAsync())
        {
            results.AddRange(cursor.Current);
        }
        Assert.AreEqual(4, results.Count);
    }
}
