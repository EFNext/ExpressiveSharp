---
url: 'https://efnext.github.io/ExpressiveSharp/guide/quickstart.md'
---
# Quick Start

This guide walks you through a complete end-to-end example -- from installing the NuGet packages to seeing the generated SQL.

## Prerequisites

* .NET 8 SDK or later (.NET 10 also supported)
* A LINQ provider such as EF Core (any provider: SQLite, SQL Server, PostgreSQL, etc.)

## Step 1 -- Install the Packages

```bash
dotnet add package ExpressiveSharp
```

For EF Core integration, also install:

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

| Package | Purpose |
|---------|---------|
| `ExpressiveSharp` | Core runtime -- expression expansion, transformers, `IExpressiveQueryable<T>`, `ExpressionPolyfill` (includes everything from Abstractions) |
| `ExpressiveSharp.Abstractions` | Lightweight -- `[Expressive]` attribute, `[ExpressiveFor]`, `IExpressionTreeTransformer`, source generator only (no runtime services) |
| `ExpressiveSharp.EntityFrameworkCore` | EF Core integration -- `UseExpressives()`, `ExpressiveDbSet<T>`, Include/ThenInclude, async methods, analyzers and code fixes |
| `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` | SQL window functions -- ROW\_NUMBER, RANK, DENSE\_RANK, NTILE (experimental) |

## Step 2 -- Define Your Entities

Add `[Expressive]` to any property or method whose body you want translated into an expression tree:

```csharp
using ExpressiveSharp;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? Email { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Tag { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Computed property -- reusable in any query, translated to SQL
    [Expressive]
    public double Total => Price * Quantity;

    // Switch expression -- normally illegal in expression trees
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}
```

The source generator runs at **compile time** and emits a companion `Expression<TDelegate>` for each `[Expressive]` member -- no runtime reflection.

## Step 3 -- Configure EF Core

Call `UseExpressives()` on your `DbContextOptionsBuilder`:

```csharp
using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives()
    .Options;
```

This automatically:

* Expands `[Expressive]` member references in queries
* Marks `[Expressive]` properties as unmapped in the EF model
* Applies database-friendly transformers

### With Dependency Injection

```csharp
services.AddDbContext<MyDbContext>(options =>
    options.UseSqlite(connectionString)
           .UseExpressives());
```

## Step 4 -- Use \[Expressive] Members in Queries

Use `ExpressiveDbSet<T>` for direct modern syntax support on your `DbSet`:

```csharp
public class MyDbContext : DbContext
{
    public DbSet<Order> OrdersRaw { get; set; }
    public DbSet<Customer> Customers { get; set; }

    // Shorthand for Set<Order>().AsExpressiveDbSet()
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
}
```

Now query with modern C# syntax -- null-conditional operators, switch expressions, and `[Expressive]` members all work:

```csharp
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        o.Total,
        Email = o.Customer?.Email,
        Grade = o.GetGrade()
    })
    .ToList();
```

## Step 5 -- Check the Generated SQL

Use `ToQueryString()` to inspect the SQL:

```csharp
var query = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        o.Total,
        Email = o.Customer?.Email,
        Grade = o.GetGrade()
    });

Console.WriteLine(query.ToQueryString());
```

Generated SQL (SQLite):

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

The `?.` operator, the `Total` property, and the `GetGrade()` switch expression are all translated to SQL. No data is loaded into memory for filtering or projection.

## Complete Working Example

```csharp
using ExpressiveSharp;
using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Entities
public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Customer? Customer { get; set; }
    public int CustomerId { get; set; }

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}

public class Customer
{
    public int Id { get; set; }
    public string? Email { get; set; }
}

// DbContext
public class AppDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => this.ExpressiveSet<Order>();
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=app.db").UseExpressives();
}

// Query
using var ctx = new AppDbContext();
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, o.Total, Grade = o.GetGrade() })
    .ToList();
```

## Next Steps

* [\[Expressive\] Properties](./expressive-properties) -- computed properties in depth
* [\[Expressive\] Methods](./expressive-methods) -- parameterized query fragments
* [Constructor Projections](./expressive-constructors) -- project DTOs directly in queries
* [EF Core Integration](./ef-core-integration) -- full EF Core setup and features
* [IExpressiveQueryable\<T>](./expressive-queryable) -- modern syntax on any `IQueryable`
