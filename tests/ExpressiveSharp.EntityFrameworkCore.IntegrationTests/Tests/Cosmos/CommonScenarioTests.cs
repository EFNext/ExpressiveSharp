#if TEST_COSMOS
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Cosmos;

/// <summary>
/// Cosmos DB runs a subset of <see cref="CommonScenarioTestBase"/>: tests that
/// use <c>GROUP BY</c> on computed expressions, cross-entity queries, or array
/// literal projections are not supported by Cosmos and are overridden as
/// <see cref="Assert.Inconclusive"/>. Seeding is also customized because
/// Customer and Address are owned types embedded in the Order document.
/// </summary>
[TestClass]
public class CommonScenarioTests : CommonScenarioTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreateCosmos();
        context = handle.Context;
        return handle;
    }

    public override async Task SeedStoreData()
    {
        // Cosmos models Customer/Address as owned types embedded in Order.
        // Seed by materializing the embedded graph rather than inserting
        // separate Customer/Address entities.
        var addressLookup = SeedData.Addresses.ToDictionary(a => a.Id);
        var customerLookup = SeedData.Customers.ToDictionary(c => c.Id);
        var lineItemsByOrder = SeedData.LineItems
            .GroupBy(li => li.OrderId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var order in SeedData.Orders)
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

            if (order.CustomerId is { } customerId && customerLookup.TryGetValue(customerId, out var customer))
            {
                cosmosOrder.Customer = new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    AddressId = customer.AddressId,
                };

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

            Context.Set<Order>().Add(cosmosOrder);
        }

        await Context.SaveChangesAsync();
    }

    // Cosmos DB does not support GROUP BY on computed expressions
    public override Task GroupBy_StatusDescription_CountsCorrectly()
    {
        Assert.Inconclusive("Cosmos DB does not support GROUP BY on computed expressions");
        return Task.CompletedTask;
    }

    public override Task GroupBy_GetGrade_CountsCorrectly()
    {
        Assert.Inconclusive("Cosmos DB does not support GROUP BY on computed expressions");
        return Task.CompletedTask;
    }

    // Customer is an owned type in Cosmos — cannot be queried as a root set
    public override Task Select_CustomerCity_ViaCustomerExpressive()
    {
        Assert.Inconclusive("Cosmos DB does not support queries on owned types as root sets");
        return Task.CompletedTask;
    }

    // Cosmos does not translate array literal projections
    public override Task Select_PriceBreakpoints_ReturnsArrayLiteral()
    {
        Assert.Inconclusive("Cosmos DB does not support array literal projection");
        return Task.CompletedTask;
    }
}
#endif
