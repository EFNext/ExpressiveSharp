# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ExpressiveSharp is a C# source generator that enables modern C# syntax (e.g., null-conditional operators `?.`) to work inside LINQ expression trees, which normally only support a restricted subset of C# syntax.

## Build & Test Commands

```bash
dotnet restore              # Restore dependencies
dotnet build                # Build all projects
dotnet test                 # Run all tests
dotnet pack                 # Build NuGet packages

# Run a single test project
dotnet test tests/ExpressiveSharp.Generator.Tests/

# Run tests matching a filter
dotnet test --filter FullyQualifiedName~SomeTestName

# Accept new Verify snapshots
VERIFY_AUTO_APPROVE=true dotnet test
```

> **Note:** This project uses MSTest.Sdk with Microsoft.Testing.Platform (MTP). On .NET 10 SDK, `dotnet test` (VSTest path) is incompatible with MTP; use `dotnet msbuild -t:Test` instead.

## Code Style

Enforced via `.editorconfig`:
- **Allman braces** (opening brace on new line)
- `var` preferred for local variables
- Private fields: `camelCase` with leading underscore (`_fieldName`)
- C# 12 language features (C# 14 on `net10.0` target)
- `TreatWarningsAsErrors: true` — all warnings are errors

## Architecture

The library has two projects in `src/`:
- **ExpressiveSharp** — public API (`[Expressive]` attribute, `IRewritableQueryable<T>`, `ExpressionPolyfill`, enums) + runtime services (`ExpressiveResolver`, extension methods)
- **ExpressiveSharp.Generator** — Roslyn source generators; targets `netstandard2.0`

### Two Source Generators

**1. `ExpressiveGenerator`** (`src/ExpressiveSharp.Generator/ExpressiveGenerator.cs`)
- Finds members decorated with `[Expressive]`
- Generates `Expression<Func<...>>` method bodies and registers them in a per-assembly expression registry
- The registry maps `MemberInfo` → `LambdaExpression` for runtime lookup via `ExpressiveResolver`

**2. `PolyfillInterceptorGenerator`** (`src/ExpressiveSharp.Generator/PolyfillInterceptorGenerator.cs`)
- Uses C# 13 **method interceptors** (`[InterceptsLocation]`) to replace calls at specific call sites at compile time
- Intercepts `ExpressionPolyfill.Polyfill<TDelegate>()` calls — converts the lambda argument into an expression tree
- Intercepts `IRewritableQueryable<T>` LINQ extension methods (`Where`, `Select`, `OrderBy`, `ThenBy`, `GroupBy`) — rewrites delegate-based overloads to expression-based ones
- The `InterceptorsNamespaces` MSBuild property (set by `ExpressiveSharp.Generator.props`) enables this feature

### Expression Tree Emitter

`ExpressionTreeEmitter` (`src/ExpressiveSharp.Generator/Emitter/ExpressionTreeEmitter.cs`) walks the Roslyn `IOperation` tree for expression-bodied members and emits C# code that builds the equivalent `Expression<TDelegate>` using factory methods. Handles: literals, parameters, member access, invocations, binary/unary operators, conversions, conditionals, object creation, arrays, lambdas, tuples, pattern matching, switch expressions, and null-conditional access.

Block-bodied members are also handled by the emitter via `IBlockOperation` and `IReturnOperation`. Loop constructs (`foreach`, `for`) within block bodies are converted to LINQ equivalents (e.g., `.Sum()`, `.All()`) by `LoopConverter` before emission.

### Integration Tests

Three-project structure in `tests/`:
- **ExpressiveSharp.IntegrationTests** — shared class library (not MSTest.Sdk) with Store scenario models, seed data, and abstract test classes. References `MSTest.TestFramework` directly for `[TestMethod]`/`Assert`. Generator runs here (Analyzer reference) to produce expression registries for `[Expressive]` models.
- **ExpressiveSharp.IntegrationTests.ExpressionCompile** — compiles expression trees to delegates and executes against in-memory `List<T>`
- **ExpressiveSharp.IntegrationTests.EntityFrameworkCore** — executes against SQLite via EF Core with `UseExpressives()`

Concrete test classes are one-liners that inherit abstract tests and override `CreateRunner()`. To add a new integration (e.g., NHibernate): implement `IIntegrationTestRunner`, create concrete subclasses.

### Snapshot Tests

Tests use **MSTest** + **Verify.MSTest** for snapshot verification. Expected outputs live as `*.verified.txt` files alongside test source files, organized by subfolder (`ExpressiveGenerator/`, `PolyfillInterceptorGenerator/`). When generator output changes, accept new snapshots with:

```bash
VERIFY_AUTO_APPROVE=true dotnet msbuild -t:Test tests/ExpressiveSharp.Generator.Tests/
```

The test base class scrubs `[InterceptsLocation]` attribute arguments because they encode file paths.

### MSBuild Integration

- `Directory.Build.props` — global build settings (multi-targeting `net8.0;net10.0`, warnings-as-errors, nullable)
- `Directory.Packages.props` — centralized NuGet version pinning (edit here, not in individual `.csproj`)
- EF Core package references use `VersionOverride` per TFM — `8.0.x` for `net8.0`, `10.0.x` for `net10.0`. Do not merge into a single unconditional reference.
- `src/ExpressiveSharp.Abstractions/build/ExpressiveSharp.Abstractions.props` — exposes MSBuild properties: `Expressive_Disable` (comma-separated `ExpressionFeature` flags), `Expressive_NullConditionalMode`
