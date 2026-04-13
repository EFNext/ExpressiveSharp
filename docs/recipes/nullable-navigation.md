# Nullable Navigation Properties

This recipe covers how to work with optional (nullable) navigation properties in `[Expressive]` members and in LINQ chains, using null-conditional operators (`?.`) that are normally forbidden in expression trees.

## The Challenge

Navigation properties are often nullable -- either because the relationship is optional, or because the related entity is not loaded. Expression trees cannot represent the `?.` operator directly:

```
error CS8072: An expression tree lambda may not contain a null propagating operator
```

ExpressiveSharp eliminates this restriction entirely.

## How ExpressiveSharp Handles Null-Conditional Operators

ExpressiveSharp always generates a faithful ternary pattern for `?.`:

```
A?.B  ->  A != null ? A.B : default
```

When using EF Core with `UseExpressives()`, the `RemoveNullConditionalPatterns` transformer strips this ternary before the query reaches the database. SQL handles null propagation natively (a `LEFT JOIN` produces `NULL` for missing relationships), so the explicit check is unnecessary.

::: info No configuration needed
Unlike some other libraries, ExpressiveSharp does not expose a `NullConditionalRewriteSupport` enum or per-member null-handling options. `UseExpressives()` applies the `RemoveNullConditionalPatterns` transformer globally. This is the correct behavior for all major SQL providers (SQL Server, PostgreSQL, SQLite, MySQL, Oracle).
:::

## Single-Level Example

::: expressive-sample
db.Orders.Select(o => new { o.Id, CustomerEmail = o.CustomerEmail() })
---setup---
public static class OrderNullNavExt
{
    [Expressive]
    public static string? CustomerEmail(this Order o) => o.Customer?.Email;
}
:::

The `?.` operator is removed by the transformer (for SQL providers), and EF Core produces a clean `LEFT JOIN`. If `Customer` is `NULL`, the result is `NULL` for `Email` -- exactly matching the C# semantics.

## Multi-Level Chain

Deeply nested nullable navigation chains work the same way. For the webshop model, the chain `Order -> Customer -> Country` traverses one nullable level:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Country = o.CustomerCountry() })
---setup---
public static class OrderMultiNav
{
    [Expressive]
    public static string? CustomerCountry(this Order o) => o.Customer?.Country;
}
:::

Each `?.` in the chain produces a `LEFT JOIN`. The transformer strips all the ternaries, and the database handles null propagation naturally.

## Using with IExpressiveQueryable (Modern Syntax)

You do not need an `[Expressive]` member to use `?.` in queries. With `IExpressiveQueryable<T>` or `ExpressiveDbSet<T>`, you can write null-conditional operators directly in your LINQ lambdas:

::: expressive-sample
db.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        Name = o.Customer?.Name ?? "Unknown",
        Country = o.Customer?.Country
    })
:::

See [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) for more examples.

## Null-Conditional with Null-Coalescing

Combine `?.` with `??` for default values:

::: expressive-sample
db.Orders.Select(o => new
{
    CustomerName = o.CustomerName(),
    ShippingCountry = o.ShippingCountry()
})
---setup---
public static class OrderNullCoalesce
{
    [Expressive]
    public static string CustomerName(this Order o) => o.Customer?.Name ?? "Guest";

    [Expressive]
    public static string ShippingCountry(this Order o) => o.Customer?.Country ?? "Unknown";
}
:::

## Without EF Core: Applying the Transformer Manually

If you are not using EF Core (and therefore not using `UseExpressives()`), you can apply the transformer per-member or globally:

### Per-member

::: expressive-sample
db.Orders.Select(o => o.CustomerNameSafe())
---setup---
public static class OrderPerMemberTransformer
{
    [Expressive(Transformers = new[] { typeof(ExpressiveSharp.Transformers.RemoveNullConditionalPatterns) })]
    public static string? CustomerNameSafe(this Order o) => o.Customer?.Name;
}
:::

### Globally

```csharp
ExpressiveOptions.Default.AddTransformers(new RemoveNullConditionalPatterns());

// All subsequent ExpandExpressives() calls strip null-conditional patterns
Expression<Func<Order, string?>> expr = o => o.CustomerNameSafe();
var expanded = expr.ExpandExpressives();
```

### With ExpressionPolyfill.Create

```csharp
var expr = ExpressionPolyfill.Create(
    (Order o) => o.Customer?.Email,
    new RemoveNullConditionalPatterns());
```

## Tips

::: tip UseExpressives() handles everything
If you are using EF Core with `UseExpressives()`, null-conditional handling is fully automatic. No per-member configuration needed.
:::

::: warning Non-SQL providers
If your LINQ provider does not handle null propagation natively (for example, an in-memory provider used in tests), you may want to **not** apply `RemoveNullConditionalPatterns`. The faithful ternary pattern will evaluate correctly in those environments.
:::

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- building blocks that can include nullable navigation
- [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) -- `?.` directly in Where/Select
- [Reusable Query Filters](/recipes/reusable-query-filters) -- filters that guard against null navigation
