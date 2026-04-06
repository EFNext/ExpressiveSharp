# IOperation to Expression Mapping

Reference table mapping Roslyn `IOperation` types to `System.Linq.Expressions.Expression` factory methods, as implemented in `ExpressionTreeEmitter`.

**Roslyn version**: Microsoft.CodeAnalysis.CSharp 5.0.0
**Target**: `System.Linq.Expressions` (netstandard2.0+)

## Overview

`ExpressionTreeEmitter` walks the Roslyn `IOperation` tree for a member body and emits imperative C# code that builds the equivalent `Expression<TDelegate>` using factory methods. It receives the **original syntax** and **original semantic model** -- no syntax preprocessing is required. Transparent syntax wrappers (`checked()`, `unchecked()`, parenthesized, null-forgiving `!`) are unwrapped before `GetOperation` is called.

## Legend

| Status | Meaning |
|---|---|
| Implemented | Handled in `ExpressionTreeEmitter.EmitOperation()` dispatch |
| Not yet implemented | Has a clear path to implementation; currently falls through to `EmitUnsupported()` |
| N/A | No `Expression.*` equivalent exists; not applicable to expression trees |

Unrecognized operations fall through to `EmitUnsupported()` which emits `Expression.Default(typeof(T))` and reports diagnostic `EXP0008`.

---

## Core Expression Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `ILiteralOperation` | `Expression.Constant(value, typeof(T))` | Implemented | Special formatting for `float.NaN`, `double.PositiveInfinity`, char/string escaping, numeric suffixes. |
| `IParameterReferenceOperation` | `Expression.Parameter(typeof(T), "name")` | Implemented | Looks up pre-created `ParameterExpression` from `_symbolToVar` cache. |
| `IInstanceReferenceOperation` | `ParameterExpression` for `@this` | Implemented | Returns the `_thisVarName` parameter. |
| `ILocalReferenceOperation` | `Expression.Variable(typeof(T), "name")` | Implemented | Looks up from `_localToVar` cache, populated by `EmitBlock` variable declarations. |
| `IParenthesizedOperation` | *(transparent)* | Implemented | Unwrapped: recursively emits the inner `Operand`. |

## Member Access

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IPropertyReferenceOperation` | `Expression.Property(instance, PropertyInfo)` | Implemented | Cached via `ReflectionFieldCache.EnsurePropertyInfo()`. |
| `IFieldReferenceOperation` | `Expression.Field(instance, FieldInfo)` | Implemented | Cached via `ReflectionFieldCache.EnsureFieldInfo()`. |
| `IArrayElementReferenceOperation` | `Expression.ArrayIndex(array, index)` / `Expression.ArrayAccess(array, indices)` | Implemented | Single index uses `ArrayIndex`; multiple uses `ArrayAccess`. |
| `IEventReferenceOperation` | -- | N/A | Events cannot be represented in expression trees. |

## Invocation and Creation

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IInvocationOperation` | `Expression.Call(instance, MethodInfo, args)` | Implemented | Static/instance dispatch. Generic methods resolved via `MakeGenericMethod()`. Enum method expansion to ternary chains. |
| `IObjectCreationOperation` | `Expression.New(ConstructorInfo, args)` | Implemented | With member initializer: `Expression.MemberInit`. With collection initializer: `Expression.ListInit` via `Expression.ElementInit`. |
| `IAnonymousObjectCreationOperation` | `Expression.New(ctor, args, members)` | Implemented (interceptor path) | Uses inline runtime reflection via generic type parameters (e.g., `typeof(TResult).GetConstructors()[0]`). Nested anonymous types fall back to EXP0008. |
| `IDelegateCreationOperation` | *(transparent)* | Implemented | Emits its `Target` operation directly. |
| `IArrayCreationOperation` | `Expression.NewArrayInit` / `Expression.NewArrayBounds` | Implemented | With initializer: `NewArrayInit`; without: `NewArrayBounds`. |

## Operators

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IBinaryOperation` | `Expression.MakeBinary(ExpressionType, left, right)` | Implemented | Maps `BinaryOperatorKind` to `ExpressionType`. Supports checked variants (`AddChecked`, `SubtractChecked`, `MultiplyChecked`) when `IsChecked` is true. Unrecognized operators report `EXP0009` diagnostic. |
| `IUnaryOperation` | `Expression.MakeUnary(ExpressionType, operand, type)` | Implemented | Supports `NegateChecked` when `IsChecked` is true. Unrecognized operators report `EXP0009`. |
| `IIncrementOrDecrementOperation` | `Expression.Assign(target, MakeBinary(Add/Sub, target, 1))` | Implemented | `++`/`--` emitted as assign + binary. Supports checked variants. |
| `ICompoundAssignmentOperation` | `Expression.Assign(target, MakeBinary(op, target, value))` | Implemented | `+=`, `-=`, etc. emitted as assign + binary. Supports checked variants. |

### Binary Operator Kind Mapping

| `BinaryOperatorKind` | `ExpressionType` | Checked variant |
|---|---|---|
| `Add` | `Add` | `AddChecked` |
| `Subtract` | `Subtract` | `SubtractChecked` |
| `Multiply` | `Multiply` | `MultiplyChecked` |
| `Divide` | `Divide` | -- |
| `Remainder` | `Modulo` | -- |
| `LeftShift` | `LeftShift` | -- |
| `RightShift` | `RightShift` | -- |
| `And` (bitwise) | `And` | -- |
| `Or` (bitwise) | `Or` | -- |
| `ExclusiveOr` | `ExclusiveOr` | -- |
| `ConditionalAnd` (`&&`) | `AndAlso` | -- |
| `ConditionalOr` (`\|\|`) | `OrElse` | -- |
| `Equals` | `Equal` | -- |
| `NotEquals` | `NotEqual` | -- |
| `LessThan` | `LessThan` | -- |
| `LessThanOrEqual` | `LessThanOrEqual` | -- |
| `GreaterThan` | `GreaterThan` | -- |
| `GreaterThanOrEqual` | `GreaterThanOrEqual` | -- |
| *(unrecognized)* | -- | Reports `EXP0009` diagnostic |

### Unary Operator Kind Mapping

| `UnaryOperatorKind` | `ExpressionType` | Checked variant |
|---|---|---|
| `BitwiseNegation` (`~`) | `OnesComplement` | -- |
| `Not` (`!`) | `Not` | -- |
| `Plus` (`+`) | `UnaryPlus` | -- |
| `Minus` (`-`) | `Negate` | `NegateChecked` |
| *(unrecognized)* | -- | Reports `EXP0009` diagnostic |

## Type Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IConversionOperation` | `Expression.Convert(expr, typeof(T))` | Implemented | Identity conversions skipped. `ConvertChecked` emitted when `IsChecked` is true. User-defined operators pass `MethodInfo`. |
| `ITypeOfOperation` | `Expression.Constant(typeof(T), typeof(Type))` | Implemented | |
| `IDefaultValueOperation` | `Expression.Default(typeof(T))` | Implemented | |
| `IIsTypeOperation` | `Expression.TypeIs(expr, typeof(T))` | Implemented | Simple `is Type` check. |

## Conditional and Null Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IConditionalOperation` | `Expression.Condition` / `Expression.IfThen` / `Expression.IfThenElse` | Implemented | Ternary `? :` uses `Condition`. Statement-form if/else uses `IfThen`/`IfThenElse` (void-typed). |
| `IConditionalAccessOperation` | `Expression.Condition(notNullCheck, whenNotNull, default)` | Implemented | `?.` operator with receiver stack for chained access. Handles `Nullable<T>` `.Value` injection. |
| `IConditionalAccessInstanceOperation` | *(resolved to receiver)* | Implemented | Pops from receiver stack during conditional access processing. |
| `ICoalesceOperation` | `Expression.Coalesce(left, right)` | Implemented | `??` operator. |
| `IThrowOperation` | `Expression.Throw(expr, typeof(T))` | Implemented | `throw` expressions. Uses typed overload in value positions (`?? throw`, ternary, switch arm), void overload for statements. `ReplaceThrowWithDefault` transformer replaces with `Expression.Default` for EF Core compatibility. |

## Pattern Matching

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IIsPatternOperation` | Dispatches to pattern-specific emission | Implemented | Full pattern matching dispatch. |
| `IConstantPatternOperation` | `Expression.Equal(expr, Expression.Constant(value))` | Implemented | |
| `ITypePatternOperation` | `Expression.TypeIs(expr, typeof(T))` | Implemented | |
| `IDeclarationPatternOperation` | `Expression.TypeIs` / cast substitution in switch arms | Implemented | Named declarations in switch arms substitute with `Expression.Convert`. |
| `IRelationalPatternOperation` | `Expression.MakeBinary(op, expr, constant)` | Implemented | Unrecognized operators report `EXP0009`. |
| `INegatedPatternOperation` | `Expression.Not(innerPatternExpr)` | Implemented | |
| `IBinaryPatternOperation` | `Expression.AndAlso` / `Expression.OrElse` | Implemented | |
| `IRecursivePatternOperation` | Null-check + type guard + property/positional conditions | Implemented | Positional patterns resolve via `Deconstruct` parameters or `ItemN` fields. |
| `IDiscardPatternOperation` | `Expression.Constant(true)` | Implemented | |
| `IListPatternOperation` | Count/Length check + indexed element pattern checks | Implemented | Fixed-length and slice patterns. Requires `Count`/`Length` property and indexer on operand type. |
| `ISlicePatternOperation` | *(handled within list pattern)* | Implemented | Slice (`..`) adjusts index calculations for elements after the slice. |

## Switch Expressions

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `ISwitchExpressionOperation` | Nested `Expression.Condition` chain | Implemented | Arms processed in reverse. Discard arm becomes innermost fallback. Declaration patterns in arms substitute with cast. |

## Tuple Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `ITupleOperation` | `Expression.New(ValueTuple<...> ctor, elements)` | Implemented | Handles 1-7 elements directly. 8+ elements use nested `ValueTuple` for `Rest` argument. |
| `ITupleBinaryOperation` | Element-wise `Equal` + `AndAlso` / `Not` | Implemented | `(a, b) == (c, d)` becomes `AndAlso(Equal(a.Item1, c.Item1), Equal(a.Item2, c.Item2))`. `!=` wraps in `Not`. |

## Index and Range

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IUnaryOperation` (Hat) | `Expression.New(Index ctor, value, true)` | Implemented | `^n` becomes `new Index(n, fromEnd: true)`. |
| `IRangeOperation` | `Expression.New(Range ctor, start, end)` | Implemented | `a..b` becomes `new Range(start, end)`. Implicit start/end produce `new Index(0, false)` / `new Index(0, true)`. Operand `int` to `Index` conversions handled by `EmitConversion`. |

## Collection Expressions

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `ICollectionExpressionOperation` | `Expression.NewArrayInit` / `Expression.ListInit` | Implemented | C# 12 collection expressions. Without spread: arrays use `NewArrayInit`, collections use `ListInit` with `Add`. With spread: segments are concatenated via `Enumerable.Concat<T>` and materialized via `Enumerable.ToArray<T>` / `ToList<T>`. |
| `ISpreadOperation` | *(handled within collection expression)* | Implemented | Spread operand emitted directly as an `IEnumerable<T>` segment. |

## String Interpolation

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IInterpolatedStringOperation` | `Expression.Call(string.Concat, ...)` | Implemented | Parts flattened to string expressions, non-string parts wrapped in `.ToString()`. Format specifiers use `ToString(format)` (**not translatable to SQL** -- client-evaluated in projections, throws in `Where`/`OrderBy`). Uses optimal `string.Concat` overload based on part count (2-arg, 3-arg, 4-arg, or `string[]` for 5+). `FlattenConcatArrayCalls` transformer rewrites the array form for EF Core compatibility. Alignment specifiers emit a diagnostic and are ignored. |

## Block Body Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IBlockOperation` | `Expression.Block(variables, statements)` | Implemented | Local variable declarations, assignments, returns, expression statements. Requires `AllowBlockBody = true`. |
| `IReturnOperation` | *(result expression)* | Implemented | Extracts the returned value as the block's result. |
| `IExpressionStatementOperation` | *(transparent)* | Implemented | Unwraps to inner operation. |
| `ISimpleAssignmentOperation` | `Expression.Assign(target, value)` | Implemented | Standalone assignment in block bodies. |

## Loop Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IForEachLoopOperation` | `Expression.Loop` with enumerator pattern | Implemented | GetEnumerator/MoveNext/Current pattern. `ConvertLoopsToLinq` transformer rewrites to LINQ for providers that cannot handle `LoopExpression`. |
| `IForLoopOperation` | `Expression.Loop` with init/condition/increment | Implemented | |
| `IWhileLoopOperation` | `Expression.Loop` with condition | Implemented | Supports both while (condition-at-top) and do-while (condition-at-bottom). |

## Nested Lambda and Delegate Operations

| IOperation | Expression Factory | Status | Notes |
|---|---|---|---|
| `IAnonymousFunctionOperation` | `Expression.Lambda<TDelegate>(body, params)` | Implemented | Nested lambdas with scoped parameters. |
| `IDelegateCreationOperation` | *(transparent)* | Implemented | Emits its `Target` operation directly. |

---

---

## Not Applicable to Expression Trees

These operations are rejected early by block body validation (EXP0005/EXP0006) when inside `[Expressive(AllowBlockBody = true)]` members.

| IOperation | Reason |
|---|---|
| `IEventReferenceOperation` | Events cannot appear in expression trees |
| `ICoalesceAssignmentOperation` | `??=` is an assignment |
| `IAddressOfOperation` | Pointers are not supported in expression trees |
| `IDynamicInvocationOperation` | Dynamic dispatch has no expression tree equivalent |
| `IDynamicMemberReferenceOperation` | Dynamic dispatch has no expression tree equivalent |
| `IDynamicIndexerAccessOperation` | Dynamic dispatch has no expression tree equivalent |
| `IDynamicObjectCreationOperation` | Dynamic dispatch has no expression tree equivalent |
| `IFunctionPointerInvocationOperation` | Function pointers are not supported in expression trees |
| `IAwaitOperation` | `await` has no `Expression.*` equivalent |
| `IDeconstructionAssignmentOperation` | `(a, b) = ...` is a destructuring assignment |
| `ITryOperation` | try/catch/finally has no expression tree equivalent |
| `IUsingOperation` | using/dispose pattern not representable |
| `ILockOperation` | lock has no expression tree equivalent |

---

## Limitations and Known Issues

### 1. No Explicit Nullable Lifting

The `liftToNull` parameter in `Expression.MakeBinary()` is always hardcoded to `false`. The emitter does not inspect `IBinaryOperation.IsLifted`. In practice, the .NET expression tree runtime infers lifting from operand types, so this works for standard operators. It may produce incorrect behavior for user-defined operators on nullable types where `liftToNull` semantics differ.

### 2. IOperation Null Fallback

If `SemanticModel.GetOperation()` returns `null` for the body syntax (after unwrapping transparent syntax), the emitter reports `EXP0008` and produces `Expression.Default(typeof(ReturnType))` as the lambda body.

### 3. String Interpolation vs. `+` Operator Asymmetry

String interpolation (`$"text {value}"`) emits `value.ToString()` and then uses the appropriate `string.Concat` overload based on the number of interpolation parts (`string.Concat(string, string)`, `string.Concat(string, string, string)`, `string.Concat(string, string, string, string)`, or `string.Concat(string[])` for 5+ parts). The `+` operator for mixed types (`"text" + value`) emits `string.Concat(object, object)` with an implicit `Expression.Convert` to `object`. These produce structurally different expression trees for semantically equivalent operations. Both translate correctly in EF Core (with `FlattenConcatArrayCalls` rewriting the array form), but may produce slightly different SQL.
