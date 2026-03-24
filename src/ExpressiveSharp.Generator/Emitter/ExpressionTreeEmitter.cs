using System.Collections.Immutable;
using System.Text;
using ExpressiveSharp.Generator.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace ExpressiveSharp.Generator.Emitter;

/// <summary>
/// Describes a parameter for expression tree emission.
/// </summary>
internal sealed class EmitterParameter
{
    public string Name { get; }
    public string TypeFqn { get; }
    /// <summary>Optional: the Roslyn parameter symbol, used to match <see cref="IParameterReferenceOperation"/>.</summary>
    public IParameterSymbol? Symbol { get; }
    /// <summary>When true, this parameter is matched by <see cref="IInstanceReferenceOperation"/>.</summary>
    public bool IsThis { get; }

    public EmitterParameter(string name, string typeFqn, IParameterSymbol? symbol = null, bool isThis = false)
    {
        Name = name;
        TypeFqn = typeFqn;
        Symbol = symbol;
        IsThis = isThis;
    }
}

/// <summary>
/// Walks Roslyn <see cref="IOperation"/> nodes and emits C# code that builds
/// an equivalent <c>System.Linq.Expressions.Expression</c> tree using factory methods.
/// </summary>
internal sealed class ExpressionTreeEmitter
{
    private const string Expr = "global::System.Linq.Expressions.Expression";

    private static readonly SymbolDisplayFormat _fqnFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    private readonly SemanticModel _semanticModel;
    private readonly SourceProductionContext? _context;
    private readonly ReflectionFieldCache _fieldCache;
    private readonly List<string> _lines = new();
    private readonly Dictionary<IParameterSymbol, string> _symbolToVar = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<ILocalSymbol, string> _localToVar = new(SymbolEqualityComparer.Default);
    private string? _thisVarName;
    private int _varCounter;
    private int _lineCount;
    private readonly Stack<(string VarName, ITypeSymbol? Type)> _conditionalAccessReceiverStack = new();

    public ExpressionTreeEmitter(
        SemanticModel semanticModel,
        SourceProductionContext? context = null,
        string fieldPrefix = "")
    {
        _semanticModel = semanticModel;
        _context = context;
        _fieldCache = new ReflectionFieldCache(fieldPrefix);
    }

    /// <summary>
    /// Translates a lambda body expression into imperative <c>Expression.*</c> factory code.
    /// </summary>
    public EmitResult Emit(
        SyntaxNode bodySyntax,
        IReadOnlyList<EmitterParameter> parameters,
        string returnTypeFqn,
        string delegateTypeFqn,
        string? assignToVariable = null)
    {
        // Emit parameter declarations
        var paramVarNames = new List<string>();
        foreach (var param in parameters)
        {
            var varName = $"p_{SanitizeIdentifier(param.Name)}";
            paramVarNames.Add(varName);
            AppendLine($"var {varName} = {Expr}.Parameter(typeof({param.TypeFqn}), \"{param.Name}\");");

            if (param.Symbol is not null)
                _symbolToVar[param.Symbol] = varName;
            if (param.IsThis)
                _thisVarName = varName;
        }

        // Unwrap syntax wrappers that don't produce their own IOperation.
        // Without this, GetOperation returns null and the entire expression is silently lost.
        bodySyntax = UnwrapTransparentSyntax(bodySyntax);

        // Get IOperation for the body and emit it
        var operation = _semanticModel.GetOperation(bodySyntax);
        if (operation is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                bodySyntax.GetLocation(),
                bodySyntax.Kind().ToString());

            var fallbackVar = NextVar();
            AppendLine($"var {fallbackVar} = {Expr}.Default(typeof({returnTypeFqn}));");
            var fallbackParams = paramVarNames.Count > 0
                ? string.Join(", ", paramVarNames)
                : $"global::System.Array.Empty<global::System.Linq.Expressions.ParameterExpression>()";
            var stmt = assignToVariable is not null
                ? $"var {assignToVariable} = {Expr}.Lambda<{delegateTypeFqn}>({fallbackVar}, {fallbackParams});"
                : $"return {Expr}.Lambda<{delegateTypeFqn}>({fallbackVar}, {fallbackParams});";
            AppendLine(stmt);
            return BuildResult();
        }

        var bodyVar = EmitOperation(operation);

        var paramsArg = paramVarNames.Count > 0
            ? string.Join(", ", paramVarNames)
            : $"global::System.Array.Empty<global::System.Linq.Expressions.ParameterExpression>()";
        var finalStmt = assignToVariable is not null
            ? $"var {assignToVariable} = {Expr}.Lambda<{delegateTypeFqn}>({bodyVar}, {paramsArg});"
            : $"return {Expr}.Lambda<{delegateTypeFqn}>({bodyVar}, {paramsArg});";
        AppendLine(finalStmt);

        return BuildResult();
    }

    /// <summary>
    /// Translates a constructor body into a <c>MemberInit</c> expression tree.
    /// Collects property assignments from the body and emits
    /// <c>Expression.MemberInit(Expression.New(parameterless_ctor), bindings...)</c>.
    /// </summary>
    public EmitResult EmitConstructor(
        SyntaxNode bodySyntax,
        IReadOnlyList<EmitterParameter> parameters,
        string returnTypeFqn,
        string delegateTypeFqn,
        INamedTypeSymbol containingType,
        IReadOnlyList<(IPropertySymbol Property, SyntaxNode ValueSyntax)> delegatedBindings)
    {
        // Emit parameter declarations
        var paramVarNames = new List<string>();
        foreach (var param in parameters)
        {
            var varName = $"p_{SanitizeIdentifier(param.Name)}";
            paramVarNames.Add(varName);
            AppendLine($"var {varName} = {Expr}.Parameter(typeof({param.TypeFqn}), \"{param.Name}\");");

            if (param.Symbol is not null)
                _symbolToVar[param.Symbol] = varName;
        }

        // Get the parameterless constructor
        var parameterlessCtor = containingType.Constructors
            .FirstOrDefault(c => !c.IsStatic && c.Parameters.IsEmpty);
        var ctorField = parameterlessCtor is not null
            ? _fieldCache.EnsureConstructorInfo(parameterlessCtor)
            : null;

        var newVar = NextVar();
        if (ctorField is not null)
        {
            AppendLine($"var {newVar} = {Expr}.New({ctorField});");
        }
        else
        {
            AppendLine($"var {newVar} = {Expr}.New(typeof({returnTypeFqn}));");
        }

        // Accumulate property assignments: propName → (symbol, emitted value var)
        var propertyAssignments = new Dictionary<string, (ISymbol Symbol, string ValueVar)>();

        // Process the constructor body, collecting property assignments
        var operation = _semanticModel.GetOperation(UnwrapTransparentSyntax(bodySyntax));
        if (operation is IBlockOperation block)
        {
            ProcessConstructorStatements(block.Operations, propertyAssignments);
        }

        // Build MemberInit bindings from accumulated assignments
        var bindingVars = new List<string>();
        foreach (var kvp in propertyAssignments)
        {
            var symbol = kvp.Value.Symbol;
            var valueVar = kvp.Value.ValueVar;
            var bindingVar = NextVar();
            if (symbol is IPropertySymbol prop)
            {
                var propField = _fieldCache.EnsurePropertyInfo(prop);
                AppendLine($"var {bindingVar} = {Expr}.Bind({propField}, {valueVar});");
            }
            else if (symbol is IFieldSymbol field)
            {
                var fieldField = _fieldCache.EnsureFieldInfo(field);
                AppendLine($"var {bindingVar} = {Expr}.Bind({fieldField}, {valueVar});");
            }
            else
            {
                continue;
            }
            bindingVars.Add(bindingVar);
        }

        var resultVar = NextVar();
        if (bindingVars.Count > 0)
        {
            var bindingsExpr = string.Join(", ", bindingVars);
            AppendLine($"var {resultVar} = {Expr}.MemberInit({newVar}, {bindingsExpr});");
        }
        else
        {
            resultVar = newVar;
        }

        var paramsArg = paramVarNames.Count > 0
            ? string.Join(", ", paramVarNames)
            : $"global::System.Array.Empty<global::System.Linq.Expressions.ParameterExpression>()";
        AppendLine($"return {Expr}.Lambda<{delegateTypeFqn}>({resultVar}, {paramsArg});");

        return BuildResult();
    }

    /// <summary>
    /// Processes constructor statements, accumulating property assignments into the map.
    /// Handles simple assignments, if/else (merges branches with ternary), local variables,
    /// and early returns (ignored — assignments before the return are kept).
    /// </summary>
    private void ProcessConstructorStatements(
        ImmutableArray<IOperation> operations,
        Dictionary<string, (ISymbol Symbol, string ValueVar)> assignments)
    {
        foreach (var op in operations)
        {
            switch (op)
            {
                case IExpressionStatementOperation { Operation: ISimpleAssignmentOperation assignment }:
                    ProcessConstructorAssignment(assignment, assignments);
                    break;

                case IConditionalOperation conditional:
                    ProcessConstructorConditional(conditional, assignments);
                    break;

                case IVariableDeclarationGroupOperation varDecl:
                    foreach (var declaration in varDecl.Declarations)
                    {
                        foreach (var declarator in declaration.Declarators)
                        {
                            var localSymbol = declarator.Symbol;
                            var localTypeFqn = localSymbol.Type.ToDisplayString(_fqnFormat);
                            var localVar = NextVar();
                            AppendLine($"var {localVar} = {Expr}.Variable(typeof({localTypeFqn}), \"{localSymbol.Name}\");");
                            _localToVar[localSymbol] = localVar;

                            if (declarator.Initializer is not null)
                            {
                                var initVar = EmitOperation(declarator.Initializer.Value);
                                // For constructor local variables, store the value expression
                                // so it can be referenced later (e.g. in conditions)
                                var assignVar = NextVar();
                                AppendLine($"var {assignVar} = {Expr}.Assign({localVar}, {initVar});");
                            }
                        }
                    }
                    break;

                case IReturnOperation:
                    // Early returns in constructors — stop processing further statements
                    return;

                case IBlockOperation nestedBlock:
                    // Nested block scope (e.g. { if (...) { ... } })
                    ProcessConstructorStatements(nestedBlock.Operations, assignments);
                    break;

                default:
                    // Skip other statement types
                    break;
            }
        }
    }

    private void ProcessConstructorAssignment(
        ISimpleAssignmentOperation assignment,
        Dictionary<string, (ISymbol Symbol, string ValueVar)> assignments)
    {
        ISymbol? memberSymbol = null;
        string? propName = null;

        if (assignment.Target is IPropertyReferenceOperation propRef)
        {
            memberSymbol = propRef.Property;
            propName = propRef.Property.Name;
        }
        else if (assignment.Target is IFieldReferenceOperation fieldRef)
        {
            memberSymbol = fieldRef.Field;
            propName = fieldRef.Field.Name;
        }

        if (memberSymbol is null || propName is null)
            return;

        var valueVar = EmitOperation(assignment.Value);
        assignments[propName] = (memberSymbol, valueVar);
    }

    private void ProcessConstructorConditional(
        IConditionalOperation conditional,
        Dictionary<string, (ISymbol Symbol, string ValueVar)> assignments)
    {
        // Collect assignments from both branches
        var trueAssignments = new Dictionary<string, (ISymbol Symbol, string ValueVar)>();
        var falseAssignments = new Dictionary<string, (ISymbol Symbol, string ValueVar)>();

        if (conditional.WhenTrue is IBlockOperation trueBlock)
            ProcessConstructorStatements(trueBlock.Operations, trueAssignments);
        else if (conditional.WhenTrue is IExpressionStatementOperation { Operation: ISimpleAssignmentOperation trueAssign })
            ProcessConstructorAssignment(trueAssign, trueAssignments);

        if (conditional.WhenFalse is IBlockOperation falseBlock)
            ProcessConstructorStatements(falseBlock.Operations, falseAssignments);
        else if (conditional.WhenFalse is IExpressionStatementOperation { Operation: ISimpleAssignmentOperation falseAssign })
            ProcessConstructorAssignment(falseAssign, falseAssignments);
        else if (conditional.WhenFalse is IConditionalOperation elseIf)
        {
            // else if chain
            ProcessConstructorConditional(elseIf, falseAssignments);
        }

        var conditionVar = EmitOperation(conditional.Condition);

        // Merge: for each property assigned in either branch, produce a ternary
        var allProps = new HashSet<string>(trueAssignments.Keys.Union(falseAssignments.Keys));
        foreach (var propName in allProps)
        {
            trueAssignments.TryGetValue(propName, out var trueEntry);
            falseAssignments.TryGetValue(propName, out var falseEntry);

            // Determine symbol (from whichever branch has it)
            var symbol = trueEntry.Symbol ?? falseEntry.Symbol;
            if (symbol is null) continue;

            // Determine type for the ternary
            var typeFqn = symbol switch
            {
                IPropertySymbol p => p.Type.ToDisplayString(_fqnFormat),
                IFieldSymbol f => f.Type.ToDisplayString(_fqnFormat),
                _ => "object"
            };

            // True value: from true branch, or fall back to previous assignment
            var trueVal = trueEntry.ValueVar;
            if (trueVal is null)
            {
                // Property not assigned in true branch — use the previously accumulated value
                if (assignments.TryGetValue(propName, out var prev))
                    trueVal = prev.ValueVar;
                else
                {
                    trueVal = NextVar();
                    AppendLine($"var {trueVal} = {Expr}.Default(typeof({typeFqn}));");
                }
            }

            // False value: from false branch, or fall back to previous assignment
            var falseVal = falseEntry.ValueVar;
            if (falseVal is null)
            {
                if (assignments.TryGetValue(propName, out var prev))
                    falseVal = prev.ValueVar;
                else
                {
                    falseVal = NextVar();
                    AppendLine($"var {falseVal} = {Expr}.Default(typeof({typeFqn}));");
                }
            }

            var ternaryVar = NextVar();
            AppendLine($"var {ternaryVar} = {Expr}.Condition({conditionVar}, {trueVal}, {falseVal}, typeof({typeFqn}));");
            assignments[propName] = (symbol, ternaryVar);
        }
    }

    // ── Main dispatch ────────────────────────────────────────────────────────

    private string EmitOperation(IOperation operation)
    {
        var lineCountBefore = _lineCount;

        var result = operation switch
        {
            ILiteralOperation literal => EmitLiteral(literal),
            IParameterReferenceOperation paramRef => EmitParameterReference(paramRef),
            IInstanceReferenceOperation => EmitInstanceReference(),
            ILocalReferenceOperation localRef => EmitLocalReference(localRef),
            IPropertyReferenceOperation propRef => EmitPropertyReference(propRef),
            IFieldReferenceOperation fieldRef => EmitFieldReference(fieldRef),
            IInvocationOperation invocation => EmitInvocation(invocation),
            IBinaryOperation binary => EmitBinary(binary),
            IUnaryOperation unary => EmitUnary(unary),
            IConversionOperation conversion => EmitConversion(conversion),
            IConditionalOperation conditional => EmitConditional(conditional),
            IObjectCreationOperation creation => EmitObjectCreation(creation),
            IDefaultValueOperation defaultVal => EmitDefault(defaultVal),
            ITypeOfOperation typeOf => EmitTypeOf(typeOf),
            IParenthesizedOperation paren => EmitOperation(paren.Operand),
            IIsTypeOperation isType => EmitIsType(isType),
            ICoalesceOperation coalesce => EmitCoalesce(coalesce),
            IArrayCreationOperation arrayCreate => EmitArrayCreation(arrayCreate),
            IArrayElementReferenceOperation arrayElement => EmitArrayElementReference(arrayElement),
            IAnonymousFunctionOperation lambda => EmitNestedLambda(lambda),
            IDelegateCreationOperation delegateCreate => EmitDelegateCreation(delegateCreate),
            ITupleOperation tuple => EmitTuple(tuple),
            IIsPatternOperation isPattern => EmitIsPattern(isPattern),
            ISwitchExpressionOperation switchExpr => EmitSwitchExpression(switchExpr),
            IConditionalAccessOperation condAccess => EmitConditionalAccess(condAccess),
            IConditionalAccessInstanceOperation => EmitConditionalAccessInstance(),
            IBlockOperation block => EmitBlock(block),
            IReturnOperation ret => EmitReturn(ret),
            IInterpolatedStringOperation interp => EmitInterpolatedString(interp),
            _ => EmitUnsupported(operation),
        };

        // Annotate the first line emitted by this operation with the source syntax
        if (_lineCount > lineCountBefore && operation.Syntax is not null)
        {
            var syntaxText = operation.Syntax.ToString().Replace("\r", "").Replace("\n", " ");
            if (syntaxText.Length > 60)
                syntaxText = syntaxText.Substring(0, 57) + "...";
            AnnotateFirstLine(lineCountBefore, syntaxText);
        }

        return result;
    }

    // ── Literals ─────────────────────────────────────────────────────────────

    private string EmitLiteral(ILiteralOperation literal)
    {
        var resultVar = NextVar();
        var type = literal.Type;
        var typeFqn = type?.ToDisplayString(_fqnFormat) ?? "object";

        if (literal.ConstantValue.HasValue)
        {
            var value = literal.ConstantValue.Value;
            var valueLiteral = FormatConstantValue(value, type);
            AppendLine($"var {resultVar} = {Expr}.Constant({valueLiteral}, typeof({typeFqn}));");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Constant(null, typeof({typeFqn}));");
        }

        return resultVar;
    }

    // ── Parameter & instance references ──────────────────────────────────────

    private string EmitParameterReference(IParameterReferenceOperation paramRef)
    {
        if (_symbolToVar.TryGetValue(paramRef.Parameter, out var varName))
            return varName;

        // Fallback: parameter not in our map
        var resultVar = NextVar();
        var typeFqn = paramRef.Parameter.Type.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.Parameter(typeof({typeFqn}), \"{paramRef.Parameter.Name}\");");
        _symbolToVar[paramRef.Parameter] = resultVar;
        return resultVar;
    }

    private string EmitLocalReference(ILocalReferenceOperation localRef)
    {
        if (_localToVar.TryGetValue(localRef.Local, out var varName))
            return varName;

        // Fallback: local not yet declared (shouldn't happen in well-formed code)
        var resultVar = NextVar();
        var typeFqn = localRef.Local.Type.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.Variable(typeof({typeFqn}), \"{localRef.Local.Name}\");");
        _localToVar[localRef.Local] = resultVar;
        return resultVar;
    }

    private string EmitInstanceReference()
    {
        if (_thisVarName is not null)
            return _thisVarName;

        // Fallback
        _thisVarName = "p___this";
        AppendLine($"var {_thisVarName} = {Expr}.Parameter(typeof(object), \"@this\");");
        return _thisVarName;
    }

    // ── Member access ────────────────────────────────────────────────────────

    private string EmitPropertyReference(IPropertyReferenceOperation propRef)
    {
        var resultVar = NextVar();
        var fieldName = _fieldCache.EnsurePropertyInfo(propRef.Property);

        if (propRef.Instance is not null)
        {
            var instanceVar = EmitOperation(propRef.Instance);
            AppendLine($"var {resultVar} = {Expr}.Property({instanceVar}, {fieldName});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Property(null, {fieldName});");
        }

        return resultVar;
    }

    private string EmitFieldReference(IFieldReferenceOperation fieldRef)
    {
        var resultVar = NextVar();
        var fieldName = _fieldCache.EnsureFieldInfo(fieldRef.Field);

        if (fieldRef.Instance is not null)
        {
            var instanceVar = EmitOperation(fieldRef.Instance);
            AppendLine($"var {resultVar} = {Expr}.Field({instanceVar}, {fieldName});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Field(null, {fieldName});");
        }

        return resultVar;
    }

    // ── Invocations ──────────────────────────────────────────────────────────

    private bool TryEmitEnumMethodExpansion(IInvocationOperation invocation, out string resultVar)
    {
        resultVar = "";

        var method = invocation.TargetMethod;

        // Determine the enum receiver — could be instance call or extension method (first arg)
        ITypeSymbol? receiverType = null;
        IOperation? receiverOperation = null;

        if (invocation.Instance is not null)
        {
            receiverType = invocation.Instance.Type;
            receiverOperation = invocation.Instance;
        }
        else if (method.IsExtensionMethod && invocation.Arguments.Length > 0)
        {
            receiverType = invocation.Arguments[0].Value.Type;
            receiverOperation = invocation.Arguments[0].Value;
        }

        if (receiverType is null || receiverOperation is null)
            return false;

        // Handle Nullable<EnumType>
        ITypeSymbol enumType;
        var isNullable = false;
        if (receiverType is INamedTypeSymbol { IsGenericType: true } nullableType &&
            nullableType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
            nullableType.TypeArguments[0].TypeKind == TypeKind.Enum)
        {
            enumType = nullableType.TypeArguments[0];
            isNullable = true;
        }
        else if (receiverType.TypeKind == TypeKind.Enum)
        {
            enumType = receiverType;
        }
        else
        {
            return false;
        }

        var enumMembers = enumType.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.HasConstantValue)
            .ToList();

        if (enumMembers.Count == 0)
            return false;

        var receiverVar = EmitOperation(receiverOperation);
        var returnType = method.ReturnType;
        var returnTypeFqn = returnType.ToDisplayString(_fqnFormat);
        var enumTypeFqn = enumType.ToDisplayString(_fqnFormat);

        // Resolve the original (unreduced) method for the static call
        var originalMethod = method.ReducedFrom ?? method;
        var methodField = _fieldCache.EnsureMethodInfo(originalMethod);

        // Build default value
        string defaultVar;
        if (returnType.IsReferenceType || returnType.NullableAnnotation == NullableAnnotation.Annotated ||
            (returnType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T }))
        {
            defaultVar = NextVar();
            AppendLine($"var {defaultVar} = {Expr}.Constant(null, typeof({returnTypeFqn}));");
        }
        else
        {
            defaultVar = NextVar();
            AppendLine($"var {defaultVar} = {Expr}.Default(typeof({returnTypeFqn}));");
        }

        // Build ternary chain for each enum value (reverse order)
        var currentVar = defaultVar;
        foreach (var member in enumMembers.AsEnumerable().Reverse())
        {
            // Enum value constant
            var enumValueVar = NextVar();
            AppendLine($"var {enumValueVar} = {Expr}.Constant({enumTypeFqn}.{member.Name}, typeof({enumTypeFqn}));");

            // Method call: ExtensionClass.Method(EnumType.Value, ...additionalArgs)
            var callArgVars = new List<string> { enumValueVar };
            // Add any additional arguments (skip first arg for extension methods since it's the receiver)
            var argOffset = method.IsExtensionMethod ? 1 : 0;
            for (var i = argOffset; i < invocation.Arguments.Length; i++)
            {
                callArgVars.Add(EmitOperation(invocation.Arguments[i].Value));
            }
            var callArgsExpr = $"new global::System.Linq.Expressions.Expression[] {{ {string.Join(", ", callArgVars)} }}";

            var callVar = NextVar();
            AppendLine($"var {callVar} = {Expr}.Call({methodField}, {callArgsExpr});");

            // Condition: receiver == enumValue
            var condVar = NextVar();
            AppendLine($"var {condVar} = {Expr}.Equal({receiverVar}, {enumValueVar});");

            // Ternary: condition ? call : current
            var ternaryVar = NextVar();
            AppendLine($"var {ternaryVar} = {Expr}.Condition({condVar}, {callVar}, {currentVar}, typeof({returnTypeFqn}));");
            currentVar = ternaryVar;
        }

        // Nullable wrapper: receiver == null ? default : chain
        if (isNullable)
        {
            var nullConst = NextVar();
            var receiverTypeFqn = receiverType.ToDisplayString(_fqnFormat);
            AppendLine($"var {nullConst} = {Expr}.Constant(null, typeof({receiverTypeFqn}));");

            var nullCheck = NextVar();
            AppendLine($"var {nullCheck} = {Expr}.Equal({receiverVar}, {nullConst});");

            var wrappedVar = NextVar();
            AppendLine($"var {wrappedVar} = {Expr}.Condition({nullCheck}, {defaultVar}, {currentVar}, typeof({returnTypeFqn}));");
            currentVar = wrappedVar;
        }

        resultVar = currentVar;
        return true;
    }

    private string EmitInvocation(IInvocationOperation invocation)
    {
        // Enum method expansion: when the receiver is an enum type, expand to ternary chain
        // Always expand enum method calls to ternary chains
        if (TryEmitEnumMethodExpansion(invocation, out var enumResult))
        {
            return enumResult;
        }

        var resultVar = NextVar();
        var method = invocation.TargetMethod;
        var methodFieldName = _fieldCache.EnsureMethodInfo(method);

        var argVars = new List<string>();
        foreach (var arg in invocation.Arguments)
        {
            argVars.Add(EmitOperation(arg.Value));
        }

        var argsExpr = argVars.Count > 0
            ? $"new global::System.Linq.Expressions.Expression[] {{ {string.Join(", ", argVars)} }}"
            : "global::System.Array.Empty<global::System.Linq.Expressions.Expression>()";

        if (method.IsStatic || invocation.Instance is null)
        {
            AppendLine($"var {resultVar} = {Expr}.Call({methodFieldName}, {argsExpr});");
        }
        else
        {
            var instanceVar = EmitOperation(invocation.Instance);
            AppendLine($"var {resultVar} = {Expr}.Call({instanceVar}, {methodFieldName}, {argsExpr});");
        }

        return resultVar;
    }

    // ── Binary operators ─────────────────────────────────────────────────────

    private string EmitBinary(IBinaryOperation binary)
    {
        var exprType = MapBinaryOperatorKind(binary.OperatorKind);
        if (exprType is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperator, binary.Syntax?.GetLocation() ?? Location.None, binary.OperatorKind.ToString());
            return EmitUnsupported(binary);
        }

        // Use checked variants when in a checked context
        if (binary.IsChecked)
        {
            exprType = exprType switch
            {
                "Add" => "AddChecked",
                "Subtract" => "SubtractChecked",
                "Multiply" => "MultiplyChecked",
                _ => exprType,
            };
        }

        var resultVar = NextVar();
        var leftVar = EmitOperation(binary.LeftOperand);
        var rightVar = EmitOperation(binary.RightOperand);

        if (binary.OperatorMethod is not null)
        {
            var methodField = _fieldCache.EnsureMethodInfo(binary.OperatorMethod);
            AppendLine($"var {resultVar} = {Expr}.MakeBinary(global::System.Linq.Expressions.ExpressionType.{exprType}, {leftVar}, {rightVar}, false, {methodField});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.MakeBinary(global::System.Linq.Expressions.ExpressionType.{exprType}, {leftVar}, {rightVar});");
        }

        return resultVar;
    }

    private static string? MapBinaryOperatorKind(BinaryOperatorKind kind)
    {
        return kind switch
        {
            BinaryOperatorKind.Add => "Add",
            BinaryOperatorKind.Subtract => "Subtract",
            BinaryOperatorKind.Multiply => "Multiply",
            BinaryOperatorKind.Divide => "Divide",
            BinaryOperatorKind.Remainder => "Modulo",
            BinaryOperatorKind.LeftShift => "LeftShift",
            BinaryOperatorKind.RightShift => "RightShift",
            BinaryOperatorKind.And => "And",
            BinaryOperatorKind.Or => "Or",
            BinaryOperatorKind.ExclusiveOr => "ExclusiveOr",
            BinaryOperatorKind.ConditionalAnd => "AndAlso",
            BinaryOperatorKind.ConditionalOr => "OrElse",
            BinaryOperatorKind.Equals => "Equal",
            BinaryOperatorKind.NotEquals => "NotEqual",
            BinaryOperatorKind.LessThan => "LessThan",
            BinaryOperatorKind.LessThanOrEqual => "LessThanOrEqual",
            BinaryOperatorKind.GreaterThan => "GreaterThan",
            BinaryOperatorKind.GreaterThanOrEqual => "GreaterThanOrEqual",
            _ => null,
        };
    }

    // ── Unary operators ──────────────────────────────────────────────────────

    private string EmitUnary(IUnaryOperation unary)
    {
        var exprType = MapUnaryOperatorKind(unary.OperatorKind);
        if (exprType is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperator, unary.Syntax?.GetLocation() ?? Location.None, unary.OperatorKind.ToString());
            return EmitUnsupported(unary);
        }

        // Use checked variant when in a checked context
        if (unary.IsChecked && exprType == "Negate")
        {
            exprType = "NegateChecked";
        }

        var resultVar = NextVar();
        var operandVar = EmitOperation(unary.Operand);
        var typeFqn = unary.Type?.ToDisplayString(_fqnFormat) ?? "object";

        if (unary.OperatorMethod is not null)
        {
            var methodField = _fieldCache.EnsureMethodInfo(unary.OperatorMethod);
            AppendLine($"var {resultVar} = {Expr}.MakeUnary(global::System.Linq.Expressions.ExpressionType.{exprType}, {operandVar}, typeof({typeFqn}), {methodField});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.MakeUnary(global::System.Linq.Expressions.ExpressionType.{exprType}, {operandVar}, typeof({typeFqn}));");
        }

        return resultVar;
    }

    private static string? MapUnaryOperatorKind(UnaryOperatorKind kind)
    {
        return kind switch
        {
            UnaryOperatorKind.BitwiseNegation => "OnesComplement",
            UnaryOperatorKind.Not => "Not",
            UnaryOperatorKind.Plus => "UnaryPlus",
            UnaryOperatorKind.Minus => "Negate",
            _ => null,
        };
    }

    // ── Type conversions ─────────────────────────────────────────────────────

    private string EmitConversion(IConversionOperation conversion)
    {
        if (conversion.Conversion.IsIdentity)
            return EmitOperation(conversion.Operand);

        var resultVar = NextVar();
        var operandVar = EmitOperation(conversion.Operand);
        var targetTypeFqn = conversion.Type?.ToDisplayString(_fqnFormat) ?? "object";

        var convertMethod = conversion.IsChecked ? "ConvertChecked" : "Convert";

        if (conversion.Conversion.MethodSymbol is not null)
        {
            var methodField = _fieldCache.EnsureMethodInfo(conversion.Conversion.MethodSymbol);
            AppendLine($"var {resultVar} = {Expr}.{convertMethod}({operandVar}, typeof({targetTypeFqn}), {methodField});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.{convertMethod}({operandVar}, typeof({targetTypeFqn}));");
        }

        return resultVar;
    }

    // ── Conditional (ternary) ────────────────────────────────────────────────

    private string EmitConditional(IConditionalOperation conditional)
    {
        var resultVar = NextVar();
        var testVar = EmitOperation(conditional.Condition);
        var ifTrueVar = EmitOperation(conditional.WhenTrue);

        // Determine the result type. For statement-form if/else (not ternary),
        // conditional.Type is null or void. Infer from the branch return types.
        var condType = conditional.Type;
        if (condType is null || condType.SpecialType == SpecialType.System_Void
            || condType.SpecialType == SpecialType.System_Object)
        {
            condType = InferBranchType(conditional.WhenTrue)
                ?? InferBranchType(conditional.WhenFalse)
                ?? condType;
        }
        var typeFqn = condType?.ToDisplayString(_fqnFormat) ?? "object";

        if (conditional.WhenFalse is not null)
        {
            var ifFalseVar = EmitOperation(conditional.WhenFalse);
            AppendLine($"var {resultVar} = {Expr}.Condition({testVar}, {ifTrueVar}, {ifFalseVar}, typeof({typeFqn}));");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Condition({testVar}, {ifTrueVar}, {Expr}.Default(typeof({typeFqn})), typeof({typeFqn}));");
        }

        return resultVar;
    }

    // ── Object creation ──────────────────────────────────────────────────────

    private string EmitObjectCreation(IObjectCreationOperation creation)
    {
        var resultVar = NextVar();

        var argVars = new List<string>();
        foreach (var arg in creation.Arguments)
        {
            argVars.Add(EmitOperation(arg.Value));
        }

        if (creation.Constructor is not null)
        {
            var ctorField = _fieldCache.EnsureConstructorInfo(creation.Constructor);

            if (creation.Initializer is not null && creation.Initializer.Initializers.Length > 0)
            {
                var newVar = NextVar();
                if (argVars.Count > 0)
                {
                    var argsExpr = string.Join(", ", argVars);
                    AppendLine($"var {newVar} = {Expr}.New({ctorField}, {argsExpr});");
                }
                else
                {
                    AppendLine($"var {newVar} = {Expr}.New({ctorField});");
                }

                var bindingVars = new List<string>();
                var elementInitVars = new List<string>();

                foreach (var initializer in creation.Initializer.Initializers)
                {
                    if (initializer is ISimpleAssignmentOperation assignment &&
                        assignment.Target is IMemberReferenceOperation memberRef)
                    {
                        // Member binding: new T { Prop = value }
                        var valueVar = EmitOperation(assignment.Value);
                        var bindingVar = NextVar();

                        if (memberRef.Member is IPropertySymbol prop)
                        {
                            var propField = _fieldCache.EnsurePropertyInfo(prop);
                            AppendLine($"var {bindingVar} = {Expr}.Bind({propField}, {valueVar});");
                            bindingVars.Add(bindingVar);
                        }
                        else if (memberRef.Member is IFieldSymbol field)
                        {
                            var fieldField = _fieldCache.EnsureFieldInfo(field);
                            AppendLine($"var {bindingVar} = {Expr}.Bind({fieldField}, {valueVar});");
                            bindingVars.Add(bindingVar);
                        }
                        else
                        {
                            ReportDiagnostic(Diagnostics.UnsupportedInitializer,
                                initializer.Syntax?.GetLocation() ?? Location.None,
                                memberRef.Member.Kind.ToString());
                        }
                    }
                    else if (initializer is IInvocationOperation invocation)
                    {
                        // Collection initializer: .Add(element) call
                        var addMethodField = _fieldCache.EnsureMethodInfo(invocation.TargetMethod);
                        var elemVars = new List<string>();
                        foreach (var arg in invocation.Arguments)
                        {
                            elemVars.Add(EmitOperation(arg.Value));
                        }
                        var elemInitVar = NextVar();
                        var elemsExpr = string.Join(", ", elemVars);
                        AppendLine($"var {elemInitVar} = {Expr}.ElementInit({addMethodField}, {elemsExpr});");
                        elementInitVars.Add(elemInitVar);
                    }
                    else
                    {
                        ReportDiagnostic(Diagnostics.UnsupportedInitializer,
                            initializer.Syntax?.GetLocation() ?? Location.None,
                            initializer.GetType().Name);
                    }
                }

                if (elementInitVars.Count > 0)
                {
                    // Collection initializer: Expression.ListInit(new, elementInits)
                    var elementsExpr = string.Join(", ", elementInitVars);
                    AppendLine($"var {resultVar} = {Expr}.ListInit({newVar}, {elementsExpr});");
                }
                else
                {
                    // Member initializer: Expression.MemberInit(new, bindings)
                    var bindingsExpr = string.Join(", ", bindingVars);
                    AppendLine($"var {resultVar} = {Expr}.MemberInit({newVar}, {bindingsExpr});");
                }
            }
            else
            {
                if (argVars.Count > 0)
                {
                    var argsExpr = string.Join(", ", argVars);
                    AppendLine($"var {resultVar} = {Expr}.New({ctorField}, {argsExpr});");
                }
                else
                {
                    AppendLine($"var {resultVar} = {Expr}.New({ctorField});");
                }
            }
        }
        else
        {
            var typeFqn = creation.Type?.ToDisplayString(_fqnFormat) ?? "object";
            AppendLine($"var {resultVar} = {Expr}.New(typeof({typeFqn}));");
        }

        return resultVar;
    }

    // ── Default & typeof ─────────────────────────────────────────────────────

    private string EmitDefault(IDefaultValueOperation defaultVal)
    {
        var resultVar = NextVar();
        var typeFqn = defaultVal.Type?.ToDisplayString(_fqnFormat) ?? "object";
        AppendLine($"var {resultVar} = {Expr}.Default(typeof({typeFqn}));");
        return resultVar;
    }

    private string EmitTypeOf(ITypeOfOperation typeOf)
    {
        var resultVar = NextVar();
        var typeFqn = typeOf.TypeOperand.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.Constant(typeof({typeFqn}), typeof(global::System.Type));");
        return resultVar;
    }

    // ── Type checking ────────────────────────────────────────────────────────

    private string EmitIsType(IIsTypeOperation isType)
    {
        var resultVar = NextVar();
        var operandVar = EmitOperation(isType.ValueOperand);
        var typeFqn = isType.TypeOperand.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.TypeIs({operandVar}, typeof({typeFqn}));");
        return resultVar;
    }

    // ── Null coalescing ──────────────────────────────────────────────────────

    private string EmitCoalesce(ICoalesceOperation coalesce)
    {
        var resultVar = NextVar();
        var leftVar = EmitOperation(coalesce.Value);
        var rightVar = EmitOperation(coalesce.WhenNull);
        AppendLine($"var {resultVar} = {Expr}.Coalesce({leftVar}, {rightVar});");
        return resultVar;
    }

    // ── Arrays ───────────────────────────────────────────────────────────────

    private string EmitArrayCreation(IArrayCreationOperation arrayCreate)
    {
        var resultVar = NextVar();
        var elementType = (arrayCreate.Type as IArrayTypeSymbol)?.ElementType;
        var elementTypeFqn = elementType?.ToDisplayString(_fqnFormat) ?? "object";

        if (arrayCreate.Initializer is not null)
        {
            var elementVars = new List<string>();
            foreach (var element in arrayCreate.Initializer.ElementValues)
            {
                elementVars.Add(EmitOperation(element));
            }

            var elementsExpr = string.Join(", ", elementVars);
            AppendLine($"var {resultVar} = {Expr}.NewArrayInit(typeof({elementTypeFqn}), {elementsExpr});");
        }
        else
        {
            var dimVars = new List<string>();
            foreach (var dim in arrayCreate.DimensionSizes)
            {
                dimVars.Add(EmitOperation(dim));
            }

            var dimsExpr = string.Join(", ", dimVars);
            AppendLine($"var {resultVar} = {Expr}.NewArrayBounds(typeof({elementTypeFqn}), {dimsExpr});");
        }

        return resultVar;
    }

    private string EmitArrayElementReference(IArrayElementReferenceOperation arrayElement)
    {
        var resultVar = NextVar();
        var arrayVar = EmitOperation(arrayElement.ArrayReference);

        var indexVars = new List<string>();
        foreach (var index in arrayElement.Indices)
        {
            indexVars.Add(EmitOperation(index));
        }

        if (indexVars.Count == 1)
        {
            AppendLine($"var {resultVar} = {Expr}.ArrayIndex({arrayVar}, {indexVars[0]});");
        }
        else
        {
            var indicesExpr = string.Join(", ", indexVars);
            AppendLine($"var {resultVar} = {Expr}.ArrayAccess({arrayVar}, {indicesExpr});");
        }

        return resultVar;
    }

    // ── Nested lambdas & delegates ───────────────────────────────────────────

    private string EmitNestedLambda(IAnonymousFunctionOperation lambda)
    {
        var lambdaSymbol = lambda.Symbol;
        var lambdaParams = lambdaSymbol.Parameters;
        var paramVarNames = new List<string>();

        foreach (var param in lambdaParams)
        {
            var paramTypeFqn = param.Type.ToDisplayString(_fqnFormat);
            var varName = $"p_{SanitizeIdentifier(param.Name)}_{_varCounter++}";
            _symbolToVar[param] = varName;
            paramVarNames.Add(varName);
            AppendLine($"var {varName} = {Expr}.Parameter(typeof({paramTypeFqn}), \"{param.Name}\");");
        }

        var bodyVar = EmitOperation(lambda.Body);

        var delegateType = BuildDelegateType(lambdaSymbol);
        var resultVar = NextVar();
        var paramsExpr = string.Join(", ", paramVarNames);
        AppendLine($"var {resultVar} = {Expr}.Lambda<{delegateType}>({bodyVar}, {paramsExpr});");

        return resultVar;
    }

    private string EmitDelegateCreation(IDelegateCreationOperation delegateCreate)
    {
        return EmitOperation(delegateCreate.Target);
    }

    // ── Tuples ───────────────────────────────────────────────────────────────

    private string EmitTuple(ITupleOperation tuple)
    {
        var tupleType = tuple.Type as INamedTypeSymbol;
        if (tupleType is null)
            return EmitUnsupported(tuple);

        return EmitTupleConstruction(tupleType, tuple.Elements);
    }

    private string EmitTupleConstruction(INamedTypeSymbol tupleType, IReadOnlyList<IOperation> elements)
    {
        // For 8+ element tuples, C# nests as ValueTuple<T1,...,T7, ValueTuple<T8,...>>.
        // The underlying type strips tuple element names.
        var underlyingType = tupleType.TupleUnderlyingType ?? tupleType;
        var typeArgs = underlyingType.TypeArguments;

        if (typeArgs.Length == 8
            && typeArgs[7] is INamedTypeSymbol restType
            && restType.OriginalDefinition.ContainingNamespace?.ToDisplayString() == "System"
            && restType.OriginalDefinition.Name == "ValueTuple")
        {
            // First 7 elements are direct, the 8th is a nested ValueTuple
            var first7 = new List<string>();
            for (var i = 0; i < 7; i++)
            {
                first7.Add(EmitOperation(elements[i]));
            }

            var restElements = new List<IOperation>();
            for (var i = 7; i < elements.Count; i++)
            {
                restElements.Add(elements[i]);
            }

            var restVar = EmitTupleConstruction(restType, restElements);
            first7.Add(restVar);

            var resultVar = NextVar();
            var ctorField = _fieldCache.EnsureConstructorInfo(underlyingType.Constructors
                .First(c => c.Parameters.Length == typeArgs.Length));
            var argsExpr = string.Join(", ", first7);
            AppendLine($"var {resultVar} = {Expr}.New({ctorField}, {argsExpr});");
            return resultVar;
        }

        // Standard case: ≤7 elements
        var elementVars = new List<string>();
        foreach (var element in elements)
        {
            elementVars.Add(EmitOperation(element));
        }

        var result = NextVar();
        var ctor = underlyingType.Constructors
            .FirstOrDefault(c => c.Parameters.Length == typeArgs.Length);

        if (ctor is not null)
        {
            var ctorFieldName = _fieldCache.EnsureConstructorInfo(ctor);
            var args = string.Join(", ", elementVars);
            AppendLine($"var {result} = {Expr}.New({ctorFieldName}, {args});");
        }
        else
        {
            var typeFqn = underlyingType.ToDisplayString(_fqnFormat);
            AppendLine($"var {result} = {Expr}.New(typeof({typeFqn}));");
        }

        return result;
    }

    // ── Pattern matching ────────────────────────────────────────────────────

    private string EmitIsPattern(IIsPatternOperation isPattern)
    {
        var operandVar = EmitOperation(isPattern.Value);
        return EmitPattern(isPattern.Pattern, operandVar, isPattern.Value.Type);
    }

    private string EmitPattern(IPatternOperation pattern, string operandVar, ITypeSymbol? operandType)
    {
        return pattern switch
        {
            IConstantPatternOperation constant => EmitConstantPattern(constant, operandVar),
            ITypePatternOperation typePattern => EmitTypePattern(typePattern, operandVar),
            IDeclarationPatternOperation declaration => EmitDeclarationPattern(declaration, operandVar),
            IRelationalPatternOperation relational => EmitRelationalPattern(relational, operandVar),
            INegatedPatternOperation negated => EmitNegatedPattern(negated, operandVar, operandType),
            IBinaryPatternOperation binaryPattern => EmitBinaryPattern(binaryPattern, operandVar, operandType),
            IDiscardPatternOperation => EmitDiscardPattern(),
            IRecursivePatternOperation recursive => EmitRecursivePattern(recursive, operandVar, operandType),
            _ => EmitUnsupported(pattern),
        };
    }

    private string EmitConstantPattern(IConstantPatternOperation constant, string operandVar)
    {
        var resultVar = NextVar();
        var valueVar = EmitOperation(constant.Value);
        AppendLine($"var {resultVar} = {Expr}.Equal({operandVar}, {valueVar});");
        return resultVar;
    }

    private string EmitTypePattern(ITypePatternOperation typePattern, string operandVar)
    {
        var resultVar = NextVar();
        var typeFqn = typePattern.NarrowedType.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.TypeIs({operandVar}, typeof({typeFqn}));");
        return resultVar;
    }

    private string EmitDeclarationPattern(IDeclarationPatternOperation declaration, string operandVar)
    {
        // Declaration patterns with named variables (e.g. `x is string s`) are not
        // representable in expression trees — pattern variables don't exist.
        // Discard designations (e.g. `x is string _`) are pure type checks.
        var resultVar = NextVar();
        var typeFqn = declaration.NarrowedType.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.TypeIs({operandVar}, typeof({typeFqn}));");
        return resultVar;
    }

    private string EmitRelationalPattern(IRelationalPatternOperation relational, string operandVar)
    {
        var resultVar = NextVar();
        var valueVar = EmitOperation(relational.Value);
        var exprType = relational.OperatorKind switch
        {
            BinaryOperatorKind.LessThan => "LessThan",
            BinaryOperatorKind.LessThanOrEqual => "LessThanOrEqual",
            BinaryOperatorKind.GreaterThan => "GreaterThan",
            BinaryOperatorKind.GreaterThanOrEqual => "GreaterThanOrEqual",
            _ => null,
        };

        if (exprType is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperator,
                relational.Syntax?.GetLocation() ?? Location.None,
                relational.OperatorKind.ToString());
            // Fall back to constant false (never matches) — safer than true (always matches)
            AppendLine($"var {resultVar} = {Expr}.Constant(false);");
            return resultVar;
        }

        AppendLine($"var {resultVar} = {Expr}.MakeBinary(global::System.Linq.Expressions.ExpressionType.{exprType}, {operandVar}, {valueVar});");
        return resultVar;
    }

    private string EmitNegatedPattern(INegatedPatternOperation negated, string operandVar, ITypeSymbol? operandType)
    {
        var resultVar = NextVar();
        var innerVar = EmitPattern(negated.Pattern, operandVar, operandType);
        AppendLine($"var {resultVar} = {Expr}.Not({innerVar});");
        return resultVar;
    }

    private string EmitBinaryPattern(IBinaryPatternOperation binaryPattern, string operandVar, ITypeSymbol? operandType)
    {
        var resultVar = NextVar();
        var leftVar = EmitPattern(binaryPattern.LeftPattern, operandVar, operandType);
        var rightVar = EmitPattern(binaryPattern.RightPattern, operandVar, operandType);
        var op = binaryPattern.OperatorKind == BinaryOperatorKind.And ? "AndAlso" : "OrElse";
        AppendLine($"var {resultVar} = {Expr}.{op}({leftVar}, {rightVar});");
        return resultVar;
    }

    private string EmitDiscardPattern()
    {
        var resultVar = NextVar();
        AppendLine($"var {resultVar} = {Expr}.Constant(true);");
        return resultVar;
    }

    private string EmitRecursivePattern(IRecursivePatternOperation recursive, string operandVar, ITypeSymbol? operandType)
    {
        var conditions = new List<string>();

        // Null check for reference types and Nullable<T>
        if (operandType is null
            || !operandType.IsValueType
            || operandType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            var nullCheck = NextVar();
            var nullConst = NextVar();
            AppendLine($"var {nullConst} = {Expr}.Constant(null, typeof({(operandType?.ToDisplayString(_fqnFormat) ?? "object")}));");
            AppendLine($"var {nullCheck} = {Expr}.NotEqual({operandVar}, {nullConst});");
            conditions.Add(nullCheck);
        }

        // Type guard — MatchedType is set when the pattern has an explicit type (e.g. `is SomeType { ... }`)
        string memberBase = operandVar;
        if (recursive.MatchedType is not null && !SymbolEqualityComparer.Default.Equals(recursive.InputType, recursive.NarrowedType))
        {
            var narrowedTypeFqn = recursive.NarrowedType.ToDisplayString(_fqnFormat);
            var typeCheck = NextVar();
            AppendLine($"var {typeCheck} = {Expr}.TypeIs({operandVar}, typeof({narrowedTypeFqn}));");
            conditions.Add(typeCheck);

            // Cast for member access
            memberBase = NextVar();
            AppendLine($"var {memberBase} = {Expr}.Convert({operandVar}, typeof({narrowedTypeFqn}));");
        }

        // Property sub-patterns
        foreach (var prop in recursive.PropertySubpatterns)
        {
            if (prop.Member is not IMemberReferenceOperation memberRef)
            {
                ReportDiagnostic(Diagnostics.UnresolvablePatternMember,
                    prop.Syntax?.GetLocation() ?? Location.None,
                    prop.Syntax?.ToString() ?? "unknown");
                continue;
            }

            string propAccessVar;
            ITypeSymbol? propType = null;

            if (memberRef.Member is IPropertySymbol propSymbol)
            {
                propAccessVar = NextVar();
                var propField = _fieldCache.EnsurePropertyInfo(propSymbol);
                AppendLine($"var {propAccessVar} = {Expr}.Property({memberBase}, {propField});");
                propType = propSymbol.Type;
            }
            else if (memberRef.Member is IFieldSymbol fieldSymbol)
            {
                propAccessVar = NextVar();
                var fieldField = _fieldCache.EnsureFieldInfo(fieldSymbol);
                AppendLine($"var {propAccessVar} = {Expr}.Field({memberBase}, {fieldField});");
                propType = fieldSymbol.Type;
            }
            else
            {
                ReportDiagnostic(Diagnostics.UnresolvablePatternMember,
                    memberRef.Syntax?.GetLocation() ?? Location.None,
                    memberRef.Member.Name);
                continue;
            }

            var subCondition = EmitPattern(prop.Pattern, propAccessVar, propType);
            conditions.Add(subCondition);
        }

        // Positional sub-patterns (deconstruct)
        if (recursive.DeconstructionSubpatterns.Length > 0)
        {
            var targetType = recursive.NarrowedType as INamedTypeSymbol ?? operandType as INamedTypeSymbol;
            var deconstructSymbol = recursive.DeconstructSymbol as IMethodSymbol;

            for (var i = 0; i < recursive.DeconstructionSubpatterns.Length; i++)
            {
                var subPattern = recursive.DeconstructionSubpatterns[i];
                if (subPattern is IDiscardPatternOperation)
                    continue;

                string? propName = null;
                ITypeSymbol? propType = null;

                // Try to resolve via Deconstruct parameter name → property name
                if (deconstructSymbol is not null && i < deconstructSymbol.Parameters.Length)
                {
                    var paramName = deconstructSymbol.Parameters[i].Name;
                    var propSymbol = targetType?.GetMembers()
                        .OfType<IPropertySymbol>()
                        .FirstOrDefault(p => string.Equals(p.Name, paramName, StringComparison.OrdinalIgnoreCase));
                    if (propSymbol is not null)
                    {
                        propName = propSymbol.Name;
                        propType = propSymbol.Type;
                    }
                }

                // Fallback: tuple Item1, Item2, ...
                if (propName is null && targetType is not null)
                {
                    var itemName = $"Item{i + 1}";
                    var fieldSymbol = targetType.GetMembers()
                        .OfType<IFieldSymbol>()
                        .FirstOrDefault(f => f.Name == itemName);
                    if (fieldSymbol is not null)
                    {
                        propName = fieldSymbol.Name;
                        propType = fieldSymbol.Type;
                    }
                }

                if (propName is null)
                {
                    ReportDiagnostic(Diagnostics.UnresolvablePatternMember,
                        subPattern.Syntax?.GetLocation() ?? Location.None,
                        $"positional element {i}");
                    continue;
                }

                // Access the property/field on memberBase
                var accessVar = NextVar();
                var memberSymbol = targetType?.GetMembers(propName).FirstOrDefault();
                if (memberSymbol is IPropertySymbol ps)
                {
                    var pf = _fieldCache.EnsurePropertyInfo(ps);
                    AppendLine($"var {accessVar} = {Expr}.Property({memberBase}, {pf});");
                }
                else if (memberSymbol is IFieldSymbol fs)
                {
                    var ff = _fieldCache.EnsureFieldInfo(fs);
                    AppendLine($"var {accessVar} = {Expr}.Field({memberBase}, {ff});");
                }
                else
                {
                    ReportDiagnostic(Diagnostics.UnresolvablePatternMember,
                        subPattern.Syntax?.GetLocation() ?? Location.None,
                        propName);
                    continue;
                }

                var subCondition = EmitPattern(subPattern, accessVar, propType);
                conditions.Add(subCondition);
            }
        }

        // Combine all conditions with AndAlso
        if (conditions.Count == 0)
        {
            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Constant(true);");
            return resultVar;
        }

        var combined = conditions[0];
        for (var i = 1; i < conditions.Count; i++)
        {
            var andVar = NextVar();
            AppendLine($"var {andVar} = {Expr}.AndAlso({combined}, {conditions[i]});");
            combined = andVar;
        }

        return combined;
    }

    // ── Switch expressions ──────────────────────────────────────────────────

    private string EmitSwitchExpression(ISwitchExpressionOperation switchExpr)
    {
        var governingVar = EmitOperation(switchExpr.Value);
        var typeFqn = switchExpr.Type?.ToDisplayString(_fqnFormat) ?? "object";

        // Find default arm (discard pattern) or use Expression.Default as fallback
        string? currentVar = null;
        ISwitchExpressionArmOperation? defaultArm = null;
        foreach (var arm in switchExpr.Arms)
        {
            if (arm.Pattern is IDiscardPatternOperation)
            {
                defaultArm = arm;
                break;
            }
        }

        if (defaultArm is not null)
        {
            currentVar = EmitOperation(defaultArm.Value);
        }
        else
        {
            currentVar = NextVar();
            AppendLine($"var {currentVar} = {Expr}.Default(typeof({typeFqn}));");
        }

        // Build ternary chain in reverse (skip default arm)
        var arms = switchExpr.Arms;
        for (var i = arms.Length - 1; i >= 0; i--)
        {
            var arm = arms[i];
            if (arm.Pattern is IDiscardPatternOperation)
                continue;

            var conditionVar = EmitPattern(arm.Pattern, governingVar, switchExpr.Value.Type);

            // Combine with when-clause if present
            if (arm.Guard is not null)
            {
                var guardVar = EmitOperation(arm.Guard);
                var combinedGuard = NextVar();
                AppendLine($"var {combinedGuard} = {Expr}.AndAlso({conditionVar}, {guardVar});");
                conditionVar = combinedGuard;
            }

            var armValueVar = EmitOperation(arm.Value);
            var ternaryVar = NextVar();
            AppendLine($"var {ternaryVar} = {Expr}.Condition({conditionVar}, {armValueVar}, {currentVar}, typeof({typeFqn}));");
            currentVar = ternaryVar;
        }

        return currentVar;
    }

    // ── Null-conditional access ─────────────────────────────────────────────

    private string EmitConditionalAccess(IConditionalAccessOperation condAccess)
    {
        var receiverVar = EmitOperation(condAccess.Operation);
        var receiverType = condAccess.Operation.Type;
        var typeFqn = condAccess.Type?.ToDisplayString(_fqnFormat) ?? "object";

        // For Nullable<T>, member access needs .Value
        var accessVar = receiverVar;
        if (receiverType is { IsValueType: true } &&
            receiverType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            var valueVar = NextVar();
            AppendLine($"var {valueVar} = {Expr}.Property({receiverVar}, typeof({receiverType.ToDisplayString(_fqnFormat)}).GetProperty(\"Value\"));");
            accessVar = valueVar;
        }

        // Push receiver for IConditionalAccessInstanceOperation resolution
        _conditionalAccessReceiverStack.Push((accessVar, receiverType));

        var whenNotNullVar = EmitOperation(condAccess.WhenNotNull);

        // Always emit faithful null-check ternary: receiver != null ? whenNotNull : default(T)
        // If the whenNotNull type differs from the overall type (e.g. int vs int?),
        // insert Expression.Convert to match.
        var whenNotNullType = condAccess.WhenNotNull.Type;
        var overallType = condAccess.Type;
        var needsConvert = whenNotNullType is not null && overallType is not null
            && !SymbolEqualityComparer.Default.Equals(whenNotNullType, overallType);
        if (needsConvert)
        {
            var convertedVar = NextVar();
            AppendLine($"var {convertedVar} = {Expr}.Convert({whenNotNullVar}, typeof({typeFqn}));");
            whenNotNullVar = convertedVar;
        }

        var resultVar = NextVar();
        var nullConst = NextVar();
        var receiverTypeFqn = receiverType?.ToDisplayString(_fqnFormat) ?? "object";
        AppendLine($"var {nullConst} = {Expr}.Constant(null, typeof({receiverTypeFqn}));");

        var notNullCheck = NextVar();
        AppendLine($"var {notNullCheck} = {Expr}.NotEqual({receiverVar}, {nullConst});");

        var defaultVar = NextVar();
        AppendLine($"var {defaultVar} = {Expr}.Default(typeof({typeFqn}));");

        AppendLine($"var {resultVar} = {Expr}.Condition({notNullCheck}, {whenNotNullVar}, {defaultVar}, typeof({typeFqn}));");

        return resultVar;
    }

    private string EmitConditionalAccessInstance()
    {
        if (_conditionalAccessReceiverStack.Count > 0)
        {
            var (varName, _) = _conditionalAccessReceiverStack.Pop();
            return varName;
        }

        // Should not happen if the IOperation tree is well-formed
        ReportDiagnostic(Diagnostics.UnsupportedOperation, Location.None, "ConditionalAccessInstance (empty receiver stack)");
        var resultVar = NextVar();
        AppendLine($"var {resultVar} = {Expr}.Default(typeof(object));");
        return resultVar;
    }

    // ── Block bodies ─────────────────────────────────────────────────────────

    private string EmitBlock(IBlockOperation block)
    {
        // Single return statement: just emit the return value directly (no Block needed)
        if (block.Operations.Length == 1 && block.Operations[0] is IReturnOperation singleReturn)
        {
            return EmitReturn(singleReturn);
        }

        // General case: Expression.Block(variables, statements)
        var variables = new List<string>();
        var statements = new List<string>();

        foreach (var op in block.Operations)
        {
            if (op is IVariableDeclarationGroupOperation varDeclGroup)
            {
                foreach (var declaration in varDeclGroup.Declarations)
                {
                    foreach (var declarator in declaration.Declarators)
                    {
                        var localSymbol = declarator.Symbol;
                        var localTypeFqn = localSymbol.Type.ToDisplayString(_fqnFormat);
                        var localVar = NextVar();
                        AppendLine($"var {localVar} = {Expr}.Variable(typeof({localTypeFqn}), \"{localSymbol.Name}\");");
                        _localToVar[localSymbol] = localVar;
                        variables.Add(localVar);

                        // If there's an initializer, emit assignment
                        if (declarator.Initializer is not null)
                        {
                            var initVar = EmitOperation(declarator.Initializer.Value);
                            var assignVar = NextVar();
                            AppendLine($"var {assignVar} = {Expr}.Assign({localVar}, {initVar});");
                            statements.Add(assignVar);
                        }
                    }
                }
            }
            else if (op is IReturnOperation returnOp)
            {
                // The last expression in Expression.Block becomes the block's return value
                if (returnOp.ReturnedValue is not null)
                {
                    statements.Add(EmitOperation(returnOp.ReturnedValue));
                }
            }
            else if (op is IExpressionStatementOperation exprStmt)
            {
                statements.Add(EmitOperation(exprStmt.Operation));
            }
            else
            {
                // For other statement types (if/else as statement, loops, etc.)
                statements.Add(EmitOperation(op));
            }
        }

        if (statements.Count == 0)
        {
            var empty = NextVar();
            var typeFqn = block.Type?.ToDisplayString(_fqnFormat) ?? "object";
            AppendLine($"var {empty} = {Expr}.Default(typeof({typeFqn}));");
            return empty;
        }

        // If no variables, and only one statement, just return it directly
        if (variables.Count == 0 && statements.Count == 1)
        {
            return statements[0];
        }

        var resultVar = NextVar();
        var variablesExpr = variables.Count > 0
            ? $"new global::System.Linq.Expressions.ParameterExpression[] {{ {string.Join(", ", variables)} }}"
            : "global::System.Array.Empty<global::System.Linq.Expressions.ParameterExpression>()";
        var statementsExpr = $"new global::System.Linq.Expressions.Expression[] {{ {string.Join(", ", statements)} }}";
        AppendLine($"var {resultVar} = {Expr}.Block({variablesExpr}, {statementsExpr});");
        return resultVar;
    }

    private string EmitReturn(IReturnOperation ret)
    {
        if (ret.ReturnedValue is not null)
        {
            return EmitOperation(ret.ReturnedValue);
        }

        var resultVar = NextVar();
        AppendLine($"var {resultVar} = {Expr}.Default(typeof(void));");
        return resultVar;
    }

    // ── String interpolation ────────────────────────────────────────────────

    private string EmitInterpolatedString(IInterpolatedStringOperation operation)
    {
        var partVars = new List<string>();

        foreach (var part in operation.Parts)
        {
            switch (part)
            {
                case IInterpolatedStringTextOperation text:
                {
                    var constVar = NextVar();
                    var textValue = text.Text.ConstantValue.Value?.ToString() ?? "";
                    AppendLine($"var {constVar} = {Expr}.Constant(\"{EscapeString(textValue)}\", typeof(string));");
                    partVars.Add(constVar);
                    break;
                }

                case IInterpolationOperation interp:
                {
                    if (interp.Alignment is not null)
                        return EmitUnsupported(operation);

                    var innerVar = EmitOperation(interp.Expression);
                    var innerType = interp.Expression.Type;

                    if (interp.FormatString is not null)
                    {
                        // Has format specifier: call ToString(format)
                        var formatValue = interp.FormatString.ConstantValue.Value?.ToString() ?? "";
                        var toStringMethod = FindToStringWithFormat(innerType);
                        if (toStringMethod is not null)
                        {
                            var methodField = _fieldCache.EnsureMethodInfo(toStringMethod);
                            var fmtVar = NextVar();
                            AppendLine($"var {fmtVar} = {Expr}.Constant(\"{EscapeString(formatValue)}\", typeof(string));");
                            var formattedVar = NextVar();
                            AppendLine($"var {formattedVar} = {Expr}.Call({innerVar}, {methodField}, {fmtVar});");
                            partVars.Add(formattedVar);
                        }
                        else
                        {
                            // Type doesn't have ToString(string) — format specifier is lost
                            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                                interp.FormatString.Syntax?.GetLocation() ?? Location.None,
                                $"Format specifier '{formatValue}' on type without ToString(string)");
                            partVars.Add(EmitToStringCall(innerVar, innerType));
                        }
                    }
                    else if (innerType is not null && innerType.SpecialType != SpecialType.System_String)
                    {
                        // Non-string type: call ToString()
                        partVars.Add(EmitToStringCall(innerVar, innerType));
                    }
                    else
                    {
                        // Already string-typed
                        partVars.Add(innerVar);
                    }

                    break;
                }
            }
        }

        // Reduce parts via string.Concat(string, string)
        if (partVars.Count == 0)
        {
            var emptyVar = NextVar();
            AppendLine($"var {emptyVar} = {Expr}.Constant(\"\", typeof(string));");
            return emptyVar;
        }

        if (partVars.Count == 1)
            return partVars[0];

        // Left-fold: Concat(Concat(p0, p1), p2) ...
        var concatMethod = EnsureStringConcatMethod();
        var current = partVars[0];
        for (var i = 1; i < partVars.Count; i++)
        {
            var concatVar = NextVar();
            AppendLine($"var {concatVar} = {Expr}.Call({concatMethod}, {current}, {partVars[i]});");
            current = concatVar;
        }

        return current;
    }

    private string EmitToStringCall(string innerVar, ITypeSymbol? innerType)
    {
        var toStringMethod = FindParameterlessToString(innerType);
        if (toStringMethod is not null)
        {
            var methodField = _fieldCache.EnsureMethodInfo(toStringMethod);
            var strVar = NextVar();
            AppendLine($"var {strVar} = {Expr}.Call({innerVar}, {methodField});");
            return strVar;
        }

        // Fallback: box to object and call object.ToString()
        var boxed = NextVar();
        AppendLine($"var {boxed} = {Expr}.Convert({innerVar}, typeof(object));");
        var result = NextVar();
        AppendLine($"var {result} = {Expr}.Call({boxed}, typeof(object).GetMethod(\"ToString\"));");
        return result;
    }

    private IMethodSymbol? FindParameterlessToString(ITypeSymbol? type)
    {
        if (type is null) return null;
        return type.GetMembers("ToString")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.Parameters.Length == 0 && !m.IsStatic);
    }

    private IMethodSymbol? FindToStringWithFormat(ITypeSymbol? type)
    {
        if (type is null) return null;
        return type.GetMembers("ToString")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.Parameters.Length == 1
                && m.Parameters[0].Type.SpecialType == SpecialType.System_String
                && !m.IsStatic);
    }

    private string? _concatMethodField;

    private string EnsureStringConcatMethod()
    {
        if (_concatMethodField is not null)
            return _concatMethodField;

        var stringType = _semanticModel.Compilation.GetSpecialType(SpecialType.System_String);
        var concatMethod = stringType.GetMembers("Concat")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic
                && m.Parameters.Length == 2
                && m.Parameters[0].Type.SpecialType == SpecialType.System_String
                && m.Parameters[1].Type.SpecialType == SpecialType.System_String);

        if (concatMethod is not null)
        {
            _concatMethodField = _fieldCache.EnsureMethodInfo(concatMethod);
        }
        else
        {
            // Fallback: emit inline reflection
            _concatMethodField = "_stringConcat";
            _fieldCache.GetDeclarations(); // ensure we can add to it
        }

        return _concatMethodField;
    }

    // ── Unsupported fallback ─────────────────────────────────────────────────

    private string EmitUnsupported(IOperation operation)
    {
        // Don't report diagnostics for operations handled by the legacy block statement converter
        // (loops are rewritten at the syntax level before the emitter runs)
        if (operation is not ILoopOperation)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                operation.Syntax?.GetLocation() ?? Location.None,
                operation.Kind.ToString());
        }

        var resultVar = NextVar();
        var typeFqn = operation.Type?.ToDisplayString(_fqnFormat) ?? "object";
        AppendLine($"/* Unsupported IOperation: {operation.Kind} */");
        AppendLine($"var {resultVar} = {Expr}.Default(typeof({typeFqn}));");
        return resultVar;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private string NextVar() => $"expr_{_varCounter++}";

    private void AppendLine(string line)
    {
        _lines.Add($"            {line}");
        _lineCount++;
    }

    /// <summary>
    /// Infers the effective type of a conditional branch (which may be a block with a return).
    /// </summary>
    private static ITypeSymbol? InferBranchType(IOperation? branch)
    {
        if (branch is null) return null;
        if (branch.Type is not null && branch.Type.SpecialType != SpecialType.System_Void)
            return branch.Type;

        // Block containing a single return statement
        if (branch is IBlockOperation block)
        {
            foreach (var op in block.Operations)
            {
                if (op is IReturnOperation ret && ret.ReturnedValue?.Type is { } retType)
                    return retType;
            }
        }

        // Direct return
        if (branch is IReturnOperation directRet && directRet.ReturnedValue?.Type is { } directType)
            return directType;

        return null;
    }

    private void AnnotateFirstLine(int lineIndex, string comment)
    {
        if (lineIndex >= 0 && lineIndex < _lines.Count && !_lines[lineIndex].Contains(" // "))
        {
            _lines[lineIndex] = $"{_lines[lineIndex]} // {comment}";
        }
    }

    private EmitResult BuildResult()
    {
        var capacity = 0;
        foreach (var line in _lines)
            capacity += line.Length + 1; // +1 for newline
        var sb = new StringBuilder(capacity);
        foreach (var line in _lines)
        {
            sb.AppendLine(line);
        }
        return new EmitResult(sb.ToString(), _fieldCache.GetDeclarations());
    }

    private void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, params object[] messageArgs)
    {
        _context?.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));
    }

    /// <summary>
    /// Unwraps syntax nodes that are transparent to the IOperation model.
    /// These wrappers (checked, unchecked, parenthesized, null-forgiving !)
    /// cause <c>GetOperation</c> to return null if not stripped first.
    /// </summary>
    private static SyntaxNode UnwrapTransparentSyntax(SyntaxNode node)
    {
        while (true)
        {
            switch (node)
            {
                case CheckedExpressionSyntax checkedExpr:
                    node = checkedExpr.Expression;
                    continue;
                case ParenthesizedExpressionSyntax paren:
                    node = paren.Expression;
                    continue;
                case PostfixUnaryExpressionSyntax postfix
                    when postfix.IsKind(SyntaxKind.SuppressNullableWarningExpression):
                    node = postfix.Operand;
                    continue;
                default:
                    return node;
            }
        }
    }

    private static string SanitizeIdentifier(string name)
    {
        return name.Replace("@", "_").Replace(".", "_").Replace("<", "_").Replace(">", "_");
    }

    private static string FormatConstantValue(object? value, ITypeSymbol? type)
    {
        if (value is null)
            return "null";

        return value switch
        {
            bool b => b ? "true" : "false",
            char c => $"'{EscapeChar(c)}'",
            string s => $"\"{EscapeString(s)}\"",
            float f => float.IsPositiveInfinity(f) ? "float.PositiveInfinity"
                : float.IsNegativeInfinity(f) ? "float.NegativeInfinity"
                : float.IsNaN(f) ? "float.NaN"
                : $"{f}f",
            double d => double.IsPositiveInfinity(d) ? "double.PositiveInfinity"
                : double.IsNegativeInfinity(d) ? "double.NegativeInfinity"
                : double.IsNaN(d) ? "double.NaN"
                : $"{d}d",
            decimal m => $"{m}m",
            long l => $"{l}L",
            ulong ul => $"{ul}UL",
            uint ui => $"{ui}U",
            byte b => $"(byte){b}",
            sbyte sb => $"(sbyte){sb}",
            short s => $"(short){s}",
            ushort us => $"(ushort){us}",
            int i => i.ToString(),
            _ => value.ToString() ?? "null",
        };
    }

    private static string EscapeString(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }

    private static string EscapeChar(char c)
    {
        return c switch
        {
            '\'' => "\\'",
            '\\' => "\\\\",
            '\n' => "\\n",
            '\r' => "\\r",
            '\t' => "\\t",
            '\0' => "\\0",
            _ => c.ToString(),
        };
    }

    private static string BuildDelegateType(IMethodSymbol lambdaSymbol)
    {
        var returnType = lambdaSymbol.ReturnType;
        var paramTypes = lambdaSymbol.Parameters.Select(p => p.Type.ToDisplayString(_fqnFormat)).ToList();

        if (returnType.SpecialType == SpecialType.System_Void)
        {
            if (paramTypes.Count == 0)
                return "global::System.Action";
            return $"global::System.Action<{string.Join(", ", paramTypes)}>";
        }

        paramTypes.Add(returnType.ToDisplayString(_fqnFormat));
        return $"global::System.Func<{string.Join(", ", paramTypes)}>";
    }
}
