# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Is

ExpressiveSharp is a Roslyn source generator that enables modern C# syntax (null-conditional `?.`, switch expressions, pattern matching) in LINQ expression trees, which normally only support a restricted subset of C#. It works by generating `Expression<TDelegate>` factory code at compile time from `[Expressive]`-decorated members.

## Build & Test Commands

```bash
dotnet restore
dotnet build --no-restore -c Release
dotnet test --no-build -c Release

# Run a single test project
dotnet test tests/ExpressiveSharp.Generator.Tests -c Release

# Run a single test by name
dotnet test --filter "FullyQualifiedName~MyTestName" -c Release

# Run tests against a specific target framework
dotnet test -f net8.0 -c Release

# Accept new Verify snapshots (for snapshot tests)
VERIFY_AUTO_APPROVE=true dotnet test tests/ExpressiveSharp.Generator.Tests

# Pack NuGet packages locally
dotnet pack -c Release

# Run benchmarks (all)
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj -- --filter "*"

# Run benchmarks (specific class)
dotnet run -c Release --project benchmarks/ExpressiveSharp.Benchmarks/ExpressiveSharp.Benchmarks.csproj -- --filter "*GeneratorBenchmarks*"
```

CI targets both .NET 8.0 and .NET 10.0 SDKs.

## Architecture

### Two-Phase Design

1. **Compile-time (source generation):** Two incremental generators in `ExpressiveSharp.Generator` (targets netstandard2.0):
   - `ExpressiveGenerator` — finds `[Expressive]` members, validates them via `ExpressiveInterpreter`, emits expression trees via `ExpressionTreeEmitter`, and builds a runtime registry via `ExpressionRegistryEmitter`
   - `PolyfillInterceptorGenerator` — uses C# 13 `[InterceptsLocation]` to rewrite `ExpressionPolyfill.Create()` and `IRewritableQueryable<T>` LINQ call sites from delegate form to expression tree form. Supports all standard `Queryable` methods, multi-lambda methods (Join, GroupJoin, GroupBy overloads), non-lambda-first methods (Zip, ExceptBy, etc.), and custom target types via `[PolyfillTarget]` (e.g., EF Core's `EntityFrameworkQueryableExtensions` for async methods)

2. **Runtime:** `ExpressiveResolver` looks up generated expressions by (DeclaringType, MemberName, ParameterTypes). `ExpressiveReplacer` is an `ExpressionVisitor` that substitutes `[Expressive]` member accesses with the generated expression trees. Transformers (in `Transformers/`) post-process trees for provider compatibility.

### Key Source Files

- `src/ExpressiveSharp.Generator/ExpressiveGenerator.cs` — main generator entry point
- `src/ExpressiveSharp.Generator/Emitter/ExpressionTreeEmitter.cs` — maps IOperation nodes to `Expression.*` factory calls (the heart of code generation). Uses `varPrefix` to ensure unique local variable names across multi-lambda emitters
- `src/ExpressiveSharp.Generator/Interpretation/ExpressiveInterpreter.cs` — validates and prepares `[Expressive]` members
- `src/ExpressiveSharp.Generator/PolyfillInterceptorGenerator.cs` — interceptor generation. Dedicated emitters for complex methods (Join, GroupJoin, GroupBy multi-lambda), enhanced generic fallback (`EmitGenericSingleLambda`) for single-lambda methods with non-lambda arg forwarding, `[PolyfillTarget]` support for custom target types
- `src/ExpressiveSharp/Services/ExpressiveResolver.cs` — runtime expression registry lookup
- `src/ExpressiveSharp/PolyfillTargetAttribute.cs` — specifies a non-`Queryable` target type for interceptor forwarding (e.g., `EntityFrameworkQueryableExtensions`)
- `src/ExpressiveSharp/Extensions/RewritableQueryableLinqExtensions.cs` — delegate-based LINQ stubs on `IRewritableQueryable<T>` (~85 intercepted + ~15 passthrough methods)
- `src/ExpressiveSharp.EntityFrameworkCore/Extensions/RewritableQueryableEfCoreExtensions.cs` — EF Core-specific stubs: chain-continuity (AsNoTracking, TagWith, etc.), Include/ThenInclude, and async lambda methods (AnyAsync, SumAsync, etc.)
- `src/ExpressiveSharp.EntityFrameworkCore/IIncludableRewritableQueryable.cs` — hybrid interface bridging `IIncludableQueryable` and `IRewritableQueryable` for Include/ThenInclude chain continuity

### Project Dependencies

```
ExpressiveSharp (core runtime, net8.0;net10.0)
  └── no external deps

ExpressiveSharp.Generator (source generator, netstandard2.0)
  └── Microsoft.CodeAnalysis.CSharp 5.0.0

ExpressiveSharp.EntityFrameworkCore (net8.0;net10.0)
  ├── ExpressiveSharp
  ├── EF Core 8.0.25 / 10.0.0
  └── Provides: ExpressiveDbSet<T>, IIncludableRewritableQueryable<T,P>,
      chain-continuity stubs, async lambda stubs with [PolyfillTarget]

ExpressiveSharp.EntityFrameworkCore.Relational (net8.0;net10.0)
  ├── ExpressiveSharp.EntityFrameworkCore
  ├── EF Core Relational 8.0.25 / 10.0.0
  └── Provides: Window functions (ROW_NUMBER, RANK, DENSE_RANK, NTILE),
      auto-discovered via IExpressivePlugin assembly attribute

ExpressiveSharp.EntityFrameworkCore.CodeFixers (Roslyn analyzer, netstandard2.0)
  └── Microsoft.CodeAnalysis.CSharp.Workspaces 4.12.0
  (packed into ExpressiveSharp.EntityFrameworkCore NuGet package)
```

### Diagnostics

12 diagnostic codes EXP0001–EXP0012 defined in `src/ExpressiveSharp.Generator/Infrastructure/Diagnostics.cs`. Key ones: EXP0001 (requires body), EXP0004 (block body requires opt-in), EXP0008 (unsupported operation).

## Testing

### Test Projects

| Project | Purpose |
|---------|---------|
| `ExpressiveSharp.Generator.Tests` | Snapshot tests (Verify.MSTest) — validates generated C# output |
| `ExpressiveSharp.Tests` | Unit tests for runtime services, transformers, extensions |
| `ExpressiveSharp.IntegrationTests` | End-to-end tests using the generator |
| `ExpressiveSharp.IntegrationTests.ExpressionCompile` | Compiles and invokes generated expression trees directly |
| `ExpressiveSharp.IntegrationTests.EntityFrameworkCore` | EF Core query translation validation |
| `ExpressiveSharp.EntityFrameworkCore.Tests` | EF Core integration-specific tests |
| `ExpressiveSharp.EntityFrameworkCore.Relational.Tests` | Window function SQL shape + integration tests (SQLite) |
| `ExpressiveSharp.Benchmarks` | BenchmarkDotNet performance benchmarks (generator, resolver, replacer, transformers, EF Core) |

### Three Verification Levels (see `docs/testing-strategy.md`)

1. **Compilation** — generated code compiles without errors
2. **Diagnostics** — no warnings (TreatWarningsAsErrors is enabled globally)
3. **Behavioral** — expression tree is functionally equivalent to the original code

Snapshot tests use `GeneratorTestBase.RunExpressiveGenerator()` to compile via Roslyn and compare output.

## Code Conventions

- `TreatWarningsAsErrors: true` — all warnings are errors
- `Nullable: enable` — full nullable reference types
- C# 12.0 on net8.0, C# 14.0 on net10.0
- Allman brace style, 4-space indentation, `var` preferred
- Instance fields: `_camelCase`
- Expression-bodied members preferred for methods/properties

## Gotchas

- **Generator targets netstandard2.0** — no modern C# APIs (no spans, no `Index`/`Range`, limited BCL). All generator code must compile against netstandard2.0 even though the rest of the solution uses C# 12+/14.
- **Snapshot tests** — `*.verified.cs` files alongside tests in `tests/ExpressiveSharp.Generator.Tests/` are the expected output. Commit them. Use `VERIFY_AUTO_APPROVE=true` to accept new baselines.
- **Incremental generator caching** — never store live Roslyn objects (symbols, syntax nodes) in cached pipeline state. Use immutable data snapshots with custom equality comparers.
- **Central Package Management** — all package versions are pinned in `Directory.Packages.props`. When adding a new dependency, add its version there (not in the individual `.csproj`).

## MSBuild Properties

- `Expressive_AllowBlockBody` — global default for block-bodied `[Expressive]` members (defaults to false; can also be set per-member via attribute)
