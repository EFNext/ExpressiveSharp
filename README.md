# ExpressiveSharp

[![CI](https://github.com/EFNext/ExpressiveSharp/actions/workflows/ci.yml/badge.svg)](https://github.com/EFNext/ExpressiveSharp/actions/workflows/ci.yml)

Source generator that enables modern C# syntax in LINQ expression trees. All expression trees are generated at compile time with minimal runtime overhead.

## The Problem

There are two problems when using C# with LINQ providers like EF Core:

**1. Expression tree syntax restrictions.** You write a perfectly reasonable query and hit:

```
error CS8072: An expression tree lambda may not contain a null propagating operator
```

Expression trees (`Expression<Func<...>>`) only support a restricted subset of C# — no `?.`, no switch expressions, no pattern matching. So you end up writing ugly ternary chains instead of the clean code you'd write anywhere else.

**2. Computed properties are opaque to LINQ providers.** You define `public string FullName => FirstName + " " + LastName` and use it in a query — but EF Core can't see inside the property getter. It either throws a runtime translation error, or worse, silently fetches the entire entity to evaluate `FullName` on the client (overfetching). The only workaround is to duplicate the logic as an inline expression in every query that needs it.

ExpressiveSharp fixes this. Write natural C# and the source generator builds the expression tree at compile time:

```csharp
public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Customer? Customer { get; set; }

    // Computed property — reusable in any query, translated to SQL
    [Expressive]
    public double Total => Price * Quantity;

    // Switch expression — normally illegal in expression trees
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}

// [Expressive] members + ?. syntax — all translated to SQL
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, o.Total, Email = o.Customer?.Email, Grade = o.GetGrade() })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total",
       "c"."Email",
       CASE
           WHEN "o"."Price" >= 100.0 THEN 'Premium'
           WHEN "o"."Price" >= 50.0 THEN 'Standard'
           ELSE 'Budget'
       END AS "Grade"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE "c"."Email" IS NOT NULL
```

## Quick Start

```bash
dotnet add package ExpressiveSharp
# Optional: EF Core integration
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

### Which API Should I Use?

Mark computed properties and methods with [`[Expressive]`](#expressive-attribute) to generate companion expression trees. Then choose how to wire them into your queries:

| Scenario | API |
|---|---|
| **EF Core** — modern syntax + `[Expressive]` expansion on `DbSet` | [`ExpressiveDbSet<T>`](#ef-core-integration) (or [`UseExpressives()`](#ef-core-integration) for global `[Expressive]` expansion) |
| **Any `IQueryable`** — modern syntax + `[Expressive]` expansion | [`.WithExpressionRewrite()`](#irewritablequeryt) |
| **Advanced** — build an `Expression<T>` inline, no attribute needed | [`ExpressionPolyfill.Create`](#expressionpolyfillcreate) |
| **Advanced** — expand `[Expressive]` members in an existing expression tree | [`.ExpandExpressives()`](#expressive-attribute) |

## Usage

### `[Expressive]` Attribute

Decorate properties, methods, or constructors to generate companion expression trees. Supports modern C# syntax that normally can't appear in expression trees.

```csharp
public class Order
{
    public Customer? Customer { get; set; }
    public string? Tag { get; set; }

    // Null-conditional operators
    [Expressive]
    public string? CustomerEmail => Customer?.Email;

    // Switch expressions with pattern matching
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50 => "Standard",
        _ => "Budget",
    };

    // Block bodies are opt-in and experimental
    [Expressive(AllowBlockBody = true)]
    public string GetCategory()
    {
        var threshold = Quantity * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}
```

EF Core translates block bodies to SQL CASE expressions:

```sql
SELECT CASE
    WHEN ("o"."Quantity" * 10) > 100 THEN 'Bulk'
    ELSE 'Regular'
END AS "Category"
FROM "Orders" AS "o"
```

Expand `[Expressive]` members manually in expression trees — this replaces `o.Total` (a property access) with the generated expression (`o.Price * o.Quantity`), so LINQ providers can translate it:

```csharp
Expression<Func<Order, double>> expr = o => o.Total;
// expr body is: o.Total (opaque property access)
var expanded = expr.ExpandExpressives();
// expanded body is: o.Price * o.Quantity (translatable by EF Core / other providers)
```

### `IRewritableQueryable<T>`

Wrap any `IQueryable<T>` to use modern syntax directly in LINQ chains:

```csharp
var results = queryable
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, Name = o.Customer?.Name ?? "Unknown" })
    .OrderBy(o => o.Name)
    .ToList();
```

The source generator intercepts these calls at compile time and rewrites them to use proper expression trees — no runtime overhead.

Available LINQ methods: `Where`, `Select`, `SelectMany`, `OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending`, `GroupBy`.

### `ExpressionPolyfill.Create`

Create expression trees inline using modern syntax — no attribute needed:

```csharp
var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
// expr is Expression<Func<Order, int?>>

var compiled = expr.Compile();
var result = compiled(order);
```

With transformers:

```csharp
var expr = ExpressionPolyfill.Create(
    (Order o) => o.Customer?.Email,
    new RemoveNullConditionalPatterns());
```

### EF Core Integration

Install `ExpressiveSharp.EntityFrameworkCore` and call `UseExpressives()` on your `DbContextOptionsBuilder`:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives()
    .Options;
```

This automatically:
- Expands `[Expressive]` member references in queries
- Marks `[Expressive]` properties as unmapped in the EF model
- Applies database-friendly transformers (`ConvertLoopsToLinq`, `RemoveNullConditionalPatterns`, `FlattenTupleComparisons`, `FlattenBlockExpressions`)

For direct access on `DbSet`, use `ExpressiveDbSet<T>`:

```csharp
public class MyDbContext : DbContext
{
    // Shorthand for Set<Order>().AsExpressiveDbSet()
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
}

// Modern syntax works directly — no .WithExpressionRewrite() needed
ctx.Orders.Where(o => o.Customer?.Name == "Alice");
```

## Supported C# Features

### Expression-Level

| Feature | Status | Notes |
|---|---|---|
| Null-conditional `?.` (member access and indexer) | Supported | Generates faithful null-check ternary; `UseExpressives()` strips it for SQL |
| Switch expressions | Supported | Translated to nested CASE/ternary |
| Pattern matching (constant, type, relational, logical, property, positional) | Supported | |
| Declaration patterns with named variables | Partial | Works in switch arms only |
| String interpolation | Supported | Converted to `string.Concat` calls |
| Tuple literals | Supported | |
| Enum method expansion | Supported | Expands enum extension methods into per-value ternary chains |
| C# 14 extension members | Supported | |
| List patterns (fixed-length and slice) | Supported | |
| Index/range (`^1`, `1..3`) | Supported | |
| `with` expressions (records) | Supported | |
| Collection expressions (`[1, 2, 3]`, `[..items]`) | Supported | |
| Dictionary indexer initializers | Supported | |
| `this`/`base` references | Supported | |

### Block-Body

| Feature | Status |
|---|---|
| `return`, `if`/`else`, `switch` statements | Supported |
| Local variable declarations (inlined) | Supported |
| `foreach` loops (converted to LINQ) | Supported |
| `for` loops (array/list iteration) | Supported |
| `while`/`do-while`, `try`/`catch`, `async`/`await` | Not supported |
| Assignments, `++`, `--` | Not supported |

### Constructor Projection (EF Core)

Mark constructors with `[Expressive]` to generate `MemberInit` expressions that EF Core can translate to SQL projections:

```csharp
public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderSummaryDto() { }

    [Expressive]
    public OrderSummaryDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}

// Constructor call is translated to SQL projection
var dtos = db.Orders
    .Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total))
    .ToList();
```

```sql
SELECT "o"."Id",
       COALESCE("o"."Tag", 'N/A') AS "Description",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total"
FROM "Orders" AS "o"
```

Property assignments, local variables, `if`/`else`, and `base()`/`this()` initializer chains are all supported.

## Expression Transformers

ExpressiveSharp generates faithful expression trees that mirror the original C# code. Transformers adapt these trees for specific consumers at runtime.

### Built-in Transformers

**`RemoveNullConditionalPatterns`** — Strips null-check ternaries (`x != null ? x.Prop : default` becomes `x.Prop`). Useful for databases that handle null propagation natively.

**`FlattenBlockExpressions`** — Inlines block-local variables and removes `Expression.Block` nodes. Required for LINQ providers that don't support block expressions (including EF Core).

**`FlattenTupleComparisons`** — Replaces `ValueTuple` field access on inline tuple construction with the underlying arguments, so `(Price, Quantity) == (50.0, 5)` translates to `Price == 50.0 AND Quantity == 5` instead of requiring `ValueTuple` construction.

**`ConvertLoopsToLinq`** — Converts loop expressions (produced by the emitter for `foreach`/`for` loops) into equivalent LINQ method calls (`Sum`, `Count`, `Any`, `All`, etc.) that LINQ providers can translate to SQL.

All four transformers are applied automatically when using `UseExpressives()` with EF Core. To apply them per-member without EF Core, use the `Transformers` property:

```csharp
[Expressive(Transformers = new[] { typeof(RemoveNullConditionalPatterns) })]
public string? CustomerName => Customer?.Name;
```

### Custom Transformers

Implement `IExpressionTreeTransformer` to create your own:

```csharp
public class MyTransformer : IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
    {
        // Rewrite expression tree as needed
        return expression;
    }
}
```

Apply via the attribute or at runtime:

```csharp
[Expressive(Transformers = new[] { typeof(MyTransformer) })]
public double Total => Price * Quantity;

// Or at runtime
expr.ExpandExpressives(new MyTransformer());
```

## How It Works

ExpressiveSharp uses two Roslyn source generators:

1. **`ExpressiveGenerator`** — Finds `[Expressive]` members, analyzes them at the semantic level (IOperation), and generates `Expression<Func<...>>` factory code using `Expression.*` calls. Registers them in a per-assembly expression registry for runtime lookup.

2. **`PolyfillInterceptorGenerator`** — Uses C# 13 method interceptors to replace `ExpressionPolyfill.Create` calls and `IRewritableQueryable<T>` LINQ methods at their call sites, converting lambdas to expression trees at compile time.

All expression trees are generated at compile time. There is no runtime reflection or expression compilation.

## FAQ

### Does this have any runtime overhead?

No practical impact. The source generators emit `Expression.*` factory calls at compile time. At runtime, `UseExpressives()` replaces opaque property accesses with the pre-built expressions during EF Core query compilation — this adds a small cost on first execution, but the expanded query is cached by EF Core afterward. There is no runtime reflection, no `Compile()`, and no expression tree parsing.

### Can `[Expressive]` members call other `[Expressive]` members?

Yes. `ExpandExpressives()` (and `UseExpressives()`) recursively resolves nested `[Expressive]` references. You can compose computed properties freely:

```csharp
[Expressive]
public double Total => Price * Quantity;

[Expressive]
public double TotalWithTax => Total * (1 + TaxRate);  // references Total
```

### Is this EF Core specific?

No. The core `ExpressiveSharp` package works with any LINQ provider or standalone expression tree use case. The `ExpressiveSharp.EntityFrameworkCore` package adds EF Core-specific integration (auto-expansion, model conventions, transformers).

### Coming from EntityFrameworkCore.Projectables?

ExpressiveSharp is its spiritual successor. See the [Migration Guide](docs/migration-from-projectables.md) for a step-by-step walkthrough including automated code fixers.

Key improvements: broader C# syntax support (switch expressions, pattern matching, string interpolation, tuples), customizable transformer pipeline, inline expression creation via `ExpressionPolyfill.Create`, modern syntax in LINQ chains via `IRewritableQueryable<T>`, and no EF Core coupling.

## Requirements

| | .NET 8.0 | .NET 10.0 |
|---|---|---|
| **ExpressiveSharp** | C# 12 | C# 14 |
| **ExpressiveSharp.EntityFrameworkCore** | EF Core 8.x | EF Core 10.x |

## Contributing

Development docs for contributors:

- [Testing Strategy](docs/testing-strategy.md) — snapshot tests, functional tests, and test consumers
- [IOperation to Expression Mapping](docs/ioperation-to-expression-mapping.md) — reference table for the expression tree emitter

```bash
dotnet build    # Build all projects
dotnet test     # Run all tests
```

## License

MIT
