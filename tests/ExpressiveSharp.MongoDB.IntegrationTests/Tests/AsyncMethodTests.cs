using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies MongoDB-specific async methods work through delegate-based stubs
/// with <c>[PolyfillTarget(typeof(MongoQueryable))]</c>.
/// </summary>
[TestClass]
public class AsyncMethodTests : MongoTestBase
{
    [TestMethod]
    public async Task AnyAsync_WithDelegatePredicate_Executes()
    {
        var result = await MongoQueryable.AnyAsync(
            Query.Where(o => o.Price > 100));
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CountAsync_WithDelegatePredicate_Executes()
    {
        var result = await MongoQueryable.CountAsync(
            Query.Where(o => o.Status == OrderStatus.Pending));
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public async Task FirstAsync_WithDelegatePredicate_Executes()
    {
        var result = await MongoQueryable.FirstAsync(
            Query.Where(o => o.Id == 1));
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual(120.0, result.Price);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_WithDelegatePredicate_ReturnsNull()
    {
        var result = await MongoQueryable.FirstOrDefaultAsync(
            Query.Where(o => o.Id == 999));
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SingleAsync_WithDelegatePredicate_Executes()
    {
        var result = await MongoQueryable.SingleAsync(
            Query.Where(o => o.Id == 2));
        Assert.AreEqual(2, result.Id);
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_WithDelegatePredicate_ReturnsNull()
    {
        var result = await MongoQueryable.SingleOrDefaultAsync(
            Query.Where(o => o.Id == 999));
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SumAsync_WithSelector_Executes()
    {
        var result = await MongoQueryable.SumAsync(
            Query.Select(o => (int)o.Price));
        // 120 + 75 + 10 + 50 = 255
        Assert.AreEqual(255, result);
    }

    [TestMethod]
    public async Task MinAsync_WithSelector_Executes()
    {
        var result = await MongoQueryable.MinAsync(
            Query.Select(o => o.Price));
        Assert.AreEqual(10.0, result);
    }

    [TestMethod]
    public async Task MaxAsync_WithSelector_Executes()
    {
        var result = await MongoQueryable.MaxAsync(
            Query.Select(o => o.Price));
        Assert.AreEqual(120.0, result);
    }

    [TestMethod]
    public async Task AverageAsync_WithSelector_Executes()
    {
        var result = await MongoQueryable.AverageAsync(
            Query.Select(o => o.Price));
        // (120 + 75 + 10 + 50) / 4 = 63.75
        Assert.AreEqual(63.75, result);
    }

    [TestMethod]
    public async Task ToListAsync_Works()
    {
        var results = await MongoQueryable.ToListAsync(Query);
        Assert.AreEqual(4, results.Count);
    }
}
