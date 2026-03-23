# IOperation to Expression Factory Mapping

Reference table mapping Roslyn `IOperation` types to `System.Linq.Expressions.Expression` factory methods,
as implemented in `ExpressionTreeEmitter`.

**Roslyn version**: Microsoft.CodeAnalysis.CSharp 5.0.0
**Target**: `System.Linq.Expressions` (netstandard2.0+)

## Overview

`ExpressionTreeEmitter` walks the Roslyn `IOperation` tree for a member body and emits imperative C# code
that builds the equivalent `Expression<TDelegate>` using factory methods. It receives the **original syntax**
and **original semantic model** — no syntax preprocessing is required.

## Legend

| Status | Meaning |
|--------|---------|
| Implemented | Handled in `ExpressionTreeEmitter.EmitOperation()` dispatch |
| Not yet implemented | Has a clear path to implementation; currently falls through to `EmitUnsupported()` |
| Unsupported | Falls through to `EmitUnsupported()` — emits `Expression.Default(typeof(T))` with a comment |
| N/A | No `Expression.*` equivalent exists; not applicable to expression trees |

---

## Core Expression Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `ILiteralOperation` | `Expression.Constant(value, typeof(T))` | Implemented | `ConstantValue` provides typed value. When `ConstantValue.HasValue` is false (e.g. `null` literal), emits `Expression.Constant(null, typeof(T))`. Special formatting for `float.NaN`, `double.PositiveInfinity`, char/string escaping, and numeric type suffixes (`f`, `d`, `m`, `L`, `UL`, `U`, casts for `byte`/`sbyte`/`short`/`ushort`). |
| `IParameterReferenceOperation` | `Expression.Parameter(typeof(T), "name")` | Implemented | Looks up a pre-created `ParameterExpression` from `_symbolToVar` cache (populated during `Emit()`). Falls back to creating a new one if not found. |
| `IInstanceReferenceOperation` | `ParameterExpression` for `@this` | Implemented | `this`/`base` → returns the `_thisVarName` parameter. Falls back to creating `Expression.Parameter(typeof(object), "@this")` if not pre-registered. |
| `ILocalReferenceOperation` | `Expression.Variable(typeof(T), "name")` | Not yet implemented | Only valid inside `Expression.Block`; needed for block body support at the IOperation level. |
| `IParenthesizedOperation` | *(transparent)* | Implemented | Unwrapped: recursively emits the inner `Operand` with no wrapping node. |

## Member Access

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IPropertyReferenceOperation` | `Expression.Property(instance, PropertyInfo)` | Implemented | Reflection lookup cached via `ReflectionFieldCache.EnsurePropertyInfo()` as a `private static readonly PropertyInfo` field using `typeof(T).GetProperty("Name")`. When `Instance` is null (static property), emits `Expression.Property(null, fieldName)`. |
| `IFieldReferenceOperation` | `Expression.Field(instance, FieldInfo)` | Implemented | Reflection lookup cached via `ReflectionFieldCache.EnsureFieldInfo()` with appropriate `BindingFlags` for public/private and static/instance. When `Instance` is null (static field), emits `Expression.Field(null, fieldName)`. |
| `IEventReferenceOperation` | — | N/A | Events cannot be represented in expression trees. |
| `IArrayElementReferenceOperation` | `Expression.ArrayIndex(array, index)` / `Expression.ArrayAccess(array, indices)` | Implemented | Single index → `ArrayIndex`; multiple indices (multidimensional arrays) → `ArrayAccess`. |
| `IMethodReferenceOperation` | — | Unsupported | Method groups (without invocation) are not handled. Typically wrapped in `IDelegateCreationOperation` which delegates to its target. |
| `IImplicitIndexerReferenceOperation` | — | Unsupported | `System.Index`/`System.Range` implicit indexer access (`x[^1]`, `x[1..3]`). Would need to be lowered to explicit method/indexer calls. |

## Invocation & Creation

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IInvocationOperation` | `Expression.Call(instance, MethodInfo, args)` / `Expression.Call(MethodInfo, args)` | Implemented | Checks `method.IsStatic \|\| invocation.Instance is null` to choose the static or instance overload. Extension methods in IOperation already have `IsStatic == true` with the receiver as the first argument — no special handling needed. Reflection lookup cached via `ReflectionFieldCache.EnsureMethodInfo()`. Generic methods: the cached `MethodInfo` is resolved via a LINQ query + `MakeGenericMethod()`. |
| `IObjectCreationOperation` | `Expression.New(ConstructorInfo, args)` | Implemented | Reflection lookup cached via `ReflectionFieldCache.EnsureConstructorInfo()`. **With initializer**: emits `Expression.MemberInit(Expression.New(...), bindings)` where each `ISimpleAssignmentOperation` targeting an `IMemberReferenceOperation` becomes `Expression.Bind(PropertyInfo/FieldInfo, value)`. **Without constructor** (e.g. `new T()` for type parameters): emits `Expression.New(typeof(T))`. **Limitation**: collection initializers (`Expression.ListInit`) are not implemented — only member-binding initializers are supported. |
| `IAnonymousObjectCreationOperation` | `Expression.New(ctor, args, members)` | Not yet implemented | Anonymous types have compiler-generated constructors; requires the overload that accepts `MemberInfo[]` for property mapping. |
| `ITypeParameterObjectCreationOperation` | `Expression.New(typeof(T))` | Not yet implemented | `new T()` where `T` has `new()` constraint. Partially handled via `IObjectCreationOperation` null-constructor path, but `ITypeParameterObjectCreationOperation` as a distinct kind is not in the dispatch switch. |
| `IDelegateCreationOperation` | *(transparent)* | Implemented | Delegates to `EmitOperation(delegateCreate.Target)` — the inner operation (typically `IAnonymousFunctionOperation`) is emitted directly. |
| `IArrayCreationOperation` | `Expression.NewArrayInit(typeof(T), elements)` / `Expression.NewArrayBounds(typeof(T), lengths)` | Implemented | With initializer → `NewArrayInit`; without initializer (dimension sizes only) → `NewArrayBounds`. Element type extracted from `IArrayTypeSymbol.ElementType`. |

## Operators

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IBinaryOperation` | `Expression.MakeBinary(ExpressionType, left, right)` | Implemented | Maps `BinaryOperatorKind` → `ExpressionType` (see mapping below). When `OperatorMethod` is non-null (user-defined operators), passes it as the method parameter: `MakeBinary(type, left, right, false, methodInfo)`. **The hardcoded `false` is the `liftToNull` parameter** — lifted nullable semantics are *not* explicitly handled (see Limitations). |
| `IUnaryOperation` | `Expression.MakeUnary(ExpressionType, operand, type)` | Implemented | Maps `UnaryOperatorKind` → `ExpressionType` (see mapping below). When `OperatorMethod` is non-null, passes it as the method parameter. |
| `IIncrementOrDecrementOperation` | — | N/A | `++`/`--` are side-effecting and not representable in expression trees. |
| `ICompoundAssignmentOperation` | — | N/A | `+=`, `-=`, etc. are not representable in expression trees. |

### Binary Operator Kind Mapping

Source: [`ExpressionTreeEmitter.MapBinaryOperatorKind()`](src/ExpressiveSharp.Generator/Emitter/ExpressionTreeEmitter.cs#L266-L290)

| `BinaryOperatorKind` | `ExpressionType` | Notes |
|---|---|---|
| `Add` | `Add` | |
| `Subtract` | `Subtract` | |
| `Multiply` | `Multiply` | |
| `Divide` | `Divide` | |
| `Remainder` | `Modulo` | |
| `LeftShift` | `LeftShift` | |
| `RightShift` | `RightShift` | |
| `And` (bitwise) | `And` | |
| `Or` (bitwise) | `Or` | |
| `ExclusiveOr` | `ExclusiveOr` | |
| `ConditionalAnd` (`&&`) | `AndAlso` | |
| `ConditionalOr` (`\|\|`) | `OrElse` | |
| `Equals` | `Equal` | |
| `NotEquals` | `NotEqual` | |
| `LessThan` | `LessThan` | |
| `LessThanOrEqual` | `LessThanOrEqual` | |
| `GreaterThan` | `GreaterThan` | |
| `GreaterThanOrEqual` | `GreaterThanOrEqual` | |
| *(any other value)* | `Add` | **Silent fallback** — unrecognized operator kinds (including `UnsignedRightShift` from C# 11) default to `Add`, which will produce incorrect expression trees. |

### Unary Operator Kind Mapping

Source: [`ExpressionTreeEmitter.MapUnaryOperatorKind()`](src/ExpressiveSharp.Generator/Emitter/ExpressionTreeEmitter.cs#L314-L324)

| `UnaryOperatorKind` | `ExpressionType` | Notes |
|---|---|---|
| `BitwiseNegation` (`~`) | `OnesComplement` | |
| `Not` (`!`) | `Not` | |
| `Plus` (`+`) | `UnaryPlus` | |
| `Minus` (`-`) | `Negate` | |
| *(any other value)* | `Not` | **Silent fallback** — unrecognized operator kinds default to `Not`. |

## Type Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IConversionOperation` | `Expression.Convert(expr, typeof(T))` | Implemented | **Identity conversions are skipped** — returns the operand directly when `Conversion.IsIdentity` is true. User-defined conversion operators: when `Conversion.MethodSymbol` is non-null, passes it as a method parameter: `Expression.Convert(operand, type, methodInfo)`. **Corner case**: `ConvertChecked` is never emitted — checked conversion contexts are not preserved (see Limitations). |
| `ITypeOfOperation` | `Expression.Constant(typeof(T), typeof(Type))` | Implemented | Wraps a `typeof()` expression as a runtime `Type` constant. |
| `IDefaultValueOperation` | `Expression.Default(typeof(T))` | Implemented | `default` / `default(T)`. |
| `IIsTypeOperation` | `Expression.TypeIs(expr, typeof(T))` | Implemented | Simple `is Type` check (no pattern variable, no pattern matching). For `is` with patterns, see `IIsPatternOperation` below. |
| `ISizeOfOperation` | — | Unsupported | `sizeof(T)`. Would need compile-time evaluation to become a constant. |
| `INameOfOperation` | — | Unsupported | `nameof(x)`. The C# compiler typically lowers this to a string constant before it reaches the IOperation tree, so this kind rarely appears. |

## Conditional & Null Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IConditionalOperation` | `Expression.Condition(test, ifTrue, ifFalse, typeof(T))` | Implemented | Ternary `? :`. When `WhenFalse` is null (statement-form if without else), emits `Expression.Default(typeof(T))` as the false branch. The explicit `typeof(T)` parameter ensures the result type is set correctly when branches have different types. |
| `IConditionalAccessOperation` | `Expression.Condition(notNullCheck, whenNotNull, default)` | Not yet implemented | `?.` operator. Emit as: `Expression.Condition(Expression.NotEqual(receiver, null), whenNotNull, Expression.Default(type))`. **Corner cases**: `Nullable<T>` targets need `.Value` access on the underlying type; `?[]` (element binding) produces indexer access instead of member access; chained `?.` requires tracking the receiver through `IConditionalAccessInstanceOperation` placeholders. The `NullConditionalMode` setting (Rewrite vs Ignore) should control whether null checks are emitted or stripped. |
| `IConditionalAccessInstanceOperation` | *(resolved to receiver)* | Not yet implemented | Placeholder inside `IConditionalAccessOperation.WhenNotNull`; replace with the actual receiver expression during emission. |
| `ICoalesceOperation` | `Expression.Coalesce(left, right)` | Implemented | `??` operator. Direct `Expression.Coalesce` support exists in `System.Linq.Expressions`. |
| `ICoalesceAssignmentOperation` | — | N/A | `??=` involves assignment; not representable in expression trees. |

## Nested Lambda & Delegate Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IAnonymousFunctionOperation` | `Expression.Lambda<TDelegate>(body, params)` | Implemented | Nested lambdas inside expressions. Parameters are scoped per-lambda with unique variable names (`p_name_N`). Delegate type is inferred: `Action` / `Action<T...>` for void return, `Func<T..., TResult>` for non-void. Body is recursively emitted. |
| `IDelegateCreationOperation` | *(transparent)* | Implemented | Simply emits its `Target` operation (typically an `IAnonymousFunctionOperation`). No wrapping node is created. |

---

## Not Yet Implemented — With Implementation Specs

The following IOperation kinds are not in the emitter dispatch switch but have a clear path to implementation.

### `IConditionalAccessOperation` — Null-Conditional (`?.`, `?[]`)

**Target emission**: `Expression.Condition(Expression.NotEqual(receiver, null), whenNotNull, Expression.Default(type))`

**Implementation approach**:
- Extract receiver from `IConditionalAccessOperation.Operation`
- Emit the `WhenNotNull` branch, replacing `IConditionalAccessInstanceOperation` with the receiver
- Build ternary: `receiver != null ? whenNotNull : (T)default`
- **`Nullable<T>` targets**: inject `.Value` member access before accessing members on the underlying type. Detect via `IsValueType && OriginalDefinition.SpecialType == System_Nullable_T`.
- **Element binding** (`?[]`): produces `Expression.Call` or `Expression.Property` (indexer) instead of `Expression.Property` (member)
- **`NullConditionalMode`**: `Rewrite` emits full null-check ternary; `Ignore` strips the null check and emits only the `WhenNotNull` branch; disabled (`NullConditional` in `Disable`) reports `EXP0002` diagnostic

### `ISwitchExpressionOperation` — Switch Expressions

**Target emission**: Nested `Expression.Condition` chain

**Implementation approach**:
- Walk `ISwitchExpressionArmOperation` arms in reverse
- Discard arm (`_`) becomes the innermost fallback value; if absent, use `Expression.Default(type)`
- For each non-discard arm, convert the pattern to a boolean condition (see `IIsPatternOperation` below)
- Combine pattern condition with when-clause (if present) via `Expression.AndAlso`
- Build: `Expression.Condition(condition, armValue, nextArm)`
- **Declaration patterns in arms** (`is SomeType t => t.Prop`): the declared variable must be replaced with `Expression.Convert(governing, typeof(SomeType))` throughout the arm body. This requires tracking the variable binding and substituting during emission.

### `IIsPatternOperation` — Pattern Matching

**Target emission**: Depends on inner `IPatternOperation` subtype

| Pattern | Emission |
|---|---|
| `IConstantPatternOperation` | `Expression.Equal(expr, Expression.Constant(value))` |
| `ITypePatternOperation` | `Expression.TypeIs(expr, typeof(T))` |
| `IDeclarationPatternOperation` (discard) | `Expression.TypeIs(expr, typeof(T))` |
| `IDeclarationPatternOperation` (named) | Not representable — pattern variables don't exist in expression trees. Report `EXP0007`. **Exception**: in switch arms, substitute with cast (see above). |
| `IRelationalPatternOperation` | `Expression.MakeBinary(op, expr, constant)` — map relational operator to `LessThan`/`GreaterThan`/etc. |
| `INegatedPatternOperation` | `Expression.Not(innerPatternExpr)` |
| `IBinaryPatternOperation` | `Expression.AndAlso(left, right)` / `Expression.OrElse(left, right)` |
| `IRecursivePatternOperation` | Null-check (for reference/nullable types) + type guard + property/positional conditions combined with `Expression.AndAlso`. Positional patterns resolve via `Deconstruct` method parameters → property names (case-insensitive), with fallback to `Item1`/`Item2` for `ValueTuple`. Value-type properties skip null checks. |
| `IDiscardPatternOperation` | Always matches — return `Expression.Constant(true)` |
| `IListPatternOperation` | Not yet designed |
| `ISlicePatternOperation` | Not yet designed |

### `ITupleOperation` — Tuple Literals

**Target emission**: `Expression.New(ValueTuple<...> ctor, elements)`

**Implementation approach**:
- Get the `ValueTuple` constructor from the tuple type symbol
- Emit each `ElementValues` operand
- Build: `Expression.New(ctorInfo, element1, element2, ...)`
- **8+ elements**: C# nests as `ValueTuple<T1, ..., T7, ValueTuple<T8, ...>>`. Recursively build the nested constructor for the `Rest` argument.

### Other

| IOperation | Target Expression Factory | Notes |
|---|---|---|
| `ITupleBinaryOperation` | Expanded equality/inequality | `(a, b) == (c, d)` → `Expression.AndAlso(Expression.Equal(a, c), Expression.Equal(b, d))` |
| `IInterpolatedStringOperation` | `Expression.Call(null, string.Format, ...)` | C# compiler typically lowers to `string.Format`/`string.Concat` before IOperation, so this rarely appears. |
| `IThrowOperation` | `Expression.Throw(expr, typeof(T))` | `throw` expressions (C# 7+). `Expression.Throw` exists in .NET but not all LINQ providers support it. |
| `IRangeOperation` | `Expression.New(Range ctor, start, end)` | `a..b` → `new Range(new Index(a), new Index(b))` |
| `IWithOperation` | `Expression.MemberInit(...)` with copied props | Record `with { }` expressions; requires lowering to constructor + member init with all properties. |
| `ITypeParameterObjectCreationOperation` | `Expression.New(typeof(T))` | `new T()` with `new()` constraint. Partially handled via `IObjectCreationOperation` null-constructor path. |
| `ICollectionExpressionOperation` | `Expression.NewArrayInit` or `Expression.ListInit` | C# 12 collection expressions; lower based on target type. |

---

## Not Applicable to Expression Trees

| IOperation | Reason |
|---|---|
| `IEventReferenceOperation` | Events cannot appear in expression trees |
| `ICompoundAssignmentOperation` | `+=`, `-=`, etc. are side-effecting assignments |
| `ICoalesceAssignmentOperation` | `??=` is an assignment |
| `IIncrementOrDecrementOperation` | `++`/`--` are side-effecting |
| `IAddressOfOperation` | Pointers are not supported in expression trees |
| `IDynamicInvocationOperation` | Dynamic dispatch has no expression tree equivalent |
| `IDynamicMemberReferenceOperation` | Dynamic dispatch has no expression tree equivalent |
| `IDynamicIndexerAccessOperation` | Dynamic dispatch has no expression tree equivalent |
| `IDynamicObjectCreationOperation` | Dynamic dispatch has no expression tree equivalent |
| `IFunctionPointerInvocationOperation` | Function pointers are not supported in expression trees |
| `IAwaitOperation` | `await` has no standard `Expression.*` equivalent |
| `IDiscardOperation` | `_` discard is context-dependent; no standalone expression tree form |
| `IDeconstructionAssignmentOperation` | `(a, b) = ...` is an assignment |
| `ISimpleAssignmentOperation` | Only handled within object initializers; standalone assignment is not representable |

---

## Limitations and Known Issues

### 1. No Checked Arithmetic Support

The emitter **never** emits `AddChecked`, `SubtractChecked`, `MultiplyChecked`, `ConvertChecked`, or `NegateChecked`. All arithmetic operations use their unchecked variants regardless of whether the source code is in a `checked` context. The `IBinaryOperation.IsChecked`, `IUnaryOperation.IsChecked`, and `IConversionOperation.IsChecked` properties are not consulted.

### 2. No Explicit Nullable Lifting

The `liftToNull` parameter in `Expression.MakeBinary()` is always hardcoded to `false`. The emitter does not inspect `IBinaryOperation.IsLifted` to determine whether an operator is lifted over nullable types. In practice, the .NET expression tree runtime infers lifting from the operand types, so this works for standard operators. It may produce incorrect behavior for user-defined operators on nullable types where `liftToNull` semantics differ.

### 3. Silent Fallback for Unrecognized Operator Kinds

Both `MapBinaryOperatorKind` and `MapUnaryOperatorKind` have default cases that silently map unrecognized kinds to `Add` and `Not` respectively. This means:
- **`UnsignedRightShift` (C# 11, `>>>`)** will silently emit `Expression.MakeBinary(ExpressionType.Add, ...)` which is incorrect. `System.Linq.Expressions.ExpressionType` has no `UnsignedRightShift` equivalent.
- Any future `BinaryOperatorKind` or `UnaryOperatorKind` values added by Roslyn will also silently produce incorrect expressions.

### 4. Collection Initializers Not Supported

`IObjectCreationOperation` with `IObjectOrCollectionInitializerOperation` only handles **member binding** initializers (`Expression.Bind`). Collection initializers (e.g., `new List<int> { 1, 2, 3 }` → `Expression.ListInit`) are not implemented — the collection elements would be silently ignored if the initializer contains `IInvocationOperation` (`.Add()` calls) instead of `ISimpleAssignmentOperation`.

### 5. IOperation Null Fallback

If `SemanticModel.GetOperation()` returns `null` for the body syntax, the emitter produces `Expression.Default(typeof(ReturnType))` as the lambda body — a valid but semantically empty expression tree.

---

## Notes

1. **Implicit conversions**: `IConversionOperation` appears in the IOperation tree for compiler-inserted implicit conversions (not just explicit casts). This is a major advantage over syntax-based translation since `Expression.Convert` must be emitted for these invisible conversions.

2. **Extension methods in IOperation**: `IInvocationOperation` for extension methods has `TargetMethod.IsStatic == true` with the receiver as the first argument in `Arguments`. The emitter does not need to distinguish extension from static calls.

3. **Reflection field caching**: All reflective member lookups (`PropertyInfo`, `FieldInfo`, `MethodInfo`, `ConstructorInfo`) are emitted as `private static readonly` fields in the generated class. This avoids repeated reflection at runtime and ensures referential equality for the same member across multiple expression trees.

4. **Variable naming**: Emitted expression variables use the pattern `expr_N` (sequential counter). Parameter variables use `p_sanitizedName` (top-level) or `p_sanitizedName_N` (nested lambdas). The sanitizer replaces `@`, `.`, `<`, `>` with `_`.

5. **Delegate type inference**: For nested lambdas, the emitter builds the delegate type from the method symbol: void-returning lambdas use `Action<T...>`, non-void use `Func<T..., TResult>`. All types are fully qualified (`global::System.Func<...>`).
