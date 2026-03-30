# <img src="assets/icon.png" width="32" height="32" alt="ExpressiveSharp icon" style="vertical-align: middle"> ExpressiveSharp

[![CI](https://github.com/EFNext/ExpressiveSharp/actions/workflows/ci.yml/badge.svg)](https://github.com/EFNext/ExpressiveSharp/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/ExpressiveSharp.svg)](https://www.nuget.org/packages/ExpressiveSharp)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ExpressiveSharp.svg)](https://www.nuget.org/packages/ExpressiveSharp)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-purple)](https://dotnet.microsoft.com)
[![GitHub Stars](https://img.shields.io/github/stars/EFNext/ExpressiveSharp)](https://github.com/EFNext/ExpressiveSharp/stargazers)
[![GitHub Issues](https://img.shields.io/github/issues/EFNext/ExpressiveSharp)](https://github.com/EFNext/ExpressiveSharp/issues)

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

Generated SQL (SQLite — other providers produce equivalent dialect-specific SQL):

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

See the [BasicSample](samples/BasicSample) and [EFCoreSample](samples/EFCoreSample) projects for runnable examples.

### Which API Should I Use?

Mark computed properties and methods with [`[Expressive]`](#expressive-attribute) to generate companion expression trees. Then choose how to wire them into your queries:

| Scenario | API |
|---|---|
| **EF Core** — modern syntax + `[Expressive]` expansion on `DbSet` | [`ExpressiveDbSet<T>`](#ef-core-integration) (or [`UseExpressives()`](#ef-core-integration) for global `[Expressive]` expansion) |
| **Any `IQueryable`** — modern syntax + `[Expressive]` expansion | [`.WithExpressionRewrite()`](#irewritablequeryt--modern-syntax-on-any-iqueryable) |
| **EF Core** — SQL window functions (ROW_NUMBER, RANK, etc.) | [`WindowFunction.*`](#window-functions-sql) (install `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions`) |
| **Advanced** — build an `Expression<T>` inline, no attribute needed | [`ExpressionPolyfill.Create`](#expressionpolyfillcreate) |
| **Advanced** — expand `[Expressive]` members in an existing expression tree | [`.ExpandExpressives()`](#expressive-attribute) |
| **Advanced** — make third-party/BCL members expressable | [`[ExpressiveFor]`](#expressivefor--external-member-mapping) |

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

    // Block bodies are opt-in and experimental (per-member or globally via MSBuild)
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

### `IRewritableQueryable<T>` — Modern Syntax on Any `IQueryable`

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

Most common `Queryable` methods are supported — filtering (`Where`, `Any`, `All`), projection (`Select`, `SelectMany`), ordering (`OrderBy`, `ThenBy`), grouping (`GroupBy`), joins (`Join`, `GroupJoin`, `Zip`), aggregation (`Sum`, `Average`, `Min`, `Max`, `Count`), element access (`First`, `Single`, `Last` and their `OrDefault` variants), set operations (`ExceptBy`, `IntersectBy`, `UnionBy`, `DistinctBy`), and more. Non-lambda operators like `Take`, `Skip`, `Distinct`, and `Reverse` preserve the `IRewritableQueryable<T>` chain. Comparer overloads (`IEqualityComparer<T>`, `IComparer<T>`) are also supported.

On .NET 10+, additional methods are available: `LeftJoin`, `RightJoin`, `CountBy`, `AggregateBy`, and `Index`.

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

`ExpressiveDbSet<T>` preserves chain continuity across EF Core operations — `Include`/`ThenInclude`, `AsNoTracking`, `IgnoreQueryFilters`, `TagWith`, and all async lambda methods (`AnyAsync`, `FirstAsync`, `SumAsync`, etc.) work seamlessly:

```csharp
var result = await ctx.Orders
    .Include(o => o.Customer)
    .ThenInclude(c => c.Address)
    .AsNoTracking()
    .Where(o => o.Customer?.Name == "Alice")
    .FirstOrDefaultAsync(o => o.Total > 100);
```

### Window Functions (SQL) — Experimental

Install `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` for SQL window function support (ROW_NUMBER, RANK, DENSE_RANK, NTILE):

> **Note:** This package is experimental. EF Core has an [open issue](https://github.com/dotnet/efcore/issues/12747) for native window function support — this package may be superseded when that ships.

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore.RelationalExtensions
```

Enable it in your `DbContextOptionsBuilder`:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives(o => o.UseRelationalExtensions())
    .Options;
```

Then use window functions in your queries:

```csharp
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

var ranked = db.Orders.Select(o => new
{
    o.Id,
    o.Price,
    RowNum = WindowFunction.RowNumber(
        Window.OrderBy(o.Price)),
    PriceRank = WindowFunction.Rank(
        Window.PartitionBy(o.CustomerId)
              .OrderByDescending(o.Price)),
    Quartile = WindowFunction.Ntile(4,
        Window.OrderBy(o.Id))
});
```

Generated SQL:

```sql
SELECT "o"."Id", "o"."Price",
    ROW_NUMBER() OVER(ORDER BY "o"."Price"),
    RANK() OVER(PARTITION BY "o"."CustomerId" ORDER BY "o"."Price" DESC),
    NTILE(4) OVER(ORDER BY "o"."Id")
FROM "Orders" AS "o"
```

Build window specifications with the fluent API:

| Method | SQL |
|---|---|
| `Window.OrderBy(expr)` | `ORDER BY expr ASC` |
| `Window.OrderByDescending(expr)` | `ORDER BY expr DESC` |
| `Window.PartitionBy(expr)` | `PARTITION BY expr` |
| `.ThenBy(expr)` / `.ThenByDescending(expr)` | Additional ordering columns |

Window functions are supported across SQLite, SQL Server, PostgreSQL, MySQL, and Oracle.

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
| Checked arithmetic (`checked(...)`) | Supported | |

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

## `[ExpressiveFor]` — External Member Mapping

Provide expression-tree bodies for members on types you don't own — BCL methods, third-party libraries, or your own members that can't use `[Expressive]` directly. This lets you use those members in EF Core queries that would otherwise fail with "could not be translated".

```csharp
using ExpressiveSharp.Mapping;

// Static method — params match the target signature
static class MathMappings
{
    [ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);
}

// Instance method — first param is the receiver
static class StringMappings
{
    [ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}

// Instance property on your own type
static class EntityMappings
{
    [ExpressiveFor(typeof(MyType), nameof(MyType.FullName))]
    static string FullName(MyType obj)
        => obj.FirstName + " " + obj.LastName;
}
```

Call sites are unchanged — the replacer substitutes the mapping automatically:

```csharp
// Without [ExpressiveFor]: throws "could not be translated"
// With [ExpressiveFor]:    Math.Clamp → ternary expression → translated to SQL
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => Math.Clamp(o.Price, 20, 100) > 50)
    .ToList();
```

Use `[ExpressiveForConstructor]` for constructors:

```csharp
[ExpressiveForConstructor(typeof(MyDto))]
static MyDto Create(int id, string name) => new MyDto { Id = id, Name = name };
```

> **Note:** If a member already has `[Expressive]`, adding `[ExpressiveFor]` targeting it is a compile error (EXP0019). `[ExpressiveFor]` is for members that *don't* have `[Expressive]`.

## Configuration

### `Expressive_AllowBlockBody` MSBuild Property

Instead of opting in to block bodies per-member, you can enable them globally for a project:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

This is equivalent to setting `AllowBlockBody = true` on every `[Expressive]` member.

### Global Transformers

Register transformers globally so all `ExpandExpressives()` calls apply them without explicit parameters:

```csharp
ExpressiveOptions.Default.AddTransformers(new RemoveNullConditionalPatterns());

// All subsequent ExpandExpressives() calls use this transformer
expr.ExpandExpressives(); // RemoveNullConditionalPatterns applied automatically
```

### Plugin Architecture (Advanced)

Create custom EF Core integrations by implementing `IExpressivePlugin`:

```csharp
public class MyPlugin : IExpressivePlugin
{
    public void ApplyServices(IServiceCollection services)
    {
        // Register custom EF Core services
    }

    public IExpressionTreeTransformer[] GetTransformers()
        => [new MyCustomTransformer()];
}

// Register during setup
options.UseExpressives(o => o.AddPlugin(new MyPlugin()));
```

The built-in `RelationalExtensions` package uses this plugin architecture.

## Diagnostics

Common diagnostic codes emitted by the source generator:

| Code | Severity | Description |
|------|----------|-------------|
| EXP0001 | Error | Member must have a body (expression-bodied or block) |
| EXP0004 | Error | Block body requires `AllowBlockBody = true` |
| EXP0005 | Error | Side effects detected in block body |
| EXP0008 | Warning | Unsupported operation — default value substituted |
| EXP0014 | Error | `[ExpressiveFor]` target type not found |
| EXP0015 | Error | `[ExpressiveFor]` target member not found |
| EXP0019 | Error | `[ExpressiveFor]` conflicts with existing `[Expressive]` |
| EXP0020 | Error | Duplicate `[ExpressiveFor]` mapping |

## How It Works

ExpressiveSharp uses two Roslyn source generators:

1. **`ExpressiveGenerator`** — Finds `[Expressive]` and `[ExpressiveFor]` members, analyzes them at the semantic level (IOperation), and generates `Expression<Func<...>>` factory code using `Expression.*` calls. Registers them in a per-assembly expression registry for runtime lookup.

2. **`PolyfillInterceptorGenerator`** — Uses C# 13 method interceptors to replace `ExpressionPolyfill.Create` calls and `IRewritableQueryable<T>` LINQ methods at their call sites, converting lambdas to expression trees at compile time.

All expression trees are generated at compile time. There is no runtime reflection or expression compilation.

## FAQ

### Does this have any runtime overhead?

No practical impact. The source generators emit `Expression.*` factory calls at compile time. At runtime, `ExpandExpressives()` (or `UseExpressives()` in EF Core) replaces opaque property accesses with the pre-built expressions — this adds a small cost on first execution, but LINQ providers like EF Core cache the expanded query afterward. There is no runtime reflection, no `Compile()`, and no expression tree parsing.

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

ExpressiveSharp is its spiritual successor. See the [Migration Guide](docs/guide/migration-from-projectables.md) for a step-by-step walkthrough including automated code fixers.

Key improvements: broader C# syntax support (switch expressions, pattern matching, string interpolation, tuples), customizable transformer pipeline, inline expression creation via `ExpressionPolyfill.Create`, modern syntax in LINQ chains via `IRewritableQueryable<T>`, and no EF Core coupling.

## Requirements

| | .NET 8.0 | .NET 10.0 |
|---|---|---|
| **ExpressiveSharp** | C# 12 | C# 14 |
| **ExpressiveSharp.EntityFrameworkCore** | EF Core 8.x | EF Core 10.x |
| **ExpressiveSharp.EntityFrameworkCore.RelationalExtensions** | EF Core 8.x | EF Core 10.x |

## Contributing

Development docs for contributors:

- [Testing Strategy](docs/advanced/testing-strategy.md) — snapshot tests, functional tests, and test consumers
- [IOperation to Expression Mapping](docs/advanced/ioperation-mapping.md) — reference table for the expression tree emitter

```bash
dotnet build    # Build all projects
dotnet test     # Run all tests
```

## License

MIT
