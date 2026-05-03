# Quick Start

This guide walks you through a complete end-to-end example — from installing the NuGet packages to seeing the translated output for your provider.

## Prerequisites

- .NET 8 SDK or later (.NET 10 also supported)
- A LINQ provider. ExpressiveSharp integrates with **EF Core**, **MongoDB**, or **any `IQueryable<T>`**.

## Step 1 — Install the Packages

Install the core package first:

```bash
dotnet add package ExpressiveSharp
```

Then pick the integration that matches your data source:

::: code-group

```bash [EF Core]
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

```bash [MongoDB]
dotnet add package ExpressiveSharp.MongoDB
```

```bash [Custom IQueryable]
# Nothing else — call .AsExpressive() on your IQueryable<T>
```

:::

| Package | Purpose |
|---------|---------|
| `ExpressiveSharp` | Core runtime — expression expansion, transformers, `IExpressiveQueryable<T>`, `ExpressionPolyfill` (includes Abstractions) |
| `ExpressiveSharp.Abstractions` | Lightweight — `[Expressive]` attribute, `[ExpressiveFor]`, `IExpressionTreeTransformer`, source generator only (no runtime services) |
| `ExpressiveSharp.EntityFrameworkCore` | EF Core integration — `UseExpressives()`, `ExpressiveDbSet<T>`, Include/ThenInclude, async methods, analyzers and code fixes |
| `ExpressiveSharp.MongoDB` | MongoDB integration — `.AsExpressive()` on `IMongoCollection<T>`, MQL aggregation translation |
| `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` | SQL window functions — ranking (ROW_NUMBER, RANK, DENSE_RANK, NTILE, PERCENT_RANK, CUME_DIST), aggregate (SUM, AVG, COUNT, MIN, MAX), and navigation (LAG, LEAD, FIRST_VALUE, LAST_VALUE, NTH_VALUE) with PARTITION BY / ORDER BY / ROWS\|RANGE frame support, plus indexed Select. |

## Step 2 — Define Your Entities

Add `[Expressive]` to any property or method whose body you want translated into an expression tree:

```csharp
using ExpressiveSharp;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    // Computed property — reusable in any query, translated for any provider
    [Expressive]
    public bool IsVip => Orders.Count() > 10;
}

public class Order
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Expressive]
    public decimal Total => Price * Quantity;

    // Switch expression — normally illegal in expression trees
    [Expressive]
    public string Grade => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}
```

The source generator runs at **compile time** and emits a companion `Expression<TDelegate>` for each `[Expressive]` member — no runtime reflection.

## Step 3 — Wire Up Your Provider

::: code-group

```csharp [EF Core]
using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // ExpressiveDbSet<T> lets modern C# syntax flow through DbSet chains
    public ExpressiveDbSet<Customer> Customers => this.ExpressiveSet<Customer>();
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=app.db")
                  .UseExpressives();  // register [Expressive] expansion
}
```

```csharp [MongoDB]
using ExpressiveSharp.MongoDB.Extensions;
using MongoDB.Driver;

var db = new MongoClient("mongodb://localhost:27017").GetDatabase("shop");
var customers = db.GetCollection<Customer>("customers").AsExpressive();
var orders = db.GetCollection<Order>("orders").AsExpressive();
```

```csharp [Custom IQueryable]
using ExpressiveSharp;

// Any IQueryable<T> — your own provider, LINQ to Objects, etc.
IQueryable<Customer> raw = GetCustomers();
var customers = raw.AsExpressive();
```

:::

## Step 4 — Write Modern-Syntax Queries

Modern C# syntax — null-conditional operators, switch expressions, pattern matching, and `[Expressive]` member access — all work directly in the query:

::: expressive-sample
db.Orders
    .Where(o => o.Customer.Email != null && o.Total() > 50)
    .Select(o => new { o.Id, Total = o.Total(), Grade = o.Grade(), Email = o.Customer.Email })
    .OrderByDescending(x => x.Total)
    .Take(10)
---setup---
public static class OrderExt
{
    // Computed sum of line items — reusable in any query, translated to SQL/MQL
    [Expressive]
    public static decimal Total(this Order o) => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    // Switch expression over the computed total — illegal in raw expression trees,
    // but [Expressive] expands it into a provider-translatable tree.
    [Expressive]
    public static string Grade(this Order o) => o.Total() switch
    {
        >= 100m => "Premium",
        >= 50m  => "Standard",
        _       => "Budget",
    };
}
:::

The tabs above show how this exact query translates for each provider. The `?.` operator, the `[Expressive]` `Total` and `Grade` members, and the switch expression inside `Grade` are all compiled into the provider's native query language — no data is loaded into memory for filtering or projection.

## Step 5 — Inspect the Generated Query

::: code-group

```csharp [EF Core]
// Use ToQueryString() to inspect the SQL without executing
var sql = ctx.Orders
    .Where(o => o.Customer.Email != null)
    .Select(o => new { o.Id, o.Grade })
    .ToQueryString();
Console.WriteLine(sql);
```

```csharp [MongoDB]
// ToString() on the queryable yields the aggregation pipeline
var pipeline = orders
    .Where(o => o.Customer.Email != null)
    .Select(o => new { o.Id, o.Grade })
    .ToString();
Console.WriteLine(pipeline);
```

:::

## Next Steps

- [IExpressiveQueryable\<T\>](./expressive-queryable) — the core provider-agnostic API
- [[Expressive] Properties](./expressive-properties) — computed properties in depth
- [[Expressive] Methods](./expressive-methods) — parameterized query fragments
- [Constructor Projections](./expressive-constructors) — project DTOs directly in queries
- [EF Core Integration](./integrations/ef-core) — full EF Core setup
- [MongoDB Integration](./integrations/mongodb) — full MongoDB setup
