# Introduction

**ExpressiveSharp** is a Roslyn source generator that enables modern C# syntax in LINQ expression trees. It generates `Expression<TDelegate>` factory code at compile time, so you can use null-conditional operators, switch expressions, pattern matching, and more in queries that target EF Core or any LINQ provider.

## The Two Problems

When using C# with LINQ providers like EF Core, you hit two walls:

### 1. Expression tree syntax restrictions

You write a perfectly reasonable query and hit:

```
error CS8072: An expression tree lambda may not contain a null propagating operator
```

Expression trees (`Expression<Func<...>>`) only support a restricted subset of C#. There is no `?.`, no switch expressions, no pattern matching. You end up writing ugly ternary chains instead of the clean code you would write anywhere else. For the full story of why this restriction has persisted since 2007, see [The Expression Tree Problem](./expression-tree-problem).

### 2. Computed properties are opaque to LINQ providers

You define `public string FullName => FirstName + " " + LastName` and use it in a query, but EF Core cannot see inside the property getter. It either throws a runtime translation error, or worse, silently fetches the entire entity to evaluate `FullName` on the client (overfetching). The only workaround is to duplicate the logic as an inline expression in every query that needs it.

## How ExpressiveSharp Works

ExpressiveSharp fixes both problems with a two-phase design:

### Compile time (source generation)

Two Roslyn incremental source generators analyze your code:

1. **ExpressiveGenerator** finds members decorated with `[Expressive]`, validates them, and emits `Expression.*` factory code that builds the equivalent expression tree. These are registered in a per-assembly expression registry.

2. **PolyfillInterceptorGenerator** uses C# 13 method interceptors to rewrite `ExpressionPolyfill.Create()` calls and `IRewritableQueryable<T>` LINQ method calls, converting delegate lambdas into expression trees at their call sites.

### Runtime (expression expansion)

When you query with EF Core (via `UseExpressives()`) or call `.ExpandExpressives()` manually, an `ExpressionVisitor` walks the query tree and replaces opaque `[Expressive]` member accesses with the pre-built expression trees. Transformers then adapt the trees for the target provider (stripping null-conditional patterns for SQL, flattening blocks, etc.).

```
Your LINQ query
    -> [Expressive member accesses replaced with generated expressions]
    -> [Transformers adapt for SQL provider]
    -> Expanded expression tree
    -> EF Core SQL translation
    -> SQL query
```

All expression trees are generated at compile time. There is no runtime reflection or expression compilation.

## Which API Should I Use?

Mark computed properties and methods with `[Expressive]` to generate companion expression trees. Then choose how to wire them into your queries:

| Scenario | API |
|---|---|
| **EF Core** -- modern syntax + `[Expressive]` expansion on `DbSet` | [`ExpressiveDbSet<T>`](./ef-core-integration) (or [`UseExpressives()`](./ef-core-integration) for global expansion) |
| **Any `IQueryable`** -- modern syntax + `[Expressive]` expansion | [`.AsExpressive()`](./rewritable-queryable) |
| **EF Core** -- SQL window functions (ROW_NUMBER, RANK, etc.) | [`WindowFunction.*`](./window-functions) (install `RelationalExtensions` package) |
| **Advanced** -- build an `Expression<T>` inline, no attribute needed | [`ExpressionPolyfill.Create`](./expression-polyfill) |
| **Advanced** -- expand `[Expressive]` members in an existing expression tree | `.ExpandExpressives()` |
| **Advanced** -- make third-party/BCL members expressable | `[ExpressiveFor]` |

## Comparison with Similar Libraries

| Feature | ExpressiveSharp | Projectables | Expressionify | LinqKit |
|---|---|---|---|---|
| Source generator based | Yes | Yes | Yes | No |
| Works with entity methods | Yes | Yes | Yes | Partial |
| Works with extension methods | Yes | Yes | Yes | Yes |
| Composable members | Yes | Yes | No | Partial |
| Constructor projections | Yes | Yes | No | No |
| Null-conditional `?.` rewriting | Yes | Yes | No | No |
| Switch expressions / pattern matching | Yes | Yes | No | No |
| Block-bodied members | Yes (experimental) | Yes (experimental) | No | No |
| Enum method expansion | Yes | Yes | No | No |
| Inline expression creation | Yes (`ExpressionPolyfill.Create`) | No | No | No |
| Modern syntax in LINQ chains | Yes (`IRewritableQueryable<T>`) | No | No | No |
| SQL window functions | Yes (RelationalExtensions) | No | No | No |
| String interpolation support | Yes | No | No | No |
| Tuple literals support | Yes | No | No | No |
| List patterns / index-range | Yes | No | No | No |
| `with` expressions (records) | Yes | No | No | No |
| Collection expressions | Yes | No | No | No |
| Customizable transformer pipeline | Yes | No | No | No |
| Plugin architecture (EF Core) | Yes | No | No | No |
| External member mapping | Yes (`[ExpressiveFor]`) | No | No | No |
| Not coupled to EF Core | Yes | No | No | No |
| Roslyn analyzers and code fixes | Yes | Yes | No | No |

## Requirements

| | .NET 8.0 | .NET 10.0 |
|---|---|---|
| **ExpressiveSharp** | C# 12 | C# 14 |
| **ExpressiveSharp.Abstractions** | C# 12 | C# 14 |
| **ExpressiveSharp.EntityFrameworkCore** | EF Core 8.x | EF Core 10.x |
| **ExpressiveSharp.EntityFrameworkCore.RelationalExtensions** | EF Core 8.x | EF Core 10.x |

## Next Steps

- [Quick Start](./quickstart) -- install, configure, and run your first query
- [[Expressive] Properties](./expressive-properties) -- computed properties translated to SQL
- [EF Core Integration](./ef-core-integration) -- full setup for Entity Framework Core
