using ExpressiveSharp.Generator.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ExpressiveSharp.Generator.Interpretation;

/// <summary>
/// Recognizes the IOperation shape produced by a <c>[Expressive(Projectable = true)]</c> property.
/// <para>
/// The get accessor must be of the form <c>field ?? (&lt;formula&gt;)</c> or
/// <c>_backingField ?? (&lt;formula&gt;)</c> where the backing field is either the C# 14
/// synthesized property backing field or a manually declared private nullable field of the
/// corresponding type.
/// </para>
/// <para>
/// The init/set accessor must be a single statement that assigns the implicit <c>value</c>
/// parameter into the same backing field.
/// </para>
/// </summary>
static internal class ProjectablePatternRecognizer
{
    /// <summary>
    /// Inspects the get accessor's IOperation and, if it matches the Projectable pattern,
    /// returns the backing field symbol and the formula operation (the right operand of the coalesce).
    /// Reports EXP0022 or EXP0025 on mismatch.
    /// </summary>
    public static bool TryRecognizeGetPattern(
        IPropertySymbol property,
        IOperation getAccessorOperation,
        SourceProductionContext context,
        Location diagnosticLocation,
        out IFieldSymbol? backingField,
        out IOperation? formulaOperation)
    {
        backingField = null;
        formulaOperation = null;

        // Step 1: unwrap MethodBodyOperation → BlockOperation → ReturnOperation → <expression>.
        // Expression-bodied accessors produce the same shape through Roslyn.
        var coalesce = UnwrapToReturnExpression(getAccessorOperation);
        if (coalesce is null)
        {
            ReportGetAccessorPattern(context, diagnosticLocation, "get accessor body is empty or not a single return expression");
            return false;
        }

        // Step 2: the return expression must be a top-level ?? coalesce.
        if (coalesce is not ICoalesceOperation coalesceOp)
        {
            ReportGetAccessorPattern(context, diagnosticLocation, DescribeOperation(coalesce));
            return false;
        }

        // Step 3: left operand must be a backing field reference.
        if (coalesceOp.Value is not IFieldReferenceOperation fieldRef)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"the left operand of '??' must be a backing field reference, found {DescribeOperation(coalesceOp.Value)}");
            return false;
        }

        // Receiver must be null (field keyword, no explicit receiver) or an implicit `this`.
        if (fieldRef.Instance is not null and not IInstanceReferenceOperation)
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                "the backing field reference must have an implicit 'this' receiver (or none, for the 'field' keyword)");
            return false;
        }

        // Step 4: the field must satisfy Pattern A (C# 14 field keyword) or Pattern B (manual private nullable field).
        if (!MatchesPatternAOrB(property, fieldRef.Field, context, diagnosticLocation))
        {
            return false;
        }

        backingField = fieldRef.Field;
        formulaOperation = coalesceOp.WhenNull;
        return true;
    }

    /// <summary>
    /// Validates that the init/set accessor body is a single assignment of the implicit
    /// <c>value</c> parameter into the same backing field recognized in the get accessor.
    /// Reports EXP0023 on mismatch.
    /// </summary>
    public static bool ValidateSetterPattern(
        IOperation setterAccessorOperation,
        IFieldSymbol expectedBackingField,
        SourceProductionContext context,
        Location diagnosticLocation)
    {
        // Unwrap MethodBodyOperation → BlockOperation → single statement.
        var statement = UnwrapToSingleStatement(setterAccessorOperation);
        if (statement is null)
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor must contain exactly one assignment statement");
            return false;
        }

        if (statement is not IExpressionStatementOperation { Operation: ISimpleAssignmentOperation assignment })
        {
            ReportSetterPattern(context, diagnosticLocation,
                $"init/set accessor must be a simple assignment, found {DescribeOperation(statement)}");
            return false;
        }

        if (assignment.Target is not IFieldReferenceOperation targetFieldRef)
        {
            ReportSetterPattern(context, diagnosticLocation,
                $"init/set accessor must assign into a backing field, found {DescribeOperation(assignment.Target)}");
            return false;
        }

        if (!SymbolEqualityComparer.Default.Equals(targetFieldRef.Field, expectedBackingField))
        {
            ReportSetterPattern(context, diagnosticLocation,
                $"init/set accessor assigns into '{targetFieldRef.Field.Name}' but the get accessor reads from '{expectedBackingField.Name}'");
            return false;
        }

        if (assignment.Value is not IParameterReferenceOperation paramRef
            || paramRef.Parameter.Name != "value"
            || !paramRef.Parameter.IsImplicitlyDeclared)
        {
            ReportSetterPattern(context, diagnosticLocation,
                "init/set accessor value must be a plain reference to the implicit 'value' parameter; transformations like 'value?.Trim()' are not supported in v1");
            return false;
        }

        return true;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static bool MatchesPatternAOrB(
        IPropertySymbol property,
        IFieldSymbol field,
        SourceProductionContext context,
        Location diagnosticLocation)
    {
        // Pattern A: C# 14 `field` keyword — the synthesized backing field whose AssociatedSymbol
        // is the containing property. Probed against Roslyn 5.0.0 and confirmed to surface both
        // IsImplicitlyDeclared == true and AssociatedSymbol == the property symbol.
        if (field.IsImplicitlyDeclared
            && SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, property))
        {
            return true;
        }

        // Pattern B: manually declared private nullable backing field on the same type.
        if (field.DeclaredAccessibility != Accessibility.Private
            || !SymbolEqualityComparer.Default.Equals(field.ContainingType, property.ContainingType))
        {
            ReportGetAccessorPattern(context, diagnosticLocation,
                $"the backing field '{field.Name}' must be the 'field' keyword or a private instance field on '{property.ContainingType.Name}'");
            return false;
        }

        if (!IsNullableOfPropertyType(field.Type, property.Type))
        {
            ReportBackingFieldTypeMismatch(context, diagnosticLocation,
                property.Name,
                property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                field.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            return false;
        }

        return true;
    }

    private static bool IsNullableOfPropertyType(ITypeSymbol fieldType, ITypeSymbol propertyType)
    {
        // For value types: manually declared backing field must be Nullable<T> where T matches
        // the property type.
        if (propertyType.IsValueType)
        {
            if (fieldType is not INamedTypeSymbol named || !named.IsGenericType) return false;
            if (named.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T) return false;
            return SymbolEqualityComparer.Default.Equals(named.TypeArguments[0], propertyType);
        }

        // For reference types: manually declared backing field must be the same type with a
        // nullable annotation (e.g. string? for a string property).
        return SymbolEqualityComparer.Default.Equals(fieldType.OriginalDefinition, propertyType.OriginalDefinition)
            && fieldType.NullableAnnotation == NullableAnnotation.Annotated;
    }

    private static IOperation? UnwrapToReturnExpression(IOperation operation)
    {
        // Expression-bodied accessors may give us the expression directly; block-bodied produce
        // MethodBodyOperation → BlockOperation → ReturnOperation.
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
                    return ret.ReturnedValue;

                default:
                    return current;
            }
        }
    }

    private static IOperation? UnwrapToSingleStatement(IOperation operation)
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
                    return block.Operations[0];

                default:
                    return current;
            }
        }
    }

    private static string DescribeOperation(IOperation? operation) => operation switch
    {
        null => "<null>",
        ICoalesceOperation => "coalesce (??)",
        IBinaryOperation bin => $"binary operator '{bin.OperatorKind}'",
        IConditionalOperation => "ternary '?:' (use '??' instead)",
        IInvocationOperation => "method invocation",
        IPropertyReferenceOperation => "property access",
        IFieldReferenceOperation => "field access",
        IParameterReferenceOperation => "parameter reference",
        ILiteralOperation => "literal",
        IExpressionStatementOperation => "expression statement",
        ISimpleAssignmentOperation => "simple assignment",
        _ => operation.Kind.ToString()
    };

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
}
