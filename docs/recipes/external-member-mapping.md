# External Member Mapping

This recipe shows how to use `[ExpressiveFor]` and `[ExpressiveForConstructor]` to provide expression-tree bodies for members on types you do not own -- BCL methods, third-party libraries, or your own members that cannot use `[Expressive]` directly. This enables those members to be translated to SQL by EF Core.

## When to Use `[ExpressiveFor]`

Use `[ExpressiveFor]` when:

- The member is on a BCL type (`Math`, `string`, `DateTime`, etc.) and EF Core does not translate it
- The member is on a third-party library type you cannot modify
- The member is on your own type but uses logic that cannot be expressed as an `[Expressive]` body (reflection, I/O, etc.) and you want to provide a SQL-friendly alternative
- You want to override how a specific member translates to SQL

::: info
If a member already has `[Expressive]`, adding `[ExpressiveFor]` targeting it is a compile error (EXP0019). `[ExpressiveFor]` is specifically for members that **do not** have `[Expressive]`.
:::

## Static Method: `Math.Clamp`

`Math.Clamp` is a BCL method that EF Core cannot translate. Provide an expression-tree equivalent:

```csharp
using ExpressiveSharp.Mapping;

static class MathMappings
{
    [ExpressiveFor(typeof(Math), nameof(Math.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);
}
```

Now `Math.Clamp` works in EF Core queries:

```csharp
var results = dbContext.Orders
    .Select(o => new
    {
        o.Id,
        ClampedPrice = Math.Clamp(o.Price, 20.0, 100.0)
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       CASE
           WHEN "o"."Price" < 20.0 THEN 20.0
           WHEN "o"."Price" > 100.0 THEN 100.0
           ELSE "o"."Price"
       END AS "ClampedPrice"
FROM "Orders" AS "o"
```

::: tip
The call site is unchanged -- you still write `Math.Clamp(...)`. The `ExpressiveReplacer` detects the mapping at runtime and substitutes the ternary expression automatically.
:::

## Static Method: `string.IsNullOrWhiteSpace`

Another common BCL method that some providers cannot translate:

```csharp
using ExpressiveSharp.Mapping;

static class StringMappings
{
    [ExpressiveFor(typeof(string), nameof(string.IsNullOrWhiteSpace))]
    static bool IsNullOrWhiteSpace(string? s)
        => s == null || s.Trim().Length == 0;
}
```

```csharp
var results = dbContext.Customers
    .Where(c => !string.IsNullOrWhiteSpace(c.Email))
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT *
FROM "Customers" AS "c"
WHERE NOT ("c"."Email" IS NULL OR LENGTH(TRIM("c"."Email")) = 0)
```

## Instance Property on Your Own Type

For instance properties or methods, the first parameter of the stub is the receiver:

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

Generated SQL (SQLite):

```sql
SELECT "p"."Id",
       "p"."FirstName" || ' ' || "p"."LastName" AS "FullName"
FROM "People" AS "p"
ORDER BY "p"."FirstName" || ' ' || "p"."LastName"
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
    .Select(o => new OrderDto(o.Id, o.Tag ?? "N/A"))
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       COALESCE("o"."Tag", 'N/A') AS "Name"
FROM "Orders" AS "o"
```

## Combining with EF Core Queries

`[ExpressiveFor]` mappings integrate seamlessly with `UseExpressives()` and `ExpressiveDbSet<T>`:

```csharp
var results = ctx.Orders
    .Where(o => Math.Clamp(o.Price, 20, 100) > 50)
    .Where(o => !string.IsNullOrWhiteSpace(o.Tag))
    .Select(o => new
    {
        o.Id,
        SafePrice = Math.Clamp(o.Price, 20, 100),
        Label = o.Customer?.FullName ?? "Unknown"
    })
    .ToList();
```

All three mappings (`Math.Clamp`, `string.IsNullOrWhiteSpace`, `Person.FullName`) are expanded automatically.

## Common Use Cases

### Math Functions

```csharp
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
| EXP0016 | `[ExpressiveFor]` stub method must be `static` |
| EXP0017 | Return type of stub does not match target member's return type |
| EXP0019 | Target member already has `[Expressive]` -- use `[Expressive]` directly instead |
| EXP0020 | Duplicate `[ExpressiveFor]` mapping for the same target member |

## Tips

::: tip Match the signature exactly
For static methods, the stub parameters must match the target method signature. For instance members, add the receiver as the first parameter (e.g., `static string FullName(Person p)`).
:::

::: tip Consider [Expressive] first
Many `[ExpressiveFor]` use cases exist because of syntax limitations in other libraries. Since ExpressiveSharp supports switch expressions, pattern matching, string interpolation, and more, you may be able to put `[Expressive]` directly on the member and skip the external mapping entirely.
:::

::: warning Placement
`[ExpressiveFor]` stubs must be in a `static` class. The class can be in any namespace -- it is discovered at compile time by the source generator.
:::

## See Also

- [DTO Projections with Constructors](/recipes/dto-projections) -- `[ExpressiveForConstructor]` in depth
- [Computed Entity Properties](/recipes/computed-properties) -- `[Expressive]` on your own types
- [Migrating from Projectables](/recipes/migration-from-projectables) -- replacing `UseMemberBody` with `[ExpressiveFor]`
