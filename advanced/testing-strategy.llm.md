# Testing Strategy

This document defines the testing strategy for ExpressiveSharp, covering snapshot tests, functional tests, integration test architecture, and performance benchmarks.

## Design Principle: Emit the Closest Expression Node

The emitter maps each `IOperation` to the most direct `System.Linq.Expressions` equivalent. For example, `IForEachLoopOperation` emits `Expression.Loop()` with the enumerator pattern -- not a direct LINQ rewrite. Transformers (like `ConvertLoopsToLinq`) handle the conversion to LINQ for providers that cannot translate `LoopExpression`.

This means:

- The **emitter** targets the full `System.Linq.Expressions` API surface
- **Transformers** adapt expression trees for specific providers (EF Core, etc.)
- The **ExpressionCompile runner** validates all expression types (compiles and executes in-process)
- The **EntityFrameworkCore runner** validates the EF Core-translatable subset

## Three Verification Levels

### Level 1: Compilation -- Does the generated code compile?

The source generator produces C# code that builds an `Expression<TDelegate>` using factory methods. This generated code must compile without errors.

**Test mechanism:** `GeneratorTestBase.RunExpressiveGenerator()` compiles the generated output via Roslyn and asserts zero diagnostics.

### Level 2: Diagnostics -- Does it compile without warnings?

`TreatWarningsAsErrors` is enabled project-wide. The generator reports diagnostics (EXP0001 through EXP0012) for unsupported constructs, and these are verified by tests.

### Level 3: Behavioral -- Does the expression tree produce correct results?

The generated expression tree must be functionally equivalent to the original method body. Integration tests seed canonical data, execute queries via expanded expression trees, and assert results match expected values.

## Test Projects

| Project | Purpose | Level |
|---|---|---|
| `ExpressiveSharp.Generator.Tests` | Snapshot tests (Verify.MSTest) -- validates generated C# output | 1, 2 |
| `ExpressiveSharp.Tests` | Unit tests for runtime services, transformers, extensions | 3 |
| `ExpressiveSharp.IntegrationTests` | In-memory compiled-delegate tests (ExpressionCompile path). Also hosts the shared Store scenario models (`Order`, `Customer`, `LineItem`, `Address`) and seed data used by both test projects. | 3 |
| `ExpressiveSharp.EntityFrameworkCore.IntegrationTests` | All EF Core integration tests (scenarios, async queryables, window functions, ExecuteUpdate, Include, query filters, conventions). Default: SQLite. Containers: SqlServer/Postgres/PomeloMySql/Cosmos via `-p:TestDatabase=<provider>` | 3 |

## Integration Test Architecture

Two test projects, one feature set per backend — no runner abstraction:

### ExpressionCompile (in-memory)

**`ExpressiveSharp.IntegrationTests`** -- Contains the shared Store scenario models / seed data plus `[TestClass]` test classes (`CommonScenarioTests`, `StoreQueryTests`, `AnonymousTypeInterceptorTests`, `MultiLambdaInterceptorTests`). Each test compiles an expanded expression tree to a delegate via `.Compile()` and runs it against seeded `List<T>` collections through the internal `ExpressionCompileRunner` helper. Validates all expression types including `LoopExpression`.

### EntityFrameworkCore

**`ExpressiveSharp.EntityFrameworkCore.IntegrationTests`** -- Two-level test base hierarchy:
- **`EFCoreTestBase`** — base lifecycle (context create / `EnsureCreated` / per-test database drop). Derived test bases that work on any EF Core provider (including Cosmos) extend this. `Context` is typed as `DbContext` so tests use `Context.Set<T>()`.
- **`EFCoreRelationalTestBase : EFCoreTestBase`** — adds a strongly-typed `IntegrationTestDbContext Context` shadow. Derived test bases that require relational features (window functions, `ExecuteUpdate`, typed DbSet access) extend this; Cosmos concrete classes never do.

Feature test bases:
- Extend `EFCoreTestBase`: `CommonScenarioTestBase`, `StoreQueryTestBase` (run on Cosmos + all relational providers)
- Extend `EFCoreRelationalTestBase`: `WindowFunctionTestBase`, `ExecuteUpdateTestBase`, `AsyncQueryableTestBase`, `CapturedVariableTestBase`, `IncludeTestBase`, `UseExpressivesConventionTestBase`, `ExpressiveExpansionTestBase`
- Standalone (own DbContext): `QueryFilterTestBase`

Per-provider concrete classes live under `Tests/<Provider>/` and implement `CreateContextHandle` using `TestContextFactories.Create<Provider>()`. Default `dotnet test` runs SQLite only; `-p:TestDatabase=<provider>` enables container-backed testing (see `.test-containers.sh`).

## Adding Tests for a New IOperation Mapping

When implementing a new `EmitOperation` case:

1. **Snapshot test** -- Add a test in `ExpressiveSharp.Generator.Tests` that feeds source code through the generator and verifies the output compiles and matches a snapshot.

2. **Integration test** -- Add an `[Expressive]` member to the Store model or use `ExpressionPolyfill.Create(...)`, then add assertions in an abstract test class. Both ExpressionCompile and EntityFrameworkCore runners pick it up automatically.

3. **Transformer unit test** -- If the feature requires a transformer for LINQ provider compatibility, add tests in `ExpressiveSharp.Tests/Transformers/` that build expression trees manually and verify the transformer rewrites them correctly.

## Snapshot Tests

Snapshot tests use [Verify.MSTest](https://github.com/VerifyTests/Verify) to compare generated C# output against committed `*.verified.cs` files.

::: tip Accepting New Snapshots
When generated output changes (new feature, bugfix, etc.), accept the new baselines:

```bash
VERIFY_AUTO_APPROVE=true dotnet test tests/ExpressiveSharp.Generator.Tests
```

Review the diff in `*.verified.cs` files before committing.
:::

::: warning
The `*.verified.cs` files alongside tests in `tests/ExpressiveSharp.Generator.Tests/` are the expected output. Always commit them with your changes.
:::

## Running Tests

```bash
# All tests
dotnet test

# Single project
dotnet test tests/ExpressiveSharp.Generator.Tests

# Single test by name
dotnet test --filter "FullyQualifiedName~MyTestName"

# Run against a specific target framework
dotnet test -f net8.0 -c Release
```

CI targets both .NET 8.0 and .NET 10.0 SDKs.

## Performance Benchmarks

Beyond correctness testing, the project includes BenchmarkDotNet benchmarks in `benchmarks/ExpressiveSharp.Benchmarks/` that track performance across key hot paths.

| Benchmark class | What it measures |
|---|---|
| `GeneratorBenchmarks` | Cold and incremental `ExpressiveGenerator` runs (parameterized by member count) |
| `PolyfillGeneratorBenchmarks` | Cold and incremental `PolyfillInterceptorGenerator` runs |
| `ExpressionResolverBenchmarks` | Registry vs. reflection lookup for properties, methods, constructors |
| `ExpressionReplacerBenchmarks` | `ExpressiveReplacer.Replace` on various expression tree shapes |
| `TransformerBenchmarks` | Each transformer in isolation + full `ExpandExpressives` pipeline |
| `EFCoreQueryOverheadBenchmarks` | End-to-end EF Core `ToQueryString()` overhead + cold-start cost |

### CI Regression Detection

A separate GitHub Actions workflow (`.github/workflows/benchmarks.yml`) runs benchmarks on every push to `main` and on pull requests. Results are stored on the `gh-pages` branch and compared against the last `main` baseline using `benchmark-action/github-action-benchmark`. PRs that regress beyond 20% receive an automated comment.

### Running Benchmarks Locally

```bash
# Run all benchmarks (full BenchmarkDotNet defaults)
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj \
    -- --filter "*"

# Run a specific class
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj \
    -- --filter "*GeneratorBenchmarks*"

# Quick run (CI-style, fewer iterations)
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj \
    -- --filter "*" --job short --iterationCount 3 --warmupCount 1
```

### Benchmark Dashboard

The benchmark results are published to an interactive dashboard at [`/dev/bench/`](https://efnext.github.io/ExpressiveSharp/dev/bench/). The dashboard tracks the following metrics over time:

- **Generator cold start** -- Time for the `ExpressiveGenerator` to process a compilation from scratch
- **Generator incremental** -- Time for an incremental re-run after a non-semantic edit
- **Resolver lookup** -- Time for `ExpressiveResolver.FindGeneratedExpression` via registry and reflection paths
- **Replacer traversal** -- Time for `ExpressiveReplacer` to walk and replace expression trees of varying complexity
- **Transformer pipeline** -- Per-transformer and end-to-end pipeline throughput
- **EF Core overhead** -- The delta between a vanilla EF Core query and one with `UseExpressives()` expansion

Use the dashboard to identify regressions before they reach production and to evaluate the performance impact of new features.
