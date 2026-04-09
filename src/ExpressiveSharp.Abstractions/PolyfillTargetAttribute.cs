namespace ExpressiveSharp;

/// <summary>
/// Specifies the static type whose extension method the polyfill interceptor should forward to,
/// instead of the default <see cref="System.Linq.Queryable"/>.
/// </summary>
/// <remarks>
/// Apply this attribute to delegate-based <see cref="IExpressiveQueryable{T}"/> stubs when the
/// matching <c>Expression&lt;Func&lt;…&gt;&gt;</c> overload lives in a type other than
/// <see cref="System.Linq.Queryable"/> (e.g., EF Core's <c>EntityFrameworkQueryableExtensions</c>).
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class PolyfillTargetAttribute : Attribute
{
    /// <summary>
    /// The static type that declares the target extension method.
    /// </summary>
    public Type TargetType { get; }

    public PolyfillTargetAttribute(Type targetType) => TargetType = targetType;
}
