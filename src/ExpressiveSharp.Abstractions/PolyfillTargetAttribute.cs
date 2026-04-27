namespace ExpressiveSharp;

/// <summary>
/// Specifies the static type whose extension method the polyfill interceptor should forward to,
/// instead of the default <see cref="System.Linq.Queryable"/>. Apply to delegate-based
/// <see cref="IExpressiveQueryable{T}"/> stubs when the matching <c>Expression&lt;Func&lt;…&gt;&gt;</c>
/// overload lives elsewhere (e.g., EF Core's <c>EntityFrameworkQueryableExtensions</c>).
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class PolyfillTargetAttribute : Attribute
{
    public Type TargetType { get; }

    public PolyfillTargetAttribute(Type targetType) => TargetType = targetType;
}
