using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Shared seed logic for tests that exercise the Store scenario graph
/// (Address → Customer → Order → LineItem). Every EF Core feature test base
/// that needs the canonical Store dataset calls <see cref="SeedStoreAsync"/>
/// rather than duplicating the four-entity insert ceremony.
///
/// Entities are re-materialized (not the originals from <see cref="SeedData"/>)
/// to avoid tracker conflicts when the same seed runs against multiple
/// contexts in the same test session.
/// </summary>
internal static class StoreSeedExtensions
{
    public static async Task SeedStoreAsync(this DbContext context)
    {
        context.Set<Address>().AddRange(SeedData.Addresses);
        await context.SaveChangesAsync();

        foreach (var c in SeedData.Customers)
        {
            context.Set<Customer>().Add(new Customer
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                AddressId = c.AddressId,
            });
        }
        await context.SaveChangesAsync();

        foreach (var o in SeedData.Orders)
        {
            context.Set<Order>().Add(new Order
            {
                Id = o.Id,
                Tag = o.Tag,
                Price = o.Price,
                Quantity = o.Quantity,
                Status = o.Status,
                CustomerId = o.CustomerId,
            });
        }
        await context.SaveChangesAsync();

        context.Set<LineItem>().AddRange(SeedData.LineItems);
        await context.SaveChangesAsync();
    }
}
