using System.Linq.Expressions;
using ExpressiveSharp;
using ExpressiveSharp.Extensions;

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

// ══════════════════════════════════════════════════════════════════════════════
// 1. ExpressionPolyfill.Create
//    Converts a lambda with modern C# syntax into an Expression<T> at compile
//    time via source generator interception. Without this, using ?. in an
//    expression tree would cause CS8072.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("1. ExpressionPolyfill.Create");

// 1a. Inferred type argument (C# 10+)
var tagLengthExpr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
Console.WriteLine($"  Expression: {tagLengthExpr}");
var tagLengthFn = tagLengthExpr.Compile();
Console.WriteLine($"  Tag='hello' → {tagLengthFn(new Order { Tag = "hello" })}");
Console.WriteLine($"  Tag=null    → {tagLengthFn(new Order { Tag = null })}");
Console.WriteLine();

// 1b. With transformers — strips the null-conditional pattern
var tagLengthStripped = ExpressionPolyfill.Create((Order o) => o.Tag?.Length,
    new ExpressiveSharp.Transformers.RemoveNullConditionalPatterns());
Console.WriteLine($"  Stripped:    {tagLengthStripped}");
Console.WriteLine();

// 1c. Explicit type argument
var expensiveExpr = ExpressionPolyfill.Create<Func<Order, bool>>(o => o.Price > 50);
Console.WriteLine($"  Expression: {expensiveExpr}");
var expensiveFn = expensiveExpr.Compile();
Console.WriteLine($"  Price=120 → {expensiveFn(new Order { Price = 120 })}");
Console.WriteLine($"  Price=10  → {expensiveFn(new Order { Price = 10 })}");

// ══════════════════════════════════════════════════════════════════════════════
// 2. [Expressive] Properties + ExpandExpressives()
//    Mark properties with [Expressive] to generate companion expression trees.
//    At runtime, ExpandExpressives() inlines them into your expression tree.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("2. [Expressive] Properties");

Expression<Func<Order, double>> totalExpr = o => o.Total;
var expandedTotal = (Expression<Func<Order, double>>)totalExpr.ExpandExpressives();
Console.WriteLine($"  Original:  {totalExpr}");
Console.WriteLine($"  Expanded:  {expandedTotal}");

var totals = orders.AsQueryable().Select(expandedTotal).ToList();
Console.WriteLine($"  Totals: [{string.Join(", ", totals)}]");

// ══════════════════════════════════════════════════════════════════════════════
// 3. Null-Conditional Transformers
//    The emitter always faithfully translates ?. to a null-check ternary.
//    Transformers can strip or reshape null-check patterns at runtime.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("3. Null-Conditional Transformers");

// Default: ?. → explicit null check (Customer != null ? Customer.Name : null)
Expression<Func<Order, string?>> custNameExpr = o => o.CustomerName;
var expandedDefault = custNameExpr.ExpandExpressives();
Console.WriteLine($"  Default:     {expandedDefault}");

// With RemoveNullConditionalPatterns (set via [Expressive(RemoveNullConditionalPatterns = true)])
Expression<Func<Order, string?>> custNameUnsafe = o => o.CustomerNameUnsafe;
var expandedStripped = custNameUnsafe.ExpandExpressives();
Console.WriteLine($"  Stripped:    {expandedStripped}");

// Or apply the transformer at the call site via ExpandExpressives()
var expandedManual = custNameExpr.ExpandExpressives(
    new ExpressiveSharp.Transformers.RemoveNullConditionalPatterns());
Console.WriteLine($"  Manual:      {expandedManual}");

// ══════════════════════════════════════════════════════════════════════════════
// 4. [Expressive] Methods — Switch Expressions
//    Switch expressions with relational patterns are fully supported.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("4. Switch Expressions");

Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
var expandedGrade = gradeExpr.ExpandExpressives();
Console.WriteLine($"  Expression: {expandedGrade}");

foreach (var o in orders)
    Console.WriteLine($"  Order {o.Id} (${o.Price}) → {o.GetGrade()}");

// ══════════════════════════════════════════════════════════════════════════════
// 5. [Expressive] Extension Methods — Type Patterns
//    Extension methods work with [Expressive]. Here we use type-pattern switch.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("5. Extension Methods with Type Patterns");

var shapes = new List<Shape>
{
    new Circle { Name = "Unit circle", Radius = 1.0 },
    new Rectangle { Name = "Square", Width = 4.0, Height = 4.0 },
    new Circle { Name = "Big circle", Radius = 5.0 },
};

Expression<Func<Shape, double>> areaExpr = s => s.GetArea();
var expandedArea = areaExpr.ExpandExpressives();
Console.WriteLine($"  Expression: {expandedArea}");

foreach (var s in shapes)
    Console.WriteLine($"  {s.Name} → area = {s.GetArea():F2}");

// ══════════════════════════════════════════════════════════════════════════════
// 6. [Expressive] Constructors
//    Mark constructors with [Expressive] to generate MemberInit expressions,
//    enabling projection in LINQ queries.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("6. Projectable Constructors");

Expression<Func<Order, OrderDto>> projExpr = o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total);
var expandedProj = projExpr.ExpandExpressives();
Console.WriteLine($"  Original:  {projExpr}");
Console.WriteLine($"  Expanded:  {expandedProj}");

var dtos = orders.Select(o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total)).ToList();
foreach (var dto in dtos)
    Console.WriteLine($"  OrderDto {{ Id={dto.Id}, Desc=\"{dto.Description}\", Total={dto.Total} }}");

// ══════════════════════════════════════════════════════════════════════════════
// 7. EnumMethodExpansion
//    Expands enum method calls into ternary chains — each enum value is
//    evaluated at compile time, making the expression SQL-translatable.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("7. EnumMethodExpansion");

Expression<Func<Order, string>> statusExpr = o => o.StatusDescription;
var expandedStatus = statusExpr.ExpandExpressives();
Console.WriteLine($"  Expression: {expandedStatus}");

foreach (var o in orders)
    Console.WriteLine($"  Order {o.Id} ({o.Status}) → \"{o.StatusDescription}\"");

// ══════════════════════════════════════════════════════════════════════════════
// 8. Block Body
//    Block-bodied methods (if/else, local variables) are automatically converted
//    to expression trees. Local vars are inlined, if/else becomes ternary.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("8. Block Body");

Expression<Func<Order, string>> catExpr = o => o.GetCategory();
var expandedCat = catExpr.ExpandExpressives();
Console.WriteLine($"  Expression: {expandedCat}");

foreach (var o in orders)
    Console.WriteLine($"  Order {o.Id} (qty={o.Quantity}) → {o.GetCategory()}");

// ══════════════════════════════════════════════════════════════════════════════
// 9. IRewritableQueryable + WithExpressionRewrite
//     Wraps IQueryable<T> so that delegate-based LINQ methods (Where, Select,
//     OrderBy, GroupBy) accept modern C# syntax. The source generator intercepts
//     these calls and rewrites lambdas to expression trees at compile time.
// ══════════════════════════════════════════════════════════════════════════════
PrintHeader("9. IRewritableQueryable");

// 10a. WithExpressionRewrite + Where with null-conditional
var aliceOrders = orders.AsQueryable()
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Name == "Alice")
    .Select(o => o.Tag)
    .ToList();

Console.WriteLine($"  Alice's orders: [{string.Join(", ", aliceOrders.Select(t => $"\"{t}\""))}]");

// 10b. OrderBy + GroupBy
var grouped = orders.AsQueryable()
    .WithExpressionRewrite()
    .OrderBy(o => o.Price)
    .GroupBy(o => o.Status)
    .ToList();

Console.WriteLine("  Orders grouped by status (ordered by price):");
foreach (var group in grouped)
    Console.WriteLine($"    {group.Key}: [{string.Join(", ", group.Select(o => $"#{o.Id}"))}]");

// ══════════════════════════════════════════════════════════════════════════════

static void PrintHeader(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine($"  {title}");
    Console.WriteLine(new string('=', 70));
}
