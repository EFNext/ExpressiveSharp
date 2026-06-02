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

::: expressive-sample
db.Orders.Select(o => o.GetCategory())
\---setup---
public static class OrderBlockExt
{
\[Expressive(AllowBlockBody = true)]
public static string GetCategory(this Order o)
{
var threshold = o.Items.Count() \* 10;
if (threshold > 100) return "Bulk";
return "Regular";
}
}
:::

```csharp
db
    .Orders
    .Select(o => o.GetCategory())

// Setup
public static class OrderBlockExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetCategory(this Order o)
    {
        var threshold = o.Items.Count() * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}
```

**Generated SQL:**

```sql
SELECT CASE
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId") * 10 > 100 THEN 'Bulk'
    ELSE 'Regular'
END
FROM "Orders" AS "o"
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

::: expressive-sample
db.Orders.Select(o => o.CustomerName())
\---setup---
public static class OrderTransformerExt
{
\[Expressive(Transformers = new\[] { typeof(ExpressiveSharp.Transformers.RemoveNullConditionalPatterns) })]
public static string? CustomerName(this Order o) => o.Customer?.Name;
}
:::

```csharp
db
    .Orders
    .Select(o => o.CustomerName())

// Setup
public static class OrderTransformerExt
{
    [Expressive(Transformers = new[] { typeof(ExpressiveSharp.Transformers.RemoveNullConditionalPatterns) })]
    public static string? CustomerName(this Order o) => o.Customer?.Name;
}
```

**Generated SQL:**

```sql
SELECT "c"."Name"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
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
Expression<Func<Order, decimal>> expr = o => o.Total();
// expr body is: o.Total() (opaque method call)

var expanded = expr.ExpandExpressives();
// expanded body is: o.Items.Sum(i => i.UnitPrice * i.Quantity) (translatable by your provider)
```

This replaces `[Expressive]` member references with their generated expression trees. Expansion is recursive -- if `TotalWithTax` references `Total`, both are expanded:

```csharp
[Expressive]
public static decimal Total(this Order o) => o.Items.Sum(i => i.UnitPrice * i.Quantity);

[Expressive]
public static decimal TotalWithTax(this Order o) => o.Total() * 1.08m;

Expression<Func<Order, decimal>> expr = o => o.TotalWithTax();
var expanded = expr.ExpandExpressives();
// expanded body is: o.Items.Sum(i => i.UnitPrice * i.Quantity) * 1.08m
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

## Opting Out: `[NotExpressive]`

Use `[NotExpressive]` to mark a member that *looks* expressive-eligible (it has an expression body that the source generator could lift) but should intentionally remain runtime-evaluated. The attribute suppresses the analyzer suggestions:

* [EXP0025](./diagnostics#exp0025) — "Member could benefit from `[Expressive]`"
* [EXP0028](./diagnostics#exp0028) — "Plain `IQueryable` chain references an `[Expressive]` member without `.AsExpressive()`"
* [EXP0029](./diagnostics#exp0029) — "`IExpressiveQueryable<T>` chain dropped to plain `IQueryable<T>`" (when applied to the method that drops the chain)

```csharp
public class Order
{
    public Guid Id { get; set; }

    // Always evaluated in-memory — captures process-local state that would not
    // survive translation. Suppress the "could be [Expressive]" suggestion.
    [NotExpressive]
    public string DebugLabel => $"{Id} (pid {System.Environment.ProcessId})";
}
```

`[NotExpressive]` cannot be combined with `[Expressive]` on the same member.

## Complete Example

::: expressive-sample
db.Orders
.Where(o => o.CustomerEmail() != null)
.Select(o => new OrderSummaryDto(o.Id, o.SafeTag(), o.Total()))
\---setup---
public static class OrderComplete
{
// Simple computed method
\[Expressive]
public static decimal Total(this Order o) => o.Items.Sum(i => i.UnitPrice \* i.Quantity);

```
// Composing expressives
[Expressive]
public static decimal TotalWithTax(this Order o) => o.Total() * 1.08m;

// Null-conditional operators -- always generates faithful ternary
[Expressive]
public static string? CustomerEmail(this Order o) => o.Customer?.Email;

// Switch expressions with pattern matching
[Expressive]
public static string GetGrade(this Order o) => o.Items.Count() switch
{
    >= 10 => "Premium",
    >= 5  => "Standard",
    _     => "Budget",
};

// Per-member transformer
[Expressive(Transformers = new[] { typeof(ExpressiveSharp.Transformers.RemoveNullConditionalPatterns) })]
public static string? CustomerNameSafe(this Order o) => o.Customer?.Name;

// Block body (opt-in)
[Expressive(AllowBlockBody = true)]
public static string GetCategory(this Order o)
{
    var threshold = o.Items.Count() * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}

// Extension method with null-coalescing
[Expressive]
public static string SafeTag(this Order o) => o.Customer != null ? o.Customer.Name : "N/A";
```

}

public class OrderSummaryDto
{
public int Id { get; set; }
public string Description { get; set; } = "";
public decimal Total { get; set; }

```
public OrderSummaryDto() { }

// Constructor projection -- translates to MemberInit
[Expressive]
public OrderSummaryDto(int id, string description, decimal total)
{
    Id = id;
    Description = description;
    Total = total;
}
```

}
:::

```csharp
db
    .Orders
    .Where(o => o.CustomerEmail() != null)
    .Select(o => new OrderSummaryDto(o.Id, o.SafeTag(), o.Total()))

// Setup
public static class OrderComplete
{
    // Simple computed method
    [Expressive]
    public static decimal Total(this Order o) => o.Items.Sum(i => i.UnitPrice * i.Quantity);

    // Composing expressives
    [Expressive]
    public static decimal TotalWithTax(this Order o) => o.Total() * 1.08m;

    // Null-conditional operators -- always generates faithful ternary
    [Expressive]
    public static string? CustomerEmail(this Order o) => o.Customer?.Email;

    // Switch expressions with pattern matching
    [Expressive]
    public static string GetGrade(this Order o) => o.Items.Count() switch
    {
        >= 10 => "Premium",
        >= 5  => "Standard",
        _     => "Budget",
    };

    // Per-member transformer
    [Expressive(Transformers = new[] { typeof(ExpressiveSharp.Transformers.RemoveNullConditionalPatterns) })]
    public static string? CustomerNameSafe(this Order o) => o.Customer?.Name;

    // Block body (opt-in)
    [Expressive(AllowBlockBody = true)]
    public static string GetCategory(this Order o)
    {
        var threshold = o.Items.Count() * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }

    // Extension method with null-coalescing
    [Expressive]
    public static string SafeTag(this Order o) => o.Customer != null ? o.Customer.Name : "N/A";
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Total { get; set; }

    public OrderSummaryDto() { }

    // Constructor projection -- translates to MemberInit
    [Expressive]
    public OrderSummaryDto(int id, string description, decimal total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", "c"."Name" AS "Description", (
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "Total"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE "c"."Email" IS NOT NULL
```
