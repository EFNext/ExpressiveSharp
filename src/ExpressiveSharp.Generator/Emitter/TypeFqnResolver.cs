using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Emitter;

internal static class TypeFqnResolver
{
    private static readonly SymbolDisplayFormat _fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    // Aliases can sit at any nesting depth (IGrouping<int, Anon>, IEnumerable<Anon>, Anon[]), so
    // arguments resolve recursively; the rebuild stays gated on an actual substitution so
    // alias-free types keep their plain display string.
    internal static string Resolve(ITypeSymbol type, Dictionary<ITypeSymbol, string> typeAliases)
    {
        if (typeAliases.TryGetValue(type, out var alias))
            return alias;

        if (type is IArrayTypeSymbol array)
        {
            var elementFqn = Resolve(array.ElementType, typeAliases);
            if (elementFqn != array.ElementType.ToDisplayString(_fullyQualifiedFormat))
                return elementFqn + "[" + new string(',', array.Rank - 1) + "]";
        }

        if (type is INamedTypeSymbol named && named.TypeArguments.Length > 0)
        {
            var anyResolved = false;
            var resolvedArgs = new string[named.TypeArguments.Length];
            for (var i = 0; i < named.TypeArguments.Length; i++)
            {
                resolvedArgs[i] = Resolve(named.TypeArguments[i], typeAliases);
                anyResolved |= resolvedArgs[i] != named.TypeArguments[i].ToDisplayString(_fullyQualifiedFormat);
            }
            if (anyResolved)
            {
                var openType = named.ConstructedFrom.ToDisplayString(_fullyQualifiedFormat);
                var idx = openType.LastIndexOf('<');
                if (idx >= 0)
                    openType = openType.Substring(0, idx);
                return openType + "<" + string.Join(", ", resolvedArgs) + ">";
            }
        }

        return type.ToDisplayString(_fullyQualifiedFormat);
    }
}
