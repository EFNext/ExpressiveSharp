namespace ExpressiveSharp;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor,
    Inherited = true, AllowMultiple = false)]
public sealed class ExpressiveAttribute : Attribute
{
    /// <summary>
    /// When <c>true</c>, allows block-bodied members (methods/properties with <c>{ }</c> bodies).
    /// When not explicitly set, the MSBuild property <c>Expressive_AllowBlockBody</c> is used
    /// (defaults to <c>false</c>).
    /// </summary>
    public bool AllowBlockBody { get; set; }

    /// <summary>
    /// <see cref="IExpressionTreeTransformer"/> types to apply at runtime.
    /// Each type must have a parameterless constructor.
    /// </summary>
    public Type[]? Transformers { get; set; }
}
