using ExpressiveSharp.Generator.Emitter;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Interpretation;

static internal class ExpressiveForInterpreter
{
    public static ExpressiveDescriptor? GetDescriptor(
        SemanticModel semanticModel,
        MemberDeclarationSyntax stubMember,
        ISymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        Compilation compilation)
    {
        var stubIdentifierLocation = stubMember switch
        {
            MethodDeclarationSyntax m => m.Identifier.GetLocation(),
            PropertyDeclarationSyntax p => p.Identifier.GetLocation(),
            _ => stubMember.GetLocation()
        };

        // Two-arg form [ExpressiveFor(typeof(T), "Name")] resolves T; single-arg form defaults to the stub's containing type.
        INamedTypeSymbol? targetType;
        if (attributeData.TargetTypeMetadataName is not null)
        {
            targetType = compilation.GetTypeByMetadataName(attributeData.TargetTypeMetadataName);
            if (targetType is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.ExpressiveForTargetTypeNotFound,
                    stubIdentifierLocation,
                    attributeData.TargetTypeFullName));
                return null;
            }
        }
        else
        {
            targetType = stubSymbol.ContainingType;
        }

        // Property stubs can only target properties (no parameter list to carry method args).
        if (stubMember is PropertyDeclarationSyntax stubProperty && stubSymbol is IPropertySymbol stubPropertySymbol)
        {
            if (attributeData.MemberKind == ExpressiveForMemberKind.Constructor)
                return null;

            return ResolvePropertyStub(semanticModel, stubProperty, stubPropertySymbol, attributeData,
                globalOptions, context, compilation, targetType, stubIdentifierLocation);
        }

        if (stubMember is MethodDeclarationSyntax stubMethod && stubSymbol is IMethodSymbol stubMethodSymbol)
        {
            return attributeData.MemberKind switch
            {
                ExpressiveForMemberKind.MethodOrProperty =>
                    ResolveMethodOrProperty(semanticModel, stubMethod, stubMethodSymbol, attributeData, globalOptions, context, compilation, targetType),
                ExpressiveForMemberKind.Constructor =>
                    ResolveConstructor(semanticModel, stubMethod, stubMethodSymbol, attributeData, globalOptions, context, compilation, targetType),
                _ => null
            };
        }

        return null;
    }

    private static ExpressiveDescriptor? ResolveMethodOrProperty(
        SemanticModel semanticModel,
        MethodDeclarationSyntax stubMethod,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        Compilation compilation,
        INamedTypeSymbol targetType)
    {
        var memberName = attributeData.MemberName;
        if (memberName is null)
            return null;

        var property = FindTargetProperty(targetType, memberName, stubSymbol);
        if (property is not null)
        {
            if (HasExpressiveAttribute(property, compilation))
            {
                ReportConflict(context, stubMethod, memberName, targetType);
                return null;
            }

            return BuildPropertyDescriptor(semanticModel, stubMethod, stubSymbol, attributeData,
                globalOptions, context, targetType, property);
        }

        var method = FindTargetMethod(targetType, memberName, stubSymbol);
        if (method is not null)
        {
            if (HasExpressiveAttribute(method, compilation))
            {
                ReportConflict(context, stubMethod, memberName, targetType);
                return null;
            }

            return BuildMethodDescriptor(semanticModel, stubMethod, stubSymbol, attributeData,
                globalOptions, context, targetType, method);
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.ExpressiveForMemberNotFound,
            stubMethod.Identifier.GetLocation(),
            memberName,
            targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
        return null;
    }

    private static ExpressiveDescriptor? ResolveConstructor(
        SemanticModel semanticModel,
        MethodDeclarationSyntax stubMethod,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        Compilation compilation,
        INamedTypeSymbol targetType)
    {
        var ctor = FindTargetConstructor(targetType, stubSymbol);
        if (ctor is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForMemberNotFound,
                stubMethod.Identifier.GetLocation(),
                ".ctor",
                targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        if (HasExpressiveAttribute(ctor, compilation))
        {
            ReportConflict(context, stubMethod, ".ctor", targetType);
            return null;
        }

        var stubReturnType = stubSymbol.ReturnType;
        if (!SymbolEqualityComparer.Default.Equals(stubReturnType, targetType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForReturnTypeMismatch,
                stubMethod.ReturnType.GetLocation(),
                ".ctor",
                targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                stubReturnType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        return BuildDescriptorFromStub(semanticModel, stubMethod, stubSymbol, attributeData,
            globalOptions, context, targetType, "_ctor",
            ctor.Parameters, isInstanceMember: false);
    }

    private static ExpressiveDescriptor? ResolvePropertyStub(
        SemanticModel semanticModel,
        PropertyDeclarationSyntax stubProperty,
        IPropertySymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        Compilation compilation,
        INamedTypeSymbol targetType,
        Location stubIdentifierLocation)
    {
        var memberName = attributeData.MemberName;
        if (memberName is null)
            return null;

        var target = FindTargetPropertyForPropertyStub(targetType, memberName, stubSymbol);
        if (target is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForMemberNotFound,
                stubIdentifierLocation,
                memberName,
                targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        if (HasExpressiveAttribute(target, compilation))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForConflictsWithExpressive,
                stubIdentifierLocation,
                memberName,
                targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        if (!SymbolEqualityComparer.Default.Equals(stubSymbol.Type, target.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForReturnTypeMismatch,
                stubProperty.Type.GetLocation(),
                target.Name,
                target.Type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                stubSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        return BuildDescriptorFromPropertyStub(semanticModel, stubProperty, stubSymbol, attributeData,
            globalOptions, context, targetType, target.Name);
    }

    private static IPropertySymbol? FindTargetPropertyForPropertyStub(
        INamedTypeSymbol targetType, string memberName, IPropertySymbol stubSymbol)
        => targetType.GetMembers(memberName)
            .OfType<IPropertySymbol>()
            .FirstOrDefault(property => ExpressiveForSignatureMatcher.MatchesPropertyFromPropertyStub(
                property, targetType, stubSymbol.IsStatic, stubSymbol.ContainingType));

    private static IPropertySymbol? FindTargetProperty(
        INamedTypeSymbol targetType, string memberName, IMethodSymbol stubSymbol)
        => targetType.GetMembers(memberName)
            .OfType<IPropertySymbol>()
            .FirstOrDefault(property => ExpressiveForSignatureMatcher.MatchesPropertyFromMethodStub(
                property, targetType, stubSymbol.IsStatic, stubSymbol.ContainingType, stubSymbol.Parameters));

    private static IMethodSymbol? FindTargetMethod(
        INamedTypeSymbol targetType, string memberName, IMethodSymbol stubSymbol)
        => targetType.GetMembers(memberName)
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind is not (MethodKind.PropertyGet or MethodKind.PropertySet))
            .FirstOrDefault(method => ExpressiveForSignatureMatcher.MatchesMethodSignature(
                method, targetType, stubSymbol.IsStatic, stubSymbol.ContainingType, stubSymbol.Parameters));

    private static IMethodSymbol? FindTargetConstructor(
        INamedTypeSymbol targetType, IMethodSymbol stubSymbol)
    {
        // Constructor stubs are static-only: an instance stub producing a new instance of its own
        // containing type has no natural `this` semantics.
        if (!stubSymbol.IsStatic)
            return null;

        foreach (var ctor in targetType.Constructors)
        {
            if (ctor.IsStatic)
                continue;

            if (ctor.Parameters.Length != stubSymbol.Parameters.Length)
                continue;

            var match = true;
            for (var i = 0; i < ctor.Parameters.Length; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(ctor.Parameters[i].Type, stubSymbol.Parameters[i].Type))
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return ctor;
        }
        return null;
    }

    private static ExpressiveDescriptor? BuildPropertyDescriptor(
        SemanticModel semanticModel,
        MethodDeclarationSyntax stubMethod,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        INamedTypeSymbol targetType,
        IPropertySymbol targetProperty)
    {
        if (!SymbolEqualityComparer.Default.Equals(stubSymbol.ReturnType, targetProperty.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForReturnTypeMismatch,
                stubMethod.ReturnType.GetLocation(),
                targetProperty.Name,
                targetProperty.Type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                stubSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        var targetParams = System.Collections.Immutable.ImmutableArray<IParameterSymbol>.Empty;

        return BuildDescriptorFromStub(semanticModel, stubMethod, stubSymbol, attributeData,
            globalOptions, context, targetType, targetProperty.Name,
            targetParams, isInstanceMember: !targetProperty.IsStatic);
    }

    private static ExpressiveDescriptor? BuildMethodDescriptor(
        SemanticModel semanticModel,
        MethodDeclarationSyntax stubMethod,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        INamedTypeSymbol targetType,
        IMethodSymbol targetMethod)
    {
        if (!SymbolEqualityComparer.Default.Equals(stubSymbol.ReturnType, targetMethod.ReturnType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForReturnTypeMismatch,
                stubMethod.ReturnType.GetLocation(),
                targetMethod.Name,
                targetMethod.ReturnType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                stubSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        return BuildDescriptorFromStub(semanticModel, stubMethod, stubSymbol, attributeData,
            globalOptions, context, targetType, targetMethod.Name,
            targetMethod.Parameters, isInstanceMember: !targetMethod.IsStatic);
    }

    private static ExpressiveDescriptor? BuildDescriptorFromStub(
        SemanticModel semanticModel,
        MethodDeclarationSyntax stubMethod,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        INamedTypeSymbol targetType,
        string targetMemberName,
        System.Collections.Immutable.ImmutableArray<IParameterSymbol> targetParameters,
        bool isInstanceMember)
    {
        var rewriter = new DeclarationSyntaxRewriter(semanticModel);
        var allowBlockBody = attributeData.AllowBlockBody ?? globalOptions.AllowBlockBody;

        SyntaxNode bodySyntax;
        if (stubMethod.ExpressionBody is not null)
        {
            bodySyntax = stubMethod.ExpressionBody.Expression;
        }
        else if (stubMethod.Body is not null)
        {
            if (!allowBlockBody)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.BlockBodyRequiresOptIn,
                    stubMethod.Identifier.GetLocation(),
                    stubSymbol.Name));
                return null;
            }
            bodySyntax = stubMethod.Body;
        }
        else
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.RequiresBodyDefinition,
                stubMethod.GetLocation(),
                stubSymbol.Name));
            return null;
        }

        var returnTypeName = rewriter.Visit(stubMethod.ReturnType).ToString();

        var rewrittenParamList = (ParameterListSyntax)rewriter.Visit(stubMethod.ParameterList);
        var explicitSyntax = rewrittenParamList.Parameters.ToList();
        var explicitEmitterParams = stubSymbol.Parameters
            .Select(p => new EmitterParameter(
                p.Name,
                p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                symbol: p))
            .ToList();

        return BuildDescriptorCore(semanticModel, context, stubMethod.SyntaxTree, stubSymbol,
            attributeData, targetType, targetMemberName, targetParameters,
            explicitSyntax, explicitEmitterParams, returnTypeName, bodySyntax);
    }

    /// <summary>
    /// Property stubs are parameterless; an instance stub's <c>this</c> becomes a synthetic receiver on the generated factory.
    /// </summary>
    private static ExpressiveDescriptor? BuildDescriptorFromPropertyStub(
        SemanticModel semanticModel,
        PropertyDeclarationSyntax stubProperty,
        IPropertySymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        INamedTypeSymbol targetType,
        string targetMemberName)
    {
        var rewriter = new DeclarationSyntaxRewriter(semanticModel);
        var allowBlockBody = attributeData.AllowBlockBody ?? globalOptions.AllowBlockBody;

        SyntaxNode? bodySyntax = null;
        if (stubProperty.ExpressionBody is not null)
        {
            bodySyntax = stubProperty.ExpressionBody.Expression;
        }
        else if (stubProperty.AccessorList is not null)
        {
            var getter = stubProperty.AccessorList.Accessors
                .FirstOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);

            if (getter is not null)
            {
                if (getter.ExpressionBody is not null)
                {
                    bodySyntax = getter.ExpressionBody.Expression;
                }
                else if (getter.Body is not null)
                {
                    if (!allowBlockBody)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Diagnostics.BlockBodyRequiresOptIn,
                            stubProperty.Identifier.GetLocation(),
                            stubSymbol.Name));
                        return null;
                    }
                    bodySyntax = getter.Body;
                }
            }
        }

        if (bodySyntax is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.RequiresBodyDefinition,
                stubProperty.GetLocation(),
                stubSymbol.Name));
            return null;
        }

        var returnTypeName = rewriter.Visit(stubProperty.Type).ToString();

        return BuildDescriptorCore(semanticModel, context, stubProperty.SyntaxTree, stubSymbol,
            attributeData, targetType, targetMemberName,
            targetParameters: System.Collections.Immutable.ImmutableArray<IParameterSymbol>.Empty,
            explicitStubParamSyntax: [],
            explicitStubEmitterParams: [],
            returnTypeName, bodySyntax);
    }

    private static ExpressiveDescriptor BuildDescriptorCore(
        SemanticModel semanticModel,
        SourceProductionContext context,
        SyntaxTree stubSyntaxTree,
        ISymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        INamedTypeSymbol targetType,
        string targetMemberName,
        System.Collections.Immutable.ImmutableArray<IParameterSymbol> targetParameters,
        IReadOnlyList<ParameterSyntax> explicitStubParamSyntax,
        IReadOnlyList<EmitterParameter> explicitStubEmitterParams,
        string returnTypeName,
        SyntaxNode bodySyntax)
    {
        var targetClassNamespace = targetType.ContainingNamespace.IsGlobalNamespace
            ? null
            : targetType.ContainingNamespace.ToDisplayString();

        var descriptor = new ExpressiveDescriptor
        {
            UsingDirectives = stubSyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>(),
            ClassName = targetType.Name,
            ClassNamespace = targetClassNamespace,
            MemberName = targetMemberName,
            NestedInClassNames = GetNestedInClassPath(targetType),
            TargetClassNamespace = targetClassNamespace,
            TargetNestedInClassNames = GetNestedInClassPath(targetType),
            ParametersList = SyntaxFactory.ParameterList(),
            ReturnTypeName = returnTypeName,
        };

        foreach (var typeName in attributeData.TransformerTypeNames)
            descriptor.DeclaredTransformerTypeNames.Add(typeName);

        // Parameter types disambiguate method overloads in the registry; properties and parameterless targets keep ParameterTypeNames null.
        if (!targetParameters.IsEmpty)
        {
            descriptor.ParameterTypeNames = targetParameters
                .Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                .ToList();
        }

        var emitterParams = new List<EmitterParameter>();

        // Prepend `@this` so IInstanceReferenceOperation in the body binds to it.
        if (!stubSymbol.IsStatic)
        {
            var thisTypeFqn = stubSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            descriptor.ParametersList = descriptor.ParametersList.AddParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("@this"))
                    .WithType(SyntaxFactory.ParseTypeName(thisTypeFqn)));
            emitterParams.Add(new EmitterParameter("@this", thisTypeFqn, isThis: true));
        }

        foreach (var p in explicitStubParamSyntax)
            descriptor.ParametersList = descriptor.ParametersList.AddParameters(p);
        emitterParams.AddRange(explicitStubEmitterParams);

        var allTypeArgs = emitterParams.Select(p => p.TypeFqn).ToList();
        allTypeArgs.Add(returnTypeName);
        var delegateTypeFqn = $"global::System.Func<{string.Join(", ", allTypeArgs)}>";

        var emitter = new ExpressionTreeEmitter(semanticModel, context);
        descriptor.ExpressionTreeEmission = emitter.Emit(bodySyntax, emitterParams,
            returnTypeName, delegateTypeFqn);

        return descriptor;
    }

    private static IEnumerable<string> GetNestedInClassPath(ITypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.ContainingType is not null)
        {
            foreach (var nestedInClassName in GetNestedInClassPath(namedTypeSymbol.ContainingType))
            {
                yield return nestedInClassName;
            }
        }

        yield return namedTypeSymbol.Name;
    }

    private static bool HasExpressiveAttribute(ISymbol member, Compilation compilation)
    {
        var expressiveAttributeType = compilation.GetTypeByMetadataName("ExpressiveSharp.ExpressiveAttribute");
        if (expressiveAttributeType is null)
            return false;

        foreach (var attr in member.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, expressiveAttributeType))
                return true;
        }
        return false;
    }

    private static void ReportConflict(
        SourceProductionContext context,
        MethodDeclarationSyntax stubMethod,
        string memberName,
        INamedTypeSymbol targetType)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.ExpressiveForConflictsWithExpressive,
            stubMethod.Identifier.GetLocation(),
            memberName,
            targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
    }
}
