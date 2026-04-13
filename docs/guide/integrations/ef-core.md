# EF Core

The `ExpressiveSharp.EntityFrameworkCore` package provides first-class integration with Entity Framework Core for all relational providers (SQL Server, PostgreSQL, SQLite, MySQL, Oracle) and Cosmos DB. This page covers EF Core-specific setup and features.

## Installation

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

Depends on `ExpressiveSharp` (core runtime) and includes Roslyn analyzers and code fixes.

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

1. **Expands `[Expressive]` member references** — walks query expression trees and replaces opaque property/method accesses with the generated expression trees
2. **Marks `[Expressive]` properties as unmapped** — adds a model convention that tells EF Core to ignore these properties in the database model (no corresponding column)
3. **Applies database-friendly transformers** (in this order):
   - `ConvertLoopsToLinq` — converts loop expressions to LINQ method calls
   - `RemoveNullConditionalPatterns` — strips null-check ternaries for SQL providers
   - `FlattenTupleComparisons` — rewrites tuple field access to direct comparisons
   - `FlattenConcatArrayCalls` — flattens `string.Concat(string[])` into chained 2/3/4-arg `Concat` calls
   - `FlattenBlockExpressions` — inlines block-local variables and removes `Expression.Block` nodes

::: warning String interpolation format specifiers
String interpolation with format specifiers like `$"{Price:F2}"` generates `ToString(format)` at the expression tree level. EF Core cannot translate `ToString(string)` to SQL — in a final `Select` projection this silently falls back to client evaluation, but in `Where`, `OrderBy`, or other server-evaluated positions it throws at runtime. Simple interpolation without format specifiers (e.g., `$"Order #{Id}"`) is usually server-translatable because EF Core natively translates the 2/3/4-argument `string.Concat` overloads to SQL concatenation. For interpolations with 5+ parts, the emitter produces `string.Concat(string[])`; the `FlattenConcatArrayCalls` transformer listed above rewrites this into EF Core-translatable `Concat` calls.
:::

## ExpressiveDbSet\<T\>

`ExpressiveDbSet<T>` is the primary API for using modern syntax directly on a `DbSet<T>`. It combines `[Expressive]` member expansion with `IExpressiveQueryable<T>` modern syntax support:

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

With `ExpressiveDbSet<T>`, modern C# syntax works directly — no `.AsExpressive()` needed:

::: expressive-sample
db.Orders
    .Where(o => o.Customer.Email != null)
    .Select(o => new
    {
        o.Id,
        Total = o.Items.Sum(i => i.UnitPrice * i.Quantity),
        Email = o.Customer.Email
    })
:::

## Include and ThenInclude

`ExpressiveDbSet<T>` preserves chain continuity across `Include`/`ThenInclude` calls:

```csharp
var orders = ctx.Orders
    .Include(o => o.Customer)
    .ThenInclude(c => c.Orders)
    .Where(o => o.Customer?.Name != null)
    .ToList();
```

String-based includes are also supported:

```csharp
var orders = ctx.Orders
    .Include("Customer")
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
    .FirstOrDefaultAsync(o => o.Price * o.Quantity > 100);

// Async aggregation
var totalRevenue = await ctx.Orders
    .SumAsync(o => o.Price * o.Quantity);
```

Supported async methods:

| Category | Methods |
|----------|---------|
| **Predicates** | `AnyAsync`, `AllAsync`, `CountAsync`, `LongCountAsync` |
| **Element access** | `FirstAsync`, `FirstOrDefaultAsync`, `LastAsync`, `LastOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync` |
| **Aggregation** | `SumAsync` (all numeric types), `AverageAsync` (all numeric types), `MinAsync`, `MaxAsync` |

## Chain Continuity Stubs

The following EF Core operations preserve the `ExpressiveDbSet<T>`/`IExpressiveQueryable<T>` chain:

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
        .SetProperty(o => o.Status, o => o.Price switch
        {
            >= 100 => OrderStatus.Paid,
            >= 50  => OrderStatus.Pending,
            _      => OrderStatus.Refunded
        }));
```

Switch expressions and null-conditional operators inside `SetProperty` value lambdas are normally rejected by the C# compiler in expression tree contexts. The source generator converts them to `CASE WHEN` and `COALESCE` SQL expressions.

::: info
`ExecuteDelete` works on `IExpressiveQueryable<T>` / `ExpressiveDbSet<T>` without any additional setup — it has no lambda parameter, so no interception is needed.
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

The built-in `RelationalExtensions` package (for window functions) uses this plugin architecture.

## NuGet Packages

| Package | Description |
|---------|-------------|
| [`ExpressiveSharp`](https://www.nuget.org/packages/ExpressiveSharp/) | Core runtime — `[Expressive]` attribute, source generator, expression expansion, transformers |
| [`ExpressiveSharp.EntityFrameworkCore`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore/) | EF Core integration — `UseExpressives()`, `ExpressiveDbSet<T>`, Include/ThenInclude, async methods, analyzers and code fixes |
| [`ExpressiveSharp.EntityFrameworkCore.RelationalExtensions`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore.RelationalExtensions/) | Relational extensions — `ExecuteUpdate`/`ExecuteUpdateAsync` with modern syntax, SQL window functions (ROW_NUMBER, RANK, DENSE_RANK, NTILE) |

::: info
The `ExpressiveSharp.EntityFrameworkCore` package bundles Roslyn analyzers and code fixes from `ExpressiveSharp.EntityFrameworkCore.CodeFixers`. These provide compile-time diagnostics and IDE quick-fix actions for common issues like missing `[Expressive]` attributes.
:::

## Next Steps

- [Window Functions](../window-functions) — SQL window functions via the RelationalExtensions package
- [IExpressiveQueryable\<T\>](../expressive-queryable) — the core provider-agnostic API
- [[Expressive] Properties](../expressive-properties) — computed properties in depth
