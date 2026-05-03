---
layout: home

hero:
  name: "ExpressiveSharp"
  text: "Modern C# syntax in LINQ expression trees"
  tagline: Write null-conditional operators, switch expressions, and pattern matching in your queries — source-generated at compile time with zero runtime overhead. Works with EF Core, MongoDB, and any IQueryable provider.
  actions:
    - theme: brand
      text: Introduction
      link: /guide/introduction
    - theme: alt
      text: Quick Start
      link: /guide/quickstart
    - theme: alt
      text: View on GitHub
      link: https://github.com/EFNext/ExpressiveSharp

features:
  - icon: "\U0001F3F7\uFE0F"
    title: Just Add [Expressive]
    details: Decorate any property, method, or constructor with [Expressive] and the source generator does the rest — no boilerplate, no manual expression trees.

  - icon: "\u2728"
    title: Modern C# Everywhere
    details: "Null-conditional ?., switch expressions, pattern matching, string interpolation, tuples, list patterns, and more — all valid inside expression trees."

  - icon: "\U0001F310"
    title: Provider-Agnostic
    details: Works with EF Core (every provider — SQL Server, Postgres, SQLite, MySQL, Oracle, …), MongoDB, and any IQueryable. One library, every backend.

  - icon: "\u26A1"
    title: Zero Runtime Overhead
    details: All expression trees are generated at compile time using Roslyn source generators. No reflection, no Compile(), no expression tree parsing at runtime.

  - icon: "\U0001F517"
    title: Composable by Design
    details: "[Expressive] members can call other [Expressive] members. Build a library of reusable query fragments and compose them freely in any query."

  - icon: "\U0001F504"
    title: Modern Syntax in LINQ Chains
    details: "IExpressiveQueryable<T> enables ?. and switch expressions directly in .Where(), .Select(), and more. Full async method support for EF Core."

  - icon: "\U0001F3D7\uFE0F"
    title: Constructor Projections
    details: "Mark a constructor with [Expressive] to project DTOs directly in queries — new OrderDto(o) translates to a full provider projection."

  - icon: "\U0001F4CA"
    title: SQL Window Functions
    details: "Ranking (ROW_NUMBER, RANK, DENSE_RANK, NTILE, PERCENT_RANK, CUME_DIST), aggregate (SUM, AVG, COUNT, MIN, MAX), and navigation (LAG, LEAD, FIRST_VALUE, LAST_VALUE, NTH_VALUE) functions with a fluent PARTITION BY / ORDER BY / frame API — via the RelationalExtensions package."

  - icon: "\U0001F527"
    title: Customizable Transformer Pipeline
    details: "Built-in transformers adapt expression trees for your provider, plus plugin-contributed transformers. Implement IExpressionTreeTransformer for custom rewrites."

  - icon: "\U0001FA7A"
    title: Roslyn Analyzers & Code Fixes
    details: EXP0001–EXP0036 diagnostics catch projection errors at compile time. Quick-fix actions and migration fixers from Projectables included.
---

## At a Glance

**Without ExpressiveSharp** — you hit two walls immediately:

```csharp
// Problem 1: Computed properties are opaque to LINQ providers
public class Order
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // The provider can't see inside this — it throws or silently fetches everything
    public decimal Total => Price * Quantity;
}

// Problem 2: Modern C# syntax is illegal in expression trees
db.Orders
    .Where(o => o.Customer?.Email != null)       // CS8072: ?. not allowed
    .Select(o => new { Grade = o.Price switch {  // CS8514: switch not allowed
        >= 100 => "Premium",
        _ => "Budget"
    }});
```

You end up duplicating formulas as inline expressions and writing ugly ternary chains.

**With ExpressiveSharp** — write natural C#. The source generator handles it, and your provider (EF Core / MongoDB / your own `IQueryable`) gets a clean, translatable expression tree. Every doc page's live samples render the same query for SQLite, Postgres, SQL Server, MongoDB, and the generator output side-by-side — so you see exactly how it translates for your stack.

```csharp
db
    .Orders
    .Where(o => o.Customer.Email != null && o.Total() > 500m)
    .Select(o => new { o.Id, Total = o.Total(), Grade = o.Grade(), Email = o.Customer.Email })

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Total(this Order o) => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static string Grade(this Order o) => o.Total() switch
    {
        >= 1000m => "Premium",
        >= 100m  => "Standard",
        _        => "Budget",
    };
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId") AS "Total", CASE
    WHEN ef_compare((
        SELECT COALESCE(ef_sum(ef_multiply("l1"."UnitPrice", CAST("l1"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l1"
        WHERE "o"."Id" = "l1"."OrderId"), '1000.0') >= 0 THEN 'Premium'
    WHEN ef_compare((
        SELECT COALESCE(ef_sum(ef_multiply("l2"."UnitPrice", CAST("l2"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l2"
        WHERE "o"."Id" = "l2"."OrderId"), '100.0') >= 0 THEN 'Standard'
    ELSE 'Budget'
END AS "Grade", "c"."Email"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE "c"."Email" IS NOT NULL AND ef_compare((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), '500.0') > 0
```


Computed properties are **inlined into the provider's native query language** — no client-side evaluation, no N+1. Modern syntax **just works**.

## NuGet Packages

| Package | Description |
|---------|-------------|
| [`ExpressiveSharp`](https://www.nuget.org/packages/ExpressiveSharp/) | Core runtime — expression expansion, transformers, `IExpressiveQueryable<T>`, `ExpressionPolyfill` |
| [`ExpressiveSharp.Abstractions`](https://www.nuget.org/packages/ExpressiveSharp.Abstractions/) | Lightweight — attributes (`[Expressive]`, `[ExpressiveFor]`), `IExpressionTreeTransformer`, source generator only |
| [`ExpressiveSharp.EntityFrameworkCore`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore/) | EF Core integration — `UseExpressives()`, `ExpressiveDbSet<T>`, Include/ThenInclude, async methods |
| [`ExpressiveSharp.MongoDB`](https://www.nuget.org/packages/ExpressiveSharp.MongoDB/) | MongoDB integration — `.AsExpressive()` on `IMongoCollection<T>`, MQL translation |
| [`ExpressiveSharp.EntityFrameworkCore.RelationalExtensions`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore.RelationalExtensions/) | SQL window functions — ranking, aggregate, navigation with ROWS/RANGE frames |
