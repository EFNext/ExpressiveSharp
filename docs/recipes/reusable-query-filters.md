# Reusable Query Filters

This recipe shows how to define reusable filtering logic as `[Expressive]` properties and extension methods, and compose them across multiple queries without duplicating LINQ expressions.

## The Pattern

Define your filtering criteria as `[Expressive]` members that return `bool`. Use them in `Where()` clauses exactly as you would any other property. EF Core translates the expanded expression to a SQL `WHERE` clause.

## Example: Active Entity Filter

::: expressive-sample
db.Customers.Where(c => c.IsActive())
---setup---
public static class CustomerActiveExt
{
    [Expressive]
    public static bool IsActive(this Customer c) =>
        c.Email != null
        && c.JoinedAt >= new DateTime(2023, 1, 1);
}
:::

Reuse everywhere:

::: expressive-sample
db.Customers.Where(c => c.IsActive() && c.Country == "US").Select(c => c.Id)
---setup---
public static class CustomerActiveExt2
{
    [Expressive]
    public static bool IsActive(this Customer c) =>
        c.Email != null
        && c.JoinedAt >= new DateTime(2023, 1, 1);
}
:::

## Example: Parameterized Filters with Extension Methods

Extension methods are ideal for filters that accept parameters:

::: expressive-sample
db.Orders
    .Where(o => o.IsWithinDateRange(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31)))
    .Where(o => o.IsHighValue(500m))
---setup---
public static class OrderParamFilters
{
    [Expressive]
    public static bool IsWithinDateRange(this Order order, DateTime from, DateTime to) =>
        order.PlacedAt >= from && order.PlacedAt <= to;

    [Expressive]
    public static bool IsHighValue(this Order order, decimal threshold) =>
        order.Items.Sum(i => i.UnitPrice * i.Quantity) >= threshold;

    [Expressive]
    public static bool BelongsToCountry(this Order order, string country) =>
        order.Customer != null && order.Customer.Country == country;
}
:::

::: tip
Parameters (`from`, `to`, `500m`) are captured as provider parameters -- there is no string concatenation or SQL injection risk.
:::

## Example: Composing Filters

Build complex filters by composing simpler `[Expressive]` members:

::: expressive-sample
db.Orders.Where(o => o.IsRecentPaidOrder())
---setup---
public static class OrderComposedFilters
{
    [Expressive]
    public static bool IsPaid(this Order o) => o.Status == OrderStatus.Paid;

    [Expressive]
    public static bool IsRecent(this Order o) => o.PlacedAt >= new DateTime(2024, 1, 1);

    // Composed from simpler [Expressive] members
    [Expressive]
    public static bool IsRecentPaidOrder(this Order o) => o.IsPaid() && o.IsRecent();

    [Expressive]
    public static bool IsEligibleForReturn(this Order order) =>
        order.Status == OrderStatus.Delivered
        && order.PlacedAt >= new DateTime(2024, 1, 1);
}
:::

The composed filters are expanded recursively -- `IsRecentPaidOrder` references `IsPaid` and `IsRecent`, which are both expanded to their underlying expressions before translation.

## Example: Global Query Filters with EF Core

`[Expressive]` properties work in EF Core's global query filters (configured in `OnModelCreating`):

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Active-order global filter using an [Expressive] extension
    modelBuilder.Entity<Order>()
        .HasQueryFilter(o => o.Status != OrderStatus.Refunded);

    // Tenant isolation filter
    modelBuilder.Entity<Order>()
        .HasQueryFilter(o => o.CustomerId == _currentCustomerId);
}
```

::: info
When using global query filters, ensure that `UseExpressives()` is configured on your `DbContext`. The library includes a convention that expands `[Expressive]` member references in global filters automatically.
:::

```csharp
// Bypass the global filter when needed
var allOrders = dbContext.Orders
    .IgnoreQueryFilters()
    .ToList();
```

## Example: Specification Pattern

`[Expressive]` members pair naturally with the Specification pattern:

::: expressive-sample
db.Orders.Where(o => o.RequiresAttention()).Select(o => o.Id)
---setup---
public static class OrderSpecifications
{
    [Expressive]
    public static bool IsActive(this Order order) =>
        order.Status != OrderStatus.Refunded;

    [Expressive]
    public static bool IsOverdue(this Order order) =>
        order.IsActive()
        && order.PlacedAt < new DateTime(2024, 6, 1)
        && order.Status == OrderStatus.Pending;

    [Expressive]
    public static bool RequiresAttention(this Order order) =>
        order.IsOverdue()
        || order.Status == OrderStatus.Pending;
}
:::

All specification methods are expanded recursively -- `RequiresAttention` calls `IsOverdue`, which calls `IsActive`. The entire chain becomes a flat `WHERE` clause.

## Using Filters with ExpressiveDbSet

With `ExpressiveDbSet<T>`, you can combine `[Expressive]` filters with inline modern syntax:

::: expressive-sample
db.Orders
    .Where(o => o.IsActive() && o.Customer.Country == "US")
    .Select(o => new
    {
        o.Id,
        StatusLabel = o.Status switch
        {
            OrderStatus.Paid    => "Paid",
            OrderStatus.Pending => "Pending",
            _                   => "Other"
        }
    })
---setup---
public static class OrderActiveForDbSet
{
    [Expressive]
    public static bool IsActive(this Order order) =>
        order.Status != OrderStatus.Refunded;
}
:::

See [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) for more on this approach.

## Tips

::: tip Compose at the member level
Compose filters inside `[Expressive]` members rather than chaining multiple `.Where()` calls. This creates more reusable building blocks.
:::

::: tip Name clearly
Use names that express business intent (`IsEligibleForRefund`) rather than technical details (`HasRefundDateNullAndStatusIsComplete`).
:::

::: tip Prefer entity-level properties for entity-specific filters
Use extension methods for cross-entity or parameterized filters.
:::

::: warning Keep filters pure
Filter members should only read data, never modify it. Everything in the body must be translatable by your provider.
:::

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- building blocks for filter composition
- [Scoring and Classification](/recipes/scoring-classification) -- combine filters with classification logic
- [Nullable Navigation Properties](/recipes/nullable-navigation) -- filters that guard against null navigation
