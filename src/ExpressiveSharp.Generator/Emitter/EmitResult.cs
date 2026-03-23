namespace ExpressiveSharp.Generator.Emitter;

/// <summary>
/// The result of translating a C# expression body into imperative
/// <c>System.Linq.Expressions.Expression.*</c> factory calls.
/// </summary>
internal sealed class EmitResult
{
    /// <summary>
    /// C# statements that build the expression tree (the method body).
    /// The last statement is a <c>return Expression.Lambda&lt;…&gt;(…);</c>.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// <c>private static readonly</c> field declarations for cached
    /// <see cref="System.Reflection.MethodInfo"/>, <see cref="System.Reflection.PropertyInfo"/>,
    /// and <see cref="System.Reflection.ConstructorInfo"/> instances used by <see cref="Body"/>.
    /// </summary>
    public IReadOnlyList<string> StaticFields { get; }

    public EmitResult(string body, IReadOnlyList<string> staticFields)
    {
        Body = body;
        StaticFields = staticFields;
    }
}
