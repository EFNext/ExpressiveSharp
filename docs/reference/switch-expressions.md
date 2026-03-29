# Switch Expressions

Switch expressions are one of the most useful C# features that ExpressiveSharp enables in expression trees. They are translated to nested ternary expressions at compile time, which LINQ providers like EF Core map to SQL `CASE` expressions.

## Basic Syntax

Mark any property or method with `[Expressive]` and use a switch expression in the body:

```csharp
[Expressive]
public string GetGrade() => Price switch
{
    >= 100 => "Premium",
    >= 50  => "Standard",
    _      => "Budget",
};
```

The source generator produces a chain of conditional expressions:

```csharp
Price >= 100 ? "Premium"
: Price >= 50 ? "Standard"
: "Budget"
```

EF Core translates this to:

```sql
SELECT CASE
    WHEN "o"."Price" >= 100.0 THEN 'Premium'
    WHEN "o"."Price" >= 50.0 THEN 'Standard'
    ELSE 'Budget'
END AS "Grade"
FROM "Orders" AS "o"
```

## Relational Patterns

Relational operators (`<`, `<=`, `>`, `>=`) work in switch arms:

```csharp
[Expressive]
public string PriceCategory => Price switch
{
    < 10    => "Cheap",
    < 50    => "Moderate",
    < 100   => "Expensive",
    >= 100  => "Premium",
};
```

::: warning
Without a discard arm (`_`), the generated expression has no fallback. If no arm matches at runtime, a `SwitchExpressionException` would be thrown in C#. In SQL, the result is `NULL` (the `ELSE` clause is omitted). Always include a discard arm for safety.
:::

## `and` / `or` Combinators

Combine patterns with `and` and `or` for range checks and alternatives:

```csharp
[Expressive]
public string GetBand() => Score switch
{
    >= 90 and <= 100 => "Excellent",
    >= 70 and < 90   => "Good",
    >= 50 and < 70   => "Average",
    _                => "Poor",
};
```

Generated SQL:

```sql
CASE
    WHEN "s"."Score" >= 90 AND "s"."Score" <= 100 THEN 'Excellent'
    WHEN "s"."Score" >= 70 AND "s"."Score" < 90 THEN 'Good'
    WHEN "s"."Score" >= 50 AND "s"."Score" < 70 THEN 'Average'
    ELSE 'Poor'
END
```

Using `or` for alternative values:

```csharp
[Expressive]
public string GetDayType() => DayOfWeek switch
{
    0 or 6 => "Weekend",
    _      => "Weekday",
};
```

## `when` Guards

Guards add additional boolean conditions to switch arms:

```csharp
[Expressive]
public string Classify() => Quantity switch
{
    > 100 when Price < 10 => "Bulk Bargain",
    > 100                 => "Bulk Order",
    > 0                   => "Standard",
    _                     => "Empty",
};
```

Generated expression:

```csharp
(Quantity > 100 && Price < 10) ? "Bulk Bargain"
: Quantity > 100 ? "Bulk Order"
: Quantity > 0 ? "Standard"
: "Empty"
```

The guard condition is combined with the pattern using `&&` in the generated expression.

## Type Patterns with Declaration Variables

Switch arms can match on type and bind the result to a variable:

```csharp
[Expressive]
public static string Describe(this Animal animal) => animal switch
{
    Dog d   => "Dog named " + d.Name,
    Cat c   => "Cat: " + c.Breed,
    _       => "Unknown animal",
};
```

The generator produces type-check and cast expressions:

```csharp
animal is Dog ? "Dog named " + ((Dog)animal).Name
: animal is Cat ? "Cat: " + ((Cat)animal).Breed
: "Unknown animal"
```

::: info
Declaration variables work within switch arms. The generated expression binds them via cast expressions. This is particularly useful for EF Core inheritance hierarchies (TPH, TPT, TPC).
:::

## Constant Patterns

Match against specific constant values:

```csharp
[Expressive]
public string StatusLabel => StatusCode switch
{
    0 => "Pending",
    1 => "Active",
    2 => "Completed",
    3 => "Cancelled",
    _ => "Unknown",
};
```

## Nested Switch Expressions

Switch expressions can be nested for multi-dimensional classification:

```csharp
[Expressive]
public string GetPriority() => Category switch
{
    "Electronics" => Price switch
    {
        >= 500 => "High",
        >= 100 => "Medium",
        _      => "Low",
    },
    "Food" => "Standard",
    _      => "Default",
};
```

Generated SQL:

```sql
CASE
    WHEN "o"."Category" = 'Electronics' THEN
        CASE
            WHEN "o"."Price" >= 500.0 THEN 'High'
            WHEN "o"."Price" >= 100.0 THEN 'Medium'
            ELSE 'Low'
        END
    WHEN "o"."Category" = 'Food' THEN 'Standard'
    ELSE 'Default'
END
```

## Property Patterns in Switch Arms

Match against an object's properties:

```csharp
[Expressive]
public string ClassifyOrder() => this switch
{
    { Quantity: > 100, Price: >= 50 } => "Large Premium",
    { Quantity: > 100 }               => "Large Standard",
    { Price: >= 50 }                  => "Small Premium",
    _                                 => "Small Standard",
};
```

## SQL `CASE` Expression Output

All switch expressions map to SQL `CASE` expressions. Here is a summary of how different patterns translate:

| C# Pattern | SQL Condition |
|-------------|---------------|
| `>= 100` | `WHEN col >= 100` |
| `>= 80 and < 90` | `WHEN col >= 80 AND col < 90` |
| `1 or 2` | `WHEN col = 1 OR col = 2` |
| `"Premium"` | `WHEN col = 'Premium'` |
| `_ (discard)` | `ELSE` |
| `> 50 when Flag` | `WHEN col > 50 AND flag = 1` |

## Best Practices

1. **Always include a discard arm** (`_`) to ensure the `CASE` expression has an `ELSE` clause.

2. **Keep arms simple** for SQL translation. Each arm's pattern and result should be a simple expression. Avoid calling methods that cannot be translated to SQL.

3. **Order arms from most specific to least specific**, just as you would in C#. The generated ternary chain evaluates top-to-bottom, matching the SQL `CASE WHEN` evaluation order.

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
