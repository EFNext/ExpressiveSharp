# [Expressive] Methods

Expressive methods work like expressive properties but accept parameters, making them ideal for reusable query fragments that vary based on runtime values.

## Defining an Expressive Method

Add `[Expressive]` to any **expression-bodied method**:

```csharp
using ExpressiveSharp;

public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedDate { get; set; }

    [Expressive]
    public bool IsExpensive(double threshold) => Price > threshold;
}
```

The source generator emits a companion `Expression<Func<Order, double, bool>>` at compile time. When the method is called in a LINQ query, the expression tree is substituted automatically.

The webshop entities in these samples don't have built-in `[Expressive]` methods, so the examples below define them as extension methods in `---setup---` blocks. The behavior is identical to instance methods.

## Using Expressive Methods in Queries

```csharp
db
    .Orders
    .Where(o => o.IsExpensive(500m))
    .Select(o => new { o.Id, Expensive = o.IsExpensive(1000m) })

// Setup
public static class OrderExt
{
    [Expressive]
    public static bool IsExpensive(this Order o, decimal threshold) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity) > threshold;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", CASE
    WHEN ef_compare((
        SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId"), '1000.0') > 0 THEN 1
    ELSE 0
END AS "Expensive"
FROM "Orders" AS "o"
WHERE ef_compare((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), '500.0') > 0
```


The method argument (`500m` or `1000m`) is captured and translated into the generated expression for each provider.

## Methods with Multiple Parameters

```csharp
db
    .Products
    .Select(p => new
    {
        p.Id,
        FinalPrice = p.CalculatePrice(0.05m, 10),
    })

// Setup
public static class ProductExt
{
    [Expressive]
    public static decimal CalculatePrice(this Product p, decimal additionalDiscount, int quantity) =>
        p.ListPrice * (1 - additionalDiscount) * quantity;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Id", ef_multiply(ef_multiply("p"."ListPrice", '0.95'), '10.0') AS "FinalPrice"
FROM "Products" AS "p"
```


## Switch Expressions in Methods

Switch expressions and pattern matching work inside `[Expressive]` methods -- this is one of the key features that plain expression trees cannot do:

```csharp
db
    .Orders
    .Select(o => new { o.Id, Grade = o.GetGrade() })

// Setup
public static class OrderExt
{
    [Expressive]
    public static string GetGrade(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity) switch
        {
            >= 100m => "Premium",
            >= 50m  => "Standard",
            _       => "Budget",
        };
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", CASE
    WHEN ef_compare((
        SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId"), '100.0') >= 0 THEN 'Premium'
    WHEN ef_compare((
        SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId"), '50.0') >= 0 THEN 'Standard'
    ELSE 'Budget'
END AS "Grade"
FROM "Orders" AS "o"
```


The query tabs above show how each provider translates the switch expression (typically as a CASE expression for SQL providers).

## Composing Methods and Properties

Expressive methods can call other expressive members and vice versa. The runtime expander resolves the entire chain:

```csharp
db
    .Orders
    .Where(o => o.ExceedsThreshold(500m))
    .Select(o => o.Id)

// Setup
public static class OrderExt
{
    [Expressive]
    public static decimal Subtotal(this Order o) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity);

    [Expressive]
    public static decimal Tax(this Order o, decimal rate) =>
        o.Subtotal() * rate;

    [Expressive]
    public static bool ExceedsThreshold(this Order o, decimal threshold) =>
        (o.Subtotal() + o.Tax(0.21m)) > threshold;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id"
FROM "Orders" AS "o"
WHERE ef_compare(ef_add((
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId"), ef_multiply((
    SELECT COALESCE(ef_sum(ef_multiply("l0"."UnitPrice", CAST("l0"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l0"
    WHERE "o"."Id" = "l0"."OrderId"), '0.21')), '500.0') > 0
```


## Block-Bodied Methods

Methods can use traditional block bodies when `AllowBlockBody = true`:

```csharp
db
    .Orders
    .Select(o => new { o.Id, Category = o.GetCategory() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetCategory(this Order o)
    {
        var totalQty = o.Items.Sum(i => i.Quantity);
        if (totalQty > 10) return "Bulk";
        return "Regular";
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", 'Regular' AS "Category"
FROM "Orders" AS "o"
```


Block bodies support:
- Local variable declarations (inlined at each usage point)
- `if`/`else` chains (converted to ternary / CASE expressions)
- `switch` statements
- `foreach` loops (converted to LINQ method calls)
- `for` loops (array/list iteration)

::: warning
Not all constructs are supported in block bodies. Unsupported statements (`while`/`do-while`, `try`/`catch`, `async`/`await`) trigger diagnostic **EXP0006**. Side-effect constructs (assignments, `++`/`--`) trigger diagnostic **EXP0005**.
:::

You can also enable block bodies globally for a project:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

## Static Methods

`[Expressive]` can be applied to static methods as well. Here, `CalculateLinePrice` is a pure static helper with no receiver:

```csharp
db
    .LineItems
    .Select(i => new
    {
        i.Id,
        Discounted = OrderHelpers.CalculateLinePrice(i.UnitPrice, i.Quantity),
    })

// Setup
public static class OrderHelpers
{
    [Expressive]
    public static decimal CalculateLinePrice(decimal price, int quantity) =>
        price * quantity > 1000m ? price * quantity * 0.9m : price * quantity;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "l"."Id", CASE
    WHEN ef_compare(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT)), '1000.0') > 0 THEN ef_multiply(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT)), '0.9')
    ELSE ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))
END AS "Discounted"
FROM "LineItems" AS "l"
```


## Important Rules

- Methods must be **expression-bodied** (`=>`) unless `AllowBlockBody = true`.
- If a method has no body, the generator reports diagnostic **EXP0001**.
- If a method uses a block body without opting in, the generator reports diagnostic **EXP0004**.
- Parameter types must be supported by your LINQ provider (primitive types, enums, and other provider-translatable types).

## Next Steps

- [[Expressive] Properties](./expressive-properties) -- computed properties on entities
- [Constructor Projections](./expressive-constructors) -- project DTOs directly in queries
- [ExpressionPolyfill.Create](./expression-polyfill) -- inline expression trees without attributes
