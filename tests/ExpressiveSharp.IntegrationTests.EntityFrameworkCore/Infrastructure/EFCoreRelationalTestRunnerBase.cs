using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

public abstract class EFCoreRelationalTestRunnerBase : EFCoreTestRunnerBase
{
    protected new IntegrationTestDbContext Context => (IntegrationTestDbContext)base.Context;

    protected EFCoreRelationalTestRunnerBase(DbContextOptions<IntegrationTestDbContext> options)
        : base(new IntegrationTestDbContext(options))
    {
        Context.Database.EnsureCreated();
    }

    public override async Task SeedAsync(
        IReadOnlyList<Address> addresses,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Order> orders,
        IReadOnlyList<LineItem> lineItems)
    {
        Context.Set<Address>().AddRange(addresses);
        await Context.SaveChangesAsync();

        foreach (var c in customers)
        {
            Context.Customers.Add(new Customer
            {
                Id = c.Id, Name = c.Name, Email = c.Email, AddressId = c.AddressId,
            });
        }
        await Context.SaveChangesAsync();

        foreach (var order in orders)
        {
            Context.Orders.Add(new Order
            {
                Id = order.Id, Tag = order.Tag, Price = order.Price,
                Quantity = order.Quantity, Status = order.Status, CustomerId = order.CustomerId,
            });
        }
        await Context.SaveChangesAsync();

        Context.Set<LineItem>().AddRange(lineItems);
        await Context.SaveChangesAsync();
    }
}
