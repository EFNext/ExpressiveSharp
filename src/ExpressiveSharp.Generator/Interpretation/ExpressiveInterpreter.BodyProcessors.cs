using ExpressiveSharp.Generator.Emitter;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace ExpressiveSharp.Generator.Interpretation;

static internal partial class ExpressiveInterpreter
{
    /// <summary>
    /// Fills <paramref name="descriptor"/> from a method declaration body.
    /// Returns <c>false</c> and reports diagnostics on failure.
    /// </summary>
    private static bool TryApplyMethodBody(
        MethodDeclarationSyntax methodDeclarationSyntax,
        ISymbol memberSymbol,
        SemanticModel semanticModel,
        DeclarationSyntaxRewriter declarationSyntaxRewriter,
        SourceProductionContext context,
        ExpressiveDescriptor descriptor,
        bool allowBlockBody)
    {
        SyntaxNode bodySyntax;

        if (methodDeclarationSyntax.ExpressionBody is not null)
        {
            bodySyntax = methodDeclarationSyntax.ExpressionBody.Expression;
        }
        else if (methodDeclarationSyntax.Body is not null)
        {
            if (!allowBlockBody)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.BlockBodyRequiresOptIn,
                    methodDeclarationSyntax.Identifier.GetLocation(),
                    memberSymbol.Name));
                return false;
            }
            bodySyntax = methodDeclarationSyntax.Body;
            ValidateBlockBody(semanticModel, bodySyntax, memberSymbol.Name, context);
        }
        else
        {
            return ReportRequiresBodyAndFail(context, methodDeclarationSyntax, memberSymbol.Name);
        }

        var returnTypeSyntax = declarationSyntaxRewriter.Visit(methodDeclarationSyntax.ReturnType);
        descriptor.ReturnTypeName = returnTypeSyntax.ToString();
        ApplyParameterList(methodDeclarationSyntax.ParameterList, declarationSyntaxRewriter, descriptor);
        ApplyTypeParameters(methodDeclarationSyntax, declarationSyntaxRewriter, descriptor);

        var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
        if (methodSymbol is null)
        {
            return ReportRequiresBodyAndFail(context, methodDeclarationSyntax, memberSymbol.Name);
        }

        descriptor.ExpressionTreeEmission = EmitExpressionTree(
            bodySyntax, semanticModel, context, descriptor, methodSymbol);

        return true;
    }

    /// <summary>
    /// Fills <paramref name="descriptor"/> from a property declaration body.
    /// Returns <c>false</c> and reports diagnostics on failure.
    /// </summary>
    private static bool TryApplyPropertyBody(
        PropertyDeclarationSyntax propertyDeclarationSyntax,
        ISymbol memberSymbol,
        SemanticModel semanticModel,
        DeclarationSyntaxRewriter declarationSyntaxRewriter,
        SourceProductionContext context,
        ExpressiveDescriptor descriptor,
        bool allowBlockBody)
    {
        SyntaxNode? bodySyntax = null;
        var isBlockBody = false;

        if (propertyDeclarationSyntax.ExpressionBody is not null)
        {
            bodySyntax = propertyDeclarationSyntax.ExpressionBody.Expression;
        }
        else if (propertyDeclarationSyntax.AccessorList is not null)
        {
            var getter = propertyDeclarationSyntax.AccessorList.Accessors
                .FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));

            if (getter?.ExpressionBody is not null)
            {
                bodySyntax = getter.ExpressionBody.Expression;
            }
            else if (getter?.Body is not null)
            {
                isBlockBody = true;
                bodySyntax = getter.Body;
            }
        }

        if (isBlockBody && !allowBlockBody)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.BlockBodyRequiresOptIn,
                propertyDeclarationSyntax.Identifier.GetLocation(),
                memberSymbol.Name));
            return false;
        }

        if (isBlockBody && bodySyntax is not null)
        {
            ValidateBlockBody(semanticModel, bodySyntax, memberSymbol.Name, context);
        }

        if (bodySyntax is null)
        {
            return ReportRequiresBodyAndFail(context, propertyDeclarationSyntax, memberSymbol.Name);
        }

        var returnTypeSyntax = declarationSyntaxRewriter.Visit(propertyDeclarationSyntax.Type);
        descriptor.ReturnTypeName = returnTypeSyntax.ToString();

        descriptor.ExpressionTreeEmission = EmitExpressionTreeForProperty(
            bodySyntax, semanticModel, context, descriptor, memberSymbol);

        return true;
    }

    /// <summary>
    /// Fills <paramref name="descriptor"/> from a constructor declaration body.
    /// Constructors produce <c>Expression.MemberInit</c> (object initializer) for EF Core projections.
    /// </summary>
    private static bool TryApplyConstructorBody(
        ConstructorDeclarationSyntax constructorDeclarationSyntax,
        ISymbol memberSymbol,
        SemanticModel semanticModel,
        DeclarationSyntaxRewriter declarationSyntaxRewriter,
        SourceProductionContext context,
        Compilation? compilation,
        ExpressiveDescriptor descriptor)
    {
        if (constructorDeclarationSyntax.Body is null && constructorDeclarationSyntax.ExpressionBody is null)
        {
            return ReportRequiresBodyAndFail(context, constructorDeclarationSyntax, memberSymbol.Name);
        }

        var containingType = memberSymbol.ContainingType;
        var fullTypeName = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        descriptor.ReturnTypeName = fullTypeName;
        ApplyParameterList(constructorDeclarationSyntax.ParameterList, declarationSyntaxRewriter, descriptor);

        // Detect `: this(...)` chaining to a parameterized ctor (records' primary ctor or any
        // `this(args)` overload). In that case we emit Expression.New(targetCtor, args) so the
        // target ctor is invoked with the caller's values — the parameterless requirement is
        // then irrelevant because we never synthesize `new T()`.
        IMethodSymbol? chainedTargetCtor = null;
        List<SyntaxNode>? chainedArgExpressions = null;
        if (constructorDeclarationSyntax.Initializer is { } initializer
            && initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.ThisKeyword))
        {
            if (semanticModel.GetSymbolInfo(initializer).Symbol is IMethodSymbol target
                && target.Parameters.Length > 0)
            {
                chainedTargetCtor = target;
                chainedArgExpressions = initializer.ArgumentList.Arguments
                    .Select(a => (SyntaxNode)a.Expression)
                    .ToList();
            }
        }

        // Verify parameterless constructor exists — skip when chaining to a parameterized ctor.
        if (chainedTargetCtor is null)
        {
            var hasAccessibleParameterlessConstructor = containingType.Constructors
                .Any(c => !c.IsStatic
                          && c.Parameters.IsEmpty
                          && c.DeclaredAccessibility is Accessibility.Public
                              or Accessibility.Internal
                              or Accessibility.ProtectedOrInternal);

            if (!hasAccessibleParameterlessConstructor)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.MissingParameterlessConstructor,
                    constructorDeclarationSyntax.GetLocation(),
                    containingType.Name));
                return false;
            }
        }

        // Preserve EXP0003: report when a `: base(...)` initializer targets a ctor with no
        // available source (e.g. BCL exception base classes). The expression tree cannot
        // represent a call to such a ctor — we still emit the outer body's bindings via the
        // parameterless ctor, but users should be warned that the base-ctor side effects are
        // not captured.
        if (constructorDeclarationSyntax.Initializer is { } baseInit
            && baseInit.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseKeyword)
            && semanticModel.GetSymbolInfo(baseInit).Symbol is IMethodSymbol baseTarget
            && !baseTarget.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is ConstructorDeclarationSyntax))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.NoSourceAvailableForDelegatedConstructor,
                baseTarget.Locations.FirstOrDefault() ?? Location.None,
                baseTarget.ToDisplayString(),
                baseTarget.ContainingType?.ToDisplayString() ?? "<unknown>",
                memberSymbol.Name));
        }

        // Pass the constructor body to the emitter — it will emit the block as-is.
        // The constructor body contains property assignments (this.Prop = expr) which
        // the IOperation tree represents as ISimpleAssignmentOperation nodes.
        // We use EmitConstructorBody which wraps the result in Expression.MemberInit.
        var bodySyntax = (SyntaxNode?)constructorDeclarationSyntax.Body
            ?? constructorDeclarationSyntax.ExpressionBody?.Expression;

        if (bodySyntax is null)
        {
            return ReportRequiresBodyAndFail(context, constructorDeclarationSyntax, memberSymbol.Name);
        }

        var methodSymbol = semanticModel.GetDeclaredSymbol(constructorDeclarationSyntax) as IMethodSymbol;
        if (methodSymbol is null)
        {
            return ReportRequiresBodyAndFail(context, constructorDeclarationSyntax, memberSymbol.Name);
        }

        var emitter = new ExpressionTreeEmitter(semanticModel, context);

        // Build emitter parameters (constructor params, no @this)
        var emitterParams = new List<EmitterParameter>();
        foreach (var param in methodSymbol.Parameters)
        {
            emitterParams.Add(new EmitterParameter(
                param.Name,
                param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                symbol: param));
        }

        var allTypeArgs = emitterParams.Select(p => p.TypeFqn).ToList();
        allTypeArgs.Add(descriptor.ReturnTypeName!);
        var delegateTypeFqn = $"global::System.Func<{string.Join(", ", allTypeArgs)}>";

        descriptor.ExpressionTreeEmission = emitter.EmitConstructor(
            bodySyntax, emitterParams, descriptor.ReturnTypeName!, delegateTypeFqn,
            containingType, chainedTargetCtor, chainedArgExpressions);

        return true;
    }

    /// <summary>
    /// Shared helper: emits expression tree building code for a method body.
    /// </summary>
    private static EmitResult EmitExpressionTree(
        SyntaxNode bodyExpression,
        SemanticModel semanticModel,
        SourceProductionContext context,
        ExpressiveDescriptor descriptor,
        IMethodSymbol methodSymbol)
    {
        var emitter = new ExpressionTreeEmitter(semanticModel, context);
        var emitterParams = BuildEmitterParameters(descriptor, methodSymbol);

        var allTypeArgs = emitterParams.Select(p => p.TypeFqn).ToList();
        allTypeArgs.Add(descriptor.ReturnTypeName!);
        var delegateTypeFqn = $"global::System.Func<{string.Join(", ", allTypeArgs)}>";

        return emitter.Emit(bodyExpression, emitterParams,
            descriptor.ReturnTypeName!, delegateTypeFqn);
    }

    /// <summary>
    /// Shared helper: emits expression tree building code for a property body.
    /// Properties always have a single @this parameter.
    /// </summary>
    private static EmitResult EmitExpressionTreeForProperty(
        SyntaxNode bodyExpression,
        SemanticModel semanticModel,
        SourceProductionContext context,
        ExpressiveDescriptor descriptor,
        ISymbol memberSymbol)
    {
        var emitter = new ExpressionTreeEmitter(semanticModel, context);
        var emitterParams = new List<EmitterParameter>();

        // Properties always have the implicit @this parameter
        if (descriptor.ParametersList?.Parameters.Count > 0)
        {
            var thisParam = descriptor.ParametersList.Parameters[0];
            var thisTypeFqn = thisParam.Type?.ToString() ?? "object";
            emitterParams.Add(new EmitterParameter("@this", thisTypeFqn, isThis: true));
        }

        var allTypeArgs = emitterParams.Select(p => p.TypeFqn).ToList();
        allTypeArgs.Add(descriptor.ReturnTypeName!);
        var delegateTypeFqn = $"global::System.Func<{string.Join(", ", allTypeArgs)}>";

        return emitter.Emit(bodyExpression, emitterParams,
            descriptor.ReturnTypeName!, delegateTypeFqn);
    }

    /// <summary>
    /// Builds the list of <see cref="EmitterParameter"/> for the emitter,
    /// including the implicit @this parameter when applicable.
    /// </summary>
    private static List<EmitterParameter> BuildEmitterParameters(
        ExpressiveDescriptor descriptor,
        IMethodSymbol methodSymbol)
    {
        var result = new List<EmitterParameter>();

        // Check if the descriptor has more parameters than the method
        // (the extra one is the implicit @this)
        var hasThisParam = descriptor.ParametersList?.Parameters.Count > methodSymbol.Parameters.Length;
        if (hasThisParam && descriptor.ParametersList is not null)
        {
            var thisParam = descriptor.ParametersList.Parameters[0];
            var thisTypeFqn = thisParam.Type?.ToString() ?? "object";
            result.Add(new EmitterParameter("@this", thisTypeFqn, isThis: true));
        }

        foreach (var param in methodSymbol.Parameters)
        {
            result.Add(new EmitterParameter(
                param.Name,
                param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                symbol: param));
        }

        return result;
    }

    /// <summary>
    /// Walks a block body's IOperation tree and reports diagnostics for constructs
    /// that cannot be translated to expression trees. Called at interpretation time
    /// (before emission) so users get early compile-time feedback.
    /// </summary>
    private static void ValidateBlockBody(
        SemanticModel semanticModel,
        SyntaxNode bodySyntax,
        string memberName,
        SourceProductionContext context)
    {
        var operation = semanticModel.GetOperation(bodySyntax);
        if (operation is null) return;

        WalkOperations(operation, memberName, context);
    }

    private static void WalkOperations(
        IOperation operation,
        string memberName,
        SourceProductionContext context)
    {
        switch (operation)
        {
            case ITryOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "try/catch/finally"));
                return;

            case IUsingOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "using statement"));
                return;

            case ILockOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "lock statement"));
                return;

            case IForLoopOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "for loop — use foreach for LINQ provider compatibility"));
                return;

            case IWhileLoopOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "while/do-while loop — use foreach for LINQ provider compatibility"));
                return;

            case IAwaitOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.SideEffectInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    $"Member '{memberName}' contains 'await' which cannot be represented in an expression tree."));
                return;

            case ICoalesceAssignmentOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "??= operator"));
                return;

            case IDeconstructionAssignmentOperation decon:
                // Accept the tuple-literal-to-tuple-literal shape (e.g. `(A, B) = (x, y);`) —
                // the ctor-body emitter decomposes it into individual property bindings.
                // Reject everything else (Deconstruct method calls, nested tuples, discards, etc.).
                if (!IsSimpleTupleLiteralDeconstruction(decon))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.UnsupportedStatementInBlockBody,
                        operation.Syntax?.GetLocation() ?? Location.None,
                        memberName, "deconstruction assignment"));
                }
                return;

            case IDynamicInvocationOperation or IDynamicMemberReferenceOperation
                or IDynamicIndexerAccessOperation or IDynamicObjectCreationOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "dynamic operation"));
                return;
        }

        // Recurse into child operations
        foreach (var child in operation.ChildOperations)
        {
            WalkOperations(child, memberName, context);
        }
    }

    /// <summary>
    /// Returns true when the deconstruction has the shape
    /// <c>(member, member, ...) = (value, value, ...)</c> — two tuple literals of equal arity
    /// with the left side referencing only properties or fields of <c>this</c>. Only this shape
    /// is supported by the constructor-body emitter.
    /// </summary>
    private static bool IsSimpleTupleLiteralDeconstruction(IDeconstructionAssignmentOperation decon)
    {
        var target = UnwrapConversions(decon.Target);
        var value = UnwrapConversions(decon.Value);

        if (target is not ITupleOperation targetTuple || value is not ITupleOperation valueTuple
            || targetTuple.Elements.Length != valueTuple.Elements.Length)
        {
            return false;
        }

        foreach (var element in targetTuple.Elements)
        {
            var unwrapped = UnwrapConversions(element);
            if (unwrapped is not IPropertyReferenceOperation && unwrapped is not IFieldReferenceOperation)
                return false;
        }
        return true;
    }

    private static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conv)
            operation = conv.Operand;
        return operation;
    }
}
