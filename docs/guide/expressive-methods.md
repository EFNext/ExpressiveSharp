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

::: expressive-sample
db.Orders
    .Where(o => o.IsExpensive(500m))
    .Select(o => new { o.Id, Expensive = o.IsExpensive(1000m) })
---setup---
public static class OrderExt
{
    [Expressive]
    public static bool IsExpensive(this Order o, decimal threshold) =>
        o.Items.Sum(i => i.UnitPrice * i.Quantity) > threshold;
}
:::

The method argument (`500m` or `1000m`) is captured and translated into the generated expression for each provider.

## Methods with Multiple Parameters

::: expressive-sample
db.Products.Select(p => new
{
    p.Id,
    FinalPrice = p.CalculatePrice(0.05m, 10),
})
---setup---
public static class ProductExt
{
    [Expressive]
    public static decimal CalculatePrice(this Product p, decimal additionalDiscount, int quantity) =>
        p.ListPrice * (1 - additionalDiscount) * quantity;
}
:::

## Switch Expressions in Methods

Switch expressions and pattern matching work inside `[Expressive]` methods -- this is one of the key features that plain expression trees cannot do:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Grade = o.GetGrade() })
---setup---
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
:::

The query tabs above show how each provider translates the switch expression (typically as a CASE expression for SQL providers).

## Composing Methods and Properties

Expressive methods can call other expressive members and vice versa. The runtime expander resolves the entire chain:

::: expressive-sample
db.Orders.Where(o => o.ExceedsThreshold(500m)).Select(o => o.Id)
---setup---
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
:::

## Block-Bodied Methods

Methods can use traditional block bodies when `AllowBlockBody = true`:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Category = o.GetCategory() })
---setup---
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
:::

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

::: expressive-sample
db.LineItems.Select(i => new
{
    i.Id,
    Discounted = OrderHelpers.CalculateLinePrice(i.UnitPrice, i.Quantity),
})
---setup---
public static class OrderHelpers
{
    [Expressive]
    public static decimal CalculateLinePrice(decimal price, int quantity) =>
        price * quantity > 1000m ? price * quantity * 0.9m : price * quantity;
}
:::

## Important Rules

- Methods must be **expression-bodied** (`=>`) unless `AllowBlockBody = true`.
- If a method has no body, the generator reports diagnostic **EXP0001**.
- If a method uses a block body without opting in, the generator reports diagnostic **EXP0004**.
- Parameter types must be supported by your LINQ provider (primitive types, enums, and other provider-translatable types).

## Next Steps

- [[Expressive] Properties](./expressive-properties) -- computed properties on entities
- [Constructor Projections](./expressive-constructors) -- project DTOs directly in queries
- [ExpressionPolyfill.Create](./expression-polyfill) -- inline expression trees without attributes
