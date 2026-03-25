using ExpressiveSharp.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// ── Setup ────────────────────────────────────────────────────────────────────

var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<SampleDbContext>()
    .UseSqlite(connection)
    .UseExpressives()      // ← this one line enables everything below
    .Options;

using var db = new SampleDbContext(options);
db.Database.EnsureCreated();
SeedData(db);

// ── 1. Computed properties in SQL ────────────────────────────────────────────
// Total => Price * Quantity becomes a SQL expression, not a client-side eval.
Section("Computed Properties in SQL");

var totalsQuery = db.Orders.AsQueryable().Select(o => new { o.Id, o.Total });
PrintSql(totalsQuery);
foreach (var r in totalsQuery) Console.WriteLine($"  Order #{r.Id}: Total = {r.Total}");

// ── 2. Modern syntax in queries ──────────────────────────────────────────────
// ExpressiveDbSet lets you use ?. directly in delegate lambdas.
// The source generator rewrites them to expression trees at compile time.
Section("Modern Syntax in Queries");

// Where with null-conditional ?.
var emailFilter = db.Orders.Where(o => o.Customer?.Email != null);
PrintSql(emailFilter);
foreach (var o in emailFilter) Console.WriteLine($"  Order #{o.Id}: {o.Customer.Name} <{o.Customer.Email}>");

// Switch expression in Select — becomes CASE WHEN in SQL
var gradesQuery = db.Orders.AsQueryable().Select(o => new { o.Id, o.Price, Grade = o.GetGrade() });
PrintSql(gradesQuery);
foreach (var r in gradesQuery) Console.WriteLine($"  Order #{r.Id} (${r.Price}): {r.Grade}");

// GroupBy with ?.
var groupQuery = db.Orders.GroupBy(o => o.Customer?.Email);
PrintSql(groupQuery);
foreach (var g in groupQuery)
    Console.WriteLine($"  Email={g.Key ?? "(null)"}: [{string.Join(", ", g.Select(o => $"#{o.Id}"))}]");

// ── 3. Block bodies and enum expansion ───────────────────────────────────────
// Block-bodied methods and enum method calls are flattened into SQL-translatable
// expressions automatically.
Section("Block Bodies and Enum Expansion");

// GetCategory() — block body with local variable + if/else → CASE WHEN
var catsQuery = db.Orders.AsQueryable().Select(o => new { o.Id, o.Quantity, Category = o.GetCategory() });
PrintSql(catsQuery);
foreach (var r in catsQuery) Console.WriteLine($"  Order #{r.Id} (qty={r.Quantity}): {r.Category}");

// StatusDescription — enum method expansion → ternary chain in SQL
var statusQuery = db.Orders.AsQueryable().Select(o => new { o.Id, o.Status, Desc = o.StatusDescription });
PrintSql(statusQuery);
foreach (var r in statusQuery) Console.WriteLine($"  Order #{r.Id} ({r.Status}): \"{r.Desc}\"");

// ── 4. Constructor projection ────────────────────────────────────────────────
// [Expressive] constructors expand to MemberInit, so EF Core translates the
// projection to a clean SQL SELECT instead of loading entire entities.
Section("Constructor Projection");

var dtoQuery = db.Orders.AsQueryable().Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total));
PrintSql(dtoQuery);
foreach (var dto in dtoQuery) Console.WriteLine($"  {{ Id={dto.Id}, Desc=\"{dto.Description}\", Total={dto.Total} }}");

// ── 5. Combining everything ──────────────────────────────────────────────────
// One query: ?. filter + [Expressive] property + switch expression.
Section("Combining Everything");

// ExpressiveDbSet handles the ?. in Where; UseExpressives() expands
// [Expressive] members when they appear elsewhere in the query pipeline.
var combined = db.Orders.Where(o => o.Customer?.Name == "Alice");
PrintSql(combined);
foreach (var o in combined) Console.WriteLine($"  Order #{o.Id}: Total={o.Total}, Grade={o.GetGrade()}");

// ── Helpers ──────────────────────────────────────────────────────────────────

connection.Close();

static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"--- {title} ---");
    Console.WriteLine();
}

static void PrintSql<T>(IQueryable<T> query)
{
    Console.WriteLine($"  SQL: {query.ToQueryString()}");
    Console.WriteLine();
}

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
