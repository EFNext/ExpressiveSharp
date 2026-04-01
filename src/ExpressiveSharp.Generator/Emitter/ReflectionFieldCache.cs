using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Emitter;

/// <summary>
/// Returns inline reflection expressions for
/// <see cref="System.Reflection.MethodInfo"/>, <see cref="System.Reflection.PropertyInfo"/>,
/// <see cref="System.Reflection.ConstructorInfo"/>, and <see cref="System.Reflection.FieldInfo"/>
/// needed by the emitted expression-tree-building code.
/// Each method returns a C# expression string that evaluates to the
/// reflection object at runtime.
/// </summary>
internal sealed class ReflectionFieldCache
{
    private static readonly SymbolDisplayFormat _fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    private readonly Dictionary<ITypeSymbol, string> _typeAliases;

    public ReflectionFieldCache(Dictionary<ITypeSymbol, string> typeAliases)
    {
        _typeAliases = typeAliases;
    }

    private string ResolveTypeFqn(ITypeSymbol type)
        => _typeAliases.TryGetValue(type, out var alias) ? alias : type.ToDisplayString(_fullyQualifiedFormat);

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    public string EnsurePropertyInfo(IPropertySymbol property)
    {
        var typeFqn = ResolveTypeFqn(property.ContainingType);
        return $"typeof({typeFqn}).GetProperty(\"{property.Name}\")";
    }

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.FieldInfo"/>.
    /// </summary>
    public string EnsureFieldInfo(IFieldSymbol field)
    {
        var typeFqn = ResolveTypeFqn(field.ContainingType);
        var flags = field.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";
        return $"typeof({typeFqn}).GetField(\"{field.Name}\", {flags})";
    }

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.MethodInfo"/>.
    /// </summary>
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
            // (e.g., SetProperty<P>(Func<T,P>, P) vs SetProperty<P>(Func<T,P>, Func<T,P>))
            // by checking whether each parameter is a generic type or a type parameter.
            var paramChecksBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < originalDef.Parameters.Length; i++)
            {
                var paramType = originalDef.Parameters[i].Type;
                if (paramType is ITypeParameterSymbol)
                    paramChecksBuilder.Append($" && !m.GetParameters()[{i}].ParameterType.IsGenericType");
                else if (paramType is INamedTypeSymbol { IsGenericType: true })
                    paramChecksBuilder.Append($" && m.GetParameters()[{i}].ParameterType.IsGenericType");
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

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.ConstructorInfo"/>.
    /// </summary>
    public string EnsureConstructorInfo(IMethodSymbol constructor)
    {
        var typeFqn = ResolveTypeFqn(constructor.ContainingType);
        var paramTypes = string.Join(", ", constructor.Parameters.Select(p =>
            $"typeof({ResolveTypeFqn(p.Type)})"));
        return $"typeof({typeFqn}).GetConstructor(new global::System.Type[] {{ {paramTypes} }})";
    }
}
