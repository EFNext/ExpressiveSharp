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

    [TestInitialize]
    public override async Task SeedStoreData()
    {
        // Cosmos models Customer/Address as owned types embedded in Order.
        // Seed by materializing the embedded graph rather than inserting
        // separate Customer/Address entities.
        //
        // Each order is saved individually so that transient-error retries
        // are idempotent — a batch SaveChangesAsync can partially commit
        // (Cosmos has no cross-partition transactions), and retrying the
        // whole batch would cause 409 Conflict for already-saved documents.
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
            await RetryCosmosTransientAsync(() => Context.SaveChangesAsync());
        }
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

    // Cosmos DB cannot translate implicit numeric type conversions (int → double)
    // in Where/OrderBy clauses. The expanded expression for Total contains
    // Expression.Convert(Quantity, typeof(double)) which the Cosmos provider
    // cannot translate — this is standard C# expression tree representation
    // that the provider simply doesn't support.
    public override Task Where_TotalGreaterThan100_FiltersCorrectly()
    {
        Assert.Inconclusive("Cosmos DB cannot translate implicit numeric type conversions");
        return Task.CompletedTask;
    }

    public override Task Where_NoMatch_ReturnsEmpty()
    {
        Assert.Inconclusive("Cosmos DB cannot translate implicit numeric type conversions");
        return Task.CompletedTask;
    }

    public override Task OrderByDescending_Total_ReturnsSortedDescending()
    {
        Assert.Inconclusive("Cosmos DB cannot translate implicit numeric type conversions");
        return Task.CompletedTask;
    }

    public override Task Where_CheckedTotalGreaterThan100_FiltersCorrectly()
    {
        Assert.Inconclusive("Cosmos DB cannot translate implicit numeric type conversions");
        return Task.CompletedTask;
    }

    // Cosmos DB cannot translate LINQ subqueries on owned collections.
    // Loop-based [Expressive] members (ItemCount, ItemTotal, etc.) are
    // transformed into Queryable.Count/Sum/Any/All, but the Cosmos provider
    // does not support subqueries over owned collection navigations.
    public override Task Select_ItemCount_ReturnsCorrectCounts()
    {
        Assert.Inconclusive("Cosmos DB does not support LINQ subqueries on owned collections");
        return Task.CompletedTask;
    }

    public override Task Select_ItemTotal_ReturnsCorrectTotals()
    {
        Assert.Inconclusive("Cosmos DB does not support LINQ subqueries on owned collections");
        return Task.CompletedTask;
    }

    public override Task Select_HasExpensiveItems_ReturnsCorrectFlags()
    {
        Assert.Inconclusive("Cosmos DB does not support LINQ subqueries on owned collections");
        return Task.CompletedTask;
    }

    public override Task Select_AllItemsAffordable_ReturnsCorrectFlags()
    {
        Assert.Inconclusive("Cosmos DB does not support LINQ subqueries on owned collections");
        return Task.CompletedTask;
    }

    public override Task Select_ItemTotalForExpensive_ReturnsCorrectTotals()
    {
        Assert.Inconclusive("Cosmos DB does not support LINQ subqueries on owned collections");
        return Task.CompletedTask;
    }

    // Cosmos DB projects owned entities differently — projecting an owned
    // entity without its owner requires AsNoTracking
    public override Task Polyfill_NullConditional_ProjectsCorrectly()
    {
        Assert.Inconclusive("Cosmos DB cannot project owned entities without their owner");
        return Task.CompletedTask;
    }

    // Cosmos DB does not support ORDER BY on computed expressions
    // (only simple document paths are allowed)
    public override Task OrderBy_TagLength_NullsAppearFirst()
    {
        Assert.Inconclusive("Cosmos DB does not support ORDER BY on computed expressions");
        return Task.CompletedTask;
    }

    public override Task OrderBy_GetGrade_ReturnsSorted()
    {
        Assert.Inconclusive("Cosmos DB does not support ORDER BY on computed expressions");
        return Task.CompletedTask;
    }

    public override Task OrderByDescending_GetGrade_ReturnsSortedDescending()
    {
        Assert.Inconclusive("Cosmos DB does not support ORDER BY on computed expressions");
        return Task.CompletedTask;
    }

    // Cosmos DB does not translate int.ToString() in Where clauses
    public override Task Where_Summary_TranslatesToSql()
    {
        Assert.Inconclusive("Cosmos DB does not translate int.ToString()");
        return Task.CompletedTask;
    }

    public override Task Where_DetailedSummary_ConcatArrayTranslatesToSql()
    {
        Assert.Inconclusive("Cosmos DB does not translate int.ToString()");
        return Task.CompletedTask;
    }

    // Cosmos DB has different null equality semantics for owned types
    public override Task Where_CustomerNameIsNull_FiltersCorrectly()
    {
        Assert.Inconclusive("Cosmos DB has different null semantics for owned type properties");
        return Task.CompletedTask;
    }

    // EF Core 9+ Cosmos provider drops rows from projections when
    // null-conditional chains evaluate to null (returns fewer rows
    // instead of including null values in the result set).
    // EF Core 8 handles this correctly.
#if !NET8_0
    public override Task Select_CustomerName_ReturnsCorrectNullableValues()
    {
        Assert.Inconclusive("EF Core 9+ Cosmos provider drops null rows in null-conditional projections");
        return Task.CompletedTask;
    }

    public override Task Select_TagLength_ReturnsCorrectNullableValues()
    {
        Assert.Inconclusive("EF Core 9+ Cosmos provider drops null rows in null-conditional projections");
        return Task.CompletedTask;
    }

    public override Task Select_CustomerCountry_TwoLevelChain()
    {
        Assert.Inconclusive("EF Core 9+ Cosmos provider drops null rows in null-conditional projections");
        return Task.CompletedTask;
    }
#endif
}
#endif
