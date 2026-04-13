---
url: 'https://efnext.github.io/ExpressiveSharp/recipes/computed-properties.md'
---
# Computed Entity Properties

This recipe shows how to define reusable computed values on your entities and use them across multiple query operations -- all translated to SQL without any duplication.

## The Pattern

Define computed values as `[Expressive]` members -- either as properties directly on your entity, or as extension methods in a helper class when you cannot modify the entity. These members can then be used in `Select`, `Where`, `GroupBy`, `OrderBy`, and any combination thereof. `[Expressive]` members can reference other `[Expressive]` members, so you can build from simple building blocks to complex compositions.

## Example: Order Totals

For entities you own, put `[Expressive]` properties directly on them:

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

When you cannot modify the entity, define the same logic as extension methods in a helper class. Here we compute order totals over the webshop `Order` / `LineItem` model:

### Use in Select

::: expressive-sample
db.Orders
.Select(o => new { o.Id, Subtotal = o.Subtotal(), Tax = o.Tax(), GrandTotal = o.GrandTotal() })
\---setup---
public static class OrderTotals
{
\[Expressive]
public static decimal Subtotal(this Order o)
\=> o.Items.Sum(i => i.UnitPrice \* i.Quantity);

```
[Expressive]
public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

[Expressive]
public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
```

}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Subtotal = o.Subtotal(), Tax = o.Tax(), GrandTotal = o.GrandTotal() })

// Setup
public static class OrderTotals
{
    [Expressive]
    public static decimal Subtotal(this Order o)
        => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

    [Expressive]
    public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "Subtotal", ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId"), '0.2') AS "Tax", ef_add((
    SELECT COALESCE(ef_sum(ef_multiply("l1"."UnitPrice", CAST("l1"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l1"
    WHERE "o"."Id" = "l1"."OrderId"), ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l2"."UnitPrice", CAST("l2"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l2"
    WHERE "o"."Id" = "l2"."OrderId"), '0.2')) AS "GrandTotal"
FROM "Orders" AS "o"
```

### Use in Where

::: expressive-sample
db.Orders.Where(o => o.GrandTotal() > 1000)
\---setup---
public static class OrderTotals
{
\[Expressive]
public static decimal Subtotal(this Order o)
\=> o.Items.Sum(i => i.UnitPrice \* i.Quantity);

```
[Expressive]
public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

[Expressive]
public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
```

}
:::

```csharp
db
    .Orders
    .Where(o => o.GrandTotal() > 1000)

// Setup
public static class OrderTotals
{
    [Expressive]
    public static decimal Subtotal(this Order o)
        => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

    [Expressive]
    public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", "o"."CustomerId", "o"."PlacedAt", "o"."Status"
FROM "Orders" AS "o"
WHERE ef_compare(ef_add((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId"), '0.2')), '1000.0') > 0
```

### Use in OrderBy

::: expressive-sample
db.Orders.OrderByDescending(o => o.GrandTotal()).Take(10)
\---setup---
public static class OrderTotals
{
\[Expressive]
public static decimal Subtotal(this Order o)
\=> o.Items.Sum(i => i.UnitPrice \* i.Quantity);

```
[Expressive]
public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

[Expressive]
public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
```

}
:::

```csharp
db
    .Orders
    .OrderByDescending(o => o.GrandTotal())
    .Take(10)

// Setup
public static class OrderTotals
{
    [Expressive]
    public static decimal Subtotal(this Order o)
        => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

    [Expressive]
    public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
}
```

**Generated SQL:**

```sql
.param set @p 10

SELECT "o"."Id", "o"."CustomerId", "o"."PlacedAt", "o"."Status"
FROM "Orders" AS "o"
ORDER BY ef_add((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId"), '0.2')) COLLATE EF_DECIMAL DESC
LIMIT @p
```

### All Together

::: expressive-sample
db.Orders
.Where(o => o.GrandTotal() > 500)
.GroupBy(o => o.PlacedAt.Year)
.Select(g => new { Year = g.Key, Count = g.Count(), TotalRevenue = g.Sum(o => o.GrandTotal()) })
\---setup---
public static class OrderTotals
{
\[Expressive]
public static decimal Subtotal(this Order o)
\=> o.Items.Sum(i => i.UnitPrice \* i.Quantity);

```
[Expressive]
public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

[Expressive]
public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
```

}
:::

```csharp
db
    .Orders
    .Where(o => o.GrandTotal() > 500)
    .GroupBy(o => o.PlacedAt.Year)
    .Select(g => new { Year = g.Key, Count = g.Count(), TotalRevenue = g.Sum(o => o.GrandTotal()) })

// Setup
public static class OrderTotals
{
    [Expressive]
    public static decimal Subtotal(this Order o)
        => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static decimal Tax(this Order o) => o.Subtotal() * 0.2m;

    [Expressive]
    public static decimal GrandTotal(this Order o) => o.Subtotal() + o.Tax();
}
```

**Generated SQL:**

```sql
SELECT "o0"."Key" AS "Year", COUNT(*) AS "Count", COALESCE(ef_sum(ef_add((
    SELECT COALESCE(ef_sum(ef_multiply("l1"."UnitPrice", CAST("l1"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l1"
    WHERE "o0"."Id" = "l1"."OrderId"), ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l2"."UnitPrice", CAST("l2"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l2"
    WHERE "o0"."Id" = "l2"."OrderId"), '0.2'))), '0.0') AS "TotalRevenue"
FROM (
    SELECT "o"."Id", CAST(strftime('%Y', "o"."PlacedAt") AS INTEGER) AS "Key"
    FROM "Orders" AS "o"
    WHERE ef_compare(ef_add((
        SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId"), ef_multiply((
        SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId"), '0.2')), '500.0') > 0
) AS "o0"
GROUP BY "o0"."Key"
```

All computed values are evaluated **in the database** -- no data is fetched to memory for filtering or aggregation.

## Example: Customer Profile

String concatenation, date arithmetic, and nullable checks all translate cleanly to SQL:

::: expressive-sample
db.Customers
.Where(c => c.IsActive())
.OrderBy(c => c.DisplayName())
.Select(c => new { c.Id, Display = c.DisplayName() })
\---setup---
public static class CustomerProfile
{
\[Expressive]
public static string DisplayName(this Customer c)
\=> c.Name + (c.Country != null ? " (" + c.Country + ")" : "");

```
[Expressive]
public static bool IsActive(this Customer c)
    => c.JoinedAt >= new DateTime(2023, 1, 1);
```

}
:::

```csharp
db
    .Customers
    .Where(c => c.IsActive())
    .OrderBy(c => c.DisplayName())
    .Select(c => new { c.Id, Display = c.DisplayName() })

// Setup
public static class CustomerProfile
{
    [Expressive]
    public static string DisplayName(this Customer c)
        => c.Name + (c.Country != null ? " (" + c.Country + ")" : "");

    [Expressive]
    public static bool IsActive(this Customer c)
        => c.JoinedAt >= new DateTime(2023, 1, 1);
}
```

**Generated SQL:**

```sql
SELECT "c"."Id", "c"."Name" || CASE
    WHEN "c"."Country" IS NOT NULL THEN ' (' || "c"."Country" || ')'
    ELSE ''
END AS "Display"
FROM "Customers" AS "c"
WHERE "c"."JoinedAt" >= '2023-01-01 00:00:00'
ORDER BY "c"."Name" || CASE
    WHEN "c"."Country" IS NOT NULL THEN ' (' || "c"."Country" || ')'
    ELSE ''
END
```

## Example: Product Catalog

Boolean flags and arithmetic combine naturally. Here `IsAvailable` and a derived price-tier flag compose into a single predicate:

::: expressive-sample
db.Products
.Where(p => p.IsAvailable() && p.IsBudget())
.OrderBy(p => p.StockQuantity)
.Select(p => new { p.Id, p.Name, p.ListPrice, p.StockQuantity })
\---setup---
public static class ProductCatalog
{
\[Expressive]
public static bool IsAvailable(this Product p) => p.StockQuantity > 0;

```
[Expressive]
public static bool IsBudget(this Product p) => p.ListPrice < 50m;

[Expressive]
public static decimal SavingsVs(this Product p, decimal msrp)
    => msrp - p.ListPrice;
```

}
:::

```csharp
db
    .Products
    .Where(p => p.IsAvailable() && p.IsBudget())
    .OrderBy(p => p.StockQuantity)
    .Select(p => new { p.Id, p.Name, p.ListPrice, p.StockQuantity })

// Setup
public static class ProductCatalog
{
    [Expressive]
    public static bool IsAvailable(this Product p) => p.StockQuantity > 0;

    [Expressive]
    public static bool IsBudget(this Product p) => p.ListPrice < 50m;

    [Expressive]
    public static decimal SavingsVs(this Product p, decimal msrp)
        => msrp - p.ListPrice;
}
```

**Generated SQL:**

```sql
SELECT "p"."Id", "p"."Name", "p"."ListPrice", "p"."StockQuantity"
FROM "Products" AS "p"
WHERE "p"."StockQuantity" > 0 AND ef_compare("p"."ListPrice", '50.0') < 0
ORDER BY "p"."StockQuantity"
```

## Collection Aggregates

Computed members can include LINQ aggregation over navigation collections. EF Core translates these to efficient correlated subqueries:

::: expressive-sample
db.Customers
.Where(c => c.LifetimeSpend() > 500m)
.Select(c => new { c.Id, c.Name, Spend = c.LifetimeSpend(), Orders = c.OrderCount() })
\---setup---
public static class CustomerStats
{
\[Expressive]
public static int OrderCount(this Customer c) => c.Orders.Count();

```
[Expressive]
public static decimal LifetimeSpend(this Customer c)
    => c.Orders.Sum(o => o.Items.Sum(i => i.UnitPrice * i.Quantity));

[Expressive]
public static bool HasRecentOrder(this Customer c)
    => c.Orders.Any(o => o.PlacedAt >= new DateTime(2024, 1, 1));
```

}
:::

```csharp
db
    .Customers
    .Where(c => c.LifetimeSpend() > 500m)
    .Select(c => new { c.Id, c.Name, Spend = c.LifetimeSpend(), Orders = c.OrderCount() })

// Setup
public static class CustomerStats
{
    [Expressive]
    public static int OrderCount(this Customer c) => c.Orders.Count();

    [Expressive]
    public static decimal LifetimeSpend(this Customer c)
        => c.Orders.Sum(o => o.Items.Sum(i => i.UnitPrice * i.Quantity));

    [Expressive]
    public static bool HasRecentOrder(this Customer c)
        => c.Orders.Any(o => o.PlacedAt >= new DateTime(2024, 1, 1));
}
```

**Generated SQL:**

```sql
SELECT "c"."Id", "c"."Name", (
    SELECT COALESCE(ef_sum((
        SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l0"
        WHERE "o0"."Id" = "l0"."OrderId")), '0.0')
    FROM "Orders" AS "o0"
    WHERE "c"."Id" = "o0"."CustomerId") AS "Spend", (
    SELECT COUNT(*)
    FROM "Orders" AS "o1"
    WHERE "c"."Id" = "o1"."CustomerId") AS "Orders"
FROM "Customers" AS "c"
WHERE ef_compare((
    SELECT COALESCE(ef_sum((
        SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId")), '0.0')
    FROM "Orders" AS "o"
    WHERE "c"."Id" = "o"."CustomerId"), '500.0') > 0
```

## Tips

::: tip Compose freely
`[Expressive]` members can call other `[Expressive]` members. Build from simple building blocks to complex compositions -- the expander resolves them recursively.
:::

::: tip Keep it pure
Expressive members should be pure computations with no side effects. Everything must be translatable to SQL by your LINQ provider.
:::

::: tip Property vs. extension method
If you own the entity, prefer `[Expressive]` properties for a natural call site (`o.GrandTotal`). For types you cannot modify, `[Expressive]` extension methods (`o.GrandTotal()`) are equivalent in translation power.
:::

::: warning Avoid N+1 traps
If a computed member references navigation properties, make sure to structure your queries so EF Core can generate a single efficient query. Using computed members in `Select` and `Where` at the top level is safe.
:::

## See Also

* [Reusable Query Filters](./reusable-query-filters) -- Boolean computed properties as filter predicates
* [DTO Projections with Constructors](./dto-projections) -- project computed values into DTOs
* [Scoring and Classification](./scoring-classification) -- computed properties with switch expressions
