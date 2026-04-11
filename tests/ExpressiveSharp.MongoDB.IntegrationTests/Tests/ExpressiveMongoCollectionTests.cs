using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies the <see cref="ExpressiveMongoCollection{TDocument}"/> high-level wrapper
/// correctly provides queryable access and delegates CRUD operations.
/// </summary>
[TestClass]
public class ExpressiveMongoCollectionTests : MongoTestBase
{
    [TestMethod]
    public void AsQueryable_ReturnsExpressiveMongoQueryable()
    {
        var wrapper = new ExpressiveMongoCollection<Order>(Orders);
        var queryable = wrapper.AsQueryable();
        Assert.IsInstanceOfType<IExpressiveMongoQueryable<Order>>(queryable);
    }

    [TestMethod]
    public async Task CrudOperations_DelegateToInner()
    {
        var wrapper = new ExpressiveMongoCollection<Order>(Orders);

        // Insert via inner
        var newOrder = new Order { Id = 100, Tag = "TEST", Price = 99.0, Quantity = 1, Status = OrderStatus.Approved };
        await wrapper.Inner.InsertOneAsync(newOrder);

        // Query via wrapper
        var results = await MongoQueryable.ToListAsync(
            wrapper.AsQueryable().Where(o => o.Id == 100));
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("TEST", results[0].Tag);

        // Delete via inner
        await wrapper.Inner.DeleteOneAsync(Builders<Order>.Filter.Eq(o => o.Id, 100));
        var count = await MongoQueryable.CountAsync(
            wrapper.AsQueryable().Where(o => o.Id == 100));
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public async Task AsQueryable_WithExpressiveExpansion_Works()
    {
        var wrapper = new ExpressiveMongoCollection<Order>(Orders);

        // End-to-end: collection → queryable → Where(computed property) → ToListAsync
        var results = await MongoQueryable.ToListAsync(
            wrapper.AsQueryable()
                .Where(o => o.Price > 50)
                .OrderBy(o => o.Id));

        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].Id);
        Assert.AreEqual(2, results[1].Id);
    }
}
