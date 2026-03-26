# Testing Strategy

This document defines the testing strategy for ExpressiveSharp.

## Design Principle: Emit the Closest Expression Node

The emitter maps each IOperation to the most direct `System.Linq.Expressions` equivalent.
For example, `IForEachLoopOperation` emits `Expression.Loop()` with the enumerator pattern —
not a direct LINQ rewrite. Transformers (like `ConvertLoopsToLinq`) handle the conversion to
LINQ for providers that can't translate `LoopExpression`.

This means:
- The **emitter** targets the full `System.Linq.Expressions` API surface
- **Transformers** adapt expression trees for specific providers (EF Core, etc.)
- The **ExpressionCompile runner** validates all expression types (compiles and executes in-process)
- The **EntityFrameworkCore runner** validates the EF Core-translatable subset

## Three Verification Levels

### Level 1: Compilation — Does the generated code compile?

The source generator produces C# code that builds an `Expression<TDelegate>` using factory methods.
This generated code must compile without errors.

**Test mechanism:** `GeneratorTestBase.RunExpressiveGenerator()` compiles the generated output
via Roslyn and asserts zero diagnostics.

### Level 2: Diagnostics — Does it compile without warnings?

`TreatWarningsAsErrors` is enabled project-wide. The generator reports diagnostics (EXP0001–EXP0012)
for unsupported constructs, and these are verified by tests.

### Level 3: Behavioral — Does the expression tree produce correct results?

The generated expression tree must be functionally equivalent to the original method body.
Integration tests seed canonical data, execute queries via expanded expression trees, and
assert results match expected values.

## Test Projects

| Project | Purpose | Level |
|---------|---------|-------|
| `ExpressiveSharp.Generator.Tests` | Snapshot tests (Verify.MSTest) — validates generated C# output | 1, 2 |
| `ExpressiveSharp.Tests` | Unit tests for runtime services, transformers, extensions | 3 |
| `ExpressiveSharp.IntegrationTests` | Shared abstract test classes, Store scenario models, seed data | — |
| `ExpressiveSharp.IntegrationTests.ExpressionCompile` | Compiles expression trees to delegates, executes in-memory | 3 |
| `ExpressiveSharp.IntegrationTests.EntityFrameworkCore` | EF Core + SQLite query translation and execution | 3 |
| `ExpressiveSharp.EntityFrameworkCore.Tests` | EF Core-specific hooks (conventions, ExpressiveDbSet) | 3 |

## Integration Test Architecture

Three-project structure:

- **ExpressiveSharp.IntegrationTests** — shared class library (not MSTest.Sdk) with Store scenario
  models (`Order`, `Customer`, `LineItem`, `Address`), seed data, and abstract test classes.
  The generator runs here to produce expression registries for `[Expressive]` models.

- **ExpressiveSharp.IntegrationTests.ExpressionCompile** — compiles expression trees to delegates
  and executes against in-memory `List<T>`. Validates all expression types including `LoopExpression`.

- **ExpressiveSharp.IntegrationTests.EntityFrameworkCore** — executes against SQLite via EF Core
  with `UseExpressives()`. Validates the LINQ-translatable subset. Transformers (`ConvertLoopsToLinq`,
  `RemoveNullConditionalPatterns`, `FlattenBlockExpressions`) run before queries.

Concrete test classes are one-liners that inherit abstract tests and override `CreateRunner()`:

```csharp
[TestClass]
public class ArithmeticTests : Scenarios.Common.Tests.ArithmeticTests
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
```

To add a new integration (e.g., NHibernate): implement `IIntegrationTestRunner`, create concrete subclasses.

## Adding Tests for a New IOperation Mapping

When implementing a new `EmitOperation` case:

1. **Snapshot test** — add a test in `ExpressiveSharp.Generator.Tests` that feeds source code
   through the generator and verifies the output compiles and matches a snapshot.

2. **Integration test** — add an `[Expressive]` member to the Store model or use
   `ExpressionPolyfill.Create(...)`, then add assertions in an abstract test class.
   Both ExpressionCompile and EntityFrameworkCore runners pick it up automatically.

3. **Transformer unit test** — if the feature requires a transformer for LINQ provider
   compatibility, add tests in `ExpressiveSharp.Tests/Transformers/` that build expression
   trees manually and verify the transformer rewrites them correctly.

## Performance Benchmarks

Beyond correctness testing, the project includes BenchmarkDotNet benchmarks in
`benchmarks/ExpressiveSharp.Benchmarks/` that track performance across key hot paths:

| Benchmark class | What it measures |
|-----------------|-----------------|
| `GeneratorBenchmarks` | Cold and incremental `ExpressiveGenerator` runs (parameterized by member count) |
| `PolyfillGeneratorBenchmarks` | Cold and incremental `PolyfillInterceptorGenerator` runs |
| `ExpressionResolverBenchmarks` | Registry vs. reflection lookup for properties, methods, constructors |
| `ExpressionReplacerBenchmarks` | `ExpressiveReplacer.Replace` on various expression tree shapes |
| `TransformerBenchmarks` | Each transformer in isolation + full `ExpandExpressives` pipeline |
| `EFCoreQueryOverheadBenchmarks` | End-to-end EF Core `ToQueryString()` overhead + cold-start cost |

### CI Regression Detection

A separate GitHub Actions workflow (`.github/workflows/benchmarks.yml`) runs benchmarks on every
push to `main` and on pull requests. Results are stored on the `gh-pages` branch and compared
against the last `main` baseline using `benchmark-action/github-action-benchmark`. PRs that
regress beyond 20% receive an automated comment.

### Running Benchmarks Locally

```bash
# Run all benchmarks (full BenchmarkDotNet defaults)
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj -- --filter "*"

# Run a specific class
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj -- --filter "*GeneratorBenchmarks*"

# Quick run (CI-style, fewer iterations)
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj -- --filter "*" --job short --iterationCount 3 --warmupCount 1
```

## Running Tests

```bash
# All tests
dotnet test

# Single project
dotnet test tests/ExpressiveSharp.Generator.Tests

# Accept new snapshots
VERIFY_AUTO_APPROVE=true dotnet test tests/ExpressiveSharp.Generator.Tests

# Single test by name
dotnet test --filter "FullyQualifiedName~MyTestName"
```
