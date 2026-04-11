---
url: 'https://efnext.github.io/ExpressiveSharp/reference/expressive-for.md'
---
# `[ExpressiveFor]` Mapping

The `[ExpressiveFor]` attribute lets you provide expression-tree bodies for members on types you do not own -- BCL methods, third-party library members, or your own types that cannot use `[Expressive]` directly. This enables those members to be used in EF Core queries and other LINQ providers that would otherwise fail with "could not be translated".

## Namespace

```csharp
using ExpressiveSharp.Mapping;
```

## How It Works

You write a static stub method whose body defines the expression-tree replacement. The `[ExpressiveFor]` attribute tells the generator which external member this stub maps to. At runtime, the replacer substitutes calls to the target member with the stub's expression tree -- call sites remain unchanged.

## Mapping Rules

* The stub method **must be `static`** (EXP0016 if not).
* For **static methods**, the stub's parameters must match the target method's parameters exactly.
* For **instance methods**, the first parameter of the stub is the receiver (`this`), followed by the target method's parameters.
* For **instance properties**, the stub takes a single parameter: the receiver.
* The return type must match (EXP0017 if not).

## Static Method Mapping

Map a static method by matching its parameter signature:

```csharp
static class MathMappings
{
    [ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);
}
```

Now `Math.Clamp` can be used in queries:

```csharp
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => Math.Clamp(o.Price, 20, 100) > 50)
    .ToList();
```

Generated SQL:

```sql
SELECT "o"."Id", "o"."Price", "o"."Quantity"
FROM "Orders" AS "o"
WHERE CASE
    WHEN "o"."Price" < 20.0 THEN 20.0
    WHEN "o"."Price" > 100.0 THEN 100.0
    ELSE "o"."Price"
END > 50.0
```

## Instance Method Mapping

For instance methods, the first parameter represents the receiver:

```csharp
static class StringMappings
{
    [ExpressiveFor(typeof(string), nameof(string.Contains))]
    static bool Contains(string self, string value)
        => self.IndexOf(value) >= 0;
}
```

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

::: tip
The stub can use any C# syntax that `[Expressive]` supports -- switch expressions, pattern matching, null-conditional operators, and more.
:::

## Constructor Mapping with `[ExpressiveForConstructor]`

Use `[ExpressiveForConstructor]` to provide an expression-tree body for a constructor on a type you do not own:

```csharp
[ExpressiveForConstructor(typeof(MyDto))]
static MyDto Create(int id, string name) => new MyDto { Id = id, Name = name };
```

## Properties

Both `[ExpressiveFor]` and `[ExpressiveForConstructor]` support the same optional properties as `[Expressive]`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowBlockBody` | `bool` | `false` | Enables block-bodied stubs (`if`/`else`, local variables, etc.) |
| `Transformers` | `Type[]?` | `null` | Per-mapping transformers applied when expanding the mapped member |

```csharp
[ExpressiveFor(typeof(Math), nameof(Math.Clamp), AllowBlockBody = true)]
static double Clamp(double value, double min, double max)
{
    if (value < min) return min;
    if (value > max) return max;
    return value;
}
```

## Diagnostics

The following diagnostics are specific to `[ExpressiveFor]` and `[ExpressiveForConstructor]`:

| Code | Severity | Description |
|------|----------|-------------|
| [EXP0014](./diagnostics#exp0014) | Error | Target type specified in `[ExpressiveFor]` could not be resolved |
| [EXP0015](./diagnostics#exp0015) | Error | No member with the given name found on the target type matching the stub's parameter signature |
| [EXP0016](./diagnostics#exp0016) | Error | The stub method must be `static` |
| [EXP0017](./diagnostics#exp0017) | Error | Return type of the stub does not match the target member's return type |
| [EXP0019](./diagnostics#exp0019) | Error | The target member already has `[Expressive]` -- remove one of the two attributes |
| [EXP0020](./diagnostics#exp0020) | Error | Duplicate mapping -- only one stub per target member is allowed |

::: warning
If a member already has `[Expressive]`, adding `[ExpressiveFor]` targeting it is a compile error (EXP0019). `[ExpressiveFor]` is only for members that do not have `[Expressive]`.
:::

## Complete Usage Example

```csharp
using ExpressiveSharp.Mapping;

// Map Math.Clamp for SQL translation
static class MathMappings
{
    [ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);
}

// Map string.IsNullOrWhiteSpace for SQL translation
static class StringMappings
{
    [ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}

// Map a third-party DTO constructor
static class DtoMappings
{
    [ExpressiveForConstructor(typeof(ExternalDto))]
    static ExternalDto Create(int id, string name)
        => new ExternalDto { Id = id, Name = name };
}
```

Using the mappings in an EF Core query:

```csharp
var results = db.Orders
    .AsExpressiveDbSet()
    .Where(o => !string.IsNullOrWhiteSpace(o.Tag))
    .Where(o => Math.Clamp(o.Price, 20, 100) > 50)
    .Select(o => new ExternalDto(o.Id, o.Tag ?? "N/A"))
    .ToList();
```

All three mapped members are replaced with their expression-tree equivalents and translated to SQL. No changes are needed at call sites.
