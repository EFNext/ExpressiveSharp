namespace ExpressiveSharp.Mapping;

/// <summary>
/// Maps an external method or property to an expression-tree body provided by the decorated stub method.
/// The stub's body is compiled into an <c>Expression&lt;TDelegate&gt;</c> that replaces
/// calls to the target member during expression-tree expansion.
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
    /// The type that declares the target member, or <c>null</c> when the single-argument constructor
    /// is used (in which case the target defaults to the stub's containing type at generator time).
    /// </summary>
    public Type? TargetType { get; }

    /// <summary>
    /// The name of the target member on <see cref="TargetType"/>.
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// When <c>true</c>, allows block-bodied stubs (methods with <c>{ }</c> bodies).
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
    /// When <c>true</c>, the generator synthesizes the target property on the stub's containing
    /// type (which must be <c>partial</c>). The stub becomes the formula source and the synthesized
    /// property caches materialized values (from query results) using a <c>?? formula</c> coalesce
    /// for non-nullable types or a <c>hasValue ? stored : formula</c> ternary for nullable types.
    /// Only valid with the single-argument form <c>[ExpressiveFor(nameof(Member), Synthesize = true)]</c>.
    /// </summary>
    public bool Synthesize { get; set; }

    public ExpressiveForAttribute(Type targetType, string memberName)
    {
        TargetType = targetType;
        MemberName = memberName;
    }

    /// <summary>
    /// Shorthand for <c>[ExpressiveFor(typeof(ContainingType), memberName)]</c> —
    /// use when the target member is on the same type as the stub.
    /// </summary>
    public ExpressiveForAttribute(string memberName)
    {
        TargetType = null;
        MemberName = memberName;
    }
}
