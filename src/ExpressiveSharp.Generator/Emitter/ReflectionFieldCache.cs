using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Emitter;

/// <summary>
/// Returns inline reflection expressions for
/// <see cref="System.Reflection.MethodInfo"/>, <see cref="System.Reflection.PropertyInfo"/>,
/// <see cref="System.Reflection.ConstructorInfo"/>, and <see cref="System.Reflection.FieldInfo"/>
/// needed by the emitted expression-tree-building code.
/// Each <c>Ensure*</c> method returns a C# expression string that evaluates to the
/// reflection object at runtime, rather than a static field name.
/// </summary>
internal sealed class ReflectionFieldCache
{
    private static readonly SymbolDisplayFormat _fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    private readonly Dictionary<string, string> _expressionsByKey = new();
    private readonly Dictionary<ITypeSymbol, string> _typeAliases;

    public ReflectionFieldCache(string prefix = "", Dictionary<ITypeSymbol, string>? typeAliases = null)
    {
        _typeAliases = typeAliases ?? new Dictionary<ITypeSymbol, string>(SymbolEqualityComparer.Default);
    }

    private string ResolveTypeFqn(ITypeSymbol type)
        => _typeAliases.TryGetValue(type, out var alias) ? alias : type.ToDisplayString(_fullyQualifiedFormat);

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    public string EnsurePropertyInfo(IPropertySymbol property)
    {
        var typeFqn = ResolveTypeFqn(property.ContainingType);
        var key = $"P:{typeFqn}.{property.Name}";
        if (_expressionsByKey.TryGetValue(key, out var cached))
            return cached;

        var expr = $"typeof({typeFqn}).GetProperty(\"{property.Name}\")";
        _expressionsByKey[key] = expr;
        return expr;
    }

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.FieldInfo"/>.
    /// </summary>
    public string EnsureFieldInfo(IFieldSymbol field)
    {
        var typeFqn = ResolveTypeFqn(field.ContainingType);
        var key = $"F:{typeFqn}.{field.Name}";
        if (_expressionsByKey.TryGetValue(key, out var cached))
            return cached;

        var flags = field.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";
        var expr = $"typeof({typeFqn}).GetField(\"{field.Name}\", {flags})";
        _expressionsByKey[key] = expr;
        return expr;
    }

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.MethodInfo"/>.
    /// </summary>
    public string EnsureMethodInfo(IMethodSymbol method)
    {
        var typeFqn = ResolveTypeFqn(method.ContainingType);
        var paramTypes = string.Join(", ", method.Parameters.Select(p =>
            $"typeof({ResolveTypeFqn(p.Type)})"));
        var key = $"M:{typeFqn}.{method.Name}({paramTypes})";
        if (_expressionsByKey.TryGetValue(key, out var cached))
            return cached;

        var flags = method.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";

        string expr;
        if (method.IsGenericMethod)
        {
            var originalDef = method.OriginalDefinition;
            var genericArity = originalDef.TypeParameters.Length;
            var paramCount = originalDef.Parameters.Length;
            var typeArgs = string.Join(", ", method.TypeArguments.Select(t =>
                $"typeof({ResolveTypeFqn(t)})"));
            expr = $"global::System.Linq.Enumerable.First(global::System.Linq.Enumerable.Where(typeof({typeFqn}).GetMethods({flags}), m => m.Name == \"{method.Name}\" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == {genericArity} && m.GetParameters().Length == {paramCount})).MakeGenericMethod({typeArgs})";
        }
        else
        {
            expr = $"typeof({typeFqn}).GetMethod(\"{method.Name}\", {flags}, null, new global::System.Type[] {{ {paramTypes} }}, null)";
        }

        _expressionsByKey[key] = expr;
        return expr;
    }

    /// <summary>
    /// Returns an inline reflection expression for a <see cref="System.Reflection.ConstructorInfo"/>.
    /// </summary>
    public string EnsureConstructorInfo(IMethodSymbol constructor)
    {
        var typeFqn = ResolveTypeFqn(constructor.ContainingType);
        var paramTypes = string.Join(", ", constructor.Parameters.Select(p =>
            $"typeof({ResolveTypeFqn(p.Type)})"));
        var key = $"C:{typeFqn}({paramTypes})";
        if (_expressionsByKey.TryGetValue(key, out var cached))
            return cached;

        var expr = $"typeof({typeFqn}).GetConstructor(new global::System.Type[] {{ {paramTypes} }})";
        _expressionsByKey[key] = expr;
        return expr;
    }

    /// <summary>
    /// Returns all static field declarations. Always empty since reflection is now inlined.
    /// </summary>
    public IReadOnlyList<string> GetDeclarations()
    {
        return Array.Empty<string>();
    }
}
