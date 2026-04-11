---
url: 'https://efnext.github.io/ExpressiveSharp/reference/null-conditional-rewrite.md'
---
# Null-Conditional Rewrite

Expression trees -- the representation LINQ providers like EF Core use internally -- cannot directly express the null-conditional operator (`?.`). ExpressiveSharp handles this transparently by generating faithful null-check ternaries at compile time and providing a transformer to strip them when targeting SQL databases.

## The Problem

Consider this property:

```csharp
[Expressive]
public string? CustomerEmail => Customer?.Email;
```

This is valid C#, but the null-conditional operator `?.` has no direct representation in `Expression<Func<T, TResult>>`. Attempting to write this in a lambda expression tree produces:

```
error CS8072: An expression tree lambda may not contain a null propagating operator
```

## ExpressiveSharp's Approach

ExpressiveSharp always generates a **faithful ternary** for null-conditional operators. The `?.` in `Customer?.Email` becomes:

```csharp
Customer != null ? Customer.Email : default(string)
```

This is the generated expression tree equivalent -- it preserves the exact semantics of the original C# code. There is no per-member configuration needed; `?.` simply works.

::: info
Unlike Projectables, which required a per-member `NullConditionalRewriteSupport` enum (`None`, `Ignore`, or `Rewrite`), ExpressiveSharp always generates the faithful ternary. The stripping of null checks is handled separately by the `RemoveNullConditionalPatterns` transformer.
:::

## The `RemoveNullConditionalPatterns` Transformer

SQL databases handle NULL propagation natively -- `NULL.column` evaluates to `NULL` without needing an explicit null check. The generated ternaries add unnecessary complexity to SQL output. The `RemoveNullConditionalPatterns` transformer strips them:

**Before transformer (faithful ternary):**

```csharp
Customer != null ? Customer.Email : default(string)
```

**After transformer (simplified):**

```csharp
Customer.Email
```

### Automatic Application with `UseExpressives()`

When you call `UseExpressives()` on your EF Core `DbContextOptionsBuilder`, the `RemoveNullConditionalPatterns` transformer is applied automatically to all queries:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives()
    .Options;
```

No additional configuration is needed. All `[Expressive]` members with `?.` operators will have their null-check ternaries stripped before EF Core translates the query to SQL.

### Manual Application with `Transformers` Property

If you are not using EF Core (or want per-member control without `UseExpressives()`), apply the transformer on individual members:

```csharp
[Expressive(Transformers = new[] { typeof(RemoveNullConditionalPatterns) })]
public string? CustomerName => Customer?.Name;
```

Or apply it when expanding expressions manually:

```csharp
Expression<Func<Order, string?>> expr = o => o.CustomerEmail;
var expanded = expr.ExpandExpressives(new RemoveNullConditionalPatterns());
```

## Multi-Level Nullable Chain

Chained null-conditional operators generate nested ternaries:

```csharp
[Expressive]
public string? CustomerCity => Customer?.Address?.City;
```

Generated expression (before transformer):

```csharp
Customer != null
    ? (Customer.Address != null ? Customer.Address.City : default(string))
    : default(string)
```

After `RemoveNullConditionalPatterns`:

```csharp
Customer.Address.City
```

### SQL Output Comparison

**Without transformer** (faithful ternary preserved):

```sql
SELECT CASE
    WHEN "c"."Id" IS NOT NULL THEN
        CASE
            WHEN "a"."Id" IS NOT NULL THEN "a"."City"
            ELSE NULL
        END
    ELSE NULL
END
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
LEFT JOIN "Addresses" AS "a" ON "c"."AddressId" = "a"."Id"
```

**With transformer** (null checks stripped -- applied by `UseExpressives()`):

```sql
SELECT "a"."City"
FROM "Orders" AS "o"
LEFT JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
LEFT JOIN "Addresses" AS "a" ON "c"."AddressId" = "a"."Id"
```

The second form is cleaner and produces identical results because the `LEFT JOIN` already handles the null case -- if there is no matching customer or address, the column value is `NULL`.

## When to Keep the Null Checks

In most SQL database scenarios, you want the transformer active (which `UseExpressives()` does automatically). However, there are cases where you may want to keep the explicit null checks:

* **Client-side evaluation** -- if the expression will be compiled and executed in-memory, keeping the null checks prevents `NullReferenceException`.
* **Cosmos DB** -- some NoSQL providers may evaluate expressions differently and benefit from explicit null checks.
* **Non-database LINQ providers** -- providers that do not handle null propagation implicitly.

In these cases, do not apply `RemoveNullConditionalPatterns`, and the faithful ternary will be preserved.

## Comparison with Projectables

| Aspect | Projectables | ExpressiveSharp |
|--------|-------------|-----------------|
| Configuration | Per-member `NullConditionalRewriteSupport` enum | No per-member config needed |
| Default behavior | `None` -- rejects `?.` with error | Always generates faithful ternary |
| Stripping null checks | `Ignore` mode on the attribute | `RemoveNullConditionalPatterns` transformer |
| Explicit null checks | `Rewrite` mode on the attribute | Default behavior (always faithful) |
| Global control | Not available | `UseExpressives()` applies transformer globally |

The ExpressiveSharp approach is simpler: write `?.` naturally, and the right thing happens based on whether you are targeting a SQL database (transformer strips null checks) or not (ternaries preserved).
