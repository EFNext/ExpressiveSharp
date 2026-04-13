---
url: 'https://efnext.github.io/ExpressiveSharp/guide/expressive-constructors.md'
---
# Constructor Projections

Mark a constructor with `[Expressive]` to project your DTOs directly inside LINQ queries. The generator emits a `MemberInit` expression (`new T() { Prop = value, ... }`) that your LINQ provider can translate to a native projection.

## Why Constructor Projections?

Without constructor projections, you must write inline anonymous types or repeat object-initializer logic in every query:

::: expressive-sample
db.Orders
.Select(o => new OrderSummaryDto
{
Id = o.Id,
Description = "Order #" + o.Id,
Total = o.Items.Sum(i => i.UnitPrice \* i.Quantity),
})
\---setup---
public class OrderSummaryDto
{
public int Id { get; set; }
public string Description { get; set; } = "";
public decimal Total { get; set; }
}
:::

```csharp
db
    .Orders
    .Select(o => new OrderSummaryDto
    {
        Id = o.Id,
        Description = "Order #" + o.Id,
        Total = o.Items.Sum(i => i.UnitPrice * i.Quantity),
    })

// Setup
public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Total { get; set; }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId")
FROM "Orders" AS "o"
```

With an `[Expressive]` constructor, you define the projection once and use it everywhere:

::: expressive-sample
db.Orders.Select(o => new OrderSummaryDto(o.Id, "Order #" + o.Id, o.Total()))
\---setup---
public class OrderSummaryDto
{
public int Id { get; set; }
public string Description { get; set; } = "";
public decimal Total { get; set; }

```
public OrderSummaryDto() { }

[Expressive]
public OrderSummaryDto(int id, string description, decimal total)
{
    Id = id;
    Description = description;
    Total = total;
}
```

}

public static class OrderExt
{
\[Expressive]
public static decimal Total(this Order o)
\=> o.Items.Sum(i => i.UnitPrice \* i.Quantity);
}
:::

```csharp
db
    .Orders
    .Select(o => new OrderSummaryDto(o.Id, "Order #" + o.Id, o.Total()))

// Setup
public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Total { get; set; }

    public OrderSummaryDto() { }

    [Expressive]
    public OrderSummaryDto(int id, string description, decimal total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}

public static class OrderExt
{
    [Expressive]
    public static decimal Total(this Order o)
        => o.Items.Sum(i => i.UnitPrice * i.Quantity);
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum(ef_multiply("l"."UnitPrice", CAST("l"."Quantity" AS TEXT))), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId")
FROM "Orders" AS "o"
```

## Basic Example

```csharp
public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Total { get; set; }

    public OrderSummaryDto() { }   // required parameterless constructor

    [Expressive]
    public OrderSummaryDto(int id, string description, decimal total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
```

The generator produces an expression equivalent to:

```csharp
(int id, string description, decimal total) => new OrderSummaryDto()
{
    Id = id,
    Description = description,
    Total = total
}
```

Use it in a query -- the query tabs above show how each provider translates the projection.

::: tip
`o.Total()` is itself an `[Expressive]` extension -- it is expanded recursively into `o.Items.Sum(i => i.UnitPrice * i.Quantity)` before translation. Constructor projections compose with expressive properties and methods seamlessly.
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
| `if`/`else` chains | Converted to ternary expressions / provider CASE |
| Switch expressions | Translated to nested ternary / CASE |
| `base()`/`this()` initializer chains | Recursively inlines the delegated constructor's assignments |

## Inheritance -- Base/This Initializer Chains

The generator recursively inlines delegated constructor assignments. This is useful with DTO inheritance hierarchies:

::: expressive-sample
db.Customers.Select(c => new CustomerDetailDto(c))
\---setup---
public class CustomerDto
{
public int Id { get; set; }
public string Name { get; set; } = "";

```
public CustomerDto() { }

[Expressive]
public CustomerDto(Customer c)
{
    Id = c.Id;
    Name = c.Name;
}
```

}

public class CustomerDetailDto : CustomerDto
{
public string? Email { get; set; }
public string Tier { get; set; } = "";

```
public CustomerDetailDto() { }

[Expressive]
public CustomerDetailDto(Customer c) : base(c)
{
    Email = c.Email;
    Tier = c.Orders.Count() >= 10 ? "Gold" : "Standard";
}
```

}
:::

```csharp
db
    .Customers
    .Select(c => new CustomerDetailDto(c))

// Setup
public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public CustomerDto() { }

    [Expressive]
    public CustomerDto(Customer c)
    {
        Id = c.Id;
        Name = c.Name;
    }
}

public class CustomerDetailDto : CustomerDto
{
    public string? Email { get; set; }
    public string Tier { get; set; } = "";

    public CustomerDetailDto() { }

    [Expressive]
    public CustomerDetailDto(Customer c) : base(c)
    {
        Email = c.Email;
        Tier = c.Orders.Count() >= 10 ? "Gold" : "Standard";
    }
}
```

**Generated SQL:**

```sql
SELECT "c"."Email", CASE
    WHEN (
        SELECT COUNT(*)
        FROM "Orders" AS "o"
        WHERE "c"."Id" = "o"."CustomerId") >= 10 THEN 'Gold'
    ELSE 'Standard'
END AS "Tier"
FROM "Customers" AS "c"
```

The generated expression inlines both the base constructor and the derived constructor body:

```csharp
(Customer c) => new CustomerDetailDto()
{
    Id = c.Id,
    Name = c.Name,
    Email = c.Email,
    Tier = c.Orders.Count() >= 10 ? "Gold" : "Standard"
}
```

## Constructor Overloads

Multiple `[Expressive]` constructors per class are supported -- each overload generates its own expression, distinguished by parameter types:

```csharp
public class OrderDto
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string? Note { get; set; }

    public OrderDto() { }

    [Expressive]
    public OrderDto(int id, decimal total)
    {
        Id = id;
        Total = total;
    }

    [Expressive]
    public OrderDto(int id, decimal total, string note)
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
* [EF Core Integration](./integrations/ef-core) -- full EF Core setup and features
