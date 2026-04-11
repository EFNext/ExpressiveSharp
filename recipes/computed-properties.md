---
url: 'https://efnext.github.io/ExpressiveSharp/recipes/computed-properties.md'
---
# Computed Entity Properties

This recipe shows how to define reusable computed properties on your entities and use them across multiple query operations -- all translated to SQL without any duplication.

## The Pattern

Define computed values as `[Expressive]` properties directly on your entity. These properties can then be used in `Select`, `Where`, `GroupBy`, `OrderBy`, and any combination thereof. `[Expressive]` members can reference other `[Expressive]` members, so you can build from simple building blocks to complex compositions.

## Example: Order Totals

```csharp
public class Order
{
    public int Id { get; set; }
    public decimal TaxRate { get; set; }
    public DateTime CreatedDate { get; set; }
    public ICollection<OrderItem> Items { get; set; }

    // Building blocks
    [Expressive]
    public decimal Subtotal => Items.Sum(item => item.Product.ListPrice * item.Quantity);

    [Expressive]
    public decimal Tax => Subtotal * TaxRate;

    // Composed from other [Expressive] members
    [Expressive]
    public decimal GrandTotal => Subtotal + Tax;
}
```

### Use in Select

```csharp
var summaries = dbContext.Orders
    .Select(o => new OrderSummaryDto
    {
        Id = o.Id,
        Subtotal = o.Subtotal,
        Tax = o.Tax,
        GrandTotal = o.GrandTotal
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       (SELECT COALESCE(SUM("p"."ListPrice" * "i"."Quantity"), 0)
        FROM "OrderItems" AS "i"
        INNER JOIN "Products" AS "p" ON "i"."ProductId" = "p"."Id"
        WHERE "o"."Id" = "i"."OrderId") AS "Subtotal",
       (SELECT COALESCE(SUM("p"."ListPrice" * "i"."Quantity"), 0)
        FROM "OrderItems" AS "i"
        INNER JOIN "Products" AS "p" ON "i"."ProductId" = "p"."Id"
        WHERE "o"."Id" = "i"."OrderId") * "o"."TaxRate" AS "Tax",
       (SELECT COALESCE(SUM("p"."ListPrice" * "i"."Quantity"), 0)
        FROM "OrderItems" AS "i"
        INNER JOIN "Products" AS "p" ON "i"."ProductId" = "p"."Id"
        WHERE "o"."Id" = "i"."OrderId") * (1 + "o"."TaxRate") AS "GrandTotal"
FROM "Orders" AS "o"
```

### Use in Where

```csharp
// Only load high-value orders
var highValue = dbContext.Orders
    .Where(o => o.GrandTotal > 1000)
    .ToList();
```

### Use in OrderBy

```csharp
// Sort by computed value -- top 10 by total
var ranked = dbContext.Orders
    .OrderByDescending(o => o.GrandTotal)
    .Take(10)
    .ToList();
```

### All Together

```csharp
var report = dbContext.Orders
    .Where(o => o.GrandTotal > 500)
    .OrderByDescending(o => o.GrandTotal)
    .GroupBy(o => o.CreatedDate.Year)
    .Select(g => new
    {
        Year = g.Key,
        Count = g.Count(),
        TotalRevenue = g.Sum(o => o.GrandTotal)
    })
    .ToList();
```

All computed values are evaluated **in the database** -- no data is fetched to memory for filtering or aggregation.

## Example: User Profile

```csharp
public class User
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public DateTime? LastLoginDate { get; set; }

    [Expressive]
    public string FullName => FirstName + " " + LastName;

    [Expressive]
    public string DisplayName => FirstName + " " + LastName.Substring(0, 1) + ".";

    [Expressive]
    public bool IsActive => LastLoginDate != null
        && LastLoginDate >= DateTime.UtcNow.AddDays(-30);
}
```

```csharp
// Find active users, sorted by name
var results = dbContext.Users
    .Where(u => u.IsActive)
    .OrderBy(u => u.FullName)
    .Select(u => new { u.FullName, u.DisplayName })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "u"."FirstName" || ' ' || "u"."LastName" AS "FullName",
       "u"."FirstName" || ' ' || SUBSTR("u"."LastName", 1, 1) || '.' AS "DisplayName"
FROM "Users" AS "u"
WHERE "u"."LastLoginDate" IS NOT NULL
  AND "u"."LastLoginDate" >= DATETIME('now', '-30 days')
ORDER BY "u"."FirstName" || ' ' || "u"."LastName"
```

## Example: Product Catalog

```csharp
public class Product
{
    public int Id { get; set; }
    public decimal ListPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderPoint { get; set; }

    [Expressive]
    public decimal DiscountedPrice => ListPrice * (1 - DiscountRate);

    [Expressive]
    public decimal SavingsAmount => ListPrice - DiscountedPrice;

    [Expressive]
    public bool IsAvailable => StockQuantity > 0;

    [Expressive]
    public bool NeedsReorder => StockQuantity <= ReorderPoint;
}
```

```csharp
// Available products on sale that need restocking
var reorder = dbContext.Products
    .Where(p => p.IsAvailable && p.NeedsReorder && p.DiscountedPrice < 50)
    .OrderBy(p => p.StockQuantity)
    .Select(p => new
    {
        p.Id,
        p.DiscountedPrice,
        p.SavingsAmount,
        p.StockQuantity
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "p"."Id",
       "p"."ListPrice" * (1 - "p"."DiscountRate") AS "DiscountedPrice",
       "p"."ListPrice" - "p"."ListPrice" * (1 - "p"."DiscountRate") AS "SavingsAmount",
       "p"."StockQuantity"
FROM "Products" AS "p"
WHERE "p"."StockQuantity" > 0
  AND "p"."StockQuantity" <= "p"."ReorderPoint"
  AND "p"."ListPrice" * (1 - "p"."DiscountRate") < 50
ORDER BY "p"."StockQuantity"
```

## Collection Aggregates

Computed properties can include LINQ aggregation over navigation collections:

```csharp
public class Customer
{
    public ICollection<Order> Orders { get; set; }
    public ICollection<Review> Reviews { get; set; }

    [Expressive]
    public int OrderCount => Orders.Count();

    [Expressive]
    public decimal LifetimeSpend => Orders.Sum(o => o.GrandTotal);

    [Expressive]
    public bool HasRecentOrder =>
        Orders.Any(o => o.CreatedDate >= DateTime.UtcNow.AddDays(-30));
}
```

EF Core translates these to efficient correlated subqueries.

## Tips

::: tip Compose freely
`[Expressive]` members can call other `[Expressive]` members. Build from simple building blocks to complex compositions -- the expander resolves them recursively.
:::

::: tip Keep it pure
Expressive properties should be pure computations with no side effects. Everything must be translatable to SQL by your LINQ provider.
:::

::: warning Avoid N+1 traps
If a computed property references navigation properties, make sure to structure your queries so EF Core can generate a single efficient query. Using computed properties in `Select` and `Where` at the top level is safe.
:::

## See Also

* [Reusable Query Filters](/recipes/reusable-query-filters) -- Boolean computed properties as filter predicates
* [DTO Projections with Constructors](/recipes/dto-projections) -- project computed values into DTOs
* [Scoring and Classification](/recipes/scoring-classification) -- computed properties with switch expressions
