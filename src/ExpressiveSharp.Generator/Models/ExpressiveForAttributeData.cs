using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Models;

/// <summary>
/// Plain-data snapshot of an [ExpressiveFor] or [ExpressiveForConstructor] attribute's arguments.
/// Immutable record struct — safe for incremental generator caching.
/// </summary>
readonly internal record struct ExpressiveForAttributeData
{
    /// <summary>
    /// Fully qualified name of the target type (using <see cref="SymbolDisplayFormat.FullyQualifiedFormat"/>).
    /// </summary>
    public string TargetTypeFullName { get; }

    /// <summary>
    /// Metadata name of the target type (for <see cref="Compilation.GetTypeByMetadataName"/>).
    /// </summary>
    public string? TargetTypeMetadataName { get; }

    /// <summary>
    /// The target member name. Null for constructors.
    /// </summary>
    public string? MemberName { get; }

    /// <summary>
    /// The kind of target member this mapping represents.
    /// </summary>
    public ExpressiveForMemberKind MemberKind { get; }

    public bool? AllowBlockBody { get; }

    public IReadOnlyList<string> TransformerTypeNames { get; }

    public ExpressiveForAttributeData(AttributeData attribute, ExpressiveForMemberKind memberKind)
    {
        MemberKind = memberKind;
        bool? allowBlockBody = null;
        var transformerTypeNames = new List<string>();

        // Extract target type from first constructor argument
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is INamedTypeSymbol targetTypeSymbol)
        {
            TargetTypeFullName = targetTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            TargetTypeMetadataName = GetMetadataName(targetTypeSymbol);
        }
        else
        {
            TargetTypeFullName = "";
            TargetTypeMetadataName = null;
        }

        // Extract member name from second constructor argument (only for ExpressiveFor, not ExpressiveForConstructor)
        if (memberKind != ExpressiveForMemberKind.Constructor &&
            attribute.ConstructorArguments.Length > 1 &&
            attribute.ConstructorArguments[1].Value is string memberName)
        {
            MemberName = memberName;
        }

        // Extract named arguments
        foreach (var namedArgument in attribute.NamedArguments)
        {
            var key = namedArgument.Key;
            var value = namedArgument.Value;
            switch (key)
            {
                case "AllowBlockBody":
                    allowBlockBody = value.Value is true;
                    break;
                case "Transformers":
                    if (value.Kind == TypedConstantKind.Array)
                    {
                        foreach (var element in value.Values)
                        {
                            if (element.Value is INamedTypeSymbol typeSymbol)
                            {
                                transformerTypeNames.Add(
                                    typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                            }
                        }
                    }
                    break;
            }
        }

        AllowBlockBody = allowBlockBody;
        TransformerTypeNames = transformerTypeNames.ToArray();
    }

    private static string? GetMetadataName(INamedTypeSymbol symbol)
    {
        // Build the metadata name by traversing containing types and namespace
        var parts = new List<string>();
        var current = symbol;

        while (current is not null)
        {
            parts.Add(current.MetadataName);
            current = current.ContainingType;
        }

        parts.Reverse();
        var typePart = string.Join("+", parts);

        var ns = symbol.ContainingNamespace;
        if (ns is not null && !ns.IsGlobalNamespace)
        {
            return ns.ToDisplayString() + "." + typePart;
        }

        return typePart;
    }
}

internal enum ExpressiveForMemberKind
{
    /// <summary>Method or property — determined by resolving the target member.</summary>
    MethodOrProperty,

    /// <summary>Constructor.</summary>
    Constructor,
}
