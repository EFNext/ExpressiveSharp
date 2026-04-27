using ExpressiveSharp.Mapping;
using ExpressiveSharp.MongoDB.Infrastructure;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

[TestClass]
public class SynthesizedMongoIgnoreTests
{
    private MongoClient? _client;
    private string? _dbName;
    private IMongoCollection<SynthesizedMongoDocument> _collection = null!;

    [TestInitialize]
    public async Task InitMongo()
    {
        if (!MongoContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        // IMPORTANT: The ignore convention must be registered BEFORE the first call to
        // IMongoDatabase.GetCollection<T>(), which builds and caches the BSON class map for
        // T eagerly. A convention registered afterward would not apply to the cached map.
        ExpressiveMongoIgnoreConvention.EnsureRegistered();

        _client = new MongoClient(MongoContainerFixture.ConnectionString);
        _dbName = $"test_{Guid.NewGuid():N}";
        var database = _client.GetDatabase(_dbName);
        _collection = database.GetCollection<SynthesizedMongoDocument>("people");

        await _collection.InsertManyAsync(
        [
            new SynthesizedMongoDocument { Id = 1, FirstName = "Ada",  LastName = "Lovelace" },
            new SynthesizedMongoDocument { Id = 2, FirstName = "Alan", LastName = "Turing" },
        ]);
    }

    [TestCleanup]
    public async Task CleanupMongo()
    {
        if (_client is not null && _dbName is not null)
            await _client.DropDatabaseAsync(_dbName);
    }

    [TestMethod]
    public void SynthesizedProperty_IsNotInClassMap()
    {
        var classMap = BsonClassMap.LookupClassMap(typeof(SynthesizedMongoDocument));
        var mappedNames = classMap.AllMemberMaps.Select(m => m.MemberName).ToArray();

        CollectionAssert.Contains(mappedNames, nameof(SynthesizedMongoDocument.FirstName));
        CollectionAssert.Contains(mappedNames, nameof(SynthesizedMongoDocument.LastName));
        CollectionAssert.DoesNotContain(mappedNames, nameof(SynthesizedMongoDocument.FullName),
            "Synthesized property FullName must be unmapped from the BsonClassMap");
    }

    [TestMethod]
    public async Task SynthesizedProperty_IsNotPersistedToBsonDocument()
    {
        var rawCollection = _collection.Database.GetCollection<BsonDocument>("people");
        var rawDocument = await rawCollection.Find(FilterDefinition<BsonDocument>.Empty).FirstAsync();

        Assert.IsTrue(rawDocument.Contains("FirstName"), "FirstName should be persisted");
        Assert.IsTrue(rawDocument.Contains("LastName"),  "LastName should be persisted");
        Assert.IsFalse(rawDocument.Contains("FullName"),
            "Synthesized property FullName must NOT be persisted to the BSON document");
    }

    [TestMethod]
    public async Task SynthesizedProperty_RoundTrip_RetainsDependenciesOnly()
    {
        var retrieved = await _collection.Find(d => d.Id == 1).FirstAsync();

        Assert.AreEqual("Ada", retrieved.FirstName);
        Assert.AreEqual("Lovelace", retrieved.LastName);
        Assert.AreEqual("Lovelace, Ada", retrieved.FullName,
            "After BSON deserialization (which skips the ignored FullName), reading FullName falls through to the formula");
    }
}

public partial class SynthesizedMongoDocument
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [ExpressiveProperty("FullName")]
    private string FullNameExpression => LastName + ", " + FirstName;
}
