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

```csharp
db
    .Products
    .Where(p => p.IsReasonablyPriced())
    .Select(p => p.Name)

// Setup
public static class ProductExt
{
    [Expressive]
    public static bool IsReasonablyPriced(this Product p) => p.ListPrice is >= 1m and <= 100m;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name"
FROM "Products" AS "p"
WHERE ef_compare("p"."ListPrice", '1.0') >= 0 AND ef_compare("p"."ListPrice", '100.0') <= 0
```


Alternative values with `or`:

```csharp
db
    .Orders
    .Where(o => o.IsBoundary())
    .Select(o => o.Id)

// Setup
public static class OrderExt
{
    [Expressive]
    public static bool IsBoundary(this Order o) => o.Items.Count is 0 or 100;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id"
FROM "Orders" AS "o"
WHERE (
    SELECT COUNT(*)
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") = 0 OR (
    SELECT COUNT(*)
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId") = 100
```


### `not null` / `not`

```csharp
db
    .Customers
    .Where(c => c.HasEmail())
    .Select(c => c.Name)

// Setup
public static class CustomerExt
{
    [Expressive]
    public static bool HasEmail(this Customer c) => c.Email is not null;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "c"."Name"
FROM "Customers" AS "c"
WHERE "c"."Email" IS NOT NULL
```


### Property Patterns

```csharp
db
    .Orders
    .Where(o => o.IsLargePaid())
    .Select(o => o.Id)

// Setup
public static class OrderExt
{
    [Expressive]
    public static bool IsLargePaid(this Order o) =>
        o is { Status: ExpressiveSharp.Docs.PlaygroundModel.Webshop.OrderStatus.Paid, Items.Count: > 5 };
}
```

**Generated SQL (SQLite):**

```sql
.param set @Paid 1

SELECT "o"."Id"
FROM "Orders" AS "o"
WHERE "o"."Status" = @Paid AND (
    SELECT COUNT(*)
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") > 5
```


Property patterns can be nested:

```csharp
db
    .Orders
    .Where(o => o.HasNamedCustomer())
    .Select(o => o.Id)

// Setup
public static class OrderExt
{
    [Expressive]
    public static bool HasNamedCustomer(this Order o) =>
        o is { Customer: { Name: not null, Country: not null } };
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE "c"."Country" IS NOT NULL
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
db
    .Products
    .Select(p => new { p.Name, Grade = p.GetGrade() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN ef_compare("p"."ListPrice", '500.0') >= 0 THEN 'A'
    WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'B'
    WHEN ef_compare("p"."ListPrice", '20.0') >= 0 THEN 'C'
    ELSE 'F'
END AS "Grade"
FROM "Products" AS "p"
```


The tabs above show how each provider renders the generated ternary chain.

### `and` / `or` Combined Patterns

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
        _                => "Poor",
    };
}
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN "p"."StockQuantity" >= 90 AND "p"."StockQuantity" <= 100 THEN 'Excellent'
    WHEN "p"."StockQuantity" >= 70 AND "p"."StockQuantity" < 90 THEN 'Good'
    ELSE 'Poor'
END AS "Band"
FROM "Products" AS "p"
```


### `when` Guards

```csharp
db
    .Products
    .Select(p => new { p.Name, Class = p.Classify() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Name", CASE
    WHEN "p"."StockQuantity" = 4 AND "p"."Category" = 'Special' THEN 'Special Four'
    WHEN "p"."StockQuantity" = 4 THEN 'Regular Four'
    ELSE 'Other'
END AS "Class"
FROM "Products" AS "p"
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
db
    .Orders
    .Select(o => new { o.Id, Tag = o.ClassifyOrder() })

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", "c"."Country" = 'US' AND "c"."Country" IS NOT NULL, "c"."Id", "l"."Id", "l"."OrderId", "l"."ProductId", "l"."Quantity", "l"."UnitPrice", CASE
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId") >= 10 THEN 1
    ELSE 0
END, 1, "l1"."Id", "l1"."OrderId", "l1"."ProductId", "l1"."Quantity", "l1"."UnitPrice", CASE
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l2"
        WHERE "o"."Id" = "l2"."OrderId") >= 5 THEN 1
    ELSE 0
END
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
LEFT JOIN "LineItems" AS "l" ON "o"."Id" = "l"."OrderId"
LEFT JOIN "LineItems" AS "l1" ON "o"."Id" = "l1"."OrderId"
ORDER BY "o"."Id", "c"."Id", "l"."Id"
```


## Translation Output

All pattern-matching constructs compile down to nested conditional expressions. The generator produces a chain of ternaries, which each provider maps to its own conditional syntax (SQL `CASE WHEN ... THEN ... ELSE ... END`, MongoDB `$switch`/`$cond`, etc.). The tabs on the samples above let you inspect the exact translation for your target.

::: warning
Keep patterns reasonably simple for translation. Very deeply nested patterns produce complex output that may be harder to debug and could impact query performance.
:::
