namespace ExpressiveSharp.Generator.Models;

/// <summary>
/// Side-information attached to an <see cref="ExpressiveDescriptor"/> when
/// <c>[ExpressiveFor(..., Synthesize = true)]</c> is applied. Instructs the generator to emit
/// an additional partial-class file declaring the synthesized property on the target type.
/// </summary>
internal sealed class SynthesizedPropertySpec
{
    /// <summary>Fully-qualified property type (e.g. <c>decimal</c>, <c>string?</c>, <c>global::System.Nullable&lt;decimal&gt;</c>).</summary>
    public string PropertyTypeFqn { get; set; } = "";

    /// <summary>The synthesized property's name (the first argument of <c>[ExpressiveFor(nameof(X))]</c>).</summary>
    public string PropertyName { get; set; } = "";

    /// <summary>Name of the stub member whose body produces the formula (called from the getter's fallback branch).</summary>
    public string StubMemberName { get; set; } = "";

    /// <summary><c>true</c> when the stub is a method (<c>AmountExpression()</c>); <c>false</c> when it is a property (<c>AmountExpression</c>).</summary>
    public bool StubIsMethod { get; set; }

    /// <summary>When <c>true</c>, emit the ternary+flag shape. When <c>false</c>, emit the coalesce shape.</summary>
    public bool UseTernaryShape { get; set; }

    /// <summary>
    /// Type to use for the backing field. For coalesce: nullable form of <see cref="PropertyTypeFqn"/>.
    /// For ternary: same as <see cref="PropertyTypeFqn"/>.
    /// </summary>
    public string BackingFieldTypeFqn { get; set; } = "";

    /// <summary>Name of the containing class (e.g. <c>Account</c>).</summary>
    public string ContainingTypeName { get; set; } = "";

    /// <summary>Containing class's namespace, or null for the global namespace.</summary>
    public string? ContainingTypeNamespace { get; set; }

    /// <summary>Containing type path from outermost to target (for nested types).</summary>
    public IReadOnlyList<string> ContainingTypePath { get; set; } = System.Array.Empty<string>();

    /// <summary>Keyword for the containing type declaration (<c>class</c>, <c>record</c>, <c>struct</c>, etc.).</summary>
    public string ContainingTypeKeyword { get; set; } = "class";
}
