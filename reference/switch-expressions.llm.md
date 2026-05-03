# Switch Expressions

Switch expressions are one of the most useful C# features that ExpressiveSharp enables in expression trees. They are translated to nested ternary expressions at compile time, which LINQ providers map to their native conditional forms (SQL `CASE`, MongoDB `$switch`, etc.).

## Basic Syntax

Mark any property or method with `[Expressive]` and use a switch expression in the body:

```csharp
db
    .Products
    .Select(p => new { p.Name, Tier = p.GetTier() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'Premium'
    WHEN ef_compare("p"."ListPrice", '50.0') >= 0 THEN 'Standard'
    ELSE 'Budget'
END AS "Tier"
FROM "Products" AS "p"
```


The source generator produces a chain of conditional expressions that each provider renders in its own dialect (see the tabs above).

## Relational Patterns

Relational operators (`<`, `<=`, `>`, `>=`) work in switch arms:

```csharp
db
    .Products
    .Select(p => new { p.Name, Category = p.PriceCategory() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN ef_compare("p"."ListPrice", '10.0') < 0 THEN 'Cheap'
    WHEN ef_compare("p"."ListPrice", '50.0') < 0 THEN 'Moderate'
    WHEN ef_compare("p"."ListPrice", '100.0') < 0 THEN 'Expensive'
    WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'Premium'
END AS "Category"
FROM "Products" AS "p"
```


::: warning
Without a discard arm (`_`), the generated expression has no fallback. If no arm matches at runtime, a `SwitchExpressionException` would be thrown in C#. In SQL, the result is `NULL` (the `ELSE` clause is omitted). Always include a discard arm for safety.
:::

## `and` / `or` Combinators

Combine patterns with `and` and `or` for range checks and alternatives:

```csharp
db
    .Products
    .Select(p => new { p.Name, Band = p.GetBand() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN "p"."StockQuantity" >= 90 AND "p"."StockQuantity" <= 100 THEN 'Excellent'
    WHEN "p"."StockQuantity" >= 70 AND "p"."StockQuantity" < 90 THEN 'Good'
    WHEN "p"."StockQuantity" >= 50 AND "p"."StockQuantity" < 70 THEN 'Average'
    ELSE 'Poor'
END AS "Band"
FROM "Products" AS "p"
```


Using `or` for alternative values:

```csharp
db
    .Orders
    .Select(o => new { o.Id, Type = o.GetDayType() })

// Setup
public static class OrderExt
{
    [Expressive]
    public static string GetDayType(this Order o) => (int)o.PlacedAt.DayOfWeek switch
    {
        0 or 6 => "Weekend",
        _      => "Weekday",
    };
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", CASE
    WHEN CAST(strftime('%w', "o"."PlacedAt") AS INTEGER) = 0 OR CAST(strftime('%w', "o"."PlacedAt") AS INTEGER) = 6 THEN 'Weekend'
    ELSE 'Weekday'
END AS "Type"
FROM "Orders" AS "o"
```


## `when` Guards

Guards add additional boolean conditions to switch arms:

```csharp
db
    .LineItems
    .Select(i => new { i.Id, Tag = i.Classify() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "l"."Id", CASE
    WHEN "l"."Quantity" > 100 AND ef_compare("l"."UnitPrice", '10.0') < 0 THEN 'Bulk Bargain'
    WHEN "l"."Quantity" > 100 THEN 'Bulk Order'
    WHEN "l"."Quantity" > 0 THEN 'Standard'
    ELSE 'Empty'
END AS "Tag"
FROM "LineItems" AS "l"
```


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

```csharp
db
    .Orders
    .Select(o => new { o.Id, Label = o.StatusLabel() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", CASE
    WHEN "o"."Status" = 0 THEN 'Pending'
    WHEN "o"."Status" = 1 THEN 'Paid'
    WHEN "o"."Status" = 2 THEN 'Shipped'
    WHEN "o"."Status" = 3 THEN 'Delivered'
    WHEN "o"."Status" = 4 THEN 'Refunded'
    ELSE 'Unknown'
END AS "Label"
FROM "Orders" AS "o"
```


## Nested Switch Expressions

Switch expressions can be nested for multi-dimensional classification:

```csharp
db
    .Products
    .Select(p => new { p.Name, Priority = p.GetPriority() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN "p"."Category" = 'Electronics' THEN CASE
        WHEN ef_compare("p"."ListPrice", '500.0') >= 0 THEN 'High'
        WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'Medium'
        ELSE 'Low'
    END
    WHEN "p"."Category" = 'Food' THEN 'Standard'
    ELSE 'Default'
END AS "Priority"
FROM "Products" AS "p"
```


## Property Patterns in Switch Arms

Match against an object's properties:

```csharp
db
    .LineItems
    .Select(i => new { i.Id, Tag = i.ClassifyItem() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "l"."Id", CASE
    WHEN "l"."Quantity" > 100 AND ef_compare("l"."UnitPrice", '50.0') >= 0 THEN 'Large Premium'
    WHEN "l"."Quantity" > 100 THEN 'Large Standard'
    WHEN ef_compare("l"."UnitPrice", '50.0') >= 0 THEN 'Small Premium'
    ELSE 'Small Standard'
END AS "Tag"
FROM "LineItems" AS "l"
```


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
