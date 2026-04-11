---
url: 'https://efnext.github.io/ExpressiveSharp/guide/expressive-methods.md'
---
# \[Expressive] Methods

Expressive methods work like expressive properties but accept parameters, making them ideal for reusable query fragments that vary based on runtime values.

## Defining an Expressive Method

Add `[Expressive]` to any **expression-bodied method** on an entity:

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

## Using Expressive Methods in Queries

```csharp
// Pass runtime values as arguments
var expensive = ctx.Orders
    .Where(o => o.IsExpensive(100))
    .ToList();

// Use in Select
var summary = ctx.Orders
    .Select(o => new { o.Id, Expensive = o.IsExpensive(50) })
    .ToList();
```

The method argument (`100` or `50`) is captured and translated into the generated SQL expression.

## Methods with Multiple Parameters

```csharp
public class Product
{
    public double ListPrice { get; set; }
    public double DiscountRate { get; set; }

    [Expressive]
    public double CalculatePrice(double additionalDiscount, int quantity) =>
        ListPrice * (1 - DiscountRate - additionalDiscount) * quantity;
}

// Usage
var prices = ctx.Products
    .Select(p => new
    {
        p.Id,
        FinalPrice = p.CalculatePrice(0.05, 10)
    })
    .ToList();
```

## Switch Expressions in Methods

Switch expressions and pattern matching work inside `[Expressive]` methods -- this is one of the key features that plain expression trees cannot do:

```csharp
public class Order
{
    public double Price { get; set; }

    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };
}

var graded = ctx.Orders
    .Select(o => new { o.Id, Grade = o.GetGrade() })
    .ToList();
```

Generated SQL:

```sql
SELECT "o"."Id",
       CASE
           WHEN "o"."Price" >= 100.0 THEN 'Premium'
           WHEN "o"."Price" >= 50.0 THEN 'Standard'
           ELSE 'Budget'
       END AS "Grade"
FROM "Orders" AS "o"
```

## Composing Methods and Properties

Expressive methods can call expressive properties and vice versa. The runtime expander resolves the entire chain:

```csharp
public class Order
{
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double TaxRate { get; set; }

    [Expressive]
    public double Subtotal => Price * Quantity;

    [Expressive]
    public double Tax => Subtotal * TaxRate;

    // Method calling expressive properties
    [Expressive]
    public bool ExceedsThreshold(double threshold) =>
        (Subtotal + Tax) > threshold;
}

var highValue = ctx.Orders
    .Where(o => o.ExceedsThreshold(500))
    .ToList();
```

## Block-Bodied Methods

Methods can use traditional block bodies when `AllowBlockBody = true`:

```csharp
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    var threshold = Quantity * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}
```

Block bodies support:

* Local variable declarations (inlined at each usage point)
* `if`/`else` chains (converted to ternary / CASE expressions)
* `switch` statements
* `foreach` loops (converted to LINQ method calls)
* `for` loops (array/list iteration)

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

`[Expressive]` can be applied to static methods as well:

```csharp
public static class OrderHelpers
{
    [Expressive]
    public static double CalculateDiscount(double price, int quantity) =>
        price * quantity > 1000 ? 0.1 : 0.0;
}
```

## Important Rules

* Methods must be **expression-bodied** (`=>`) unless `AllowBlockBody = true`.
* If a method has no body, the generator reports diagnostic **EXP0001**.
* If a method uses a block body without opting in, the generator reports diagnostic **EXP0004**.
* Parameter types must be supported by your LINQ provider (primitive types, enums, and other provider-translatable types).

## Next Steps

* [\[Expressive\] Properties](./expressive-properties) -- computed properties on entities
* [Constructor Projections](./expressive-constructors) -- project DTOs directly in queries
* [ExpressionPolyfill.Create](./expression-polyfill) -- inline expression trees without attributes
