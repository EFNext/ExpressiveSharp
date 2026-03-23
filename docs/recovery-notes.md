# Recovery Notes: Legacy Syntax Rewriting Removal

This document tracks functionality that was removed when migrating from the legacy syntax-rewriting
path to the IOperation-based `ExpressionTreeEmitter`. Each item needs to be re-implemented at
either the emitter level or the interpreter level.

## Removed Functionality

### 1. Block-Bodied Methods and Properties

**What it did:** `BlockStatementConverter` converted `{ statement; statement; return expr; }` blocks
into a single expression by:
- Inlining local variable declarations at their use sites
- Converting `if/else` chains to nested ternary `? :` expressions
- Converting `for`/`foreach` loops to LINQ (`.Select()`, `.Where()`, `.Sum()`, `.Count()`,
  `.Any()`, `.All()`, `.Aggregate()`, etc.) via `ForLoopToLinqConverter` and `ForEachToLinqConverter`
- Folding the block right-to-left into a single expression

**Recovery path:** The emitter needs to handle `IBlockOperation`, `IReturnOperation`,
`IVariableDeclarationGroupOperation`, `IExpressionStatementOperation`, and loop operations.
This requires `Expression.Block()`, `Expression.Variable()`, `Expression.Assign()`, and
`Expression.Loop()` support. Per the testing strategy, emit the closest `Expression.*` node
rather than lowering to LINQ.

**Affected tests:** Block-bodied method and property tests in `GlobalOptionsTests`.

### 2. Constructor Body Processing

**What it did:** `ConstructorBodyConverter` extracted property assignments from constructor bodies
and produced `new T() { Prop1 = expr1, Prop2 = expr2 }` object-initializer expressions for
EF Core projections.

**Special features:**
- Followed `base()`/`this()` initializer chains recursively via `CollectDelegatedConstructorAssignments()`
- Substituted parameters through the delegation chain
- Required `Compilation` object for cross-file semantic model access
- Verified parameterless constructor existence (diagnostic `EXP0008`)
- Handled `if/else` logic inside constructors (conditional property assignment)

**Recovery path:** The emitter needs an `IConstructorBodyOperation` or `IBlockOperation` handler
that detects constructor patterns and emits `Expression.MemberInit()`. Constructor delegation
chain following may need to remain at the interpreter level since it requires cross-file analysis.

### 3. Enum Method Expansion (was §4)

**What it did:** Expanded calls to methods on enum values into ternary chains comparing each
enum member:
```csharp
myEnum.GetName()
→ (myEnum == MyEnum.A) ? ExtClass.GetName(MyEnum.A)
  : (myEnum == MyEnum.B) ? ExtClass.GetName(MyEnum.B)
  : default(string)
```

**Recovery path:** This is a domain-specific semantic transformation, not an IOperation mapping.
It requires enumerating all enum members via the Roslyn symbol API. Options:
- Implement as a post-processing step on the emitted expression tree
- Implement as a pre-processing step in the interpreter that modifies the syntax before
  passing to the emitter (would need re-compilation for correct IOperation tree)
- Add as a special case in the emitter's `IInvocationOperation` handler that detects
  enum receiver types and expands inline

**Controlled by:** `ExpressionFeature.EnumMethodExpansion` in `Disable` property.

### 5. NullConditionalMode Configuration

**What it did:** Three modes for `?.` operator handling:
- **Rewrite** (default): expand to explicit null check ternary
- **Ignore**: strip `?.`, use direct member access
- **Disabled**: report `EXP0002` diagnostic

**Recovery path:** The emitter currently always emits the Rewrite-style ternary for
`IConditionalAccessOperation`. The `NullConditionalMode` and `ExpressionFeature.NullConditional`
settings need to be passed to the emitter (or handled at the interpreter level before emission).

**Controlled by:** `NullConditionalMode` enum, `ExpressionFeature.NullConditional` in `Disable`.

### 6. Legacy SyntaxFactory Code Generation

**What it did:** Generated a class with a method that returns a lambda expression:
```csharp
static Expression<Func<T, TResult>> Expression()
{
    return (@this, arg) => <expressionBody>;
}
```

**Recovery path:** Fully replaced by `EmitExpressionTreeSource()` which emits imperative
`Expression.*` factory calls with cached reflection fields. No recovery needed — this is
the intentional replacement.

## Removed Files

| File | Was Used For |
|---|---|
| `SyntaxRewriters/BlockStatementConverter.cs` | Block body → expression conversion |
| `SyntaxRewriters/ConstructorBodyConverter.cs` | Constructor → object initializer |
| `SyntaxRewriters/ForEachToLinqConverter.cs` | foreach → LINQ |
| `SyntaxRewriters/ForLoopToLinqConverter.cs` | for → LINQ |
| `SyntaxRewriters/PatternConverter.cs` | Pattern syntax → boolean expressions |
| `SyntaxRewriters/DictionaryIdentifierReplacer.cs` | Local variable identifier replacement |
| `SyntaxRewriters/VariableReplacementRewriter.cs` | Single variable renaming |
| `SyntaxRewriters/ExpressionSyntaxRewriter.cs` | Core syntax rewriter (this/base, implicit members) |
| `SyntaxRewriters/ExpressionSyntaxRewriter.EnumMethodExpansion.cs` | Enum → ternary chains |
| `SyntaxRewriters/ExpressionSyntaxRewriter.InvocationAndMemberRewrite.cs` | Extension method qualification |
| `SyntaxRewriters/ExpressionSyntaxRewriter.NullConditionalRewrite.cs` | `?.` → ternary |
| `SyntaxRewriters/ExpressionSyntaxRewriter.SwitchExpressionRewrite.cs` | Switch → ternary chains |
| `SyntaxRewriters/ExpressionSyntaxRewriter.TupleRewrite.cs` | Tuples → ValueTuple ctor |
| `SyntaxRewriters/ExpressionSyntaxRewriter.TypeRewrite.cs` | Type fully-qualification |

### 7. PolyfillInterceptorGenerator Lambda Rewriting

**What it did:** `PolyfillInterceptorGenerator.RewriteBody()` used `ExpressionSyntaxRewriter` to
rewrite lambda bodies intercepted via `ExpressionPolyfill.Create<T>()` and `IRewritableQueryable<T>`
LINQ methods. It applied null-conditional expansion, enum expansion, extension method qualification,
type qualification, and block body conversion to the intercepted lambdas.

**Recovery path:** The polyfill generator produces interceptor methods that return lambda expressions
(not imperative `Expression.*` factory code). It needs either:
- Its own emitter integration (generate `Expression.*` factory code in the interceptor)
- Or a lightweight rewriting pass for the specific features it needs (null-conditional, type qualification)

The polyfill generator currently passes through the original lambda body without rewriting.

## Recovery Status

### Completed
1. **NullConditionalMode** — settings plumbed to emitter (Rewrite/Ignore/Disabled)
2. **UseMemberBody** — feature removed (no longer needed)
3. **Block-bodied methods/properties** — emitter handles IBlockOperation, IReturnOperation, ILocalReferenceOperation
4. **Constructor bodies** — emitter handles MemberInit with property bindings from constructor body
5. **Enum method expansion** — emitter expands enum method calls to ternary chains

6. **Constructor edge cases** — if/else chains, early returns, local vars, sequential ifs, else-if chains
7. **Loop diagnostics** — EXP0013 (loop feature disable) and EXP0014 (unsupported loop patterns)
8. **PolyfillInterceptorGenerator** — migrated to emit `Expression.*` factory code via `ExpressionTreeEmitter`
9. **ExpressionPolyfill.Create with transformers** — overload added, interceptor generates transformer application

10. **FlattenBlockExpressions transformer** — inlines block-local variables and removes `Expression.Block` nodes for LINQ provider compatibility
11. **NullConditionalMode → transformer** — legacy attribute property removed. Behavior now controlled by runtime `IExpressionTreeTransformer` pipeline (see `docs/expression-transformers.md`)

### Remaining
All generator tests (193/202) and functional tests (28/28) passing. No remaining recovery items.
