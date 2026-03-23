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
    /// Additional <see cref="IExpressionTreeTransformer"/> types to apply at runtime.
    /// Each type must have a parameterless constructor.
    /// </summary>
    public Type[]? Transformers { get; set; }
}
