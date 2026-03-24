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
}
