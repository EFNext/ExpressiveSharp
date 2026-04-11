---
url: 'https://efnext.github.io/ExpressiveSharp/reference/diagnostics.md'
---
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
| [EXP0013](#exp0013) | Warning | Member could benefit from `[Expressive]` | [Add `[Expressive]`](#exp0013-fix) |
| [EXP0014](#exp0014) | Error | `[ExpressiveFor]` target type not found | -- |
| [EXP0015](#exp0015) | Error | `[ExpressiveFor]` target member not found | -- |
| [EXP0016](#exp0016) | Error | `[ExpressiveFor]` stub must be static | -- |
| [EXP0017](#exp0017) | Error | `[ExpressiveFor]` return type mismatch | -- |
| [EXP0019](#exp0019) | Error | `[ExpressiveFor]` conflicts with `[Expressive]` | -- |
| [EXP0020](#exp0020) | Error | Duplicate `[ExpressiveFor]` mapping | -- |
| [EXP1001](#exp1001) | Warning | Replace `[Projectable]` with `[Expressive]` | [Replace attribute](#exp1001-fix) |
| [EXP1002](#exp1002) | Warning | Replace `UseProjectables()` with `UseExpressives()` | [Replace method call](#exp1002-fix) |
| [EXP1003](#exp1003) | Warning | Replace Projectables namespace | [Replace namespace](#exp1003-fix) |

***

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
public string FullName => FirstName + " " + LastName;
```

***

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
        Name = c.FirstName + " " + c.LastName;
    }
}
```

***

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

***

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

***

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

***

### EXP0006 -- Unsupported statement in block body {#exp0006}

**Severity:** Warning
**Category:** Design

**Message:**

```
Method '{0}' contains an unsupported statement: {1}
```

**Cause:** A block-bodied `[Expressive]` member contains a statement type that cannot be converted to an expression tree (e.g., `while` loops, `try`/`catch`, `throw`, `async`/`await`).

**Fix:** Refactor to use only supported constructs (`if`/`else`, `switch`, `foreach`, local variables, `return`), or convert to an expression-bodied member.

***

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

***

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

***

### EXP0009 -- Unsupported operator {#exp0009}

**Severity:** Warning
**Category:** Design

**Message:**

```
Operator '{0}' is not supported in expression trees. A default value will be used instead.
```

**Cause:** An operator in the expression body has no expression tree equivalent.

**Fix:** Rewrite using a supported operator or method call.

***

### EXP0010 -- Interceptor emission failed {#exp0010}

**Severity:** Warning
**Category:** Design

**Message:**

```
Failed to generate interceptor for call site: {0}. The original delegate stub will be used at runtime.
```

**Cause:** The polyfill interceptor generator could not produce an interceptor for a specific call site (e.g., on `IExpressiveQueryable<T>` or `ExpressionPolyfill.Create`). The original delegate-based stub will be used instead.

**Fix:** This is typically an internal generator issue. If you encounter it, check that the call site uses supported syntax and consider filing an issue.

***

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

***

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
    Name = c.FirstName + " " + c.LastName
};

// After: expressive constructor
[Expressive]
public CustomerDto(Customer c)
{
    Id = c.Id;
    Name = c.FirstName + " " + c.LastName;
}
```

***

## Analyzer Diagnostic (EXP0013)

### EXP0013 -- Member could benefit from `[Expressive]` {#exp0013}

**Severity:** Warning
**Category:** Design
**Source:** `MissingExpressiveAnalyzer` (in `ExpressiveSharp.CodeFixers`)

**Message:**

```
Member '{0}' is referenced in an [Expressive] expression but is not marked [Expressive].
Adding [Expressive] would allow its body to be inlined into the expression tree.
```

**Cause:** A member referenced inside an `[Expressive]` body, an `ExpressionPolyfill.Create()` lambda, or an `IExpressiveQueryable` LINQ lambda has an expandable body (expression-bodied or block-bodied) but is not marked `[Expressive]`. Without the attribute, the member call remains opaque in the generated expression tree and cannot be translated by LINQ providers.

**Fix:** {#exp0013-fix}

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

***

## External Mapping Diagnostics (EXP0014--EXP0020)

These diagnostics are specific to `[ExpressiveFor]` and `[ExpressiveForConstructor]`. See [`[ExpressiveFor]` Mapping](./expressive-for) for full usage details.

### EXP0014 -- Target type not found {#exp0014}

**Severity:** Error
**Category:** Design

**Message:**

```
[ExpressiveFor] target type '{0}' could not be resolved
```

**Cause:** The `Type` argument passed to `[ExpressiveFor]` does not resolve to a valid type in the compilation.

**Fix:** Ensure the type is accessible and correctly spelled. Add the necessary `using` directive or assembly reference.

***

### EXP0015 -- Target member not found {#exp0015}

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

***

### EXP0016 -- Stub must be static {#exp0016}

**Severity:** Error
**Category:** Design

**Message:**

```
[ExpressiveFor] stub method '{0}' must be static
```

**Cause:** The stub method is not declared `static`.

**Fix:** Add the `static` modifier:

```csharp
// Error: not static
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
double Clamp(double value, double min, double max) => /* ... */;

// Fixed
[ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static double Clamp(double value, double min, double max) => /* ... */;
```

***

### EXP0017 -- Return type mismatch {#exp0017}

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

***

### EXP0019 -- Conflicts with `[Expressive]` {#exp0019}

**Severity:** Error
**Category:** Design

**Message:**

```
Target member '{0}' on type '{1}' already has [Expressive]; remove [ExpressiveFor] or [Expressive]
```

**Cause:** The target member already has its own `[Expressive]` attribute. `[ExpressiveFor]` is meant for members that cannot use `[Expressive]` directly.

**Fix:** Remove either `[ExpressiveFor]` (if the member's own `[Expressive]` is sufficient) or `[Expressive]` (if you want the external mapping to take precedence).

***

### EXP0020 -- Duplicate mapping {#exp0020}

**Severity:** Error
**Category:** Design

**Message:**

```
Duplicate [ExpressiveFor] mapping for member '{0}' on type '{1}'; only one stub per target member is allowed
```

**Cause:** Two or more `[ExpressiveFor]` stubs target the same member on the same type.

**Fix:** Remove the duplicate. Only one mapping per target member is allowed.

***

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

***

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

***

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

***

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
