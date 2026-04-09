# Migrating from Projectables

ExpressiveSharp is the successor to EntityFrameworkCore.Projectables. It keeps the same core concept -- mark members with an attribute, get companion expression trees via source generation -- but is rebuilt with significantly broader C# syntax support, a customizable transformer pipeline, and no coupling to EF Core.

This guide covers a complete step-by-step migration, including automated code fixers that handle most of the mechanical changes.

## Why Migrate

- **Modern C# syntax in LINQ chains** -- Use null-conditional operators (`?.`), switch expressions, pattern matching, and more directly in `.Where()`, `.Select()`, `.OrderBy()` via `IExpressiveQueryable<T>`.
- **Broader C# syntax in `[Expressive]` members** -- Switch expressions, pattern matching (constant, type, relational, logical, property, positional), string interpolation, tuples, and constructor projections all work out of the box.
- **Not EF Core specific** -- Works standalone with any LINQ provider, or use `ExpressionPolyfill.Create` to build expression trees without a queryable.
- **More accurate code generation** -- The source generator now analyzes code at the semantic level rather than rewriting syntax.
- **Customizable transformers** -- The `IExpressionTreeTransformer` interface lets you plug in your own expression tree transformations.
- **Simpler configuration** -- No `CompatibilityMode`. `UseExpressives()` handles all the EF Core defaults automatically.

## Package Changes

| Old Package | New Package | Notes |
|---|---|---|
| `EntityFrameworkCore.Projectables` | `ExpressiveSharp.EntityFrameworkCore` | Direct replacement -- includes core as a dependency |
| `EntityFrameworkCore.Projectables.Abstractions` | *(included above)* | No longer a separate package |
| `EntityFrameworkCore.Projectables.Generator` | *(included above)* | Generator ships as an analyzer inside the package |

```bash
# Remove old packages
dotnet remove package EntityFrameworkCore.Projectables
dotnet remove package EntityFrameworkCore.Projectables.Abstractions

# Add new package
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

## Automated Migration with Code Fixers

`ExpressiveSharp.EntityFrameworkCore` includes built-in Roslyn analyzers that detect old Projectables API usage and offer automatic code fixes:

| Diagnostic | Detects | Auto-fix |
|---|---|---|
| `EXP1001` | `[Projectable]` attribute | Renames to `[Expressive]`, removes obsolete properties |
| `EXP1002` | `UseProjectables(...)` call | Replaces with `UseExpressives()` |
| `EXP1003` | `using EntityFrameworkCore.Projectables*` | Replaces with `using ExpressiveSharp*` |

::: tip Automated bulk fix
After installing the package, build your solution -- warnings will appear on all Projectables API usage. Use **Fix All in Solution** (lightbulb menu in your IDE) to apply all fixes at once.
:::

## Namespace Changes

| Old | New |
|---|---|
| `using EntityFrameworkCore.Projectables;` | `using ExpressiveSharp;` |
| `using EntityFrameworkCore.Projectables.Extensions;` | `using ExpressiveSharp;` |
| `using EntityFrameworkCore.Projectables.Infrastructure;` | *(removed)* |

The EF Core extension methods (`UseExpressives`, `AsExpressiveDbSet`) live in the `Microsoft.EntityFrameworkCore` namespace, which you likely already import.

## API Changes

### Attribute Rename

```csharp
// Before
[Projectable]
public double Total => Price * Quantity;

// After
[Expressive]
public double Total => Price * Quantity;
```

### DbContext Configuration

```csharp
// Before
options.UseSqlServer(connectionString)
       .UseProjectables(opts =>
       {
           opts.CompatibilityMode(CompatibilityMode.Full);
       });

// After -- no compatibility mode; optional callback for plugins
options.UseSqlServer(connectionString)
       .UseExpressives();
```

An optional configuration callback is available for registering plugins:

```csharp
options.UseSqlServer(connectionString)
       .UseExpressives(opts => opts.AddPlugin(new MyPlugin()));
```

`UseExpressives()` automatically registers four transformers as global defaults (`ConvertLoopsToLinq`, `RemoveNullConditionalPatterns`, `FlattenTupleComparisons`, `FlattenBlockExpressions`), sets up the query compiler decorator, and configures model conventions.

### Null-Conditional Handling

Projectables had a three-value enum controlling null-conditional behavior:

| `NullConditionalRewriteSupport` | Behavior |
|---|---|
| `None` | Null-conditional operators not allowed |
| `Ignore` | `A?.B` becomes `A.B` (strip the null check) |
| `Rewrite` | `A?.B` becomes `A != null ? A.B : default` |

ExpressiveSharp always generates the faithful ternary pattern (`A != null ? A.B : default`). The `RemoveNullConditionalPatterns` transformer, applied globally by `UseExpressives()`, strips it before queries reach the database. No per-member configuration needed.

```csharp
// Before
[Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
public string? CustomerName => Customer?.Name;

// After -- just remove the property; UseExpressives() handles it globally
[Expressive]
public string? CustomerName => Customer?.Name;
```

::: info
Both the old `Ignore` and `Rewrite` behaviors converge to the same result in ExpressiveSharp. The transformer strips the explicit null check, and the database handles null propagation natively via LEFT JOIN.
:::

### Changed and Removed Properties

| Old Property | Migration |
|---|---|
| `UseMemberBody = "SomeMethod"` | Replace with `[ExpressiveFor]`. See [Migrating UseMemberBody](#migrating-usememberbody) below. |
| `AllowBlockBody = true` | Keep -- block bodies remain opt-in. Set per-member or globally via `Expressive_AllowBlockBody` MSBuild property. |
| `ExpandEnumMethods = true` | Remove -- enum method expansion is enabled by default. |
| `CompatibilityMode.Full / .Limited` | Remove -- only the full approach exists. |

### Migrating `UseMemberBody`

In Projectables, `UseMemberBody` let you point one member's expression body at another member -- typically to work around syntax limitations or to provide an expression-tree-friendly alternative.

ExpressiveSharp replaces this with `[ExpressiveFor]` (in the `ExpressiveSharp.Mapping` namespace), which is more explicit and works for external types too.

**Scenario 1: Same-type member with an alternative body**

```csharp
// Before (Projectables)
public string FullName => $"{FirstName} {LastName}".Trim().ToUpper();

[Projectable(UseMemberBody = nameof(FullNameProjection))]
public string FullName => ...;
private string FullNameProjection => FirstName + " " + LastName;

// After (ExpressiveSharp)
using ExpressiveSharp.Mapping;

public string FullName => $"{FirstName} {LastName}".Trim().ToUpper();

[ExpressiveFor(typeof(MyEntity), nameof(MyEntity.FullName))]
static string FullNameExpr(MyEntity e) => e.FirstName + " " + e.LastName;
```

**Scenario 2: External/third-party type methods**

`[ExpressiveFor]` also enables a use case that `UseMemberBody` never supported -- providing expression tree bodies for methods on types you do not own:

```csharp
using ExpressiveSharp.Mapping;

// Make Math.Clamp usable in EF Core queries
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static double Clamp(double value, double min, double max)
    => value < min ? min : (value > max ? max : value);

// Now this translates to SQL instead of throwing:
db.Orders.Where(o => Math.Clamp(o.Price, 20, 100) > 50)
```

**Scenario 3: Constructors**

```csharp
using ExpressiveSharp.Mapping;

[ExpressiveForConstructor(typeof(OrderDto))]
static OrderDto CreateDto(int id, string name)
    => new OrderDto { Id = id, Name = name };
```

**Key differences from `UseMemberBody`:**

| | `UseMemberBody` (Projectables) | `[ExpressiveFor]` (ExpressiveSharp) |
|---|---|---|
| Scope | Same type only | Any type (including external/third-party) |
| Syntax | Property on `[Projectable]` | Separate attribute on a stub method |
| Target member | Must be in the same class | Any accessible type |
| Namespace | `EntityFrameworkCore.Projectables` | `ExpressiveSharp.Mapping` |
| Constructors | Not supported | `[ExpressiveForConstructor]` |

::: tip
Many `UseMemberBody` use cases in Projectables existed because of syntax limitations. Since ExpressiveSharp supports switch expressions, pattern matching, string interpolation, and block bodies, you may be able to put `[Expressive]` directly on the member and delete the helper entirely.
:::

### MSBuild Properties

| Old Property | Migration |
|---|---|
| `Projectables_NullConditionalRewriteSupport` | Remove -- `UseExpressives()` handles this globally |
| `Projectables_ExpandEnumMethods` | Remove -- always enabled |
| `Projectables_AllowBlockBody` | Rename to `Expressive_AllowBlockBody` |

The `InterceptorsNamespaces` MSBuild property needed for method interceptors is set automatically.

## Breaking Changes

1. **Namespace change** -- All `EntityFrameworkCore.Projectables.*` namespaces become `ExpressiveSharp.*`. This is a project-wide find-and-replace (or use the `EXP1003` code fixer).

2. **Attribute rename** -- `[Projectable]` becomes `[Expressive]` (use the `EXP1001` code fixer).

3. **`NullConditionalRewriteSupport` enum removed** -- ExpressiveSharp always generates faithful null-conditional ternaries. `UseExpressives()` globally registers the `RemoveNullConditionalPatterns` transformer to strip them.

4. **`ProjectableOptionsBuilder` replaced by `ExpressiveOptionsBuilder`** -- `UseProjectables(opts => { ... })` becomes `UseExpressives()` (or `UseExpressives(opts => opts.AddPlugin(...))` for plugin registration).

5. **`UseMemberBody` property removed** -- Replaced by `[ExpressiveFor]` from `ExpressiveSharp.Mapping`.

6. **`CompatibilityMode` removed** -- ExpressiveSharp always uses the full query-compiler-decoration approach.

7. **`AllowBlockBody` retained (opt-in)** -- Block bodies require `AllowBlockBody = true` per-member or the MSBuild property `Expressive_AllowBlockBody`. `UseExpressives()` registers `FlattenBlockExpressions` for runtime.

8. **MSBuild properties `Projectables_*` removed** -- Remove any `Projectables_NullConditionalRewriteSupport`, `Projectables_ExpandEnumMethods`, or `Projectables_AllowBlockBody` from `.csproj` / `Directory.Build.props`.

9. **Package consolidation** -- Remove all old packages and install `ExpressiveSharp.EntityFrameworkCore`.

10. **Target framework** -- ExpressiveSharp targets .NET 8.0 and .NET 10.0. If you are on .NET 6 or 7, you will need to upgrade.

## Feature Comparison

| Feature | Projectables | ExpressiveSharp |
|---|---|---|
| Attribute | `[Projectable]` | `[Expressive]` |
| Expression-bodied properties/methods | Yes | Yes |
| Block-bodied methods | Opt-in | Opt-in |
| Null-conditional `?.` | `NullConditionalRewriteSupport` enum | Always emitted; `UseExpressives()` strips for EF Core |
| Switch expressions | No | Yes |
| Pattern matching | No | Yes (constant, type, relational, logical, property, positional) |
| String interpolation | No | Yes |
| Tuple literals | No | Yes |
| Constructor projections | No | Yes |
| Inline expression creation | No | `ExpressionPolyfill.Create(...)` |
| Modern syntax in LINQ chains | No | Yes (`IExpressiveQueryable<T>`) |
| Custom transformers | No | `IExpressionTreeTransformer` interface |
| `ExpressiveDbSet<T>` | No | Yes |
| External member mapping | `UseMemberBody` (same type only) | `[ExpressiveFor]` (any type) |
| SQL window functions | No | Yes (RelationalExtensions package) |
| EF Core specific | Yes | No -- works standalone |
| Compatibility modes | Full / Limited | Full only (simpler) |
| Code generation approach | Syntax tree rewriting | Semantic (IOperation) analysis |
| Target frameworks | .NET 6+ | .NET 8 / .NET 10 |

## New Features Available After Migration

After migrating, you gain access to features that Projectables never had. Here are some highlights:

### Modern Syntax in LINQ Chains

Use `IExpressiveQueryable<T>` or `ExpressiveDbSet<T>` to write LINQ queries with modern C# syntax:

```csharp
var results = ctx.Orders
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, Name = o.Customer?.Name ?? "Unknown" })
    .ToList();
```

See [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq).

### `ExpressionPolyfill.Create`

Create expression trees inline without needing an attribute:

```csharp
var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
```

### Switch Expressions and Pattern Matching

```csharp
[Expressive]
public string GetGrade() => Price switch
{
    >= 100 => "Premium",
    >= 50  => "Standard",
    _      => "Budget",
};

[Expressive]
public bool IsSpecialOrder => this is { Quantity: > 100, Price: >= 50 };
```

See [Scoring and Classification](/recipes/scoring-classification).

### Constructor Projections

```csharp
public class OrderSummary
{
    [Expressive]
    public OrderSummary(Order o)
    {
        Id = o.Id;
        Total = o.Price * o.Quantity;
    }
}
```

See [DTO Projections with Constructors](/recipes/dto-projections).

### External Member Mapping

```csharp
using ExpressiveSharp.Mapping;

[ExpressiveFor(typeof(Math), nameof(Math.Abs))]
static int Abs(int value) => value < 0 ? -value : value;
```

See [External Member Mapping](/recipes/external-member-mapping).

### Custom Transformers

```csharp
public class MyTransformer : IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
    {
        return expression; // your custom transformation
    }
}

[Expressive(Transformers = new[] { typeof(MyTransformer) })]
public double AdjustedTotal => Price * Quantity * 1.1;
```

### SQL Window Functions

```csharp
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

var ranked = dbContext.Orders.Select(o => new
{
    o.Id,
    Rank = WindowFunction.Rank(
        Window.PartitionBy(o.CustomerId)
              .OrderByDescending(o.GrandTotal))
});
```

See [Window Functions and Ranking](/recipes/window-functions-ranking).

## Quick Migration Checklist

::: warning Before you begin
Make sure you have a clean working tree (commit or stash your changes) and a passing test suite on the Projectables codebase before starting the migration.
:::

1. Remove all `EntityFrameworkCore.Projectables*` NuGet packages
2. Add `ExpressiveSharp.EntityFrameworkCore`
3. Build -- the built-in migration analyzers will flag all Projectables API usage
4. Use **Fix All in Solution** for each diagnostic (`EXP1001`, `EXP1002`, `EXP1003`) to auto-fix
5. Remove any `Projectables_*` MSBuild properties from `.csproj` / `Directory.Build.props`
6. Replace any `UseMemberBody` usage with `[ExpressiveFor]` (see [Migrating UseMemberBody](#migrating-usememberbody))
7. Remove any `ExpandEnumMethods`, `NullConditionalRewriteSupport`, or `CompatibilityMode` settings
8. Build again and fix any remaining compilation errors
9. Run your test suite to verify query behavior is unchanged
10. Optionally adopt new features: `ExpressiveDbSet<T>`, switch expressions, pattern matching, `ExpressionPolyfill.Create`

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- the foundational recipe
- [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) -- the biggest new capability
- [External Member Mapping](/recipes/external-member-mapping) -- replaces `UseMemberBody`
