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
db
    .Customers
    .Where(c => c.Country != null)
    .Select(c => new CustomerDto(c))

// Setup
public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Country { get; set; }
    public int OrderCount { get; set; }

    public CustomerDto() { }

    [Expressive]
    public CustomerDto(Customer c)
    {
        Id = c.Id;
        Name = c.Name;
        Country = c.Country;
        OrderCount = c.Orders.Count();
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "c"."Id", "c"."Name", "c"."Country", (
    SELECT COUNT(*)
    FROM "Orders" AS "o"
    WHERE "c"."Id" = "o"."CustomerId") AS "OrderCount"
FROM "Customers" AS "c"
WHERE "c"."Country" IS NOT NULL
```


The constructor body is inlined as SQL -- no data is fetched to memory for the projection.

## Basic Constructor Projection: OrderSummaryDto

A straightforward example showing how constructor parameters map to SQL expressions:

```csharp
db
    .Orders
    .Select(o => new OrderSummaryDto(o.Id, o.Status.ToString(), o.ItemCount()))

// Setup
public static class OrderExt
{
    [Expressive]
    public static int ItemCount(this Order o) => o.Items.Count();
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public int Items { get; set; }

    public OrderSummaryDto() { }

    [Expressive]
    public OrderSummaryDto(int id, string description, int items)
    {
        Id = id;
        Description = description;
        Items = items;
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", CASE
    WHEN "o"."Status" = 0 THEN 'Pending'
    WHEN "o"."Status" = 1 THEN 'Paid'
    WHEN "o"."Status" = 2 THEN 'Shipped'
    WHEN "o"."Status" = 3 THEN 'Delivered'
    WHEN "o"."Status" = 4 THEN 'Refunded'
END AS "Description", (
    SELECT COUNT(*)
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "Items"
FROM "Orders" AS "o"
```


::: tip
Notice that `o.ItemCount()` is an `[Expressive]` extension method -- it gets expanded to `o.Items.Count()` automatically. Constructor projections compose naturally with computed members.
:::

## Inheritance Chains with Base Initializers

When your DTOs form an inheritance hierarchy, use `: base(...)` to avoid duplicating base-class assignments. The generator inlines both the base and derived assignments:

```csharp
db
    .Customers
    .Select(c => new PremiumCustomerDto(c))

// Setup
public class CustomerBaseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public CustomerBaseDto() { }

    [Expressive]
    public CustomerBaseDto(Customer c)
    {
        Id = c.Id;
        Name = c.Name;
    }
}

public class PremiumCustomerDto : CustomerBaseDto
{
    public string Country { get; set; } = "";
    public string Tier { get; set; } = "";

    public PremiumCustomerDto() { }

    [Expressive]
    public PremiumCustomerDto(Customer c) : base(c)
    {
        Country = c.Country ?? "Unknown";
        Tier = c.Orders.Count() >= 10 ? "Gold" : "Standard";
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT COALESCE("c"."Country", 'Unknown') AS "Country", CASE
    WHEN (
        SELECT COUNT(*)
        FROM "Orders" AS "o"
        WHERE "c"."Id" = "o"."CustomerId") >= 10 THEN 'Gold'
    ELSE 'Standard'
END AS "Tier"
FROM "Customers" AS "c"
```


All fields -- `Id`, `Name`, `Country`, and `Tier` -- are projected in a single query.

## Constructor Overloads

If you need different projections from the same DTO, use constructor overloads. Each gets its own generated expression:

```csharp
db
    .Orders
    .Select(o => new OrderDto(o))

// Setup
public class OrderDto
{
    public int Id { get; set; }
    public int ItemCount { get; set; }
    public string? CustomerName { get; set; }

    public OrderDto() { }

    // Full projection (with customer name -- requires navigation join)
    [Expressive]
    public OrderDto(Order o)
    {
        Id = o.Id;
        ItemCount = o.Items.Count();
        CustomerName = o.Customer.Name;
    }

    // Lightweight projection (no navigation join needed)
    [Expressive]
    public OrderDto(Order o, bool lightweight)
    {
        Id = o.Id;
        ItemCount = o.Items.Count();
        CustomerName = null;
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", (
    SELECT COUNT(*)
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "ItemCount", "c"."Name" AS "CustomerName"
FROM "Orders" AS "o"
INNER JOIN "Customers" AS "c" ON "o"."CustomerId" = "c"."Id"
```


The lightweight variant is called the same way, just with the extra argument:

```csharp
db
    .Orders
    .Select(o => new OrderDto(o, true))

// Setup
public class OrderDto
{
    public int Id { get; set; }
    public int ItemCount { get; set; }
    public string? CustomerName { get; set; }

    public OrderDto() { }

    [Expressive]
    public OrderDto(Order o)
    {
        Id = o.Id;
        ItemCount = o.Items.Count();
        CustomerName = o.Customer.Name;
    }

    [Expressive]
    public OrderDto(Order o, bool lightweight)
    {
        Id = o.Id;
        ItemCount = o.Items.Count();
        CustomerName = null;
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "o"."Id", (
    SELECT COUNT(*)
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "ItemCount", NULL AS "CustomerName"
FROM "Orders" AS "o"
```


## Using Switch Expressions in Constructors

Constructor bodies support the same modern C# syntax as other `[Expressive]` members:

```csharp
db
    .Products
    .Select(p => new ProductDto(p))

// Setup
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
        Price = p.ListPrice;
        PriceTier = p.ListPrice switch
        {
            > 500m => "Premium",
            > 100m => "Standard",
            _      => "Budget"
        };
    }
}
```

**Generated SQL (SQLite):**

```sql
SELECT "p"."Id", "p"."Name", "p"."ListPrice" AS "Price", CASE
    WHEN ef_compare("p"."ListPrice", '500.0') > 0 THEN 'Premium'
    WHEN ef_compare("p"."ListPrice", '100.0') > 0 THEN 'Standard'
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

See [External Member Mapping](./external-member-mapping) for details.

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

- [Computed Entity Properties](./computed-properties) -- reusable computed values referenced in constructor projections
- [External Member Mapping](./external-member-mapping) -- `[ExpressiveForConstructor]` for types you do not own
- [Scoring and Classification](./scoring-classification) -- switch expressions and pattern matching in projections
