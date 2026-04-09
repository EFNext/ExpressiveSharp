using System.Linq.Expressions;
using ExpressiveSharp;

using ExpressiveSharp.Transformers;

// ── The problem ──────────────────────────────────────────────────────────────
// Expression trees only accept a restricted subset of C#.
// Uncomment the next line to see CS8072 — null-conditional, switch expressions,
// and pattern matching are all rejected:
//
//   Expression<Func<Order, int?>> broken = o => o.Tag?.Length;   // CS8072!
//
// ExpressiveSharp fixes this at compile time via source generation.

// ── Sample data ──────────────────────────────────────────────────────────────

var orders = new List<Order>
{
    new() { Id = 1, Tag = "urgent", Price = 120.0, Quantity = 2, Status = OrderStatus.Approved,
            Customer = new Customer { Name = "Alice", Email = "alice@example.com" } },
    new() { Id = 2, Tag = "normal", Price = 45.0,  Quantity = 5, Status = OrderStatus.Pending,
            Customer = new Customer { Name = "Bob" } },
    new() { Id = 3, Tag = null,     Price = 200.0, Quantity = 1, Status = OrderStatus.Rejected,
            Customer = null },
    new() { Id = 4, Tag = "bulk",   Price = 8.0,   Quantity = 50, Status = OrderStatus.Approved,
            Customer = new Customer { Name = "Charlie" } },
};

// ── 1. ExpressionPolyfill.Create ─────────────────────────────────────────────
// Build expression trees inline with modern C# syntax.
Section("ExpressionPolyfill.Create");

// Null-conditional — would cause CS8072 without ExpressiveSharp
var tagLenExpr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
Console.WriteLine($"  Expression:  {tagLenExpr}");
var tagLenFn = tagLenExpr.Compile();
Console.WriteLine($"  Tag='hello' → {tagLenFn(new Order { Tag = "hello" })}");
Console.WriteLine($"  Tag=null    → {tagLenFn(new Order { Tag = null })}");

// Explicit type argument
var expensiveExpr = ExpressionPolyfill.Create<Func<Order, bool>>(o => o.Price > 50);
Console.WriteLine($"  Expression:  {expensiveExpr}");

// With transformer — strips the null-check pattern for providers that don't need it
var stripped = ExpressionPolyfill.Create((Order o) => o.Tag?.Length,
    new RemoveNullConditionalPatterns());
Console.WriteLine($"  Stripped:    {stripped}");

// ── 2. [Expressive] attribute ────────────────────────────────────────────────
// Mark properties and methods with [Expressive] to generate companion expression
// trees. At runtime, ExpandExpressives() inlines them into your expressions.
Section("[Expressive] Attribute");

// Simple computed property: Total => Price * Quantity
Expression<Func<Order, double>> totalExpr = o => o.Total;
var expandedTotal = totalExpr.ExpandExpressives();
Console.WriteLine($"  Before: {totalExpr}");
Console.WriteLine($"  After:  {expandedTotal}");
Console.WriteLine($"  Totals: [{string.Join(", ", orders.AsQueryable().Select((Expression<Func<Order, double>>)expandedTotal).ToList())}]");

// Null-conditional property: CustomerName => Customer?.Name
Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
Console.WriteLine($"  CustomerName expanded: {nameExpr.ExpandExpressives()}");

// You can also strip null-checks at the call site via a transformer:
Console.WriteLine($"  Stripped:              {nameExpr.ExpandExpressives(new RemoveNullConditionalPatterns())}");

// Switch expression: GetGrade() with relational patterns
Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
Console.WriteLine($"  GetGrade() expanded: {gradeExpr.ExpandExpressives()}");
foreach (var o in orders)
    Console.WriteLine($"    Order {o.Id} (${o.Price}) → {o.GetGrade()}");

// Block body: GetCategory() with local variable + if/else
Expression<Func<Order, string>> catExpr = o => o.GetCategory();
Console.WriteLine($"  GetCategory() expanded: {catExpr.ExpandExpressives()}");
foreach (var o in orders)
    Console.WriteLine($"    Order {o.Id} (qty={o.Quantity}) → {o.GetCategory()}");

// ── 3. Extension methods + type patterns ─────────────────────────────────────
// [Expressive] works on extension methods, including type-pattern switches.
Section("Extension Methods + Type Patterns");

var shapes = new List<Shape>
{
    new Circle { Name = "Unit circle", Radius = 1.0 },
    new Rectangle { Name = "Square", Width = 4.0, Height = 4.0 },
    new Circle { Name = "Big circle", Radius = 5.0 },
};

Expression<Func<Shape, double>> areaExpr = s => s.GetArea();
Console.WriteLine($"  Expression: {areaExpr.ExpandExpressives()}");
foreach (var s in shapes)
    Console.WriteLine($"    {s.Name} → area = {s.GetArea():F2}");

// ── 4. Constructor projection ────────────────────────────────────────────────
// [Expressive] constructors expand to MemberInit expressions, so LINQ providers
// can translate projections to SQL instead of loading entire entities.
Section("Constructor Projection");

Expression<Func<Order, OrderDto>> projExpr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
Console.WriteLine($"  Before: {projExpr}");
Console.WriteLine($"  After:  {projExpr.ExpandExpressives()}");

var dtos = orders.Select(o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total)).ToList();
foreach (var dto in dtos)
    Console.WriteLine($"    OrderDto {{ Id={dto.Id}, Desc=\"{dto.Description}\", Total={dto.Total} }}");

// ── 5. AsExpressive ─────────────────────────────────────────────────
// Wraps IQueryable<T> so delegate-based LINQ methods (Where, Select, OrderBy,
// GroupBy) accept modern C# syntax. The source generator rewrites the lambdas
// to expression trees at compile time.
Section("AsExpressive");

var aliceOrders = orders.AsQueryable()
    .AsExpressive()
    .Where(o => o.Customer?.Name == "Alice")
    .Select(o => o.Tag)
    .ToList();
Console.WriteLine($"  Alice's orders: [{string.Join(", ", aliceOrders.Select(t => $"\"{t}\""))}]");

var grouped = orders.AsQueryable()
    .AsExpressive()
    .OrderBy(o => o.Price)
    .GroupBy(o => o.Status)
    .ToList();
Console.WriteLine("  Grouped by status (ordered by price):");
foreach (var group in grouped)
    Console.WriteLine($"    {group.Key}: [{string.Join(", ", group.Select(o => $"#{o.Id}"))}]");

// ── Helpers ──────────────────────────────────────────────────────────────────

static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"--- {title} ---");
    Console.WriteLine();
}
