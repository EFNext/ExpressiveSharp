# Window Functions (SQL)

ExpressiveSharp provides SQL window function support through the `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` package. This enables ROW_NUMBER, RANK, DENSE_RANK, and NTILE directly in LINQ queries with a fluent window specification API.

::: warning Experimental
This package is experimental. EF Core has an [open issue (#12747)](https://github.com/dotnet/efcore/issues/12747) for native window function support. This package may be superseded when that ships.
:::

## Installation

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore.RelationalExtensions
```

## Configuration

Enable window functions via the `UseRelationalExtensions()` plugin in your `UseExpressives()` call:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives(o => o.UseRelationalExtensions())
    .Options;
```

Or with dependency injection:

```csharp
services.AddDbContext<MyDbContext>(options =>
    options.UseSqlite(connectionString)
           .UseExpressives(o => o.UseRelationalExtensions()));
```

::: tip Concise syntax with `using static`
Add these imports for a compact, SQL-like syntax without class prefixes:
```csharp
using static ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions.WindowFunction;
using static ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions.WindowFrameBound;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

// Then in queries:
RowNumber(Window.PartitionBy(o.CustomerId).OrderBy(o.Price))
Sum(o.Price, Window.OrderBy(o.Date).RowsBetween(UnboundedPreceding, CurrentRow))
Lag(o.Price, 1, 0.0, Window.OrderBy(o.Date))
```
:::

## Available Functions

### Ranking Functions

| Function | SQL | Description |
|----------|-----|-------------|
| `WindowFunction.RowNumber(window)` | `ROW_NUMBER() OVER(...)` | Sequential row number within the partition. Returns `long`. |
| `WindowFunction.Rank(window)` | `RANK() OVER(...)` | Rank with gaps for ties. Returns `long`. |
| `WindowFunction.DenseRank(window)` | `DENSE_RANK() OVER(...)` | Rank without gaps for ties. Returns `long`. |
| `WindowFunction.Ntile(n, window)` | `NTILE(n) OVER(...)` | Distributes rows into `n` roughly equal groups. Returns `long`. |
| `WindowFunction.PercentRank(window)` | `PERCENT_RANK() OVER(...)` | Relative rank as a value between 0.0 and 1.0. Returns `double`. |
| `WindowFunction.CumeDist(window)` | `CUME_DIST() OVER(...)` | Cumulative distribution (0.0–1.0]. Returns `double`. |

### Aggregate Functions

Aggregate window functions compute values over a set of rows defined by the window specification. Unlike ranking functions, they support [window frame clauses](#window-frame-specification).

| Function | SQL | Description |
|----------|-----|-------------|
| `WindowFunction.Sum(expr, window)` | `SUM(expr) OVER(...)` | Sum of values. Returns same type as input. |
| `WindowFunction.Average(expr, window)` | `AVG(expr) OVER(...)` | Average of values. Returns `T?` (or `double` for `int`/`long` input). |
| `WindowFunction.Count(window)` | `COUNT(*) OVER(...)` | Count of all rows. Returns `long`. |
| `WindowFunction.Count(expr, window)` | `COUNT(expr) OVER(...)` | Count of non-null values. Returns `long`. |
| `WindowFunction.Min(expr, window)` | `MIN(expr) OVER(...)` | Minimum value. Returns same type as input. |
| `WindowFunction.Max(expr, window)` | `MAX(expr) OVER(...)` | Maximum value. Returns same type as input. |

### Navigation Functions

Navigation functions access specific rows relative to the current row. LAG/LEAD do not support frame clauses; FIRST_VALUE/LAST_VALUE do.

| Function | SQL | Frame? | Description |
|----------|-----|--------|-------------|
| `WindowFunction.Lag(expr, window)` | `LAG(expr) OVER(...)` | No | Previous row's value (offset 1). |
| `WindowFunction.Lag(expr, n, window)` | `LAG(expr, n) OVER(...)` | No | Value `n` rows back. |
| `WindowFunction.Lag(expr, n, default, window)` | `LAG(expr, n, default) OVER(...)` | No | Value `n` rows back, with default. |
| `WindowFunction.Lead(expr, window)` | `LEAD(expr) OVER(...)` | No | Next row's value (offset 1). |
| `WindowFunction.Lead(expr, n, window)` | `LEAD(expr, n) OVER(...)` | No | Value `n` rows ahead. |
| `WindowFunction.Lead(expr, n, default, window)` | `LEAD(expr, n, default) OVER(...)` | No | Value `n` rows ahead, with default. |
| `WindowFunction.FirstValue(expr, window)` | `FIRST_VALUE(expr) OVER(...)` | Yes | First value in the frame. |
| `WindowFunction.LastValue(expr, window)` | `LAST_VALUE(expr) OVER(...)` | Yes | Last value in the frame. |
| `WindowFunction.NthValue(expr, n, window)` | `NTH_VALUE(expr, n) OVER(...)` | Yes | Value at the Nth row (1-based) in the frame. |

::: tip Nullable results from LAG/LEAD
When no row exists at the requested offset (e.g. LAG on the first row), SQL returns NULL. For value-type columns, cast to a nullable type in the projection to detect this: `(double?)WindowFunction.Lag(o.Price, window)`. When a default value is provided (3-arg overload), NULL is never returned.
:::

::: warning LAST_VALUE needs an explicit frame
With the default frame (`RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW`), `LAST_VALUE` returns the *current row's* value — not the partition's last. Use an explicit frame:
```csharp
WindowFunction.LastValue(o.Price,
    Window.OrderBy(o.Price)
          .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.UnboundedFollowing))
```
:::

::: tip
Ranking functions return `long`. When projecting into a typed DTO with `int` properties, use an explicit cast: `(int)WindowFunction.RowNumber(...)`.
:::

## Window Specification API

Build window specifications using the fluent `Window` API:

| Method | SQL Output |
|---|---|
| `Window.OrderBy(expr)` | `ORDER BY expr ASC` |
| `Window.OrderByDescending(expr)` | `ORDER BY expr DESC` |
| `Window.PartitionBy(expr)` | `PARTITION BY expr` |
| `.ThenBy(expr)` | Additional `ORDER BY expr ASC` column |
| `.ThenByDescending(expr)` | Additional `ORDER BY expr DESC` column |
| `.RowsBetween(start, end)` | `ROWS BETWEEN start AND end` (see [Window Frame Specification](#window-frame-specification)) |
| `.RangeBetween(start, end)` | `RANGE BETWEEN start AND end` (see [Window Frame Specification](#window-frame-specification)) |

Chain these methods to build the full window specification:

```csharp
// ORDER BY Price ASC
Window.OrderBy(o.Price)

// ORDER BY Price DESC
Window.OrderByDescending(o.Price)

// PARTITION BY CustomerId ORDER BY Price DESC
Window.PartitionBy(o.CustomerId).OrderByDescending(o.Price)

// PARTITION BY CustomerId ORDER BY Price DESC, Id ASC
Window.PartitionBy(o.CustomerId)
      .OrderByDescending(o.Price)
      .ThenBy(o.Id)
```

## Window Frame Specification

Aggregate window functions support frame clauses that narrow the set of rows used for the computation. Frames use `RowsBetween` or `RangeBetween` chained onto an ordered window specification:

```csharp
Window.OrderBy(o.Price)
      .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow)
```

The `WindowFrameBound` factory members produce the five SQL:2003 frame boundaries:

| Bound | SQL |
|-------|-----|
| `WindowFrameBound.UnboundedPreceding` | `UNBOUNDED PRECEDING` |
| `WindowFrameBound.Preceding(n)` | `n PRECEDING` |
| `WindowFrameBound.CurrentRow` | `CURRENT ROW` |
| `WindowFrameBound.Following(n)` | `n FOLLOWING` |
| `WindowFrameBound.UnboundedFollowing` | `UNBOUNDED FOLLOWING` |

Example — running total with `SUM`:

```csharp
var results = db.Orders.Select(o => new
{
    o.Id,
    o.Price,
    RunningTotal = WindowFunction.Sum(o.Price,
        Window.PartitionBy(o.CustomerId)
              .OrderBy(o.Price)
              .RowsBetween(WindowFrameBound.UnboundedPreceding, WindowFrameBound.CurrentRow))
});
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id", "o"."Price",
    SUM("o"."Price") OVER(PARTITION BY "o"."CustomerId" ORDER BY "o"."Price" ASC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS "RunningTotal"
FROM "Orders" AS "o"
```

::: tip Default frame behavior
When no explicit frame is specified, SQL defaults to `RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW` in the presence of `ORDER BY`. This produces a running total/min/max by default.
:::

::: warning Ranking functions don't support frames
The SQL standard forbids frame clauses on ranking functions (ROW_NUMBER, RANK, DENSE_RANK, NTILE) — SQL Server and PostgreSQL will reject the query. Only aggregate functions (SUM, AVG, COUNT, MIN, MAX) accept frames.
:::

::: warning Literal offsets only
`Preceding(n)` and `Following(n)` accept an integer **constant**. Passing a variable or captured value will fail translation: SQL requires literal integer constants in the frame clause.
:::

## Complete Example

```csharp
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

var ranked = db.Orders.Select(o => new
{
    o.Id,
    o.Price,
    RowNum = WindowFunction.RowNumber(
        Window.OrderBy(o.Price)),
    PriceRank = WindowFunction.Rank(
        Window.PartitionBy(o.CustomerId)
              .OrderByDescending(o.Price)),
    Quartile = WindowFunction.Ntile(4,
        Window.OrderBy(o.Id))
});
```

Generated SQL:

```sql
SELECT "o"."Id", "o"."Price",
    ROW_NUMBER() OVER(ORDER BY "o"."Price"),
    RANK() OVER(PARTITION BY "o"."CustomerId" ORDER BY "o"."Price" DESC),
    NTILE(4) OVER(ORDER BY "o"."Id")
FROM "Orders" AS "o"
```

## Indexed Select

The `RelationalExtensions` package also supports indexed `Select`, which is useful in combination with window functions:

```csharp
var result = db.Orders
    .OrderBy(o => o.Price)
    .Select((o, index) => new
    {
        o.Id,
        o.Price,
        Position = index + 1
    });
```

## Supported Database Providers

Window functions are supported across all major relational database providers:

| Provider | Supported |
|----------|-----------|
| SQLite | Yes |
| SQL Server | Yes |
| PostgreSQL | Yes |
| MySQL | Yes |
| Oracle | Yes |

The generated SQL uses standard ANSI window function syntax. Each provider translates the expressions using its native SQL dialect.

## Full Configuration Example

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlite("Data Source=app.db")
            .UseExpressives(o => o.UseRelationalExtensions());
}

// Query with window functions
using var ctx = new AppDbContext();

var rankings = ctx.Orders
    .Select(o => new
    {
        o.Id,
        o.Price,
        o.CustomerId,
        OverallRank = WindowFunction.RowNumber(
            Window.OrderByDescending(o.Price)),
        CustomerRank = WindowFunction.DenseRank(
            Window.PartitionBy(o.CustomerId)
                  .OrderByDescending(o.Price)),
        PriceQuartile = WindowFunction.Ntile(4,
            Window.OrderByDescending(o.Price))
    })
    .ToList();
```

::: info
Window functions are implemented as a plugin using the `IExpressivePlugin` architecture. The `UseRelationalExtensions()` call registers custom EF Core services and expression translators that handle the `WindowFunction.*` method calls during SQL generation.
:::

## Next Steps

- [EF Core Integration](./ef-core-integration) -- full EF Core setup and features
- [IRewritableQueryable\<T\>](./rewritable-queryable) -- modern syntax in LINQ chains
- [Introduction](./introduction) -- overview of all ExpressiveSharp APIs
