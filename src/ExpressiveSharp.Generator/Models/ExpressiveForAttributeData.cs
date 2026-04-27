using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Models;

// Plain-data snapshot of an [ExpressiveFor] or [ExpressiveForConstructor] attribute's
// arguments. Immutable record struct — safe for incremental generator caching.
readonly internal record struct ExpressiveForAttributeData
{
    public string TargetTypeFullName { get; }

    // For Compilation.GetTypeByMetadataName.
    public string? TargetTypeMetadataName { get; }

    // Null for constructors.
    public string? MemberName { get; }

    public ExpressiveForMemberKind MemberKind { get; }

    public bool? AllowBlockBody { get; }

    public IReadOnlyList<string> TransformerTypeNames { get; }

    public ExpressiveForAttributeData(AttributeData attribute, ExpressiveForMemberKind memberKind)
    {
        MemberKind = memberKind;
        bool? allowBlockBody = null;
        var transformerTypeNames = new List<string>();

        // ExpressiveFor has two ctors: (Type, string) and (string). ExpressiveForConstructor has (Type).
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

        if (memberKind != ExpressiveForMemberKind.Constructor)
        {
            // [ExpressiveFor(typeof(T), "Name")]
            if (attribute.ConstructorArguments.Length > 1 &&
                attribute.ConstructorArguments[1].Value is string memberNameTwoArg)
            {
                MemberName = memberNameTwoArg;
            }
            // [ExpressiveFor("Name")] — target defaults to the stub's containing type.
            else if (attribute.ConstructorArguments.Length == 1 &&
                     attribute.ConstructorArguments[0].Value is string memberNameOneArg)
            {
                MemberName = memberNameOneArg;
            }
        }

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
    // Determined by resolving the target member.
    MethodOrProperty,
    Constructor,
}
