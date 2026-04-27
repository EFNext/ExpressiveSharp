# [Expressive] Properties

Expressive properties let you define computed values on your entities using standard C# syntax, and have those computations automatically translated for your LINQ provider when used in queries.

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

Since the webshop entities in these samples have no built-in `[Expressive]` members, the examples below define helpers as extension methods in a `---setup---` block. The behavior is identical — `[Expressive]` works on instance properties, extension properties, and methods alike.

## Using Expressive Properties in Queries

Once defined, expressive properties can be used in **any part of a LINQ query**.

### In `Select`

```csharp
db
    .Orders
    .Select(o => new { o.Id, Total = o.Total() })

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Total(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity);
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "Total"
FROM "Orders" AS "o"
```


### In `Where`

```csharp
db
    .Orders
    .Where(o => o.Total() > 500m)
    .Select(o => o.Id)

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Total(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity);
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id"
FROM "Orders" AS "o"
WHERE ef_compare((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), '500.0') > 0
```


### In `GroupBy`

```csharp
db
    .Orders
    .GroupBy(o => o.CustomerEmail())
    .Select(g => new { Email = g.Key, Count = g.Count() })

// Setup
public static class OrderExt
{
    [Expressive]
    public static string? CustomerEmail(this Order o) => o.Customer.Email;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "c"."Email", COUNT(*) AS "Count"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
GROUP BY "c"."Email"
```


### In `OrderBy`

```csharp
db
    .Orders
    .OrderByDescending(o => o.Total())
    .Select(o => o.Id)

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Total(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity);
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id"
FROM "Orders" AS "o"
ORDER BY (
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") COLLATE EF_DECIMAL DESC
```


### In multiple clauses at once

```csharp
db
    .Orders
    .Where(o => o.Total() > 100m)
    .OrderByDescending(o => o.Total())
    .Select(o => new { o.Id, Total = o.Total(), Email = o.CustomerEmail() })

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Total(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static string? CustomerEmail(this Order o) => o.Customer.Email;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum(ef_multiply("l1"."UnitPrice", CAST("l1"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l1"
    WHERE "o"."Id" = "l1"."OrderId") AS "Total", "c"."Email"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE ef_compare((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), '100.0') > 0
ORDER BY (
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId") COLLATE EF_DECIMAL DESC
```


## Composing Expressive Properties

Expressive members can reference **other expressive members**. The entire chain is expanded transitively into the final query:

```csharp
db
    .Orders
    .Select(o => new { o.Id, Total = o.TotalWithTax(0.21m) })

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Subtotal(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static decimal Tax(this Order o, decimal taxRate) =>
        o.Subtotal() * taxRate;

    [Expressive]
    public static decimal TotalWithTax(this Order o, decimal taxRate) =>
        o.Subtotal() + o.Tax(taxRate);
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", ef_add((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId"), '0.21')) AS "Total"
FROM "Orders" AS "o"
```


When you query `TotalWithTax`, the runtime expander recursively resolves `Subtotal` and `Tax`, producing a fully flattened expression — the query tabs above show the translation for each provider. All computation happens in the database — no data is loaded into memory.

## Null-Conditional Properties

The null-conditional operator `?.` works naturally in expressive members:

```csharp
db
    .Orders
    .Select(o => new { o.Id, Email = o.CustomerEmail() })

// Setup
public static class OrderExt
{
    [Expressive]
    public static string? CustomerEmail(this Order o) => o.Customer?.Email;

    [Expressive]
    public static string? CustomerCountry(this Order o) => o.Customer?.Country;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", "c"."Email"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
```


The source generator emits a faithful null-check ternary expression. When used with EF Core and `UseExpressives()`, the `RemoveNullConditionalPatterns` transformer strips the null checks for SQL providers that handle null propagation natively.

## Block-Bodied Properties

By default, `[Expressive]` only supports expression-bodied members (`=>`). To use block bodies with `if`/`else`, local variables, and other statements, set `AllowBlockBody = true`:

```csharp
db
    .Orders
    .Select(o => new { o.Id, Category = o.Category() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static string Category(this Order o)
    {
        var totalQty = o.Items.Sum(i => i.Quantity);
        if (totalQty > 10) return "Bulk";
        return "Regular";
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", CASE
    WHEN (
        SELECT COALESCE(SUM("l"."Quantity"), 0)
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId") > 10 THEN 'Bulk'
    ELSE 'Regular'
END AS "Category"
FROM "Orders" AS "o"
```


Block bodies translate to CASE expressions — the query tabs above show how each provider renders the conditional.

::: warning
Block bodies are experimental. Not all constructs are supported -- `while`/`do-while`, `try`/`catch`, `async`/`await`, assignments, and `++`/`--` are not translatable. Use expression-bodied members for full compatibility.
:::

You can also enable block bodies globally for an entire project via MSBuild instead of opting in per-member:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

## Expanding Properties Manually

You can expand `[Expressive]` members manually in expression trees outside of your query provider:

```csharp
Expression<Func<Order, decimal>> expr = o => o.Total();
// expr body is: o.Total() (opaque method call)

var expanded = expr.ExpandExpressives();
// expanded body is: o.Items.Sum(i => i.UnitPrice * i.Quantity)
```

This is useful when you work with LINQ providers directly or need to inspect the expanded expression tree.

## Important Rules

- The member **must be expression-bodied** (using `=>`) unless `AllowBlockBody = true` is set.
- The expression must be translatable by your LINQ provider -- it can only use members that the provider understands (mapped columns, navigation properties, and other `[Expressive]` members).
- The body has access to `this` (the entity or extension receiver) and its navigation properties.
- If a member has no body, the generator reports diagnostic **EXP0001**.
- If a member uses a block body without opting in, the generator reports diagnostic **EXP0004**.

## Next Steps

- [[Expressive] Methods](./expressive-methods) -- parameterized query fragments
- [Constructor Projections](./expressive-constructors) -- project DTOs directly in queries
- [ExpressionPolyfill.Create](./expression-polyfill) -- inline expression trees without attributes
