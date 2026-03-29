# Scoring & Classification

This recipe shows how to use C# pattern matching -- switch expressions, `is` patterns, relational patterns, and more -- inside `[Expressive]` members to compute scores, grades, tiers, and labels directly in SQL.

## Grading with Relational Patterns

Classic grading logic maps numeric ranges to labels. The switch expression reads naturally and the generator translates it to a SQL `CASE` expression:

```csharp
public class Student
{
    public int Id { get; set; }
    public int Score { get; set; }

    [Expressive]
    public string Grade => Score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _     => "F"
    };

    [Expressive]
    public bool IsPassing => Score >= 60;

    [Expressive]
    public bool IsHonors => Score >= 90;
}
```

```csharp
// Grade distribution report
var distribution = dbContext.Students
    .GroupBy(s => s.Grade)
    .Select(g => new { Grade = g.Key, Count = g.Count() })
    .OrderBy(x => x.Grade)
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT CASE
    WHEN "s"."Score" >= 90 THEN 'A'
    WHEN "s"."Score" >= 80 THEN 'B'
    WHEN "s"."Score" >= 70 THEN 'C'
    WHEN "s"."Score" >= 60 THEN 'D'
    ELSE 'F'
END AS "Grade",
COUNT(*) AS "Count"
FROM "Students" AS "s"
GROUP BY CASE
    WHEN "s"."Score" >= 90 THEN 'A'
    WHEN "s"."Score" >= 80 THEN 'B'
    WHEN "s"."Score" >= 70 THEN 'C'
    WHEN "s"."Score" >= 60 THEN 'D'
    ELSE 'F'
END
ORDER BY "Grade"
```

## Customer Tiers with `and` / `or` Patterns

Use `and` and `or` patterns to express range bands cleanly:

```csharp
public class Customer
{
    public int Id { get; set; }
    public int LifetimeOrderCount { get; set; }
    public decimal LifetimeSpend { get; set; }

    [Expressive]
    public string Tier => LifetimeSpend switch
    {
        >= 10_000              => "Platinum",
        >= 5_000 and < 10_000  => "Gold",
        >= 1_000 and < 5_000   => "Silver",
        _                      => "Bronze"
    };

    [Expressive]
    public bool IsLoyalty => LifetimeOrderCount >= 10;
}
```

```csharp
// Segment customers for a marketing campaign
var segments = dbContext.Customers
    .GroupBy(c => c.Tier)
    .Select(g => new { Tier = g.Key, Count = g.Count(), TotalSpend = g.Sum(c => c.LifetimeSpend) })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT CASE
    WHEN "c"."LifetimeSpend" >= 10000 THEN 'Platinum'
    WHEN "c"."LifetimeSpend" >= 5000 AND "c"."LifetimeSpend" < 10000 THEN 'Gold'
    WHEN "c"."LifetimeSpend" >= 1000 AND "c"."LifetimeSpend" < 5000 THEN 'Silver'
    ELSE 'Bronze'
END AS "Tier",
COUNT(*) AS "Count",
COALESCE(SUM("c"."LifetimeSpend"), 0) AS "TotalSpend"
FROM "Customers" AS "c"
GROUP BY CASE
    WHEN "c"."LifetimeSpend" >= 10000 THEN 'Platinum'
    WHEN "c"."LifetimeSpend" >= 5000 AND "c"."LifetimeSpend" < 10000 THEN 'Gold'
    WHEN "c"."LifetimeSpend" >= 1000 AND "c"."LifetimeSpend" < 5000 THEN 'Silver'
    ELSE 'Bronze'
END
```

## Risk Scoring with Property Patterns

Property patterns match on multiple fields of the current instance simultaneously. This is useful for multi-dimensional classification:

```csharp
public class Loan
{
    public int Id { get; set; }
    public int CreditScore { get; set; }
    public decimal DebtToIncomeRatio { get; set; }
    public decimal LoanAmount { get; set; }

    [Expressive]
    public string RiskCategory => this switch
    {
        { CreditScore: >= 750, DebtToIncomeRatio: < 0.3m } => "Low",
        { CreditScore: >= 700 }                             => "Medium",
        { CreditScore: >= 600 }                             => "High",
        _                                                    => "Very High"
    };
}
```

```csharp
// Risk distribution across the loan portfolio
var riskReport = dbContext.Loans
    .GroupBy(l => l.RiskCategory)
    .Select(g => new
    {
        Risk = g.Key,
        Count = g.Count(),
        TotalExposure = g.Sum(l => l.LoanAmount)
    })
    .ToList();
```

Generated SQL (SQLite):

```sql
SELECT CASE
    WHEN "l"."CreditScore" >= 750 AND "l"."DebtToIncomeRatio" < 0.3 THEN 'Low'
    WHEN "l"."CreditScore" >= 700 THEN 'Medium'
    WHEN "l"."CreditScore" >= 600 THEN 'High'
    ELSE 'Very High'
END AS "Risk",
COUNT(*) AS "Count",
COALESCE(SUM("l"."LoanAmount"), 0) AS "TotalExposure"
FROM "Loans" AS "l"
GROUP BY CASE
    WHEN "l"."CreditScore" >= 750 AND "l"."DebtToIncomeRatio" < 0.3 THEN 'Low'
    WHEN "l"."CreditScore" >= 700 THEN 'Medium'
    WHEN "l"."CreditScore" >= 600 THEN 'High'
    ELSE 'Very High'
END
```

## Positional Patterns

ExpressiveSharp supports positional (deconstruct) patterns. If your type defines a `Deconstruct` method, you can match on it:

```csharp
public class Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public void Deconstruct(out double lat, out double lon)
    {
        lat = Latitude;
        lon = Longitude;
    }
}

public class Location
{
    public int Id { get; set; }
    public Coordinate Position { get; set; }

    [Expressive]
    public string Hemisphere => Position switch
    {
        (>= 0, _) => "Northern",
        _          => "Southern"
    };
}
```

## `is` Patterns for Boolean Flags

Use `is` patterns for concise Boolean properties:

```csharp
public class Product
{
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public int ReorderPoint { get; set; }

    [Expressive]
    public bool IsInStock => Stock is > 0;

    [Expressive]
    public bool NeedsReorder => Stock is >= 0 and <= ReorderPoint;

    [Expressive]
    public bool IsBudget => Price is > 0 and < 25;

    [Expressive]
    public bool HasNoStock => Stock is 0;
}
```

## List Patterns

ExpressiveSharp supports list patterns for fixed-length matching:

```csharp
public class Measurement
{
    public int Id { get; set; }
    public int[] Readings { get; set; }

    [Expressive]
    public string ReadingCategory => Readings switch
    {
        [0, 0, 0] => "Zero",
        [_, _, _] => "Triple",
        [_, _]    => "Double",
        [_]       => "Single",
        _         => "Other"
    };
}
```

## Combining Classification with Aggregation

Compose `[Expressive]` classification properties to build rich query results:

```csharp
public class Order
{
    public int Id { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedDate { get; set; }

    [Expressive]
    public string ValueBand => GrandTotal switch
    {
        >= 1000 => "High",
        >= 250  => "Medium",
        _       => "Low"
    };

    [Expressive]
    public bool IsRecent => CreatedDate >= DateTime.UtcNow.AddDays(-30);
}
```

```csharp
// Breakdown of recent orders by value band
var breakdown = dbContext.Orders
    .Where(o => o.IsRecent)
    .GroupBy(o => o.ValueBand)
    .Select(g => new
    {
        Band = g.Key,
        Count = g.Count(),
        Total = g.Sum(o => o.GrandTotal)
    })
    .OrderBy(x => x.Band)
    .ToList();
```

## Using Switch Expressions with ExpressiveDbSet

You can also use switch expressions directly in LINQ chains via `ExpressiveDbSet<T>`, without defining a separate `[Expressive]` property:

```csharp
var results = ctx.Orders
    .Select(o => new
    {
        o.Id,
        Tier = o.GrandTotal switch
        {
            >= 1000 => "Premium",
            >= 250  => "Standard",
            _       => "Basic"
        }
    })
    .ToList();
```

See [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) for more on this approach.

## Tips

::: tip Use `_` as the default arm
Always include a discard arm to avoid generating a ternary chain with no final fallback. This prevents potential null results.
:::

::: tip Keep arms ordered from most to least specific
The generator emits a ternary chain in arm order. Put the most restrictive cases first for correct evaluation.
:::

::: tip Compose with filters
Classification properties work in `Where`, `GroupBy`, and `OrderBy` just like any other `[Expressive]` member. This is how you build reporting queries that compute business categories entirely in SQL.
:::

## See Also

- [Computed Entity Properties](/recipes/computed-properties) -- building blocks for classification
- [Modern Syntax in LINQ Chains](/recipes/modern-syntax-in-linq) -- switch expressions inline in queries
- [Nullable Navigation Properties](/recipes/nullable-navigation) -- safely handling null in classification logic
