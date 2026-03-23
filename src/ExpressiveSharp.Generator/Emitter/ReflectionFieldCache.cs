using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Emitter;

/// <summary>
/// Tracks and deduplicates <c>private static readonly</c> reflection field declarations
/// (<see cref="System.Reflection.MethodInfo"/>, <see cref="System.Reflection.PropertyInfo"/>,
/// <see cref="System.Reflection.ConstructorInfo"/>, <see cref="System.Reflection.FieldInfo"/>)
/// needed by the emitted expression-tree-building code.
/// </summary>
internal sealed class ReflectionFieldCache
{
    private static readonly SymbolDisplayFormat _fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat;

    private readonly string _prefix;
    private readonly Dictionary<string, string> _fieldNamesByKey = new();
    private readonly List<string> _declarations = new();
    private int _propertyCounter;
    private int _methodCounter;
    private int _constructorCounter;
    private int _fieldCounter;

    public ReflectionFieldCache(string prefix = "")
    {
        _prefix = prefix;
    }

    /// <summary>
    /// Returns the field name for a cached <see cref="System.Reflection.PropertyInfo"/>,
    /// creating the declaration if this property hasn't been seen before.
    /// </summary>
    public string EnsurePropertyInfo(IPropertySymbol property)
    {
        var typeFqn = property.ContainingType.ToDisplayString(_fullyQualifiedFormat);
        var key = $"P:{typeFqn}.{property.Name}";
        if (_fieldNamesByKey.TryGetValue(key, out var fieldName))
            return fieldName;

        fieldName = $"_{_prefix}p{_propertyCounter++}";
        var declaration = $"""private static readonly global::System.Reflection.PropertyInfo {fieldName} = typeof({typeFqn}).GetProperty("{property.Name}");""";
        _fieldNamesByKey[key] = fieldName;
        _declarations.Add(declaration);
        return fieldName;
    }

    /// <summary>
    /// Returns the field name for a cached <see cref="System.Reflection.FieldInfo"/>,
    /// creating the declaration if this field hasn't been seen before.
    /// </summary>
    public string EnsureFieldInfo(IFieldSymbol field)
    {
        var typeFqn = field.ContainingType.ToDisplayString(_fullyQualifiedFormat);
        var key = $"F:{typeFqn}.{field.Name}";
        if (_fieldNamesByKey.TryGetValue(key, out var fieldName))
            return fieldName;

        fieldName = $"_{_prefix}f{_fieldCounter++}";
        var flags = field.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";
        var declaration = $"""private static readonly global::System.Reflection.FieldInfo {fieldName} = typeof({typeFqn}).GetField("{field.Name}", {flags});""";
        _fieldNamesByKey[key] = fieldName;
        _declarations.Add(declaration);
        return fieldName;
    }

    /// <summary>
    /// Returns the field name for a cached <see cref="System.Reflection.MethodInfo"/>,
    /// creating the declaration if this method hasn't been seen before.
    /// </summary>
    public string EnsureMethodInfo(IMethodSymbol method)
    {
        var typeFqn = method.ContainingType.ToDisplayString(_fullyQualifiedFormat);
        var paramTypes = string.Join(", ", method.Parameters.Select(p =>
            $"typeof({p.Type.ToDisplayString(_fullyQualifiedFormat)})"));
        var key = $"M:{typeFqn}.{method.Name}({paramTypes})";
        if (_fieldNamesByKey.TryGetValue(key, out var fieldName))
            return fieldName;

        fieldName = $"_{_prefix}m{_methodCounter++}";
        var flags = method.IsStatic
            ? "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static"
            : "global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance";

        string declaration;
        if (method.IsGenericMethod)
        {
            // Generic methods: find by name + generic arity + param count, then MakeGenericMethod.
            // We can't use GetMethod with parameter types because the definition's parameters
            // reference its own type parameters (e.g. IEnumerable<TSource>) which aren't valid C# types.
            var originalDef = method.OriginalDefinition;
            var genericArity = originalDef.TypeParameters.Length;
            var paramCount = originalDef.Parameters.Length;
            var typeArgs = string.Join(", ", method.TypeArguments.Select(t =>
                $"typeof({t.ToDisplayString(_fullyQualifiedFormat)})"));
            declaration = $"private static readonly global::System.Reflection.MethodInfo {fieldName} = global::System.Linq.Enumerable.First(global::System.Linq.Enumerable.Where(typeof({typeFqn}).GetMethods({flags}), m => m.Name == \"{method.Name}\" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == {genericArity} && m.GetParameters().Length == {paramCount})).MakeGenericMethod({typeArgs});";
        }
        else
        {
            declaration = $"private static readonly global::System.Reflection.MethodInfo {fieldName} = typeof({typeFqn}).GetMethod(\"{method.Name}\", {flags}, null, new global::System.Type[] {{ {paramTypes} }}, null);";
        }

        _fieldNamesByKey[key] = fieldName;
        _declarations.Add(declaration);
        return fieldName;
    }

    /// <summary>
    /// Returns the field name for a cached <see cref="System.Reflection.ConstructorInfo"/>,
    /// creating the declaration if this constructor hasn't been seen before.
    /// </summary>
    public string EnsureConstructorInfo(IMethodSymbol constructor)
    {
        var typeFqn = constructor.ContainingType.ToDisplayString(_fullyQualifiedFormat);
        var paramTypes = string.Join(", ", constructor.Parameters.Select(p =>
            $"typeof({p.Type.ToDisplayString(_fullyQualifiedFormat)})"));
        var key = $"C:{typeFqn}({paramTypes})";
        if (_fieldNamesByKey.TryGetValue(key, out var fieldName))
            return fieldName;

        fieldName = $"_{_prefix}c{_constructorCounter++}";
        var declaration = $"private static readonly global::System.Reflection.ConstructorInfo {fieldName} = typeof({typeFqn}).GetConstructor(new global::System.Type[] {{ {paramTypes} }});";
        _fieldNamesByKey[key] = fieldName;
        _declarations.Add(declaration);
        return fieldName;
    }

    /// <summary>
    /// Returns all generated <c>private static readonly</c> field declarations.
    /// </summary>
    public IReadOnlyList<string> GetDeclarations()
    {
        return _declarations;
    }
}
