# Testing Strategy

This document defines the testing strategy for verifying IOperation-to-Expression mappings
implemented in `ExpressionTreeEmitter`.

## Design Principle: Emit the Closest Expression Node

The emitter should map each IOperation to the most direct `System.Linq.Expressions` equivalent,
even when that node type is not yet supported by downstream consumers like EF Core.

For example, `ILoopOperation` should emit `Expression.Loop()` — not a manual unrolling or LINQ
rewrite. The fact that EF Core cannot currently translate `LoopExpression` is not a reason to
avoid it. A compatibility layer will be added on top of EF Core to rewrite these nodes into
translatable equivalents before query execution.

This means:
- The **emitter** targets the full `System.Linq.Expressions` API surface
- The **EF Core consumer** validates only the subset it can translate today
- The **direct `Compile()` + invoke** consumer validates all expression types (it runs in-process, no SQL translation needed)
- Tests that use expression nodes not yet supported by EF Core should run against the visitor and direct-invoke consumers, and skip the EF Core consumer until the compatibility layer is in place

## Three Verification Levels

Every IOperation mapping must be validated at three levels:

### Level 1: Compilation — Does the generated code compile?

The source generator produces C# code that builds an `Expression<TDelegate>` using factory methods.
This generated code must compile without errors when included in the target project.

**What can go wrong:**
- Incorrect type names (not fully qualified, wrong generic arguments)
- Invalid `Expression.*` factory calls (wrong argument count, type mismatches)
- Missing reflection field declarations (`MethodInfo`, `PropertyInfo`, etc.)
- Referencing symbols that don't exist at runtime

**Test mechanism:** `GeneratorTestBase.RunExpressiveGenerator()` compiles the generated output
via Roslyn. The test asserts `result.Diagnostics.Length == 0`.

```csharp
var result = RunExpressiveGenerator(compilation);
Assert.AreEqual(0, result.Diagnostics.Length);
```

### Level 2: Diagnostics — Does it compile without warnings?

`TreatWarningsAsErrors` is enabled project-wide. The generator itself should not produce
unexpected diagnostics, and the generated code should be warning-free.

**What can go wrong:**
- Nullable reference type warnings from incorrect type annotations
- Unused variable warnings from emitter variable naming
- Obsolete API usage in generated reflection code

**Test mechanism:** Same as Level 1 — `result.Diagnostics` captures both errors and warnings.
With `TreatWarningsAsErrors`, any warning becomes a compilation failure.

### Level 3: Behavioral — Does the expression tree do what the source code does?

The generated expression tree must be functionally equivalent to the original method body.
When compiled via `Expression.Compile()` and invoked, it must produce the same result as
calling the original method directly.

**Example:** Given:
```csharp
[Expressive]
public int Add(int a, int b) => a + b;
```

The generator produces code that builds an `Expression<Func<MyClass, int, int, int>>`.
Compiling and invoking that expression with inputs `(instance, 1, 1)` must return `2`.

**What can go wrong:**
- Operands emitted in wrong order
- Missing implicit conversions (`IConversionOperation` skipped)
- Incorrect operator mapping (`BinaryOperatorKind.Remainder` → wrong `ExpressionType`)
- Null-handling logic incorrect (wrong branch in ternary)
- Type mismatches that compile but produce wrong runtime behavior

**Test mechanisms:**

1. **Expression visitor validation** — walks the generated expression tree and asserts it contains
   only valid node types (no unsupported constructs leaked through):
   ```csharp
   var visitor = new StrictExpressionVisitor();
   visitor.Visit(expression); // throws on invalid nodes
   ```

2. **EF Core translation** — executes the expression against an in-memory SQLite database via
   EF Core, proving it can be translated to SQL and produce correct query results:
   ```csharp
   await AssertWhereTranslates(predicate);   // runs query, verifies no exceptions
   await AssertSelectTranslates(selector);
   ```

3. **Direct compilation and invocation** — compiles the expression tree to a delegate and
   invokes it with known inputs, asserting the output matches expected values:
   ```csharp
   var compiled = expression.Compile();
   var result = compiled(instance, arg1, arg2);
   Assert.AreEqual(expected, result);
   ```

## Test Infrastructure

### Snapshot Tests (`ExpressiveSharp.Generator.Tests`)

Verify the **generated C# source code** against checked-in `.verified.txt` snapshots.
Covers Level 1 and Level 2.

```
tests/ExpressiveSharp.Generator.Tests/
├── Infrastructure/
│   └── GeneratorTestBase.cs          # Roslyn compilation helpers
├── ExpressiveGenerator/              # Snapshot tests per feature
│   ├── MethodTests.cs
│   ├── PropertyTests.cs
│   ├── EnumTests.cs
│   ├── NullableTests.cs
│   ├── SwitchPatternTests.cs
│   ├── TupleTests.cs
│   └── ...
└── PolyfillInterceptorGenerator/     # Interceptor snapshot tests
    ├── WhereTests.cs
    ├── SelectTests.cs
    └── ...
```

**Pattern:**
```csharp
[TestMethod]
public Task SimpleExpressiveMethod()
{
    var compilation = CreateCompilation("""
        class C {
            [Expressive]
            public int Add(int a, int b) => a + b;
        }
        """);
    var result = RunExpressiveGenerator(compilation);

    Assert.AreEqual(0, result.Diagnostics.Length);   // Level 1 + 2
    Assert.AreEqual(1, result.GeneratedTrees.Length);

    return Verifier.Verify(result.GeneratedTrees[0].ToString());
}
```

**Running:** `dotnet msbuild -t:Test tests/ExpressiveSharp.Generator.Tests/`

**Accepting new snapshots:** `VERIFY_AUTO_APPROVE=true dotnet msbuild -t:Test tests/ExpressiveSharp.Generator.Tests/`

### Functional Tests (`ExpressiveSharp.FunctionalTests`)

Verify **runtime behavior** of generated expression trees. Covers Level 3.

```
tests/ExpressiveSharp.FunctionalTests/
├── Infrastructure/
│   ├── IExpressionConsumer.cs              # Consumer interface
│   ├── ExpressionVisitorConsumer.cs        # Visitor-based validation
│   ├── EFCoreSqliteConsumer.cs             # EF Core + SQLite validation
│   └── StrictExpressionVisitor.cs          # Rejects invalid expression nodes
├── Models/
│   ├── TestOrder.cs                        # Test entities
│   └── TestCustomer.cs
├── PatternTests/                           # Abstract test bases (one per feature)
│   ├── TranslationTestBase.cs
│   ├── NullConditionalTranslationTests.cs
│   ├── SwitchExpressionTranslationTests.cs
│   ├── EnumExpansionTranslationTests.cs
│   └── ...
└── Consumers/                              # Concrete test classes (one per consumer)
    ├── Visitor/
    │   ├── VisitorNullConditionalTests.cs
    │   └── ...
    └── EFCore/
        ├── EFCoreNullConditionalTests.cs
        └── ...
```

**Pattern:** Each feature has an abstract test base defining scenarios. Concrete test classes
inherit from the base and plug in a specific consumer. This means every scenario runs against
both the expression visitor and EF Core:

```csharp
// Abstract base — defines the test scenario
public abstract class NullConditionalTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task NullConditional_MemberAccess_Translates()
    {
        Expression<Func<TestOrder, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<TestOrder, string?>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, string?>(expanded);
    }
}

// Concrete — plugs in the visitor consumer
public class VisitorNullConditionalTests : NullConditionalTranslationTests
{
    protected override IExpressionConsumer CreateConsumer()
        => new ExpressionVisitorConsumer();
}

// Concrete — plugs in the EF Core consumer
public class EFCoreNullConditionalTests : NullConditionalTranslationTests
{
    protected override IExpressionConsumer CreateConsumer()
        => new EFCoreSqliteConsumer();
}
```

**Running:** `dotnet msbuild -t:Test tests/ExpressiveSharp.FunctionalTests/`

## Adding Tests for a New IOperation Mapping

When implementing a new `EmitOperation` case (e.g., `ITupleOperation`):

1. **Snapshot test** — add a test in `ExpressiveSharp.Generator.Tests` that feeds source code
   using the feature through the generator and verifies the output compiles and matches
   a snapshot. Assert zero diagnostics.

2. **Functional test** — add an abstract test base in `PatternTests/` with scenarios that
   exercise the mapping end-to-end. Create concrete classes in `Consumers/Visitor/` and,
   if the expression nodes are EF Core-translatable, in `Consumers/EFCore/`. If the mapping
   produces nodes outside EF Core's current support (e.g., `LoopExpression`, `BlockExpression`),
   skip the EF Core consumer — it will be added when the compatibility layer lands.

3. **Direct invocation test** — for pure computation mappings (arithmetic, type conversions,
   pattern matching), add a test that compiles the expression tree and invokes it with known
   inputs, asserting the result. This catches semantic bugs that structural tests miss.
   This consumer works for all expression types since it runs in-process via `Expression.Compile()`.

## Consumers

| Consumer | What It Validates | Scope | How |
|---|---|---|---|
| `ExpressionVisitorConsumer` | Expression tree structure is valid | All expression types | Walks tree with `StrictExpressionVisitor`; throws on unsupported node types |
| Direct `Compile()` + invoke | Expression produces correct values | All expression types | Compiles to delegate, invokes with test inputs, asserts output |
| `EFCoreSqliteConsumer` | Expression translates to SQL and executes | EF Core-compatible subset | Runs the expression as a LINQ query against an in-memory SQLite database via EF Core |

The visitor and direct-invoke consumers accept the full `System.Linq.Expressions` API.
The EF Core consumer only handles the subset that EF Core can translate to SQL today.
When a mapping produces expression nodes outside that subset (e.g., `LoopExpression`,
`BlockExpression`), tests should target the first two consumers. EF Core coverage for
those nodes will follow once the compatibility/rewrite layer is in place.

## Running All Tests

```bash
# All generator tests (snapshot + compilation)
dotnet msbuild -t:Test tests/ExpressiveSharp.Generator.Tests/

# All functional tests (visitor + EF Core)
dotnet msbuild -t:Test tests/ExpressiveSharp.FunctionalTests/

# Single test by name
dotnet msbuild -t:Test tests/ExpressiveSharp.Generator.Tests/ \
  -p:TestingPlatformCommandLineArguments="--filter FullyQualifiedName~SimpleExpressiveMethod"
```
