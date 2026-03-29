# Block-Bodied Members

ExpressiveSharp supports **block-bodied** properties and methods decorated with `[Expressive]`, in addition to expression-bodied members (`=>`).

::: warning Opt-In Feature
Block-bodied member support requires explicit opt-in. Set `AllowBlockBody = true` on the attribute or enable it globally via an MSBuild property. Without this, block bodies produce diagnostic EXP0004.
:::

## Why Block Bodies?

Expression-bodied members are concise but can become difficult to read when the logic involves complex conditionals:

```csharp
// Hard to read as a nested ternary
[Expressive]
public string GetCategory() => Quantity * 10 > 100 ? "Bulk" : "Regular";

// Much clearer as a block body
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    var threshold = Quantity * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}
```

Both forms generate equivalent expression trees and produce identical SQL when used with EF Core.

## Enabling Block Bodies

### Per-Member

Add `AllowBlockBody = true` to the `[Expressive]` attribute:

```csharp
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    if (Price >= 100) return "Premium";
    if (Price >= 50) return "Standard";
    return "Budget";
}
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

```csharp
[Expressive(AllowBlockBody = true)]
public int GetConstant()
{
    return 42;
}
```

---

### If/Else Statements

If/else chains are converted to nested `Expression.Condition` (ternary) nodes:

```csharp
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    if (Price >= 100)
        return "Premium";
    else if (Price >= 50)
        return "Standard";
    else
        return "Budget";
}
```

An `if` without an `else` is supported when followed by a fallback `return`:

```csharp
[Expressive(AllowBlockBody = true)]
public string GetStatus()
{
    if (IsActive)
        return "Active";
    return "Inactive";  // Fallback
}
```

Multiple independent early-return statements are converted to a nested ternary chain:

```csharp
[Expressive(AllowBlockBody = true)]
public string GetPriceRange()
{
    if (Price > 1000) return "Very High";
    if (Price > 100)  return "High";
    if (Price > 10)   return "Medium";
    return "Low";
}
```

---

### Switch Statements

Switch statements are converted to nested conditional expressions:

```csharp
[Expressive(AllowBlockBody = true)]
public string GetLabel()
{
    switch (Status)
    {
        case 1: return "New";
        case 2: return "Active";
        case 3: return "Closed";
        default: return "Unknown";
    }
}
```

---

### Local Variable Declarations

Local variables declared at the method body level are emitted as `Expression.Variable` nodes within an `Expression.Block`:

```csharp
[Expressive(AllowBlockBody = true)]
public int CalculateDouble()
{
    var doubled = Price * 2;
    return doubled + 5;
}
```

Transitive references are supported:

```csharp
[Expressive(AllowBlockBody = true)]
public int CalculateComplex()
{
    var a = Price * 2;
    var b = a + 5;
    return b + 10;
}
```

::: warning Variable Duplication Caveat
The `FlattenBlockExpressions` transformer (applied by `UseExpressives()` in EF Core) inlines local variables at each usage point. If a variable is referenced multiple times, its initializer is duplicated:

```csharp
[Expressive(AllowBlockBody = true)]
public double Foo()
{
    var x = Price * Quantity;
    return x + x;
    // After FlattenBlockExpressions: (Price * Quantity) + (Price * Quantity)
}
```

For pure expressions (no side effects), this is semantically identical. The generator detects potential side effects and reports EXP0005.
:::

---

### Foreach Loops

`foreach` loops are emitted as `Expression.Loop` with the enumerator pattern (GetEnumerator/MoveNext/Current). The `ConvertLoopsToLinq` transformer then rewrites these to equivalent LINQ method calls for providers like EF Core that cannot translate loop expressions:

```csharp
[Expressive(AllowBlockBody = true)]
public double GetTotalLineItemPrice()
{
    var total = 0.0;
    foreach (var item in LineItems)
        total += item.Price;
    return total;
}
```

After the `ConvertLoopsToLinq` transformer, this becomes equivalent to `LineItems.Sum(item => item.Price)`.

---

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
[Expressive]
public double TotalPrice => LineItems.Sum(i => i.Price);
```
:::

## SQL Output Examples

### If/Else to CASE WHEN

```csharp
[Expressive(AllowBlockBody = true)]
public string GetCategory()
{
    var threshold = Quantity * 10;
    if (threshold > 100) return "Bulk";
    return "Regular";
}
```

Generated SQL:

```sql
SELECT CASE
    WHEN ("o"."Quantity" * 10) > 100 THEN 'Bulk'
    ELSE 'Regular'
END AS "Category"
FROM "Orders" AS "o"
```

### Switch Expression Equivalent

Block-body switch statements and expression-bodied switch expressions produce the same SQL:

```csharp
[Expressive]
public string GetGrade() => Price switch
{
    >= 100 => "Premium",
    >= 50  => "Standard",
    _      => "Budget",
};
```

```sql
SELECT CASE
    WHEN "o"."Price" >= 100.0 THEN 'Premium'
    WHEN "o"."Price" >= 50.0 THEN 'Standard'
    ELSE 'Budget'
END AS "Grade"
FROM "Orders" AS "o"
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
