# ExpressiveSharp

[![CI](https://github.com/EFNext/ExpressiveSharp/actions/workflows/ci.yml/badge.svg)](https://github.com/EFNext/ExpressiveSharp/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/EFNext/ExpressiveSharp/graph/badge.svg)](https://codecov.io/gh/EFNext/ExpressiveSharp)
[![NuGet](https://img.shields.io/nuget/v/ExpressiveSharp.svg)](https://www.nuget.org/packages/ExpressiveSharp)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ExpressiveSharp.svg)](https://www.nuget.org/packages/ExpressiveSharp)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-purple)](https://dotnet.microsoft.com)
[![GitHub Stars](https://img.shields.io/github/stars/EFNext/ExpressiveSharp)](https://github.com/EFNext/ExpressiveSharp/stargazers)
[![GitHub Issues](https://img.shields.io/github/issues/EFNext/ExpressiveSharp)](https://github.com/EFNext/ExpressiveSharp/issues)

**[Documentation](https://efnext.github.io/ExpressiveSharp/)** | **[Getting Started](https://efnext.github.io/ExpressiveSharp/guide/quickstart)**

Source generator that enables modern C# syntax in LINQ expression trees. All expression trees are generated at compile time with minimal runtime overhead.

## The Problem

There are two problems when using C# with LINQ providers like EF Core:

**1. Expression tree syntax restrictions.** You write a perfectly reasonable query and hit:

```
error CS8072: An expression tree lambda may not contain a null propagating operator
```

Expression trees (`Expression<Func<...>>`) only support a restricted subset of C# — no `?.`, no switch expressions, no pattern matching. So you end up writing ugly ternary chains instead of the clean code you'd write anywhere else. ([Why hasn't this been fixed?](https://efnext.github.io/ExpressiveSharp/guide/expression-tree-problem.html))

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

Mark computed properties and methods with `[Expressive]` to generate companion expression trees. Then choose how to wire them into your queries:

| Scenario | API |
|---|---|
| **EF Core** — modern syntax + `[Expressive]` expansion on `DbSet` | [`ExpressiveDbSet<T>`](https://efnext.github.io/ExpressiveSharp/guide/ef-core-integration) |
| **Any `IQueryable`** — modern syntax + `[Expressive]` expansion | [`.WithExpressionRewrite()`](https://efnext.github.io/ExpressiveSharp/guide/rewritable-queryable) |
| **EF Core** — SQL window functions (ROW_NUMBER, RANK, etc.) | [`WindowFunction.*`](https://efnext.github.io/ExpressiveSharp/guide/window-functions) |
| **Advanced** — build an `Expression<T>` inline, no attribute needed | [`ExpressionPolyfill.Create`](https://efnext.github.io/ExpressiveSharp/guide/expression-polyfill) |
| **Advanced** — expand `[Expressive]` members in an existing expression tree | [`.ExpandExpressives()`](https://efnext.github.io/ExpressiveSharp/guide/expressive-properties) |
| **Advanced** — make third-party/BCL members expressable | [`[ExpressiveFor]`](https://efnext.github.io/ExpressiveSharp/reference/expressive-for) |

## Features

| Category | Examples |
|---|---|
| Null-conditional operators | `?.` member access and indexer |
| Switch expressions | With relational, logical, type, and property patterns |
| Pattern matching | Constant, type, relational, logical, property, positional, list patterns |
| String interpolation | Converted to `string.Concat` |
| Constructor projections | `[Expressive]` constructors for DTO projections with inheritance support |
| Block bodies (opt-in) | `if`/`else`, `switch` statements, `foreach`/`for` loops, local variables |
| External member mapping | `[ExpressiveFor]` for BCL/third-party members |
| Tuples, index/range, `with`, collection expressions | And more modern C# syntax |
| Expression transformers | Built-in + custom `IExpressionTreeTransformer` pipeline |
| SQL window functions | ROW_NUMBER, RANK, DENSE_RANK, NTILE (experimental) |

See the [full documentation](https://efnext.github.io/ExpressiveSharp/guide/introduction) for detailed usage, [reference](https://efnext.github.io/ExpressiveSharp/reference/expressive-attribute), and [recipes](https://efnext.github.io/ExpressiveSharp/recipes/computed-properties).

## Contributing

```bash
dotnet build    # Build all projects
dotnet test     # Run all tests
```

- [Testing Strategy](https://efnext.github.io/ExpressiveSharp/advanced/testing-strategy) — snapshot tests, functional tests, and test consumers
- [How It Works](https://efnext.github.io/ExpressiveSharp/advanced/how-it-works) — source generator internals

## License

MIT
