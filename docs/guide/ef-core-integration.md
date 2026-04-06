# EF Core Integration

ExpressiveSharp provides first-class EF Core integration through the `ExpressiveSharp.EntityFrameworkCore` package. This page covers the full setup and all EF Core-specific features.

## Installation

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

This package depends on `ExpressiveSharp` (core runtime) and includes Roslyn analyzers and code fixes.

## UseExpressives() Configuration

Call `UseExpressives()` on your `DbContextOptionsBuilder`:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives()
    .Options;
```

Or with dependency injection:

```csharp
services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString)
           .UseExpressives());
```

### What UseExpressives() Does

`UseExpressives()` automatically:

1. **Expands `[Expressive]` member references** -- walks query expression trees and replaces opaque property/method accesses with the generated expression trees
2. **Marks `[Expressive]` properties as unmapped** -- adds a model convention that tells EF Core to ignore these properties in the database model (no corresponding column)
3. **Applies database-friendly transformers** (in this order):
   - `ConvertLoopsToLinq` -- converts loop expressions to LINQ method calls
   - `RemoveNullConditionalPatterns` -- strips null-check ternaries for SQL providers
   - `FlattenTupleComparisons` -- rewrites tuple field access to direct comparisons
   - `FlattenConcatArrayCalls` -- flattens `string.Concat(string[])` into chained 2/3/4-arg `Concat` calls
   - `FlattenBlockExpressions` -- inlines block-local variables and removes `Expression.Block` nodes

::: warning String interpolation format specifiers
String interpolation with format specifiers like `$"{Price:F2}"` generates `ToString(format)` at the expression tree level. EF Core cannot translate `ToString(string)` to SQL -- in a final `Select` projection this silently falls back to client evaluation, but in `Where`, `OrderBy`, or other server-evaluated positions it throws at runtime. Simple interpolation without format specifiers (e.g., `$"Order #{Id}"`) is usually server-translatable because EF Core natively translates the 2/3/4-argument `string.Concat` overloads to SQL concatenation. For interpolations with 5+ parts, the emitter produces `string.Concat(string[])`; the `FlattenConcatArrayCalls` transformer listed above rewrites this into EF Core-translatable `Concat` calls.
:::

## ExpressiveDbSet\<T\>

`ExpressiveDbSet<T>` is the primary API for using modern syntax directly on a `DbSet<T>`. It combines `[Expressive]` member expansion with `IRewritableQueryable<T>` modern syntax support:

```csharp
public class MyDbContext : DbContext
{
    public DbSet<Customer> CustomersRaw { get; set; }

    // Shorthand for Set<Order>().AsExpressiveDbSet()
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
}
```

::: tip
`this.ExpressiveSet<T>()` is a convenience extension method that calls `Set<T>().AsExpressiveDbSet()`. You can also call `.AsExpressiveDbSet()` on any `DbSet<T>` or `IQueryable<T>` directly.
:::

With `ExpressiveDbSet<T>`, modern C# syntax works directly -- no `.AsExpressive()` needed:

```csharp
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        o.Total,           // [Expressive] property -- expanded automatically
        Grade = o.GetGrade() // [Expressive] method -- expanded automatically
    })
    .ToList();
```

## Include and ThenInclude

`ExpressiveDbSet<T>` preserves chain continuity across `Include`/`ThenInclude` calls:

```csharp
var orders = ctx.Orders
    .Include(o => o.Customer)
    .ThenInclude(c => c.Address)
    .Where(o => o.Customer?.Name != null)
    .Select(o => new
    {
        o.Id,
        Name = o.Customer?.Name,
        City = o.Customer?.Address?.City
    })
    .ToList();
```

String-based includes are also supported:

```csharp
var orders = ctx.Orders
    .Include("Customer.Address")
    .Where(o => o.Customer?.Name != null)
    .ToList();
```

## Async Methods

All EF Core async methods that accept lambdas are supported with modern syntax:

```csharp
// Async predicate
var hasAliceOrders = await ctx.Orders
    .AnyAsync(o => o.Customer?.Name == "Alice");

// Async element access
var firstOrder = await ctx.Orders
    .FirstOrDefaultAsync(o => o.Total > 100);

// Async aggregation
var totalRevenue = await ctx.Orders
    .SumAsync(o => o.Total);

var avgPrice = await ctx.Orders
    .AverageAsync(o => o.Price);

var maxTotal = await ctx.Orders
    .MaxAsync(o => o.Total);
```

Supported async methods:

| Category | Methods |
|----------|---------|
| **Predicates** | `AnyAsync`, `AllAsync`, `CountAsync`, `LongCountAsync` |
| **Element access** | `FirstAsync`, `FirstOrDefaultAsync`, `LastAsync`, `LastOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync` |
| **Aggregation** | `SumAsync` (all numeric types), `AverageAsync` (all numeric types), `MinAsync`, `MaxAsync` |

## Chain Continuity Stubs

The following EF Core operations preserve the `ExpressiveDbSet<T>`/`IRewritableQueryable<T>` chain:

```csharp
var orders = ctx.Orders
    .AsNoTracking()
    .IgnoreQueryFilters()
    .IgnoreAutoIncludes()
    .TagWith("Dashboard query")
    .Where(o => o.Customer?.Email != null)
    .ToList();
```

Supported chain-preserving operations:

- `AsNoTracking()`, `AsNoTrackingWithIdentityResolution()`, `AsTracking()`
- `IgnoreQueryFilters()`, `IgnoreAutoIncludes()`
- `TagWith(tag)`, `TagWithCallSite()`

## Bulk Updates with ExecuteUpdate

With the `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` package, you can use modern C# syntax inside `ExecuteUpdate` / `ExecuteUpdateAsync`:

```csharp
// Requires: .UseExpressives(o => o.UseRelationalExtensions())
ctx.Orders
    .ExecuteUpdate(s => s
        .SetProperty(o => o.Tag, o => o.Price switch
        {
            >= 100 => "Premium",
            >= 50  => "Standard",
            _      => "Budget"
        }));

// Async variant
await ctx.Orders
    .ExecuteUpdateAsync(s => s.SetProperty(
        o => o.Tag,
        o => o.Customer?.Name ?? "Unknown"));
```

Switch expressions and null-conditional operators inside `SetProperty` value lambdas are normally rejected by the C# compiler in expression tree contexts. The source generator converts them to `CASE WHEN` and `COALESCE` SQL expressions.

::: info
`ExecuteDelete` works on `IRewritableQueryable<T>` / `ExpressiveDbSet<T>` without any additional setup — it has no lambda parameter, so no interception is needed.
:::

::: warning
This feature is available on EF Core 8 and 9. EF Core 10 changed the `ExecuteUpdate` API to use `Action<UpdateSettersBuilder<T>>`, which natively supports modern C# syntax in the outer lambda. For inner `SetProperty` value expressions on EF Core 10, use `ExpressionPolyfill.Create()`.
:::

## Plugin Architecture

`UseExpressives()` accepts an optional configuration callback for registering plugins:

```csharp
options.UseExpressives(o =>
{
    o.UseRelationalExtensions();  // built-in plugin for window functions
    o.AddPlugin(new MyCustomPlugin());
});
```

### Implementing a Custom Plugin

Implement `IExpressivePlugin` to add custom EF Core services and transformers:

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
```

Plugins can:
- Register additional EF Core services via dependency injection
- Provide custom expression tree transformers that are applied to all queries

The built-in `RelationalExtensions` package (for window functions) uses this plugin architecture.

## NuGet Packages

| Package | Description |
|---------|-------------|
| [`ExpressiveSharp`](https://www.nuget.org/packages/ExpressiveSharp/) | Core runtime -- `[Expressive]` attribute, source generator, expression expansion, transformers |
| [`ExpressiveSharp.EntityFrameworkCore`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore/) | EF Core integration -- `UseExpressives()`, `ExpressiveDbSet<T>`, Include/ThenInclude, async methods, analyzers and code fixes |
| [`ExpressiveSharp.EntityFrameworkCore.RelationalExtensions`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore.RelationalExtensions/) | Relational extensions -- `ExecuteUpdate`/`ExecuteUpdateAsync` with modern syntax, SQL window functions (ROW_NUMBER, RANK, DENSE_RANK, NTILE) |

::: info
The `ExpressiveSharp.EntityFrameworkCore` package bundles Roslyn analyzers and code fixes from `ExpressiveSharp.EntityFrameworkCore.CodeFixers`. These provide compile-time diagnostics and IDE quick-fix actions for common issues like missing `[Expressive]` attributes.
:::

## Full Example

```csharp
using ExpressiveSharp;
using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Tag { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
}

public class AppDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=app.db").UseExpressives();
}

// Query with full feature set
using var ctx = new AppDbContext();

var results = await ctx.Orders
    .Include(o => o.Customer)
    .AsNoTracking()
    .Where(o => o.Customer?.Email != null)
    .OrderByDescending(o => o.Total)
    .Select(o => new
    {
        o.Id,
        o.Total,
        Email = o.Customer?.Email,
        Grade = o.GetGrade()
    })
    .ToListAsync();
```

## Next Steps

- [Window Functions](./window-functions) -- SQL window functions via the RelationalExtensions package
- [IRewritableQueryable\<T\>](./rewritable-queryable) -- modern syntax on any `IQueryable`
- [[Expressive] Properties](./expressive-properties) -- computed properties in depth
- [Quick Start](./quickstart) -- minimal setup walkthrough
