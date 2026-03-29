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

## Available Functions

| Function | SQL | Description |
|----------|-----|-------------|
| `WindowFunction.RowNumber(window)` | `ROW_NUMBER() OVER(...)` | Sequential row number within the partition. Returns `long`. |
| `WindowFunction.Rank(window)` | `RANK() OVER(...)` | Rank with gaps for ties. Returns `long`. |
| `WindowFunction.DenseRank(window)` | `DENSE_RANK() OVER(...)` | Rank without gaps for ties. Returns `long`. |
| `WindowFunction.Ntile(n, window)` | `NTILE(n) OVER(...)` | Distributes rows into `n` roughly equal groups. Returns `long`. |

::: tip
All window functions return `long`. When projecting into a typed DTO with `int` properties, use an explicit cast: `(int)WindowFunction.RowNumber(...)`.
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
