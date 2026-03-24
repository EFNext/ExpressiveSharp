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

        // Verify parameterless constructor exists
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

        // Collect assignments from base/this delegated constructors
        var delegatedBindings = new List<(IPropertySymbol Property, SyntaxNode ValueSyntax)>();
        if (constructorDeclarationSyntax.Initializer is { } initializer && compilation is not null)
        {
            var initializerSymbol = semanticModel.GetSymbolInfo(initializer).Symbol as IMethodSymbol;
            if (initializerSymbol is not null)
            {
                CollectDelegatedBindings(initializerSymbol, compilation, delegatedBindings, context, memberSymbol.Name);
            }
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
            containingType, delegatedBindings);

        return true;
    }

    /// <summary>
    /// Recursively collects property assignments from a delegated constructor chain.
    /// </summary>
    private static void CollectDelegatedBindings(
        IMethodSymbol delegatedCtor,
        Compilation compilation,
        List<(IPropertySymbol Property, SyntaxNode ValueSyntax)> bindings,
        SourceProductionContext context,
        string memberName)
    {
        var syntax = delegatedCtor.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();

        if (syntax is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.NoSourceAvailableForDelegatedConstructor,
                delegatedCtor.Locations.FirstOrDefault() ?? Location.None,
                delegatedCtor.ToDisplayString(),
                delegatedCtor.ContainingType?.ToDisplayString() ?? "<unknown>",
                memberName));
            return;
        }

        // Follow the chain recursively
        if (syntax.Initializer is { } initializer)
        {
            var delegatedSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var initializerSymbol = delegatedSemanticModel.GetSymbolInfo(initializer).Symbol as IMethodSymbol;
            if (initializerSymbol is not null)
            {
                CollectDelegatedBindings(initializerSymbol, compilation, bindings, context, memberName);
            }
        }

        // Collect property assignments from this constructor's body
        if (syntax.Body is not null)
        {
            var ctorSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            foreach (var statement in syntax.Body.Statements)
            {
                if (statement is ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment }
                    && assignment.Left is MemberAccessExpressionSyntax memberAccess)
                {
                    var symbol = ctorSemanticModel.GetSymbolInfo(memberAccess).Symbol;
                    if (symbol is IPropertySymbol prop)
                    {
                        bindings.Add((prop, assignment.Right));
                    }
                }
                else if (statement is ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax simpleAssignment }
                    && simpleAssignment.Left is IdentifierNameSyntax identifierName)
                {
                    var symbol = ctorSemanticModel.GetSymbolInfo(identifierName).Symbol;
                    if (symbol is IPropertySymbol prop)
                    {
                        bindings.Add((prop, simpleAssignment.Right));
                    }
                }
            }
        }
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

            case IThrowOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "throw expression"));
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

            case IDeconstructionAssignmentOperation:
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedStatementInBlockBody,
                    operation.Syntax?.GetLocation() ?? Location.None,
                    memberName, "deconstruction assignment"));
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
}
