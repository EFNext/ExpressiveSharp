# Migrating from LINQKit

LINQKit and ExpressiveSharp solve overlapping problems from opposite directions. LINQKit composes `Expression<Func<...>>` **objects** at runtime (`Invoke`, `Expand`, `PredicateBuilder`). ExpressiveSharp lets you write reusable query logic as ordinary C# **members** and generates the expression trees at compile time.

That difference shapes the migration:

- Logic that is **known at compile time** -- reusable filters, computed values, DTO projections -- moves to `[Expressive]` members, and the `Invoke` / `AsExpandable` plumbing disappears.
- Logic that is **assembled at runtime** -- `PredicateBuilder` loops over user-supplied search criteria -- has no ExpressiveSharp equivalent. Keep LINQKit for that part. The two libraries run side by side, with one caveat covered in [Runtime Edge Cases](#runtime-edge-cases).

This is not an all-or-nothing migration. You can convert one expression at a time and ship in between.

::: info Verified behavior
Every coexistence claim on this page was verified against `LinqKit.Microsoft.EntityFrameworkCore` 8.1.11 (`LinqKit.Core` 1.2.11) on EF Core 8 with SQLite.
:::

## Concept Mapping

| LINQKit | ExpressiveSharp | Notes |
|---|---|---|
| `Expression<Func<T, R>>` field + `.Invoke(x)` | `[Expressive]` property or method, called normally | [Details](#invoke-to-expressive-members) |
| `[Expandable(nameof(Impl))]` stub method | `[Expressive]` on the method itself | [Details](#expandable-attribute) |
| `.AsExpandable()` per query | Nothing -- `UseExpressives()` expands globally | Outside EF Core: `.AsExpressive()` |
| `optionsBuilder.WithExpressionExpanding()` | `optionsBuilder.UseExpressives()` | See the [edge case](#global-expansion-order) before combining them |
| `expr.Expand()` | `expr.ExpandExpressives()` | Expands `[Expressive]` members only -- it does **not** inline `Invoke` calls |
| `PredicateBuilder` / `ExpressionStarter<T>` | *(no equivalent)* | Keep LINQKit; [it works with `[Expressive]` members](#predicatebuilder) |
| `Invoke` of an expression **passed in as a parameter** | *(no equivalent)* | Keep LINQKit for these call sites |
| Provider-untranslatable BCL method workaround | `[ExpressiveFor]` | See [External Member Mapping](/recipes/external-member-mapping) |

## Package Changes

```bash
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

Then enable it on your `DbContext`:

```csharp
options.UseSqlServer(connectionString)
       .UseExpressives();
```

Do not remove `LinqKit.Microsoft.EntityFrameworkCore` (or `LinqKit.Core`) yet. Remove it at the end, and only if no `PredicateBuilder`, `Invoke`, or `Expand` usage is left.

## API Changes

### `Invoke` to `[Expressive]` Members {#invoke-to-expressive-members}

The classic LINQKit pattern stores an expression in a field and splices it into queries with `Invoke`:

```csharp
// Before (LINQKit)
public static class OrderExpressions
{
    public static readonly Expression<Func<Order, int, bool>> IsBig =
        (o, threshold) => o.Total > threshold;
}

var customers = db.Customers
    .AsExpandable()
    .Where(c => c.Orders.Any(o => OrderExpressions.IsBig.Invoke(o, 100)))
    .ToList();
```

With ExpressiveSharp the expression becomes a plain method. There is no `Invoke` and no `AsExpandable()`:

```csharp
// After (ExpressiveSharp)
public static class OrderExtensions
{
    [Expressive]
    public static bool IsBig(this Order o, int threshold) => o.Total > threshold;
}

var customers = db.Customers
    .Where(c => c.Orders.Any(o => o.IsBig(100)))
    .ToList();
```

Both produce the same SQL:

```sql
SELECT "c"."Id", "c"."Country", "c"."Name"
FROM "Customers" AS "c"
WHERE EXISTS (
    SELECT 1
    FROM "Orders" AS "o"
    WHERE "c"."Id" = "o"."CustomerId" AND "o"."Total" > 100)
```

Expressions that only read the entity's own state are usually better as properties on the entity:

```csharp
public class Customer
{
    public List<Order> Orders { get; set; } = new();

    [Expressive]
    public bool IsBigSpender => Orders.Sum(o => o.Total) > 100;
}
```

`[Expressive]` members compose: one member can call another, and the whole chain is expanded before the query reaches the provider. See [Reusable Query Filters](/recipes/reusable-query-filters).

### Projections

Selector expressions convert the same way.

```csharp
// Before (LINQKit)
static readonly Expression<Func<Customer, CustomerDto>> ToDto =
    c => new CustomerDto { Name = c.Name, OrderCount = c.Orders.Count };

db.Customers.AsExpandable().Select(c => new { Dto = ToDto.Invoke(c) });

// After (ExpressiveSharp)
[Expressive]
public static CustomerDto ToDto(this Customer c) =>
    new CustomerDto { Name = c.Name, OrderCount = c.Orders.Count };

db.Customers.Select(c => new { Dto = c.ToDto() });
```

See [DTO Projections with Constructors](/recipes/dto-projections).

### `[Expandable]` Attribute {#expandable-attribute}

LINQKit's `[Expandable]` needs a stub method plus a separate method that returns the expression. `[Expressive]` needs only the real method:

```csharp
// Before (LINQKit)
[Expandable(nameof(IsDomesticImpl))]
public static bool IsDomestic(this Customer c) => throw new NotSupportedException();

private static Expression<Func<Customer, bool>> IsDomesticImpl() => c => c.Country == "US";

// After (ExpressiveSharp)
[Expressive]
public static bool IsDomestic(this Customer c) => c.Country == "US";
```

::: tip In-memory calls now work
A LINQKit stub typically throws (or compiles its expression on every call) when it is used outside a query. An `[Expressive]` member is a normal C# member: calling `customer.IsDomestic()` in application code or a unit test runs the real body.
:::

### `AsExpandable()` and `WithExpressionExpanding()`

With EF Core, `UseExpressives()` expands `[Expressive]` members in every query, so converted call sites simply drop `.AsExpandable()`.

Keep `.AsExpandable()` on any query that still contains an `Invoke` or an `[Expandable]` method. Without it, EF Core receives the raw `Invoke` and fails to translate it -- exactly as it did before the migration.

Outside EF Core, the closest equivalent is [`.AsExpressive()`](./expressive-queryable), which wraps any `IQueryable<T>`.

### `PredicateBuilder` {#predicatebuilder}

Keep using it. `PredicateBuilder` output is an ordinary lambda, so it works with `UseExpressives()` without `AsExpandable()`, and the predicates can reference `[Expressive]` members:

```csharp
var predicate = PredicateBuilder.New<Customer>(true);

if (filter.DomesticOnly)
    predicate = predicate.And(c => c.IsDomestic());     // [Expressive] method

if (filter.BigSpendersOnly)
    predicate = predicate.And(c => c.IsBigSpender);     // [Expressive] property

var customers = db.Customers.Where(predicate).ToList();
```

A lambda assigned to `Expression<Func<...>>` is still bound by the normal C# expression-tree restrictions, so `?.`, switch expressions, and pattern matching do not compile there. Wrap the lambda in [`ExpressionPolyfill.Create`](./expression-polyfill) to lift that restriction:

```csharp
predicate = predicate.And(
    ExpressionPolyfill.Create((Customer c) => c.Name?.Length > 2));
```

## Runtime Edge Cases

### Global expansion order {#global-expansion-order}

This is the one combination that fails. With LINQKit's **global** `WithExpressionExpanding()` enabled next to `UseExpressives()`, an `[Expressive]` member that is referenced *inside an invoked expression variable* is not expanded:

```csharp
options.UseSqlite(conn).UseExpressives().WithExpressionExpanding();   // either order

Expression<Func<Customer, bool>> isBig = c => c.IsBigSpender;         // [Expressive] inside

db.Customers.Where(c => isBig.Invoke(c)).ToList();
// InvalidOperationException: The LINQ expression
//   'DbSet<Customer>().Where(c => c.IsBigSpender)' could not be translated.
```

ExpressiveSharp expands the query first. At that point `isBig` is still an opaque captured variable, so the `[Expressive]` member inside it is invisible. LINQKit then inlines the variable, and nothing expands the newly exposed member. The order of the two `Use...` calls makes no difference.

Everything else works under the global hook: plain `Invoke`, `[Expandable]` methods, and `[Expressive]` members written directly in the query.

Pick any of these fixes:

| Fix | Code |
|---|---|
| Use per-query `AsExpandable()` -- LINQKit then inlines *before* ExpressiveSharp expands | `db.Customers.AsExpandable().Where(c => isBig.Invoke(c))` |
| Call `Expand()` up front | `db.Customers.Where(outer.Expand())` |
| Pre-expand the inner expression | `var isBig2 = (Expression<Func<Customer, bool>>)isBig.ExpandExpressives();` |
| Convert the call site so no `Invoke` remains | `db.Customers.Where(c => c.IsBigSpender)` |

### `AsExpandable()` and `AsExpressive()` on the same query

Both orders work, including modern syntax together with `Invoke`:

```csharp
db.Customers.AsExpandable().AsExpressive()
    .Where(c => c.Name?.Length > 2 && isDomestic.Invoke(c));

db.Customers.AsExpressive().AsExpandable()
    .Where(c => isDomestic.Invoke(c) && c.IsBigSpender);
```

### `ExpandExpressives()` is not `Expand()`

`ExpandExpressives()` replaces `[Expressive]` member accesses. It does not inline `Invoke(...)` or `Compile()(...)` calls. If an expression contains both, call LINQKit's `Expand()` first, then `ExpandExpressives()`.

### Null-conditional operators

LINQKit expressions cannot contain `?.`, so existing code uses explicit null checks or relies on the database's null propagation. Both keep working unchanged. If you adopt `?.` after migrating, `UseExpressives()` strips the generated null checks for EF Core, so the SQL stays the same. In-memory providers keep the null check. See [Null-Conditional Rewrite](../reference/null-conditional-rewrite).

### Member bodies must be translatable

An `[Expressive]` body is translated the same way the old expression was: every call inside it must be understood by your provider. If a conversion pulls in a BCL method the provider cannot translate, map it with [`[ExpressiveFor]`](/recipes/external-member-mapping) rather than keeping an expression field around for it.

### Expressions received as parameters

A method that accepts an `Expression<Func<T, bool>>` and splices it into a larger query is inherently runtime composition:

```csharp
IQueryable<Customer> WithMatchingOrder(Expression<Func<Order, bool>> orderFilter) =>
    db.Customers.AsExpandable().Where(c => c.Orders.Any(o => orderFilter.Invoke(o)));
```

Leave these on LINQKit. Callers can still pass lambdas that use `[Expressive]` members, because per-query `AsExpandable()` inlines them before ExpressiveSharp runs.

## Quick Migration Checklist

::: warning Before you begin
Make sure you have a clean working tree and a passing test suite. If your tests capture generated SQL (`ToQueryString()`), keep them: converted call sites should produce identical SQL.
:::

1. Add `ExpressiveSharp.EntityFrameworkCore` and call `UseExpressives()`. Keep LINQKit installed.
2. If you use the global `WithExpressionExpanding()`, read [Global expansion order](#global-expansion-order) first.
3. Convert static `Expression<Func<...>>` fields into `[Expressive]` members, one at a time.
4. At each converted call site, replace `X.Invoke(args)` with a normal call and drop `.AsExpandable()` if no other `Invoke` remains in that query.
5. Replace each `[Expandable]` stub and its `Impl` method with a single `[Expressive]` method.
6. Leave `PredicateBuilder` code and expression-parameter helpers as they are.
7. Run your test suite and compare generated SQL.
8. Remove the LINQKit package only if nothing references `LinqKit` any more.

## See Also

- [Reusable Query Filters](/recipes/reusable-query-filters) -- the `[Expressive]` replacement for shared predicate expressions
- [DTO Projections with Constructors](/recipes/dto-projections) -- the replacement for shared selector expressions
- [`ExpressionPolyfill.Create`](./expression-polyfill) -- modern C# syntax in hand-built expressions
- [Migrating from Projectables](./migration-from-projectables)
