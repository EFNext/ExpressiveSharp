# Troubleshooting

This page helps you diagnose and fix common issues with ExpressiveSharp. Entries are organized by the symptom you see -- the error message, unexpected behavior, or missing output. For a complete list of diagnostics by code, see [Diagnostics & Code Fixes](./diagnostics).

## Compile-Time Errors

### "Block body requires `AllowBlockBody`" (EXP0004) {#exp0004}

**Symptom:** You write a block body (`{ }`) on an `[Expressive]` member and the compiler reports EXP0004.

**Why:** Block bodies are opt-in because not all LINQ providers can translate every block construct (loops, local variables, conditionals). Expression-bodied members (`=>`) have broader provider coverage.

**Fix — per member:**

```csharp
[Expressive(AllowBlockBody = true)]
public string Greeting(string name)
{
    var prefix = IsVip ? "Dear" : "Hi";
    return prefix + " " + name;
}
```

**Fix — globally** (in your `.csproj` or `Directory.Build.props`):

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

::: tip
If your team uses block bodies widely, the global MSBuild property avoids repeating the attribute parameter on every member.
:::

See [Block-Bodied Members](../advanced/block-bodied-members) for full details on supported constructs.

---

### "Member must have a body definition" (EXP0001) {#exp0001}

**Symptom:** You put `[Expressive]` on an auto-property, abstract member, or interface declaration.

**Why:** The generator needs a body to translate into an expression tree. Members without bodies have nothing to generate from.

**Fix:** Add an expression body or block body, or remove the `[Expressive]` attribute if the member does not need to be inlined into expression trees:

```csharp
// Error: auto-property has no body
[Expressive]
public string FullName { get; set; }

// Fixed: expression-bodied property
[Expressive]
public string FullName => FirstName + " " + LastName;

// Also valid: remove the attribute if projection is not needed
public string FullName { get; set; }
```

---

### "Side effects in block body" (EXP0005) {#exp0005}

**Symptom:** You use `++`, `--`, compound assignment (`+=`), or property assignment inside a block-bodied `[Expressive]` member.

**Why:** Expression trees cannot represent side effects. Operations that mutate state have no `System.Linq.Expressions` equivalent.

**Fix:** Rewrite as pure computation:

```csharp
// Error: side effects
[Expressive(AllowBlockBody = true)]
public int NextIndex()
{
    var i = CurrentIndex;
    i++;             // EXP0005 -- mutation
    return i;
}

// Fixed: pure expression
[Expressive]
public int NextIndex => CurrentIndex + 1;
```

---

### "Unsupported expression operation" / "Unsupported operator" (EXP0008 / EXP0009) {#exp0008}

**Symptom:** Warnings about `throw` expressions, unsigned right shift (`>>>`), or other operations that have no expression tree equivalent.

::: warning
These are **warnings**, not errors. The generated code compiles but substitutes `default` values for the unsupported parts, which may cause incorrect runtime behavior. Do not ignore these warnings.
:::

**Common cases and fixes:**

| Operation | Fix |
|---|---|
| `x ?? throw new Exception()` | Use a ternary: `x != null ? x : fallbackValue` |
| `x >>> 2` (unsigned right shift) | Cast and use regular shift: `(int)((uint)x >> 2)` |
| `with` expression on type without `Clone` | Ensure the record type is in scope |

---

### "Member could benefit from `[Expressive]`" (EXP0013) {#exp0013}

**Symptom:** A member referenced inside an `[Expressive]` body is not itself marked `[Expressive]`, producing a warning. This means the member's body is opaque to the expression tree -- it will be called as a delegate instead of being inlined.

**Fix:** Use the IDE quick fix (lightbulb) to add `[Expressive]` automatically, or add it manually:

```csharp
// Warning: Total is not [Expressive], so TotalWithTax
// cannot inline its body into the expression tree
public double Total => Price * Quantity;

[Expressive]
public double TotalWithTax => Total * (1 + TaxRate);

// Fixed: mark Total as [Expressive] too
[Expressive]
public double Total => Price * Quantity;
```

::: tip
Use **Edit > Fix All in Solution** (or equivalent) to apply this code fix across your entire project at once.
:::

---

## Runtime / EF Core Issues

### "The LINQ expression could not be translated" {#sql-translation}

**Symptom:** `InvalidOperationException` at runtime from EF Core when executing a query that uses `[Expressive]` members.

This is EF Core telling you the final expression tree contains an operation it cannot convert to SQL. There are three common causes:

**1. Untranslatable .NET method in the body**

The `[Expressive]` member calls a method with no SQL equivalent:

```csharp
// Fails: Path.Combine has no SQL translation
[Expressive]
public string FilePath => Path.Combine(Directory, FileName);

// Works: string concatenation is translated by EF Core
[Expressive]
public string FilePath => Directory + "/" + FileName;
```

For third-party methods you cannot change, provide a translatable equivalent via [`[ExpressiveFor]`](./expressive-for):

```csharp
[ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
static bool IsNullOrWhiteSpace(string? s)
    => s == null || s.Trim().Length == 0;
```

**2. Referenced member not marked `[Expressive]`**

If an `[Expressive]` member references another member that is *not* `[Expressive]`, the referenced member remains opaque to EF Core. Look for EXP0013 warnings and add `[Expressive]` to the referenced member.

**3. Unsupported operation in the body**

Check for EXP0008/EXP0009 warnings on the member. These indicate operations that were replaced with `default` values, which can confuse EF Core's translator.

::: info
For a complete list of translatable operations, see [Limitations -- EF Core Translatable Operations](../advanced/limitations#ef-core-translatable-operations-only).
:::

---

### `[Expressive]` member not expanded -- runtime delegate call instead of SQL {#not-expanded}

**Symptom:** The query executes but the `[Expressive]` property is evaluated client-side (causing N+1 queries or `InvalidOperationException`) instead of being translated to SQL.

**Cause:** `UseExpressives()` is not configured in your `DbContext`.

**Fix:** Register it in `OnConfiguring` or via dependency injection:

```csharp
// In OnConfiguring
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(connectionString)
        .UseExpressives();  // Add this
}

// Or via DI in Startup / Program.cs
services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString)
           .UseExpressives());
```

See [EF Core Integration](../guide/ef-core-integration) for the full setup guide.

---

### Duplicated SQL from block body variables {#variable-duplication}

**Symptom:** A block-bodied `[Expressive]` member with a local variable used multiple times produces verbose SQL where the initializer expression is repeated.

```csharp
[Expressive(AllowBlockBody = true)]
public double Compute()
{
    var x = Price * Quantity;
    return x + x;
    // SQL: (Price * Quantity) + (Price * Quantity)
}
```

**Why:** The `FlattenBlockExpressions` transformer inlines local variables at every usage point to remove `Expression.Block` nodes that LINQ providers cannot translate.

::: info
SQL query optimizers typically eliminate the duplication at execution time. This is a cosmetic issue in most cases. If the expression is expensive (e.g., a subquery), restructure to use the variable only once.
:::

See [Limitations -- Local Variable Inlining](../advanced/limitations#local-variable-inlining-and-duplication).

---

### Slow first query execution {#first-query-latency}

**Symptom:** The first query involving `[Expressive]` members takes noticeably longer than subsequent identical queries.

**Why:** `ExpandExpressives()` walks and substitutes the expression tree on first execution. EF Core caches the compiled query afterward, so subsequent executions of the same query shape skip expansion entirely.

**Fix:** Warm up during application startup by executing a representative query:

```csharp
// In Program.cs or a hosted service
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
_ = db.Orders.Select(o => o.Total).Take(0).ToQueryString();
```

See [Limitations -- First-Execution Overhead](../advanced/limitations#performance-first-execution-overhead).

---

## Setup & Configuration

### Source generator not running {#generator-not-running}

**Symptom:** No EXP diagnostics appear. Generated code is not visible in Solution Explorer. `[Expressive]` members are not expanded.

**Checklist:**

1. Verify the `ExpressiveSharp` NuGet package is installed (not just `ExpressiveSharp.EntityFrameworkCore` -- the core package includes the generator).
2. Ensure the project targets **.NET 8+** with **C# 12+** (`<LangVersion>` in `.csproj` or `Directory.Build.props`).
3. Clean and rebuild: `dotnet clean && dotnet build`.
4. Restart your IDE. Visual Studio, Rider, and VS Code sometimes cache stale analyzer state.
5. Check that the generator is loaded: in Visual Studio, expand **Dependencies > Analyzers > ExpressiveSharp.Generator** in Solution Explorer.

::: tip
If the generator is loaded but not producing output, check that you have at least one member decorated with `[Expressive]` and that the member has a body.
:::

---

### Interceptors not working {#interceptors-not-working}

**Symptom:** `ExpressionPolyfill.Create()` returns a delegate at runtime instead of an expression tree. `IExpressiveQueryable<T>` LINQ methods are not rewritten to expression form.

**Why:** Method interceptors require the `InterceptorsNamespaces` MSBuild property to be set. The `ExpressiveSharp` NuGet package configures this automatically via its `.props` file.

**If it is not working:**

1. Verify the package's build props are being imported (check for `ExpressiveSharp.Generator.props` in your NuGet cache).
2. Ensure `LangVersion` is `12.0` or higher (interceptors require C# 12+).
3. As a manual fallback, add this to a `<PropertyGroup>`:

```xml
<InterceptorsNamespaces>
    $(InterceptorsNamespaces);ExpressiveSharp.Generated.Interceptors
</InterceptorsNamespaces>
```

See [How It Works](../advanced/how-it-works) for details on the interceptor generation pipeline.

---

## Migration

### Migrating from EntityFrameworkCore.Projectables (EXP1001 / EXP1002 / EXP1003) {#migration-from-projectables}

**Symptom:** After installing ExpressiveSharp alongside Projectables, you see EXP1001, EXP1002, or EXP1003 warnings.

| Diagnostic | What it suggests |
|---|---|
| EXP1001 | Replace `[Projectable]` with `[Expressive]` |
| EXP1002 | Replace `UseProjectables()` with `UseExpressives()` |
| EXP1003 | Replace Projectables namespace imports |

**Fix:** All three diagnostics have automated code fixes. Use **Edit > Fix All in Solution** (Visual Studio) or equivalent to migrate in one step.

::: tip
See the [Migration from Projectables](../guide/migration-from-projectables) guide for a complete step-by-step walkthrough.
:::

---

## Frequently Asked Questions

### Does ExpressiveSharp have runtime overhead?

No practical impact. The source generators emit `Expression.*` factory calls at compile time. At runtime, `ExpandExpressives()` replaces opaque member accesses with the pre-built expressions -- this adds a small cost on first execution, but EF Core caches the expanded query afterward. There is no runtime reflection, no `Compile()`, and no expression tree parsing.

### Can `[Expressive]` members call other `[Expressive]` members?

Yes. `ExpandExpressives()` recursively resolves nested `[Expressive]` references:

```csharp
[Expressive]
public double Total => Price * Quantity;

[Expressive]
public double TotalWithTax => Total * (1 + TaxRate);  // Total is inlined
```

### Is ExpressiveSharp EF Core specific?

No. The core `ExpressiveSharp` package works with any LINQ provider or standalone expression tree use case. See [ExpressionPolyfill.Create](../guide/expression-polyfill) and [IExpressiveQueryable](../guide/expressive-queryable) for non-EF-Core usage.

### What .NET versions are supported?

| | .NET 8.0 | .NET 10.0 |
|---|---|---|
| **ExpressiveSharp** | C# 12 | C# 14 |
| **ExpressiveSharp.EntityFrameworkCore** | EF Core 8.x | EF Core 10.x |
| **RelationalExtensions** | EF Core 8.x | EF Core 10.x |

### How do I suppress a specific diagnostic?

Use `#pragma` in code or `<NoWarn>` in your project file:

```csharp
#pragma warning disable EXP0008
[Expressive]
public string Value => Name ?? throw new InvalidOperationException();
#pragma warning restore EXP0008
```

```xml
<!-- In .csproj or Directory.Build.props -->
<PropertyGroup>
    <NoWarn>$(NoWarn);EXP0008</NoWarn>
</PropertyGroup>
```
