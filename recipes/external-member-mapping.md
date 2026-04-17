---
url: 'https://efnext.github.io/ExpressiveSharp/recipes/external-member-mapping.md'
---
# External Member Mapping

This recipe shows how to use `[ExpressiveFor]` and `[ExpressiveForConstructor]` to provide expression-tree bodies for members on types you do not own -- BCL methods, third-party libraries, or your own members that cannot use `[Expressive]` directly. This enables those members to be translated to SQL by EF Core.

## When to Use `[ExpressiveFor]`

Use `[ExpressiveFor]` when:

* The member is on a BCL type (`Math`, `string`, `DateTime`, etc.) and EF Core does not translate it
* The member is on a third-party library type you cannot modify
* The member is on your own type but uses logic that cannot be expressed as an `[Expressive]` body (reflection, I/O, etc.) and you want to provide a SQL-friendly alternative
* You want to override how a specific member translates to SQL

::: info
If a member already has `[Expressive]`, adding `[ExpressiveFor]` targeting it is a compile error (EXP0019). `[ExpressiveFor]` is specifically for members that **do not** have `[Expressive]`.
:::

## Static Method: `Math.Clamp`

`Math.Clamp` is a BCL method that some providers cannot translate. Provide an expression-tree equivalent:

::: expressive-sample
db.Products.Select(p => new { p.Id, ClampedPrice = Math.Clamp(p.ListPrice, 20m, 100m) })
\---setup---
static class MathMappings
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
static decimal Clamp(decimal value, decimal min, decimal max)
\=> value < min ? min : (value > max ? max : value);
}
:::

```csharp
db
    .Products
    .Select(p => new { p.Id, ClampedPrice = Math.Clamp(p.ListPrice, 20m, 100m) })

// Setup
static class MathMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
    static decimal Clamp(decimal value, decimal min, decimal max)
        => value < min ? min : (value > max ? max : value);
}
```

**Generated SQL:**

```sql
SELECT "p"."Id", CASE
    WHEN ef_compare("p"."ListPrice", '20.0') < 0 THEN '20.0'
    WHEN ef_compare("p"."ListPrice", '100.0') > 0 THEN '100.0'
    ELSE "p"."ListPrice"
END AS "ClampedPrice"
FROM "Products" AS "p"
```

::: tip
The call site is unchanged -- you still write `Math.Clamp(...)`. The `ExpressiveReplacer` detects the mapping at runtime and substitutes the ternary expression automatically.
:::

## Static Method: `string.IsNullOrWhiteSpace`

Another common BCL method that some providers cannot translate:

::: expressive-sample
db.Customers.Where(c => !string.IsNullOrWhiteSpace(c.Email))
\---setup---
static class StringMappings
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
static bool IsNullOrWhiteSpace(string? s)
\=> s == null || s.Trim().Length == 0;
}
:::

```csharp
db
    .Customers
    .Where(c => !string.IsNullOrWhiteSpace(c.Email))

// Setup
static class StringMappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}
```

**Generated SQL:**

```sql
SELECT "c"."Id", "c"."Country", "c"."Email", "c"."JoinedAt", "c"."Name"
FROM "Customers" AS "c"
WHERE "c"."Email" IS NOT NULL AND length(trim("c"."Email")) <> 0
```

## Instance Members on Your Own Type

For instance properties or methods, the first parameter of the stub is the receiver. You can use this to provide a SQL-friendly alternative for a property whose body relies on non-translatable logic:

```csharp
using ExpressiveSharp.Mapping;

public class Person
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    // This property uses string interpolation and Trim() -- works with [Expressive],
    // but imagine it used reflection or other non-translatable logic
    public string FullName => $"{FirstName} {LastName}".Trim().ToUpper();
}

static class PersonMappings
{
    // Provide a SQL-friendly alternative
    [ExpressiveFor(typeof(Person), nameof(Person.FullName))]
    static string FullName(Person p)
        => p.FirstName + " " + p.LastName;
}
```

```csharp
var names = dbContext.People
    .OrderBy(p => p.FullName)
    .Select(p => new { p.Id, p.FullName })
    .ToList();
```

## `[ExpressiveForConstructor]` for Constructors

When you need to provide an expression-tree body for a constructor on a type you do not own:

```csharp
using ExpressiveSharp.Mapping;

// External DTO from a shared package
public class OrderDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public OrderDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

// Provide the expression-tree body
[ExpressiveForConstructor(typeof(OrderDto))]
static OrderDto CreateOrderDto(int id, string name)
    => new OrderDto { Id = id, Name = name };
```

```csharp
var dtos = dbContext.Orders
    .Select(o => new OrderDto(o.Id, o.Status.ToString()))
    .ToList();
```

## Combining with EF Core Queries

`[ExpressiveFor]` mappings integrate seamlessly with `UseExpressives()` and `ExpressiveDbSet<T>`. Here we combine `Math.Clamp` on a numeric field with `string.IsNullOrWhiteSpace` on a nullable string field:

::: expressive-sample
db.Customers
.Where(c => !string.IsNullOrWhiteSpace(c.Email))
.Select(c => new { c.Id, c.Name, Label = c.Country ?? "Unknown" })
\---setup---
static class Mappings
{
\[ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
static bool IsNullOrWhiteSpace(string? s)
\=> s == null || s.Trim().Length == 0;
}
:::

```csharp
db
    .Customers
    .Where(c => !string.IsNullOrWhiteSpace(c.Email))
    .Select(c => new { c.Id, c.Name, Label = c.Country ?? "Unknown" })

// Setup
static class Mappings
{
    [ExpressiveSharp.Mapping.ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}
```

**Generated SQL:**

```sql
SELECT "c"."Id", "c"."Name", COALESCE("c"."Country", 'Unknown') AS "Label"
FROM "Customers" AS "c"
WHERE "c"."Email" IS NOT NULL AND length(trim("c"."Email")) <> 0
```

## Common Use Cases

### Math Functions

```csharp
using ExpressiveSharp.Mapping;

static class MathMappings
{
    [ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);

    [ExpressiveFor(typeof(Math), nameof(Math.Abs))]
    static int Abs(int value)
        => value < 0 ? -value : value;

    [ExpressiveFor(typeof(Math), nameof(Math.Sign))]
    static int Sign(double value)
        => value > 0 ? 1 : (value < 0 ? -1 : 0);
}
```

### String Helpers

```csharp
using ExpressiveSharp.Mapping;

static class StringMappings
{
    [ExpressiveFor(typeof(string), nameof(string.IsNullOrEmpty))]
    static bool IsNullOrEmpty(string? s)
        => s == null || s.Length == 0;

    [ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}
```

### DateTime Calculations

```csharp
using ExpressiveSharp.Mapping;

static class DateTimeMappings
{
    // Custom helper method on your utility class
    [ExpressiveFor(typeof(DateTimeHelpers), nameof(DateTimeHelpers.DaysBetween))]
    static int DaysBetween(DateTimeHelpers _, DateTime start, DateTime end)
        => (end - start).Days;
}
```

## Related Diagnostics

| Code | Description |
|------|-------------|
| EXP0014 | `[ExpressiveFor]` target type not found |
| EXP0015 | `[ExpressiveFor]` target member not found on the specified type |
| EXP0017 | Return type of stub does not match target member's return type |
| EXP0019 | Target member already has `[Expressive]` -- use `[Expressive]` directly instead |
| EXP0020 | Duplicate `[ExpressiveFor]` mapping for the same target member |

## Tips

::: tip Match the signature exactly
For static methods, the stub parameters must match the target method signature. For instance members you have three options: write a `static` method stub whose first parameter is the receiver (e.g. `static string FullName(Person p)`), write an `instance` method stub on the target type itself where the stub's `this` is the receiver, or write a **property stub** on the target type (parameterless; `this` is the receiver) -- property stubs can only target other properties.
:::

::: tip Co-locate when possible
When the target is on the same type as the stub, you can drop `typeof(...)` and use the single-argument form -- it targets a member on the stub's containing type. Combined with an **instance property stub**, this is the ergonomic shape for the case where a property has its own backing storage (a plain settable auto-property used for DTO shape, serialization, or in-memory assignment in tests) but you want queries to compute it from other columns:

```csharp
public class Person
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    // Regular auto-property — can be assigned directly (for DTOs, tests, deserialization).
    public string FullName { get; set; } = "";

    // When used in a LINQ expression tree, FullName is rewritten to this body,
    // so EF Core projects it from the underlying columns instead of trying to
    // map it to a column of its own. `this` is the receiver automatically.
    [ExpressiveFor(nameof(FullName))]
    private string FullNameExpression => FirstName + " " + LastName;
}
```

A method stub (`string FullNameExpression() => ...`) works the same way and is appropriate when the target is a method or you need a block body.

If the property has no backing storage and the same body works at both runtime and query time, put `[Expressive]` directly on the property and delete the stub.
:::

::: tip Consider \[Expressive] first
Many `[ExpressiveFor]` use cases exist because of syntax limitations in other libraries. Since ExpressiveSharp supports switch expressions, pattern matching, string interpolation, and more, you may be able to put `[Expressive]` directly on the member and skip the external mapping entirely.
:::

::: warning Placement
`[ExpressiveFor]` stubs can live either in a `static` helper class (with a `static` method stub) or on the target type itself as an instance method or property. Either way, they are discovered at compile time by the source generator.
:::

## See Also

* [DTO Projections with Constructors](./dto-projections) -- `[ExpressiveForConstructor]` in depth
* [Computed Entity Properties](./computed-properties) -- `[Expressive]` on your own types
* [Migrating from Projectables](../guide/migration-from-projectables) -- replacing `UseMemberBody` with `[ExpressiveFor]`
