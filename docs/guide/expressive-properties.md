# [Expressive] Properties

Expressive properties let you define computed values on your entities using standard C# syntax, and have those computations automatically translated into SQL when used in LINQ queries.

## Defining an Expressive Property

Add `[Expressive]` to any **expression-bodied property**:

```csharp
using ExpressiveSharp;

public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Customer? Customer { get; set; }

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string? CustomerEmail => Customer?.Email;
}
```

The source generator emits a companion `Expression<Func<Order, double>>` for `Total` and `Expression<Func<Order, string?>>` for `CustomerEmail` at compile time. When the property is used in a LINQ query, the expression tree is substituted automatically.

## Using Expressive Properties in Queries

Once defined, expressive properties can be used in **any part of a LINQ query**.

### In `Select`

```csharp
var totals = ctx.Orders
    .Select(o => new { o.Id, o.Total })
    .ToList();
```

Generated SQL:
```sql
SELECT "o"."Id",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total"
FROM "Orders" AS "o"
```

### In `Where`

```csharp
var expensive = ctx.Orders
    .Where(o => o.Total > 500)
    .ToList();
```

### In `GroupBy`

```csharp
var grouped = ctx.Orders
    .GroupBy(o => o.CustomerEmail)
    .Select(g => new { Email = g.Key, Count = g.Count() })
    .ToList();
```

### In `OrderBy`

```csharp
var sorted = ctx.Orders
    .OrderByDescending(o => o.Total)
    .ToList();
```

### In multiple clauses at once

```csharp
var query = ctx.Orders
    .Where(o => o.Total > 100)
    .OrderByDescending(o => o.Total)
    .Select(o => new { o.Id, o.Total, o.CustomerEmail });
```

## Composing Expressive Properties

Expressive properties can reference **other expressive properties**. The entire chain is expanded transitively into the final SQL:

```csharp
public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double TaxRate { get; set; }

    [Expressive]
    public double Subtotal => Price * Quantity;

    [Expressive]
    public double Tax => Subtotal * TaxRate;        // references Subtotal

    [Expressive]
    public double Total => Subtotal + Tax;           // references Subtotal and Tax

    [Expressive]
    public double TotalWithTax => Total * (1 + TaxRate);  // references Total
}
```

When you query `Total`, the runtime expander recursively resolves `Subtotal` and `Tax`, producing a fully flattened SQL expression:

```csharp
var result = ctx.Orders
    .Select(o => new { o.Id, o.Total })
    .ToList();
```

```sql
SELECT "o"."Id",
       ("o"."Price" * CAST("o"."Quantity" AS REAL)) +
       (("o"."Price" * CAST("o"."Quantity" AS REAL)) * "o"."TaxRate") AS "Total"
FROM "Orders" AS "o"
```

All computation happens in the database -- no data is loaded into memory.

## Null-Conditional Properties

The null-conditional operator `?.` works naturally in expressive properties:

```csharp
public class Order
{
    public Customer? Customer { get; set; }

    [Expressive]
    public string? CustomerEmail => Customer?.Email;

    [Expressive]
    public string? CustomerCity => Customer?.Address?.City;
}
```

The source generator emits a faithful null-check ternary expression. When used with EF Core and `UseExpressives()`, the `RemoveNullConditionalPatterns` transformer strips the null checks for SQL providers that handle null propagation natively.

## Block-Bodied Properties

By default, `[Expressive]` only supports expression-bodied properties (`=>`). To use block bodies with `if`/`else`, local variables, and other statements, set `AllowBlockBody = true`:

```csharp
[Expressive(AllowBlockBody = true)]
public string Category
{
    get
    {
        var threshold = Quantity * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}
```

EF Core translates block bodies to SQL CASE expressions:

```sql
SELECT CASE
    WHEN ("o"."Quantity" * 10) > 100 THEN 'Bulk'
    ELSE 'Regular'
END AS "Category"
FROM "Orders" AS "o"
```

::: warning
Block bodies are experimental. Not all constructs are supported -- `while`/`do-while`, `try`/`catch`, `async`/`await`, assignments, and `++`/`--` are not translatable. Use expression-bodied properties for full compatibility.
:::

You can also enable block bodies globally for an entire project via MSBuild instead of opting in per-member:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

## Expanding Properties Manually

You can expand `[Expressive]` properties manually in expression trees outside of EF Core:

```csharp
Expression<Func<Order, double>> expr = o => o.Total;
// expr body is: o.Total (opaque property access)

var expanded = expr.ExpandExpressives();
// expanded body is: o.Price * o.Quantity (translatable by any LINQ provider)
```

This is useful when you work with LINQ providers other than EF Core or need to inspect the expanded expression tree.

## Important Rules

- The property **must be expression-bodied** (using `=>`) unless `AllowBlockBody = true` is set.
- The expression must be translatable by your LINQ provider -- it can only use members that the provider understands (mapped columns, navigation properties, and other `[Expressive]` members).
- The property body has access to `this` (the entity instance) and its navigation properties.
- If a property has no body, the generator reports diagnostic **EXP0001**.
- If a property uses a block body without opting in, the generator reports diagnostic **EXP0004**.

## Next Steps

- [[Expressive] Methods](./expressive-methods) -- parameterized query fragments
- [Constructor Projections](./expressive-constructors) -- project DTOs directly in queries
- [ExpressionPolyfill.Create](./expression-polyfill) -- inline expression trees without attributes
