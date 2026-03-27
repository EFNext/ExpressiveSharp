using ExpressiveSharp.Generator.Emitter;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Interpretation;

/// <summary>
/// Interprets [ExpressiveFor] and [ExpressiveForConstructor] stubs:
/// resolves the target member on the external type, validates the signature,
/// and builds an <see cref="ExpressiveDescriptor"/> with the stub's body as the expression source.
/// </summary>
static internal class ExpressiveForInterpreter
{
    public static ExpressiveDescriptor? GetDescriptor(
        SemanticModel semanticModel,
        MethodDeclarationSyntax stubMethod,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        Compilation compilation)
    {
        // Validate: stub must be static
        if (!stubSymbol.IsStatic)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForStubMustBeStatic,
                stubMethod.Identifier.GetLocation(),
                stubSymbol.Name));
            return null;
        }

        // Resolve target type
        var targetType = attributeData.TargetTypeMetadataName is not null
            ? compilation.GetTypeByMetadataName(attributeData.TargetTypeMetadataName)
            : null;

        if (targetType is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressiveForTargetTypeNotFound,
                stubMethod.Identifier.GetLocation(),
                attributeData.TargetTypeFullName));
            return null;
        }

        return attributeData.MemberKind switch
        {
            ExpressiveForMemberKind.MethodOrProperty =>
                ResolveMethodOrProperty(semanticModel, stubMethod, stubSymbol, attributeData, globalOptions, context, compilation, targetType),
            ExpressiveForMemberKind.Constructor =>
                ResolveConstructor(semanticModel, stubMethod, stubSymbol, attributeData, globalOptions, context, compilation, targetType),
            _ => null
        };
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

        // Try to find a property first
        var property = FindTargetProperty(targetType, memberName, stubSymbol);
        if (property is not null)
        {
            // Check for [Expressive] conflict
            if (HasExpressiveAttribute(property, compilation))
            {
                ReportConflict(context, stubMethod, memberName, targetType);
                return null;
            }

            return BuildPropertyDescriptor(semanticModel, stubMethod, stubSymbol, attributeData,
                globalOptions, context, targetType, property);
        }

        // Try to find a method
        var method = FindTargetMethod(targetType, memberName, stubSymbol);
        if (method is not null)
        {
            // Check for [Expressive] conflict
            if (HasExpressiveAttribute(method, compilation))
            {
                ReportConflict(context, stubMethod, memberName, targetType);
                return null;
            }

            return BuildMethodDescriptor(semanticModel, stubMethod, stubSymbol, attributeData,
                globalOptions, context, targetType, method);
        }

        // Neither found
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
        // For constructors, all stub params map to constructor params
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

        // Check for [Expressive] conflict
        if (HasExpressiveAttribute(ctor, compilation))
        {
            ReportConflict(context, stubMethod, ".ctor", targetType);
            return null;
        }

        // Return type must match the target type
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

    private static IPropertySymbol? FindTargetProperty(
        INamedTypeSymbol targetType, string memberName, IMethodSymbol stubSymbol)
    {
        var members = targetType.GetMembers(memberName);
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property)
                continue;

            // Exclude indexers (they have parameters)
            if (property.Parameters.Length > 0)
                continue;

            // Instance property: stub should have 1 param (the instance) whose type matches targetType
            // Static property: stub should have 0 params
            if (property.IsStatic && stubSymbol.Parameters.Length == 0)
                return property;
            if (!property.IsStatic && stubSymbol.Parameters.Length == 1 &&
                SymbolEqualityComparer.Default.Equals(stubSymbol.Parameters[0].Type, targetType))
                return property;
        }
        return null;
    }

    private static IMethodSymbol? FindTargetMethod(
        INamedTypeSymbol targetType, string memberName, IMethodSymbol stubSymbol)
    {
        var members = targetType.GetMembers(memberName);
        foreach (var member in members)
        {
            if (member is not IMethodSymbol method || method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
                continue;

            // For instance methods: first stub param = this, rest = method params
            // For static methods: all stub params = method params
            var expectedStubParamCount = method.IsStatic
                ? method.Parameters.Length
                : method.Parameters.Length + 1;

            if (stubSymbol.Parameters.Length != expectedStubParamCount)
                continue;

            // For instance methods, validate that the stub's first parameter matches the target type
            if (!method.IsStatic &&
                !SymbolEqualityComparer.Default.Equals(stubSymbol.Parameters[0].Type, targetType))
                continue;

            // Check parameter types match
            var offset = method.IsStatic ? 0 : 1;
            var match = true;
            for (var i = 0; i < method.Parameters.Length; i++)
            {
                var targetParamType = method.Parameters[i].Type;
                var stubParamType = stubSymbol.Parameters[i + offset].Type;
                if (!SymbolEqualityComparer.Default.Equals(targetParamType, stubParamType))
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return method;
        }
        return null;
    }

    private static IMethodSymbol? FindTargetConstructor(
        INamedTypeSymbol targetType, IMethodSymbol stubSymbol)
    {
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
        // Validate return type
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

        // For properties, the registry uses the getter's parameters (none for regular properties)
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
        // Validate return type
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

    /// <summary>
    /// Builds the <see cref="ExpressiveDescriptor"/> from the stub method's body,
    /// using the target type's namespace/class path for generated class naming.
    /// </summary>
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
        var declarationSyntaxRewriter = new DeclarationSyntaxRewriter(semanticModel);

        // Use target type's namespace/class path for naming — this is what the registry will use
        var targetClassNamespace = targetType.ContainingNamespace.IsGlobalNamespace
            ? null
            : targetType.ContainingNamespace.ToDisplayString();

        var descriptor = new ExpressiveDescriptor
        {
            UsingDirectives = stubMethod.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>(),
            ClassName = targetType.Name,
            ClassNamespace = targetClassNamespace,
            MemberName = targetMemberName,
            NestedInClassNames = GetNestedInClassPath(targetType),
            TargetClassNamespace = targetClassNamespace,
            TargetNestedInClassNames = GetNestedInClassPath(targetType),
            ParametersList = SyntaxFactory.ParameterList()
        };

        // Populate declared transformers from attribute
        foreach (var typeName in attributeData.TransformerTypeNames)
            descriptor.DeclaredTransformerTypeNames.Add(typeName);

        // Collect parameter type names for registry disambiguation
        // Use the TARGET member's parameter types (not the stub's)
        if (!targetParameters.IsEmpty)
        {
            descriptor.ParameterTypeNames = targetParameters
                .Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                .ToList();
        }

        // Build the stub's parameter list on the descriptor
        // (this is what the expression factory method will use)
        var rewrittenParamList = (ParameterListSyntax)declarationSyntaxRewriter.Visit(stubMethod.ParameterList);
        foreach (var p in rewrittenParamList.Parameters)
        {
            descriptor.ParametersList = descriptor.ParametersList.AddParameters(p);
        }

        // Extract and emit the body
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

        var returnTypeSyntax = declarationSyntaxRewriter.Visit(stubMethod.ReturnType);
        descriptor.ReturnTypeName = returnTypeSyntax.ToString();

        // Build emitter parameters from the stub's parameters
        var emitter = new ExpressionTreeEmitter(semanticModel, context);
        var emitterParams = new List<EmitterParameter>();
        foreach (var param in stubSymbol.Parameters)
        {
            emitterParams.Add(new EmitterParameter(
                param.Name,
                param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                symbol: param));
        }

        var allTypeArgs = emitterParams.Select(p => p.TypeFqn).ToList();
        allTypeArgs.Add(descriptor.ReturnTypeName);
        var delegateTypeFqn = $"global::System.Func<{string.Join(", ", allTypeArgs)}>";

        descriptor.ExpressionTreeEmission = emitter.Emit(bodySyntax, emitterParams,
            descriptor.ReturnTypeName, delegateTypeFqn);

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
