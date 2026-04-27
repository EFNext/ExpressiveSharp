using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

// Entities are re-materialized (not the originals from SeedData) to avoid
// tracker conflicts when the same seed runs against multiple contexts in the
// same test session.
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
