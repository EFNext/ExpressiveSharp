---
url: 'https://efnext.github.io/ExpressiveSharp/guide/expressive-constructors.md'
---
# Constructor Projections

Mark a constructor with `[Expressive]` to project your DTOs directly inside LINQ queries. The generator emits a `MemberInit` expression (`new T() { Prop = value, ... }`) that EF Core can translate to a SQL projection.

## Why Constructor Projections?

Without constructor projections, you must write inline anonymous types or repeat object-initializer logic in every query:

```csharp
// Without [Expressive] constructors -- repeated in every query
var dtos = ctx.Orders
    .Select(o => new OrderSummaryDto
    {
        Id = o.Id,
        Description = o.Tag ?? "N/A",
        Total = o.Price * o.Quantity
    })
    .ToList();
```

With an `[Expressive]` constructor, you define the projection once and use it everywhere:

```csharp
var dtos = ctx.Orders
    .Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total))
    .ToList();
```

## Basic Example

```csharp
public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderSummaryDto() { }   // required parameterless constructor

    [Expressive]
    public OrderSummaryDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
```

The generator produces an expression equivalent to:

```csharp
(int id, string description, double total) => new OrderSummaryDto()
{
    Id = id,
    Description = description,
    Total = total
}
```

Use it in a query:

```csharp
var dtos = ctx.Orders
    .Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total))
    .ToList();
```

Generated SQL:

```sql
SELECT "o"."Id",
       COALESCE("o"."Tag", 'N/A') AS "Description",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total"
FROM "Orders" AS "o"
```

::: tip
Notice that `o.Total` is an `[Expressive]` property -- it is expanded recursively into `o.Price * o.Quantity` before SQL translation. Constructor projections compose with expressive properties and methods seamlessly.
:::

## Requirements

The class **must** expose an accessible **parameterless constructor** (public, internal, or protected-internal). The generated code uses `new T() { ... }` (object-initializer syntax), which requires a parameterless constructor.

If the parameterless constructor is missing, the generator reports **EXP0002**.

## Supported Constructs

Constructor bodies support the following constructs:

| Construct | Notes |
|---|---|
| Simple property assignments | `Id = id;` `Description = description;` |
| Local variable declarations | Inlined at each usage point |
| `if`/`else` chains | Converted to ternary expressions / SQL CASE |
| Switch expressions | Translated to nested ternary / CASE |
| `base()`/`this()` initializer chains | Recursively inlines the delegated constructor's assignments |

## Inheritance -- Base/This Initializer Chains

The generator recursively inlines delegated constructor assignments. This is useful with DTO inheritance hierarchies:

```csharp
public class PersonDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";

    public PersonDto() { }

    [Expressive]
    public PersonDto(Person person)
    {
        FullName = person.FirstName + " " + person.LastName;
        Email = person.Email;
    }
}

public class EmployeeDto : PersonDto
{
    public string Department { get; set; } = "";
    public string Grade { get; set; } = "";

    public EmployeeDto() { }

    [Expressive]
    public EmployeeDto(Employee employee) : base(employee)
    {
        Department = employee.Department.Name;
        Grade = employee.YearsOfService >= 10 ? "Senior" : "Junior";
    }
}

var employees = ctx.Employees
    .Select(e => new EmployeeDto(e))
    .ToList();
```

The generated expression inlines both the base constructor and the derived constructor body:

```csharp
(Employee employee) => new EmployeeDto()
{
    FullName = employee.FirstName + " " + employee.LastName,
    Email = employee.Email,
    Department = employee.Department.Name,
    Grade = employee.YearsOfService >= 10 ? "Senior" : "Junior"
}
```

## Constructor Overloads

Multiple `[Expressive]` constructors per class are supported -- each overload generates its own expression, distinguished by parameter types:

```csharp
public class OrderDto
{
    public int Id { get; set; }
    public double Total { get; set; }
    public string? Note { get; set; }

    public OrderDto() { }

    [Expressive]
    public OrderDto(int id, double total)
    {
        Id = id;
        Total = total;
    }

    [Expressive]
    public OrderDto(int id, double total, string note)
    {
        Id = id;
        Total = total;
        Note = note;
    }
}
```

## Factory Method Conversion

If you have an existing `[Expressive]` factory method that returns `new T { ... }`, the generator emits diagnostic **EXP0012** (Info severity) suggesting a conversion to a constructor.

## Diagnostics

| Code | Severity | Description |
|------|----------|-------------|
| **EXP0002** | Error | Class is missing a parameterless constructor |
| **EXP0003** | Error | Delegated constructor source not available (base/this chain cannot be analyzed) |
| **EXP0012** | Info | Factory method can be converted to an `[Expressive]` constructor |

## Next Steps

* [\[Expressive\] Properties](./expressive-properties) -- computed properties on entities
* [\[Expressive\] Methods](./expressive-methods) -- parameterized query fragments
* [EF Core Integration](./ef-core-integration) -- full EF Core setup and features
