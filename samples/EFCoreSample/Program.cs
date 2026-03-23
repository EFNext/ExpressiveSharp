using ExpressiveSharp.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// ── Setup: in-memory SQLite ─────────────────────────────────────────────────

var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<SampleDbContext>()
    .UseSqlite(connection)
    .UseExpressives()      // <-- enables [Expressive] expansion + conventions
    .Options;

using var db = new SampleDbContext(options);
db.Database.EnsureCreated();

// ── Seed data ───────────────────────────────────────────────────────────────

db.Set<Customer>().AddRange(
    new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" },
    new Customer { Id = 2, Name = "Bob", Email = null },
    new Customer { Id = 3, Name = "Charlie", Email = "charlie@example.com" });

db.Set<Order>().AddRange(
    new Order { Id = 1, Tag = "urgent",  Price = 120.0, Quantity = 2,  CustomerId = 1 },
    new Order { Id = 2, Tag = "normal",  Price = 45.0,  Quantity = 5,  CustomerId = 2 },
    new Order { Id = 3, Tag = null,      Price = 200.0, Quantity = 1,  CustomerId = 1 },
    new Order { Id = 4, Tag = "bulk",    Price = 8.0,   Quantity = 50, CustomerId = 3 });

db.SaveChanges();

// ══════════════════════════════════════════════════════════════════════════════
// 1. [Expressive] property — auto-expanded by UseExpressives()
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("1. [Expressive] Property: Total => Price * Quantity");

var totalsQuery = db.Set<Order>().Select(o => new { o.Id, o.Total });
PrintSql(totalsQuery);
foreach (var r in totalsQuery) Console.WriteLine($"  Order #{r.Id}: Total = {r.Total}");

// ══════════════════════════════════════════════════════════════════════════════
// 2. [Expressive] method — switch expression
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("2. [Expressive] Method: GetGrade() — switch expression");

var gradesQuery = db.Set<Order>().Select(o => new { o.Id, o.Price, Grade = o.GetGrade() });
PrintSql(gradesQuery);
foreach (var r in gradesQuery) Console.WriteLine($"  Order #{r.Id} (${r.Price}): {r.Grade}");

// ══════════════════════════════════════════════════════════════════════════════
// 3. [Expressive] method — block body with local variables
//    FlattenBlockExpressions transformer inlines the local variable
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("3. [Expressive] Block Body: GetCategory() — if/else with local var");

var catsQuery = db.Set<Order>().Select(o => new { o.Id, o.Quantity, Category = o.GetCategory() });
PrintSql(catsQuery);
foreach (var r in catsQuery) Console.WriteLine($"  Order #{r.Id} (qty={r.Quantity}): {r.Category}");

// ══════════════════════════════════════════════════════════════════════════════
// 4. Null-conditional with RemoveNullConditionalPatterns
//    [Expressive(RemoveNullConditionalPatterns = true)] strips the null check
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("4. Null-Conditional Transformer: CustomerEmail");

var emailsQuery = db.Set<Order>().Select(o => new { o.Id, o.CustomerEmail });
PrintSql(emailsQuery);
foreach (var r in emailsQuery) Console.WriteLine($"  Order #{r.Id}: Email = {r.CustomerEmail ?? "(null)"}");

// ══════════════════════════════════════════════════════════════════════════════
// 5. ExpressiveDbSet — use ?. directly in delegate lambdas
//    No .WithExpressionRewrite() needed — ExpressiveDbSet handles it
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("5. ExpressiveDbSet: Null-conditional ?. in Where");

var emailQuery = db.Orders.Where(o => o.Customer?.Email != null);
PrintSql(emailQuery);
foreach (var o in emailQuery) Console.WriteLine($"  Order #{o.Id}: {o.Customer.Name} <{o.Customer.Email}>");

// ══════════════════════════════════════════════════════════════════════════════
// 6. ExpressiveDbSet — GroupBy with ?.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("6. ExpressiveDbSet: GroupBy with ?.");

var groupQuery = db.Orders.GroupBy(o => o.Customer?.Email);
PrintSql(groupQuery);
foreach (var g in groupQuery)
    Console.WriteLine($"  Email={g.Key ?? "(null)"}: [{string.Join(", ", g.Select(o => $"#{o.Id}"))}]");

// ══════════════════════════════════════════════════════════════════════════════
// 7. Constructor projection — [Expressive] constructor expands to MemberInit
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("7. Constructor Projection: OrderSummaryDto");

var dtoQuery = db.Set<Order>().Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total));
PrintSql(dtoQuery);
foreach (var dto in dtoQuery) Console.WriteLine($"  {{ Id={dto.Id}, Desc=\"{dto.Description}\", Total={dto.Total} }}");

// ══════════════════════════════════════════════════════════════════════════════
// 8. Combining features — ?. in Where + [Expressive] in Select
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("8. Combined: ?. filter + [Expressive] expansion");

var combinedQuery = db.Orders.Where(o => o.Customer?.Name == "Alice");
PrintSql(combinedQuery);
foreach (var o in combinedQuery) Console.WriteLine($"  Order #{o.Id}: Total={o.Total}, Grade={o.GetGrade()}");

// ── Helpers ─────────────────────────────────────────────────────────────────

connection.Close();

static void PrintHeader(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine($"  {title}");
    Console.WriteLine(new string('=', 70));
}

static void PrintSql<T>(IQueryable<T> query)
{
    Console.WriteLine($"  SQL: {query.ToQueryString()}");
    Console.WriteLine();
}
