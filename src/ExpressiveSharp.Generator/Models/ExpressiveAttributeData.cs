using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Models;

readonly internal record struct ExpressiveAttributeData
{
    public bool? AllowBlockBody { get; }

    public IReadOnlyList<string> TransformerTypeNames { get; }

    public ExpressiveAttributeData(AttributeData attribute)
    {
        bool? allowBlockBody = null;
        var transformerTypeNames = new List<string>();

        foreach (var namedArgument in attribute.NamedArguments)
        {
            var key = namedArgument.Key;
            var value = namedArgument.Value;
            switch (key)
            {
                case nameof(AllowBlockBody):
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
}
