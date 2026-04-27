namespace ExpressiveSharp.Mapping;

/// <summary>
/// Maps an external constructor to an expression-tree body provided by the decorated stub method.
/// The stub's parameters must match the target constructor's parameters and its return type must
/// be the target type. Replaces <c>new T(...)</c> calls during expression-tree expansion.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ExpressiveForConstructorAttribute : Attribute
{
    public Type TargetType { get; }

    /// <summary>
    /// When <c>true</c>, allows block-bodied stubs. When not explicitly set, the MSBuild property
    /// <c>Expressive_AllowBlockBody</c> is used (defaults to <c>false</c>).
    /// </summary>
    public bool AllowBlockBody { get; set; }

    /// <summary>
    /// <see cref="IExpressionTreeTransformer"/> types to apply at runtime.
    /// Each type must have a parameterless constructor.
    /// </summary>
    public Type[]? Transformers { get; set; }

    public ExpressiveForConstructorAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}
