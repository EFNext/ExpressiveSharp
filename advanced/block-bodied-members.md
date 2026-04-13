---
url: 'https://efnext.github.io/ExpressiveSharp/advanced/block-bodied-members.md'
---
# Block-Bodied Members

ExpressiveSharp supports **block-bodied** properties and methods decorated with `[Expressive]`, in addition to expression-bodied members (`=>`).

::: warning Opt-In Feature
Block-bodied member support requires explicit opt-in. Set `AllowBlockBody = true` on the attribute or enable it globally via an MSBuild property. Without this, block bodies produce diagnostic EXP0004.
:::

## Why Block Bodies?

Expression-bodied members are concise but can become difficult to read when the logic involves complex conditionals:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Category = o.GetCategory() })
\---setup---
public static class OrderExt
{
// Hard to read as a nested ternary
\[Expressive]
public static string GetCategoryTerse(this Order o) =>
o.Items.Sum(i => i.Quantity) \* 10 > 100 ? "Bulk" : "Regular";

```
// Much clearer as a block body
[Expressive(AllowBlockBody = true)]
public static string GetCategory(this Order o)
{
    var threshold = o.Items.Sum(i => i.Quantity) * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}
```

}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Category = o.GetCategory() })

// Setup
public static class OrderExt
{
    // Hard to read as a nested ternary
    [Expressive]
    public static string GetCategoryTerse(this Order o) =>
        o.Items.Sum(i => i.Quantity) * 10 > 100 ? "Bulk" : "Regular";

    // Much clearer as a block body
    [Expressive(AllowBlockBody = true)]
    public static string GetCategory(this Order o)
    {
        var threshold = o.Items.Sum(i => i.Quantity) * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", 'Regular' AS "Category"
FROM "Orders" AS "o"
```

Both forms generate equivalent expression trees and produce identical SQL when used with EF Core.

## Enabling Block Bodies

### Per-Member

Add `AllowBlockBody = true` to the `[Expressive]` attribute:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Tier = o.Tier() })
\---setup---
public static class OrderExt
{
\[Expressive(AllowBlockBody = true)]
public static string Tier(this Order o)
{
var total = o.Items.Sum(i => i.UnitPrice \* i.Quantity);
if (total >= 1000m) return "Premium";
if (total >= 100m) return "Standard";
return "Budget";
}
}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Tier = o.Tier() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static string Tier(this Order o)
    {
        var total = o.Items.Sum(i => i.UnitPrice * i.Quantity);
        if (total >= 1000m) return "Premium";
        if (total >= 100m) return "Standard";
        return "Budget";
    }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", 'Budget' AS "Tier"
FROM "Orders" AS "o"
```

### Globally via MSBuild

Enable block bodies for all `[Expressive]` members in a project by adding the `Expressive_AllowBlockBody` property:

```xml
<PropertyGroup>
    <Expressive_AllowBlockBody>true</Expressive_AllowBlockBody>
</PropertyGroup>
```

This is equivalent to setting `AllowBlockBody = true` on every `[Expressive]` member in the project.

## Supported Constructs

### Return Statements

Simple return statements are the most basic block body form:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Constant = o.GetConstant() })
\---setup---
public static class OrderExt
{
\[Expressive(AllowBlockBody = true)]
public static int GetConstant(this Order o)
{
return 42;
}
}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Constant = o.GetConstant() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static int GetConstant(this Order o)
    {
        return 42;
    }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", 42 AS "Constant"
FROM "Orders" AS "o"
```

***

### If/Else Statements

If/else chains are converted to nested `Expression.Condition` (ternary) nodes:

::: expressive-sample
db.Products.Select(p => new { p.Name, Category = p.GetCategory() })
\---setup---
public static class ProductExt
{
\[Expressive(AllowBlockBody = true)]
public static string GetCategory(this Product p)
{
if (p.ListPrice >= 100)
return "Premium";
else if (p.ListPrice >= 50)
return "Standard";
else
return "Budget";
}
}
:::

```csharp
db
    .Products
    .Select(p => new { p.Name, Category = p.GetCategory() })

// Setup
public static class ProductExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetCategory(this Product p)
    {
        if (p.ListPrice >= 100)
            return "Premium";
        else if (p.ListPrice >= 50)
            return "Standard";
        else
            return "Budget";
    }
}
```

**Generated SQL:**

```sql
SELECT "p"."Name", CASE
    WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'Premium'
    WHEN ef_compare("p"."ListPrice", '50.0') >= 0 THEN 'Standard'
    ELSE 'Budget'
END AS "Category"
FROM "Products" AS "p"
```

An `if` without an `else` is supported when followed by a fallback `return`:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Status = o.GetStatus() })
\---setup---
public static class OrderExt
{
\[Expressive(AllowBlockBody = true)]
public static string GetStatus(this Order o)
{
if (o.Status == OrderStatus.Paid)
return "Active";
return "Inactive";  // Fallback
}
}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Status = o.GetStatus() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetStatus(this Order o)
    {
        if (o.Status == OrderStatus.Paid)
            return "Active";
        return "Inactive";  // Fallback
    }
}
```

**Generated SQL:**

```sql
Specified method is not supported.
```

Multiple independent early-return statements are converted to a nested ternary chain:

::: expressive-sample
db.Products.Select(p => new { p.Name, Range = p.GetPriceRange() })
\---setup---
public static class ProductExt
{
\[Expressive(AllowBlockBody = true)]
public static string GetPriceRange(this Product p)
{
if (p.ListPrice > 1000) return "Very High";
if (p.ListPrice > 100)  return "High";
if (p.ListPrice > 10)   return "Medium";
return "Low";
}
}
:::

```csharp
db
    .Products
    .Select(p => new { p.Name, Range = p.GetPriceRange() })

// Setup
public static class ProductExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetPriceRange(this Product p)
    {
        if (p.ListPrice > 1000) return "Very High";
        if (p.ListPrice > 100)  return "High";
        if (p.ListPrice > 10)   return "Medium";
        return "Low";
    }
}
```

**Generated SQL:**

```sql
Specified method is not supported.
```

***

### Switch Statements

Switch statements are converted to nested conditional expressions:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Label = o.GetLabel() })
\---setup---
public static class OrderExt
{
\[Expressive(AllowBlockBody = true)]
public static string GetLabel(this Order o)
{
switch (o.Status)
{
case OrderStatus.Pending: return "New";
case OrderStatus.Paid: return "Active";
case OrderStatus.Delivered: return "Closed";
default: return "Unknown";
}
}
}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Label = o.GetLabel() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetLabel(this Order o)
    {
        switch (o.Status)
        {
            case OrderStatus.Pending: return "New";
            case OrderStatus.Paid: return "Active";
            case OrderStatus.Delivered: return "Closed";
            default: return "Unknown";
        }
    }
}
```

**Generated SQL:**

```sql
Expression of type 'System.Object' cannot be used for return type 'System.String'
```

***

### Local Variable Declarations

Local variables declared at the method body level are emitted as `Expression.Variable` nodes within an `Expression.Block`:

::: expressive-sample
db.LineItems.Select(i => new { i.Id, Doubled = i.CalculateDouble() })
\---setup---
public static class LineItemExt
{
\[Expressive(AllowBlockBody = true)]
public static decimal CalculateDouble(this LineItem i)
{
var doubled = i.UnitPrice \* 2;
return doubled + 5;
}
}
:::

```csharp
db
    .LineItems
    .Select(i => new { i.Id, Doubled = i.CalculateDouble() })

// Setup
public static class LineItemExt
{
    [Expressive(AllowBlockBody = true)]
    public static decimal CalculateDouble(this LineItem i)
    {
        var doubled = i.UnitPrice * 2;
        return doubled + 5;
    }
}
```

**Generated SQL:**

```sql
SELECT "l"."Id", ef_add(ef_multiply("l"."UnitPrice", '2.0'), '5.0') AS "Doubled"
FROM "LineItems" AS "l"
```

Transitive references are supported:

::: expressive-sample
db.LineItems.Select(i => new { i.Id, Complex = i.CalculateComplex() })
\---setup---
public static class LineItemExt
{
\[Expressive(AllowBlockBody = true)]
public static decimal CalculateComplex(this LineItem i)
{
var a = i.UnitPrice \* 2;
var b = a + 5;
return b + 10;
}
}
:::

```csharp
db
    .LineItems
    .Select(i => new { i.Id, Complex = i.CalculateComplex() })

// Setup
public static class LineItemExt
{
    [Expressive(AllowBlockBody = true)]
    public static decimal CalculateComplex(this LineItem i)
    {
        var a = i.UnitPrice * 2;
        var b = a + 5;
        return b + 10;
    }
}
```

**Generated SQL:**

```sql
SELECT "l"."Id", ef_add(ef_add(ef_multiply("l"."UnitPrice", '2.0'), '5.0'), '10.0') AS "Complex"
FROM "LineItems" AS "l"
```

::: warning Variable Duplication Caveat
The `FlattenBlockExpressions` transformer (applied by `UseExpressives()` in EF Core) inlines local variables at each usage point. If a variable is referenced multiple times, its initializer is duplicated:

```csharp
[Expressive(AllowBlockBody = true)]
public static decimal Foo(this LineItem i)
{
    var x = i.UnitPrice * i.Quantity;
    return x + x;
    // After FlattenBlockExpressions: (UnitPrice * Quantity) + (UnitPrice * Quantity)
}
```

For pure expressions (no side effects), this is semantically identical. The generator detects potential side effects and reports EXP0005.
:::

***

### Foreach Loops

`foreach` loops are emitted as `Expression.Loop` with the enumerator pattern (GetEnumerator/MoveNext/Current). The `ConvertLoopsToLinq` transformer then rewrites these to equivalent LINQ method calls for providers like EF Core that cannot translate loop expressions:

::: expressive-sample
db.Orders.Select(o => new { o.Id, Total = o.GetTotalLineItemPrice() })
\---setup---
public static class OrderExt
{
\[Expressive(AllowBlockBody = true)]
public static decimal GetTotalLineItemPrice(this Order o)
{
var total = 0m;
foreach (var item in o.Items)
total += item.UnitPrice;
return total;
}
}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Total = o.GetTotalLineItemPrice() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static decimal GetTotalLineItemPrice(this Order o)
    {
        var total = 0m;
        foreach (var item in o.Items)
            total += item.UnitPrice;
        return total;
    }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", (
    SELECT COALESCE(ef_sum("l"."UnitPrice"), '0.0')
    FROM "LineItems" AS "l"
    WHERE "o"."Id" = "l"."OrderId") AS "Total"
FROM "Orders" AS "o"
```

After the `ConvertLoopsToLinq` transformer, this becomes equivalent to `o.Items.Sum(item => item.UnitPrice)`.

***

### For Loops

`for` loops over arrays or lists are emitted by the generator, but produce a **EXP0006 warning** recommending `foreach` for better LINQ provider compatibility:

```csharp
[Expressive(AllowBlockBody = true)]
public int SumArray()
{
    var sum = 0;
    for (var i = 0; i < Items.Length; i++)
        sum += Items[i];
    return sum;
}
```

## Unsupported Constructs

The following constructs are **not supported** in block bodies and produce diagnostics:

| Construct | Diagnostic | Severity | Reason |
|---|---|---|---|
| `while` / `do-while` loops | EXP0006 | Warning | No reliable expression tree equivalent |
| `try` / `catch` / `finally` | EXP0006 | Warning | No expression tree equivalent |
| `throw` statements | EXP0006 | Warning | Not reliably translatable by LINQ providers |
| `async` / `await` | EXP0005 | Error | Side effects incompatible with expression trees |
| Assignments (`x = y`) | EXP0005 | Error | Side effects in expression trees |
| `++` / `--` | EXP0005 | Error | Side effects in expression trees |

::: tip Use LINQ Instead of Loops
If you need aggregation logic, prefer LINQ methods in an expression-bodied member:

```csharp
// Instead of a loop
public static class OrderExt
{
    [Expressive]
    public static decimal TotalPrice(this Order o) => o.Items.Sum(i => i.UnitPrice);
}
```

:::

## SQL Output Examples

### If/Else to CASE WHEN

::: expressive-sample
db.Orders.Select(o => new { o.Id, Category = o.GetCategoryFromThreshold() })
\---setup---
public static class OrderExt
{
\[Expressive(AllowBlockBody = true)]
public static string GetCategoryFromThreshold(this Order o)
{
var threshold = o.Items.Sum(i => i.Quantity) \* 10;
if (threshold > 100) return "Bulk";
return "Regular";
}
}
:::

```csharp
db
    .Orders
    .Select(o => new { o.Id, Category = o.GetCategoryFromThreshold() })

// Setup
public static class OrderExt
{
    [Expressive(AllowBlockBody = true)]
    public static string GetCategoryFromThreshold(this Order o)
    {
        var threshold = o.Items.Sum(i => i.Quantity) * 10;
        if (threshold > 100) return "Bulk";
        return "Regular";
    }
}
```

**Generated SQL:**

```sql
SELECT "o"."Id", 'Regular' AS "Category"
FROM "Orders" AS "o"
```

### Switch Expression Equivalent

Block-body switch statements and expression-bodied switch expressions produce the same SQL:

::: expressive-sample
db.Products.Select(p => new { p.Name, Grade = p.GetGrade() })
\---setup---
public static class ProductExt
{
\[Expressive]
public static string GetGrade(this Product p) => p.ListPrice switch
{
\>= 100m => "Premium",
\>= 50m  => "Standard",
\_       => "Budget",
};
}
:::

```csharp
db
    .Products
    .Select(p => new { p.Name, Grade = p.GetGrade() })

// Setup
public static class ProductExt
{
    [Expressive]
    public static string GetGrade(this Product p) => p.ListPrice switch
    {
        >= 100m => "Premium",
        >= 50m  => "Standard",
        _       => "Budget",
    };
}
```

**Generated SQL:**

```sql
SELECT "p"."Name", CASE
    WHEN ef_compare("p"."ListPrice", '100.0') >= 0 THEN 'Premium'
    WHEN ef_compare("p"."ListPrice", '50.0') >= 0 THEN 'Standard'
    ELSE 'Budget'
END AS "Grade"
FROM "Products" AS "p"
```

## Side Effect Detection

The source generator actively detects statements with side effects and reports diagnostics:

| Pattern | Diagnostic |
|---|---|
| Property/field assignment (`Bar = 10;`) | EXP0005 -- side effect detected |
| Compound assignment (`Bar += 10;`) | EXP0005 -- side effect detected |
| Increment/decrement (`Bar++;`) | EXP0005 -- side effect detected |
| Block body without `AllowBlockBody = true` | EXP0004 -- block body requires opt-in |

See [Limitations](./limitations) for the full list of restrictions.
