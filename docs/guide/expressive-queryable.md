# IExpressiveQueryable\<T\>

`IExpressiveQueryable<T>` is the core provider-agnostic API. It enables modern C# syntax directly in LINQ chains — null-conditional operators, switch expressions, and pattern matching work in `.Where()`, `.Select()`, `.OrderBy()`, and more — on any `IQueryable<T>`.

## Basic Usage

Wrap any `IQueryable<T>` with `.AsExpressive()`:

::: expressive-sample
db.Customers
    .Where(c => c.Email != null && c.Email.Length > 5)
    .Select(c => new { c.Id, Name = c.Name })
    .OrderBy(c => c.Name)
:::

The source generator intercepts these calls at compile time and rewrites the delegate lambdas to proper expression trees. There is no runtime overhead from delegate-to-expression conversion.

## How It Works

When you call `.AsExpressive()`, you get back an `IExpressiveQueryable<T>` wrapper. This wrapper exposes the same LINQ methods as `IQueryable<T>`, but they accept `Func<...>` delegates instead of `Expression<Func<...>>`.

At compile time, the `PolyfillInterceptorGenerator` uses C# 13 method interceptors to replace each call site with code that:

1. Converts the delegate lambda into an `Expression<Func<...>>` using `Expression.*` factory calls
2. Forwards the expression to the underlying `IQueryable<T>` LINQ method

The delegate stubs are never actually called at runtime — they are completely replaced by the interceptor.

## Available LINQ Methods

Most common `Queryable` methods are supported:

**Filtering:**
`Where`, `Any`, `All`

**Projection:**
`Select`, `SelectMany`

**Ordering:**
`OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending`

**Grouping:**
`GroupBy`

**Joins:**
`Join`, `GroupJoin`, `Zip`

**Aggregation:**
`Sum`, `Average`, `Min`, `Max`, `Count`, `LongCount`

**Element access:**
`First`, `FirstOrDefault`, `Single`, `SingleOrDefault`, `Last`, `LastOrDefault` (and their predicate overloads)

**Set operations:**
`ExceptBy`, `IntersectBy`, `UnionBy`, `DistinctBy`

**Chain-preserving operators** (return `IExpressiveQueryable<T>`):
`Take`, `Skip`, `Distinct`, `Reverse`, `DefaultIfEmpty`, `Append`, `Prepend`, `Concat`, `Union`, `Intersect`, `Except`, `SkipWhile`, `TakeWhile`

**Comparer overloads** (`IEqualityComparer<T>`, `IComparer<T>`) are also supported.

### .NET 9 / .NET 10 Additional Methods

On .NET 9 and later: `CountBy`, `AggregateBy`, `Index`.

On .NET 10 and later (in addition to the above): `LeftJoin`, `RightJoin`.

## Pattern Matching and Switch Expressions

Switch expressions, null-conditional operators, and pattern matching compose naturally in the chain:

::: expressive-sample
db.Orders
    .Select(o => new
    {
        o.Id,
        Tier = o.Status switch
        {
            OrderStatus.Paid => "Confirmed",
            OrderStatus.Shipped => "Out for delivery",
            OrderStatus.Delivered => "Complete",
            _ => "Pending"
        }
    })
:::

## EF Core: Include and ThenInclude

When using `IExpressiveQueryable<T>` with EF Core, `Include` and `ThenInclude` are fully supported with chain continuity:

```csharp
var orders = ctx.Set<Order>()
    .AsExpressive()
    .Include(o => o.Customer)
    .ThenInclude(c => c.Orders)
    .Where(o => o.Customer?.Email != null)
    .ToList();
```

The `Include`/`ThenInclude` calls return `IIncludableExpressiveQueryable<TEntity, TProperty>`, a hybrid interface that preserves both the includable chain and the rewritable chain.

::: info
`Include` and `ThenInclude` accept standard `Expression<Func<...>>` lambdas (not rewritten delegates), since navigation property paths do not typically need modern syntax. The chain continuity ensures you can seamlessly go from `Include`/`ThenInclude` back to rewritable LINQ methods like `Where` and `Select`.
:::

## EF Core: Async Lambda Methods

All EF Core async methods that accept a lambda predicate or selector are supported on `IExpressiveQueryable<T>`:

**Async predicates:**
`AnyAsync`, `AllAsync`, `CountAsync`, `LongCountAsync`

**Async element access:**
`FirstAsync`, `FirstOrDefaultAsync`, `LastAsync`, `LastOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync`

**Async aggregation:**
`SumAsync` (all numeric types), `AverageAsync` (all numeric types), `MinAsync`, `MaxAsync`

```csharp
var hasExpensive = await ctx.Set<Order>()
    .AsExpressive()
    .AnyAsync(o => o.Price switch
    {
        >= 100 => true,
        _      => false,
    });

var total = await ctx.Set<Order>()
    .AsExpressive()
    .SumAsync(o => o.Customer?.Email != null ? o.Price : 0);
```

These async methods are forwarded to `EntityFrameworkQueryableExtensions` at compile time via the `[PolyfillTarget]` attribute.

## EF Core: Chain Continuity Stubs

The following EF Core operations preserve the `IExpressiveQueryable<T>` chain, so you can continue using modern syntax after calling them:

- `AsNoTracking()`, `AsNoTrackingWithIdentityResolution()`, `AsTracking()`
- `IgnoreQueryFilters()`, `IgnoreAutoIncludes()`
- `TagWith(tag)`, `TagWithCallSite()`

```csharp
var orders = ctx.Set<Order>()
    .AsExpressive()
    .AsNoTracking()
    .IgnoreQueryFilters()
    .TagWith("Admin query")
    .Where(o => o.Customer?.Email != null)
    .ToList();
```

## EF Core: Bulk Updates with ExecuteUpdate

::: info
Requires the `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` package and `.UseExpressives(o => o.UseRelationalExtensions())` configuration. Available on EF Core 8 and 9. On EF Core 10+, `ExecuteUpdate` natively accepts delegates — use `ExpressionPolyfill.Create()` for modern syntax in individual `SetProperty` value expressions.
:::

`ExecuteUpdate` and `ExecuteUpdateAsync` are supported on `IExpressiveQueryable<T>`, enabling modern C# syntax inside `SetProperty` value expressions — which is normally impossible in expression trees:

```csharp
ctx.ExpressiveSet<Order>()
    .ExecuteUpdate(s => s
        .SetProperty(o => o.Status, o => o.Price switch
        {
            > 100 => OrderStatus.Paid,
            > 50  => OrderStatus.Pending,
            _     => OrderStatus.Refunded
        }));
```

This generates a single SQL `UPDATE` with `CASE WHEN` and `COALESCE` expressions — no entity loading required.

`ExecuteDelete` works out of the box on `IExpressiveQueryable<T>` without any stubs (it has no lambda parameter).

## IAsyncEnumerable Support

`IExpressiveQueryable<T>` supports `AsAsyncEnumerable()` for streaming results:

```csharp
await foreach (var order in ctx.Set<Order>()
    .AsExpressive()
    .Where(o => o.Customer?.Name != null)
    .AsAsyncEnumerable())
{
    Console.WriteLine(order.Id);
}
```

## Choosing the Right Entry Point

| Entry point | When to use |
|---|---|
| `.AsExpressive()` on `IQueryable<T>` | Any provider (EF Core, MongoDB, custom, in-memory) |
| `ExpressiveDbSet<T>` on `DbContext` | EF Core — preferred, also triggers `[Expressive]` expansion via `UseExpressives()` |
| `.AsExpressive()` on `IMongoCollection<T>` | MongoDB |
| `ExpressionPolyfill.Create(...)` | You need a bare `Expression<T>` (no queryable involved) |

::: tip
For EF Core projects, `ExpressiveDbSet<T>` is the most convenient option — it combines both `[Expressive]` expansion and modern syntax in one API. Use `.AsExpressive()` when you need modern syntax on a non-EF Core `IQueryable<T>` or want explicit control over the wrapping.
:::

## Next Steps

- [EF Core Integration](./integrations/ef-core) — full setup with `ExpressiveDbSet<T>` and `UseExpressives()`
- [MongoDB Integration](./integrations/mongodb) — MongoDB-specific setup
- [ExpressionPolyfill.Create](./expression-polyfill) — inline expression trees without LINQ chains
- [[Expressive] Properties](./expressive-properties) — reusable computed properties
