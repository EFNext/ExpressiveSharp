---
url: 'https://efnext.github.io/ExpressiveSharp/advanced/limitations.md'
---
# Limitations

This page documents the current limitations of ExpressiveSharp and guidance on how to work around them.

::: info
For step-by-step help resolving specific issues, see [Troubleshooting](../reference/troubleshooting).
:::

## Members Must Have a Body

An `[Expressive]` member must have an **expression body** or a **block body** (with `AllowBlockBody = true`). Abstract members, interface declarations, and auto-properties without accessors produce diagnostic EXP0001.

```csharp
// EXP0001 -- no body
[Expressive]
public string FullName { get; set; }

// Expression-bodied property -- works
[Expressive]
public string FullName => $"{FirstName} {LastName}";
```

## Block Body Restrictions

When using [block-bodied members](./block-bodied-members), the following constructs are **not supported**:

| Construct | Diagnostic | Severity | Reason |
|---|---|---|---|
| `while` / `do-while` loops | EXP0006 | Warning | No reliable expression tree equivalent |
| `try` / `catch` / `finally` | EXP0006 | Warning | No expression tree equivalent |
| `throw` statements | EXP0006 | Warning | Not reliably translatable by LINQ providers |
| `async` / `await` | EXP0005 | Error | Side effects incompatible with expression trees |
| Assignments (`x = y`) | EXP0005 | Error | Side effects in expression trees |
| `++` / `--` | EXP0005 | Error | Side effects in expression trees |

```csharp
// Not supported -- while loop
[Expressive(AllowBlockBody = true)]
public int Process()
{
    int total = 0;
    while (total < 100) { total += Price; }  // EXP0005
    return total;
}

// Use LINQ instead
[Expressive]
public double TotalPrice => LineItems.Sum(i => i.Price);
```

::: tip
`foreach` loops **are** supported. They are emitted as `Expression.Loop` and rewritten to LINQ calls by the `ConvertLoopsToLinq` transformer. `for` loops are also emitted but produce an **EXP0006 warning** recommending `foreach` instead. See [Block-Bodied Members](./block-bodied-members) for details.
:::

## Local Variable Inlining and Duplication

The `FlattenBlockExpressions` transformer (applied by `UseExpressives()` in EF Core) inlines local variables at every usage point. If a variable is referenced multiple times, the initializer expression is duplicated:

```csharp
[Expressive(AllowBlockBody = true)]
public double Compute()
{
    var x = Price * Quantity;
    return x + x;
    // After FlattenBlockExpressions: (Price * Quantity) + (Price * Quantity)
}
```

This can increase SQL complexity and, in theory, change semantics if the initializer has observable side effects. The generator detects potential side effects and reports EXP0005.

## Expression Tree Standard Restrictions

Since `[Expressive]` members are ultimately compiled to expression trees, all standard `System.Linq.Expressions` limitations apply:

| Restriction | Explanation |
|---|---|
| No `dynamic` typing | Expression trees must be statically typed |
| No `ref` / `out` parameters | `Expression.Parameter` does not support by-ref semantics |
| No `unsafe` code | Pointers and address-of have no expression tree equivalent |
| No `stackalloc` | Stack allocation cannot appear in expression trees |
| No multi-statement lambdas (in expression position) | Expression-bodied members must be a single expression; block bodies go through the converter with the restrictions listed above |

## EF Core Translatable Operations Only

When targeting EF Core, the body of an `[Expressive]` member can only use operations that EF Core knows how to translate to SQL:

* Mapped entity properties and navigation properties
* Other `[Expressive]` members (transitively expanded)
* EF Core built-in functions (`EF.Functions.Like(...)`, `DateTime.Now`, etc.)
* LINQ methods EF Core supports (`Where`, `Sum`, `Any`, `Select`, etc.)
* String methods (`Contains`, `StartsWith`, `ToUpper`, etc.)
* Math methods (`Math.Abs`, `Math.Round`, etc.)

```csharp
// Runtime translation error -- Path.Combine has no SQL equivalent
[Expressive]
public string FilePath => Path.Combine(Directory, FileName);

// Works -- string interpolation is translated by EF Core
[Expressive]
public string FilePath => $"{Directory}/{FileName}";
```

::: info Using ExpressiveFor for Unsupported Methods
If you need to use a method that EF Core cannot translate, provide a translatable equivalent via [`[ExpressiveFor]`](/reference/expressive-for):

```csharp
[ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
static bool IsNullOrWhiteSpace(string? s)
    => s == null || s.Trim().Length == 0;
```

:::

## Virtual and Polymorphic Members {#virtual-polymorphic-members}

Virtual, `abstract`, and `override` `[Expressive]` members **dispatch polymorphically at runtime**. When such a member is expanded for a query provider, `ExpressiveReplacer` discovers the derived `[Expressive]` overrides across the loaded assemblies and emits a runtime type-test chain, so each row uses its own runtime type's body:

```csharp
public class Animal
{
    public string Name { get; set; } = "";

    [Expressive]
    public virtual string Describe() => $"Animal: {Name}";
}

public class Dog : Animal
{
    [Expressive]
    public override string Describe() => $"Dog: {Name}";
}

// Expands to: a is Dog ? ("Dog: " + ((Dog)a).Name) : ("Animal: " + a.Name)
db.Animals.AsExpressive().Select(a => a.Describe());
```

EF Core translates `a is Dog` to a table-per-hierarchy discriminator check, so the query emits a `CASE` over the discriminator column and returns the right text per row. In-memory delegates (`.Compile()`) evaluate the same `is` test against the CLR runtime type, so behavior converges with provider translation.

### Mark every override `[Expressive]`

Only overrides that are themselves `[Expressive]` participate. An override that forgets the attribute is invisible to expansion — instances of that type silently fall back to the **base** body — so the analyzer reports [EXP0032](../reference/diagnostics#exp0032) with an "Add `[Expressive]`" fix. If an override is intentionally client-only, mark it `[NotExpressive]` to opt out.

### Opting out

Polymorphic dispatch is on by default. To turn it off for an entire context (e.g. a provider that cannot translate type tests), call `DisablePolymorphicDispatch()` — virtual members then expand using the static (declared) type only:

```csharp
// EF Core
optionsBuilder.UseExpressives(o => o.DisablePolymorphicDispatch());

// Standalone
var options = new ExpressiveOptions();
options.DisablePolymorphicDispatch();
expression.ExpandExpressives(options);
```

Per-override `[NotExpressive]` is independent of this switch.

### Caveats

* **Discovery is runtime and best-effort.** Overrides are found in the assemblies loaded when the query first runs (the plan is cached and refreshed when new assemblies load). For EF Core this is a non-issue: entity types are registered when the model is built, before any query. An override whose assembly loads later and is not referenced by the query falls back to the base body.
* **Provider must translate type tests.** EF Core relational providers translate `is`/discriminator checks for TPH; some mappings (TPC) or providers may not. If a provider cannot translate the conditional, the query throws at translation time — the same failure mode as any untranslatable expression.
* **Interfaces are not in scope.** Default interface members keep static interface-implementation resolution; only class virtual members dispatch polymorphically.

::: tip
Declaring entity members `virtual` is common in EF Core because it enables lazy-loading proxies. That remains fine for plain navigation and scalar properties; for `[Expressive]` members it now additionally enables polymorphic dispatch.
:::

## Performance: First-Execution Overhead

`ExpandExpressives()` walks the expression tree and substitutes `[Expressive]` member references on every query execution. This adds a small cost to the first execution of each unique query shape. EF Core caches the compiled query afterward, so subsequent executions of the same shape skip the expansion entirely.

For standalone use (without EF Core), the resolved expressions are cached in `ExpressiveResolver` after the first lookup. The reflection-based slow path (for open-generic types) is also cached.

::: tip
If first-execution latency is critical, warm up the cache by calling `ExpandExpressives()` on your query expressions during application startup.
:::

## Supported C# Features

### Expression-Level

| Feature | Status | Notes |
|---|---|---|
| Null-conditional `?.` (member access and indexer) | Supported | Generates faithful null-check ternary; `UseExpressives()` strips it for SQL |
| Switch expressions | Supported | Translated to nested CASE/ternary |
| Pattern matching (constant, type, relational, logical, property, positional) | Supported | |
| Declaration patterns with named variables | Partial | Works in switch arms only |
| String interpolation | Supported | Converted to `string.Concat` calls. Format specifiers (`:F2`, etc.) emit `ToString(format)` -- see warning below. Alignment specifiers are unsupported (EXP0008). |
| Tuple literals | Supported | |
| Enum method expansion | Supported | Expands enum extension methods into per-value ternary chains |
| C# 14 extension members | Supported | |
| List patterns (fixed-length and slice) | Supported | |
| Index/range (`^1`, `1..3`) | Supported | |
| `with` expressions (records) | Supported | |
| Collection expressions (`[1, 2, 3]`, `[..items]`) | Supported | |
| Dictionary indexer initializers | Supported | |
| `this`/`base` references | Supported | |
| Checked arithmetic (`checked(...)`) | Supported | |

::: warning Format specifiers in string interpolation
String interpolation with format specifiers like `$"{Price:F2}"` introduces a `ToString(string)` call into the generated expression tree. EF Core cannot translate `ToString(string)` to SQL. In a final `Select` projection this silently falls back to client evaluation (performance cost), but in `Where`, `OrderBy`, or other server-evaluated positions it throws `InvalidOperationException`. Simple interpolation without format specifiers (e.g., `$"Order #{Id}"`) is server-translatable because it lowers to `string.Concat` overloads that EF Core supports (2/3/4-arg). For interpolations with 5+ parts, the emitter uses `string.Concat(string[])`; the `FlattenConcatArrayCalls` transformer rewrites this into supported `Concat` calls when using `UseExpressives()`.
:::

### Block-Body

| Feature | Status |
|---|---|
| `return`, `if`/`else`, `switch` statements | Supported |
| Local variable declarations (inlined) | Supported |
| `foreach` loops (converted to LINQ) | Supported |
| `for` loops (array/list iteration) | Supported |
| `while`/`do-while`, `try`/`catch`, `async`/`await` | Not supported |
| Assignments, `++`, `--` | Not supported |

## Window Functions: Provider Support

The `ExpressiveSharp.EntityFrameworkCore.RelationalExtensions` package implements SQL window functions for relational providers compatible with SQL:2003 window function syntax:

| Provider | Status |
|---|---|
| SQL Server | Supported (`NTH_VALUE` not implemented by SQL Server itself) |
| PostgreSQL | Supported |
| SQLite | Supported |
| MySQL | Supported |
| Oracle | Supported |

Non-relational providers (Cosmos DB, in-memory) are not supported for window functions.

EF Core also tracks native window function support in [dotnet/efcore#12747](https://github.com/dotnet/efcore/issues/12747); see the [window functions guide](../guide/window-functions#forward-compatibility) for forward-compatibility notes.
