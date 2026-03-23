using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store;

public static class SeedData
{
    public static IReadOnlyList<Customer> Customers => new[]
    {
        new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" },
        new Customer { Id = 2, Name = "Bob", Email = "bob@example.com" },
        new Customer { Id = 3, Name = null, Email = "anon@example.com" },
    };

    public static IReadOnlyList<Order> Orders => new[]
    {
        // Premium grade, Regular category (Qty*10=20), Approved, has customer (Alice), has tag
        new Order { Id = 1, Tag = "RUSH", Price = 120.0, Quantity = 2,
                    Status = OrderStatus.Approved, CustomerId = 1 },
        // Standard grade, Bulk category (Qty*10=200), Pending, has customer (Bob), has tag
        new Order { Id = 2, Tag = "STD", Price = 75.0, Quantity = 20,
                    Status = OrderStatus.Pending, CustomerId = 2 },
        // Budget grade, Regular category (Qty*10=30), Rejected, NO customer, NO tag
        new Order { Id = 3, Tag = null, Price = 10.0, Quantity = 3,
                    Status = OrderStatus.Rejected, CustomerId = null },
        // Standard grade, Regular category (Qty*10=50), Pending, customer with null Name, has tag
        new Order { Id = 4, Tag = "SPECIAL", Price = 50.0, Quantity = 5,
                    Status = OrderStatus.Pending, CustomerId = 3 },
    };
}
