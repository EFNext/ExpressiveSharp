---
url: 'https://efnext.github.io/ExpressiveSharp/guide/extension-members.md'
---
# Extension Members

ExpressiveSharp supports `[Expressive]` on both traditional extension methods (any .NET version) and C# 14 extension members (.NET 10+). This lets you define query logic outside of your entity classes -- useful for keeping entities clean, applying logic to types you don't own, or grouping related query helpers.

## Extension Methods

Add `[Expressive]` to any extension method in a **static class** and use it inside your queries:

::: expressive-sample
db.Orders
.Where(o => o.IsHighValue(500m))
.Select(o => new { o.Id, Email = o.SafeCustomerEmail() })
\---setup---
public static class OrderExtensions
{
\[Expressive]
public static bool IsHighValue(this Order order, decimal threshold)
\=> order.Items.Sum(i => i.UnitPrice \* i.Quantity) > threshold;

```
[Expressive]
public static string? SafeCustomerEmail(this Order order)
    => order.Customer != null ? order.Customer.Email : null;
```

}
:::

```csharp
db
    .Orders
    .Where(o => o.IsHighValue(500m))
    .Select(o => new { o.Id, Email = o.SafeCustomerEmail() })

// Setup
public static class OrderExtensions
{
    [Expressive]
    public static bool IsHighValue(this Order order, decimal threshold)
        => order.Items.Sum(i => i.UnitPrice * i.Quantity) > threshold;

    [Expressive]
    public static string? SafeCustomerEmail(this Order order)
        => order.Customer != null ? order.Customer.Email : null;
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", "c"."Email"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE ef_compare((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), '500.0') > 0
```

The extension method body is inlined into the expression tree -- the provider sees the expanded arithmetic and member access, not a method call. The query tabs above show how each provider translates the result.

## Extension Methods on Non-Entity Types

Extension methods work on **any type**, not just entities:

::: expressive-sample
db.Products.Where(p => p.Name.ContainsIgnoreCase("widget"))
\---setup---
public static class StringExtensions
{
\[Expressive]
public static bool ContainsIgnoreCase(this string source, string value)
\=> source.ToLower().Contains(value.ToLower());
}
:::

```csharp
db
    .Products
    .Where(p => p.Name.ContainsIgnoreCase("widget"))

// Setup
public static class StringExtensions
{
    [Expressive]
    public static bool ContainsIgnoreCase(this string source, string value)
        => source.ToLower().Contains(value.ToLower());
}
```

**Generated SQL:**

```sql
SELECT "p"."Id", "p"."Category", "p"."ListPrice", "p"."Name", "p"."StockQuantity"
FROM "Products" AS "p"
WHERE instr(lower("p"."Name"), 'widget') > 0
```

Primitive extensions compose the same way:

::: expressive-sample
db.LineItems.Select(i => new { i.Id, SquaredQty = i.Quantity.Squared() })
\---setup---
public static class IntExtensions
{
\[Expressive]
public static int Squared(this int i) => i \* i;
}
:::

```csharp
db
    .LineItems
    .Select(i => new { i.Id, SquaredQty = i.Quantity.Squared() })

// Setup
public static class IntExtensions
{
    [Expressive]
    public static int Squared(this int i) => i * i;
}
```

**Generated SQL:**

```sql
SELECT "l"."Id", "l"."Quantity" * "l"."Quantity" AS "SquaredQty"
FROM "LineItems" AS "l"
```

## Composing Extension Methods

Extension methods can reference other `[Expressive]` members -- properties, methods, or other extension methods. `ExpandExpressives()` resolves them transitively:

::: expressive-sample
db.Customers.Where(c => c.IsVip())
\---setup---
public static class CustomerExtensions
{
\[Expressive]
public static decimal TotalSpent(this Customer c)
\=> c.Orders.Sum(o => o.Items.Sum(i => i.UnitPrice \* i.Quantity));

```
[Expressive]
public static bool IsVip(this Customer c)
    => c.TotalSpent() > 10000m;   // calls another [Expressive] extension
```

}
:::

```csharp
db
    .Customers
    .Where(c => c.IsVip())

// Setup
public static class CustomerExtensions
{
    [Expressive]
    public static decimal TotalSpent(this Customer c)
        => c.Orders.Sum(o => o.Items.Sum(i => i.UnitPrice * i.Quantity));

    [Expressive]
    public static bool IsVip(this Customer c)
        => c.TotalSpent() > 10000m;   // calls another [Expressive] extension
}
```

**Generated SQL:**

```sql
SELECT "c"."Id", "c"."Country", "c"."Email", "c"."JoinedAt", "c"."Name"
FROM "Customers" AS "c"
WHERE ef_compare((
    SELECT COALESCE(ef_sum((
        SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId")), '0.0')
    FROM "Orders" AS "o"
    WHERE "c"."Id" = "o"."CustomerId"), '10000.0') > 0
```

## C# 14 Extension Members (.NET 10+)

On .NET 10 with C# 14, you can use the new `extension(T)` syntax to define extension **properties** and methods. This is cleaner than traditional extension methods for property-like logic:

```csharp
public static class OrderExtensions
{
    extension(Order o)
    {
        [Expressive]
        public decimal Total => o.Items.Sum(i => i.UnitPrice * i.Quantity);

        [Expressive]
        public string Grade => o.Items.Sum(i => i.UnitPrice * i.Quantity) switch
        {
            >= 1000m => "Premium",
            >= 100m  => "Standard",
            _        => "Budget",
        };

        [Expressive]
        public int ScaledItemCount(int factor) => o.Items.Count() * factor;
    }
}

// Use like any other property/method:
var orders = db.Orders
    .Where(o => o.Total > 500m)
    .Select(o => new { o.Id, o.Total, o.Grade });
```

### Extension Members on Primitives and Interfaces

C# 14 extensions work on any type, including primitives and interfaces:

```csharp
public static class IntExtensions
{
    extension(int i)
    {
        [Expressive]
        public int Squared => i * i;
    }
}

db.LineItems.Select(i => new { i.Id, SquaredQty = i.Quantity.Squared });
```

### Block Bodies and Switch Expressions

C# 14 extension members support all the same features as regular `[Expressive]` members -- block bodies, switch expressions, pattern matching, and null-conditional operators:

```csharp
public static class OrderExtensions
{
    extension(Order o)
    {
        [Expressive(AllowBlockBody = true)]
        public string GetStatus()
        {
            if (o.Status == OrderStatus.Delivered && o.Items.Count() > 0)
                return "Completed";
            return "In Progress";
        }

        [Expressive]
        public bool IsHighValue => o.Items.Sum(i => i.UnitPrice * i.Quantity) is > 100m;
    }
}
```

::: warning .NET 10+ Only
C# 14 extension members require .NET 10 or later. On .NET 8, use traditional extension methods in static classes instead.
:::

## Extension Methods vs `[ExpressiveFor]`

Both let you add query logic to types you don't own, but they serve different purposes:

| | Extension Methods | `[ExpressiveFor]` |
|---|---|---|
| **Purpose** | Add new query logic | Provide an expression body for an existing method |
| **Call site** | `entity.MyExtension()` | `Math.Clamp(value, min, max)` (original call site) |
| **When to use** | You're adding new functionality | You want an existing method (BCL, third-party) to become translatable |

See [\[ExpressiveFor\] Mapping](/reference/expressive-for) for details on mapping existing members.

## Important Rules

* Traditional extension methods **must be in a static class**.
* C# 14 extension members must be in a static class with an `extension(T)` block.
* The `this` parameter (or `extension(T)` parameter) represents the entity instance in the generated expression.
* All standard `[Expressive]` features work: `AllowBlockBody`, `Transformers`, composition with other `[Expressive]` members.

## See Also

* [\[Expressive\] Properties](./expressive-properties) -- defining computed properties on entities directly
* [\[Expressive\] Methods](./expressive-methods) -- defining computed methods on entities
* [Reusable Query Filters](/recipes/reusable-query-filters) -- practical example of extension methods as reusable filters
* [\[ExpressiveFor\] Mapping](/reference/expressive-for) -- mapping existing methods on types you don't own
