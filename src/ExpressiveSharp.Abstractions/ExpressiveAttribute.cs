namespace ExpressiveSharp;

/// <summary>
/// Declares this property, method or constructor to be Expressive.
/// A companion expression tree will be generated.
/// </summary>
/// <remarks>
/// Use the <see cref="Transformers"/> property to apply custom <see cref="IExpressionTreeTransformer"/>
/// implementations at runtime when the expression is resolved.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor,
    Inherited = true, AllowMultiple = false)]
public sealed class ExpressiveAttribute : Attribute
{
    /// <summary>
    /// When <c>true</c>, allows block-bodied members (methods/properties with <c>{ }</c> bodies).
    /// Block bodies support local variables, if/else, foreach loops, and more, but not all
    /// constructs are translatable by every LINQ provider.
    /// When not explicitly set, the MSBuild property <c>Expressive_AllowBlockBody</c> is used
    /// (defaults to <c>false</c>).
    /// </summary>
    public bool AllowBlockBody { get; set; }

    /// <summary>
    /// Additional <see cref="IExpressionTreeTransformer"/> types to apply at runtime.
    /// Each type must have a parameterless constructor.
    /// </summary>
    public Type[]? Transformers { get; set; }

    /// <summary>
    /// When <c>true</c>, the property's body is treated as a SQL formula and the property gains
    /// dual semantics: in-memory reads evaluate the formula, while values materialized from
    /// query results (e.g. by EF Core or HotChocolate's projection middleware) are stored and
    /// returned verbatim. Requires the property's get accessor to use the pattern
    /// <c>=&gt; field ?? (&lt;formula&gt;)</c> (or with a manually declared private nullable backing field
    /// in place of <c>field</c>), and an init or set accessor that stores into the same backing
    /// location. The property must not be nullable.
    /// </summary>
    public bool Projectable { get; set; }
}
