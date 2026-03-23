using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Models;

/// <summary>
/// Plain-data snapshot of the [Expressive] attribute arguments.
/// </summary>
readonly internal record struct ExpressiveAttributeData
{
    // Built-in transformer flags
    public bool RemoveNullConditionalPatterns { get; }
    public bool FlattenBlockExpressions { get; }

    // Custom transformer type names (fully qualified)
    public IReadOnlyList<string> TransformerTypeNames { get; }

    public ExpressiveAttributeData(AttributeData attribute)
    {
        var removeNullConditionalPatterns = false;
        var flattenBlockExpressions = false;
        var transformerTypeNames = new List<string>();

        foreach (var namedArgument in attribute.NamedArguments)
        {
            var key = namedArgument.Key;
            var value = namedArgument.Value;
            switch (key)
            {
                case nameof(RemoveNullConditionalPatterns):
                    if (value.Value is true)
                        removeNullConditionalPatterns = true;
                    break;
                case nameof(FlattenBlockExpressions):
                    if (value.Value is true)
                        flattenBlockExpressions = true;
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

        RemoveNullConditionalPatterns = removeNullConditionalPatterns;
        FlattenBlockExpressions = flattenBlockExpressions;
        TransformerTypeNames = transformerTypeNames;
    }
}
