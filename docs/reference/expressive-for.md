# `[ExpressiveFor]` Mapping

The `[ExpressiveFor]` attribute lets you provide expression-tree bodies for members on types you do not own -- BCL methods, third-party library members, or your own types that cannot use `[Expressive]` directly. This enables those members to be used in EF Core queries and other LINQ providers that would otherwise fail with "could not be translated".

## Namespace

```csharp
using ExpressiveSharp.Mapping;
```

## How It Works

You write a stub member -- a method **or** a property -- whose body defines the expression-tree replacement. The `[ExpressiveFor]` attribute tells the generator which external member this stub maps to. At runtime, the replacer substitutes calls to the target member with the stub's expression tree -- call sites remain unchanged.

## Mapping Rules

- The stub can be a **method** (receiver supplied as the first parameter for instance targets, or `this` for instance stubs on the target type) **or** a **property** (parameterless; `this` is the receiver for instance stubs).
- The single-argument form `[ExpressiveFor(nameof(X))]` is shorthand for `[ExpressiveFor(typeof(ContainingType), nameof(X))]` -- use it when the target member is on the same type as the stub.
- For **static methods** (and static stubs over static members), the stub's parameters must match the target method's parameters exactly.
- For **instance methods** with a `static` stub, the first parameter of the stub is the receiver (`this`), followed by the target method's parameters.
- For **instance methods** with an `instance` stub on the target type, `this` is the receiver; remaining parameters match the target's exactly.
- For **instance properties** with a `static` method stub, the stub takes a single parameter: the receiver.
- For **instance properties** with an `instance` method or property stub on the target type, the stub is parameterless.
- For **static properties**, the stub is parameterless.
- Property stubs can only target other properties (no parameters to carry method arguments).
- The return type / property type must match (EXP0017 if not).
- Constructor stubs (`[ExpressiveForConstructor]`) must still be `static` methods; instance or property ctor stubs have no coherent meaning.

## Static Method Mapping

Map a static method by matching its parameter signature:

::: expressive-sample
db.Orders.Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
---setup---
public static class MathMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp))]
    public static int ClampInt(int value, int min, int max)
        => value < min ? min : (value > max ? max : value);
}
:::

## Instance Method Mapping

For instance methods, the first parameter represents the receiver:

::: expressive-sample
db.Products.Where(p => p.Name.Contains("box"))
---setup---
public static class StringMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.Contains))]
    public static bool Contains(string self, string value)
        => self.IndexOf(value) >= 0;
}
:::

## Instance Property Mapping

For instance properties, the stub takes a single parameter (the instance):

```csharp
static class EntityMappings
{
    [ExpressiveFor(typeof(MyType), nameof(MyType.FullName))]
    static string FullName(MyType obj)
        => obj.FirstName + " " + obj.LastName;
}
```

## Co-located Form (Instance Stub + Single-argument Attribute)

When the target is on the same type as the stub, the most ergonomic form combines an **instance stub** with the **single-argument** attribute. `this` is the receiver automatically. Use this form when a property has its own backing storage -- e.g. a plain settable auto-property used for DTO shape, serialization, or in-memory assignment in tests -- but queries should still compute it from other columns.

A **property stub** is often the cleanest choice for this (no parentheses, reads like the target it replaces):

```csharp
public class Person
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    // Regular auto-property — assignable directly (for DTOs, tests, deserialization).
    public string FullName { get; set; } = "";

    // When FullName appears in a LINQ expression tree, it is rewritten to this body,
    // so EF Core projects it from FirstName/LastName instead of mapping it to its own column.
    [ExpressiveFor(nameof(FullName))]
    private string FullNameExpression => FirstName + " " + LastName;
}
```

A **method stub** is equivalent in behaviour and appropriate when the target is a method or when you need a block body:

```csharp
[ExpressiveFor(nameof(FullName))]
private string FullNameExpression() => FirstName + " " + LastName;
```

Both forms are equivalent to the verbose `[ExpressiveFor(typeof(Person), nameof(Person.FullName))] static string FullName(Person obj) => obj.FirstName + " " + obj.LastName;` but reuse `this` instead of threading a receiver parameter. When the EF Core integration is enabled, both the target property **and** the stub property itself are automatically excluded from the model (no `[NotMapped]` needed -- see [Automatic NotMapped for `[ExpressiveFor]` targets](#automatic-notmapped-for-expressivefor-targets)).

::: warning When to prefer `[Expressive]` instead
If the property has no backing storage and the same body works at both runtime and query time, put `[Expressive]` directly on it (`[Expressive] public string FullName => FirstName + " " + LastName;`) and skip the stub. `[ExpressiveFor]` is for the dual-body case; `[Expressive]` is for the single-body case.
:::

::: tip
The stub can use any C# syntax that `[Expressive]` supports -- switch expressions, pattern matching, null-conditional operators, and more.
:::

## Automatic NotMapped for `[ExpressiveFor]` targets

When `UseExpressives()` is active, EF Core's model builder automatically ignores properties that are:

1. Decorated with `[Expressive]`,
2. Decorated with `[ExpressiveFor]` (a property stub itself), **or**
3. The target of an `[ExpressiveFor]` stub anywhere in the loaded assemblies.

You do not need to add `[NotMapped]` to a property you are expressing externally or using as a property stub -- the `ExpressivePropertiesNotMappedConvention` detects these cases via attribute metadata and the generated registry and calls `Ignore()` for you.

## Constructor Mapping with `[ExpressiveForConstructor]`

Use `[ExpressiveForConstructor]` to provide an expression-tree body for a constructor on a type you do not own:

```csharp
public static class MyDtoBuilder
{
    // Applied to a static stub method that returns the target type — the
    // generator replaces `new MyDto(id, name)` call sites with the stub's body.
    [ExpressiveForConstructor(typeof(MyDto))]
    public static MyDto Build(int id, string name)
        => new MyDto { Id = id, Name = name };
}
```

## Synthesizing a property with `Synthesize = true`

When you want the target property to exist **purely as an expressive-backed projection** -- no manual declaration, no backing storage to wire up, but still settable so projection middleware (EF Core materialization, HotChocolate's `ProjectTo`, AutoMapper, Mapperly) can populate it from query results -- set `Synthesize = true` on the single-argument form. The generator declares the property for you inside a partial class and wires its accessors to the stub:

```csharp
public partial class Account
{
    public decimal? TotalAmount { get; set; }
    public decimal? Discount    { get; set; }

    // The target property Amount is NOT declared here — the generator emits it.
    [ExpressiveFor("Amount", Synthesize = true)]
    private decimal? AmountExpression =>
        TotalAmount != null && Discount != null
            ? TotalAmount.Value - Discount.Value
            : null;
}
```

The generator produces the following partial declaration, which the C# compiler merges with your class:

```csharp
// <auto-generated/>
namespace YourNamespace
{
    partial class Account
    {
        private decimal? _amount;
        private bool _amountHasValue;
        public decimal? Amount
        {
            get => _amountHasValue ? _amount : AmountExpression;
            init
            {
                _amountHasValue = true;
                _amount = value;
            }
        }
    }
}
```

### Shape selection

The generator picks between two shapes based on the target type's nullability:

- **Coalesce shape** (non-nullable targets -- `string`, `decimal`, `int`, ...): `get => _field ?? stub;`, with a nullable backing field. Minimal overhead; `null` unambiguously means "not yet materialized."
- **Ternary + flag shape** (nullable targets -- `string?`, `decimal?`, `int?`, ...): `get => _hasValue ? _field : stub;`, with a separate `bool` flag. Required because stored `null` is a legitimate value that must be distinguished from "not materialized."

### Requirements

- Use the single-argument form `[ExpressiveFor("Name", Synthesize = true)]`. The two-argument `typeof(...)` form is rejected with **EXP0033**; `Synthesize` always targets the stub's containing type.
- Supply the target name as a **string literal**, not `nameof(Name)` -- because `Name` is declared by the generator, `nameof(Name)` fails to resolve during the initial compilation pass.
- The containing type must be declared `partial` (**EXP0032**) so the generator can add the property declaration.
- The target name must not already exist on the containing type (**EXP0031**) -- that would be an ambiguous conflict with a user-written member.
- The stub must be a parameterless instance member (property or method) on the same type.

### How it interacts with providers

Because the stub flows through the normal `[ExpressiveFor]` pipeline, the registry is keyed on the **synthesized property's getter**. At query time, `ExpressiveReplacer` rewrites references to `Amount` with the stub's formula -- exactly as if you had written `[ExpressiveFor(nameof(Amount))]` against a manually-declared `Amount` property. The difference is purely who writes the property declaration.

For EF Core and Mongo, the synthesized property is automatically excluded from mapping by `ExpressivePropertiesNotMappedConvention` and `ExpressiveMongoIgnoreConvention` -- no `[NotMapped]` attribute needed.

## Properties

Both `[ExpressiveFor]` and `[ExpressiveForConstructor]` support the same optional properties as `[Expressive]`, plus `Synthesize`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowBlockBody` | `bool` | `false` | Enables block-bodied stubs (`if`/`else`, local variables, etc.) |
| `Transformers` | `Type[]?` | `null` | Per-mapping transformers applied when expanding the mapped member |
| `Synthesize` | `bool` | `false` | Generates the target property on the stub's containing type (see [Synthesizing a property](#synthesizing-a-property-with-synthesize-true)). `[ExpressiveFor]` only. |

::: expressive-sample
db.Orders.Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
---setup---
public static class MathBlockMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp), AllowBlockBody = true)]
    public static int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
:::

## Diagnostics

The following diagnostics are specific to `[ExpressiveFor]` and `[ExpressiveForConstructor]`:

| Code | Severity | Description |
|------|----------|-------------|
| [EXP0014](./diagnostics#exp0014) | Error | Target type specified in `[ExpressiveFor]` could not be resolved |
| [EXP0015](./diagnostics#exp0015) | Error | No member with the given name found on the target type matching the stub's parameter signature |
| [EXP0017](./diagnostics#exp0017) | Error | Return type of the stub does not match the target member's return type |
| [EXP0019](./diagnostics#exp0019) | Error | The target member already has `[Expressive]` -- remove one of the two attributes |
| [EXP0020](./diagnostics#exp0020) | Error | Duplicate mapping -- only one stub per target member is allowed |
| [EXP0031](./diagnostics#exp0031) | Error | `Synthesize = true` target name is already defined on the containing type |
| [EXP0032](./diagnostics#exp0032) | Error | `Synthesize = true` requires the containing type to be declared `partial` |
| [EXP0033](./diagnostics#exp0033) | Error | `Synthesize = true` must use the single-argument form, not `typeof(...)` |

::: warning
If a member already has `[Expressive]`, adding `[ExpressiveFor]` targeting it is a compile error (EXP0019). `[ExpressiveFor]` is only for members that do not have `[Expressive]`.
:::

## Complete Usage Example

::: expressive-sample
db.Orders
    .Where(o => !string.IsNullOrWhiteSpace(o.Customer.Name))
    .Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
    .Select(o => new OrderMappingDto(o.Id, o.Customer.Name ?? "N/A"))
---setup---
public static class MathMappingsComplete
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp))]
    public static int ClampInt(int value, int min, int max)
        => value < min ? min : (value > max ? max : value);
}

public static class StringMappingsComplete
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    public static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}

public class OrderMappingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // The constructor that call sites (new OrderMappingDto(id, name)) invoke.
    public OrderMappingDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public static class OrderMappingDtoBuilder
{
    // Provides a translatable body for the constructor above — call sites
    // `new OrderMappingDto(id, name)` are rewritten to this object-init form
    // during expression-tree expansion, so the provider sees a translatable
    // MemberInit instead of a constructor call.
    [ExpressiveSharp.Mapping.ExpressiveForConstructor(typeof(OrderMappingDto))]
    public static OrderMappingDto Build(int id, string name)
        => new OrderMappingDto(0, "") { Id = id, Name = name };
}
:::

All three mapped members are replaced with their expression-tree equivalents and translated for your provider. No changes are needed at call sites.
