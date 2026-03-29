# How It Works

Understanding the internals of ExpressiveSharp helps you use it effectively and debug issues when they arise. The library operates in two phases: **build-time source generation** and **runtime expression expansion**.

## Architecture Overview

```
+-----------------------------------------------------------+
|                       BUILD TIME                          |
|                                                           |
|  Your C# code with [Expressive] members                  |
|           |                                               |
|           v                                               |
|  +-----------------------------------------------------+ |
|  | ExpressiveGenerator                                  | |
|  | - Finds [Expressive] / [ExpressiveFor] members       | |
|  | - Analyzes at semantic level via IOperation           | |
|  | - Generates Expression<> factory code                 | |
|  | - Registers in per-assembly ExpressionRegistry        | |
|  +-----------------------------------------------------+ |
|           |                                               |
|  +-----------------------------------------------------+ |
|  | PolyfillInterceptorGenerator                         | |
|  | - Intercepts ExpressionPolyfill.Create calls         | |
|  | - Intercepts IRewritableQueryable<T> LINQ methods    | |
|  | - Uses C# 13 [InterceptsLocation] attribute          | |
|  +-----------------------------------------------------+ |
|           |                                               |
|           v                                               |
|  Auto-generated *.g.cs files with Expression<> trees      |
+-----------------------------------------------------------+

+-----------------------------------------------------------+
|                        RUNTIME                            |
|                                                           |
|  LINQ query using [Expressive] member                     |
|           |                                               |
|           v                                               |
|  +-----------------------------------------------------+ |
|  | ExpressiveQueryCompiler (EF Core decorator)          | |
|  | - Intercepts Execute / CreateCompiledQuery            | |
|  | - Calls ExpandExpressives() before compilation        | |
|  +-----------------------------------------------------+ |
|           |                                               |
|           v                                               |
|  +-----------------------------------------------------+ |
|  | ExpressiveReplacer (ExpressionVisitor)               | |
|  | - Walks the LINQ expression tree                     | |
|  | - Detects [Expressive] member accesses               | |
|  | - Loads generated Expression<> via registry           | |
|  | - Substitutes the access with the expression body    | |
|  +-----------------------------------------------------+ |
|           |                                               |
|           v                                               |
|  +-----------------------------------------------------+ |
|  | Transformer Pipeline                                 | |
|  | - ConvertLoopsToLinq                                 | |
|  | - RemoveNullConditionalPatterns                      | |
|  | - FlattenTupleComparisons                            | |
|  | - FlattenBlockExpressions                            | |
|  | - (Plugin-contributed transformers)                  | |
|  +-----------------------------------------------------+ |
|           |                                               |
|           v                                               |
|  Expanded + transformed expression tree                   |
|           |                                               |
|           v                                               |
|  Standard EF Core SQL translation                         |
+-----------------------------------------------------------+
```

## Build Time: Source Generators

ExpressiveSharp uses two Roslyn incremental source generators, both targeting `netstandard2.0`.

### `ExpressiveGenerator`

This is the main generator. It finds all members decorated with `[Expressive]` or `[ExpressiveFor]`, analyzes them, and produces `Expression<Func<...>>` factory code.

**Pipeline:**

1. **Filter** -- Uses `ForAttributeWithMetadataName` to find all `MemberDeclarationSyntax` nodes with the `[Expressive]` or `[ExpressiveFor]` attribute.
2. **Interpret** -- Calls `ExpressiveInterpreter` to validate the member (body exists, block body is allowed, no unsupported constructs) and extract a descriptor.
3. **Emit** -- `ExpressionTreeEmitter` walks the Roslyn `IOperation` tree for the member body and emits imperative C# code that calls `Expression.*` factory methods to build the equivalent expression tree.
4. **Register** -- `ExpressionRegistryEmitter` generates a per-assembly `ExpressionRegistry` class with a `TryGet(MemberInfo)` method that maps members to their generated expression factories.

::: info Semantic Analysis via IOperation
Unlike syntax-tree-based approaches (such as EntityFrameworkCore.Projectables, which rewrites syntax nodes), ExpressiveSharp works at the **semantic level** using Roslyn's `IOperation` API. This means it sees the compiler's fully resolved view of the code -- implicit conversions, operator overload resolution, pattern matching semantics, and more -- rather than raw syntax tokens. This is what enables support for complex features like switch expressions, pattern matching, and collection expressions.
:::

### `PolyfillInterceptorGenerator`

This generator uses C# 13 method interceptors (`[InterceptsLocation]`) to replace call sites at compile time:

- **`ExpressionPolyfill.Create` calls** -- The delegate-form lambda is rewritten to an expression tree, enabling modern syntax without any attribute.
- **`IRewritableQueryable<T>` LINQ methods** -- Methods like `Where`, `Select`, `OrderBy`, etc. that accept delegates are rewritten to their `Queryable` equivalents that accept `Expression<Func<...>>`.

The interceptor handles complex multi-lambda methods (Join, GroupJoin, GroupBy overloads), non-lambda-first methods (Zip, ExceptBy), and custom target types registered via `[PolyfillTarget]` (such as EF Core's `EntityFrameworkQueryableExtensions` for async methods).

### Generated Code

For a property like:

```csharp
public class Order
{
    [Expressive]
    public double Total => Price * Quantity;
}
```

The generator produces factory code that builds the expression tree imperatively. Conceptually (simplified pseudocode):

```csharp
// Auto-generated — actual output uses Expression.* factory calls
internal static class Order__Expressives
{
    static Expression<Func<Order, double>> Total_Expression()
    {
        var param = Expression.Parameter(typeof(Order), "@this");
        var price = Expression.Property(param, "Price");
        var quantity = Expression.Convert(
            Expression.Property(param, "Quantity"), typeof(double));
        var body = Expression.Multiply(price, quantity);
        return Expression.Lambda<Func<Order, double>>(body, param);
    }
}
```

And a registry entry:

```csharp
internal static class ExpressionRegistry
{
    public static LambdaExpression? TryGet(MemberInfo member)
    {
        // Dictionary-based lookup by (DeclaringType, MemberName, ParameterTypes)
        ...
    }
}
```

## Runtime: Expression Expansion

### `ExpressiveResolver`

The resolver is the lookup mechanism. When an `[Expressive]` member is encountered at runtime, the resolver:

1. **Fast path** -- Checks the per-assembly `ExpressionRegistry` (generated by the source generator). This is an O(1) dictionary lookup via a cached delegate.
2. **Slow path** -- Falls back to reflection for open-generic class members and generic methods not yet in the registry. Results are cached in a `ConcurrentDictionary` after the first lookup.

For `[ExpressiveFor]` mappings (external member mappings), the resolver scans all loaded assemblies once on first use, caching the results.

### `ExpressiveReplacer`

An `ExpressionVisitor` that walks a LINQ expression tree and:

1. **Detects** property accesses and method calls to `[Expressive]` members.
2. **Loads** the generated expression via `ExpressiveResolver`.
3. **Substitutes** the lambda parameters with the actual arguments from the call site.
4. **Recurses** -- the substituted body is itself visited, expanding any nested `[Expressive]` references.

This means `[Expressive]` members can reference other `[Expressive]` members freely -- the replacer handles the transitive expansion.

### Transformer Pipeline

After expansion, transformers adapt the expression tree for the target LINQ provider. When `UseExpressives()` is active, four built-in transformers run automatically:

| Transformer | Purpose |
|---|---|
| `ConvertLoopsToLinq` | Rewrites `LoopExpression` (from `foreach`/`for`) into LINQ method calls (`Sum`, `Count`, `Any`, `All`) |
| `RemoveNullConditionalPatterns` | Strips null-check ternaries (`x != null ? x.Prop : default` becomes `x.Prop`) for SQL null propagation |
| `FlattenTupleComparisons` | Replaces `ValueTuple` field access with underlying arguments for direct comparison |
| `FlattenBlockExpressions` | Inlines block-local variables and removes `Expression.Block` nodes |

Plugins can contribute additional transformers via `IExpressivePlugin.GetTransformers()`.

See [Custom Transformers](./custom-transformers) for details on writing your own.

## EF Core Integration

### `ExpressiveQueryCompiler`

When `UseExpressives()` is called on `DbContextOptionsBuilder`, ExpressiveSharp registers an `ExpressiveOptionsExtension` that decorates EF Core's `IQueryCompiler`. The `ExpressiveQueryCompiler` wraps the original compiler and intercepts all execution entry points:

```csharp
public override TResult Execute<TResult>(Expression query)
    => _decoratedQueryCompiler.Execute<TResult>(Expand(query));

private Expression Expand(Expression expression)
    => expression.ExpandExpressives(_options);
```

Expansion happens **before** the query reaches EF Core's pipeline. The expanded expression is what gets compiled and cached by EF Core.

### Model Conventions

Two convention plugins are registered automatically:

- **`ExpressivePropertiesNotMappedConvention`** -- Marks `[Expressive]` properties as unmapped in the EF model, so EF Core does not try to create database columns for computed properties.
- **`ExpressiveExpandQueryFiltersConvention`** -- Expands `[Expressive]` member references inside global query filters at model finalization time.

## Plugin Architecture

The `IExpressivePlugin` interface allows extensions to hook into the EF Core integration:

```csharp
public interface IExpressivePlugin
{
    void ApplyServices(IServiceCollection services);
    IExpressionTreeTransformer[] GetTransformers() => [];
}
```

Plugins are registered during setup:

```csharp
options.UseExpressives(o => o.AddPlugin(new MyPlugin()));
```

The built-in `RelationalExtensions` package uses this architecture to register window function SQL translators and the `RewriteIndexedSelectToRowNumber` transformer.

## Component Summary

| Phase | Component | Responsibility |
|---|---|---|
| Build | `ExpressiveGenerator` | Source gen entry point, orchestration |
| Build | `ExpressiveInterpreter` | Validates members, extracts descriptors |
| Build | `ExpressionTreeEmitter` | Maps `IOperation` nodes to `Expression.*` factory calls |
| Build | `ExpressionRegistryEmitter` | Generates per-assembly expression registry |
| Build | `PolyfillInterceptorGenerator` | Intercepts `ExpressionPolyfill.Create` and `IRewritableQueryable<T>` call sites |
| Runtime | `ExpressiveResolver` | Locates generated expressions via registry or reflection |
| Runtime | `ExpressiveReplacer` | Walks and replaces `[Expressive]` member references |
| Runtime | `ExpressiveQueryCompiler` | EF Core decorator: expands before compilation |
| Runtime | `ExpressiveOptionsExtension` | Registers conventions + transformer pipeline |
| Runtime | Transformers | Adapt expression trees for LINQ provider compatibility |
| Runtime | `IExpressivePlugin` | Extension point for service + transformer registration |

## Key Difference from Projectables

EntityFrameworkCore.Projectables uses **syntax tree rewriting** -- it reads the C# syntax of a `[Projectable]` member and emits a new syntax tree that constructs the expression. This means it operates on the textual representation of the code.

ExpressiveSharp uses **semantic analysis via IOperation** -- it reads the compiler's fully resolved operation tree and maps each operation to the corresponding `Expression.*` factory call. This approach:

- Handles implicit conversions, operator overloads, and type inference automatically
- Supports complex language features (switch expressions, pattern matching, collection expressions) that would require extensive syntax rewriting
- Produces correct expression trees even when the syntax is ambiguous or sugar-coated (e.g., `?.` chains, string interpolation, checked arithmetic)

See the [IOperation to Expression Mapping](./ioperation-mapping) reference for the complete mapping table.
