using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using ExpressiveSharp.Generator.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace ExpressiveSharp.Generator.Emitter;

internal sealed class EmitterParameter
{
    public string Name { get; }
    public string TypeFqn { get; }
    /// <summary>Optional: matched against <see cref="IParameterReferenceOperation"/>.</summary>
    public IParameterSymbol? Symbol { get; }
    /// <summary>When true, matched against <see cref="IInstanceReferenceOperation"/>.</summary>
    public bool IsThis { get; }

    public EmitterParameter(string name, string typeFqn, IParameterSymbol? symbol = null, bool isThis = false)
    {
        Name = name;
        TypeFqn = typeFqn;
        Symbol = symbol;
        IsThis = isThis;
    }
}

internal sealed class ExpressionTreeEmitter
{
    private const string Expr = "global::System.Linq.Expressions.Expression";

    private static readonly SymbolDisplayFormat _fqnFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    private readonly SemanticModel _semanticModel;
    private readonly GeneratorOutputContext? _context;
    private readonly ReflectionFieldCache _fieldCache;
    private readonly List<string> _lines = new();
    private readonly Dictionary<IParameterSymbol, string> _symbolToVar = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<ILocalSymbol, string> _localToVar = new(SymbolEqualityComparer.Default);
    private string? _thisVarName;
    private int _varCounter;
    private int _lineCount;
    private readonly Stack<(string VarName, ITypeSymbol? Type)> _conditionalAccessReceiverStack = new();
    private readonly Dictionary<ITypeSymbol, string> _typeAliases = new(SymbolEqualityComparer.Default);
    private readonly string _varPrefix;
    private readonly string? _delegateVarName;
    /// <summary>
    /// Fully-qualified return type of the outer lambda. Used by <see cref="EmitUnsupported"/>
    /// to emit a type-compatible <c>Default</c> stub — without this, top-level unsupported ops
    /// wrap <c>Default(object)</c> in a typed lambda and crash the registry's static initializer.
    /// </summary>
    private string? _outerReturnTypeFqn;

    public ExpressionTreeEmitter(
        SemanticModel semanticModel,
        GeneratorOutputContext? context = null,
        string varPrefix = "",
        string? delegateVarName = null)
    {
        _semanticModel = semanticModel;
        _context = context;
        _fieldCache = new ReflectionFieldCache(_typeAliases);
        _varPrefix = varPrefix;
        _delegateVarName = delegateVarName;
    }

    /// <summary>
    /// Emits <paramref name="type"/> as <paramref name="alias"/>. Used for anonymous types,
    /// which cannot be named in generated C# source.
    /// </summary>
    public void RegisterTypeAlias(ITypeSymbol type, string alias)
        => _typeAliases[type] = alias;

    private string ResolveTypeFqn(ITypeSymbol type)
        => _typeAliases.TryGetValue(type, out var alias) ? alias : type.ToDisplayString(_fqnFormat);

    public EmitResult Emit(
        SyntaxNode bodySyntax,
        IReadOnlyList<EmitterParameter> parameters,
        string returnTypeFqn,
        string delegateTypeFqn,
        string? assignToVariable = null)
    {
        _outerReturnTypeFqn = returnTypeFqn;
        var paramVarNames = new List<string>();
        foreach (var param in parameters)
        {
            var varName = $"{_varPrefix}p_{SanitizeIdentifier(param.Name)}";
            paramVarNames.Add(varName);
            AppendLine($"var {varName} = {Expr}.Parameter(typeof({param.TypeFqn}), \"{param.Name}\");");

            if (param.Symbol is not null)
                _symbolToVar[param.Symbol] = varName;
            if (param.IsThis)
                _thisVarName = varName;
        }

        // Without unwrapping, GetOperation returns null on transparent syntax wrappers and the body is silently lost.
        bodySyntax = UnwrapTransparentSyntax(bodySyntax);

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

        // Insert Expression.Convert for body/return type mismatch — e.g. int → int? in Join key selectors.
        if (operation.Type is not null)
        {
            var bodyTypeFqn = ResolveTypeFqn(operation.Type);
            if (bodyTypeFqn != returnTypeFqn)
            {
                var convertVar = NextVar();
                AppendLine($"var {convertVar} = {Expr}.Convert({bodyVar}, typeof({returnTypeFqn}));");
                bodyVar = convertVar;
            }
        }

        var paramsArg = paramVarNames.Count > 0
            ? string.Join(", ", paramVarNames)
            : $"global::System.Array.Empty<global::System.Linq.Expressions.ParameterExpression>()";
        var finalStmt = assignToVariable is not null
            ? $"var {assignToVariable} = {Expr}.Lambda<{delegateTypeFqn}>({bodyVar}, {paramsArg});"
            : $"return {Expr}.Lambda<{delegateTypeFqn}>({bodyVar}, {paramsArg});";
        AppendLine(finalStmt);

        return BuildResult();
    }

    public EmitResult EmitConstructor(
        SyntaxNode bodySyntax,
        IReadOnlyList<EmitterParameter> parameters,
        string returnTypeFqn,
        string delegateTypeFqn,
        INamedTypeSymbol containingType,
        IMethodSymbol? chainedTargetCtor = null,
        IReadOnlyList<SyntaxNode>? chainedArgExpressions = null)
    {
        _outerReturnTypeFqn = returnTypeFqn;
        var paramVarNames = new List<string>();
        foreach (var param in parameters)
        {
            var varName = $"{_varPrefix}p_{SanitizeIdentifier(param.Name)}";
            paramVarNames.Add(varName);
            AppendLine($"var {varName} = {Expr}.Parameter(typeof({param.TypeFqn}), \"{param.Name}\");");

            if (param.Symbol is not null)
                _symbolToVar[param.Symbol] = varName;
        }

        var newVar = NextVar();
        if (chainedTargetCtor is { Parameters.Length: > 0 } && chainedArgExpressions is not null)
        {
            // `: this(args)` chain — invoke the target ctor so its bindings (incl. record primary-ctor params) flow through.
            var argVars = new List<string>();
            foreach (var argSyntax in chainedArgExpressions)
            {
                var argOp = _semanticModel.GetOperation(UnwrapTransparentSyntax(argSyntax));
                if (argOp is null)
                {
                    var fallback = NextVar();
                    AppendLine($"var {fallback} = {Expr}.Default(typeof(object));");
                    argVars.Add(fallback);
                }
                else
                {
                    argVars.Add(EmitOperation(argOp));
                }
            }
            var chainedCtorField = _fieldCache.EnsureConstructorInfo(chainedTargetCtor);
            var argsExpr = string.Join(", ", argVars);
            AppendLine($"var {newVar} = {Expr}.New({chainedCtorField}, {argsExpr});");
        }
        else
        {
            var parameterlessCtor = containingType.Constructors
                .FirstOrDefault(c => !c.IsStatic && c.Parameters.IsEmpty);
            var ctorField = parameterlessCtor is not null
                ? _fieldCache.EnsureConstructorInfo(parameterlessCtor)
                : null;

            if (ctorField is not null)
            {
                AppendLine($"var {newVar} = {Expr}.New({ctorField});");
            }
            else
            {
                AppendLine($"var {newVar} = {Expr}.New(typeof({returnTypeFqn}));");
            }
        }

        var propertyAssignments = new Dictionary<string, (ISymbol Symbol, string ValueVar)>();

        // Block-bodied ctors yield an IBlockOperation; expression-bodied ctors yield the body's operation directly.
        var operation = _semanticModel.GetOperation(UnwrapTransparentSyntax(bodySyntax));
        if (operation is IBlockOperation block)
        {
            ProcessConstructorStatements(block.Operations, propertyAssignments);
        }
        else if (operation is not null)
        {
            ProcessConstructorStatement(operation, propertyAssignments);
        }

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

    private void ProcessConstructorStatements(
        ImmutableArray<IOperation> operations,
        Dictionary<string, (ISymbol Symbol, string ValueVar)> assignments)
    {
        foreach (var op in operations)
        {
            if (!ProcessConstructorStatement(op, assignments))
                return;
        }
    }

    /// <summary>Returns false to halt further statements (e.g. after an early <c>return</c>).</summary>
    private bool ProcessConstructorStatement(
        IOperation op,
        Dictionary<string, (ISymbol Symbol, string ValueVar)> assignments)
    {
        switch (op)
        {
            case IExpressionStatementOperation { Operation: ISimpleAssignmentOperation assignment }:
                ProcessConstructorAssignment(assignment, assignments);
                return true;

            case IExpressionStatementOperation { Operation: IDeconstructionAssignmentOperation decon }:
                ProcessConstructorDeconstruction(decon, assignments);
                return true;

            case IDeconstructionAssignmentOperation deconRoot:
                ProcessConstructorDeconstruction(deconRoot, assignments);
                return true;

            case ISimpleAssignmentOperation bareAssignment:
                ProcessConstructorAssignment(bareAssignment, assignments);
                return true;

            case IConditionalOperation conditional:
                ProcessConstructorConditional(conditional, assignments);
                return true;

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
                            var assignVar = NextVar();
                            AppendLine($"var {assignVar} = {Expr}.Assign({localVar}, {initVar});");
                        }
                    }
                }
                return true;

            case IReturnOperation:
                return false;

            case IBlockOperation nestedBlock:
                ProcessConstructorStatements(nestedBlock.Operations, assignments);
                return true;

            default:
                return true;
        }
    }

    private void ProcessConstructorDeconstruction(
        IDeconstructionAssignmentOperation decon,
        Dictionary<string, (ISymbol Symbol, string ValueVar)> assignments)
    {
        // Walker has validated: both sides are equal-arity tuple literals; left elements are property/field refs on `this`.
        var target = UnwrapConversions(decon.Target);
        var value = UnwrapConversions(decon.Value);
        if (target is not ITupleOperation targetTuple || value is not ITupleOperation valueTuple
            || targetTuple.Elements.Length != valueTuple.Elements.Length)
        {
            return;
        }

        for (var i = 0; i < targetTuple.Elements.Length; i++)
        {
            var leftOp = UnwrapConversions(targetTuple.Elements[i]);
            var rightOp = valueTuple.Elements[i];

            ISymbol? memberSymbol = leftOp switch
            {
                IPropertyReferenceOperation p => p.Property,
                IFieldReferenceOperation f => f.Field,
                _ => null
            };
            var memberName = memberSymbol?.Name;
            if (memberSymbol is null || memberName is null) continue;

            var valueVar = EmitOperation(rightOp);
            assignments[memberName] = (memberSymbol, valueVar);
        }
    }

    private static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conv)
            operation = conv.Operand;
        return operation;
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
            ProcessConstructorConditional(elseIf, falseAssignments);
        }

        var conditionVar = EmitOperation(conditional.Condition);

        // For each property assigned in either branch, emit a ternary that picks the right value at construction time.
        var allProps = new HashSet<string>(trueAssignments.Keys.Union(falseAssignments.Keys));
        foreach (var propName in allProps)
        {
            trueAssignments.TryGetValue(propName, out var trueEntry);
            falseAssignments.TryGetValue(propName, out var falseEntry);

            var symbol = trueEntry.Symbol ?? falseEntry.Symbol;
            if (symbol is null) continue;

            var typeFqn = symbol switch
            {
                IPropertySymbol p => p.Type.ToDisplayString(_fqnFormat),
                IFieldSymbol f => f.Type.ToDisplayString(_fqnFormat),
                _ => "object"
            };

            // Missing branches fall back to the previously accumulated value (or default if none).
            var trueVal = trueEntry.ValueVar;
            if (trueVal is null)
            {
                if (assignments.TryGetValue(propName, out var prev))
                    trueVal = prev.ValueVar;
                else
                {
                    trueVal = NextVar();
                    AppendLine($"var {trueVal} = {Expr}.Default(typeof({typeFqn}));");
                }
            }

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

    private string EmitOperation(IOperation operation)
    {
        var lineCountBefore = _lineCount;

        var result = operation switch
        {
            ILiteralOperation literal => EmitLiteral(literal),
            IParameterReferenceOperation paramRef => EmitParameterReference(paramRef),
            IInstanceReferenceOperation instRef => EmitInstanceReference(instRef.Type),
            ILocalReferenceOperation localRef => EmitLocalReference(localRef),
            IPropertyReferenceOperation propRef => EmitPropertyReference(propRef),
            IFieldReferenceOperation fieldRef => EmitFieldReference(fieldRef),
            IInvocationOperation invocation => EmitInvocation(invocation),
            IBinaryOperation binary => EmitBinary(binary),
            IUnaryOperation unary => EmitUnary(unary),
            IConversionOperation conversion => EmitConversion(conversion),
            IConditionalOperation conditional => EmitConditional(conditional),
            IObjectCreationOperation creation => EmitObjectCreation(creation),
            IAnonymousObjectCreationOperation anonCreation => EmitAnonymousObjectCreation(anonCreation),
            IDefaultValueOperation defaultVal => EmitDefault(defaultVal),
            ITypeOfOperation typeOf => EmitTypeOf(typeOf),
            IParenthesizedOperation paren => EmitOperation(paren.Operand),
            IIsTypeOperation isType => EmitIsType(isType),
            ICoalesceOperation coalesce => EmitCoalesce(coalesce),
            IArrayCreationOperation arrayCreate => EmitArrayCreation(arrayCreate),
            IArrayElementReferenceOperation arrayElement => EmitArrayElementReference(arrayElement),
            IImplicitIndexerReferenceOperation implicitIdx => EmitImplicitIndexerReference(implicitIdx),
            IAnonymousFunctionOperation lambda => EmitNestedLambda(lambda),
            IDelegateCreationOperation delegateCreate => EmitDelegateCreation(delegateCreate),
            ITupleOperation tuple => EmitTuple(tuple),
            ITupleBinaryOperation tupleBinary => EmitTupleBinary(tupleBinary),
            IIsPatternOperation isPattern => EmitIsPattern(isPattern),
            ISwitchExpressionOperation switchExpr => EmitSwitchExpression(switchExpr),
            ISwitchOperation switchStmt => EmitSwitchStatement(switchStmt),
            IConditionalAccessOperation condAccess => EmitConditionalAccess(condAccess),
            IConditionalAccessInstanceOperation => EmitConditionalAccessInstance(),
            IBlockOperation block => EmitBlock(block),
            IReturnOperation ret => EmitReturn(ret),
            IInterpolatedStringOperation interp => EmitInterpolatedString(interp),
            IWithOperation withOp => EmitWith(withOp),
            IRangeOperation range => EmitRange(range),
            ICollectionExpressionOperation collExpr => EmitCollectionExpression(collExpr),
            IExpressionStatementOperation exprStmt => EmitOperation(exprStmt.Operation),
            ISimpleAssignmentOperation assign => EmitSimpleAssignment(assign),
            ICompoundAssignmentOperation compoundAssign => EmitCompoundAssignment(compoundAssign),
            IIncrementOrDecrementOperation incDec => EmitIncrementOrDecrement(incDec),
            IForEachLoopOperation forEach => EmitForEachLoop(forEach),
            IForLoopOperation forLoop => EmitForLoop(forLoop),
            IWhileLoopOperation whileLoop => EmitWhileLoop(whileLoop),
            IThrowOperation throwOp => EmitThrow(throwOp),
            _ => EmitUnsupported(operation),
        };

        if (_lineCount > lineCountBefore && operation.Syntax is not null)
        {
            var syntaxText = operation.Syntax.ToString().Replace("\r", "").Replace("\n", " ");
            if (syntaxText.Length > 60)
                syntaxText = syntaxText.Substring(0, 57) + "...";
            AnnotateFirstLine(lineCountBefore, syntaxText);
        }

        return result;
    }

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

    private string EmitParameterReference(IParameterReferenceOperation paramRef)
    {
        if (_symbolToVar.TryGetValue(paramRef.Parameter, out var varName))
            return varName;

        // Outer-scope param — pull the captured value from the delegate's closure at runtime.
        if (_delegateVarName is not null)
        {
            var resultVar = EmitCapturedVariable(paramRef.Parameter.Name, paramRef.Parameter.Type);
            _symbolToVar[paramRef.Parameter] = resultVar;
            return resultVar;
        }

        // Non-interceptor path — fresh ParameterExpression.
        var fallbackVar = NextVar();
        var typeFqn = paramRef.Parameter.Type.ToDisplayString(_fqnFormat);
        AppendLine($"var {fallbackVar} = {Expr}.Parameter(typeof({typeFqn}), \"{paramRef.Parameter.Name}\");");
        _symbolToVar[paramRef.Parameter] = fallbackVar;
        return fallbackVar;
    }

    private string EmitLocalReference(ILocalReferenceOperation localRef)
    {
        if (_localToVar.TryGetValue(localRef.Local, out var varName))
            return varName;

        // Outer-scope local — pull from the delegate's closure.
        if (_delegateVarName is not null)
        {
            var resultVar = EmitCapturedVariable(localRef.Local.Name, localRef.Local.Type);
            _localToVar[localRef.Local] = resultVar;
            return resultVar;
        }

        var fallbackVar = NextVar();
        var typeFqn = localRef.Local.Type.ToDisplayString(_fqnFormat);
        AppendLine($"var {fallbackVar} = {Expr}.Variable(typeof({typeFqn}), \"{localRef.Local.Name}\");");
        _localToVar[localRef.Local] = fallbackVar;
        return fallbackVar;
    }

    private string EmitCapturedVariable(string variableName, ITypeSymbol variableType)
    {
        var resultVar = NextVar();
        AppendLine($"var {resultVar} = {Expr}.MakeMemberAccess(" +
            $"{Expr}.Constant({_delegateVarName}.Target), " +
            $"{_delegateVarName}.Target.GetType().GetField(\"{variableName}\", " +
            "global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic));");
        return resultVar;
    }

    private string EmitInstanceReference(ITypeSymbol? instanceType)
    {
        if (_thisVarName is not null)
            return _thisVarName;

        // Captured `this` is stored under a compiler-generated field name (e.g. <>4__this), so resolve by type.
        if (_delegateVarName is not null && instanceType is not null)
        {
            _thisVarName = $"{_varPrefix}__this";
            var typeFqn = ResolveTypeFqn(instanceType);
            AppendLine($"var {_thisVarName} = __ClosureHelper.ResolveCapturedThis({_delegateVarName}, typeof({typeFqn}));");
            return _thisVarName;
        }

        _thisVarName = "p___this";
        AppendLine($"var {_thisVarName} = {Expr}.Parameter(typeof(object), \"@this\");");
        return _thisVarName;
    }

    private string EmitPropertyReference(IPropertyReferenceOperation propRef)
    {
        // Interceptor path: this.Property may be captured as a closure field directly (auto-prop backing
        // field) or via captured `this`. ResolveCapturedInstanceMember handles both.
        if (_delegateVarName is not null &&
            propRef.Instance is IInstanceReferenceOperation &&
            propRef.Property.GetMethod is { } getter)
        {
            var resultVar = NextVar();
            var propName = propRef.Property.Name;
            var enclosingTypeFqn = ResolveTypeFqn(propRef.Instance.Type!);
            AppendLine($"var {resultVar} = __ClosureHelper.ResolveCapturedInstanceMember({_delegateVarName}, typeof({enclosingTypeFqn}), \"{propName}\");");
            return resultVar;
        }

        var propResultVar = NextVar();
        var fieldName = _fieldCache.EnsurePropertyInfo(propRef.Property);

        if (propRef.Instance is not null)
        {
            var instanceVar = EmitOperation(propRef.Instance);
            AppendLine($"var {propResultVar} = {Expr}.Property({instanceVar}, {fieldName});");
        }
        else
        {
            AppendLine($"var {propResultVar} = {Expr}.Property(null, {fieldName});");
        }

        return propResultVar;
    }

    private string EmitFieldReference(IFieldReferenceOperation fieldRef)
    {
        // Interceptor path: this._field may be captured directly or via captured `this`. Helper handles both.
        if (_delegateVarName is not null && fieldRef.Instance is IInstanceReferenceOperation)
        {
            var resultVar = NextVar();
            var memberName = fieldRef.Field.Name;
            var enclosingTypeFqn = ResolveTypeFqn(fieldRef.Instance.Type!);
            AppendLine($"var {resultVar} = __ClosureHelper.ResolveCapturedInstanceMember({_delegateVarName}, typeof({enclosingTypeFqn}), \"{memberName}\");");
            return resultVar;
        }

        var fieldResultVar = NextVar();
        var fieldName = _fieldCache.EnsureFieldInfo(fieldRef.Field);

        if (fieldRef.Instance is not null)
        {
            var instanceVar = EmitOperation(fieldRef.Instance);
            AppendLine($"var {fieldResultVar} = {Expr}.Field({instanceVar}, {fieldName});");
        }
        else
        {
            AppendLine($"var {fieldResultVar} = {Expr}.Field(null, {fieldName});");
        }

        return fieldResultVar;
    }

    private bool TryEmitEnumMethodExpansion(IInvocationOperation invocation, out string resultVar)
    {
        resultVar = "";

        var method = invocation.TargetMethod;

        // Receiver could be the instance call's instance, or the first arg of an extension method.
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

        // Use unreduced method so the static call form has the correct signature for extension methods.
        var originalMethod = method.ReducedFrom ?? method;
        var methodField = _fieldCache.EnsureMethodInfo(originalMethod);

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

        var receiverTypeFqn = receiverType.ToDisplayString(_fqnFormat);

        // Hoist non-receiver argument emission so per-arm calls and the null-branch call reuse the same vars.
        var staticArgOffset = method.IsExtensionMethod ? 1 : 0;
        var sharedExtraArgVars = new List<string>();
        var argStartIndex = originalMethod.IsStatic ? staticArgOffset : 0;
        for (var i = argStartIndex; i < invocation.Arguments.Length; i++)
        {
            sharedExtraArgVars.Add(EmitOperation(invocation.Arguments[i].Value));
        }

        string BuildCall(string operandVar)
        {
            string callArgsExpr;
            if (originalMethod.IsStatic)
            {
                var allArgs = new List<string> { operandVar };
                allArgs.AddRange(sharedExtraArgVars);
                callArgsExpr = $"new global::System.Linq.Expressions.Expression[] {{ {string.Join(", ", allArgs)} }}";
                var callVar = NextVar();
                AppendLine($"var {callVar} = {Expr}.Call({methodField}, {callArgsExpr});");
                return callVar;
            }
            else
            {
                callArgsExpr = sharedExtraArgVars.Count > 0
                    ? $"new global::System.Linq.Expressions.Expression[] {{ {string.Join(", ", sharedExtraArgVars)} }}"
                    : "global::System.Array.Empty<global::System.Linq.Expressions.Expression>()";
                var callVar = NextVar();
                AppendLine($"var {callVar} = {Expr}.Call({operandVar}, {methodField}, {callArgsExpr});");
                return callVar;
            }
        }

        // Build the ternary chain in reverse so the first member ends up as the outermost (and first-tested) branch.
        var currentVar = defaultVar;
        foreach (var member in enumMembers.AsEnumerable().Reverse())
        {
            var enumValueVar = NextVar();
            AppendLine($"var {enumValueVar} = {Expr}.Constant({enumTypeFqn}.{member.Name}, typeof({enumTypeFqn}));");

            // The MethodInfo is bound on the original receiver type — for an instance method on
            // Nullable<TEnum> or an extension whose first param is Nullable<TEnum>, the per-arm
            // operand must also be Nullable<TEnum> or Expression.Call rejects the type mismatch.
            if (isNullable)
            {
                var lifted = NextVar();
                AppendLine($"var {lifted} = {Expr}.Convert({enumValueVar}, typeof({receiverTypeFqn}));");
                enumValueVar = lifted;
            }

            var callVar = BuildCall(enumValueVar);

            var condVar = NextVar();
            AppendLine($"var {condVar} = {Expr}.Equal({receiverVar}, {enumValueVar});");

            var ternaryVar = NextVar();
            AppendLine($"var {ternaryVar} = {Expr}.Condition({condVar}, {callVar}, {currentVar}, typeof({returnTypeFqn}));");
            currentVar = ternaryVar;
        }

        // For Nullable<TEnum>, wrap in: receiver == null ? null-branch-call : chain
        // Calling the method with a null Nullable<TEnum> preserves runtime semantics — Nullable<T>.ToString()
        // returns "" for null, and extensions on Nullable<TEnum> can define their own null behavior.
        if (isNullable)
        {
            var nullConst = NextVar();
            AppendLine($"var {nullConst} = {Expr}.Constant(null, typeof({receiverTypeFqn}));");

            var nullCallVar = BuildCall(nullConst);

            var nullCheck = NextVar();
            AppendLine($"var {nullCheck} = {Expr}.Equal({receiverVar}, {nullConst});");

            var wrappedVar = NextVar();
            AppendLine($"var {wrappedVar} = {Expr}.Condition({nullCheck}, {nullCallVar}, {currentVar}, typeof({returnTypeFqn}));");
            currentVar = wrappedVar;
        }

        resultVar = currentVar;
        return true;
    }

    private string EmitInvocation(IInvocationOperation invocation)
    {
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

    private string EmitBinary(IBinaryOperation binary)
    {
        var exprType = MapBinaryOperatorKind(binary.OperatorKind);
        if (exprType is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperator, binary.Syntax?.GetLocation() ?? Location.None, binary.OperatorKind.ToString());
            return EmitUnsupported(binary);
        }

        // Built-in string + uses string.Concat, not Expression.Add. User-defined operator+ returning string
        // has OperatorMethod set and falls through to MakeBinary.
        if (binary.OperatorKind == BinaryOperatorKind.Add
            && binary.OperatorMethod is null
            && binary.Type?.SpecialType == SpecialType.System_String)
            return EmitStringConcatenation(binary);

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

        // MakeBinary rejects raw enum operands for relational operators.
        if (binary.OperatorMethod is null
            && IsRelationalOperator(binary.OperatorKind)
            && TryGetEnumComparisonUnderlyingFqn(binary.LeftOperand.Type, out var underlyingFqn))
        {
            leftVar = EmitConvert(leftVar, underlyingFqn);
            rightVar = EmitConvert(rightVar, underlyingFqn);
        }

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

    private static bool IsRelationalOperator(BinaryOperatorKind kind) =>
        kind is BinaryOperatorKind.LessThan
            or BinaryOperatorKind.LessThanOrEqual
            or BinaryOperatorKind.GreaterThan
            or BinaryOperatorKind.GreaterThanOrEqual;

    private static bool TryGetEnumComparisonUnderlyingFqn(ITypeSymbol? type, out string underlyingFqn)
    {
        underlyingFqn = "";

        if (type is INamedTypeSymbol { IsGenericType: true } nullable
            && nullable.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
            && nullable.TypeArguments[0] is INamedTypeSymbol { TypeKind: TypeKind.Enum } innerEnum
            && innerEnum.EnumUnderlyingType is { } innerUnderlying)
        {
            underlyingFqn = $"global::System.Nullable<{innerUnderlying.ToDisplayString(_fqnFormat)}>";
            return true;
        }

        if (type is INamedTypeSymbol { TypeKind: TypeKind.Enum } enumType
            && enumType.EnumUnderlyingType is { } underlying)
        {
            underlyingFqn = underlying.ToDisplayString(_fqnFormat);
            return true;
        }

        return false;
    }

    private string EmitConvert(string operandVar, string typeFqn)
    {
        var convertVar = NextVar();
        AppendLine($"var {convertVar} = {Expr}.Convert({operandVar}, typeof({typeFqn}));");
        return convertVar;
    }

    private string EmitStringConcatenation(IBinaryOperation binary)
    {
        var resultVar = NextVar();
        var leftVar = EmitOperation(binary.LeftOperand);
        var rightVar = EmitOperation(binary.RightOperand);

        // Concat(string, string) vs Concat(object, object) — non-string operands (e.g. boxed via object+string) need the latter.
        var bothString = binary.LeftOperand.Type?.SpecialType == SpecialType.System_String
                      && binary.RightOperand.Type?.SpecialType == SpecialType.System_String;

        var concatMethod = bothString
            ? EnsureStringConcatMethod()
            : EnsureStringConcatObjectMethod();

        AppendLine($"var {resultVar} = {Expr}.Call({concatMethod}, {leftVar}, {rightVar});");
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

    private string EmitUnary(IUnaryOperation unary)
    {
        // ^x is the Index-from-end operator; expression trees have no native form, so we synthesize a ctor call.
        if (unary.OperatorKind == UnaryOperatorKind.Hat)
        {
            return EmitIndexFromEnd(unary);
        }

        var exprType = MapUnaryOperatorKind(unary.OperatorKind);
        if (exprType is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperator, unary.Syntax?.GetLocation() ?? Location.None, unary.OperatorKind.ToString());
            return EmitUnsupported(unary);
        }

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

    private string EmitConversion(IConversionOperation conversion)
    {
        if (conversion.Conversion.IsIdentity)
            return EmitOperation(conversion.Operand);

        // Throw expressions are void-typed and Expression.Convert(void, T) is invalid; emit a typed Throw directly.
        if (conversion.Operand is IThrowOperation throwOp && throwOp.Exception is not null)
            return EmitThrowWithType(throwOp, conversion.Type);

        // Quoted-lambda conversion (Queryable.Where etc.) has no runtime coercion operator.
        if (IsExpressionOfTDelegate(conversion.Type))
        {
            var quoteResult = NextVar();
            var quoteOperand = EmitOperation(conversion.Operand);
            AppendLine($"var {quoteResult} = {Expr}.Quote({quoteOperand});");
            return quoteResult;
        }

        // Implicit reference upcasts from a concrete reference operands
        if (conversion.IsImplicit
            && conversion.Conversion.IsReference
            && conversion.Operand.Type is { IsReferenceType: true })
        {
            return EmitOperation(conversion.Operand);
        }

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

    private string EmitConditional(IConditionalOperation conditional)
    {
        var resultVar = NextVar();
        var testVar = EmitOperation(conditional.Condition);
        var ifTrueVar = EmitOperation(conditional.WhenTrue);

        // Statement-form if/else gives null or void Type; infer from the branch return types.
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
            if (condType is null || condType.SpecialType == SpecialType.System_Void)
            {
                AppendLine($"var {resultVar} = {Expr}.IfThenElse({testVar}, {ifTrueVar}, {ifFalseVar});");
            }
            else
            {
                AppendLine($"var {resultVar} = {Expr}.Condition({testVar}, {ifTrueVar}, {ifFalseVar}, typeof({typeFqn}));");
            }
        }
        else
        {
            if (condType is null || condType.SpecialType == SpecialType.System_Void)
            {
                AppendLine($"var {resultVar} = {Expr}.IfThen({testVar}, {ifTrueVar});");
            }
            else
            {
                AppendLine($"var {resultVar} = {Expr}.Condition({testVar}, {ifTrueVar}, {Expr}.Default(typeof({typeFqn})), typeof({typeFqn}));");
            }
        }

        return resultVar;
    }

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
                    var elementsExpr = string.Join(", ", elementInitVars);
                    AppendLine($"var {resultVar} = {Expr}.ListInit({newVar}, {elementsExpr});");
                }
                else
                {
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

    private string EmitAnonymousObjectCreation(IAnonymousObjectCreationOperation creation)
    {
        var anonType = creation.Type;
        if (anonType is null || !_typeAliases.ContainsKey(anonType))
        {
            // No alias registered — anonymous type can't be named in generated source.
            return EmitUnsupported(creation);
        }

        var resultVar = NextVar();
        var typeFqn = ResolveTypeFqn(anonType);

        var valueVars = new List<string>();
        var propertyNames = new List<string>();
        foreach (var initializer in creation.Initializers)
        {
            if (initializer is ISimpleAssignmentOperation assignment)
            {
                valueVars.Add(EmitOperation(assignment.Value));
                if (assignment.Target is IPropertyReferenceOperation propRef)
                    propertyNames.Add(propRef.Property.Name);
            }
            else
            {
                valueVars.Add(EmitOperation(initializer));
            }
        }

        // Direct value-expression initializers don't surface property names; fall back to walking the type.
        if (propertyNames.Count < valueVars.Count && anonType is INamedTypeSymbol namedType)
        {
            propertyNames.Clear();
            foreach (var member in namedType.GetMembers())
            {
                if (member is IPropertySymbol prop && !prop.IsImplicitlyDeclared)
                    propertyNames.Add(prop.Name);
            }
        }

        // Inline reflection rather than ReflectionFieldCache — the anon type is referenced via a generic
        // parameter (e.g. TResult) that's only in scope at method level, not in static field initializers.
        var ctorVar = NextVar();
        AppendLine($"var {ctorVar} = typeof({typeFqn}).GetConstructors()[0];");

        var argsArray = valueVars.Count > 0
            ? $"new {Expr}[] {{ {string.Join(", ", valueVars)} }}"
            : $"global::System.Array.Empty<{Expr}>()";

        var memberExprs = propertyNames.Select(n => $"typeof({typeFqn}).GetProperty(\"{n}\")");
        var membersArray = propertyNames.Count > 0
            ? $"new global::System.Reflection.MemberInfo[] {{ {string.Join(", ", memberExprs)} }}"
            : $"global::System.Array.Empty<global::System.Reflection.MemberInfo>()";

        AppendLine($"var {resultVar} = {Expr}.New({ctorVar}, {argsArray}, {membersArray});");
        return resultVar;
    }

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

    private string EmitIsType(IIsTypeOperation isType)
    {
        var resultVar = NextVar();
        var operandVar = EmitOperation(isType.ValueOperand);
        var typeFqn = isType.TypeOperand.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.TypeIs({operandVar}, typeof({typeFqn}));");
        return resultVar;
    }

    private string EmitCoalesce(ICoalesceOperation coalesce)
    {
        var resultVar = NextVar();
        var leftVar = EmitOperation(coalesce.Value);
        var rightVar = EmitOperation(coalesce.WhenNull);
        AppendLine($"var {resultVar} = {Expr}.Coalesce({leftVar}, {rightVar});");
        return resultVar;
    }

    private string EmitThrow(IThrowOperation throwOp)
    {
        if (throwOp.Exception is null)
            return EmitUnsupported(throwOp);

        return EmitThrowWithType(throwOp, throwOp.Type);
    }

    private string EmitThrowWithType(IThrowOperation throwOp, ITypeSymbol? targetType)
    {
        var resultVar = NextVar();
        var exceptionVar = EmitOperation(throwOp.Exception!);

        if (targetType is not null && targetType.SpecialType != SpecialType.System_Void)
        {
            var typeFqn = ResolveTypeFqn(targetType);
            AppendLine($"var {resultVar} = {Expr}.Throw({exceptionVar}, typeof({typeFqn}));");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Throw({exceptionVar});");
        }

        return resultVar;
    }

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

    private string EmitNestedLambda(IAnonymousFunctionOperation lambda)
    {
        var lambdaSymbol = lambda.Symbol;
        var lambdaParams = lambdaSymbol.Parameters;
        var paramVarNames = new List<string>();

        foreach (var param in lambdaParams)
        {
            var paramTypeFqn = param.Type.ToDisplayString(_fqnFormat);
            var varName = $"{_varPrefix}p_{SanitizeIdentifier(param.Name)}_{_varCounter++}";
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

    private string EmitTuple(ITupleOperation tuple)
    {
        var tupleType = tuple.Type as INamedTypeSymbol;
        if (tupleType is null)
            return EmitUnsupported(tuple);

        return EmitTupleConstruction(tupleType, tuple.Elements);
    }

    private string EmitTupleConstruction(INamedTypeSymbol tupleType, IReadOnlyList<IOperation> elements)
    {
        // 8+ element tuples nest as ValueTuple<T1..T7, ValueTuple<T8..>>; underlying type strips element names.
        var underlyingType = tupleType.TupleUnderlyingType ?? tupleType;
        var typeArgs = underlyingType.TypeArguments;

        if (typeArgs.Length == 8
            && typeArgs[7] is INamedTypeSymbol restType
            && restType.OriginalDefinition.ContainingNamespace?.ToDisplayString() == "System"
            && restType.OriginalDefinition.Name == "ValueTuple")
        {
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

    private string EmitTupleBinary(ITupleBinaryOperation tupleBinary)
    {
        var leftVar = EmitOperation(tupleBinary.LeftOperand);
        var rightVar = EmitOperation(tupleBinary.RightOperand);

        var leftType = tupleBinary.LeftOperand.Type as INamedTypeSymbol;
        var rightType = tupleBinary.RightOperand.Type as INamedTypeSymbol;

        if (leftType is null || rightType is null)
            return EmitUnsupported(tupleBinary);

        var leftUnderlying = leftType.TupleUnderlyingType ?? leftType;
        var leftFields = leftUnderlying.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.Name.StartsWith("Item"))
            .OrderBy(f => f.Name)
            .ToList();

        var rightUnderlying = rightType.TupleUnderlyingType ?? rightType;
        var rightFields = rightUnderlying.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.Name.StartsWith("Item"))
            .OrderBy(f => f.Name)
            .ToList();

        if (leftFields.Count == 0 || leftFields.Count != rightFields.Count)
            return EmitUnsupported(tupleBinary);

        bool isEquality = tupleBinary.OperatorKind == BinaryOperatorKind.Equals;

        var comparisons = new List<string>();
        for (var i = 0; i < leftFields.Count; i++)
        {
            var leftFieldRef = _fieldCache.EnsureFieldInfo(leftFields[i]);
            var rightFieldRef = _fieldCache.EnsureFieldInfo(rightFields[i]);

            var lAccess = NextVar();
            AppendLine($"var {lAccess} = {Expr}.Field({leftVar}, {leftFieldRef});");
            var rAccess = NextVar();
            AppendLine($"var {rAccess} = {Expr}.Field({rightVar}, {rightFieldRef});");

            var cmpVar = NextVar();
            AppendLine($"var {cmpVar} = {Expr}.Equal({lAccess}, {rAccess});");
            comparisons.Add(cmpVar);
        }

        // For !=, fold with AndAlso then Not — equivalent to OrElse over NotEqual but simpler to emit.
        var resultVar = comparisons[0];
        for (var i = 1; i < comparisons.Count; i++)
        {
            var foldVar = NextVar();
            AppendLine($"var {foldVar} = {Expr}.AndAlso({resultVar}, {comparisons[i]});");
            resultVar = foldVar;
        }

        if (!isEquality)
        {
            var negVar = NextVar();
            AppendLine($"var {negVar} = {Expr}.Not({resultVar});");
            resultVar = negVar;
        }

        return resultVar;
    }

    private string EmitIsPattern(IIsPatternOperation isPattern)
    {
        var operandVar = EmitOperation(isPattern.Value);
        return EmitPattern(isPattern.Pattern, operandVar, isPattern.Value.Type);
    }

    private string EmitPattern(IPatternOperation pattern, string operandVar, ITypeSymbol? operandType)
    {
        return pattern switch
        {
            IConstantPatternOperation constant => EmitConstantPattern(constant, operandVar, operandType),
            ITypePatternOperation typePattern => EmitTypePattern(typePattern, operandVar),
            IDeclarationPatternOperation declaration => EmitDeclarationPattern(declaration, operandVar),
            IRelationalPatternOperation relational => EmitRelationalPattern(relational, operandVar, operandType),
            INegatedPatternOperation negated => EmitNegatedPattern(negated, operandVar, operandType),
            IBinaryPatternOperation binaryPattern => EmitBinaryPattern(binaryPattern, operandVar, operandType),
            IDiscardPatternOperation => EmitDiscardPattern(),
            IRecursivePatternOperation recursive => EmitRecursivePattern(recursive, operandVar, operandType),
            IListPatternOperation listPattern => EmitListPattern(listPattern, operandVar, operandType),
            _ => EmitUnsupported(pattern),
        };
    }

    private string EmitConstantPattern(IConstantPatternOperation constant, string operandVar, ITypeSymbol? operandType)
    {
        var resultVar = NextVar();
        var valueVar = EmitOperation(constant.Value);
        AlignNullability(ref operandVar, operandType, ref valueVar, constant.Value.Type);
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
        // Pattern variables don't exist in expression trees, so emit a pure TypeIs even for `x is T name`.
        var resultVar = NextVar();
        var typeFqn = declaration.NarrowedType.ToDisplayString(_fqnFormat);
        AppendLine($"var {resultVar} = {Expr}.TypeIs({operandVar}, typeof({typeFqn}));");
        return resultVar;
    }

    // Switch arm bodies can reference pattern-declared variables (`int i => i + 1`).
    // The pattern itself emits only the TypeIs/Equal/etc. test, so we bind each
    // declared local to a Convert of the governing value before the arm body emits,
    // otherwise the local reference falls through to the closure-capture path and
    // tries to read a non-existent field on __func.Target.
    private void BindPatternDeclarations(IPatternOperation pattern, string operandVar)
    {
        if (pattern is IDeclarationPatternOperation decl
            && decl.DeclaredSymbol is ILocalSymbol localSym)
        {
            var convertVar = NextVar();
            var typeFqn = decl.NarrowedType.ToDisplayString(_fqnFormat);
            AppendLine($"var {convertVar} = {Expr}.Convert({operandVar}, typeof({typeFqn}));");
            _localToVar[localSym] = convertVar;
        }
    }

    private string EmitRelationalPattern(IRelationalPatternOperation relational, string operandVar, ITypeSymbol? operandType)
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
            // Fall back to constant false — never-matches is safer than always-matches when the operator is unknown.
            AppendLine($"var {resultVar} = {Expr}.Constant(false);");
            return resultVar;
        }

        if (TryGetEnumComparisonUnderlyingFqn(operandType, out var underlyingFqn))
        {
            operandVar = EmitConvert(operandVar, underlyingFqn);
            valueVar = EmitConvert(valueVar, underlyingFqn);
        }
        else
        {
            AlignNullability(ref operandVar, operandType, ref valueVar, relational.Value.Type);
        }

        AppendLine($"var {resultVar} = {Expr}.MakeBinary(global::System.Linq.Expressions.ExpressionType.{exprType}, {operandVar}, {valueVar});");
        return resultVar;
    }

    // Pattern-matching keeps the constant typed as T even when the input is Nullable<T>, so
    // Expression.MakeBinary(GreaterThan, int?, int) throws BinaryOperatorNotDefined at runtime.
    // Lift the non-nullable side to Nullable<T> so MakeBinary builds a lifted comparison whose
    // bool result matches pattern semantics (null operand → false).
    private void AlignNullability(ref string leftVar, ITypeSymbol? leftType, ref string rightVar, ITypeSymbol? rightType)
    {
        if (leftType is null || rightType is null)
            return;

        var leftUnderlying = GetNullableUnderlying(leftType);
        var rightUnderlying = GetNullableUnderlying(rightType);

        if (leftUnderlying is not null && rightUnderlying is null && SymbolEqualityComparer.Default.Equals(leftUnderlying, rightType))
        {
            rightVar = EmitConvert(rightVar, leftType.ToDisplayString(_fqnFormat));
        }
        else if (rightUnderlying is not null && leftUnderlying is null && SymbolEqualityComparer.Default.Equals(rightUnderlying, leftType))
        {
            leftVar = EmitConvert(leftVar, rightType.ToDisplayString(_fqnFormat));
        }
    }

    private static ITypeSymbol? GetNullableUnderlying(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol { IsGenericType: true } named
            && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return named.TypeArguments[0];
        }
        return null;
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

    private string EmitListPattern(IListPatternOperation listPattern, string operandVar, ITypeSymbol? operandType)
    {
        var conditions = new List<string>();

        var arrayType = operandType as IArrayTypeSymbol;
        IPropertySymbol? countProp = null;
        IPropertySymbol? indexer = null;

        if (arrayType is null)
        {
            countProp = operandType?.GetMembers("Count").OfType<IPropertySymbol>().FirstOrDefault()
                ?? operandType?.GetMembers("Length").OfType<IPropertySymbol>().FirstOrDefault();

            indexer = operandType?.GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(p => p.IsIndexer && p.Parameters.Length == 1
                    && p.Parameters[0].Type.SpecialType == SpecialType.System_Int32);

            if (countProp is null || indexer is null)
            {
                ReportDiagnostic(Diagnostics.UnsupportedOperation,
                    listPattern.Syntax?.GetLocation() ?? Location.None,
                    "ListPattern (type lacks Count/Length or indexer)");
                return EmitUnsupported(listPattern);
            }
        }

        // A `..` slice means minimum-length match; otherwise exact-length.
        var hasSlice = listPattern.Patterns.Any(p => p is ISlicePatternOperation);
        var fixedPatterns = listPattern.Patterns.Where(p => p is not ISlicePatternOperation).ToList();
        var requiredCount = fixedPatterns.Count;

        var countAccess = NextVar();
        if (arrayType is not null)
        {
            AppendLine($"var {countAccess} = {Expr}.ArrayLength({operandVar});");
        }
        else
        {
            var countField = _fieldCache.EnsurePropertyInfo(countProp!);
            AppendLine($"var {countAccess} = {Expr}.Property({operandVar}, {countField});");
        }
        var countConst = NextVar();
        AppendLine($"var {countConst} = {Expr}.Constant({requiredCount});");
        var lengthCheck = NextVar();
        if (hasSlice)
        {
            AppendLine($"var {lengthCheck} = {Expr}.GreaterThanOrEqual({countAccess}, {countConst});");
        }
        else
        {
            AppendLine($"var {lengthCheck} = {Expr}.Equal({countAccess}, {countConst});");
        }
        conditions.Add(lengthCheck);

        var elementIndex = 0;
        foreach (var subPattern in listPattern.Patterns)
        {
            if (subPattern is ISlicePatternOperation)
            {
                continue;
            }

            if (subPattern is IDiscardPatternOperation)
            {
                elementIndex++;
                continue;
            }

            var idxConst = NextVar();
            AppendLine($"var {idxConst} = {Expr}.Constant({elementIndex});");

            var elementAccess = NextVar();
            ITypeSymbol elementType;
            if (arrayType is not null)
            {
                AppendLine($"var {elementAccess} = {Expr}.ArrayIndex({operandVar}, {idxConst});");
                elementType = arrayType.ElementType;
            }
            else
            {
                var indexerField = _fieldCache.EnsurePropertyInfo(indexer!);
                AppendLine($"var {elementAccess} = {Expr}.Property({operandVar}, {indexerField}, {idxConst});");
                elementType = indexer!.Type;
            }

            var subCondition = EmitPattern(subPattern, elementAccess, elementType);
            conditions.Add(subCondition);
            elementIndex++;
        }

        if (conditions.Count == 0)
        {
            var trueVar = NextVar();
            AppendLine($"var {trueVar} = {Expr}.Constant(true);");
            return trueVar;
        }

        var result = conditions[0];
        for (var i = 1; i < conditions.Count; i++)
        {
            var combined = NextVar();
            AppendLine($"var {combined} = {Expr}.AndAlso({result}, {conditions[i]});");
            result = combined;
        }

        return result;
    }

    private string EmitRecursivePattern(IRecursivePatternOperation recursive, string operandVar, ITypeSymbol? operandType)
    {
        var conditions = new List<string>();

        // Null-check anything that could be null at runtime: reference types and Nullable<T>.
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

        // MatchedType is set for `is SomeType { ... }` shapes — emit a TypeIs guard plus a cast for member access.
        string memberBase = operandVar;
        if (recursive.MatchedType is not null && !SymbolEqualityComparer.Default.Equals(recursive.InputType, recursive.NarrowedType))
        {
            var narrowedTypeFqn = recursive.NarrowedType.ToDisplayString(_fqnFormat);
            var typeCheck = NextVar();
            AppendLine($"var {typeCheck} = {Expr}.TypeIs({operandVar}, typeof({narrowedTypeFqn}));");
            conditions.Add(typeCheck);

            memberBase = NextVar();
            AppendLine($"var {memberBase} = {Expr}.Convert({operandVar}, typeof({narrowedTypeFqn}));");
        }

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

        if (recursive.DeconstructionSubpatterns.Length > 0)
        {
            var targetType = recursive.NarrowedType as INamedTypeSymbol ?? operandType as INamedTypeSymbol;
            var deconstructSymbol = recursive.DeconstructSymbol as IMethodSymbol;

            for (var i = 0; i < recursive.DeconstructionSubpatterns.Length; i++)
            {
                var subPattern = recursive.DeconstructionSubpatterns[i];
                if (subPattern is IDiscardPatternOperation)
                {
                    continue;
                }

                string? propName = null;
                ITypeSymbol? propType = null;

                // Resolve positional element either via Deconstruct parameter name → matching property, or tuple ItemN.
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

    /// <summary>
    /// Lowers a switch statement to a ternary chain. Requires single-value constant patterns and
    /// single-return case bodies; fall-through, <c>goto case</c>, and non-return bodies fall back
    /// to the default-stub behavior.
    /// </summary>
    private string EmitSwitchStatement(ISwitchOperation switchStmt)
    {
        var governingVar = EmitOperation(switchStmt.Value);

        var arms = new List<(string ConditionVar, string ValueVar, IReturnOperation Return)>();
        string? defaultValueVar = null;
        ITypeSymbol? resultType = null;

        foreach (var switchCase in switchStmt.Cases)
        {
            var returnOp = FindCaseReturn(switchCase);
            if (returnOp is null || returnOp.ReturnedValue is null)
            {
                return EmitUnsupported(switchStmt);
            }

            resultType ??= returnOp.ReturnedValue.Type;
            var armValueVar = EmitOperation(returnOp.ReturnedValue);

            var isDefault = switchCase.Clauses.Any(c => c.CaseKind == CaseKind.Default);
            if (isDefault)
            {
                defaultValueVar = armValueVar;
                continue;
            }

            // Combine all non-default clauses into a single `||` condition.
            string? orVar = null;
            foreach (var clause in switchCase.Clauses)
            {
                if (clause is not ISingleValueCaseClauseOperation single || single.Value is null)
                {
                    return EmitUnsupported(switchStmt);
                }
                var valueVar = EmitOperation(single.Value);
                var eqVar = NextVar();
                AppendLine($"var {eqVar} = {Expr}.Equal({governingVar}, {valueVar});");
                if (orVar is null)
                {
                    orVar = eqVar;
                }
                else
                {
                    var combined = NextVar();
                    AppendLine($"var {combined} = {Expr}.OrElse({orVar}, {eqVar});");
                    orVar = combined;
                }
            }
            if (orVar is null)
            {
                return EmitUnsupported(switchStmt);
            }

            arms.Add((orVar, armValueVar, returnOp));
        }

        var typeFqn = resultType is not null ? ResolveTypeFqn(resultType) : "object";

        string currentVar;
        if (defaultValueVar is not null)
        {
            currentVar = defaultValueVar;
        }
        else
        {
            currentVar = NextVar();
            AppendLine($"var {currentVar} = {Expr}.Default(typeof({typeFqn}));");
        }

        // Fold in reverse so the first matching arm ends up as the outermost (first-tested) ternary.
        for (var i = arms.Count - 1; i >= 0; i--)
        {
            var (condVar, valueVar, _) = arms[i];
            var ternaryVar = NextVar();
            AppendLine($"var {ternaryVar} = {Expr}.Condition({condVar}, {valueVar}, {currentVar}, typeof({typeFqn}));");
            currentVar = ternaryVar;
        }

        return currentVar;
    }

    /// <summary>Returns the single return operation in a case body, or null if the shape isn't supported.</summary>
    private static IReturnOperation? FindCaseReturn(ISwitchCaseOperation switchCase)
    {
        foreach (var op in switchCase.Body)
        {
            switch (op)
            {
                case IReturnOperation ret:
                    return ret;
                case IBlockOperation block:
                    foreach (var inner in block.Operations)
                    {
                        if (inner is IReturnOperation innerRet)
                        {
                            return innerRet;
                        }
                        return null;
                    }
                    return null;
                default:
                    return null;
            }
        }
        return null;
    }

    private string EmitSwitchExpression(ISwitchExpressionOperation switchExpr)
    {
        var governingVar = EmitOperation(switchExpr.Value);
        var typeFqn = switchExpr.Type?.ToDisplayString(_fqnFormat) ?? "object";

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

        // Fold in reverse so earlier arms wrap later ones; default arm is the innermost fallback.
        var arms = switchExpr.Arms;
        for (var i = arms.Length - 1; i >= 0; i--)
        {
            var arm = arms[i];
            if (arm.Pattern is IDiscardPatternOperation)
            {
                continue;
            }

            var conditionVar = EmitPattern(arm.Pattern, governingVar, switchExpr.Value.Type);

            BindPatternDeclarations(arm.Pattern, governingVar);

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

    private string EmitConditionalAccess(IConditionalAccessOperation condAccess)
    {
        var receiverVar = EmitOperation(condAccess.Operation);
        var receiverType = condAccess.Operation.Type;
        var typeFqn = condAccess.Type?.ToDisplayString(_fqnFormat) ?? "object";

        // Member access on a Nullable<T> goes through .Value — unwrap before pushing onto the receiver stack.
        var accessVar = receiverVar;
        if (receiverType is { IsValueType: true } &&
            receiverType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            var valueVar = NextVar();
            AppendLine($"var {valueVar} = {Expr}.Property({receiverVar}, typeof({receiverType.ToDisplayString(_fqnFormat)}).GetProperty(\"Value\"));");
            accessVar = valueVar;
        }

        // The stack is read by IConditionalAccessInstanceOperation when emitting WhenNotNull.
        _conditionalAccessReceiverStack.Push((accessVar, receiverType));

        var whenNotNullVar = EmitOperation(condAccess.WhenNotNull);

        // Faithful null-check ternary: receiver != null ? whenNotNull : default(T). When whenNotNull
        // differs from the result type (e.g. int vs int?), wrap it in a Convert to match.
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

        ReportDiagnostic(Diagnostics.UnsupportedOperation, Location.None, "ConditionalAccessInstance (empty receiver stack)");
        var resultVar = NextVar();
        AppendLine($"var {resultVar} = {Expr}.Default(typeof(object));");
        return resultVar;
    }

    private string EmitBlock(IBlockOperation block)
    {
        if (block.Operations.Length == 1 && block.Operations[0] is IReturnOperation singleReturn)
        {
            return EmitReturn(singleReturn);
        }

        return EmitStatementSequence(block.Operations, block.Type);
    }

    /// <summary>
    /// Lowers a statement list to a single expression. Early-return shapes are restructured into
    /// nested <c>Condition</c>s so every path yields a value (rather than appending the early
    /// return as an unrelated statement).
    /// </summary>
    private string EmitStatementSequence(IReadOnlyList<IOperation> ops, ITypeSymbol? fallbackType)
    {
        if (ops.Count == 1 && ops[0] is IReturnOperation singleReturn)
        {
            return EmitReturn(singleReturn);
        }

        var variables = new List<string>();
        var statements = new List<string>();
        EmitStatementList(ops, variables, statements);

        if (statements.Count == 0)
        {
            var empty = NextVar();
            var typeFqn = fallbackType?.ToDisplayString(_fqnFormat) ?? _outerReturnTypeFqn ?? "object";
            AppendLine($"var {empty} = {Expr}.Default(typeof({typeFqn}));");
            return empty;
        }

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

    private void EmitStatementList(IReadOnlyList<IOperation> ops, List<string> variables, List<string> statements)
    {
        for (var i = 0; i < ops.Count; i++)
        {
            var op = ops[i];

            switch (op)
            {
                case IVariableDeclarationGroupOperation varDeclGroup:
                    EmitVariableDeclarationGroup(varDeclGroup, variables, statements);
                    continue;

                case IReturnOperation returnOp:
                    // Return is the block's final value; the rest of `ops` is dead code.
                    if (returnOp.ReturnedValue is not null)
                    {
                        statements.Add(EmitOperation(returnOp.ReturnedValue));
                    }
                    return;

                case IConditionalOperation cond when TryEmitEarlyReturnConditional(cond, ops, i + 1, statements):
                    // Tail was folded into the conditional; nothing more to emit at this level.
                    return;

                case IExpressionStatementOperation exprStmt:
                    statements.Add(EmitOperation(exprStmt.Operation));
                    continue;

                default:
                    statements.Add(EmitOperation(op));
                    continue;
            }
        }
    }

    private void EmitVariableDeclarationGroup(IVariableDeclarationGroupOperation varDeclGroup, List<string> variables, List<string> statements)
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

    /// <summary>
    /// Restructures `if (...) return X;` shapes into a nested <c>Condition</c>. Returning branches
    /// become Condition arms; non-returning branches are merged with the tail so every path yields
    /// a value. Returns true when the tail has been consumed.
    /// </summary>
    private bool TryEmitEarlyReturnConditional(IConditionalOperation cond, IReadOnlyList<IOperation> ops, int tailStart, List<string> statements)
    {
        var trueReturns = AlwaysReturns(cond.WhenTrue);
        var falseReturns = cond.WhenFalse is not null && AlwaysReturns(cond.WhenFalse);

        if (!trueReturns && !falseReturns)
        {
            return false;
        }

        var resultType =
            InferBranchType(cond.WhenTrue)
            ?? (cond.WhenFalse is not null ? InferBranchType(cond.WhenFalse) : null);
        var typeFqn = resultType?.ToDisplayString(_fqnFormat) ?? _outerReturnTypeFqn ?? "object";

        // Allocate result var before the test so snapshot numbering stays stable vs. EmitConditional.
        var resultVar = NextVar();
        var testVar = EmitOperation(cond.Condition);

        var trueVar = trueReturns
            ? EmitOperation(cond.WhenTrue)
            : EmitMergedBranch(cond.WhenTrue, ops, tailStart, resultType);

        string falseVar;
        if (cond.WhenFalse is not null)
        {
            falseVar = falseReturns
                ? EmitOperation(cond.WhenFalse)
                : EmitMergedBranch(cond.WhenFalse, ops, tailStart, resultType);
        }
        else
        {
            falseVar = EmitTail(ops, tailStart, resultType);
        }

        AppendLine($"var {resultVar} = {Expr}.Condition({testVar}, {trueVar}, {falseVar}, typeof({typeFqn}));");
        statements.Add(resultVar);
        return true;
    }

    private string EmitMergedBranch(IOperation branch, IReadOnlyList<IOperation> parentOps, int tailStart, ITypeSymbol? expectedType)
    {
        var merged = new List<IOperation>();
        if (branch is IBlockOperation branchBlock)
        {
            foreach (var inner in branchBlock.Operations)
            {
                merged.Add(inner);
            }
        }
        else
        {
            merged.Add(branch);
        }

        for (var i = tailStart; i < parentOps.Count; i++)
        {
            merged.Add(parentOps[i]);
        }

        return EmitStatementSequence(merged, expectedType);
    }

    private string EmitTail(IReadOnlyList<IOperation> parentOps, int tailStart, ITypeSymbol? expectedType)
    {
        var tail = new List<IOperation>();
        for (var i = tailStart; i < parentOps.Count; i++)
        {
            tail.Add(parentOps[i]);
        }
        return EmitStatementSequence(tail, expectedType);
    }

    /// <summary>True when every path through <paramref name="op"/> ends in a <c>return</c>.</summary>
    private static bool AlwaysReturns(IOperation? op)
    {
        if (op is null)
        {
            return false;
        }

        return op switch
        {
            IReturnOperation => true,
            IBlockOperation block => block.Operations.Length > 0
                && AlwaysReturns(block.Operations[block.Operations.Length - 1]),
            IConditionalOperation cond => cond.WhenFalse is not null
                && AlwaysReturns(cond.WhenTrue)
                && AlwaysReturns(cond.WhenFalse),
            _ => false,
        };
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
                    {
                        // Alignment specifiers have no expression tree equivalent — report and drop.
                        ReportDiagnostic(Diagnostics.IgnoredOperation,
                            interp.Alignment.Syntax?.GetLocation() ?? Location.None,
                            "Alignment specifier in string interpolation");
                    }

                    var innerVar = EmitOperation(interp.Expression);
                    var innerType = interp.Expression.Type;

                    if (interp.FormatString is not null)
                    {
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
                            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                                interp.FormatString.Syntax?.GetLocation() ?? Location.None,
                                $"Format specifier '{formatValue}' on type without ToString(string)");
                            partVars.Add(EmitToStringCall(innerVar, innerType));
                        }
                    }
                    else if (innerType is not null && innerType.SpecialType != SpecialType.System_String)
                    {
                        partVars.Add(EmitToStringCall(innerVar, innerType));
                    }
                    else
                    {
                        partVars.Add(innerVar);
                    }

                    break;
                }
            }
        }

        if (partVars.Count == 0)
        {
            var emptyVar = NextVar();
            AppendLine($"var {emptyVar} = {Expr}.Constant(\"\", typeof(string));");
            return emptyVar;
        }

        if (partVars.Count == 1)
        {
            return partVars[0];
        }

        if (partVars.Count == 2)
        {
            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Call({EnsureStringConcatMethod()}, {partVars[0]}, {partVars[1]});");
            return resultVar;
        }

        if (partVars.Count == 3)
        {
            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Call({EnsureStringConcat3Method()}, {partVars[0]}, {partVars[1]}, {partVars[2]});");
            return resultVar;
        }

        if (partVars.Count == 4)
        {
            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Call({EnsureStringConcat4Method()}, {partVars[0]}, {partVars[1]}, {partVars[2]}, {partVars[3]});");
            return resultVar;
        }

        // 5+ parts: emit string.Concat(string[]). FlattenConcatArrayCalls rewrites this for providers
        // like EF Core that can't translate NewArrayInit to SQL.
        {
            var arrayVar = NextVar();
            AppendLine($"var {arrayVar} = {Expr}.NewArrayInit(typeof(string), {string.Join(", ", partVars)});");
            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Call({EnsureStringConcatArrayMethod()}, {arrayVar});");
            return resultVar;
        }
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

        var boxed = NextVar();
        AppendLine($"var {boxed} = {Expr}.Convert({innerVar}, typeof(object));");
        var result = NextVar();
        AppendLine($"var {result} = {Expr}.Call({boxed}, typeof(object).GetMethod(\"ToString\"));");
        return result;
    }

    private IMethodSymbol? FindParameterlessToString(ITypeSymbol? type)
    {
        if (type is null)
        {
            return null;
        }
        return type.GetMembers("ToString")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.Parameters.Length == 0 && !m.IsStatic);
    }

    private IMethodSymbol? FindToStringWithFormat(ITypeSymbol? type)
    {
        if (type is null)
        {
            return null;
        }
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
        {
            return _concatMethodField;
        }

        var stringType = _semanticModel.Compilation.GetSpecialType(SpecialType.System_String);
        var concatMethod = stringType.GetMembers("Concat")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic
                && m.Parameters.Length == 2
                && m.Parameters[0].Type.SpecialType == SpecialType.System_String
                && m.Parameters[1].Type.SpecialType == SpecialType.System_String);

        _concatMethodField = _fieldCache.EnsureMethodInfo(concatMethod
            ?? throw new InvalidOperationException("string.Concat(string, string) not found in compilation"));
        return _concatMethodField;
    }

    private string? _concat3MethodField;

    private string EnsureStringConcat3Method()
    {
        if (_concat3MethodField is not null)
            return _concat3MethodField;

        var stringType = _semanticModel.Compilation.GetSpecialType(SpecialType.System_String);
        var concatMethod = stringType.GetMembers("Concat")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic
                && m.Parameters.Length == 3
                && m.Parameters[0].Type.SpecialType == SpecialType.System_String
                && m.Parameters[1].Type.SpecialType == SpecialType.System_String
                && m.Parameters[2].Type.SpecialType == SpecialType.System_String);

        _concat3MethodField = _fieldCache.EnsureMethodInfo(concatMethod
            ?? throw new InvalidOperationException("string.Concat(string, string, string) not found in compilation"));
        return _concat3MethodField;
    }

    private string? _concat4MethodField;

    private string EnsureStringConcat4Method()
    {
        if (_concat4MethodField is not null)
            return _concat4MethodField;

        var stringType = _semanticModel.Compilation.GetSpecialType(SpecialType.System_String);
        var concatMethod = stringType.GetMembers("Concat")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic
                && m.Parameters.Length == 4
                && m.Parameters[0].Type.SpecialType == SpecialType.System_String
                && m.Parameters[1].Type.SpecialType == SpecialType.System_String
                && m.Parameters[2].Type.SpecialType == SpecialType.System_String
                && m.Parameters[3].Type.SpecialType == SpecialType.System_String);

        _concat4MethodField = _fieldCache.EnsureMethodInfo(concatMethod
            ?? throw new InvalidOperationException("string.Concat(string, string, string, string) not found in compilation"));
        return _concat4MethodField;
    }

    private string? _concatArrayMethodField;

    private string EnsureStringConcatArrayMethod()
    {
        if (_concatArrayMethodField is not null)
            return _concatArrayMethodField;

        var stringType = _semanticModel.Compilation.GetSpecialType(SpecialType.System_String);
        var concatMethod = stringType.GetMembers("Concat")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic
                && m.Parameters.Length == 1
                && m.Parameters[0].Type is IArrayTypeSymbol arrayType
                && arrayType.ElementType.SpecialType == SpecialType.System_String);

        _concatArrayMethodField = _fieldCache.EnsureMethodInfo(concatMethod
            ?? throw new InvalidOperationException("string.Concat(string[]) not found in compilation"));
        return _concatArrayMethodField;
    }

    private string? _concatObjectMethodField;

    private string EnsureStringConcatObjectMethod()
    {
        if (_concatObjectMethodField is not null)
            return _concatObjectMethodField;

        var stringType = _semanticModel.Compilation.GetSpecialType(SpecialType.System_String);
        var concatMethod = stringType.GetMembers("Concat")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic
                && m.Parameters.Length == 2
                && m.Parameters[0].Type.SpecialType == SpecialType.System_Object
                && m.Parameters[1].Type.SpecialType == SpecialType.System_Object);

        _concatObjectMethodField = _fieldCache.EnsureMethodInfo(concatMethod
            ?? throw new InvalidOperationException("string.Concat(object, object) not found in compilation"));
        return _concatObjectMethodField;
    }

    private INamedTypeSymbol? _enumerableType;

    private string? ResolveEnumerableMethod(string methodName, int paramCount, ITypeSymbol elementType)
    {
        _enumerableType ??= _semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable");
        if (_enumerableType is null) return null;

        var methodDef = _enumerableType.GetMembers(methodName)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic && m.IsGenericMethod
                && m.TypeParameters.Length == 1 && m.Parameters.Length == paramCount);
        if (methodDef is null) return null;

        return _fieldCache.EnsureMethodInfo(methodDef.Construct(elementType));
    }

    private string EmitIndexFromEnd(IUnaryOperation unary)
    {
        var resultVar = NextVar();
        var innerVar = EmitOperation(unary.Operand);
        var trueConst = NextVar();
        AppendLine($"var {trueConst} = {Expr}.Constant(true);");
        var indexCtor = NextVar();
        AppendLine($"var {indexCtor} = typeof(global::System.Index).GetConstructor(new global::System.Type[] {{ typeof(int), typeof(bool) }});");
        AppendLine($"var {resultVar} = {Expr}.New({indexCtor}, {innerVar}, {trueConst});");
        return resultVar;
    }

    // Lowers `s[range]` on string to `s.Substring(start, length)` so the result lands
    // in expression-tree shape (the language-level Range/Index machinery doesn't survive
    // an expression tree otherwise).
    private string EmitImplicitIndexerReference(IImplicitIndexerReferenceOperation op)
    {
        if (op.Instance is null || op.Instance.Type is null)
            return EmitUnsupported(op);

        var receiverType = op.Instance.Type;
        var receiverVar = EmitOperation(op.Instance);

        if (receiverType.SpecialType == SpecialType.System_String && op.Argument is IRangeOperation range)
        {
            var lengthAccessor = NextVar();
            AppendLine($"var {lengthAccessor} = {Expr}.Property({receiverVar}, typeof(global::System.String).GetProperty(\"Length\"));");

            var startVar = EmitIndexAsInt(range.LeftOperand, lengthAccessor, defaultIsZero: true);
            var endVar = EmitIndexAsInt(range.RightOperand, lengthAccessor, defaultIsZero: false);

            var lengthVar = NextVar();
            AppendLine($"var {lengthVar} = {Expr}.Subtract({endVar}, {startVar});");

            var substringMethod = NextVar();
            AppendLine($"var {substringMethod} = typeof(global::System.String).GetMethod(\"Substring\", new global::System.Type[] {{ typeof(int), typeof(int) }});");

            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Call({receiverVar}, {substringMethod}, {startVar}, {lengthVar});");
            return resultVar;
        }

        return EmitUnsupported(op);
    }

    // Emits an int-typed expression representing the absolute offset of an Index operand.
    // `defaultIsZero=true` returns 0 when the operand is omitted (left side of `..`);
    // false returns the receiver length (right side).
    private string EmitIndexAsInt(IOperation? indexOperand, string lengthVar, bool defaultIsZero)
    {
        if (indexOperand is null)
        {
            if (defaultIsZero)
            {
                var zeroVar = NextVar();
                AppendLine($"var {zeroVar} = {Expr}.Constant(0);");
                return zeroVar;
            }
            return lengthVar;
        }

        if (indexOperand is IUnaryOperation { OperatorKind: UnaryOperatorKind.Hat } fromEnd)
        {
            var inner = EmitOperation(fromEnd.Operand);
            var resultVar = NextVar();
            AppendLine($"var {resultVar} = {Expr}.Subtract({lengthVar}, {inner});");
            return resultVar;
        }

        // Plain int (possibly wrapped in a conversion-to-Index that we can ignore).
        if (indexOperand is IConversionOperation conv && conv.Operand.Type?.SpecialType == SpecialType.System_Int32)
            return EmitOperation(conv.Operand);

        return EmitOperation(indexOperand);
    }

    private string EmitRange(IRangeOperation range)
    {
        var resultVar = NextVar();

        string startVar;
        if (range.LeftOperand is not null)
        {
            startVar = EmitOperation(range.LeftOperand);
        }
        else
        {
            startVar = NextVar();
            var zeroConst = NextVar();
            AppendLine($"var {zeroConst} = {Expr}.Constant(0);");
            var falseConst = NextVar();
            AppendLine($"var {falseConst} = {Expr}.Constant(false);");
            var startCtor = NextVar();
            AppendLine($"var {startCtor} = typeof(global::System.Index).GetConstructor(new global::System.Type[] {{ typeof(int), typeof(bool) }});");
            AppendLine($"var {startVar} = {Expr}.New({startCtor}, {zeroConst}, {falseConst});");
        }

        string endVar;
        if (range.RightOperand is not null)
        {
            endVar = EmitOperation(range.RightOperand);
        }
        else
        {
            endVar = NextVar();
            var zeroConst = NextVar();
            AppendLine($"var {zeroConst} = {Expr}.Constant(0);");
            var trueConst = NextVar();
            AppendLine($"var {trueConst} = {Expr}.Constant(true);");
            var endCtor = NextVar();
            AppendLine($"var {endCtor} = typeof(global::System.Index).GetConstructor(new global::System.Type[] {{ typeof(int), typeof(bool) }});");
            AppendLine($"var {endVar} = {Expr}.New({endCtor}, {zeroConst}, {trueConst});");
        }

        var rangeCtor = NextVar();
        AppendLine($"var {rangeCtor} = typeof(global::System.Range).GetConstructor(new global::System.Type[] {{ typeof(global::System.Index), typeof(global::System.Index) }});");
        AppendLine($"var {resultVar} = {Expr}.New({rangeCtor}, {startVar}, {endVar});");
        return resultVar;
    }

    private string EmitWith(IWithOperation withOp)
    {
        var resultVar = NextVar();
        var operandVar = EmitOperation(withOp.Operand);
        var type = withOp.Type!;
        var typeFqn = type.ToDisplayString(_fqnFormat);

        // Records expose a synthesized <Clone>$ method that returns the base type (object).
        var cloneMethod = type.GetMembers("<Clone>$")
            .OfType<IMethodSymbol>()
            .FirstOrDefault();

        if (cloneMethod is not null)
        {
            var cloneField = _fieldCache.EnsureMethodInfo(cloneMethod);
            var cloneVar = NextVar();
            AppendLine($"var {cloneVar} = {Expr}.Call({operandVar}, {cloneField});");

            // <Clone>$ returns the base type — cast back to the record's concrete type.
            var typedClone = NextVar();
            AppendLine($"var {typedClone} = {Expr}.Convert({cloneVar}, typeof({typeFqn}));");

            if (withOp.Initializer is not null)
            {
                var tempVar = NextVar();
                AppendLine($"var {tempVar} = {Expr}.Variable(typeof({typeFqn}), \"withTemp\");");
                var assignTemp = NextVar();
                AppendLine($"var {assignTemp} = {Expr}.Assign({tempVar}, {typedClone});");

                var statements = new List<string> { assignTemp };

                foreach (var init in withOp.Initializer.Initializers)
                {
                    if (init is ISimpleAssignmentOperation assignment
                        && assignment.Target is IPropertyReferenceOperation propRef)
                    {
                        var valueVar = EmitOperation(assignment.Value);
                        var propField = _fieldCache.EnsurePropertyInfo(propRef.Property);
                        var propAccess = NextVar();
                        AppendLine($"var {propAccess} = {Expr}.Property({tempVar}, {propField});");
                        var assignProp = NextVar();
                        AppendLine($"var {assignProp} = {Expr}.Assign({propAccess}, {valueVar});");
                        statements.Add(assignProp);
                    }
                }

                statements.Add(tempVar);
                AppendLine($"var {resultVar} = {Expr}.Block(new global::System.Linq.Expressions.ParameterExpression[] {{ {tempVar} }}, {string.Join(", ", statements)});");
            }
            else
            {
                AppendLine($"var {resultVar} = {typedClone};");
            }
        }
        else
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                withOp.Syntax?.GetLocation() ?? Location.None,
                "With (no Clone method found on type)");
            return EmitUnsupported(withOp);
        }

        return resultVar;
    }

    private string EmitCollectionExpression(ICollectionExpressionOperation collExpr)
    {
        var resultVar = NextVar();
        var type = collExpr.Type!;

        bool hasSpread = false;
        foreach (var element in collExpr.Elements)
        {
            if (element is ISpreadOperation)
            {
                hasSpread = true;
                break;
            }
        }

        if (hasSpread)
        {
            return EmitCollectionExpressionWithSpread(collExpr, resultVar);
        }

        var elementVars = new List<string>();
        foreach (var element in collExpr.Elements)
        {
            elementVars.Add(EmitOperation(element));
        }

        var elementsExpr = string.Join(", ", elementVars);

        if (type is IArrayTypeSymbol arrayType)
        {
            var elementTypeFqn = arrayType.ElementType.ToDisplayString(_fqnFormat);
            var arrayArgs = elementVars.Count == 0
                ? $"typeof({elementTypeFqn})"
                : $"typeof({elementTypeFqn}), {elementsExpr}";
            AppendLine($"var {resultVar} = {Expr}.NewArrayInit({arrayArgs});");
        }
        else if (type is INamedTypeSymbol namedType && namedType.IsGenericType
            && namedType.OriginalDefinition.SpecialType == SpecialType.None)
        {
            var ctor = namedType.Constructors.FirstOrDefault(c => c.Parameters.Length == 0);
            if (ctor is not null)
            {
                var ctorField = _fieldCache.EnsureConstructorInfo(ctor);
                var newVar = NextVar();
                AppendLine($"var {newVar} = {Expr}.New({ctorField});");

                var addMethod = namedType.GetMembers("Add")
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(m => m.Parameters.Length == 1);

                if (addMethod is not null && elementVars.Count > 0)
                {
                    var addField = _fieldCache.EnsureMethodInfo(addMethod);
                    var elemInitVars = new List<string>();
                    foreach (var elemVar in elementVars)
                    {
                        var eiVar = NextVar();
                        AppendLine($"var {eiVar} = {Expr}.ElementInit({addField}, {elemVar});");
                        elemInitVars.Add(eiVar);
                    }
                    AppendLine($"var {resultVar} = {Expr}.ListInit({newVar}, {string.Join(", ", elemInitVars)});");
                }
                else
                {
                    // No elements (or no Add method) — return the bare New expression.
                    AppendLine($"var {resultVar} = {newVar};");
                }
            }
            else
            {
                return EmitUnsupported(collExpr);
            }
        }
        else
        {
            return EmitUnsupported(collExpr);
        }

        return resultVar;
    }

    private string EmitCollectionExpressionWithSpread(ICollectionExpressionOperation collExpr, string resultVar)
    {
        var type = collExpr.Type!;

        ITypeSymbol elementType;
        if (type is IArrayTypeSymbol arrayType)
        {
            elementType = arrayType.ElementType;
        }
        else if (type is INamedTypeSymbol namedType && namedType.TypeArguments.Length > 0)
        {
            elementType = namedType.TypeArguments[0];
        }
        else
        {
            return EmitUnsupported(collExpr);
        }

        var elementTypeFqn = elementType.ToDisplayString(_fqnFormat);

        var materializeMethod = type is IArrayTypeSymbol ? "ToArray" : "ToList";
        var materializeField = ResolveEnumerableMethod(materializeMethod, 1, elementType);

        if (materializeField is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                collExpr.Syntax?.GetLocation() ?? Location.None,
                "Spread in collection expression (System.Linq.Enumerable not available)");
            return EmitUnsupported(collExpr);
        }

        // Group runs of literals into NewArrayInit segments; spread elements pass through as-is.
        var segments = new List<string>();
        var currentLiterals = new List<string>();

        foreach (var element in collExpr.Elements)
        {
            if (element is ISpreadOperation spread)
            {
                if (currentLiterals.Count > 0)
                {
                    var arrVar = NextVar();
                    AppendLine($"var {arrVar} = {Expr}.NewArrayInit(typeof({elementTypeFqn}), {string.Join(", ", currentLiterals)});");
                    segments.Add(arrVar);
                    currentLiterals.Clear();
                }
                segments.Add(EmitOperation(spread.Operand));
            }
            else
            {
                currentLiterals.Add(EmitOperation(element));
            }
        }

        if (currentLiterals.Count > 0)
        {
            var arrVar = NextVar();
            AppendLine($"var {arrVar} = {Expr}.NewArrayInit(typeof({elementTypeFqn}), {string.Join(", ", currentLiterals)});");
            segments.Add(arrVar);
        }

        var current = segments[0];
        if (segments.Count > 1)
        {
            var concatField = ResolveEnumerableMethod("Concat", 2, elementType);
            if (concatField is null)
            {
                ReportDiagnostic(Diagnostics.UnsupportedOperation,
                    collExpr.Syntax?.GetLocation() ?? Location.None,
                    "Spread in collection expression (System.Linq.Enumerable.Concat not available)");
                return EmitUnsupported(collExpr);
            }

            for (var i = 1; i < segments.Count; i++)
            {
                var concatVar = NextVar();
                AppendLine($"var {concatVar} = {Expr}.Call({concatField}, {current}, {segments[i]});");
                current = concatVar;
            }
        }

        AppendLine($"var {resultVar} = {Expr}.Call({materializeField}, {current});");
        return resultVar;
    }

    private string EmitSimpleAssignment(ISimpleAssignmentOperation assign)
    {
        var resultVar = NextVar();
        var targetVar = EmitOperation(assign.Target);
        var valueVar = EmitOperation(assign.Value);
        AppendLine($"var {resultVar} = {Expr}.Assign({targetVar}, {valueVar});");
        return resultVar;
    }

    private string EmitCompoundAssignment(ICompoundAssignmentOperation compoundAssign)
    {
        var resultVar = NextVar();
        var targetVar = EmitOperation(compoundAssign.Target);
        var valueVar = EmitOperation(compoundAssign.Value);

        var exprType = MapBinaryOperatorKind(compoundAssign.OperatorKind);
        if (exprType is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperator,
                compoundAssign.Syntax?.GetLocation() ?? Location.None,
                compoundAssign.OperatorKind.ToString());
            return EmitUnsupported(compoundAssign);
        }

        // String += compiles to string.Concat, not Expression.Add — same caveat as in EmitBinary.
        if (compoundAssign.OperatorKind == BinaryOperatorKind.Add
            && compoundAssign.OperatorMethod is null
            && compoundAssign.Type?.SpecialType == SpecialType.System_String)
        {
            var bothString = compoundAssign.Target.Type?.SpecialType == SpecialType.System_String
                          && compoundAssign.Value.Type?.SpecialType == SpecialType.System_String;
            var concatMethod = bothString
                ? EnsureStringConcatMethod()
                : EnsureStringConcatObjectMethod();
            var concatVar = NextVar();
            AppendLine($"var {concatVar} = {Expr}.Call({concatMethod}, {targetVar}, {valueVar});");
            AppendLine($"var {resultVar} = {Expr}.Assign({targetVar}, {concatVar});");
            return resultVar;
        }

        if (compoundAssign.IsChecked)
        {
            exprType = exprType switch
            {
                "Add" => "AddChecked",
                "Subtract" => "SubtractChecked",
                "Multiply" => "MultiplyChecked",
                _ => exprType,
            };
        }

        var binaryVar = NextVar();
        AppendLine($"var {binaryVar} = {Expr}.MakeBinary(global::System.Linq.Expressions.ExpressionType.{exprType}, {targetVar}, {valueVar});");
        AppendLine($"var {resultVar} = {Expr}.Assign({targetVar}, {binaryVar});");
        return resultVar;
    }

    private string EmitIncrementOrDecrement(IIncrementOrDecrementOperation incDec)
    {
        var resultVar = NextVar();
        var operandVar = EmitOperation(incDec.Target);
        var oneConst = NextVar();
        var typeFqn = incDec.Type?.ToDisplayString(_fqnFormat) ?? "int";
        AppendLine($"var {oneConst} = {Expr}.Constant(1, typeof({typeFqn}));");

        var isIncrement = incDec.Kind == OperationKind.Increment;
        var op = isIncrement ? "Add" : "Subtract";
        if (incDec.IsChecked)
        {
            op = isIncrement ? "AddChecked" : "SubtractChecked";
        }

        var binaryVar = NextVar();
        AppendLine($"var {binaryVar} = {Expr}.MakeBinary(global::System.Linq.Expressions.ExpressionType.{op}, {operandVar}, {oneConst});");
        if (incDec.IsPostfix && incDec.Parent is not IExpressionStatementOperation)
        {
            var tempVar = NextVar();
            AppendLine($"var {tempVar} = {Expr}.Variable(typeof({typeFqn}), \"__post\");");
            AppendLine($"var {resultVar} = {Expr}.Block(new[] {{ {tempVar} }}, {Expr}.Assign({tempVar}, {operandVar}), {Expr}.Assign({operandVar}, {binaryVar}), {tempVar});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Assign({operandVar}, {binaryVar});");
        }
        return resultVar;
    }

    private string EmitForEachLoop(IForEachLoopOperation forEach)
    {
        var resultVar = NextVar();

        var collectionVar = EmitOperation(forEach.Collection);

        var elementType = forEach.LoopControlVariable switch
        {
            IVariableDeclaratorOperation declarator => declarator.Symbol.Type,
            _ => forEach.Collection.Type is INamedTypeSymbol namedType
                ? namedType.AllInterfaces.Concat(new[] { namedType })
                    .FirstOrDefault(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                    ?.TypeArguments.FirstOrDefault()
                : null,
        };

        var elementTypeFqn = elementType?.ToDisplayString(_fqnFormat) ?? "object";

        var iterVarName = forEach.LoopControlVariable is IVariableDeclaratorOperation decl
            ? decl.Symbol.Name : "item";
        var iterVar = NextVar();
        AppendLine($"var {iterVar} = {Expr}.Variable(typeof({elementTypeFqn}), \"{iterVarName}\");");

        // Register the loop variable so its body references resolve to iterVar.
        if (forEach.LoopControlVariable is IVariableDeclaratorOperation varDecl)
        {
            _localToVar[varDecl.Symbol] = iterVar;
        }

        var collectionType = forEach.Collection.Type;
        var enumerableInterface = collectionType is INamedTypeSymbol nt
            ? nt.AllInterfaces.Concat(new[] { nt })
                .FirstOrDefault(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
            : null;

        IMethodSymbol? getEnumeratorMethod = null;
        IMethodSymbol? moveNextMethod = null;
        IPropertySymbol? currentProperty = null;

        if (enumerableInterface is not null)
        {
            getEnumeratorMethod = enumerableInterface.GetMembers("GetEnumerator")
                .OfType<IMethodSymbol>().FirstOrDefault();

            var enumeratorType = getEnumeratorMethod?.ReturnType as INamedTypeSymbol;
            if (enumeratorType is not null)
            {
                moveNextMethod = enumeratorType.AllInterfaces.Concat(new[] { enumeratorType })
                    .SelectMany(i => i.GetMembers("MoveNext").OfType<IMethodSymbol>())
                    .FirstOrDefault(m => m.Parameters.Length == 0);

                // Prefer the generic Current (T) over the non-generic IEnumerator.Current (object).
                currentProperty = enumeratorType.GetMembers("Current").OfType<IPropertySymbol>().FirstOrDefault()
                    ?? enumeratorType.AllInterfaces
                        .SelectMany(i => i.GetMembers("Current").OfType<IPropertySymbol>())
                        .OrderByDescending(p => p.Type.SpecialType != SpecialType.System_Object ? 1 : 0)
                        .FirstOrDefault();
            }
        }

        if (getEnumeratorMethod is null || moveNextMethod is null || currentProperty is null)
        {
            ReportDiagnostic(Diagnostics.UnsupportedOperation,
                forEach.Syntax?.GetLocation() ?? Location.None,
                "ForEachLoop (could not resolve enumerator pattern)");
            return EmitUnsupported(forEach);
        }

        var getEnumField = _fieldCache.EnsureMethodInfo(getEnumeratorMethod);
        var moveNextField = _fieldCache.EnsureMethodInfo(moveNextMethod);
        var currentField = _fieldCache.EnsurePropertyInfo(currentProperty);
        var enumeratorTypeFqn = getEnumeratorMethod.ReturnType.ToDisplayString(_fqnFormat);

        var enumVar = NextVar();
        AppendLine($"var {enumVar} = {Expr}.Variable(typeof({enumeratorTypeFqn}), \"enumerator\");");
        var getEnumCall = NextVar();
        AppendLine($"var {getEnumCall} = {Expr}.Call({collectionVar}, {getEnumField});");
        var assignEnum = NextVar();
        AppendLine($"var {assignEnum} = {Expr}.Assign({enumVar}, {getEnumCall});");

        var breakLabel = NextVar();
        AppendLine($"var {breakLabel} = {Expr}.Label(\"break\");");

        var getCurrent = NextVar();
        AppendLine($"var {getCurrent} = {Expr}.Property({enumVar}, {currentField});");
        var assignCurrent = NextVar();
        AppendLine($"var {assignCurrent} = {Expr}.Assign({iterVar}, {getCurrent});");

        var bodyVar = EmitOperation(forEach.Body);

        var bodyBlock = NextVar();
        AppendLine($"var {bodyBlock} = {Expr}.Block({assignCurrent}, {bodyVar});");

        var moveNextCall = NextVar();
        AppendLine($"var {moveNextCall} = {Expr}.Call({enumVar}, {moveNextField});");
        var breakExpr = NextVar();
        AppendLine($"var {breakExpr} = {Expr}.Break({breakLabel});");
        var ifThenElse = NextVar();
        AppendLine($"var {ifThenElse} = {Expr}.IfThenElse({moveNextCall}, {bodyBlock}, {breakExpr});");
        var loopExpr = NextVar();
        AppendLine($"var {loopExpr} = {Expr}.Loop({ifThenElse}, {breakLabel});");

        AppendLine($"var {resultVar} = {Expr}.Block(new global::System.Linq.Expressions.ParameterExpression[] {{ {enumVar}, {iterVar} }}, {assignEnum}, {loopExpr});");
        return resultVar;
    }

    private string EmitForLoop(IForLoopOperation forLoop)
    {
        var resultVar = NextVar();

        var initVars = new List<string>();
        var blockVariables = new List<string>();
        foreach (var beforeOp in forLoop.Before)
        {
            if (beforeOp is IVariableDeclarationGroupOperation varDeclGroup)
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
                        blockVariables.Add(localVar);

                        if (declarator.Initializer is not null)
                        {
                            var initVar = EmitOperation(declarator.Initializer.Value);
                            var assignVar = NextVar();
                            AppendLine($"var {assignVar} = {Expr}.Assign({localVar}, {initVar});");
                            initVars.Add(assignVar);
                        }
                    }
                }
            }
            else
            {
                initVars.Add(EmitOperation(beforeOp));
            }
        }

        var breakLabel = NextVar();
        AppendLine($"var {breakLabel} = {Expr}.Label(\"break\");");

        var conditionVar = forLoop.Condition is not null
            ? EmitOperation(forLoop.Condition)
            : null;

        var bodyVar = EmitOperation(forLoop.Body);

        var incrementVars = new List<string>();
        foreach (var bottomOp in forLoop.AtLoopBottom)
        {
            incrementVars.Add(EmitOperation(bottomOp));
        }

        var loopBodyParts = new List<string> { bodyVar };
        loopBodyParts.AddRange(incrementVars);
        var loopBodyBlock = NextVar();
        AppendLine($"var {loopBodyBlock} = {Expr}.Block({string.Join(", ", loopBodyParts)});");

        var breakExpr = NextVar();
        AppendLine($"var {breakExpr} = {Expr}.Break({breakLabel});");

        string loopContent;
        if (conditionVar is not null)
        {
            var ifThenElse = NextVar();
            AppendLine($"var {ifThenElse} = {Expr}.IfThenElse({conditionVar}, {loopBodyBlock}, {breakExpr});");
            loopContent = ifThenElse;
        }
        else
        {
            loopContent = loopBodyBlock;
        }

        var loopExpr = NextVar();
        AppendLine($"var {loopExpr} = {Expr}.Loop({loopContent}, {breakLabel});");

        var allStatements = new List<string>();
        allStatements.AddRange(initVars);
        allStatements.Add(loopExpr);

        if (blockVariables.Count > 0)
        {
            AppendLine($"var {resultVar} = {Expr}.Block(new global::System.Linq.Expressions.ParameterExpression[] {{ {string.Join(", ", blockVariables)} }}, {string.Join(", ", allStatements)});");
        }
        else
        {
            AppendLine($"var {resultVar} = {Expr}.Block({string.Join(", ", allStatements)});");
        }
        return resultVar;
    }

    private string EmitWhileLoop(IWhileLoopOperation whileLoop)
    {
        var resultVar = NextVar();

        var breakLabel = NextVar();
        AppendLine($"var {breakLabel} = {Expr}.Label(\"break\");");

        var conditionVar = whileLoop.Condition is not null
            ? EmitOperation(whileLoop.Condition)
            : null;
        var bodyVar = EmitOperation(whileLoop.Body);

        var breakExpr = NextVar();
        AppendLine($"var {breakExpr} = {Expr}.Break({breakLabel});");

        if (conditionVar is null)
        {
            var loopExpr = NextVar();
            AppendLine($"var {loopExpr} = {Expr}.Loop({bodyVar}, {breakLabel});");
            AppendLine($"var {resultVar} = {loopExpr};");
        }
        else if (whileLoop.ConditionIsTop)
        {
            var ifThenElse = NextVar();
            AppendLine($"var {ifThenElse} = {Expr}.IfThenElse({conditionVar}, {bodyVar}, {breakExpr});");
            var loopExpr = NextVar();
            AppendLine($"var {loopExpr} = {Expr}.Loop({ifThenElse}, {breakLabel});");
            AppendLine($"var {resultVar} = {loopExpr};");
        }
        else
        {
            // do/while: cond is checked at the bottom, so we emit Block(body, IfThen(!cond, break)) inside the Loop.
            var negatedCond = NextVar();
            AppendLine($"var {negatedCond} = {Expr}.Not({conditionVar});");
            var ifBreak = NextVar();
            AppendLine($"var {ifBreak} = {Expr}.IfThen({negatedCond}, {breakExpr});");
            var loopBody = NextVar();
            AppendLine($"var {loopBody} = {Expr}.Block({bodyVar}, {ifBreak});");
            var loopExpr = NextVar();
            AppendLine($"var {loopExpr} = {Expr}.Loop({loopBody}, {breakLabel});");
            AppendLine($"var {resultVar} = {loopExpr};");
        }

        return resultVar;
    }

    private string EmitUnsupported(IOperation operation)
    {
        ReportDiagnostic(Diagnostics.UnsupportedOperation,
            operation.Syntax?.GetLocation() ?? Location.None,
            operation.Kind.ToString());

        var resultVar = NextVar();
        // Statement-shaped ops (switch, block, return…) have null/void IOperation.Type; fall back to the
        // outer lambda's return type to avoid a Lambda<Func<T,R>>(Default(object),…) mismatch that
        // would throw at ExpressionRegistry static-init time and poison the assembly.
        var isUsableType = operation.Type is not null
            && operation.Type.SpecialType != SpecialType.System_Void;
        var typeFqn = isUsableType
            ? ResolveTypeFqn(operation.Type!)
            : (_outerReturnTypeFqn ?? "object");
        AppendLine($"/* Unsupported IOperation: {operation.Kind} */");
        AppendLine($"var {resultVar} = {Expr}.Default(typeof({typeFqn}));");
        return resultVar;
    }

    private string NextVar() => $"{_varPrefix}expr_{_varCounter++}";

    private static bool IsExpressionOfTDelegate(ITypeSymbol? type)
        => type is INamedTypeSymbol { IsGenericType: true } named
            && named.OriginalDefinition.ContainingNamespace?.ToDisplayString() == "System.Linq.Expressions"
            && named.OriginalDefinition.Name == "Expression"
            && named.TypeArguments.Length == 1;

    private void AppendLine(string line)
    {
        _lines.Add($"            {line}");
        _lineCount++;
    }

    /// <summary>Returns the effective branch type, looking through blocks-with-return.</summary>
    private static ITypeSymbol? InferBranchType(IOperation? branch)
    {
        if (branch is null)
        {
            return null;
        }
        if (branch.Type is not null && branch.Type.SpecialType != SpecialType.System_Void)
        {
            return branch.Type;
        }

        if (branch is IBlockOperation block)
        {
            foreach (var op in block.Operations)
            {
                if (op is IReturnOperation ret && ret.ReturnedValue?.Type is { } retType)
                {
                    return retType;
                }
            }
        }

        if (branch is IReturnOperation directRet && directRet.ReturnedValue?.Type is { } directType)
        {
            return directType;
        }

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
        {
            capacity += line.Length + 1;
        }
        var sb = new StringBuilder(capacity);
        foreach (var line in _lines)
        {
            sb.AppendLine(line);
        }
        return new EmitResult(sb.ToString());
    }

    private void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, params object[] messageArgs)
    {
        _context?.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));
    }

    /// <summary>
    /// Strips syntax wrappers (checked/unchecked, parens, null-forgiving <c>!</c>) that are
    /// transparent to IOperation — without this, <c>GetOperation</c> returns null on them.
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
                : f.ToString("R", CultureInfo.InvariantCulture) + "f",
            double d => double.IsPositiveInfinity(d) ? "double.PositiveInfinity"
                : double.IsNegativeInfinity(d) ? "double.NegativeInfinity"
                : double.IsNaN(d) ? "double.NaN"
                : d.ToString("R", CultureInfo.InvariantCulture) + "d",
            decimal m => m.ToString(CultureInfo.InvariantCulture) + "m",
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
