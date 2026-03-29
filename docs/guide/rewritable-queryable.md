# IRewritableQueryable\<T\>

`IRewritableQueryable<T>` enables modern C# syntax directly in LINQ chains -- null-conditional operators, switch expressions, and pattern matching work in `.Where()`, `.Select()`, `.OrderBy()`, and more, on any `IQueryable<T>`.

## Basic Usage

Wrap any `IQueryable<T>` with `.WithExpressionRewrite()`:

```csharp
using ExpressiveSharp;

var results = queryable
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, Name = o.Customer?.Name ?? "Unknown" })
    .OrderBy(o => o.Name)
    .ToList();
```

The source generator intercepts these calls at compile time and rewrites the delegate lambdas to proper expression trees. There is no runtime overhead from delegate-to-expression conversion.

## How It Works

When you call `.WithExpressionRewrite()`, you get back an `IRewritableQueryable<T>` wrapper. This wrapper exposes the same LINQ methods as `IQueryable<T>`, but they accept `Func<...>` delegates instead of `Expression<Func<...>>`.

At compile time, the `PolyfillInterceptorGenerator` uses C# 13 method interceptors to replace each call site with code that:

1. Converts the delegate lambda into an `Expression<Func<...>>` using `Expression.*` factory calls
2. Forwards the expression to the underlying `IQueryable<T>` LINQ method

The delegate stubs are never actually called at runtime -- they are completely replaced by the interceptor.

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

**Chain-preserving operators** (return `IRewritableQueryable<T>`):
`Take`, `Skip`, `Distinct`, `Reverse`, `DefaultIfEmpty`, `Append`, `Prepend`, `Concat`, `Union`, `Intersect`, `Except`, `SkipWhile`, `TakeWhile`

**Comparer overloads** (`IEqualityComparer<T>`, `IComparer<T>`) are also supported.

### .NET 10+ Additional Methods

On .NET 10 and later, these additional methods are available:

- `LeftJoin`
- `RightJoin`
- `CountBy`
- `AggregateBy`
- `Index`

## EF Core: Include and ThenInclude

When using `IRewritableQueryable<T>` with EF Core, `Include` and `ThenInclude` are fully supported with chain continuity:

```csharp
var orders = ctx.Set<Order>()
    .WithExpressionRewrite()
    .Include(o => o.Customer)
    .ThenInclude(c => c.Address)
    .Where(o => o.Customer?.Email != null)
    .ToList();
```

The `Include`/`ThenInclude` calls return `IIncludableRewritableQueryable<TEntity, TProperty>`, a hybrid interface that preserves both the includable chain and the rewritable chain.

::: info
`Include` and `ThenInclude` accept standard `Expression<Func<...>>` lambdas (not rewritten delegates), since navigation property paths do not typically need modern syntax. The chain continuity ensures you can seamlessly go from `Include`/`ThenInclude` back to rewritable LINQ methods like `Where` and `Select`.
:::

## EF Core: Async Lambda Methods

All EF Core async methods that accept a lambda predicate or selector are supported on `IRewritableQueryable<T>`:

**Async predicates:**
`AnyAsync`, `AllAsync`, `CountAsync`, `LongCountAsync`

**Async element access:**
`FirstAsync`, `FirstOrDefaultAsync`, `LastAsync`, `LastOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync`

**Async aggregation:**
`SumAsync` (all numeric types), `AverageAsync` (all numeric types), `MinAsync`, `MaxAsync`

```csharp
var hasExpensive = await ctx.Set<Order>()
    .WithExpressionRewrite()
    .AnyAsync(o => o.Price switch
    {
        >= 100 => true,
        _      => false,
    });

var total = await ctx.Set<Order>()
    .WithExpressionRewrite()
    .SumAsync(o => o.Customer?.Email != null ? o.Price : 0);
```

These async methods are forwarded to `EntityFrameworkQueryableExtensions` at compile time via the `[PolyfillTarget]` attribute.

## EF Core: Chain Continuity Stubs

The following EF Core operations preserve the `IRewritableQueryable<T>` chain, so you can continue using modern syntax after calling them:

- `AsNoTracking()`, `AsNoTrackingWithIdentityResolution()`, `AsTracking()`
- `IgnoreQueryFilters()`, `IgnoreAutoIncludes()`
- `TagWith(tag)`, `TagWithCallSite()`

```csharp
var orders = ctx.Set<Order>()
    .WithExpressionRewrite()
    .AsNoTracking()
    .IgnoreQueryFilters()
    .TagWith("Admin query")
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, Email = o.Customer?.Email })
    .ToList();
```

## IAsyncEnumerable Support

`IRewritableQueryable<T>` supports `AsAsyncEnumerable()` for streaming results:

```csharp
await foreach (var order in ctx.Set<Order>()
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Name != null)
    .AsAsyncEnumerable())
{
    Console.WriteLine(order.Id);
}
```

## ExpressiveDbSet\<T\> vs WithExpressionRewrite()

With EF Core, you have two options for modern syntax:

| | `ExpressiveDbSet<T>` | `.WithExpressionRewrite()` |
|---|---|---|
| **Setup** | Property on `DbContext` | Call on any `IQueryable<T>` |
| **`[Expressive]` expansion** | Automatic | Requires `UseExpressives()` separately |
| **Include/ThenInclude** | Yes | Yes |
| **Async methods** | Yes | Yes |
| **Works outside EF Core** | No | Yes (any `IQueryable<T>`) |

::: tip
For EF Core projects, `ExpressiveDbSet<T>` is the most convenient option -- it combines both `[Expressive]` expansion and modern syntax in one API. Use `.WithExpressionRewrite()` when you need modern syntax on a non-EF Core `IQueryable<T>` or want explicit control over the wrapping.
:::

## Next Steps

- [EF Core Integration](./ef-core-integration) -- full setup with `ExpressiveDbSet<T>` and `UseExpressives()`
- [ExpressionPolyfill.Create](./expression-polyfill) -- inline expression trees without LINQ chains
- [[Expressive] Properties](./expressive-properties) -- reusable computed properties
