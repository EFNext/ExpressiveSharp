namespace ExpressiveSharp;

/// <summary>
/// Declares this property, method or constructor to be Expressive.
/// A companion expression tree will be generated.
/// </summary>
/// <remarks>
/// Boolean properties correspond to built-in <see cref="IExpressionTreeTransformer"/> implementations.
/// Setting a property to <c>true</c> applies that transformer at runtime when the expression is resolved.
/// Use the <see cref="Transformers"/> property for custom transformer types.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor,
    Inherited = true, AllowMultiple = false)]
public sealed class ExpressiveAttribute : Attribute
{
    /// <summary>
    /// When <c>true</c>, removes null-conditional patterns
    /// (<c>x != null ? x.B : default</c> → <c>x.B</c>) from the expression tree at runtime.
    /// Useful for LINQ providers where null propagation is handled by the database engine.
    /// </summary>
    public bool RemoveNullConditionalPatterns { get; set; }

    /// <summary>
    /// When <c>true</c>, flattens <c>Expression.Block</c> nodes by inlining variables,
    /// producing a single expression without block wrappers.
    /// Useful for LINQ providers that do not support <c>BlockExpression</c>.
    /// </summary>
    public bool FlattenBlockExpressions { get; set; }

    /// <summary>
    /// Additional <see cref="IExpressionTreeTransformer"/> types to apply at runtime.
    /// Each type must have a parameterless constructor.
    /// These are applied after the built-in transformer flags above.
    /// </summary>
    public Type[]? Transformers { get; set; }
}
