# Reusable Query Filters

This recipe shows how to define reusable filtering logic as `[Expressive]` properties and extension methods, and compose them across multiple queries without duplicating LINQ expressions.

## The Pattern

Define your filtering criteria as `[Expressive]` members that return `bool`. Use them in `Where()` clauses exactly as you would any other property. EF Core translates the expanded expression to a SQL `WHERE` clause.

## Example: Active Entity Filter

```csharp
public class User
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? EmailVerifiedDate { get; set; }
    public bool IsAdmin { get; set; }

    [Expressive]
    public bool IsActive =>
        !IsDeleted
        && EmailVerifiedDate != null
        && LastLoginDate >= DateTime.UtcNow.AddDays(-90);
}
```

```csharp
// Reuse everywhere
var activeUsers = dbContext.Users.Where(u => u.IsActive).ToList();
var activeAdmins = dbContext.Users.Where(u => u.IsActive && u.IsAdmin).ToList();
var activeCount = dbContext.Users.Count(u => u.IsActive);
```

Generated SQL (SQLite):

```sql
SELECT *
FROM "Users" AS "u"
WHERE "u"."IsDeleted" = 0
  AND "u"."EmailVerifiedDate" IS NOT NULL
  AND "u"."LastLoginDate" >= DATETIME('now', '-90 days')
```

## Example: Parameterized Filters with Extension Methods

Extension methods are ideal for filters that accept parameters:

```csharp
public static class OrderExtensions
{
    [Expressive]
    public static bool IsWithinDateRange(this Order order, DateTime from, DateTime to) =>
        order.CreatedDate >= from && order.CreatedDate <= to;

    [Expressive]
    public static bool IsHighValue(this Order order, decimal threshold) =>
        order.GrandTotal >= threshold;

    [Expressive]
    public static bool BelongsToRegion(this Order order, string region) =>
        order.ShippingAddress != null && order.ShippingAddress.Region == region;
}
```

```csharp
var from = DateTime.UtcNow.AddMonths(-1);
var to = DateTime.UtcNow;

var recentHighValueOrders = dbContext.Orders
    .Where(o => o.IsWithinDateRange(from, to))
    .Where(o => o.IsHighValue(500m))
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT *
FROM "Orders" AS "o"
WHERE "o"."CreatedDate" >= @from
  AND "o"."CreatedDate" <= @to
  AND "o"."GrandTotal" >= 500.0
```

::: tip
Parameters (`from`, `to`, `500m`) are captured as SQL parameters -- there is no string concatenation or SQL injection risk.
:::

## Example: Composing Filters

Build complex filters by composing simpler `[Expressive]` members:

```csharp
public class Order
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public bool HasOpenReturnRequest { get; set; }

    [Expressive]
    public bool IsFulfilled => FulfilledDate != null;

    [Expressive]
    public bool IsRecent => CreatedDate >= DateTime.UtcNow.AddDays(-30);

    // Composed from simpler [Expressive] members
    [Expressive]
    public bool IsRecentFulfilledOrder => IsFulfilled && IsRecent;
}

public static class OrderExtensions
{
    [Expressive]
    public static bool IsEligibleForReturn(this Order order) =>
        order.IsFulfilled
        && order.FulfilledDate >= DateTime.UtcNow.AddDays(-30)
        && !order.HasOpenReturnRequest;
}
```

```csharp
// Dashboard query
var fulfilledRecently = dbContext.Orders
    .Where(o => o.IsRecentFulfilledOrder)
    .ToList();

// Return eligibility check
var returnable = dbContext.Orders
    .Where(o => o.IsEligibleForReturn())
    .Select(o => new { o.Id, o.FulfilledDate })
    .ToList();
```

The composed filters are expanded recursively -- `IsRecentFulfilledOrder` references `IsFulfilled` and `IsRecent`, which are both expanded to their underlying expressions before SQL translation.

## Example: Global Query Filters with EF Core

`[Expressive]` properties work in EF Core's global query filters (configured in `OnModelCreating`):

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Soft-delete global filter using an [Expressive] property
    modelBuilder.Entity<Order>()
        .HasQueryFilter(o => !o.IsDeleted);

    // Tenant isolation filter
    modelBuilder.Entity<Order>()
        .HasQueryFilter(o => o.TenantId == _currentTenantId);
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

```csharp
public static class OrderSpecifications
{
    [Expressive]
    public static bool IsActive(this Order order) =>
        !order.IsCancelled && !order.IsDeleted;

    [Expressive]
    public static bool IsOverdue(this Order order) =>
        order.IsActive()
        && order.DueDate < DateTime.UtcNow
        && !order.IsFulfilled;

    [Expressive]
    public static bool RequiresAttention(this Order order) =>
        order.IsOverdue()
        || order.HasOpenDispute
        || order.PaymentStatus == PaymentStatus.Failed;
}
```

```csharp
// Dashboard: count orders requiring attention
var attentionCount = await dbContext.Orders
    .Where(o => o.RequiresAttention())
    .CountAsync();

// Alert users with overdue orders
var overdueUserIds = await dbContext.Orders
    .Where(o => o.IsOverdue())
    .Select(o => o.UserId)
    .Distinct()
    .ToListAsync();
```

All specification methods are expanded recursively -- `RequiresAttention` calls `IsOverdue`, which calls `IsActive`. The entire chain becomes a flat SQL `WHERE` clause.

## Using Filters with ExpressiveDbSet

With `ExpressiveDbSet<T>`, you can combine `[Expressive]` filters with inline modern syntax:

```csharp
var results = ctx.Orders
    .Where(o => o.IsActive() && o.Customer?.Region == "US")
    .Select(o => new
    {
        o.Id,
        Status = o.PaymentStatus switch
        {
            PaymentStatus.Paid    => "Paid",
            PaymentStatus.Pending => "Pending",
            _                     => "Other"
        }
    })
    .ToList();
```

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
Filter members should only read data, never modify it. Everything in the body must be translatable to SQL.
:::

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- building blocks for filter composition
- [Scoring and Classification](/recipes/scoring-classification) -- combine filters with classification logic
- [Nullable Navigation Properties](/recipes/nullable-navigation) -- filters that guard against null navigation
