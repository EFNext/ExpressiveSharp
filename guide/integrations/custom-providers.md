---
url: >-
  https://efnext.github.io/ExpressiveSharp/guide/integrations/custom-providers.md
---
# Custom Providers

ExpressiveSharp is not tied to EF Core or MongoDB — it works with any `IQueryable<T>`. This page covers how to use it with your own provider, a third-party LINQ provider, or even LINQ to Objects.

## The Core Contract

The `ExpressiveSharp` package provides two provider-neutral entry points:

* `.AsExpressive()` — wraps any `IQueryable<T>` in an `IExpressiveQueryable<T>`. Modern C# syntax works in the query; `[Expressive]` members are expanded; the normalized expression tree is handed to the underlying provider.
* `ExpressionPolyfill.Create(...)` — builds an `Expression<TDelegate>` from a delegate lambda that uses modern syntax, for cases where you need a bare expression tree rather than a queryable.

Neither of these requires EF Core, MongoDB, or any specific provider assembly.

## Wrapping an IQueryable

Any `IQueryable<T>` works:

```csharp
using ExpressiveSharp;

IQueryable<Customer> raw = GetCustomers();       // your own provider
var customers = raw.AsExpressive();

var results = customers
    .Where(c => c.Email != null && c.IsVip)      // [Expressive] + modern syntax
    .Select(c => new { c.Name, c.Email })
    .ToList();
```

The query runs through your provider's own translation pipeline after ExpressiveSharp has:

1. Expanded `[Expressive]` member accesses into their generated expression trees
2. Normalized the tree via the built-in transformers (null-conditional flattening, block lifting, tuple comparison flattening)

## LINQ to Objects

The same extension works on `IEnumerable<T>.AsQueryable()`:

```csharp
var people = peopleList.AsQueryable().AsExpressive();

var filtered = people
    .Where(p => p.Employer?.Name == "Acme" && p.IsActive)  // [Expressive] IsActive
    .ToList();
```

Useful for unit tests or in-memory scenarios where you want the same `[Expressive]` logic you use in your database code path.

## ExpressionPolyfill.Create

Sometimes you don't have a queryable — you need an `Expression<Func<T, bool>>` to pass to a method that takes one. `ExpressionPolyfill.Create` builds that expression from a delegate lambda, so you can use modern syntax inline:

```csharp
using ExpressiveSharp;

Expression<Func<Customer, bool>> predicate =
    ExpressionPolyfill.Create((Customer c) => c.Email?.Length > 5);

// Use it with your own API that takes Expression<T>
provider.Query(predicate);
```

See [ExpressionPolyfill.Create](../expression-polyfill) for the full API.

## Implementing a Custom Plugin

If you're building a provider and want to ship ExpressiveSharp-specific transformers (e.g., to handle a dialect quirk), implement `IExpressionTreeTransformer` and register it via the plugin architecture. See [Custom Transformers](../../advanced/custom-transformers) for the full walkthrough.

## Next Steps

* [IExpressiveQueryable\<T>](../expressive-queryable) — full API surface
* [ExpressionPolyfill.Create](../expression-polyfill) — build `Expression<T>` inline
* [Custom Transformers](../../advanced/custom-transformers) — hook into the pipeline
