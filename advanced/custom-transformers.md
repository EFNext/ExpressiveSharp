---
url: 'https://efnext.github.io/ExpressiveSharp/advanced/custom-transformers.md'
---
# Custom Transformers

Expression transformers adapt expression trees for specific consumers at runtime. ExpressiveSharp includes four built-in transformers for EF Core compatibility, and you can create your own for custom LINQ providers or specialized rewriting needs.

## The `IExpressionTreeTransformer` Interface

All transformers implement a single-method interface:

```csharp
public interface IExpressionTreeTransformer
{
    Expression Transform(Expression expression);
}
```

Transformers are pure functions: they take an expression tree and return a transformed version. The input should not be mutated; return a new tree if changes are needed.

## Implementing a Custom Transformer

A typical transformer inherits from `ExpressionVisitor` and overrides the relevant `Visit*` methods:

```csharp
using System.Linq.Expressions;
using ExpressiveSharp;

public class MyTransformer : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
    {
        return Visit(expression);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        // Example: rewrite all string equality checks to case-insensitive
        if (node.NodeType == ExpressionType.Equal
            && node.Left.Type == typeof(string)
            && node.Right.Type == typeof(string))
        {
            var toLowerMethod = typeof(string).GetMethod(
                nameof(string.ToLower), Type.EmptyTypes)!;

            return Expression.Equal(
                Expression.Call(Visit(node.Left), toLowerMethod),
                Expression.Call(Visit(node.Right), toLowerMethod));
        }

        return base.VisitBinary(node);
    }
}
```

::: tip
Always call `Visit()` on child nodes before constructing the replacement. This ensures the transformer processes the tree bottom-up, catching nested matches.
:::

## Registration Methods

There are three ways to register transformers, depending on your use case.

### Per-Member via Attribute

Use the `Transformers` property on `[Expressive]` to apply transformers to a specific member. These transformers run when the generated expression is resolved via `ExpressiveResolver`:

```csharp
[Expressive(Transformers = new[] { typeof(RemoveNullConditionalPatterns) })]
public string? CustomerName => Customer?.Name;
```

Multiple transformers can be specified and are applied in order:

```csharp
[Expressive(Transformers = new[] {
    typeof(RemoveNullConditionalPatterns),
    typeof(MyTransformer)
})]
public string? SafeName => Customer?.Name?.ToUpper();
```

::: info
Transformers specified via the attribute must have a parameterless constructor. The source generator emits code to instantiate them at expression resolution time.
:::

### Globally via `ExpressiveOptions.Default`

Register transformers globally so all `ExpandExpressives()` calls apply them automatically:

```csharp
ExpressiveOptions.Default.AddTransformers(new RemoveNullConditionalPatterns());

// All subsequent ExpandExpressives() calls use this transformer
Expression<Func<Order, string?>> expr = o => o.CustomerName;
var expanded = expr.ExpandExpressives();
// RemoveNullConditionalPatterns applied automatically
```

To reset global transformers:

```csharp
ExpressiveOptions.Default.ClearTransformers();
```

### At Runtime via `ExpandExpressives()`

Pass transformers directly when expanding an expression tree:

```csharp
Expression<Func<Order, string?>> expr = o => o.CustomerName;
var expanded = expr.ExpandExpressives(new MyTransformer(), new AnotherTransformer());
```

This is useful for one-off transformations or when different call sites need different transformer sets.

## Execution Order

When `UseExpressives()` is configured for EF Core, the transformer pipeline runs in this order:

1. **Per-member transformers** (from `[Expressive(Transformers = ...)]`) -- applied during expression resolution, before the tree is substituted into the query.
2. **Built-in EF Core transformers** -- applied as part of `ExpandExpressives()`:
   * `ConvertLoopsToLinq`
   * `RemoveNullConditionalPatterns`
   * `FlattenTupleComparisons`
   * `FlattenBlockExpressions`
3. **Plugin-contributed transformers** (from `IExpressivePlugin.GetTransformers()`) -- applied after the built-in transformers.

When using `ExpandExpressives()` outside of EF Core, only the globally registered transformers (via `ExpressiveOptions.Default`) and any transformers passed as arguments are applied, along with per-member transformers from the attribute.

## Creating an `IExpressivePlugin`

For EF Core integration, you can bundle your transformer with an `IExpressivePlugin` so it participates in the `UseExpressives()` pipeline alongside services:

```csharp
using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class MyPlugin : IExpressivePlugin
{
    public void ApplyServices(IServiceCollection services)
    {
        // Register any additional EF Core services
        // (method call translators, evaluatable expression filters, etc.)
    }

    public IExpressionTreeTransformer[] GetTransformers()
        => [new MyTransformer()];
}
```

Register the plugin during setup:

```csharp
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseSqlite(connection)
    .UseExpressives(o => o.AddPlugin(new MyPlugin()))
    .Options;
```

The `RelationalExtensions` package uses this exact pattern to register the `RewriteIndexedSelectToRowNumber` transformer alongside window function SQL translators.

## Built-in Transformers Reference

| Transformer | Purpose | When to use manually |
|---|---|---|
| `RemoveNullConditionalPatterns` | Strips `x != null ? x.Prop : default` ternaries | Databases that handle null propagation natively |
| `FlattenBlockExpressions` | Inlines block-local variables, removes `Expression.Block` | LINQ providers that do not support block expressions |
| `FlattenTupleComparisons` | Replaces `ValueTuple` field access with underlying arguments | Providers that cannot translate `ValueTuple` construction |
| `ConvertLoopsToLinq` | Rewrites `LoopExpression` to LINQ method calls | Providers that cannot translate loop expressions |

All four are applied automatically by `UseExpressives()`. You only need to use them manually if you are working with a non-EF-Core LINQ provider or calling `ExpandExpressives()` directly.

## Testing Transformers

Unit test transformers by building expression trees manually and asserting the transformed output:

```csharp
[TestMethod]
public void MyTransformer_RewritesStringEquality()
{
    // Arrange
    var param = Expression.Parameter(typeof(Order), "o");
    var nameAccess = Expression.Property(param, "Name");
    var constant = Expression.Constant("Alice");
    var equality = Expression.Equal(nameAccess, constant);
    var lambda = Expression.Lambda<Func<Order, bool>>(equality, param);

    var transformer = new MyTransformer();

    // Act
    var result = transformer.Transform(lambda);

    // Assert
    var body = ((LambdaExpression)result).Body;
    Assert.IsInstanceOfType(body, typeof(BinaryExpression));

    // Verify the rewrite applied ToLower()
    var binary = (BinaryExpression)body;
    Assert.IsInstanceOfType(binary.Left, typeof(MethodCallExpression));
    Assert.AreEqual("ToLower", ((MethodCallExpression)binary.Left).Method.Name);
}
```

For integration testing with EF Core, use `ToQueryString()` to verify the generated SQL after transformation:

```csharp
[TestMethod]
public void TransformedQuery_ProducesExpectedSql()
{
    using var ctx = CreateTestContext();
    var query = ctx.Orders
        .AsExpressiveDbSet()
        .Where(o => o.Name == "Alice")
        .ToQueryString();

    Assert.IsTrue(query.Contains("LOWER"), "Expected case-insensitive comparison");
}
```

See [Testing Strategy](./testing-strategy) for more on the project's testing approach.
