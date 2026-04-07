# ExpressionPolyfill.Create

`ExpressionPolyfill.Create` lets you create expression trees inline using modern C# syntax -- no `[Expressive]` attribute needed. The source generator intercepts the call at compile time and rewrites the delegate lambda into a proper expression tree.

## Basic Usage

```csharp
using ExpressiveSharp;

// The lambda uses ?. -- normally illegal in expression trees
var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
// expr is Expression<Func<Order, int?>>

var compiled = expr.Compile();
var result = compiled(order);
```

You write a regular lambda with modern syntax, and the source generator converts it into an `Expression<Func<...>>` at compile time. The result is a fully constructed expression tree that you can compile, pass to a LINQ provider, or inspect.

## With Transformers

You can apply expression transformers inline:

```csharp
using ExpressiveSharp;
using ExpressiveSharp.Transformers;

var expr = ExpressionPolyfill.Create(
    (Order o) => o.Customer?.Email,
    new RemoveNullConditionalPatterns());
```

The generated expression tree has `RemoveNullConditionalPatterns` applied, stripping the null-check ternary so the expression reads `o.Customer.Email` directly -- suitable for SQL providers that handle null propagation natively.

## Use Cases

### Ad-hoc queries

When you need modern syntax in a one-off query without decorating entity members:

```csharp
var predicate = ExpressionPolyfill.Create(
    (Order o) => o.Customer?.Email != null && o.Price switch
    {
        >= 100 => true,
        _      => false,
    });

var results = dbContext.Orders
    .Where(predicate)
    .ToList();
```

### Testing

Build expression trees for unit tests without worrying about expression tree restrictions:

```csharp
var expr = ExpressionPolyfill.Create(
    (string? s) => s?.Trim().ToUpper() ?? "EMPTY");

var compiled = expr.Compile();
Assert.AreEqual("HELLO", compiled("  hello  "));
Assert.AreEqual("EMPTY", compiled(null));
```

### Standalone expression trees

Use modern syntax in expression trees that are not tied to any LINQ provider:

```csharp
var selector = ExpressionPolyfill.Create(
    (int x) => x switch
    {
        > 0 => "positive",
        0   => "zero",
        _   => "negative",
    });

// Inspect the expression tree
Console.WriteLine(selector.Body);
```

## How It Works

`ExpressionPolyfill.Create` is defined as a regular method that accepts a `Func<...>` delegate. At compile time, the `PolyfillInterceptorGenerator` uses C# 13 method interceptors to **replace the call site** with code that constructs the equivalent `Expression<Func<...>>` using `Expression.*` factory calls.

The method itself is never actually called at runtime -- the interceptor completely replaces it. This means:

- No runtime overhead from delegate-to-expression conversion
- All expression trees are built at compile time
- The full range of modern C# syntax is available

## Comparison with [Expressive]

| | `[Expressive]` | `ExpressionPolyfill.Create` |
|---|---|---|
| **Scope** | Entity members (properties, methods, constructors) | Any inline lambda |
| **Reusability** | High -- define once, use in any query | One-off -- scoped to the call site |
| **Composition** | Can reference other `[Expressive]` members | Standalone |
| **Registration** | Automatic via expression registry | Not registered -- returns the expression directly |
| **Best for** | Computed properties and reusable query fragments | Ad-hoc queries, testing, standalone expressions |

::: tip
Use `[Expressive]` when the logic belongs on an entity and will be reused across multiple queries. Use `ExpressionPolyfill.Create` for one-off expressions or when you need modern syntax outside of an entity context.
:::

## Supported Syntax

`ExpressionPolyfill.Create` supports the same modern C# features as `[Expressive]`:

- Null-conditional `?.` (member access and indexer)
- Switch expressions with pattern matching
- String interpolation
- Tuple literals
- List patterns and index/range
- `with` expressions (records)
- Collection expressions
- Checked arithmetic

## Next Steps

- [[Expressive] Properties](./expressive-properties) -- reusable computed properties
- [IExpressiveQueryable\<T\>](./expressive-queryable) -- modern syntax directly in LINQ chains
- [EF Core Integration](./ef-core-integration) -- full EF Core setup
