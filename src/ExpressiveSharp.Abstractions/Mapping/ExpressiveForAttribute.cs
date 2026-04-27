namespace ExpressiveSharp.Mapping;

/// <summary>
/// Maps an external method or property to an expression-tree body provided by the decorated stub method.
/// </summary>
/// <remarks>
/// <para>For <b>static methods</b>, the stub parameters must match the target method's parameters exactly.</para>
/// <para>For <b>instance methods</b>, the first stub parameter is the receiver (<c>this</c>), and
/// remaining parameters match the target method's parameters.</para>
/// <para>For <b>instance properties</b>, the stub takes a single parameter (the receiver) and returns the property type.</para>
/// <para>For <b>static properties</b>, the stub is parameterless and returns the property type.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ExpressiveForAttribute : Attribute
{
    /// <summary>
    /// The declaring type of the target member, or <c>null</c> when the single-argument constructor
    /// is used (the target then defaults to the stub's containing type).
    /// </summary>
    public Type? TargetType { get; }

    public string MemberName { get; }

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

    public ExpressiveForAttribute(Type targetType, string memberName)
    {
        TargetType = targetType;
        MemberName = memberName;
    }

    /// <summary>
    /// Shorthand for <c>[ExpressiveFor(typeof(ContainingType), memberName)]</c>.
    /// </summary>
    public ExpressiveForAttribute(string memberName)
    {
        TargetType = null;
        MemberName = memberName;
    }
}
