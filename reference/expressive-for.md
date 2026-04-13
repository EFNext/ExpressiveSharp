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

::: expressive-sample
db.Orders.Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
\---setup---
public static class MathMappings
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp))]
public static int ClampInt(int value, int min, int max)
\=> value < min ? min : (value > max ? max : value);
}
:::

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

**Generated SQL:**

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

::: expressive-sample
db.Products.Where(p => p.Name.Contains("box"))
\---setup---
public static class StringMappings
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.Contains))]
public static bool Contains(string self, string value)
\=> self.IndexOf(value) >= 0;
}
:::

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

**Generated SQL:**

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
        => obj.FirstName + " " + obj.LastName;
}
```

::: tip
The stub can use any C# syntax that `[Expressive]` supports -- switch expressions, pattern matching, null-conditional operators, and more.
:::

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

## Properties

Both `[ExpressiveFor]` and `[ExpressiveForConstructor]` support the same optional properties as `[Expressive]`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowBlockBody` | `bool` | `false` | Enables block-bodied stubs (`if`/`else`, local variables, etc.) |
| `Transformers` | `Type[]?` | `null` | Per-mapping transformers applied when expanding the mapped member |

::: expressive-sample
db.Orders.Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
\---setup---
public static class MathBlockMappings
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp), AllowBlockBody = true)]
public static int ClampInt(int value, int min, int max)
{
if (value < min) return min;
if (value > max) return max;
return value;
}
}
:::

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

**Generated SQL:**

```sql
Specified method is not supported.
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

::: expressive-sample
db.Orders
.Where(o => !string.IsNullOrWhiteSpace(o.Customer.Name))
.Where(o => System.Math.Clamp(o.Items.Count(), 0, 100) > 5)
.Select(o => new OrderMappingDto(o.Id, o.Customer.Name ?? "N/A"))
\---setup---
public static class MathMappingsComplete
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(System.Math), nameof(System.Math.Clamp))]
public static int ClampInt(int value, int min, int max)
\=> value < min ? min : (value > max ? max : value);
}

public static class StringMappingsComplete
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
public static bool IsNullOrWhiteSpace(string? s)
\=> s == null || s.Trim().Length == 0;
}

public class OrderMappingDto
{
public int Id { get; set; }
public string Name { get; set; } = "";

```
// The constructor that call sites (new OrderMappingDto(id, name)) invoke.
public OrderMappingDto(int id, string name)
{
    Id = id;
    Name = name;
}
```

}

public static class OrderMappingDtoBuilder
{
// Provides a translatable body for the constructor above — call sites
// `new OrderMappingDto(id, name)` are rewritten to this object-init form
// during expression-tree expansion, so the provider sees a translatable
// MemberInit instead of a constructor call.
\[ExpressiveSharp.Mapping.ExpressiveForConstructor(typeof(OrderMappingDto))]
public static OrderMappingDto Build(int id, string name)
\=> new OrderMappingDto(0, "") { Id = id, Name = name };
}
:::

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

**Generated SQL:**

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
