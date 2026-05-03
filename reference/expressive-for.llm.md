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

```csharp
db
    .Orders
    .Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)

// Setup
public static class MathMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp))]
    public static int ClampInt(int value, int min, int max)
        => value < min ? min : (value > max ? max : value);
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", "o"."CustomerId", "o"."PlacedAt", "o"."Status"
FROM "Orders" AS "o"
WHERE CASE
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId") < 0 THEN 0
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId") > 100 THEN 100
    ELSE (
        SELECT COUNT(*)
        FROM "LineItems" AS "l1"
        WHERE "o"."Id" = "l1"."OrderId")
END > 5
```


## Instance Method Mapping

For instance methods, the first parameter represents the receiver:

```csharp
db
    .Products
    .Where(p => p.Name.Contains("box"))

// Setup
public static class StringMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.Contains))]
    public static bool Contains(string self, string value)
        => self.IndexOf(value) >= 0;
}
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Id", "p"."Category", "p"."ListPrice", "p"."Name", "p"."StockQuantity"
FROM "Products" AS "p"
WHERE instr("p"."Name", 'box') - 1 >= 0
```


## Instance Property Mapping

For instance properties, the stub takes a single parameter (the instance):

```csharp
static class EntityMappings
{
    [ExpressiveFor(typeof(MyType), nameof(MyType.FullName))]
    static string FullName(MyType obj)
        => $"{obj.FirstName} {obj.LastName}";
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
    private string FullNameExpression => $"{FirstName} {LastName}";
}
```

A **method stub** is equivalent in behaviour and appropriate when the target is a method or when you need a block body:

```csharp
[ExpressiveFor(nameof(FullName))]
private string FullNameExpression() => $"{FirstName} {LastName}";
```

Both forms are equivalent to the verbose `[ExpressiveFor(typeof(Person), nameof(Person.FullName))] static string FullName(Person obj) => $"{obj.FirstName} {obj.LastName}";` but reuse `this` instead of threading a receiver parameter. When the EF Core integration is enabled, both the target property **and** the stub property itself are automatically excluded from the model (no `[NotMapped]` needed -- see [Automatic NotMapped for `[ExpressiveFor]` targets](#automatic-notmapped-for-expressivefor-targets)).

::: warning When to prefer `[Expressive]` instead
If the property has no backing storage and the same body works at both runtime and query time, put `[Expressive]` directly on it (`[Expressive] public string FullName => $"{FirstName} {LastName}";`) and skip the stub. `[ExpressiveFor]` is for the dual-body case; `[Expressive]` is for the single-body case.
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

## Synthesizing a new property

`[ExpressiveFor]` maps onto an **existing** member. If the target property does not yet exist and you want the generator to declare it for you (for example, so HotChocolate or EF Core projection middleware can bind to a settable member), use [`[ExpressiveProperty]`](./expressive-property) instead — it's the focused attribute for that case.

## Properties

Both `[ExpressiveFor]` and `[ExpressiveForConstructor]` support the same optional properties as `[Expressive]`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowBlockBody` | `bool` | `false` | Enables block-bodied stubs (`if`/`else`, local variables, etc.) |
| `Transformers` | `Type[]?` | `null` | Per-mapping transformers applied when expanding the mapped member |

```csharp
db
    .Orders
    .Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", "o"."CustomerId", "o"."PlacedAt", "o"."Status"
FROM "Orders" AS "o"
WHERE CASE
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId") < 0 THEN 0
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId") > 100 THEN 100
    ELSE (
        SELECT COUNT(*)
        FROM "LineItems" AS "l1"
        WHERE "o"."Id" = "l1"."OrderId")
END > 5
```


## Diagnostics

The following diagnostics are specific to `[ExpressiveFor]` and `[ExpressiveForConstructor]`:

| Code | Severity | Description |
|------|----------|-------------|
| [EXP0014](./diagnostics#exp0014) | Error | Target type specified in `[ExpressiveFor]` could not be resolved |
| [EXP0015](./diagnostics#exp0015) | Error | No member with the given name found on the target type matching the stub's parameter signature |
| [EXP0017](./diagnostics#exp0017) | Error | Return type of the stub does not match the target member's return type |
| [EXP0019](./diagnostics#exp0019) | Error | The target member already has `[Expressive]` -- remove one of the two attributes |
| [EXP0020](./diagnostics#exp0020) | Error | Duplicate mapping -- only one stub per target member is allowed |

::: warning
If a member already has `[Expressive]`, adding `[ExpressiveFor]` targeting it is a compile error (EXP0019). `[ExpressiveFor]` is only for members that do not have `[Expressive]`.
:::

## Complete Usage Example

```csharp
db
    .Orders
    .Where(o => !string.IsNullOrWhiteSpace(o.Customer.Name))
    .Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
    .Select(o => new OrderMappingDto(o.Id, o.Customer.Name ?? "N/A"))

// Setup
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
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", "c"."Name"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
WHERE length(trim("c"."Name")) <> 0 AND CASE
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l"
        WHERE "o"."Id" = "l"."OrderId") < 0 THEN 0
    WHEN (
        SELECT COUNT(*)
        FROM "LineItems" AS "l0"
        WHERE "o"."Id" = "l0"."OrderId") > 100 THEN 100
    ELSE (
        SELECT COUNT(*)
        FROM "LineItems" AS "l1"
        WHERE "o"."Id" = "l1"."OrderId")
END > 5
```


All three mapped members are replaced with their expression-tree equivalents and translated for your provider. No changes are needed at call sites.
