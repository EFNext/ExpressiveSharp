# Pattern Matching

The ExpressiveSharp source generator rewrites C# pattern-matching constructs into expression-tree-compatible ternary and binary expressions. LINQ providers like EF Core translate these into SQL `CASE` expressions.

## Supported Patterns

| Pattern | Context | Example |
|---------|---------|---------|
| Constant | switch arm, `is` | `1 => "one"`, `Value is 42` |
| Discard / default | switch arm | `_ => "other"` |
| Type | switch arm | `GroupItem g => ...` |
| Relational | switch arm, `is` | `>= 90 => "A"`, `Value is > 0` |
| `and` combined | switch arm, `is` | `>= 80 and < 90`, `Value is >= 1 and <= 100` |
| `or` combined | switch arm, `is` | `1 or 2 => "low"`, `Value is 0 or > 100` |
| `not` | `is` | `Name is not null` |
| `when` guard | switch arm | `4 when IsSpecial => ...` |
| Property | switch arm, `is` | `entity is { IsActive: true }` |
| Positional / deconstruct | switch arm, `is` | `(0, 0) => "origin"` |
| List (fixed-length) | `is` | `[1, 2, 3]` |
| List (slice) | `is` | `[1, .., 3]` |
| `var` | switch arm | `var x when x > 0 => ...` |

## `is` Patterns in Expression Bodies

### Relational `and` / `or`

```csharp
// Range check
[Expressive]
public bool IsInRange => Value is >= 1 and <= 100;
```

Generated expression:

```csharp
Value >= 1 && Value <= 100
```

```csharp
// Alternative values
[Expressive]
public bool IsEdge => Value is 0 or 100;
```

Generated expression:

```csharp
Value == 0 || Value == 100
```

### `not null` / `not`

```csharp
[Expressive]
public bool HasName => Name is not null;
```

Generated expression:

```csharp
!(Name == null)
```

### Property Patterns

```csharp
[Expressive]
public static bool IsActiveAndPositive(this Entity entity) =>
    entity is { IsActive: true, Value: > 0 };
```

Generated expression:

```csharp
entity != null && entity.IsActive == true && entity.Value > 0
```

Property patterns can be nested:

```csharp
[Expressive]
public bool HasValidCustomer =>
    Customer is { Name: not null, Address: { City: not null } };
```

### Positional / Deconstruct Patterns

Types that expose a `Deconstruct` method can use positional patterns:

```csharp
[Expressive]
public string Classify(Point p) => p switch
{
    (0, 0)     => "Origin",
    (> 0, > 0) => "Quadrant 1",
    _          => "Other",
};
```

### List Patterns

Fixed-length and slice patterns are supported on array and list types:

```csharp
[Expressive]
public bool StartsWithOne(int[] arr) => arr is [1, ..];

[Expressive]
public bool IsTriple(int[] arr) => arr is [_, _, _];
```

## Switch Expressions with Patterns

Switch expressions are the most common use of pattern matching in `[Expressive]` members. See [Switch Expressions](./switch-expressions) for a dedicated reference.

### Relational and Constant Patterns

```csharp
[Expressive]
public string GetGrade() => Score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    _     => "F",
};
```

Generated expression:

```csharp
Score >= 90 ? "A"
: Score >= 80 ? "B"
: Score >= 70 ? "C"
: "F"
```

EF Core translates this to:

```sql
SELECT CASE
    WHEN "e"."Score" >= 90 THEN 'A'
    WHEN "e"."Score" >= 80 THEN 'B'
    WHEN "e"."Score" >= 70 THEN 'C'
    ELSE 'F'
END
```

### `and` / `or` Combined Patterns

```csharp
[Expressive]
public string GetBand() => Score switch
{
    >= 90 and <= 100 => "Excellent",
    >= 70 and < 90   => "Good",
    _                => "Poor",
};
```

Generated expression:

```csharp
(Score >= 90 && Score <= 100) ? "Excellent"
: (Score >= 70 && Score < 90) ? "Good"
: "Poor"
```

### `when` Guards

```csharp
[Expressive]
public string Classify() => Value switch
{
    4 when IsSpecial => "Special Four",
    4               => "Regular Four",
    _               => "Other",
};
```

Generated expression:

```csharp
(Value == 4 && IsSpecial) ? "Special Four"
: Value == 4 ? "Regular Four"
: "Other"
```

### Type Patterns with Declaration Variables

Type patterns in switch arms produce type checks and casts:

```csharp
[Expressive]
public static string Describe(this Shape shape) => shape switch
{
    Circle c    => "Circle with radius " + c.Radius,
    Rectangle r => "Rectangle " + r.Width + "x" + r.Height,
    _           => "Unknown shape",
};
```

::: tip
Declaration variables (the `c` and `r` in the example above) are supported in switch arms. The generated expression uses type checks (`is`) and casts to bind the variable.
:::

### Nested Patterns

Patterns can be nested arbitrarily:

```csharp
[Expressive]
public string ClassifyOrder() => this switch
{
    { Customer: { Tier: CustomerTier.Premium }, Total: >= 100 } => "VIP Order",
    { Customer: not null, Total: >= 50 }                        => "Standard Order",
    _                                                            => "Basic Order",
};
```

## SQL Generation

All pattern-matching constructs compile down to nested `CASE` expressions in SQL. The generator produces a chain of conditional (ternary) expressions, which EF Core maps directly to SQL `CASE WHEN ... THEN ... ELSE ... END`.

For complex nested patterns, the SQL output may contain nested `CASE` expressions:

```sql
SELECT CASE
    WHEN "c"."Tier" = 2 AND ("o"."Price" * "o"."Quantity") >= 100.0
        THEN 'VIP Order'
    WHEN "c"."Id" IS NOT NULL AND ("o"."Price" * "o"."Quantity") >= 50.0
        THEN 'Standard Order'
    ELSE 'Basic Order'
END
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
```

::: warning
Keep patterns reasonably simple for SQL translation. Very deeply nested patterns produce complex SQL that may be harder to debug and could impact query performance.
:::
