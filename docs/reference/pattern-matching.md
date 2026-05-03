# Pattern Matching

The ExpressiveSharp source generator rewrites C# pattern-matching constructs into expression-tree-compatible ternary and binary expressions. LINQ providers translate these into their native conditional syntax (SQL `CASE`, MongoDB `$cond`/`$switch`, etc.).

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

A range check using an `[Expressive]` helper. The tabs show how each provider translates it.

::: expressive-sample
db.Products.Where(p => p.IsReasonablyPriced()).Select(p => p.Name)
---setup---
public static class ProductExt
{
    [Expressive]
    public static bool IsReasonablyPriced(this Product p) => p.ListPrice is >= 1m and <= 100m;
}
:::

Alternative values with `or`:

::: expressive-sample
db.Orders.Where(o => o.IsBoundary()).Select(o => o.Id)
---setup---
public static class OrderExt
{
    [Expressive]
    public static bool IsBoundary(this Order o) => o.Items.Count is 0 or 100;
}
:::

### `not null` / `not`

::: expressive-sample
db.Customers.Where(c => c.HasEmail()).Select(c => c.Name)
---setup---
public static class CustomerExt
{
    [Expressive]
    public static bool HasEmail(this Customer c) => c.Email is not null;
}
:::

### Property Patterns

::: expressive-sample
db.Orders.Where(o => o.IsLargePaid()).Select(o => o.Id)
---setup---
public static class OrderExt
{
    [Expressive]
    public static bool IsLargePaid(this Order o) =>
        o is { Status: ExpressiveSharp.Docs.PlaygroundModel.Webshop.OrderStatus.Paid, Items.Count: > 5 };
}
:::

Property patterns can be nested:

::: expressive-sample
db.Orders.Where(o => o.HasNamedCustomer()).Select(o => o.Id)
---setup---
public static class OrderExt
{
    [Expressive]
    public static bool HasNamedCustomer(this Order o) =>
        o is { Customer: { Name: not null, Country: not null } };
}
:::

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

::: expressive-sample
db.Products.Select(p => new { p.Name, Grade = p.GetGrade() })
---setup---
public static class ProductExt
{
    [Expressive]
    public static string GetGrade(this Product p) => p.ListPrice switch
    {
        >= 500m => "A",
        >= 100m => "B",
        >= 20m  => "C",
        _       => "F",
    };
}
:::

The tabs above show how each provider renders the generated ternary chain.

### `and` / `or` Combined Patterns

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
        _                => "Poor",
    };
}
:::

### `when` Guards

::: expressive-sample
db.Products.Select(p => new { p.Name, Class = p.Classify() })
---setup---
public static class ProductExt
{
    [Expressive]
    public static string Classify(this Product p) => p.StockQuantity switch
    {
        4 when p.Category == "Special" => "Special Four",
        4                              => "Regular Four",
        _                              => "Other",
    };
}
:::

### Type Patterns with Declaration Variables

Type patterns in switch arms produce type checks and casts:

```csharp
[Expressive]
public static string Describe(this Shape shape) => shape switch
{
    Circle c    => $"Circle with radius {c.Radius}",
    Rectangle r => $"Rectangle {r.Width}x{r.Height}",
    _           => "Unknown shape",
};
```

::: tip
Declaration variables (the `c` and `r` in the example above) are supported in switch arms. The generated expression uses type checks (`is`) and casts to bind the variable.
:::

### Nested Patterns

Patterns can be nested arbitrarily:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Tag = o.ClassifyOrder() })
---setup---
public static class OrderExt
{
    [Expressive]
    public static string ClassifyOrder(this Order o) => o switch
    {
        { Customer.Country: "US", Items.Count: >= 10 } => "VIP Order",
        { Customer: not null, Items.Count: >= 5 }      => "Standard Order",
        _                                              => "Basic Order",
    };
}
:::

## Translation Output

All pattern-matching constructs compile down to nested conditional expressions. The generator produces a chain of ternaries, which each provider maps to its own conditional syntax (SQL `CASE WHEN ... THEN ... ELSE ... END`, MongoDB `$switch`/`$cond`, etc.). The tabs on the samples above let you inspect the exact translation for your target.

::: warning
Keep patterns reasonably simple for translation. Very deeply nested patterns produce complex output that may be harder to debug and could impact query performance.
:::
