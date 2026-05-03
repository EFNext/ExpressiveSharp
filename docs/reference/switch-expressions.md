# Switch Expressions

Switch expressions are one of the most useful C# features that ExpressiveSharp enables in expression trees. They are translated to nested ternary expressions at compile time, which LINQ providers map to their native conditional forms (SQL `CASE`, MongoDB `$switch`, etc.).

## Basic Syntax

Mark any property or method with `[Expressive]` and use a switch expression in the body:

::: expressive-sample
db.Products.Select(p => new { p.Name, Tier = p.GetTier() })
---setup---
public static class ProductExt
{
    [Expressive]
    public static string GetTier(this Product p) => p.ListPrice switch
    {
        >= 100m => "Premium",
        >= 50m  => "Standard",
        _       => "Budget",
    };
}
:::

The source generator produces a chain of conditional expressions that each provider renders in its own dialect (see the tabs above).

## Relational Patterns

Relational operators (`<`, `<=`, `>`, `>=`) work in switch arms:

::: expressive-sample
db.Products.Select(p => new { p.Name, Category = p.PriceCategory() })
---setup---
public static class ProductExt
{
    [Expressive]
    public static string PriceCategory(this Product p) => p.ListPrice switch
    {
        < 10m   => "Cheap",
        < 50m   => "Moderate",
        < 100m  => "Expensive",
        >= 100m => "Premium",
    };
}
:::

::: warning
Without a discard arm (`_`), the generated expression has no fallback. If no arm matches at runtime, a `SwitchExpressionException` would be thrown in C#. In SQL, the result is `NULL` (the `ELSE` clause is omitted). Always include a discard arm for safety.
:::

## `and` / `or` Combinators

Combine patterns with `and` and `or` for range checks and alternatives:

::: expressive-sample
db.Products.Select(p => new { p.Name, Band = p.GetBand() })
---setup---
public static class ProductExt
{
    [Expressive]
    public static string GetBand(this Product p) => p.StockQuantity switch
    {
        >= 90 and <= 100 => "Excellent",
        >= 70 and < 90   => "Good",
        >= 50 and < 70   => "Average",
        _                => "Poor",
    };
}
:::

Using `or` for alternative values:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Type = o.GetDayType() })
---setup---
public static class OrderExt
{
    [Expressive]
    public static string GetDayType(this Order o) => (int)o.PlacedAt.DayOfWeek switch
    {
        0 or 6 => "Weekend",
        _      => "Weekday",
    };
}
:::

## `when` Guards

Guards add additional boolean conditions to switch arms:

::: expressive-sample
db.LineItems.Select(i => new { i.Id, Tag = i.Classify() })
---setup---
public static class LineItemExt
{
    [Expressive]
    public static string Classify(this LineItem i) => i.Quantity switch
    {
        > 100 when i.UnitPrice < 10m => "Bulk Bargain",
        > 100                        => "Bulk Order",
        > 0                          => "Standard",
        _                            => "Empty",
    };
}
:::

The guard condition is combined with the pattern using `&&` in the generated expression, which each provider renders as part of its conditional form.

## Type Patterns with Declaration Variables

Switch arms can match on type and bind the result to a variable:

```csharp
[Expressive]
public static string Describe(this Animal animal) => animal switch
{
    Dog d   => $"Dog named {d.Name}",
    Cat c   => $"Cat: {c.Breed}",
    _       => "Unknown animal",
};
```

The generator produces type-check and cast expressions:

```csharp
animal is Dog ? $"Dog named {((Dog)animal).Name}"
: animal is Cat ? $"Cat: {((Cat)animal).Breed}"
: "Unknown animal"
```

::: info
Declaration variables work within switch arms. The generated expression binds them via cast expressions. This is particularly useful for EF Core inheritance hierarchies (TPH, TPT, TPC).
:::

## Constant Patterns

Match against specific constant values:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Label = o.StatusLabel() })
---setup---
public static class OrderExt
{
    [Expressive]
    public static string StatusLabel(this Order o) => (int)o.Status switch
    {
        0 => "Pending",
        1 => "Paid",
        2 => "Shipped",
        3 => "Delivered",
        4 => "Refunded",
        _ => "Unknown",
    };
}
:::

## Nested Switch Expressions

Switch expressions can be nested for multi-dimensional classification:

::: expressive-sample
db.Products.Select(p => new { p.Name, Priority = p.GetPriority() })
---setup---
public static class ProductExt
{
    [Expressive]
    public static string GetPriority(this Product p) => p.Category switch
    {
        "Electronics" => p.ListPrice switch
        {
            >= 500m => "High",
            >= 100m => "Medium",
            _       => "Low",
        },
        "Food" => "Standard",
        _      => "Default",
    };
}
:::

## Property Patterns in Switch Arms

Match against an object's properties:

::: expressive-sample
db.LineItems.Select(i => new { i.Id, Tag = i.ClassifyItem() })
---setup---
public static class LineItemExt
{
    [Expressive]
    public static string ClassifyItem(this LineItem i) => i switch
    {
        { Quantity: > 100, UnitPrice: >= 50m } => "Large Premium",
        { Quantity: > 100 }                    => "Large Standard",
        { UnitPrice: >= 50m }                  => "Small Premium",
        _                                      => "Small Standard",
    };
}
:::

## Pattern-to-Condition Cheat Sheet

All switch expressions map to conditional expressions in the target language (SQL `CASE`, MongoDB `$switch`, etc.). Here is a summary of how different patterns translate at the C# layer:

| C# Pattern | Generated Condition |
|-------------|---------------------|
| `>= 100` | `col >= 100` |
| `>= 80 and < 90` | `col >= 80 && col < 90` |
| `1 or 2` | `col == 1 \|\| col == 2` |
| `"Premium"` | `col == "Premium"` |
| `_ (discard)` | fallback (`else`) branch |
| `> 50 when Flag` | `col > 50 && Flag` |

## Best Practices

1. **Always include a discard arm** (`_`) to ensure the conditional has a fallback branch.

2. **Keep arms simple** for translation. Each arm's pattern and result should be a simple expression. Avoid calling methods that cannot be translated by your provider.

3. **Order arms from most specific to least specific**, just as you would in C#. The generated ternary chain evaluates top-to-bottom, matching the provider's conditional evaluation order.

4. **Prefer switch expressions over nested ternaries** for readability. The source generator produces ternary chains regardless, but the switch expression in your source code is easier to read and maintain.

5. **Use `[Expressive]` methods for complex switches** rather than inline switch expressions in queries:

    ```csharp
    // Prefer this: reusable and readable
    [Expressive]
    public string GetGrade() => Price switch { ... };

    // Over this: inline in every query
    db.Orders.Select(o => o.Price switch { ... });
    ```

See also [Pattern Matching](./pattern-matching) for the full list of supported patterns.
