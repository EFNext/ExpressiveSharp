---
url: 'https://efnext.github.io/ExpressiveSharp/recipes/nullable-navigation.md'
---
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

```csharp
public class Order
{
    public int Id { get; set; }
    public Customer? Customer { get; set; }

    [Expressive]
    public string? CustomerEmail => Customer?.Email;
}
```

```csharp
var orders = dbContext.Orders
    .Select(o => new { o.Id, o.CustomerEmail })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       "c"."Email" AS "CustomerEmail"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
```

The `?.` operator is removed by the transformer, and EF Core produces a clean `LEFT JOIN`. If `Customer` is `NULL`, the SQL returns `NULL` for `Email` -- exactly matching the C# semantics.

## Multi-Level Chain

Deeply nested nullable navigation chains work the same way:

```csharp
public class User
{
    public int Id { get; set; }
    public Address? Address { get; set; }
}

public class Address
{
    public int Id { get; set; }
    public City? City { get; set; }
}

public class City
{
    public int Id { get; set; }
    public string? PostalCode { get; set; }
}
```

```csharp
public class User
{
    // ...

    [Expressive]
    public string? PostalCode => Address?.City?.PostalCode;
}
```

```csharp
var results = dbContext.Users
    .Select(u => new { u.Id, u.PostalCode })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "u"."Id",
       "c"."PostalCode"
FROM "Users" AS "u"
LEFT JOIN "Addresses" AS "a" ON "u"."AddressId" = "a"."Id"
LEFT JOIN "Cities" AS "c" ON "a"."CityId" = "c"."Id"
```

Each `?.` in the chain produces a `LEFT JOIN`. The transformer strips all the ternaries, and the database handles null propagation naturally.

## Using with IExpressiveQueryable (Modern Syntax)

You do not need an `[Expressive]` property to use `?.` in queries. With `IExpressiveQueryable<T>` or `ExpressiveDbSet<T>`, you can write null-conditional operators directly in your LINQ lambdas:

```csharp
// Using ExpressiveDbSet<T> (EF Core)
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new
    {
        o.Id,
        Name = o.Customer?.Name ?? "Unknown",
        City = o.Customer?.Address?.City?.Name
    })
    .ToList();
```

```csharp
// Using IExpressiveQueryable<T> (any IQueryable)
var results = queryable
    .AsExpressive()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, Email = o.Customer?.Email })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       COALESCE("c"."Name", 'Unknown') AS "Name",
       "c0"."Name" AS "City"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
LEFT JOIN "Addresses" AS "a" ON "c"."AddressId" = "a"."Id"
LEFT JOIN "Cities" AS "c0" ON "a"."CityId" = "c0"."Id"
WHERE "c"."Email" IS NOT NULL
```

See [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) for more examples.

## Null-Conditional with Null-Coalescing

Combine `?.` with `??` for default values:

```csharp
public class Order
{
    public Customer? Customer { get; set; }

    [Expressive]
    public string CustomerName => Customer?.Name ?? "Guest";

    [Expressive]
    public string ShippingCity => Customer?.Address?.City?.Name ?? "No City";
}
```

Generated SQL (SQLite):

```sql
SELECT COALESCE("c"."Name", 'Guest') AS "CustomerName",
       COALESCE("c0"."Name", 'No City') AS "ShippingCity"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
LEFT JOIN "Addresses" AS "a" ON "c"."AddressId" = "a"."Id"
LEFT JOIN "Cities" AS "c0" ON "a"."CityId" = "c0"."Id"
```

## Without EF Core: Applying the Transformer Manually

If you are not using EF Core (and therefore not using `UseExpressives()`), you can apply the transformer per-member or globally:

### Per-member

```csharp
[Expressive(Transformers = new[] { typeof(RemoveNullConditionalPatterns) })]
public string? CustomerName => Customer?.Name;
```

### Globally

```csharp
ExpressiveOptions.Default.AddTransformers(new RemoveNullConditionalPatterns());

// All subsequent ExpandExpressives() calls strip null-conditional patterns
Expression<Func<Order, string?>> expr = o => o.CustomerName;
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
If your LINQ provider does not handle null propagation natively (for example, Cosmos DB or an in-memory provider used in tests), you may want to **not** apply `RemoveNullConditionalPatterns`. The faithful ternary pattern will evaluate correctly in those environments.
:::

## See Also

* [Computed Entity Properties](/recipes/computed-properties) -- building blocks that can include nullable navigation
* [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) -- `?.` directly in Where/Select
* [Reusable Query Filters](/recipes/reusable-query-filters) -- filters that guard against null navigation
