// A small order-management tool that queries an SQLite database.
// ExpressiveSharp lets us write natural C# (null-conditional ?., switch expressions,
// computed properties) and have it all translate to SQL instead of evaluating client-side.


using ExpressiveSharp;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<SampleDbContext>()
    .UseSqlite(connection)
    .UseExpressives()
    .Options;

using var db = new SampleDbContext(options);
db.Database.EnsureCreated();
SeedData(db);

Console.WriteLine();

// Find all orders where the customer has an email on file.
// The ?. operator works directly in the lambda — the source generator
// rewrites it to an expression tree that EF Core translates to SQL.
var contactableOrders = db.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new OrderSummaryDto(o.Id, o.Customer.Name, o.Total, o.Grade, o.StatusDescription))
    .ToList();

Console.WriteLine("Orders with customer email on file:");
foreach (var dto in contactableOrders)
    Console.WriteLine($"  #{dto.OrderId} {dto.CustomerName} — ${dto.Total:N2} ({dto.Grade}, {dto.Status})");

Console.WriteLine();

// Premium approved orders — filter on [Expressive] computed properties.
var premiumApproved = db.Orders
    .Where(o => o.Total >= 200 && o.Status == OrderStatus.Approved)
    .Select(o => new { o.Id, o.Total, o.Grade })
    .ToList();

Console.WriteLine("Premium approved orders (total >= $200):");
foreach (var o in premiumApproved)
    Console.WriteLine($"  #{o.Id} — ${o.Total:N2} ({o.Grade})");

Console.WriteLine();

// Revenue by status — the [Expressive] StatusDescription property expands to a
// CASE WHEN chain in SQL via UseExpressives(), no special queryable needed.
var revenueByStatus = db.Orders.AsQueryable()
    .GroupBy(o => o.StatusDescription)
    .Select(g => new { Status = g.Key, Revenue = g.Sum(o => o.Total) })
    .ToList();

Console.WriteLine("Revenue by status:");
foreach (var row in revenueByStatus)
    Console.WriteLine($"  {row.Status}: ${row.Revenue:N2}");

connection.Close();

static void SeedData(SampleDbContext db)
{
    db.Set<Customer>().AddRange(
        new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" },
        new Customer { Id = 2, Name = "Bob", Email = null },
        new Customer { Id = 3, Name = "Charlie", Email = "charlie@example.com" });

    db.Set<Order>().AddRange(
        new Order { Id = 1, Tag = "urgent",  Price = 120.0, Quantity = 2,  CustomerId = 1, Status = OrderStatus.Approved },
        new Order { Id = 2, Tag = "normal",  Price = 45.0,  Quantity = 5,  CustomerId = 2, Status = OrderStatus.Pending },
        new Order { Id = 3, Tag = null,      Price = 200.0, Quantity = 1,  CustomerId = 1, Status = OrderStatus.Rejected },
        new Order { Id = 4, Tag = "bulk",    Price = 8.0,   Quantity = 50, CustomerId = 3, Status = OrderStatus.Approved });

    db.SaveChanges();
}
