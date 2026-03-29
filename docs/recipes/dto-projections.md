# DTO Projections with Constructors

This recipe shows how to use `[Expressive]` constructors to project database rows directly into DTOs inside your LINQ queries -- with no boilerplate `Select` expressions and full SQL translation.

## The Problem

Projecting entities into DTOs usually requires writing a `Select` expression that repeats the mapping logic:

```csharp
// Repetitive -- mapping duplicated in every query
var customers = dbContext.Customers
    .Select(c => new CustomerDto
    {
        Id = c.Id,
        FullName = c.FirstName + " " + c.LastName,
        IsActive = c.IsActive,
        OrderCount = c.Orders.Count()
    })
    .ToList();
```

If the mapping changes you must update every `Select` that uses it.

## The Solution: `[Expressive]` Constructor

Mark a constructor with `[Expressive]` and call it directly in your query. The source generator emits a `MemberInit` expression that EF Core translates to SQL:

```csharp
public class CustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public bool IsActive { get; set; }
    public int OrderCount { get; set; }

    public CustomerDto() { }   // parameterless constructor required

    [Expressive]
    public CustomerDto(Customer c)
    {
        Id = c.Id;
        FullName = c.FirstName + " " + c.LastName;
        IsActive = c.IsActive;
        OrderCount = c.Orders.Count();
    }
}
```

```csharp
// Clean -- mapping defined once, used everywhere
var customers = dbContext.Customers
    .Where(c => c.IsActive)
    .Select(c => new CustomerDto(c))
    .ToList();
```

The constructor body is inlined as SQL -- no data is fetched to memory for the projection.

Generated SQL (SQLite):

```sql
SELECT "c"."Id",
       "c"."FirstName" || ' ' || "c"."LastName" AS "FullName",
       "c"."IsActive",
       (SELECT COUNT(*)
        FROM "Orders" AS "o"
        WHERE "c"."Id" = "o"."CustomerId") AS "OrderCount"
FROM "Customers" AS "c"
WHERE "c"."IsActive"
```

## Basic Constructor Projection: OrderSummaryDto

A straightforward example showing how constructor parameters map to SQL expressions:

```csharp
public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderSummaryDto() { }

    [Expressive]
    public OrderSummaryDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
```

```csharp
var dtos = dbContext.Orders
    .Select(o => new OrderSummaryDto(o.Id, o.Tag ?? "N/A", o.Total))
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "o"."Id",
       COALESCE("o"."Tag", 'N/A') AS "Description",
       "o"."Price" * CAST("o"."Quantity" AS REAL) AS "Total"
FROM "Orders" AS "o"
```

::: tip
Notice that `o.Total` is an `[Expressive]` property -- it gets expanded to `Price * Quantity` automatically. Constructor projections compose naturally with computed properties.
:::

## Inheritance Chains with Base Initializers

When your DTOs form an inheritance hierarchy, use `: base(...)` to avoid duplicating base-class assignments. The generator inlines both the base and derived assignments:

```csharp
public class PersonDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";

    public PersonDto() { }

    [Expressive]
    public PersonDto(Person p)
    {
        FullName = p.FirstName + " " + p.LastName;
        Email = p.Email;
    }
}

public class EmployeeDto : PersonDto
{
    public string Department { get; set; } = "";
    public string Grade { get; set; } = "";

    public EmployeeDto() { }

    [Expressive]
    public EmployeeDto(Employee e) : base(e)   // PersonDto assignments inlined automatically
    {
        Department = e.Department.Name;
        Grade = e.YearsOfService >= 10 ? "Senior" : "Junior";
    }
}
```

```csharp
var employees = dbContext.Employees
    .Select(e => new EmployeeDto(e))
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT "e"."FirstName" || ' ' || "e"."LastName" AS "FullName",
       "e"."Email",
       "d"."Name" AS "Department",
       CASE
           WHEN "e"."YearsOfService" >= 10 THEN 'Senior'
           ELSE 'Junior'
       END AS "Grade"
FROM "Employees" AS "e"
INNER JOIN "Departments" AS "d" ON "e"."DepartmentId" = "d"."Id"
```

All fields -- `FullName`, `Email`, `Department`, and `Grade` -- are projected in a single query.

## Constructor Overloads

If you need different projections from the same DTO, use constructor overloads. Each gets its own generated expression:

```csharp
public class OrderSummaryDto
{
    public int Id { get; set; }
    public double Total { get; set; }
    public string? CustomerName { get; set; }

    public OrderSummaryDto() { }

    // Full projection (with customer name -- requires navigation join)
    [Expressive]
    public OrderSummaryDto(Order o)
    {
        Id = o.Id;
        Total = o.GrandTotal;
        CustomerName = o.Customer.FirstName + " " + o.Customer.LastName;
    }

    // Lightweight projection (no navigation join needed)
    [Expressive]
    public OrderSummaryDto(Order o, bool lightweight)
    {
        Id = o.Id;
        Total = o.GrandTotal;
        CustomerName = null;
    }
}
```

```csharp
// Full projection -- joins Customer table
var full = dbContext.Orders
    .Select(o => new OrderSummaryDto(o))
    .ToList();

// Lightweight projection -- no join
var light = dbContext.Orders
    .Select(o => new OrderSummaryDto(o, true))
    .ToList();
```

## Using Switch Expressions in Constructors

Constructor bodies support the same modern C# syntax as other `[Expressive]` members:

```csharp
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string PriceTier { get; set; } = "";

    public ProductDto() { }

    [Expressive]
    public ProductDto(Product p)
    {
        Id = p.Id;
        Name = p.Name;
        Price = p.SalePrice;
        PriceTier = p.SalePrice switch
        {
            > 500 => "Premium",
            > 100 => "Standard",
            _     => "Budget"
        };
    }
}
```

Generated SQL (SQLite):

```sql
SELECT "p"."Id",
       "p"."Name",
       "p"."ListPrice" * (1 - "p"."DiscountRate") AS "Price",
       CASE
           WHEN "p"."ListPrice" * (1 - "p"."DiscountRate") > 500 THEN 'Premium'
           WHEN "p"."ListPrice" * (1 - "p"."DiscountRate") > 100 THEN 'Standard'
           ELSE 'Budget'
       END AS "PriceTier"
FROM "Products" AS "p"
```

## Using `[ExpressiveForConstructor]` for External Types

If you do not own the DTO type (third-party library, shared package), use `[ExpressiveForConstructor]` to provide the expression body externally:

```csharp
using ExpressiveSharp.Mapping;

[ExpressiveForConstructor(typeof(ExternalOrderDto))]
static ExternalOrderDto CreateDto(int id, string name)
    => new ExternalOrderDto { Id = id, Name = name };
```

See [External Member Mapping](/recipes/external-member-mapping) for details.

## Tips

::: tip Always add a parameterless constructor
The generator emits `new T() { ... }` syntax. If the parameterless constructor is missing, the build will fail.
:::

::: tip Keep mappings pure
No side effects, no calls to non-expressible methods. Everything in the constructor body must be translatable to SQL.
:::

::: info
Constructor bodies are block-bodied by nature, but they do **not** require `AllowBlockBody = true` — the generator handles them automatically. `UseExpressives()` registers the `FlattenBlockExpressions` transformer to flatten them for EF Core.
:::

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- reusable computed values referenced in constructor projections
- [External Member Mapping](/recipes/external-member-mapping) -- `[ExpressiveForConstructor]` for types you do not own
- [Scoring and Classification](/recipes/scoring-classification) -- switch expressions and pattern matching in projections
