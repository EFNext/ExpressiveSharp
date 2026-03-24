# ExpressiveSharp

Source generator that enables modern C# syntax in LINQ expression trees.

## The Problem

C# expression trees (`Expression<Func<...>>`) only support a restricted subset of the language — no null-conditional operators (`?.`), no switch expressions, no pattern matching. This forces you to write clunky workarounds or give up on computed properties in EF Core queries entirely.

ExpressiveSharp lifts this restriction using Roslyn source generators. Write natural C# and let the generator build the expression tree for you.

```csharp
// Without ExpressiveSharp — compiler error: "An expression tree may not contain
// a null propagating operator"
db.Orders.Where(o => o.Customer?.Email != null);

// With ExpressiveSharp — just works
db.Orders
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Email != null);
```

## Quick Start

```bash
dotnet add package ExpressiveSharp
# Optional: EF Core integration
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

Mark a computed property with `[Expressive]` and use it in queries:

```csharp
public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }

    [Expressive]
    public double Total => Price * Quantity;
}

// The source generator creates a companion expression tree for Total,
// so EF Core can translate it to SQL
var expensive = db.Orders
    .Select(o => new { o.Id, o.Total })
    .Where(o => o.Total > 100)
    .ToList();
```

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

    // Block-bodied methods
    [Expressive]
    public string GetCategory()
    {
        var threshold = Quantity * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}
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

This automatically expands `[Expressive]` members in queries and applies database-friendly transformers (removes null-conditional patterns, flattens block expressions).

For direct access on `DbSet`, use `ExpressiveDbSet<T>`:

```csharp
public class MyDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => Set<Order>().AsExpressiveDbSet();
}

// Modern syntax works directly — no .WithExpressionRewrite() needed
ctx.Orders.Where(o => o.Customer?.Name == "Alice");
```

## Supported C# Features

**Legend:** supported | partial | not supported

### Expression-Level

| Feature | Status |
|---|---|
| Switch expressions | Supported |
| Pattern matching (constant, type, relational, logical, property, positional) | Supported |
| Declaration patterns with named variables | Partial — works in switch arms only |
| String interpolation | Supported |
| Null-conditional `?.` (member access and indexer) | Supported |
| Tuple literals | Supported |
| C# 14 extension members | Supported |
| Enum method expansion | Supported |
| Dictionary indexer initializers | Supported |
| `this`/`base` references | Supported |
| List patterns (fixed-length and slice) | Supported |
| Index/range (`^1`, `1..3`) | Supported |
| `with` expressions (records) | Supported |
| Collection expressions (`[1, 2, 3]`) | Supported — spread (`..`) not yet supported |

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

Property assignments, local variables, `if`/`else`, and `base()`/`this()` initializer chains are all supported.

## Expression Transformers

ExpressiveSharp generates faithful expression trees that mirror the original C# code. Transformers adapt these trees for specific consumers at runtime.

### Built-in Transformers

**`RemoveNullConditionalPatterns`** — Strips null-check ternaries (`x != null ? x.Prop : default` becomes `x.Prop`). Useful for databases that handle null propagation natively.

```csharp
[Expressive(RemoveNullConditionalPatterns = true)]
public string? CustomerName => Customer?.Name;
```

**`FlattenBlockExpressions`** — Inlines block-local variables and removes `Expression.Block` nodes. Required for LINQ providers that don't support block expressions (including EF Core).

```csharp
[Expressive(FlattenBlockExpressions = true)]
public string GetCategory()
{
    var threshold = Quantity * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}
```

Both transformers are applied automatically when using `UseExpressives()` with EF Core.

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

1. **`ExpressiveGenerator`** — Finds `[Expressive]` members and generates `Expression<Func<...>>` method bodies using `Expression.*` factory calls. Registers them in a per-assembly expression registry for runtime lookup.

2. **`PolyfillInterceptorGenerator`** — Uses C# 13 method interceptors to replace `ExpressionPolyfill.Create` calls and `IRewritableQueryable<T>` LINQ methods at their call sites, converting lambdas to expression trees at compile time.

All transformations happen at compile time. There is no runtime reflection or expression compilation.

## Requirements

| | .NET 8.0 | .NET 10.0 |
|---|---|---|
| **ExpressiveSharp** | C# 12 | C# 14 |
| **ExpressiveSharp.EntityFrameworkCore** | EF Core 8.x | EF Core 10.x |

## Contributing

Development docs for contributors:

- [Testing Strategy](docs/testing-strategy.md) — snapshot tests, functional tests, and test consumers
- [IOperation to Expression Mapping](docs/ioperation-to-expression-mapping.md) — reference table for the expression tree emitter
- [Recovery Notes](docs/recovery-notes.md) — migration status from legacy syntax rewriting to IOperation-based emitter

```bash
dotnet build    # Build all projects
dotnet test     # Run all tests
```

## License

MIT
