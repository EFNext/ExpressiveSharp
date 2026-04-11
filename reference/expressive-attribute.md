---
url: 'https://efnext.github.io/ExpressiveSharp/reference/expressive-attribute.md'
---
# `[Expressive]` Attribute

The `ExpressiveAttribute` is the primary entry point for ExpressiveSharp. Place it on any property, method, extension method, or constructor to tell the source generator to produce a companion expression tree at compile time.

## Namespace

```csharp
using ExpressiveSharp;
```

## Targets

| Target            | Supported |
|-------------------|-----------|
| Properties        | Yes       |
| Methods           | Yes       |
| Extension methods | Yes       |
| Constructors      | Yes       |
| Indexers          | No        |

The attribute can be inherited by derived types (`Inherited = true`).

## Properties

### `AllowBlockBody`

**Type:** `bool`
**Default:** `false`

Enables block-bodied member support. Without this flag, using a block body (`{ }`) with `[Expressive]` produces error [EXP0004](./diagnostics#exp0004). Setting this to `true` allows block bodies that support local variables, `if`/`else`, `switch` statements, and `foreach` loops.

When not explicitly set on the attribute, the MSBuild property `Expressive_AllowBlockBody` is used as the global default (also defaults to `false`).

```csharp
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    var threshold = Quantity * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}
```

Or enable globally for the entire project:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

***

### `Transformers`

**Type:** `Type[]?`
**Default:** `null`

Specifies additional `IExpressionTreeTransformer` types to apply at runtime when the expression is resolved. Each type must have a parameterless constructor.

```csharp
[Expressive(Transformers = new[] { typeof(RemoveNullConditionalPatterns) })]
public string? CustomerName => Customer?.Name;
```

See [Expression Transformers](./expression-transformers) for the full list of built-in transformers and how to create custom ones.

## How It Works

When the source generator encounters an `[Expressive]` member, it:

1. Analyzes the member body at the IOperation (semantic) level
2. Generates `Expression<Func<...>>` factory code using `Expression.*` calls
3. Registers the generated expression in a per-assembly expression registry

At runtime, `ExpandExpressives()` (or `UseExpressives()` in EF Core) looks up the registered expression and replaces opaque member accesses with the generated expression tree, so LINQ providers can translate them.

::: info No NullConditionalRewriteSupport enum
Unlike Projectables, which required a per-member `NullConditionalRewriteSupport` enum to configure `?.` handling, ExpressiveSharp always generates a faithful ternary (`x != null ? x.Prop : default`). If you need to strip the null checks for SQL providers, the `RemoveNullConditionalPatterns` transformer handles it globally. `UseExpressives()` applies this transformer automatically. See [Null-Conditional Rewrite](./null-conditional-rewrite) for details.
:::

::: info No ExpandEnumMethods property
ExpressiveSharp always expands enum extension methods into per-value ternary chains automatically. There is no opt-in flag needed.
:::

::: info No CompatibilityMode
ExpressiveSharp does not have a compatibility mode setting. Expression expansion always uses the full approach, which handles all scenarios correctly.
:::

## Using `ExpandExpressives()`

After marking members with `[Expressive]`, you can manually expand them in expression trees using the `.ExpandExpressives()` extension method:

```csharp
Expression<Func<Order, double>> expr = o => o.Total;
// expr body is: o.Total (opaque property access)

var expanded = expr.ExpandExpressives();
// expanded body is: o.Price * o.Quantity (translatable by EF Core / other providers)
```

This replaces `[Expressive]` member references with their generated expression trees. Expansion is recursive -- if `TotalWithTax` references `Total`, both are expanded:

```csharp
[Expressive]
public double Total => Price * Quantity;

[Expressive]
public double TotalWithTax => Total * (1 + TaxRate);

Expression<Func<Order, double>> expr = o => o.TotalWithTax;
var expanded = expr.ExpandExpressives();
// expanded body is: (o.Price * o.Quantity) * (1 + o.TaxRate)
```

You can also pass transformers to `ExpandExpressives()`:

```csharp
expr.ExpandExpressives(new RemoveNullConditionalPatterns());
```

Or register transformers globally so all calls use them:

```csharp
ExpressiveOptions.Default.AddTransformers(new RemoveNullConditionalPatterns());
expr.ExpandExpressives(); // RemoveNullConditionalPatterns applied automatically
```

## Complete Example

```csharp
public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Tag { get; set; }
    public Customer? Customer { get; set; }

    // Simple computed property
    [Expressive]
    public double Total => Price * Quantity;

    // Composing expressives
    [Expressive]
    public double TotalWithTax => Total * (1 + 0.08);

    // Null-conditional operators -- always generates faithful ternary
    [Expressive]
    public string? CustomerEmail => Customer?.Email;

    // Switch expressions with pattern matching
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50  => "Standard",
        _      => "Budget",
    };

    // Per-member transformer
    [Expressive(Transformers = new[] { typeof(RemoveNullConditionalPatterns) })]
    public string? CustomerName => Customer?.Name;

    // Block body (opt-in)
    [Expressive(AllowBlockBody = true)]
    public string GetCategory()
    {
        var threshold = Quantity * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}

// Extension methods must be in a static class
public static class OrderExtensions
{
    [Expressive]
    public static string? SafeTag(this Order o) => o.Tag ?? "N/A";
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderSummaryDto() { }

    // Constructor projection -- translates to SQL MemberInit
    [Expressive]
    public OrderSummaryDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
```

Usage in an EF Core query:

```csharp
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total))
    .ToList();
```

Generated SQL:

```sql
SELECT "o"."Id",
       COALESCE("o"."Tag", 'N/A') AS "Description",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE "c"."Email" IS NOT NULL
```
