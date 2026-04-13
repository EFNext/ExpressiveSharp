---
url: 'https://efnext.github.io/ExpressiveSharp/recipes/scoring-classification.md'
---
# Scoring & Classification

This recipe shows how to use C# pattern matching -- switch expressions, `is` patterns, relational patterns, and more -- inside `[Expressive]` members to compute scores, tiers, and labels directly in SQL.

## Banding with Relational Patterns

Mapping numeric ranges to labels reads naturally as a switch expression and translates to a SQL `CASE`:

::: expressive-sample
db.Products
.GroupBy(p => p.PriceBand())
.Select(g => new { Band = g.Key, Count = g.Count() })
.OrderBy(x => x.Band)
\---setup---
public static class ProductBand
{
\[Expressive]
public static string PriceBand(this Product p) => p.ListPrice switch
{
\>= 500m => "A",
\>= 200m => "B",
\>= 100m => "C",
\>= 50m  => "D",
\_       => "E"
};

```
[Expressive]
public static bool IsPremium(this Product p) => p.ListPrice >= 500m;
```

}
:::

```csharp
db
    .Products
    .GroupBy(p => p.PriceBand())
    .Select(g => new { Band = g.Key, Count = g.Count() })
    .OrderBy(x => x.Band)

// Setup
public static class ProductBand
{
    [Expressive]
    public static string PriceBand(this Product p) => p.ListPrice switch
    {
        >= 500m => "A",
        >= 200m => "B",
        >= 100m => "C",
        >= 50m  => "D",
        _       => "E"
    };

    [Expressive]
    public static bool IsPremium(this Product p) => p.ListPrice >= 500m;
}
```

**Generated SQL:**

```sql
SELECT "p0"."Key" AS "Band", COUNT(*) AS "Count"
FROM (
    SELECT CASE
        WHEN ef_compare("p"."ListPrice", '500.0') >= 0 THEN 'A'
        WHEN ef_compare("p"."ListPrice", '200.0') >= 0 THEN 'B'
        WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'C'
        WHEN ef_compare("p"."ListPrice", '50.0') >= 0 THEN 'D'
        ELSE 'E'
    END AS "Key"
    FROM "Products" AS "p"
) AS "p0"
GROUP BY "p0"."Key"
ORDER BY "p0"."Key"
```

## Customer Tiers with `and` / `or` Patterns

Use `and` and `or` patterns to express range bands cleanly:

::: expressive-sample
db.Customers
.GroupBy(c => c.Tier())
.Select(g => new { Tier = g.Key, Count = g.Count() })
\---setup---
public static class CustomerTier
{
\[Expressive]
public static int OrderCount(this Customer c) => c.Orders.Count();

```
[Expressive]
public static string Tier(this Customer c) => c.OrderCount() switch
{
    >= 50             => "Platinum",
    >= 20 and < 50    => "Gold",
    >= 5 and < 20     => "Silver",
    _                 => "Bronze"
};

[Expressive]
public static bool IsLoyalty(this Customer c) => c.OrderCount() >= 10;
```

}
:::

```csharp
db
    .Customers
    .GroupBy(c => c.Tier())
    .Select(g => new { Tier = g.Key, Count = g.Count() })

// Setup
public static class CustomerTier
{
    [Expressive]
    public static int OrderCount(this Customer c) => c.Orders.Count();

    [Expressive]
    public static string Tier(this Customer c) => c.OrderCount() switch
    {
        >= 50             => "Platinum",
        >= 20 and < 50    => "Gold",
        >= 5 and < 20     => "Silver",
        _                 => "Bronze"
    };

    [Expressive]
    public static bool IsLoyalty(this Customer c) => c.OrderCount() >= 10;
}
```

**Generated SQL:**

```sql
SELECT "c0"."Key" AS "Tier", COUNT(*) AS "Count"
FROM (
    SELECT CASE
        WHEN (
            SELECT COUNT(*)
            FROM "Orders" AS "o"
            WHERE "c"."Id" = "o"."CustomerId") >= 50 THEN 'Platinum'
        WHEN (
            SELECT COUNT(*)
            FROM "Orders" AS "o0"
            WHERE "c"."Id" = "o0"."CustomerId") >= 20 AND (
            SELECT COUNT(*)
            FROM "Orders" AS "o1"
            WHERE "c"."Id" = "o1"."CustomerId") < 50 THEN 'Gold'
        WHEN (
            SELECT COUNT(*)
            FROM "Orders" AS "o2"
            WHERE "c"."Id" = "o2"."CustomerId") >= 5 AND (
            SELECT COUNT(*)
            FROM "Orders" AS "o3"
            WHERE "c"."Id" = "o3"."CustomerId") < 20 THEN 'Silver'
        ELSE 'Bronze'
    END AS "Key"
    FROM "Customers" AS "c"
) AS "c0"
GROUP BY "c0"."Key"
```

## Multi-Field Classification with Property Patterns

Property patterns match on multiple fields of the current instance simultaneously. This is useful for multi-dimensional classification:

::: expressive-sample
db.Products
.Select(p => new { p.Id, p.Name, Category = p.StockCategory() })
\---setup---
public static class StockClassifier
{
\[Expressive]
public static string StockCategory(this Product p) => p switch
{
{ StockQuantity: 0 }                        => "OutOfStock",
{ StockQuantity: < 10, ListPrice: >= 500m } => "LowStockPremium",
{ StockQuantity: < 10 }                     => "LowStock",
{ StockQuantity: >= 100 }                   => "WellStocked",
\_                                           => "Normal"
};
}
:::

```csharp
db
    .Products
    .Select(p => new { p.Id, p.Name, Category = p.StockCategory() })

// Setup
public static class StockClassifier
{
    [Expressive]
    public static string StockCategory(this Product p) => p switch
    {
        { StockQuantity: 0 }                        => "OutOfStock",
        { StockQuantity: < 10, ListPrice: >= 500m } => "LowStockPremium",
        { StockQuantity: < 10 }                     => "LowStock",
        { StockQuantity: >= 100 }                   => "WellStocked",
        _                                           => "Normal"
    };
}
```

**Generated SQL:**

```sql
SELECT "p"."Id", "p"."Name", CASE
    WHEN "p"."StockQuantity" = 0 THEN 'OutOfStock'
    WHEN "p"."StockQuantity" < 10 AND ef_compare("p"."ListPrice", '500.0') >= 0 THEN 'LowStockPremium'
    WHEN "p"."StockQuantity" < 10 THEN 'LowStock'
    WHEN "p"."StockQuantity" >= 100 THEN 'WellStocked'
    ELSE 'Normal'
END AS "Category"
FROM "Products" AS "p"
```

## `is` Patterns for Boolean Flags

Use `is` patterns for concise Boolean members:

::: expressive-sample
db.Products
.Where(p => p.IsInStock() && p.IsBudget())
.Select(p => new { p.Id, p.Name, p.ListPrice })
\---setup---
public static class ProductFlags
{
\[Expressive]
public static bool IsInStock(this Product p) => p.StockQuantity is > 0;

```
[Expressive]
public static bool NeedsReorder(this Product p) => p.StockQuantity is >= 0 and <= 5;

[Expressive]
public static bool IsBudget(this Product p) => p.ListPrice is > 0m and < 50m;

[Expressive]
public static bool HasNoStock(this Product p) => p.StockQuantity is 0;
```

}
:::

```csharp
db
    .Products
    .Where(p => p.IsInStock() && p.IsBudget())
    .Select(p => new { p.Id, p.Name, p.ListPrice })

// Setup
public static class ProductFlags
{
    [Expressive]
    public static bool IsInStock(this Product p) => p.StockQuantity is > 0;

    [Expressive]
    public static bool NeedsReorder(this Product p) => p.StockQuantity is >= 0 and <= 5;

    [Expressive]
    public static bool IsBudget(this Product p) => p.ListPrice is > 0m and < 50m;

    [Expressive]
    public static bool HasNoStock(this Product p) => p.StockQuantity is 0;
}
```

**Generated SQL:**

```sql
SELECT "p"."Id", "p"."Name", "p"."ListPrice"
FROM "Products" AS "p"
WHERE "p"."StockQuantity" > 0 AND ef_compare("p"."ListPrice", '0.0') > 0 AND ef_compare("p"."ListPrice", '50.0') < 0
```

## Positional Patterns

ExpressiveSharp supports positional (deconstruct) patterns. If your type defines a `Deconstruct` method, you can match on it:

```csharp
public class Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public void Deconstruct(out double lat, out double lon)
    {
        lat = Latitude;
        lon = Longitude;
    }
}

public class Location
{
    public int Id { get; set; }
    public Coordinate Position { get; set; }

    [Expressive]
    public string Hemisphere => Position switch
    {
        (>= 0, _) => "Northern",
        _         => "Southern"
    };
}
```

## List Patterns

ExpressiveSharp supports list patterns for fixed-length matching:

```csharp
public class Measurement
{
    public int Id { get; set; }
    public int[] Readings { get; set; }

    [Expressive]
    public string ReadingCategory => Readings switch
    {
        [0, 0, 0] => "Zero",
        [_, _, _] => "Triple",
        [_, _]    => "Double",
        [_]       => "Single",
        _         => "Other"
    };
}
```

## Combining Classification with Aggregation

Compose `[Expressive]` classification members to build rich query results:

::: expressive-sample
db.Orders
.Where(o => o.IsRecent())
.GroupBy(o => o.ValueBand())
.Select(g => new { Band = g.Key, Count = g.Count(), Total = g.Sum(o => o.GrandTotal()) })
.OrderBy(x => x.Band)
\---setup---
public static class OrderScoring
{
\[Expressive]
public static decimal GrandTotal(this Order o)
\=> o.Items.Sum(i => i.UnitPrice \* i.Quantity);

```
[Expressive]
public static string ValueBand(this Order o) => o.GrandTotal() switch
{
    >= 1000m => "High",
    >= 250m  => "Medium",
    _        => "Low"
};

[Expressive]
public static bool IsRecent(this Order o) => o.PlacedAt >= new DateTime(2024, 1, 1);
```

}
:::

```csharp
db
    .Orders
    .Where(o => o.IsRecent())
    .GroupBy(o => o.ValueBand())
    .Select(g => new { Band = g.Key, Count = g.Count(), Total = g.Sum(o => o.GrandTotal()) })
    .OrderBy(x => x.Band)

// Setup
public static class OrderScoring
{
    [Expressive]
    public static decimal GrandTotal(this Order o)
        => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static string ValueBand(this Order o) => o.GrandTotal() switch
    {
        >= 1000m => "High",
        >= 250m  => "Medium",
        _        => "Low"
    };

    [Expressive]
    public static bool IsRecent(this Order o) => o.PlacedAt >= new DateTime(2024, 1, 1);
}
```

**Generated SQL:**

```sql
SELECT "o0"."Key" AS "Band", COUNT(*) AS "Count", COALESCE(ef_sum((
    SELECT COALESCE(ef_sum(ef_multiply("l1"."UnitPrice", CAST("l1"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l1"
    WHERE "o0"."Id" = "l1"."OrderId")), '0.0') AS "Total"
FROM (
    SELECT "o"."Id", CASE
        WHEN ef_compare((
            SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
            FROM "LineItems" AS "l"
            WHERE "o"."Id" = "l"."OrderId"), '1000.0') >= 0 THEN 'High'
        WHEN ef_compare((
            SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
            FROM "LineItems" AS "l0"
            WHERE "o"."Id" = "l0"."OrderId"), '250.0') >= 0 THEN 'Medium'
        ELSE 'Low'
    END AS "Key"
    FROM "Orders" AS "o"
    WHERE "o"."PlacedAt" >= '2024-01-01 00:00:00'
) AS "o0"
GROUP BY "o0"."Key"
ORDER BY "o0"."Key"
```

## Using Switch Expressions Inline in LINQ Chains

You can also use switch expressions directly in LINQ chains via `ExpressiveDbSet<T>` or `IExpressiveQueryable<T>`, without defining a separate `[Expressive]` member:

::: expressive-sample
db.Orders
.Select(o => new
{
o.Id,
Tier = o.Status switch
{
OrderStatus.Delivered => "Completed",
OrderStatus.Shipped   => "InTransit",
OrderStatus.Paid      => "Awaiting",
OrderStatus.Pending   => "New",
\_                     => "Other"
}
})
:::

```csharp
db
    .Orders
    .Select(o => new
    {
        o.Id,
        Tier = o.Status switch
        {
            OrderStatus.Delivered => "Completed",
            OrderStatus.Shipped => "InTransit",
            OrderStatus.Paid => "Awaiting",
            OrderStatus.Pending => "New",
            _ => "Other"
        }
    })
```

**Generated SQL:**

```sql
.param set @Delivered 3
.param set @Shipped 2
.param set @Paid 1
.param set @Pending 0

SELECT "o"."Id", CASE
    WHEN "o"."Status" = @Delivered THEN 'Completed'
    WHEN "o"."Status" = @Shipped THEN 'InTransit'
    WHEN "o"."Status" = @Paid THEN 'Awaiting'
    WHEN "o"."Status" = @Pending THEN 'New'
    ELSE 'Other'
END AS "Tier"
FROM "Orders" AS "o"
```

See [Modern Syntax in LINQ Chains](./modern-syntax-in-linq) for more on this approach.

## Tips

::: tip Use `_` as the default arm
Always include a discard arm to avoid generating a ternary chain with no final fallback. This prevents potential null results.
:::

::: tip Keep arms ordered from most to least specific
The generator emits a ternary chain in arm order. Put the most restrictive cases first for correct evaluation.
:::

::: tip Compose with filters
Classification members work in `Where`, `GroupBy`, and `OrderBy` just like any other `[Expressive]` member. This is how you build reporting queries that compute business categories entirely in SQL.
:::

## See Also

* [Computed Entity Properties](./computed-properties) -- building blocks for classification
* [Modern Syntax in LINQ Chains](./modern-syntax-in-linq) -- switch expressions inline in queries
* [Nullable Navigation Properties](./nullable-navigation) -- safely handling null in classification logic
