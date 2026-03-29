# Modern Syntax in LINQ Chains

This recipe shows how to use modern C# syntax -- null-conditional operators, switch expressions, pattern matching -- directly inside LINQ method chains like `.Where()`, `.Select()`, and `.OrderBy()`, without defining separate `[Expressive]` members.

## The Problem

Expression trees only support a restricted subset of C#. Try to use `?.` in a LINQ lambda and you get:

```
error CS8072: An expression tree lambda may not contain a null propagating operator
```

The same limitation applies to switch expressions, pattern matching, and many other modern C# features. Without ExpressiveSharp, your only option is to write verbose ternary chains:

```csharp
// Without ExpressiveSharp -- ugly ternary chains
var results = dbContext.Orders
    .Where(o => o.Customer != null ? o.Customer.Email != null : false)
    .Select(o => new
    {
        o.Id,
        Name = o.Customer != null ? o.Customer.Name : "Unknown",
        Grade = o.Price >= 100 ? "Premium" : (o.Price >= 50 ? "Standard" : "Budget")
    })
    .ToList();
```

## Three Solutions

ExpressiveSharp offers three ways to use modern syntax in LINQ chains. Each targets a different scenario.

### 1. `IRewritableQueryable<T>` with `.WithExpressionRewrite()`

Works with **any** `IQueryable<T>` -- not tied to EF Core:

```csharp
var results = queryable
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        Name = o.Customer?.Name ?? "Unknown",
        Grade = o.Price switch
        {
            >= 100 => "Premium",
            >= 50  => "Standard",
            _      => "Budget"
        }
    })
    .OrderBy(o => o.Name)
    .ToList();
```

The source generator intercepts these calls at compile time and rewrites the delegate lambdas to expression trees. The chain continues as an `IRewritableQueryable<T>`, preserving the ability to use modern syntax in subsequent calls.

### 2. `ExpressiveDbSet<T>` -- For EF Core

A shorthand for EF Core projects. `ExpressiveDbSet<T>` wraps a `DbSet<T>` and provides `IRewritableQueryable<T>` behavior automatically:

```csharp
public class MyDbContext : DbContext
{
    // Shorthand for Set<Order>().AsExpressiveDbSet()
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
}
```

```csharp
// Modern syntax works directly -- no .WithExpressionRewrite() needed
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        o.Total,
        Grade = o.GetGrade()
    })
    .ToList();
```

`ExpressiveDbSet<T>` also preserves chain continuity for EF Core-specific operations:

```csharp
var result = await ctx.Orders
    .Include(o => o.Customer)
    .ThenInclude(c => c.Address)
    .AsNoTracking()
    .Where(o => o.Customer?.Name == "Alice")
    .FirstOrDefaultAsync(o => o.Total > 100);
```

### 3. `ExpressionPolyfill.Create` -- For Standalone Expression Trees

When you need an `Expression<TDelegate>` without a queryable at all:

```csharp
// Returns Expression<Func<Order, int?>> -- intercepted at compile time
var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);

// With transformers
var expr = ExpressionPolyfill.Create(
    (Order o) => o.Customer?.Email,
    new RemoveNullConditionalPatterns());
```

This is useful for building expression trees that you pass to other APIs, or for testing.

## Practical Examples

### Null-Conditional in Where

```csharp
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Where(o => o.Customer?.Address?.City == "Seattle")
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o".*
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
LEFT JOIN "Addresses" AS "a" ON "c"."AddressId" = "a"."Id"
WHERE "c"."Email" IS NOT NULL
  AND "a"."City" = 'Seattle'
```

### Switch Expressions in Select

```csharp
var results = ctx.Orders
    .Select(o => new
    {
        o.Id,
        Tier = o.Price switch
        {
            >= 100 => "Premium",
            >= 50  => "Standard",
            _      => "Budget"
        },
        Priority = o.Quantity switch
        {
            >= 100 => "Bulk",
            >= 10  => "Normal",
            _      => "Small"
        }
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       CASE
           WHEN "o"."Price" >= 100.0 THEN 'Premium'
           WHEN "o"."Price" >= 50.0 THEN 'Standard'
           ELSE 'Budget'
       END AS "Tier",
       CASE
           WHEN "o"."Quantity" >= 100 THEN 'Bulk'
           WHEN "o"."Quantity" >= 10 THEN 'Normal'
           ELSE 'Small'
       END AS "Priority"
FROM "Orders" AS "o"
```

### Pattern Matching in OrderBy

```csharp
var results = ctx.Orders
    .OrderBy(o => o.Price switch
    {
        >= 100 => 1,
        >= 50  => 2,
        _      => 3
    })
    .ThenBy(o => o.Customer?.Name ?? "ZZZ")
    .ToList();
```

### Combining [Expressive] Members with Inline Modern Syntax

The two approaches compose naturally. `[Expressive]` members are expanded, and inline modern syntax is rewritten, all in the same query:

```csharp
var results = ctx.Orders
    .Where(o => o.IsRecent && o.Customer?.Region == "US")
    .Select(o => new
    {
        o.Id,
        o.Total,                          // [Expressive] property
        o.CustomerEmail,                  // [Expressive] property with ?.
        Tier = o.Total switch             // inline switch on [Expressive] result
        {
            >= 1000 => "Premium",
            >= 250  => "Standard",
            _       => "Basic"
        }
    })
    .ToList();
```

## When to Use Which Approach

| Scenario | Approach |
|---|---|
| EF Core project, modern syntax on `DbSet` | `ExpressiveDbSet<T>` |
| Any `IQueryable`, modern syntax in chains | `.WithExpressionRewrite()` |
| Standalone expression tree, no queryable | `ExpressionPolyfill.Create` |
| Reusable logic across multiple queries | `[Expressive]` property or method |
| One-off query logic, not reused elsewhere | Inline modern syntax via the above |

::: tip Combine both approaches
Use `[Expressive]` for shared business logic (computed properties, filters, classifications) and inline modern syntax for query-specific projections and conditions. They complement each other.
:::

## Available LINQ Methods

`IRewritableQueryable<T>` and `ExpressiveDbSet<T>` support most standard `Queryable` methods:

**Filtering:** `Where`, `Any`, `All`, `Contains`

**Projection:** `Select`, `SelectMany`

**Ordering:** `OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending`

**Grouping:** `GroupBy`

**Joins:** `Join`, `GroupJoin`, `Zip`

**Aggregation:** `Sum`, `Average`, `Min`, `Max`, `Count`, `LongCount`

**Element access:** `First`, `FirstOrDefault`, `Single`, `SingleOrDefault`, `Last`, `LastOrDefault`, `ElementAt`, `ElementAtOrDefault`

**Set operations:** `ExceptBy`, `IntersectBy`, `UnionBy`, `DistinctBy`

**Non-lambda (chain-preserving):** `Take`, `Skip`, `Distinct`, `Reverse`, `Append`, `Prepend`, `DefaultIfEmpty`, `Concat`, `Union`, `Intersect`, `Except`

**EF Core (ExpressiveDbSet only):** `Include`, `ThenInclude`, `AsNoTracking`, `IgnoreQueryFilters`, `TagWith`, `AnyAsync`, `FirstAsync`, `SumAsync`, and all other async lambda methods

On .NET 10+, additional methods are available: `LeftJoin`, `RightJoin`, `CountBy`, `AggregateBy`, and `Index`.

## Tips

::: warning Interceptor scope
The source generator rewrites calls at their exact call site in your source code. If you pass a delegate to a helper method that internally calls `.Where()`, the interceptor will not see it. Keep the LINQ chain in the same method where modern syntax is used.
:::

::: tip ToQueryString() for debugging
Use `.ToQueryString()` to inspect the generated SQL and verify that your modern syntax is being translated correctly.
:::

## See Also

- [Nullable Navigation Properties](/recipes/nullable-navigation) -- `?.` patterns in depth
- [Scoring and Classification](/recipes/scoring-classification) -- switch expressions and pattern matching
- [Computed Entity Properties](/recipes/computed-properties) -- reusable query building blocks
- [Window Functions and Ranking](/recipes/window-functions-ranking) -- SQL window functions with ExpressiveDbSet
