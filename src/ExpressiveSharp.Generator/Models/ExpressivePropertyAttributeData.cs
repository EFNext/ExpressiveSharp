using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Models;

// Immutable record struct — safe for incremental generator caching.
readonly internal record struct ExpressivePropertyAttributeData
{
    public string? TargetName { get; }

    public IReadOnlyList<string> TransformerTypeNames { get; }

    public ExpressivePropertyAttributeData(AttributeData attribute)
    {
        var transformerTypeNames = new List<string>();

        if (attribute.ConstructorArguments.Length == 1 &&
            attribute.ConstructorArguments[0].Value is string name)
        {
            TargetName = name;
        }
        else
        {
            TargetName = null;
        }

        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == "Transformers" && namedArgument.Value.Kind == TypedConstantKind.Array)
            {
                foreach (var element in namedArgument.Value.Values)
                {
                    if (element.Value is INamedTypeSymbol typeSymbol)
                    {
                        transformerTypeNames.Add(
                            typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    }
                }
            }
        }

        TransformerTypeNames = transformerTypeNames.ToArray();
    }
}
