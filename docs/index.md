---
layout: home

hero:
  name: "ExpressiveSharp"
  text: "Modern C# syntax in LINQ expression trees"
  tagline: Write null-conditional operators, switch expressions, and pattern matching in your queries — source-generated at compile time with zero runtime overhead.
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
    title: Not Just EF Core
    details: Works with any LINQ provider or standalone. ExpressionPolyfill.Create gives you inline expression trees with modern syntax, no EF Core required.

  - icon: "\u26A1"
    title: Zero Runtime Overhead
    details: All expression trees are generated at compile time using Roslyn source generators. No reflection, no Compile(), no expression tree parsing at runtime.

  - icon: "\U0001F517"
    title: Composable by Design
    details: "[Expressive] members can call other [Expressive] members. Build a library of reusable query fragments and compose them freely in any query."

  - icon: "\U0001F504"
    title: Modern Syntax in LINQ Chains
    details: "IRewritableQueryable<T> enables ?. and switch expressions directly in .Where(), .Select(), and more. Full async method support for EF Core."

  - icon: "\U0001F3D7\uFE0F"
    title: Constructor Projections
    details: "Mark a constructor with [Expressive] to project DTOs directly in queries — new OrderDto(o) translates to a full SQL projection."

  - icon: "\U0001F4CA"
    title: SQL Window Functions
    details: "ROW_NUMBER, RANK, DENSE_RANK, NTILE with a fluent PARTITION BY / ORDER BY API. Experimental — via the RelationalExtensions package."

  - icon: "\U0001F527"
    title: Customizable Transformer Pipeline
    details: "Four built-in transformers adapt expression trees for SQL providers, plus plugin-contributed transformers. Implement IExpressionTreeTransformer for custom rewrites."

  - icon: "\U0001FA7A"
    title: Roslyn Analyzers & Code Fixes
    details: EXP0001–EXP0020 diagnostics catch projection errors at compile time. Quick-fix actions and migration fixers from Projectables included.
---

## At a Glance

**Without ExpressiveSharp** — you hit two walls immediately:

```csharp
// Problem 1: Computed properties are opaque to EF Core
public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Customer? Customer { get; set; }

    // EF Core can't see inside this — it throws or silently fetches everything
    public double Total => Price * Quantity;
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

**With ExpressiveSharp** — write natural C#, the source generator handles the rest:

```csharp
public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Customer? Customer { get; set; }

    [Expressive]
    public double Total => Price * Quantity;      // translated to SQL

    [Expressive]
    public string GetGrade() => Price switch      // switch expression -> SQL CASE
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}

// Extension methods and C# 14 extension properties work too
public static class OrderExtensions
{
    [Expressive]
    public static bool IsHighValue(this Order o) => o.Total > 500;
}

// C# 14 extension members (.NET 10+)
public static class OrderReviewExtensions
{
    extension(Order o)
    {
        [Expressive]
        public bool NeedsReview => o.Customer?.Email == null && o.Total > 100;
    }
}

// ?. syntax, computed properties, switch expressions — all translated to SQL
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => o.Customer?.Email != null && o.IsHighValue())
    .Select(o => new { o.Id, o.Total, Grade = o.GetGrade(), o.NeedsReview })
    .ToList();
```

```sql
SELECT "o"."Id",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total",
       CASE
           WHEN "o"."Price" >= 100.0 THEN 'Premium'
           WHEN "o"."Price" >= 50.0 THEN 'Standard'
           ELSE 'Budget'
       END AS "Grade"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE "c"."Email" IS NOT NULL
```

Computed properties are **inlined into SQL** — no client-side evaluation, no N+1. Modern syntax **just works**.

## NuGet Packages

| Package | Description |
|---------|-------------|
| [`ExpressiveSharp`](https://www.nuget.org/packages/ExpressiveSharp/) | Core runtime — `[Expressive]` attribute, expression expansion, transformers |
| [`ExpressiveSharp.EntityFrameworkCore`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore/) | EF Core integration — `UseExpressives()`, `ExpressiveDbSet<T>`, Include/ThenInclude, async methods |
| [`ExpressiveSharp.EntityFrameworkCore.RelationalExtensions`](https://www.nuget.org/packages/ExpressiveSharp.EntityFrameworkCore.RelationalExtensions/) | SQL window functions — ROW_NUMBER, RANK, DENSE_RANK, NTILE (experimental) |
