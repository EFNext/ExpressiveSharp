# Window Functions & Ranking

This recipe shows how to use SQL window functions -- ROW_NUMBER, RANK, DENSE_RANK, and NTILE -- in EF Core LINQ queries via the `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` package.

::: warning Experimental
This package is experimental. EF Core has an [open issue](https://github.com/dotnet/efcore/issues/12747) for native window function support -- this package may be superseded when that ships.
:::

## Setup

Install the package:

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore.RelationalExtensions
```

Enable it via the plugin architecture:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives(o => o.UseRelationalExtensions())
    .Options;
```

Add the using directive where you write queries:

```csharp
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
```

## ROW_NUMBER for Pagination

ROW_NUMBER assigns a sequential number to each row. This is useful for implementing pagination with guaranteed deterministic ordering:

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string Category { get; set; } = "";
}
```

```csharp
var page = dbContext.Products
    .Select(p => new
    {
        p.Id,
        p.Name,
        p.Price,
        RowNum = WindowFunction.RowNumber(
            Window.OrderBy(p.Price))
    })
    .Where(x => x.RowNum > 20 && x.RowNum <= 30)  // page 3 (10 items per page)
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "p"."Id", "p"."Name", "p"."Price",
       ROW_NUMBER() OVER(ORDER BY "p"."Price") AS "RowNum"
FROM "Products" AS "p"
```

::: tip
While EF Core's `.Skip()` and `.Take()` handle most pagination, ROW_NUMBER is essential when you need the row number as a value in the result set, or when combining pagination with window-based partitioning.
:::

## RANK for Leaderboards

RANK assigns a rank to each row, with gaps for ties. If two items tie at rank 2, the next rank is 4 (not 3):

```csharp
public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Score { get; set; }
    public string League { get; set; } = "";
}
```

```csharp
var leaderboard = dbContext.Players
    .Select(p => new
    {
        p.Name,
        p.Score,
        Rank = WindowFunction.Rank(
            Window.OrderByDescending(p.Score))
    })
    .OrderBy(x => x.Rank)
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "p"."Name", "p"."Score",
       RANK() OVER(ORDER BY "p"."Score" DESC) AS "Rank"
FROM "Players" AS "p"
ORDER BY "Rank"
```

Example output:

| Name | Score | Rank |
|------|-------|------|
| Alice | 950 | 1 |
| Bob | 920 | 2 |
| Carol | 920 | 2 |
| Dave | 890 | 4 |

Notice that Carol and Bob both get rank 2 (tied), and Dave gets rank 4 (gap at 3).

## DENSE_RANK for Continuous Ranking

DENSE_RANK works like RANK but without gaps. Ties share a rank, and the next distinct value gets the next consecutive rank:

```csharp
var rankings = dbContext.Players
    .Select(p => new
    {
        p.Name,
        p.Score,
        DenseRank = WindowFunction.DenseRank(
            Window.OrderByDescending(p.Score))
    })
    .OrderBy(x => x.DenseRank)
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "p"."Name", "p"."Score",
       DENSE_RANK() OVER(ORDER BY "p"."Score" DESC) AS "DenseRank"
FROM "Players" AS "p"
ORDER BY "DenseRank"
```

Example output:

| Name | Score | DenseRank |
|------|-------|-----------|
| Alice | 950 | 1 |
| Bob | 920 | 2 |
| Carol | 920 | 2 |
| Dave | 890 | 3 |

Dave gets rank 3 instead of 4 -- no gap after the tie.

## NTILE for Bucketing

NTILE distributes rows into a specified number of roughly equal groups. This is useful for computing quartiles, deciles, or percentile buckets:

```csharp
// Divide products into 4 price quartiles
var quartiles = dbContext.Products
    .Select(p => new
    {
        p.Name,
        p.Price,
        PriceQuartile = WindowFunction.Ntile(4,
            Window.OrderBy(p.Price))
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "p"."Name", "p"."Price",
       NTILE(4) OVER(ORDER BY "p"."Price") AS "PriceQuartile"
FROM "Products" AS "p"
```

Example output for 12 products:

| Name | Price | PriceQuartile |
|------|-------|---------------|
| Widget A | 5.00 | 1 |
| Widget B | 10.00 | 1 |
| Widget C | 15.00 | 1 |
| Widget D | 25.00 | 2 |
| Widget E | 30.00 | 2 |
| Widget F | 40.00 | 2 |
| Widget G | 50.00 | 3 |
| Widget H | 60.00 | 3 |
| Widget I | 75.00 | 3 |
| Widget J | 100.00 | 4 |
| Widget K | 150.00 | 4 |
| Widget L | 200.00 | 4 |

Use NTILE(10) for deciles, NTILE(100) for percentiles.

## PARTITION BY for Per-Group Ranking

`Window.PartitionBy(...)` restricts the window function to operate within each group independently. This is the SQL equivalent of "rank within each category":

```csharp
// Rank each customer's orders by total, per customer
var rankedOrders = dbContext.Orders
    .Select(o => new
    {
        o.Id,
        o.CustomerId,
        o.GrandTotal,
        RankInCustomer = WindowFunction.Rank(
            Window.PartitionBy(o.CustomerId)
                  .OrderByDescending(o.GrandTotal))
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id", "o"."CustomerId", "o"."GrandTotal",
       RANK() OVER(PARTITION BY "o"."CustomerId" ORDER BY "o"."GrandTotal" DESC) AS "RankInCustomer"
FROM "Orders" AS "o"
```

### Top N per Group

Combine PARTITION BY with filtering to get the top N items per group:

```csharp
// Top 3 most expensive products in each category
var topPerCategory = dbContext.Products
    .Select(p => new
    {
        p.Id,
        p.Name,
        p.Category,
        p.Price,
        RankInCategory = WindowFunction.RowNumber(
            Window.PartitionBy(p.Category)
                  .OrderByDescending(p.Price))
    })
    .Where(x => x.RankInCategory <= 3)
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "p"."Id", "p"."Name", "p"."Category", "p"."Price",
       ROW_NUMBER() OVER(PARTITION BY "p"."Category" ORDER BY "p"."Price" DESC) AS "RankInCategory"
FROM "Products" AS "p"
```

## Multiple Sort Columns

Chain `.ThenBy()` and `.ThenByDescending()` for multi-column ordering within the window:

```csharp
var results = dbContext.Orders
    .Select(o => new
    {
        o.Id,
        RowNum = WindowFunction.RowNumber(
            Window.PartitionBy(o.CustomerId)
                  .OrderByDescending(o.GrandTotal)
                  .ThenBy(o.CreatedDate))
    })
    .ToList();
```

Generated SQL:

```sql
SELECT "o"."Id",
       ROW_NUMBER() OVER(PARTITION BY "o"."CustomerId"
                         ORDER BY "o"."GrandTotal" DESC, "o"."CreatedDate" ASC)
FROM "Orders" AS "o"
```

## Indexed Select for Row Numbering

The `RelationalExtensions` package also transforms `.Select((element, index) => ...)` into ROW_NUMBER-based SQL. This uses the overload of `Select` that provides the row index:

```csharp
var numbered = dbContext.Products
    .OrderBy(p => p.Name)
    .Select((p, index) => new
    {
        RowNumber = index + 1,
        p.Name,
        p.Price
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT ROW_NUMBER() OVER(ORDER BY "p"."Name") AS "RowNumber",
       "p"."Name",
       "p"."Price"
FROM "Products" AS "p"
ORDER BY "p"."Name"
```

## Window Specification Fluent API

Build window specifications with the fluent API:

| Method | SQL |
|---|---|
| `Window.OrderBy(expr)` | `ORDER BY expr ASC` |
| `Window.OrderByDescending(expr)` | `ORDER BY expr DESC` |
| `Window.PartitionBy(expr)` | `PARTITION BY expr` |
| `.ThenBy(expr)` | Additional `ORDER BY expr ASC` |
| `.ThenByDescending(expr)` | Additional `ORDER BY expr DESC` |

The type system ensures correct usage:

- `Window.OrderBy(...)` returns `OrderedWindowDefinition` -- ready for any window function
- `Window.PartitionBy(...)` returns `PartitionedWindowDefinition` -- must chain `.OrderBy(...)` before passing to `Rank` or `DenseRank`
- `.ThenBy(...)` and `.ThenByDescending(...)` return `OrderedWindowDefinition`

## Supported Providers

Window functions are supported across all major relational providers:

| Provider | Supported |
|----------|-----------|
| SQLite | Yes |
| SQL Server | Yes |
| PostgreSQL | Yes |
| MySQL | Yes |
| Oracle | Yes |

The generated SQL uses standard window function syntax, which all these providers support.

## Tips

::: tip Combine with other ExpressiveSharp features
Window functions work alongside `[Expressive]` members, null-conditional operators, and switch expressions in the same query. Use `ExpressiveDbSet<T>` or `.WithExpressionRewrite()` for the full experience.
:::

::: tip PARTITION BY for analytics
Use `Window.PartitionBy(...)` to compute per-group metrics without a subquery or `GROUP BY`. This keeps all original rows in the result set while adding aggregate-like values.
:::

::: warning Filtering on window results
Some databases do not support `WHERE` directly on window function results. EF Core typically wraps the query in a subquery to make this work, but check your provider's behavior.
:::

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- combine computed properties with window functions
- [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) -- modern syntax alongside window functions
- [Scoring and Classification](/recipes/scoring-classification) -- CASE expressions and window-based ranking together
