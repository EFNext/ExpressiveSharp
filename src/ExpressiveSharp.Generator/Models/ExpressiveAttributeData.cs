using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Models;

/// <summary>
/// Plain-data snapshot of the [Expressive] attribute arguments.
/// </summary>
readonly internal record struct ExpressiveAttributeData
{
    public bool AllowBlockBody { get; }

    // Custom transformer type names (fully qualified)
    public IReadOnlyList<string> TransformerTypeNames { get; }

    public ExpressiveAttributeData(AttributeData attribute)
    {
        var allowBlockBody = false;
        var transformerTypeNames = new List<string>();

        foreach (var namedArgument in attribute.NamedArguments)
        {
            var key = namedArgument.Key;
            var value = namedArgument.Value;
            switch (key)
            {
                case nameof(AllowBlockBody):
                    if (value.Value is true)
                        allowBlockBody = true;
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
        TransformerTypeNames = transformerTypeNames;
    }
}
