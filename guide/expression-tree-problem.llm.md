# The Expression Tree Problem

Expression trees (`System.Linq.Expressions`) were introduced alongside LINQ in C# 3.0 and have been one of the most powerful features of the .NET platform -- they allow C# code to be represented as data, inspected, and translated to other languages like SQL. But the C# compiler's ability to convert lambdas into expression trees has been essentially frozen since 2007.

.NET 4.0 *did* add new node types to `System.Linq.Expressions` (Block, Loop, Try/Catch, Switch, and others) for the Dynamic Language Runtime, but the C# compiler was never updated to emit them. Every language feature added after C# 3.0 -- null-conditional `?.`, switch expressions, pattern matching, tuples, and many more -- produces a compiler error when used inside an `Expression<Func<...>>` lambda.

This page explains why that gap exists, why it is unlikely to close through official channels, and how ExpressiveSharp sidesteps the problem entirely.

## A Brief History

| Year | Event |
|------|-------|
| 2007 | C# 3.0 ships with `System.Linq.Expressions` and LINQ. Expression trees support the full C# 3.0 lambda feature set. |
| 2010 | .NET 4.0 adds statement-level node types (Block, Loop, Try/Catch, Switch, Goto, Label, assignment) for DLR support. The C# compiler is never updated to emit them -- they remain a runtime-only API. |
| 2015--2024 | C# adds `?.` (C# 6), pattern matching (C# 7), switch expressions (C# 8), tuples (C# 7), index/range (C# 8), records (C# 9), collection expressions (C# 12), and more. None work in expression tree lambdas. |
| 2017 | The Roslyn compiler migration reveals deep compatibility issues with expression tree generation (more on this below). |
| ~2020 | `System.Linq.Expressions` is [effectively archived](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/README.md) -- no feature contributions accepted. Bug and regression fixes only. |
| 2025 | A senior .NET team member [explains publicly](https://github.com/dotnet/csharplang/discussions/158#discussioncomment-11793200) why the team has not invested further in expression tree enhancements. |

## Why It Hasn't Been Fixed

### The Roslyn Migration Revealed the Fragility

The original native C++ compiler effectively dumped its internal data structures into the expression tree API. There was -- and still is -- no specification for what tree shape the compiler should produce for a given C# construct. Consumers built their LINQ providers against whatever the compiler happened to generate.

When the C# compiler was rewritten in managed code (Roslyn), the team had to reproduce these same tree shapes from a completely different internal representation. They did a good job, but not a perfect one. Some tree shapes changed subtly. Providers broke.

The team fixed the bugs. Then providers that had *adapted to the new shapes* filed new bugs when the fixes reverted them. A vicious cycle.

::: info Source
A senior .NET team member [described this experience in detail](https://github.com/dotnet/csharplang/discussions/158#discussioncomment-11793200): the native compiler had a "kind of buggy or incorrect internal representation" for some syntax, and dumping those implementation details into public surface area made them nearly impossible to change. Even bug fixes caused cascading breaks in downstream providers.
:::

### Every Provider Breaks Simultaneously

Adding a new type like `DateOnly` to the BCL is an *additive, opt-in* change -- a provider must explicitly write code to handle it. Expression tree nodes work differently. New nodes are *implicitly consumed*: every `ExpressionVisitor` in every LINQ provider encounters them automatically, without any acknowledgement or opt-in. The default behavior is to throw.

This means that when the compiler introduces a new node type, existing code that never asked for the feature now breaks. There is no mechanism for a provider to say "I support up to version X of expression trees."

::: info Source
A Roslyn team member [explained](https://github.com/dotnet/csharplang/discussions/158) that the significant cost of new nodes "does not come from the compiler -- it's fairly easy to do in the compiler. Its significant cost comes from every downstream tool now being broken because there is a new node in the tree they were not expecting."

In a separate [discussion](https://github.com/dotnet/csharplang/discussions/158), a Roslyn team member noted: "The single greatest compat challenge the Roslyn compiler faced was getting expression trees into a shape that did not massively break customers. This is not hypothetical -- it is 'we've done this before and observed it breaking.'"
:::

### The Catch-22

These forces create a self-reinforcing loop:

1. Providers are fragile, so the team does not add new nodes.
2. Because no new nodes are added, providers never learn to handle them.
3. The gap widens with every C# release, making the eventual migration even more disruptive.
4. The more disruptive the migration would be, the harder it is to justify.

This is an engineering reality, not a failure of will. The .NET team's commitment to backward compatibility is what makes the platform reliable for production use -- but it also means some problems cannot be fixed incrementally.

## What You Cannot Write Today

These are some of the most commonly encountered restrictions. The full list is longer -- essentially any C# syntax added after C# 3.0 is rejected in expression tree lambdas.

| Feature | Introduced | Compiler Error |
|---------|-----------|----------------|
| Null-conditional `?.` | C# 6 (2015) | CS8072 |
| String interpolation | C# 6 (2015) | CS8076 |
| Pattern matching (`is`) | C# 7 (2017) | CS8122 |
| Tuple literals | C# 7 (2017) | CS8143 |
| Switch expressions | C# 8 (2019) | CS8514 |
| Index / range (`^1`, `1..3`) | C# 8 (2019) | CS8790 |
| `with` expressions (records) | C# 9 (2020) | CS8849 |
| Collection expressions | C# 12 (2023) | CS9175 |

In practice, this forces EF Core users to write verbose workarounds:

```csharp
// What you want to write
db.Orders.Where(o =>
    o.Customer?.Email != null                // CS8072
    && o.Status switch {                     // CS8514
        OrderStatus.Active => true,
        _ => false
    }
    && o.Tags is [var first, ..]             // CS8122
);

// What you actually have to write
db.Orders.Where(o =>
    (o.Customer != null ? o.Customer.Email : null) != null
    && (o.Status == OrderStatus.Active ? true : false)
    // ... and list patterns simply cannot be expressed at all
);
```

::: tip
ExpressiveSharp eliminates all of these restrictions. See the [Quick Start](./quickstart) to get running in five minutes.
:::

## Proposed Solutions and Why They Stalled

Several approaches have been discussed in the [community](https://github.com/dotnet/csharplang/discussions/158) and within Microsoft over the years:

| Proposal | Idea | Status |
|----------|------|--------|
| Expression tree versioning | Compiler emits v1 nodes by default; providers opt into v2 | Never specified. No consensus on versioning semantics. |
| New namespace (v2) | `System.Linq.Expressions.V2` with a clean break | Would require every provider to support both old and new APIs indefinitely. |
| Reducible nodes | New nodes carry a `Reduce()` method that lowers to old nodes | [Prototyped by Bart de Smet](https://github.com/bartdesmet/ExpressionFutures). Providers still break if they inspect rather than reduce. |
| Source generators + interceptors | Generate expression trees at compile time, bypassing the runtime API limitations | **This is the approach ExpressiveSharp takes.** |

::: warning
`System.Linq.Expressions` is [effectively archived](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/README.md). The library accepts bug and regression fixes only -- no new features will be added. Any solution must work outside the runtime library.
:::

## AOT and Accessibility Concerns

Two additional reasons the .NET team has signaled preference for source generation over expression tree enhancements:

**Native AOT incompatibility.** `Expression<T>.Compile()` requires the JIT compiler at runtime. In Native AOT deployments, calling `Compile()` throws. Expression trees that need to be evaluated in-process -- rather than translated to SQL -- are fundamentally incompatible with AOT. Source-generated code has no such limitation.

**Accessibility violations.** Expression trees can reference private members, bypass `protected` visibility, and access internals across assembly boundaries. The compiler does not enforce the same access checks it applies to regular code. Source generators operate within the normal compilation model and respect all accessibility rules.

## Why a Library Can Do What the Framework Cannot

The .NET team cannot afford to break the entire ecosystem. A library can afford to take risks.

**Opt-in risk.** ExpressiveSharp is a package you consciously add. If a generated tree shape causes a translation failure with your LINQ provider, you can remove the attribute, adjust the code, or file an issue. The blast radius is one project, not every .NET application in existence.

**Different compatibility contract.** The .NET runtime promises near-perfect backward compatibility across major versions. A library can ship breaking changes in a major version, communicate them in release notes, and move on. This freedom is exactly what allows ExpressiveSharp to iterate on tree shapes without the paralysis that affects the core framework.

**Stable node types, novel composition.** ExpressiveSharp emits only well-known node types that have existed since 2007 -- `ConditionalExpression`, `BinaryExpression`, `MethodCallExpression`, and others. Modern syntax is lowered to *combinations* of these nodes. We expect this to be compatible with most providers, but we cannot guarantee it for every provider in every scenario -- and that's OK. A library-level compatibility issue is a solvable problem; a framework-level one is an ecosystem crisis.

**Community-driven iteration.** When a provider does not handle a particular tree shape, the fix lands in days or weeks -- a new transformer, a workaround, a diagnostic. The framework operates on yearly release cycles with multi-year planning horizons.

## How ExpressiveSharp Sidesteps the Problem

ExpressiveSharp does not wait for `System.Linq.Expressions` to be updated. It uses Roslyn source generators to analyze your code at the semantic level (via `IOperation`) and emit `Expression.*` factory calls at compile time. The generated trees use only the existing, stable node types that every provider already understands.

Modern syntax -- `?.`, switch expressions, pattern matching, string interpolation, tuples, list patterns, and more -- is lowered to combinations of `ConditionalExpression`, `BinaryExpression`, `MethodCallExpression`, and other nodes that have existed since 2007. This is conceptually the same as what the "reducible nodes" proposal would have done, but it happens at compile time with full access to the compiler's semantic model.

Because the generated trees contain only well-known node types, they are expected to work with providers that handle standard expression trees -- but this is an expectation based on using stable APIs, not a tested guarantee for every provider. The source-generation approach also avoids the AOT and accessibility concerns: there is no `Compile()` call, and all generated code respects normal C# visibility rules.

::: tip
For a detailed walkthrough of the source generation pipeline, see [How It Works](../advanced/how-it-works).
:::

## Further Reading

- [Quick Start](./quickstart) -- install and run your first query
- [How It Works](../advanced/how-it-works) -- detailed source generation pipeline
- [IOperation to Expression Mapping](../advanced/ioperation-mapping) -- how each C# construct is lowered
- [Limitations](../advanced/limitations) -- what ExpressiveSharp currently cannot do
- [dotnet/csharplang #158](https://github.com/dotnet/csharplang/discussions/158) -- the original discussion thread on expression tree modernization
