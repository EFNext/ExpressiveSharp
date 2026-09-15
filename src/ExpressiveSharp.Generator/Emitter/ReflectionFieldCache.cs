using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Emitter;

internal sealed class ReflectionFieldCache
{
    private static readonly SymbolDisplayFormat _fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    private readonly Dictionary<ITypeSymbol, string> _typeAliases;

    public ReflectionFieldCache(Dictionary<ITypeSymbol, string> typeAliases)
    {
        _typeAliases = typeAliases;
    }

    internal string ResolveTypeFqn(ITypeSymbol type)
        => TypeFqnResolver.Resolve(type, _typeAliases);

    public string EnsurePropertyInfo(IPropertySymbol property)
    {
        var typeFqn = ResolveTypeFqn(property.ContainingType);
        var flags = property.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";
        return $"typeof({typeFqn}).GetProperty(\"{property.Name}\", {flags})";
    }

    public string EnsureFieldInfo(IFieldSymbol field)
    {
        // Named tuple elements (X, Y) only exist at compile time; the runtime field on
        // ValueTuple<...> is Item1/Item2/etc. CorrespondingTupleField maps to that.
        var runtimeField = field.CorrespondingTupleField ?? field;
        var typeFqn = ResolveTypeFqn(runtimeField.ContainingType);
        var flags = runtimeField.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";
        return $"typeof({typeFqn}).GetField(\"{runtimeField.Name}\", {flags})";
    }

    public string EnsureMethodInfo(IMethodSymbol method)
    {
        var typeFqn = ResolveTypeFqn(method.ContainingType);

        var flags = method.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";

        if (method.IsGenericMethod)
        {
            var originalDef = method.OriginalDefinition;
            var genericArity = originalDef.TypeParameters.Length;
            var paramCount = originalDef.Parameters.Length;
            var typeArgs = string.Join(", ", method.TypeArguments.Select(t =>
                $"typeof({ResolveTypeFqn(t)})"));

            // Disambiguate overloads that share name, generic arity, and parameter count
            // by pinning each parameter's shape and any closed type arguments — this is
            // what separates Sum(Func<T,int>) from Sum(Func<T,decimal>), etc.
            var paramChecksBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < originalDef.Parameters.Length; i++)
            {
                var paramType = originalDef.Parameters[i].Type;
                if (paramType is ITypeParameterSymbol)
                    paramChecksBuilder.Append($" && m.GetParameters()[{i}].ParameterType.IsGenericParameter && !m.GetParameters()[{i}].ParameterType.IsGenericType");
                else if (paramType is INamedTypeSymbol { IsGenericType: true } genericParam)
                {
                    paramChecksBuilder.Append($" && m.GetParameters()[{i}].ParameterType.IsGenericType && !m.GetParameters()[{i}].ParameterType.IsGenericParameter");
                    for (int t = 0; t < genericParam.TypeArguments.Length; t++)
                    {
                        var innerArg = genericParam.TypeArguments[t];
                        if (innerArg is ITypeParameterSymbol)
                            continue;

                        if (ContainsTypeParameter(innerArg))
                        {
                            // Pin the open-generic shape; closed FQN would leak the method type parameter.
                            if (innerArg is INamedTypeSymbol { IsGenericType: true } innerGeneric)
                            {
                                var openFqn = innerGeneric.ConstructUnboundGenericType().ToDisplayString(_fullyQualifiedFormat);
                                paramChecksBuilder.Append($" && m.GetParameters()[{i}].ParameterType.GetGenericArguments()[{t}].IsGenericType && m.GetParameters()[{i}].ParameterType.GetGenericArguments()[{t}].GetGenericTypeDefinition() == typeof({openFqn})");
                            }
                        }
                        else
                        {
                            paramChecksBuilder.Append($" && m.GetParameters()[{i}].ParameterType.GetGenericArguments()[{t}] == typeof({ResolveTypeFqn(innerArg)})");
                        }
                    }
                }
            }
            var paramChecks = paramChecksBuilder.ToString();

            return $"global::System.Linq.Enumerable.First(global::System.Linq.Enumerable.Where(typeof({typeFqn}).GetMethods({flags}), m => m.Name == \"{method.Name}\" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == {genericArity} && m.GetParameters().Length == {paramCount}{paramChecks})).MakeGenericMethod({typeArgs})";
        }
        else
        {
            var paramTypes = string.Join(", ", method.Parameters.Select(p =>
                $"typeof({ResolveTypeFqn(p.Type)})"));
            return $"typeof({typeFqn}).GetMethod(\"{method.Name}\", {flags}, null, new global::System.Type[] {{ {paramTypes} }}, null)";
        }
    }

    public string EnsureConstructorInfo(IMethodSymbol constructor)
    {
        var typeFqn = ResolveTypeFqn(constructor.ContainingType);
        var paramTypes = string.Join(", ", constructor.Parameters.Select(p =>
            $"typeof({ResolveTypeFqn(p.Type)})"));
        const string flags = "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";
        return $"typeof({typeFqn}).GetConstructor({flags}, null, new global::System.Type[] {{ {paramTypes} }}, null)";
    }

    private static bool ContainsTypeParameter(ITypeSymbol type)
    {
        if (type is ITypeParameterSymbol)
            return true;
        if (type is INamedTypeSymbol named)
        {
            foreach (var arg in named.TypeArguments)
            {
                if (ContainsTypeParameter(arg))
                    return true;
            }
        }
        if (type is IArrayTypeSymbol arr)
            return ContainsTypeParameter(arr.ElementType);
        return false;
    }
}
