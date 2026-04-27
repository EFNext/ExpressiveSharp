namespace ExpressiveSharp.Mapping;

/// <summary>
/// Declares a synthesized, settable property on the containing partial type whose read-side
/// formula is supplied by the decorated stub. The generator emits a property named
/// <see cref="TargetName"/> backed by a private field; read-access evaluates the stub formula
/// until a value is materialized (e.g. by EF Core, HotChocolate <c>[UseProjection]</c>, or
/// AutoMapper <c>ProjectTo</c>), after which the stored value wins.
/// </summary>
/// <remarks>
/// <para>The stub must be an <b>instance property</b> with an <b>expression body</b>
/// (<c>=&gt; expr</c>). Method stubs, accessor-list forms, and static stubs are rejected.</para>
/// <para>The containing type must be declared <c>partial</c>.</para>
/// <para>To map a stub onto an <i>existing</i> member instead of synthesizing a new one,
/// use <see cref="ExpressiveForAttribute"/>.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ExpressivePropertyAttribute : Attribute
{
    /// <summary>
    /// Name of the property to synthesize. Must be a string literal — <c>nameof(X)</c> cannot
    /// resolve because <c>X</c> doesn't exist yet.
    /// </summary>
    public string TargetName { get; }

    /// <summary>
    /// <see cref="IExpressionTreeTransformer"/> types to apply at runtime.
    /// Each type must have a parameterless constructor.
    /// </summary>
    public Type[]? Transformers { get; set; }

    public ExpressivePropertyAttribute(string targetName)
    {
        TargetName = targetName;
    }
}
