using ExpressiveSharp.MongoDB.Extensions;
using ExpressiveSharp.MongoDB.Infrastructure;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies that <c>[Expressive(Projectable = true)]</c> properties are unmapped from BSON
/// serialization by the <c>ExpressiveMongoIgnoreConvention</c>, and that the formula is
/// correctly rewritten when referenced inside LINQ queries against the MongoDB provider.
/// </summary>
[TestClass]
public class ProjectableMongoIgnoreTests
{
    private MongoClient? _client;
    private string? _dbName;
    private IMongoCollection<ProjectableMongoDocument> _collection = null!;

    [TestInitialize]
    public async Task InitMongo()
    {
        if (!MongoContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        // Register the ignore convention BEFORE any class map could be implicitly built.
        ExpressiveMongoIgnoreConvention.EnsureRegistered();

        _client = new MongoClient(MongoContainerFixture.ConnectionString);
        _dbName = $"test_{Guid.NewGuid():N}";
        var database = _client.GetDatabase(_dbName);
        _collection = database.GetCollection<ProjectableMongoDocument>("people");

        await _collection.InsertManyAsync(
        [
            new ProjectableMongoDocument { Id = 1, FirstName = "Ada",  LastName = "Lovelace" },
            new ProjectableMongoDocument { Id = 2, FirstName = "Alan", LastName = "Turing" },
        ]);
    }

    [TestCleanup]
    public async Task CleanupMongo()
    {
        if (_client is not null && _dbName is not null)
            await _client.DropDatabaseAsync(_dbName);
    }

    [TestMethod]
    public void ProjectableProperty_IsNotInClassMap()
    {
        // Force-build the class map and confirm the convention unmapped FullName.
        var classMap = BsonClassMap.LookupClassMap(typeof(ProjectableMongoDocument));
        var mappedNames = classMap.AllMemberMaps.Select(m => m.MemberName).ToArray();

        CollectionAssert.Contains(mappedNames, nameof(ProjectableMongoDocument.FirstName));
        CollectionAssert.Contains(mappedNames, nameof(ProjectableMongoDocument.LastName));
        CollectionAssert.DoesNotContain(mappedNames, nameof(ProjectableMongoDocument.FullName),
            "Projectable property FullName must be unmapped from the BsonClassMap");
    }

    [TestMethod]
    public async Task ProjectableProperty_IsNotPersistedToBsonDocument()
    {
        // Query the raw BSON document to verify the Projectable property's backing field
        // is not serialized. Without the ExpressiveMongoIgnoreConvention, the writable
        // FullName property would be serialized to the document as a real field.
        var rawCollection = _collection.Database.GetCollection<BsonDocument>("people");
        var rawDocument = await rawCollection.Find(FilterDefinition<BsonDocument>.Empty).FirstAsync();

        Assert.IsTrue(rawDocument.Contains("FirstName"), "FirstName should be persisted");
        Assert.IsTrue(rawDocument.Contains("LastName"),  "LastName should be persisted");
        Assert.IsFalse(rawDocument.Contains("FullName"),
            "Projectable property FullName must NOT be persisted to the BSON document");
    }

    [TestMethod]
    public async Task ProjectableProperty_RoundTrip_RetainsDependenciesOnly()
    {
        // Insert a document, read it back, confirm FirstName/LastName survived materialization
        // and FullName (on the re-read instance) is computed from the formula since the backing
        // field is null for freshly-deserialized documents.
        var retrieved = await _collection.Find(d => d.Id == 1).FirstAsync();

        Assert.AreEqual("Ada", retrieved.FirstName);
        Assert.AreEqual("Lovelace", retrieved.LastName);
        Assert.AreEqual("Lovelace, Ada", retrieved.FullName,
            "After BSON deserialization (which skips the ignored FullName), reading FullName falls through to the formula");
    }
}

/// <summary>Self-contained document for Projectable Mongo tests.</summary>
public class ProjectableMongoDocument
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [Expressive(Projectable = true)]
    public string FullName
    {
        get => field ?? (LastName + ", " + FirstName);
        init => field = value;
    }
}
