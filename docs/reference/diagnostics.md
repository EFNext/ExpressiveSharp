# Diagnostics & Code Fixes

The ExpressiveSharp source generator and companion analyzers emit diagnostics during compilation to help you identify and fix issues. Many diagnostics also have IDE code fixes that resolve them automatically.

::: tip Looking for help with a specific problem?
See [Troubleshooting](./troubleshooting) for symptom-oriented guidance -- find the error message or behavior you see and get step-by-step resolution.
:::

## Overview

| ID | Severity | Title | Code Fix |
|---|---|---|---|
| [EXP0001](#exp0001) | Error | Member must have a body definition | -- |
| [EXP0002](#exp0002) | Error | Missing parameterless constructor | -- |
| [EXP0003](#exp0003) | Error | Delegated constructor cannot be analyzed | -- |
| [EXP0004](#exp0004) | Error | Block body requires opt-in | -- |
| [EXP0005](#exp0005) | Error | Side effects in block body | -- |
| [EXP0006](#exp0006) | Warning | Unsupported statement in block body | -- |
| [EXP0007](#exp0007) | Warning | Unsupported initializer in object creation | -- |
| [EXP0008](#exp0008) | Warning | Unsupported expression operation | -- |
| [EXP0009](#exp0009) | Warning | Unsupported operator | -- |
| [EXP0010](#exp0010) | Warning | Interceptor emission failed | -- |
| [EXP0011](#exp0011) | Warning | Unresolvable member in pattern | -- |
| [EXP0012](#exp0012) | Info | Factory method can be converted to constructor | -- |
| [EXP0013](#exp0013) | Error | `[ExpressiveFor]` target type not found | -- |
| [EXP0014](#exp0014) | Error | `[ExpressiveFor]` target member not found | -- |
| [EXP0015](#exp0015) | Error | `[ExpressiveFor]` return type mismatch | -- |
| [EXP0016](#exp0016) | Error | `[ExpressiveFor]` conflicts with `[Expressive]` | -- |
| [EXP0017](#exp0017) | Error | Duplicate `[ExpressiveFor]` mapping | -- |
| [EXP0018](#exp0018) | Error | `[ExpressiveProperty]` target name is already defined | -- |
| [EXP0019](#exp0019) | Error | `[ExpressiveProperty]` requires a partial containing type | -- |
| [EXP0020](#exp0020) | Error | `[ExpressiveProperty]` requires an expression-bodied property stub | -- |
| [EXP0021](#exp0021) | Error | `[ExpressiveProperty]` requires an instance stub | -- |
| [EXP0022](#exp0022) | Error | `[ExpressiveProperty]` target shadows inherited member | -- |
| [EXP0023](#exp0023) | Warning | Unsupported operation ignored | -- |
| EXP0024 | _(retired)_ | `[Expressive]` member is virtual and will not dispatch polymorphically — virtual members now dispatch polymorphically at runtime | -- |
| [EXP0025](#exp0025) | Warning | Referenced member could benefit from `[Expressive]` | [Add `[Expressive]`](#exp0025-fix) |
| [EXP0026](#exp0026) | Warning | `IExpressiveQueryable<T>` LINQ method resolves to `Queryable` | [Add `using ExpressiveSharp;`](#exp0026-fix) |
| [EXP0027](#exp0027) | Info | No `IExpressiveQueryable<T>` overload for `Queryable` method | -- |
| [EXP0028](#exp0028) | Info | Plain `IQueryable` chain references an `[Expressive]` member without `.AsExpressive()` | [Wrap with `.AsExpressive()`](#exp0028-fix) |
| [EXP0029](#exp0029) | Info | `IExpressiveQueryable<T>` chain dropped to plain `IQueryable<T>` | -- |
| [EXP0030](#exp0030) | Warning | `WindowFunction.Ntile` requires a positive bucket count | -- |
| [EXP0031](#exp0031) | Warning | `WindowFunction.Lag`/`Lead` offset must be non-negative | -- |
| [EXP0032](#exp0032) | Warning | Override of an `[Expressive]` member is missing `[Expressive]` | [Add `[Expressive]`](#exp0025-fix) |
| [EXP1001](#exp1001) | Warning | Replace `[Projectable]` with `[Expressive]` | [Replace attribute](#exp1001-fix) |
| [EXP1002](#exp1002) | Warning | Replace `UseProjectables()` with `UseExpressives()` | [Replace method call](#exp1002-fix) |
| [EXP1003](#exp1003) | Warning | Replace Projectables namespace | [Replace namespace](#exp1003-fix) |

---

## Core Diagnostics (EXP0001--EXP0012)

### EXP0001 -- Member must have a body definition {#exp0001}

**Severity:** Error
**Category:** Design

**Message:**
```
Method or property '{0}' should expose a body definition (e.g. an expression-bodied member
or a block-bodied method) to be used as the source for the generated expression tree.
```

**Cause:** An `[Expressive]` member has no body -- it is abstract, an interface declaration, or an auto-property.

**Fix:** Provide a body:

```csharp
// Error: no body
[Expressive]
public string FullName { get; set; }

// Fixed: expression-bodied property
[Expressive]
public string FullName => $"{FirstName} {LastName}";
```

---

### EXP0002 -- Missing parameterless constructor {#exp0002}

**Severity:** Error
**Category:** Design

**Message:**
```
Class '{0}' must have a parameterless constructor to be used with an [Expressive] constructor.
The generated projection uses 'new {0}() { ... }' (object-initializer syntax), which requires
an accessible parameterless constructor.
```

**Cause:** A constructor is marked `[Expressive]`, but the class does not have an accessible parameterless constructor. The generator emits `new T() { ... }` syntax which requires one.

**Fix:** Add a parameterless constructor:

```csharp
public class CustomerDto
{
    public CustomerDto() { }  // required

    [Expressive]
    public CustomerDto(Customer c)
    {
        Id = c.Id;
        Name = $"{c.FirstName} {c.LastName}";
    }
}
```

---

### EXP0003 -- Delegated constructor cannot be analyzed {#exp0003}

**Severity:** Error
**Category:** Design

**Message:**
```
The delegated constructor '{0}' in type '{1}' has no source available and cannot be analyzed.
Base/this initializer in member '{2}' will not be projected.
```

**Cause:** An `[Expressive]` constructor delegates to another constructor via `: base(...)` or `: this(...)`, but the target constructor's source code is not available in the current compilation (e.g., it lives in a referenced binary).

**Fix:** Ensure the delegated constructor's source is available in the same project, or restructure to avoid the delegation.

---

### EXP0004 -- Block body requires AllowBlockBody {#exp0004}

**Severity:** Error
**Category:** Design

**Message:**
```
Member '{0}' uses a block body ({ }) which requires [Expressive(AllowBlockBody = true)].
Block bodies support local variables, if/else, and foreach loops, but not all constructs
are translatable by every LINQ provider. Use an expression-bodied member (=>) for full
compatibility, or opt in with AllowBlockBody = true.
```

**Cause:** An `[Expressive]` member uses a block body `{ }` without opting in.

**Fix:** Either opt in to block bodies or convert to an expression body:

```csharp
// Error: block body without opt-in
[Expressive]
public string GetCategory()
{
    if (Value > 100) return "High";
    return "Low";
}

// Option 1: opt in to block body
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    if (Value > 100) return "High";
    return "Low";
}

// Option 2: convert to expression body
[Expressive]
public string GetCategory() => Value > 100 ? "High" : "Low";
```

You can also enable block bodies globally via MSBuild:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

---

### EXP0005 -- Side effects in block body {#exp0005}

**Severity:** Error
**Category:** Design

**Message:** Context-specific (e.g., property assignment, compound assignment, or increment/decrement detected).

**Cause:** A block-bodied `[Expressive]` member modifies state. Expression trees cannot represent side effects.

**Fix:** Remove the side-effecting statement. `[Expressive]` members must be pure functions.

```csharp
// Error: side effects
[Expressive(AllowBlockBody = true)]
public int Compute()
{
    Counter++;       // EXP0005: side effect
    return Counter;
}

// Fixed: pure computation
[Expressive]
public int Compute() => Counter + 1;
```

---

### EXP0006 -- Unsupported statement in block body {#exp0006}

**Severity:** Warning
**Category:** Design

**Message:**
```
Method '{0}' contains an unsupported statement: {1}
```

**Cause:** A block-bodied `[Expressive]` member contains a statement type that cannot be converted to an expression tree (e.g., `while` loops, `try`/`catch`, `throw`, `async`/`await`).

**Fix:** Refactor to use only supported constructs (`if`/`else`, `switch`, `foreach`, local variables, `return`), or convert to an expression-bodied member.

---

### EXP0007 -- Unsupported initializer {#exp0007}

**Severity:** Warning
**Category:** Design

**Message:**
```
Object initializer contains an unsupported element ({0}). Only property and field
assignments are supported in expression trees.
```

**Cause:** An object initializer in an `[Expressive]` member contains something other than a property or field assignment (e.g., collection initializer syntax, index initializer).

**Fix:** Restructure the initializer to use only property and field assignments.

---

### EXP0008 -- Unsupported expression operation {#exp0008}

**Severity:** Warning
**Category:** Design

**Message:**
```
Expression contains an unsupported operation ({0}). A default value will be used instead.
```

**Cause:** The member body contains an operation that cannot be represented in an expression tree. The generator substitutes a `default` value to allow compilation to proceed.

**Fix:** Rewrite the unsupported operation using supported C# constructs. See the [Supported C# Features](/) table in the main documentation for what is supported.

::: warning
This diagnostic is a warning, not an error. The generated code will compile, but the defaulted value may produce incorrect results at runtime. Always address EXP0008 warnings.
:::

---

### EXP0009 -- Unsupported operator {#exp0009}

**Severity:** Warning
**Category:** Design

**Message:**
```
Operator '{0}' is not supported in expression trees. A default value will be used instead.
```

**Cause:** An operator in the expression body has no expression tree equivalent.

**Fix:** Rewrite using a supported operator or method call.

---

### EXP0010 -- Interceptor emission failed {#exp0010}

**Severity:** Warning
**Category:** Design

**Message:**
```
Failed to generate interceptor for call site: {0}. The original delegate stub will be used at runtime.
```

**Cause:** The polyfill interceptor generator could not produce an interceptor for a specific call site (e.g., on `IExpressiveQueryable<T>` or `ExpressionPolyfill.Create`). The original delegate-based stub will be used instead.

**Fix:** This is typically an internal generator issue. If you encounter it, check that the call site uses supported syntax and consider filing an issue.

---

### EXP0011 -- Unresolvable member in pattern {#exp0011}

**Severity:** Warning
**Category:** Design

**Message:**
```
Pattern sub-expression for member '{0}' could not be resolved and was skipped.
The pattern may not match correctly.
```

**Cause:** A property pattern references a member that could not be resolved during analysis. The pattern sub-expression is skipped.

**Fix:** Ensure the member referenced in the pattern exists and is accessible. Check for typos or missing `using` directives.

---

### EXP0012 -- Factory method can be converted to a constructor {#exp0012}

**Severity:** Info
**Category:** Design

**Message:**
```
Factory method '{0}' creates and returns an instance of the containing class via object
initializer. Consider converting it to an [Expressive] constructor.
```

**Cause:** An `[Expressive]` method creates and returns a `new T { ... }` object initializer where `T` is the containing class. This pattern is equivalent to an `[Expressive]` constructor.

**Fix:** Convert the factory method to a constructor:

```csharp
// Before: factory method (triggers EXP0012)
[Expressive]
public static CustomerDto FromCustomer(Customer c) => new CustomerDto
{
    Id = c.Id,
    Name = $"{c.FirstName} {c.LastName}"
};

// After: expressive constructor
[Expressive]
public CustomerDto(Customer c)
{
    Id = c.Id;
    Name = $"{c.FirstName} {c.LastName}";
}
```

---

## External Mapping Diagnostics (EXP0013--EXP0017)

These diagnostics are specific to `[ExpressiveFor]` and `[ExpressiveForConstructor]`. See [`[ExpressiveFor]` Mapping](./expressive-for) for full usage details.

### EXP0013 -- Target type not found {#exp0013}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveFor] target type '{0}' could not be resolved
```

**Cause:** The `Type` argument passed to `[ExpressiveFor]` does not resolve to a valid type in the compilation.

**Fix:** Ensure the type is accessible and correctly spelled. Add the necessary `using` directive or assembly reference.

---

### EXP0014 -- Target member not found {#exp0014}

**Severity:** Error
**Category:** Design

**Message:**
```
No member '{0}' found on type '{1}' matching the stub's parameter signature
```

**Cause:** No member with the given name exists on the target type, or no overload matches the stub's parameter types.

**Fix:** Verify the member name (use `nameof(...)` to catch typos) and ensure the stub's parameters match the target's signature:

```csharp
// Error: wrong parameter types
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static double Clamp(int value, int min, int max) // should be double, not int
    => value < min ? min : (value > max ? max : value);

// Fixed: matching parameter types
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static double Clamp(double value, double min, double max)
    => value < min ? min : (value > max ? max : value);
```

---

### EXP0015 -- Return type mismatch {#exp0015}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveFor] return type mismatch for '{0}': target returns '{1}' but stub returns '{2}'
```

**Cause:** The stub method's return type does not match the target member's return type.

**Fix:** Align the return types:

```csharp
// Error: target Math.Clamp returns double, stub returns int
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static int Clamp(double value, double min, double max) => /* ... */;

// Fixed
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static double Clamp(double value, double min, double max) => /* ... */;
```

---

### EXP0016 -- Conflicts with `[Expressive]` {#exp0016}

**Severity:** Error
**Category:** Design

**Message:**
```
Target member '{0}' on type '{1}' already has [Expressive]; remove [ExpressiveFor] or [Expressive]
```

**Cause:** The target member already has its own `[Expressive]` attribute. `[ExpressiveFor]` is meant for members that cannot use `[Expressive]` directly.

**Fix:** Remove either `[ExpressiveFor]` (if the member's own `[Expressive]` is sufficient) or `[Expressive]` (if you want the external mapping to take precedence).

---

### EXP0017 -- Duplicate mapping {#exp0017}

**Severity:** Error
**Category:** Design

**Message:**
```
Duplicate [ExpressiveFor] mapping for member '{0}' on type '{1}'; only one stub per target member is allowed
```

**Cause:** Two or more `[ExpressiveFor]` stubs target the same member on the same type.

**Fix:** Remove the duplicate. Only one mapping per target member is allowed.

---

## `[ExpressiveProperty]` Diagnostics (EXP0018--EXP0022)

These diagnostics apply to `[ExpressiveProperty]` stubs, which ask the generator to emit a new property on the stub's containing partial type. See [`[ExpressiveProperty]` Attribute](./expressive-property) for the full feature reference.

::: info Replacing `[Expressive(Projectable = true)]`
`[ExpressiveProperty]` replaces the now-removed `[Expressive(Projectable = true)]`. The migration recipe is in [Migration from Projectables](../guide/migration-from-projectables#migrating-usememberbody).
:::

### EXP0018 -- Target name is already defined {#exp0018}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveProperty] target name '{0}' is already defined on '{1}' — rename the stub,
or use [ExpressiveFor(nameof({0}))] to map onto the existing member instead
```

**Cause:** The name passed to `[ExpressiveProperty]` already resolves to a member on the containing type. Synthesis would collide with the existing declaration.

**Fix:** Either rename the stub to pick a different target, or — if you want to bind to the existing property — drop `[ExpressiveProperty]` and switch to plain `[ExpressiveFor(nameof(X))]`:

```csharp
// Error: Amount already exists on the class
public decimal Amount { get; set; }

[ExpressiveProperty("Amount")]
private decimal AmountExpression => TotalAmount - Discount;

// Fixed: map onto the existing property with [ExpressiveFor]
[ExpressiveFor(nameof(Amount))]
private decimal AmountExpression => TotalAmount - Discount;
```

---

### EXP0019 -- Requires a partial containing type {#exp0019}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveProperty] requires the containing type '{0}' to be declared 'partial'
(applies to class, struct, and record)
```

**Cause:** Source generators can only add members to types declared `partial`. Synthesized properties are emitted as a separate partial declaration alongside the user's source.

**Fix:** Add the `partial` modifier:

```csharp
// Error
public class Account
{
    [ExpressiveProperty("Amount")]
    private decimal AmountExpression => TotalAmount - Discount;
}

// Fixed
public partial class Account
{
    [ExpressiveProperty("Amount")]
    private decimal AmountExpression => TotalAmount - Discount;
}
```

---

### EXP0020 -- Requires an expression-bodied property stub {#exp0020}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveProperty] must be placed on a property with an expression body '=> expr' —
accessor-list forms and method stubs are not supported
```

**Cause:** The attribute was placed on a method, an accessor-list property (`{ get => expr; }`), or a full `{ get; set; }` shape.

**Fix:** Rewrite the stub as a top-level expression-bodied property:

```csharp
// Error: method stub
[ExpressiveProperty("Amount")]
private decimal AmountExpression() => TotalAmount - Discount;

// Error: accessor-list form
[ExpressiveProperty("Amount")]
private decimal AmountExpression { get => TotalAmount - Discount; }

// Fixed: top-level expression body
[ExpressiveProperty("Amount")]
private decimal AmountExpression => TotalAmount - Discount;
```

---

### EXP0021 -- Requires an instance stub {#exp0021}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveProperty] is not supported on static stubs — stub '{0}' must be declared as
an instance member
```

**Cause:** The decorated stub is `static`. Synthesis is an instance-only feature.

**Fix:** Drop the `static` modifier. For a static computed value, use plain `[Expressive]` on a read-only member instead.

---

### EXP0022 -- Target shadows inherited member {#exp0022}

**Severity:** Error
**Category:** Design

**Message:**
```
[ExpressiveProperty] target name '{0}' shadows an inherited member on '{1}' — rename
the target to avoid silent hiding, or drop [ExpressiveProperty] and use [Expressive]
on an override
```

**Cause:** The target name matches a member inherited from a base class. Synthesizing a hidden member would silently shadow the base declaration, which is surprising and error-prone.

**Fix:** Either pick a different target name, or drop `[ExpressiveProperty]` and make the computed value an `override` decorated with plain `[Expressive]`.

---

## General Diagnostics (EXP0023--EXP0024)

### EXP0023 -- Unsupported operation ignored {#exp0023}

**Severity:** Warning
**Category:** Design

**Message:**
```
Expression contains an unsupported operation ({0}). The operation will be ignored and the
surrounding expression emitted without it.
```

**Cause:** The member body contains an operation that has no expression-tree equivalent but can be dropped without substituting a value — most commonly a string-interpolation **alignment or format specifier** (e.g. `$"{value,10}"` or `$"{value:N2}"`). The generator emits the surrounding interpolated string but does not honor the specifier.

**Fix:** If the formatting matters, compute the formatted value outside the `[Expressive]` body, or apply formatting in the consuming code after materialization. Unlike [EXP0008](#exp0008) — which substitutes a `default` value and can change results — `EXP0023` only drops the unsupported sub-operation; the surrounding expression is still emitted correctly.

---

### EXP0024 -- Virtual member will not dispatch polymorphically _(retired)_ {#exp0024}

**Status:** Retired. This warning no longer exists.

Virtual, `abstract`, and `override` `[Expressive]` members now **dispatch polymorphically at runtime**. When a virtual `[Expressive]` member is expanded for a query provider, `ExpressiveReplacer` discovers the derived `[Expressive]` overrides and emits a runtime type-test chain — `entity is Dog ? <Dog body> : <base body>` — which EF Core translates to a table-per-hierarchy discriminator `CASE`. Each row therefore uses its runtime type's body. See [Limitations: virtual and polymorphic members](../advanced/limitations#virtual-polymorphic-members).

The ID `EXP0024` is reserved (not reused). Its derived-side successor is [EXP0032](#exp0032), which flags a derived override that forgets `[Expressive]` and would silently fall back to the base body.

---

## Analyzer & `IExpressiveQueryable<T>` Diagnostics (EXP0025--EXP0029)

These diagnostics are emitted by the companion analyzers in `ExpressiveSharp.CodeFixers` (shipped with the `ExpressiveSharp` package). They flag places where `[Expressive]` rewriting silently won't apply.

### EXP0025 -- Referenced member could benefit from `[Expressive]` {#exp0025}

**Severity:** Warning
**Category:** Design
**Source:** `MissingExpressiveAnalyzer` (in `ExpressiveSharp.CodeFixers`)

**Message:**
```
Member '{0}' is referenced in an [Expressive] expression but is not marked [Expressive].
Adding [Expressive] would allow its body to be inlined into the expression tree.
```

**Cause:** A member referenced inside an `[Expressive]` body, an `ExpressionPolyfill.Create()` lambda, or an `IExpressiveQueryable` LINQ lambda has an expandable body (expression-bodied or block-bodied) but is not marked `[Expressive]`. Without the attribute, the member call remains opaque in the generated expression tree and cannot be translated by LINQ providers.

**Fix:** {#exp0025-fix}

The IDE offers a code fix that adds `[Expressive]` to the referenced member automatically (including the `using ExpressiveSharp;` directive if needed):

```csharp
// Warning: Total is referenced in an [Expressive] body but not marked [Expressive]
public double Total => Price * Quantity;

// Fixed: add [Expressive]
[Expressive]
public double Total => Price * Quantity;
```

::: tip
Enum method calls are excluded from this diagnostic -- the generator expands those automatically via per-value ternary chains, so `[Expressive]` is not needed on the enum extension method.
:::

---

### EXP0026 -- `IExpressiveQueryable<T>` LINQ method resolves to `Queryable` {#exp0026}

**Severity:** Warning
**Category:** Usage
**Source:** `MissingExpressiveImportAnalyzer` (in `ExpressiveSharp.CodeFixers`)

**Message:**
```
LINQ method '{0}' on IExpressiveQueryable<T> resolves to System.Linq.Queryable instead of the
ExpressiveSharp overload. Add 'using ExpressiveSharp;' to enable expression tree rewriting and
maintain the IExpressiveQueryable chain.
```

**Cause:** A LINQ method invoked on an `IExpressiveQueryable<T>` receiver bound to `System.Linq.Queryable` rather than the ExpressiveSharp delegate-based overload, because `using ExpressiveSharp;` is not imported. Without that import the chain silently degrades to plain `IQueryable<T>` and `[Expressive]` members are no longer rewritten.

**Fix:** {#exp0026-fix}

Add `using ExpressiveSharp;`. The IDE offers a code fix that inserts the directive automatically.

---

### EXP0027 -- No `IExpressiveQueryable<T>` overload for `Queryable` method {#exp0027}

**Severity:** Info
**Category:** Usage
**Source:** `MissingExpressiveImportAnalyzer` (in `ExpressiveSharp.CodeFixers`)

**Message:**
```
Method '{0}' from System.Linq.Queryable has no IExpressiveQueryable<T> overload. The result
will be IQueryable<T>, breaking the IExpressiveQueryable chain.
```

**Cause:** The called `Queryable` method has no ExpressiveSharp stub, so its result is plain `IQueryable<T>` and the expressive chain ends here. Unlike [EXP0026](#exp0026), this is not fixable by adding an import — no overload exists.

**Fix:** Re-establish the chain after the call with `.AsExpressive()` if you need `[Expressive]` rewriting downstream, or accept the plain `IQueryable<T>` if the remainder of the query needs no rewriting.

---

### EXP0028 -- Plain `IQueryable` chain references an `[Expressive]` member without `.AsExpressive()` {#exp0028}

**Severity:** Info
**Category:** Usage

**Message:**
```
LINQ method '{0}' on a plain IQueryable<T> references the [Expressive] member '{1}'.
Without .AsExpressive(), the member's body will not be inlined into the expression tree;
the provider may evaluate the call in memory or fail to translate it. Wrap the source
with .AsExpressive().
```

**Cause:** A LINQ method on a plain `IQueryable<T>` receiver (one that is not `IExpressiveQueryable<T>`) is invoked with a lambda whose body references an `[Expressive]` member. Because the chain is not expressive-aware, the source generator does not rewrite the lambda into an expression tree that inlines the member's body — the underlying query provider receives a call to the runtime delegate. Most providers cannot translate this and will either evaluate the call client-side (silent overfetch) or throw at execution time.

**Fix:** Wrap the chain root with `.AsExpressive()` so that subsequent LINQ methods flow through the ExpressiveSharp delegate-based overloads, which inline `[Expressive]` member bodies at compile time.

```csharp
// Before — IsAdult is silently evaluated on the client.
var adults = users.Where(u => u.IsAdult).ToList();

// After — IsAdult is inlined into the expression tree before the provider sees it.
var adults = users.AsExpressive().Where(u => u.IsAdult).ToList();
```

When you intentionally want to evaluate a member at runtime (e.g., it captures process state), mark the member with `[NotExpressive]` to suppress the diagnostic at every call site.

#### Code Fix: Wrap source with `.AsExpressive()` {#exp0028-fix}

The IDE offers a single code action: **Wrap source with `.AsExpressive()`**. It walks the LINQ chain to the leftmost non-LINQ expression, wraps it with `.AsExpressive()`, and inserts `using ExpressiveSharp;` if it is not already imported.

---

### EXP0029 -- `IExpressiveQueryable<T>` chain dropped to plain `IQueryable<T>` {#exp0029}

**Severity:** Info
**Category:** Usage

**Message:**
```
'{0}' returns IQueryable<T> from an IExpressiveQueryable<T> receiver, dropping the expressive
chain. Downstream LINQ skips ExpressiveSharp rewriting and [Expressive] members may evaluate
on the client. Add an IExpressiveQueryable<T>-typed overload of '{0}', wrap the result with
.AsExpressive(), or mark the method [NotExpressive] if the dropout is intentional.
```

**Cause:** A method invocation is being made on a receiver that implements `IExpressiveQueryable<T>`, but the method's return type is plain `IQueryable<T>` (or some derivative that is not also expressive). The chain loses its expressive type at this call site, and every downstream LINQ operation on the result skips ExpressiveSharp's rewrite step — `[Expressive]` members in subsequent `Where` / `Select` / `Include` / etc. fall back to runtime delegate invocation, which most providers cannot translate and will evaluate on the client.

The diagnostic fires once, at the dropout point itself, regardless of how many further calls follow.

**Common dropout shapes:**

```csharp
// User-defined helper typed on plain IQueryable<T> — drops the chain.
public static IQueryable<T> Filter<T>(this IQueryable<T> source) => source.Where(...);

db.Orders.AsExpressiveDbSet().Filter()  // ⚠ EXP0029 fires on .Filter()
    .Include(o => o.Customer)           // chain is plain from here on
    .ToList();
```

**Fix (preferred):** Add a sibling overload typed on `IExpressiveQueryable<T>` that returns `IExpressiveQueryable<T>`:

```csharp
public static IExpressiveQueryable<T> Filter<T>(this IExpressiveQueryable<T> source)
    => source.Where(...);
```

**Fix (when the helper can't be modified):** wrap the result with `.AsExpressive()` to restore the chain:

```csharp
db.Orders.AsExpressiveDbSet()
    .ThirdPartyHelper()      // returns plain IQueryable<Order>
    .AsExpressive()          // re-wrap
    .Include(o => o.Customer);
```

**Exemptions:**

- `.AsQueryable()` — the standard explicit downcast — is sanctioned and never reported.
- Marking the offending method with `[NotExpressive]` suppresses the diagnostic at every call site, for cases where the dropout is intentional (the helper performs work that genuinely needs to run on the client).

---

### EXP0032 -- Override of an `[Expressive]` member is missing `[Expressive]` {#exp0032}

**Severity:** Warning
**Category:** Design

**Message:**
```
'{0}' overrides an [Expressive] member but is not itself marked [Expressive]. In expression-tree
expansion (e.g. EF Core, MongoDB) instances of this type fall back to the base body instead of
this override. Add [Expressive] so it participates in polymorphic dispatch, or [NotExpressive] to
silence this.
```

**Cause:** A `virtual`/`abstract` `[Expressive]` member is overridden, but the override is **not** itself marked `[Expressive]`. Runtime polymorphic dispatch can only inline overrides that are registered as `[Expressive]`, so for instances of the overriding type the query silently falls back to the **base** body — almost always a bug. The check walks *up* the override chain (cheap and cross-assembly), unlike the derived-type discovery that expansion performs at runtime.

**Fix:** Add `[Expressive]` to the override (the [Add `[Expressive]`](#exp0025-fix) code fix does this), so it participates in dispatch:

```csharp
class Animal
{
    [Expressive] public virtual string Description => "Animal: " + Name;
}

class Dog : Animal
{
    public override string Description => "Dog: " + Name;   // ⚠ EXP0032
    // Fix:
    [Expressive] public override string Description => "Dog: " + Name;
}
```

If the override intentionally should not be translated (it stays client-only), mark it `[NotExpressive]` to silence the diagnostic.

---

## Window Function Diagnostics (EXP0030--EXP0031)

These diagnostics are emitted by the `WindowFunctionLiteralArgsAnalyzer` in the `ExpressiveSharp.EntityFrameworkCore.CodeFixers` package (shipped with `ExpressiveSharp.EntityFrameworkCore`). They validate constant literal arguments to `WindowFunction.*` calls before they reach the database. Only compile-time constant arguments are checked; a variable count or offset is never flagged. See [Window Functions](../guide/window-functions) for the full feature reference.

### EXP0030 -- `WindowFunction.Ntile` requires a positive bucket count {#exp0030}

**Severity:** Warning
**Category:** Usage

**Message:**
```
WindowFunction.Ntile requires a positive bucket count; literal value {0} produces invalid SQL
```

**Cause:** `NTILE(n)` divides ordered rows into `n` buckets, so SQL requires `n >= 1`. A literal `0` or negative bucket count raises a database error at execution time.

**Fix:** Pass a positive literal bucket count:

```csharp
// Warning: EXP0030
WindowFunction.Ntile(0, Window.OrderBy(x => x.Score));

// Fixed
WindowFunction.Ntile(4, Window.OrderBy(x => x.Score));
```

---

### EXP0031 -- `WindowFunction.Lag`/`Lead` offset must be non-negative {#exp0031}

**Severity:** Warning
**Category:** Usage

**Message:**
```
WindowFunction.{0} offset must be non-negative; literal value {1} is rejected during EF translation
```

**Cause:** `LAG` and `LEAD` offsets count rows backward or forward from the current row; SQL requires the offset to be `>= 0`. A negative literal offset is rejected during EF translation.

**Fix:** Use a non-negative offset — swap `Lag` for `Lead` (or vice versa) instead of negating:

```csharp
// Warning: EXP0031
WindowFunction.Lag(x.Value, -1, window);

// Fixed
WindowFunction.Lead(x.Value, 1, window);
```

---

## Migration Diagnostics (EXP1001--EXP1003)

These diagnostics are emitted by the `MigrationAnalyzer` in the `ExpressiveSharp.EntityFrameworkCore.CodeFixers` package. They detect usage of the legacy `EntityFrameworkCore.Projectables` library and offer automated code fixes to migrate to ExpressiveSharp.

### EXP1001 -- Replace `[Projectable]` with `[Expressive]` {#exp1001}

**Severity:** Warning
**Category:** Migration

**Message:**
```
[Projectable] should be replaced with [Expressive] from ExpressiveSharp
```

**Cause:** The code uses `[Projectable]` from `EntityFrameworkCore.Projectables`, which has been superseded by `[Expressive]`.

**Fix:** {#exp1001-fix}

The IDE code fix replaces the attribute automatically. Only the `Transformers` property is preserved (all other properties like `NullConditionalRewriteSupport`, `ExpandEnumMethods`, and `UseMemberBody` have no equivalent and are removed):

```csharp
// Before
[Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
public string? FullAddress => Location?.AddressLine1;

// After (code fix applied)
[Expressive]
public string? FullAddress => Location?.AddressLine1;
```

---

### EXP1002 -- Replace `UseProjectables()` with `UseExpressives()` {#exp1002}

**Severity:** Warning
**Category:** Migration

**Message:**
```
UseProjectables() should be replaced with UseExpressives() from ExpressiveSharp
```

**Cause:** The code calls `UseProjectables()` to configure EF Core, which has been superseded by `UseExpressives()`.

**Fix:** {#exp1002-fix}

The IDE code fix replaces the method call and removes any configuration callback argument:

```csharp
// Before
options.UseProjectables(p => p.CompatibilityMode(CompatibilityMode.Limited));

// After (code fix applied)
options.UseExpressives();
```

---

### EXP1003 -- Replace Projectables namespace {#exp1003}

**Severity:** Warning
**Category:** Migration

**Message:**
```
Namespace '{0}' should be replaced with the ExpressiveSharp equivalent
```

**Cause:** The code has a `using` directive for an `EntityFrameworkCore.Projectables` namespace.

**Fix:** {#exp1003-fix}

The IDE code fix replaces the namespace:

| Old Namespace | New Namespace |
|---|---|
| `EntityFrameworkCore.Projectables` | `ExpressiveSharp` |
| `EntityFrameworkCore.Projectables.Extensions` | `ExpressiveSharp` |
| `EntityFrameworkCore.Projectables.Infrastructure` | *(removed -- no equivalent)* |

---

## Suppressing Diagnostics

Individual warnings can be suppressed with standard C# pragma directives:

```csharp
#pragma warning disable EXP0008
[Expressive]
public int Value => UnsupportedOperation();
#pragma warning restore EXP0008
```

Or via `.editorconfig` / `Directory.Build.props`:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);EXP0008</NoWarn>
</PropertyGroup>
```
