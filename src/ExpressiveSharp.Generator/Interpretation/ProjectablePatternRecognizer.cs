using ExpressiveSharp.Generator.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ExpressiveSharp.Generator.Interpretation;

/// <summary>
/// Recognizes the IOperation shapes produced by a <c>[Expressive(Projectable = true)]</c> property.
/// <para>
/// Two get-accessor shapes are supported:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       <b>Coalesce</b>: <c>=&gt; field ?? (&lt;formula&gt;)</c> or
///       <c>=&gt; _backingField ?? (&lt;formula&gt;)</c>. The backing field is either the C# 14
///       synthesized property backing field or a manually declared private nullable field whose
///       element type matches the property type. The set/init accessor must be a single
///       <c>field = value</c> assignment.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Ternary</b>: <c>=&gt; _hasValue ? field : (&lt;formula&gt;)</c> where
///       <c>_hasValue</c> is a private non-readonly instance <c>bool</c> field on the same
///       containing type. The backing field may be either the C# 14 synthesized backing field
///       or a manually declared private instance field of type <c>T</c> (or <c>T?</c>) matching
///       the property type. The set/init accessor must be a two-statement block that assigns
///       <c>true</c> to the flag and <c>value</c> to the backing field. This shape is required
///       for projectable properties whose type is nullable — the flag distinguishes
///       &quot;not materialized&quot; from &quot;materialized to null&quot;.
///     </description>
///   </item>
/// </list>
/// </summary>
static internal class ProjectablePatternRecognizer
{
    internal enum ProjectableGetShape
    {
        Coalesce,
        Ternary,
    }

    /// <summary>
    /// Outcome of recognizing the get accessor pattern. For the <see cref="ProjectableGetShape.Coalesce"/>
    /// shape, <see cref="HasValueFlag"/> is <c>null</c>. For the <see cref="ProjectableGetShape.Ternary"/>
    /// shape, all three symbol/operation fields are populated.
    /// </summary>
    internal readonly record struct ProjectableGetResult(
        ProjectableGetShape Shape,
        IFieldSymbol BackingField,
        IFieldSymbol? HasValueFlag,
        IOperation Formula);

    /// <summary>
    /// Inspects the get accessor's IOperation. On success, returns <c>true</c> and populates
    /// <paramref name="result"/> with the shape, backing field, optional has-value flag, and
    /// the formula operation (the branch that evaluates when the property is not yet
    /// materialized). Reports EXP0022 or EXP0025 on mismatch.
    /// </summary>
    public static bool TryRecognizeGetPattern(
        IPropertySymbol property,
        IOperation getAccessorOperation,
        SourceProductionContext context,
        Location diagnosticLocation,
        out ProjectableGetResult result)
    {
        result = default;

        var body = UnwrapToReturnExpression(getAccessorOperation);
        if (body is null)
        {
            ReportGetAccessorPattern(context, diagnosticLocation, "get accessor body is empty or not a single return expression");
            return false;
        }

        return body switch
        {
            ICoalesceOperation coalesce => TryRecognizeCoalesce(property, coalesce, context, diagnosticLocation, out result),
            IConditionalOperation conditional => TryRecognizeTernary(property, conditional, context, diagnosticLocation, out result),
            _ => ReportAndFail(context, diagnosticLocation, DescribeOperation(body)),
        };
    }

    private static bool TryRecognizeCoalesce(
        IPropertySymbol property,
        ICoalesceOperation coalesce,
        SourceProductionContext context,
        Location diagnosticLocation,
        out ProjectableGetResult result)
    {
        result = default;

        if (!TryMatchBackingFieldReference(coalesce.Value, property, context, diagnosticLocation, out var backingField))
        {
            return false;
        }

        // Coalesce requires the backing field to be nullable-of-property-type (the original rule):
        // the C# 14 synthesized field already matches the property type; a manual field must be T?.
        if (!IsValidCoalesceBackingFieldType(backingField!.Type, property.Type))
        {
            ReportBackingFieldTypeMismatch(context, diagnosticLocation,
                property.Name,
                property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                backingField.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            return false;
        }

        var formula = coalesce.WhenNull;
        if (formula is null)
        {
            ReportGetAccessorPattern(context, diagnosticLocation, "coalesce '??' is missing its right-hand side");
            return false;
        }

        result = new ProjectableGetResult(ProjectableGetShape.Coalesce, backingField!, HasValueFlag: null, Formula: formula);
        return true;
    }

    private static bool TryRecognizeTernary(
        IPropertySymbol property,
        IConditionalOperation conditional,
        SourceProductionContext context,
        Location diagnosticLocation,
        out ProjectableGetResult result)
    {
        result = default;

        // Only the bare `flag ? field : formula` shape is supported in v1 — reject the inverted
        // `!flag ? formula : field` form with a pointed message.
        if (conditional.Condition is IUnaryOperation { OperatorKind: UnaryOperatorKind.Not })
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                "ternary projectable pattern with inverted condition (e.g. '!_hasValue ? formula : field') is not supported; write it as '_hasValue ? field : formula'");
            return false;
        }

        if (!TryMatchFlagFieldReference(conditional.Condition, property, context, diagnosticLocation, out var flagField))
        {
            return false;
        }

        if (!TryMatchBackingFieldReference(conditional.WhenTrue, property, context, diagnosticLocation, out var backingField))
        {
            return false;
        }

        // Ternary accepts a broader backing-field type: either T or T? where T is the property type.
        // The cached-value branch is the raw `field` reference, which for a non-nullable property can
        // legitimately be either T (simple cache) or T? (auto-property with `field` keyword where the
        // property is nullable).
        if (!IsValidTernaryBackingFieldType(backingField!.Type, property.Type))
        {
            ReportBackingFieldTypeMismatch(context, diagnosticLocation,
                property.Name,
                property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                backingField.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            return false;
        }

        var formula = conditional.WhenFalse;
        if (formula is null)
        {
            ReportGetAccessorPattern(context, diagnosticLocation, "ternary '?:' is missing its else-branch (the formula)");
            return false;
        }

        result = new ProjectableGetResult(ProjectableGetShape.Ternary, backingField!, flagField, formula);
        return true;
    }

    /// <summary>
    /// Validates that the init/set accessor body matches the pattern implied by
    /// <paramref name="getResult"/>. For the Coalesce shape: a single assignment
    /// <c>backingField = value</c>. For the Ternary shape: a two-statement block assigning
    /// <c>true</c> to the has-value flag and <c>value</c> to the backing field (order
    /// independent). Reports EXP0023 or EXP0030 on mismatch.
    /// </summary>
    public static bool ValidateSetterPattern(
        IOperation setterAccessorOperation,
        ProjectableGetResult getResult,
        SourceProductionContext context,
        Location diagnosticLocation)
    {
        var body = UnwrapToBody(setterAccessorOperation);
        if (body is null)
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor has no analyzable body");
            return false;
        }

        return getResult.Shape switch
        {
            ProjectableGetShape.Coalesce => ValidateCoalesceSetter(body, getResult.BackingField, context, diagnosticLocation),
            ProjectableGetShape.Ternary => ValidateTernarySetter(body, getResult.BackingField, getResult.HasValueFlag!, context, diagnosticLocation),
            _ => false,
        };
    }

    private static bool ValidateCoalesceSetter(
        IOperation body,
        IFieldSymbol expectedBackingField,
        SourceProductionContext context,
        Location diagnosticLocation)
    {
        var statement = ExtractSingleStatement(body);
        if (statement is null)
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor must contain exactly one assignment statement");
            return false;
        }

        if (!TryGetSimpleAssignment(statement, out var assignment))
        {
            ReportSetterPattern(context, diagnosticLocation,
                $"init/set accessor must be a simple assignment, found {DescribeOperation(statement)}");
            return false;
        }

        if (!IsAssignmentToField(assignment!, expectedBackingField, out var mismatchReason))
        {
            if (mismatchReason == AssignmentFieldMismatch.WrongField)
            {
                ReportInconsistentBacking(context, diagnosticLocation,
                    $"Getter reads from '{expectedBackingField.Name}' but setter writes to a different field.");
                return false;
            }
            ReportSetterPattern(context, diagnosticLocation,
                $"init/set accessor must assign into backing field '{expectedBackingField.Name}'");
            return false;
        }

        if (!IsPlainValueParameterReference(assignment!.Value))
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor value must be a plain reference to the implicit 'value' parameter; transformations like 'value?.Trim()' are not supported in v1");
            return false;
        }

        return true;
    }

    private static bool ValidateTernarySetter(
        IOperation body,
        IFieldSymbol expectedBackingField,
        IFieldSymbol expectedFlagField,
        SourceProductionContext context,
        Location diagnosticLocation)
    {
        if (body is not IBlockOperation block || block.Operations.Length != 2)
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor for a ternary-form Projectable property must be a block with exactly two statements: set the has-value flag to true and assign value to the backing field");
            return false;
        }

        if (!TryGetSimpleAssignment(block.Operations[0], out var firstAssignment)
            || !TryGetSimpleAssignment(block.Operations[1], out var secondAssignment))
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor for a ternary-form Projectable property must consist of two simple assignment statements");
            return false;
        }

        // Classify each statement as either the flag-assignment-to-true or the value-assignment-to-backing-field.
        // Accept either order.
        var (flagAssignment, valueAssignment) = ClassifyTernaryAssignments(
            firstAssignment!, secondAssignment!, expectedBackingField, expectedFlagField);

        if (flagAssignment is null || valueAssignment is null)
        {
            ReportInconsistentBacking(context, diagnosticLocation,
                $"Expected one assignment of 'true' to flag '{expectedFlagField.Name}' and one assignment of 'value' to backing field '{expectedBackingField.Name}'.");
            return false;
        }

        if (!IsBooleanLiteralTrue(flagAssignment.Value))
        {
            ReportSetterPattern(context, diagnosticLocation,
                $"init/set accessor must assign the literal 'true' to has-value flag '{expectedFlagField.Name}'");
            return false;
        }

        if (!IsPlainValueParameterReference(valueAssignment.Value))
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor value must be a plain reference to the implicit 'value' parameter; transformations are not supported");
            return false;
        }

        return true;
    }

    // ── Helpers: reference matching ────────────────────────────────────────

    private static bool TryMatchBackingFieldReference(
        IOperation operation,
        IPropertySymbol property,
        SourceProductionContext context,
        Location diagnosticLocation,
        out IFieldSymbol? backingField)
    {
        backingField = null;

        if (operation is not IFieldReferenceOperation fieldRef)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"expected a backing field reference, found {DescribeOperation(operation)}");
            return false;
        }

        if (fieldRef.Instance is not null and not IInstanceReferenceOperation)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                "the backing field reference must have an implicit 'this' receiver (or none, for the 'field' keyword)");
            return false;
        }

        if (!FieldMatchesPatternAOrB(property, fieldRef.Field, context, diagnosticLocation))
        {
            return false;
        }

        backingField = fieldRef.Field;
        return true;
    }

    private static bool TryMatchFlagFieldReference(
        IOperation operation,
        IPropertySymbol property,
        SourceProductionContext context,
        Location diagnosticLocation,
        out IFieldSymbol? flagField)
    {
        flagField = null;

        if (operation is not IFieldReferenceOperation fieldRef)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"the ternary condition must be a reference to a bool field, found {DescribeOperation(operation)}");
            return false;
        }

        if (fieldRef.Instance is not null and not IInstanceReferenceOperation)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                "the has-value flag must have an implicit 'this' receiver");
            return false;
        }

        var field = fieldRef.Field;

        if (field.Type.SpecialType != SpecialType.System_Boolean)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"the has-value flag '{field.Name}' must be of type 'bool', found '{field.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}'");
            return false;
        }

        if (field.IsReadOnly)
        {
            ReportSetterPattern(context, diagnosticLocation,
                $"the has-value flag '{field.Name}' must not be readonly");
            return false;
        }

        if (field.IsStatic != property.IsStatic
            || !SymbolEqualityComparer.Default.Equals(field.ContainingType, property.ContainingType))
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"the has-value flag '{field.Name}' must be an instance field on '{property.ContainingType.Name}' with the same static-ness as the property");
            return false;
        }

        flagField = field;
        return true;
    }

    private static bool FieldMatchesPatternAOrB(
        IPropertySymbol property,
        IFieldSymbol field,
        SourceProductionContext context,
        Location diagnosticLocation)
    {
        // Pattern A: C# 14 `field` keyword — the synthesized backing field whose AssociatedSymbol
        // is the containing property.
        if (field.IsImplicitlyDeclared
            && SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, property))
        {
            return true;
        }

        // Pattern B: manually declared private instance field on the same type.
        if (field.IsStatic
            || field.DeclaredAccessibility != Accessibility.Private
            || !SymbolEqualityComparer.Default.Equals(field.ContainingType, property.ContainingType))
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"the backing field '{field.Name}' must be the 'field' keyword or a private instance field on '{property.ContainingType.Name}'");
            return false;
        }

        return true;
    }

    private static bool IsValidCoalesceBackingFieldType(ITypeSymbol fieldType, ITypeSymbol propertyType)
    {
        // Field type matches property type exactly. Covers:
        //   - `string? FullName` + `field` keyword — both string? (nullable ref).
        //   - `decimal? Amount` + `field` keyword — both Nullable<decimal> (nullable value).
        //   - `string FullName` + `field` keyword in nullable-oblivious contexts.
        // In all these cases the C# compiler already verified the `??` is valid, so no further
        // nullability check is needed here. If the property type itself is nullable, EXP0024
        // fires later (in the dispatcher) with a more actionable diagnostic.
        if (SymbolEqualityComparer.Default.Equals(fieldType, propertyType))
        {
            return true;
        }

        if (propertyType.IsValueType)
        {
            // Non-nullable value type: manual backing field must be Nullable<T> where T matches
            // the property type.
            if (fieldType is INamedTypeSymbol named
                && named.IsGenericType
                && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                && SymbolEqualityComparer.Default.Equals(named.TypeArguments[0], propertyType))
            {
                return true;
            }
            return false;
        }

        // Reference-type property: the field must be the same underlying type (annotations
        // ignored — the compiler has already verified `??` is valid, so the field is nullable
        // in whatever sense it needs to be).
        return SymbolEqualityComparer.Default.Equals(fieldType.OriginalDefinition, propertyType.OriginalDefinition);
    }

    private static bool IsValidTernaryBackingFieldType(ITypeSymbol fieldType, ITypeSymbol propertyType)
    {
        // Ternary accepts either T or T? for the backing field: the true-branch is the cached
        // value, which can be either the property type itself (non-nullable cache of a non-nullable
        // property) or Nullable<T> (e.g. field keyword on a `decimal?` property, or a manual `decimal? _x`
        // paired with a `decimal Amount` — users legitimately do this).
        if (SymbolEqualityComparer.Default.Equals(fieldType.OriginalDefinition, propertyType.OriginalDefinition))
        {
            return true;
        }

        if (propertyType.IsValueType
            && fieldType is INamedTypeSymbol named
            && named.IsGenericType
            && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
            && SymbolEqualityComparer.Default.Equals(named.TypeArguments[0], propertyType))
        {
            return true;
        }

        // Reference-type property: T? as a field type still has the same OriginalDefinition, so the
        // first check already covers it.
        return false;
    }

    // ── Helpers: assignment pattern matching ────────────────────────────────

    private enum AssignmentFieldMismatch
    {
        None,
        NotFieldTarget,
        WrongField,
    }

    private static bool TryGetSimpleAssignment(IOperation statement, out ISimpleAssignmentOperation? assignment)
    {
        if (statement is IExpressionStatementOperation { Operation: ISimpleAssignmentOperation simple })
        {
            assignment = simple;
            return true;
        }
        assignment = null;
        return false;
    }

    private static bool IsAssignmentToField(
        ISimpleAssignmentOperation assignment,
        IFieldSymbol expectedField,
        out AssignmentFieldMismatch reason)
    {
        if (assignment.Target is not IFieldReferenceOperation targetFieldRef)
        {
            reason = AssignmentFieldMismatch.NotFieldTarget;
            return false;
        }
        if (!SymbolEqualityComparer.Default.Equals(targetFieldRef.Field, expectedField))
        {
            reason = AssignmentFieldMismatch.WrongField;
            return false;
        }
        reason = AssignmentFieldMismatch.None;
        return true;
    }

    private static (ISimpleAssignmentOperation? Flag, ISimpleAssignmentOperation? Value) ClassifyTernaryAssignments(
        ISimpleAssignmentOperation first,
        ISimpleAssignmentOperation second,
        IFieldSymbol expectedBackingField,
        IFieldSymbol expectedFlagField)
    {
        var firstTarget = (first.Target as IFieldReferenceOperation)?.Field;
        var secondTarget = (second.Target as IFieldReferenceOperation)?.Field;

        if (firstTarget is null || secondTarget is null)
        {
            return (null, null);
        }

        if (SymbolEqualityComparer.Default.Equals(firstTarget, expectedFlagField)
            && SymbolEqualityComparer.Default.Equals(secondTarget, expectedBackingField))
        {
            return (first, second);
        }

        if (SymbolEqualityComparer.Default.Equals(firstTarget, expectedBackingField)
            && SymbolEqualityComparer.Default.Equals(secondTarget, expectedFlagField))
        {
            return (second, first);
        }

        return (null, null);
    }

    private static bool IsPlainValueParameterReference(IOperation operation)
    {
        // Peek through implicit conversions: `field = value` where field is `T?` and value is `T`
        // produces an IConversionOperation wrapping the parameter reference. Explicit conversions
        // are intentionally still rejected.
        while (operation is IConversionOperation { IsImplicit: true } conversion)
        {
            operation = conversion.Operand;
        }

        return operation is IParameterReferenceOperation paramRef
            && paramRef.Parameter.Name == "value"
            && paramRef.Parameter.IsImplicitlyDeclared;
    }

    private static bool IsBooleanLiteralTrue(IOperation operation)
    {
        // Peek through implicit conversions (unlikely for `true`, but harmless).
        while (operation is IConversionOperation { IsImplicit: true } conversion)
        {
            operation = conversion.Operand;
        }

        return operation is ILiteralOperation literal
            && literal.ConstantValue.HasValue
            && literal.ConstantValue.Value is true;
    }

    // ── Helpers: body unwrapping ────────────────────────────────────────────

    private static IOperation? UnwrapToReturnExpression(IOperation operation)
    {
        var current = operation;
        while (true)
        {
            switch (current)
            {
                case IMethodBodyOperation methodBody:
                    current = methodBody.BlockBody ?? methodBody.ExpressionBody;
                    if (current is null) return null;
                    break;

                case IBlockOperation block:
                    if (block.Operations.Length != 1) return null;
                    current = block.Operations[0];
                    break;

                case IReturnOperation ret:
                    if (ret.ReturnedValue is null) return null;
                    current = ret.ReturnedValue;
                    break;

                // Peek through implicit conversions wrapping the body expression. This matters
                // when the coalesce/ternary result type narrower than the property type (e.g.
                // `decimal? Amount => field ?? TotalAmount ?? 0m` where the coalesce is `decimal`
                // and gets implicitly converted to `decimal?`). Without this, the recognizer
                // would see the conversion instead of the expected ICoalesceOperation/IConditionalOperation.
                case IConversionOperation conversion when conversion.IsImplicit:
                    if (conversion.Operand is null) return current;
                    current = conversion.Operand;
                    break;

                default:
                    return current;
            }
        }
    }

    /// <summary>
    /// Unwraps an accessor's IOperation down to the user-written body, which is either an
    /// <see cref="IBlockOperation"/> (single statement or multi-statement block) or a single
    /// statement operation. Used by setter validation where both shapes are legal.
    /// </summary>
    private static IOperation? UnwrapToBody(IOperation operation)
    {
        if (operation is IMethodBodyOperation methodBody)
        {
            return methodBody.BlockBody ?? methodBody.ExpressionBody;
        }
        return operation;
    }

    private static IOperation? ExtractSingleStatement(IOperation body)
    {
        if (body is IBlockOperation block)
        {
            return block.Operations.Length == 1 ? block.Operations[0] : null;
        }
        return body;
    }

    // ── Helpers: diagnostics ────────────────────────────────────────────────

    private static string DescribeOperation(IOperation? operation) => operation switch
    {
        null => "<null>",
        ICoalesceOperation => "coalesce (??)",
        IConditionalOperation => "ternary (?:)",
        IBinaryOperation bin => $"binary operator '{bin.OperatorKind}'",
        IUnaryOperation un => $"unary operator '{un.OperatorKind}'",
        IInvocationOperation => "method invocation",
        IPropertyReferenceOperation => "property access",
        IFieldReferenceOperation => "field access",
        IParameterReferenceOperation => "parameter reference",
        ILiteralOperation => "literal",
        IExpressionStatementOperation => "expression statement",
        ISimpleAssignmentOperation => "simple assignment",
        _ => operation.Kind.ToString()
    };

    private static bool ReportAndFail(SourceProductionContext context, Location location, string detail)
    {
        ReportGetAccessorPattern(context, location, detail);
        return false;
    }

    private static void ReportGetAccessorPattern(
        SourceProductionContext context, Location location, string detail) =>
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.ProjectableGetAccessorPattern, location, detail));

    private static void ReportSetterPattern(
        SourceProductionContext context, Location location, string detail) =>
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.ProjectableSetterMustStoreToBackingField, location, detail));

    private static void ReportBackingFieldTypeMismatch(
        SourceProductionContext context, Location location,
        string propertyName, string propertyType, string actualType) =>
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.ProjectableBackingFieldTypeMismatch, location, propertyName, propertyType, actualType));

    private static void ReportInconsistentBacking(
        SourceProductionContext context, Location location, string detail) =>
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.ProjectableInconsistentGetSetBacking, location, detail));
}
