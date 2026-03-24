# Migrating from EntityFrameworkCore.Projectables to ExpressiveSharp

ExpressiveSharp is the successor to EntityFrameworkCore.Projectables. It keeps the same core concept — mark members with an attribute, get companion expression trees via source generation — but is rebuilt with significantly broader C# syntax support, a customizable transformer pipeline, and no coupling to EF Core.

## Why Migrate

- **Modern C# syntax in LINQ chains** — Use null-conditional operators (`?.`), switch expressions, pattern matching, and more directly in `.Where()`, `.Select()`, `.OrderBy()` etc. via `IRewritableQueryable<T>`. Inline LINQ queries are now on par with `[Expressive]` members in terms of syntax support.

- **Broader C# syntax in `[Expressive]` members** — Switch expressions, pattern matching (constant, type, relational, logical, property, positional), string interpolation, tuples, and constructor projections all work out of the box.

- **Not EF Core specific** — Works standalone with any LINQ provider, or use `ExpressionPolyfill.Create` to build expression trees without a queryable at all.

- **More accurate code generation** — The source generator now analyzes code at the semantic level rather than rewriting syntax, producing more correct and optimized expression trees.

- **Customizable transformers** — The `IExpressionTreeTransformer` interface lets you plug in your own expression tree transformations, replacing the fixed rewrite modes from Projectables.

- **Simpler configuration** — No `CompatibilityMode`, no `ProjectableOptionsBuilder`. `UseExpressives()` handles all the EF Core defaults automatically.

## Package Changes

| Old Package | New Package | Notes |
|---|---|---|
| `EntityFrameworkCore.Projectables` | `ExpressiveSharp.EntityFrameworkCore` | Direct replacement — includes `ExpressiveSharp` core as a dependency |
| `EntityFrameworkCore.Projectables.Abstractions` | *(included above)* | No longer a separate package |
| `EntityFrameworkCore.Projectables.Generator` | *(included above)* | Generator ships as an analyzer inside the core package |

```bash
# Remove old packages
dotnet remove package EntityFrameworkCore.Projectables
dotnet remove package EntityFrameworkCore.Projectables.Abstractions

# Add new package
dotnet add package ExpressiveSharp.EntityFrameworkCore
```

### Automated Migration with Code Fixers

`ExpressiveSharp.EntityFrameworkCore` includes built-in Roslyn analyzers that detect old Projectables API usage and offer automatic code fixes:

| Diagnostic | Detects | Auto-fix |
|---|---|---|
| `EXP1001` | `[Projectable]` attribute | Renames to `[Expressive]`, removes obsolete properties |
| `EXP1002` | `UseProjectables(...)` call | Replaces with `UseExpressives()` |
| `EXP1003` | `using EntityFrameworkCore.Projectables*` | Replaces with `using ExpressiveSharp*` |

After installing the package, build your solution — warnings will appear on all Projectables API usage. Use **Fix All in Solution** (lightbulb menu) to apply all fixes at once.

## Namespace Changes

| Old | New |
|---|---|
| `using EntityFrameworkCore.Projectables;` | `using ExpressiveSharp;` |
| `using EntityFrameworkCore.Projectables.Extensions;` | `using ExpressiveSharp.Extensions;` |
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

// After — no configuration callback, no compatibility mode
options.UseSqlServer(connectionString)
       .UseExpressives();
```

`UseExpressives()` automatically registers the `RemoveNullConditionalPatterns` and `FlattenBlockExpressions` transformers as global defaults, sets up the query compiler decorator, and configures model conventions.

### Null-Conditional Handling

Projectables had a three-value enum controlling null-conditional behavior:

| `NullConditionalRewriteSupport` | Behavior |
|---|---|
| `None` | Null-conditional operators not allowed |
| `Ignore` | `A?.B` → `A.B` (strip the null check) |
| `Rewrite` | `A?.B` → `A != null ? A.B : default` |

ExpressiveSharp always generates the faithful ternary pattern (`A != null ? A.B : default`). The consuming library decides how to handle it — `UseExpressives()` automatically registers the `RemoveNullConditionalPatterns` and `FlattenBlockExpressions` transformers as global defaults, so null-conditional patterns are stripped before queries reach the database. No per-member configuration needed.

```csharp
// Before
[Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
public string? CustomerName => Customer?.Name;

// After — just remove the property; UseExpressives() handles it globally
[Expressive]
public string? CustomerName => Customer?.Name;
```

### Removed Properties

| Old Property | Migration |
|---|---|
| `UseMemberBody = "SomeMethod"` | Remove — no longer supported. This was typically used to work around syntax limitations in Projectable expression bodies (e.g., pointing to a simpler method when block bodies weren't allowed). Since ExpressiveSharp supports block bodies, switch expressions, pattern matching, and more, you likely don't need it. If you do, please open an issue. |
| `AllowBlockBody = true` | Remove — block bodies work automatically. `UseExpressives()` registers `FlattenBlockExpressions` globally for EF Core. |
| `ExpandEnumMethods = true` | Remove — enum method expansion is enabled by default |
| `CompatibilityMode.Full / .Limited` | Remove — only the full approach exists (query compiler decoration) |

### MSBuild Properties

| Old Property | Migration |
|---|---|
| `Projectables_NullConditionalRewriteSupport` | Remove — `UseExpressives()` handles this globally |
| `Projectables_ExpandEnumMethods` | Remove — always enabled |
| `Projectables_AllowBlockBody` | Remove — always enabled |

The `InterceptorsNamespaces` MSBuild property needed for method interceptors is set automatically.

## Breaking Changes

1. **Namespace change** — All `EntityFrameworkCore.Projectables.*` namespaces become `ExpressiveSharp.*`. This is a project-wide find-and-replace.

2. **Attribute rename** — `[Projectable]` becomes `[Expressive]`.

3. **`NullConditionalRewriteSupport` enum removed** — ExpressiveSharp always generates faithful null-conditional ternaries. `UseExpressives()` globally registers the `RemoveNullConditionalPatterns` transformer to strip them for EF Core. No per-member configuration needed.

4. **`ProjectableOptionsBuilder` callback removed** — `UseProjectables(opts => { ... })` becomes `UseExpressives()` with no parameters. Global transformer configuration is done via `ExpressiveOptions.Default` if needed.

5. **`UseMemberBody` property removed** — This was typically a workaround for syntax limitations in Projectable expression bodies. Since ExpressiveSharp supports block bodies, switch expressions, pattern matching, and more, you likely don't need it. Remove any `UseMemberBody` assignments. If your use case still requires it, please [open an issue](https://github.com/koenbeuk/ExpressiveSharp/issues).

6. **`CompatibilityMode` removed** — ExpressiveSharp always uses the full query-compiler-decoration approach. The `Limited` compatibility mode does not exist.

7. **`AllowBlockBody` property removed** — Block bodies are always supported. `UseExpressives()` globally registers `FlattenBlockExpressions` for EF Core.

8. **MSBuild properties `Projectables_*` removed** — Remove any `Projectables_NullConditionalRewriteSupport`, `Projectables_ExpandEnumMethods`, or `Projectables_AllowBlockBody` properties from your `.csproj` or `Directory.Build.props`.

9. **Package consolidation** — Remove all old packages and install `ExpressiveSharp.EntityFrameworkCore` as described in [Package Changes](#package-changes).

10. **Target framework** — ExpressiveSharp targets .NET 8.0 and .NET 10.0. If you're on .NET 6 or 7, you'll need to upgrade.

## Feature Comparison

| Feature | Projectables | ExpressiveSharp |
|---|---|---|
| Attribute | `[Projectable]` | `[Expressive]` |
| Expression-bodied properties/methods | Yes | Yes |
| Block-bodied methods | Opt-in (`AllowBlockBody = true`) | Always supported |
| Null-conditional `?.` | `NullConditionalRewriteSupport` enum | Always emitted; `UseExpressives()` strips for EF Core |
| Switch expressions | No | Yes |
| Pattern matching | No | Yes (constant, type, relational, logical, property, positional) |
| String interpolation | No | Yes |
| Tuple literals | No | Yes |
| Constructor projections | No | Yes |
| Inline expression creation | No | `ExpressionPolyfill.Create(...)` |
| Modern syntax in LINQ chains | No | Yes (`IRewritableQueryable<T>`) |
| Custom transformers | No | `IExpressionTreeTransformer` interface |
| `ExpressiveDbSet<T>` | No | Yes |
| EF Core specific | Yes | No — works standalone |
| Compatibility modes | Full / Limited | Full only (simpler) |
| Code generation approach | Syntax tree rewriting | Semantic (IOperation) analysis |
| Target frameworks | .NET 6+ | .NET 8 / .NET 10 |

## New Features Available After Migration

### Modern Syntax in LINQ Chains

Use `IRewritableQueryable<T>` to write LINQ queries with modern C# syntax that normally can't compile in expression trees:

```csharp
var results = db.Orders.AsQueryable()
    .WithExpressionRewrite()
    .Where(o => o.Customer?.Email != null)
    .Select(o => new { o.Id, Name = o.Customer?.Name ?? "Unknown" })
    .OrderBy(o => o.Id)
    .ToList();
```

Or with EF Core, use `ExpressiveDbSet<T>` for seamless integration:

```csharp
public class MyDbContext : DbContext
{
    public ExpressiveDbSet<Order> Orders => Set<Order>().AsExpressiveDbSet();
}

// Modern syntax works directly — no WithExpressionRewrite() needed:
ctx.Orders.Where(o => o.Customer?.Name == "Alice").ToList();
```

### `ExpressionPolyfill.Create`

Create expression trees inline without needing an `[Expressive]` member:

```csharp
var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
// Returns Expression<Func<Order, int?>> — intercepted at compile time

// With transformers:
var expr = ExpressionPolyfill.Create(
    (Order o) => o.Customer?.Email,
    new RemoveNullConditionalPatterns());
```

### Switch Expressions

```csharp
[Expressive]
public string GetGrade() => Price switch
{
    >= 100 => "Premium",
    >= 50  => "Standard",
    _      => "Budget",
};
```

### Pattern Matching

```csharp
[Expressive]
public bool IsSpecialOrder => this is { Quantity: > 100, Price: >= 50 };
```

### String Interpolation

```csharp
[Expressive]
public string Summary => $"Order #{Id}: {Tag ?? "N/A"}";
```

### Constructor Projections

```csharp
public class OrderSummary
{
    public int Id { get; set; }
    public string Description { get; set; }

    [Expressive]
    public OrderSummary(Order order)
    {
        Id = order.Id;
        Description = $"{order.Tag}: {order.Price * order.Quantity}";
    }
}
```

### Custom Transformers

```csharp
public class MyTransformer : IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
    {
        // Your custom expression tree transformation
        return expression;
    }
}

[Expressive(Transformers = new[] { typeof(MyTransformer) })]
public double AdjustedTotal => Price * Quantity * 1.1;
```

## Quick Migration Checklist

1. Remove all `EntityFrameworkCore.Projectables*` NuGet packages
2. Add `ExpressiveSharp.EntityFrameworkCore`
3. Build — the built-in migration analyzers will flag all Projectables API usage
4. Use **Fix All in Solution** for each diagnostic (`EXP1001`, `EXP1002`, `EXP1003`) to auto-fix
5. Remove any `Projectables_*` MSBuild properties from `.csproj` / `Directory.Build.props`
6. Build again and fix any remaining compilation errors
7. Run your test suite to verify query behavior is unchanged
