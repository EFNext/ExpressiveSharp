#if TEST_COSMOS
using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Cosmos DB test runner. Uses <see cref="CosmosIntegrationTestDbContext"/> with
/// owned types (Customer/Address embedded in Order documents).
/// Seeds data by embedding related entities into the Order documents.
/// </summary>
public sealed class EFCoreCosmosTestRunner : EFCoreTestRunnerBase
{
    private new CosmosIntegrationTestDbContext Context => (CosmosIntegrationTestDbContext)base.Context;

    public EFCoreCosmosTestRunner(string connectionString, string databaseName, Action<string>? logSql = null)
        : base(CreateContext(connectionString, databaseName, logSql), logSql)
    {
        Context.Database.EnsureCreated();
    }

    private static CosmosIntegrationTestDbContext CreateContext(
        string connectionString, string databaseName, Action<string>? logSql)
    {
        var builder = new DbContextOptionsBuilder<CosmosIntegrationTestDbContext>()
            .UseCosmos(connectionString, databaseName)
            .UseExpressives();

        if (logSql is not null)
        {
            builder
                .LogTo(message => logSql(message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        return new CosmosIntegrationTestDbContext(builder.Options);
    }

    public override async Task SeedAsync(
        IReadOnlyList<Address> addresses,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Order> orders,
        IReadOnlyList<LineItem> lineItems)
    {
        // Build lookup dictionaries for embedding
        var addressLookup = addresses.ToDictionary(a => a.Id);
        var customerLookup = customers.ToDictionary(c => c.Id);
        var lineItemsByOrder = lineItems.GroupBy(li => li.OrderId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var order in orders)
        {
            var cosmosOrder = new Order
            {
                Id = order.Id,
                Tag = order.Tag,
                Price = order.Price,
                Quantity = order.Quantity,
                Status = order.Status,
                CustomerId = order.CustomerId,
            };

            // Embed customer as owned type
            if (order.CustomerId is { } customerId && customerLookup.TryGetValue(customerId, out var customer))
            {
                cosmosOrder.Customer = new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    AddressId = customer.AddressId,
                };

                // Embed address as nested owned type
                if (customer.AddressId is { } addressId && addressLookup.TryGetValue(addressId, out var address))
                {
                    cosmosOrder.Customer.Address = new Address
                    {
                        Id = address.Id,
                        City = address.City,
                        Country = address.Country,
                    };
                }
            }

            // Embed line items as owned collection
            if (lineItemsByOrder.TryGetValue(order.Id, out var items))
            {
                cosmosOrder.Items = items.Select(li => new LineItem
                {
                    Id = li.Id,
                    OrderId = li.OrderId,
                    ProductName = li.ProductName,
                    UnitPrice = li.UnitPrice,
                    Quantity = li.Quantity,
                }).ToList();
            }

            Context.Orders.Add(cosmosOrder);
        }

        await Context.SaveChangesAsync();
        EnableLogging();
    }

    public override async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
    }
}
#endif
