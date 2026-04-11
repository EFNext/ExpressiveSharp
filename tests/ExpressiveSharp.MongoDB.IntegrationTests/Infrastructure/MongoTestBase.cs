using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for all MongoDB integration tests. Creates a unique database per test,
/// seeds embedded Order documents, and drops the database on cleanup.
/// </summary>
public abstract class MongoTestBase
{
    protected IMongoDatabase Database { get; private set; } = null!;
    protected IMongoCollection<Order> Orders { get; private set; } = null!;
    protected IExpressiveMongoQueryable<Order> Query { get; private set; } = null!;

    private MongoClient? _client;
    private string? _dbName;

    [TestInitialize]
    public async Task InitMongo()
    {
        if (!MongoContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        _client = new MongoClient(MongoContainerFixture.ConnectionString);
        _dbName = $"test_{Guid.NewGuid():N}";
        Database = _client.GetDatabase(_dbName);
        Orders = Database.GetCollection<Order>("orders");
        Query = Orders.AsExpressive();

        await SeedDataAsync();
    }

    [TestCleanup]
    public async Task CleanupMongo()
    {
        if (_client is not null && _dbName is not null)
            await _client.DropDatabaseAsync(_dbName);
    }

    /// <summary>
    /// Seeds the database with embedded Order documents.
    /// Customer and Address are embedded within each Order (document model).
    /// </summary>
    protected virtual async Task SeedDataAsync()
    {
        var addressLookup = SeedData.Addresses.ToDictionary(a => a.Id);
        var customerLookup = SeedData.Customers.ToDictionary(c => c.Id);
        var lineItemsByOrder = SeedData.LineItems
            .GroupBy(li => li.OrderId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var orders = new List<Order>();

        foreach (var order in SeedData.Orders)
        {
            var mongoOrder = new Order
            {
                Id = order.Id,
                Tag = order.Tag,
                Price = order.Price,
                Quantity = order.Quantity,
                Status = order.Status,
                CustomerId = order.CustomerId,
            };

            if (order.CustomerId is { } customerId && customerLookup.TryGetValue(customerId, out var customer))
            {
                mongoOrder.Customer = new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    AddressId = customer.AddressId,
                };

                if (customer.AddressId is { } addressId && addressLookup.TryGetValue(addressId, out var address))
                {
                    mongoOrder.Customer.Address = new Address
                    {
                        Id = address.Id,
                        City = address.City,
                        Country = address.Country,
                    };
                }
            }

            if (lineItemsByOrder.TryGetValue(order.Id, out var items))
            {
                mongoOrder.Items = items.Select(li => new LineItem
                {
                    Id = li.Id,
                    OrderId = li.OrderId,
                    ProductName = li.ProductName,
                    UnitPrice = li.UnitPrice,
                    Quantity = li.Quantity,
                }).ToList();
            }

            orders.Add(mongoOrder);
        }

        await Orders.InsertManyAsync(orders);
    }
}
