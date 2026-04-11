---
url: 'https://efnext.github.io/ExpressiveSharp/guide/extension-members.md'
---
# Extension Members

ExpressiveSharp supports `[Expressive]` on both traditional extension methods (any .NET version) and C# 14 extension members (.NET 10+). This lets you define query logic outside of your entity classes — useful for keeping entities clean, applying logic to types you don't own, or grouping related query helpers.

## Extension Methods

Add `[Expressive]` to any extension method in a **static class**:

```csharp
using ExpressiveSharp;

public static class OrderExtensions
{
    [Expressive]
    public static bool IsHighValue(this Order order, double threshold)
        => order.Price * order.Quantity > threshold;

    [Expressive]
    public static string? SafeCustomerEmail(this Order order)
        => order.Customer?.Email;
}
```

Use them in queries just like regular extension methods:

```csharp
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => o.IsHighValue(500))
    .Select(o => new { o.Id, Email = o.SafeCustomerEmail() })
    .ToList();
```

The extension method body is inlined into the expression tree — EF Core sees `o.Price * o.Quantity > 500`, not a method call.

## Extension Methods on Non-Entity Types

Extension methods work on **any type**, not just entities:

```csharp
public static class IntExtensions
{
    [Expressive]
    public static int Squared(this int i) => i * i;
}

public static class StringExtensions
{
    [Expressive]
    public static bool ContainsIgnoreCase(this string source, string value)
        => source.ToLower().Contains(value.ToLower());
}
```

Usage in queries:

```csharp
var results = db.Players
    .AsExpressiveDbSet()
    .Select(p => new { p.Name, SquaredScore = p.Score.Squared() })
    .ToList();

var widgets = db.Products
    .AsExpressiveDbSet()
    .Where(p => p.Name.ContainsIgnoreCase("widget"))
    .ToList();
```

## Composing Extension Methods

Extension methods can reference other `[Expressive]` members — properties, methods, or other extension methods. `ExpandExpressives()` resolves them transitively:

```csharp
public static class UserExtensions
{
    [Expressive]
    public static double TotalSpent(this User user)
        => user.Orders.Sum(o => o.Total);   // Total is [Expressive] on Order

    [Expressive]
    public static bool IsVip(this User user)
        => user.TotalSpent() > 10000;       // calls another [Expressive] extension
}
```

## C# 14 Extension Members (.NET 10+)

On .NET 10 with C# 14, you can use the new `extension(T)` syntax to define extension **properties** and methods. This is cleaner than traditional extension methods for property-like logic:

```csharp
public static class OrderExtensions
{
    extension(Order o)
    {
        [Expressive]
        public double Total => o.Price * o.Quantity;

        [Expressive]
        public string Grade => o.Price switch
        {
            >= 100 => "Premium",
            >= 50  => "Standard",
            _      => "Budget",
        };

        [Expressive]
        public int ScaledQuantity(int factor) => o.Quantity * factor;
    }
}
```

These are used like regular properties and methods on the extended type:

```csharp
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => o.Total > 500)
    .Select(o => new { o.Id, o.Total, o.Grade })
    .ToList();
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

public static class EntityExtensions
{
    extension(IEntity e)
    {
        [Expressive]
        public string Label => e.Id + ": " + e.Name;
    }
}
```

### Block Bodies and Switch Expressions

C# 14 extension members support all the same features as regular `[Expressive]` members — block bodies, switch expressions, pattern matching, and null-conditional operators:

```csharp
public static class EntityExtensions
{
    extension(Entity e)
    {
        [Expressive(AllowBlockBody = true)]
        public string GetStatus()
        {
            if (e.IsActive && e.Value > 0)
                return "Active";
            return "Inactive";
        }

        [Expressive]
        public bool IsHighValue => e.Value is > 100;
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

* [\[Expressive\] Properties](./expressive-properties) — defining computed properties on entities directly
* [\[Expressive\] Methods](./expressive-methods) — defining computed methods on entities
* [Reusable Query Filters](/recipes/reusable-query-filters) — practical example of extension methods as reusable filters
* [\[ExpressiveFor\] Mapping](/reference/expressive-for) — mapping existing methods on types you don't own
