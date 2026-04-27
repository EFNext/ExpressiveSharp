namespace ExpressiveSharp.Generator.Models;

internal sealed class SynthesizedPropertySpec
{
    public string PropertyTypeFqn { get; set; } = "";

    public string PropertyName { get; set; } = "";

    // Stub member whose body produces the formula (called from the getter's fallback branch).
    public string StubMemberName { get; set; } = "";

    // True when the stub is a method (`AmountExpression()`), false for a property (`AmountExpression`).
    public bool StubIsMethod { get; set; }

    // True → ternary+flag shape; false → coalesce shape.
    public bool UseTernaryShape { get; set; }

    // For coalesce: nullable form of PropertyTypeFqn. For ternary: same as PropertyTypeFqn.
    public string BackingFieldTypeFqn { get; set; } = "";

    public string ContainingTypeName { get; set; } = "";

    // Null for the global namespace.
    public string? ContainingTypeNamespace { get; set; }

    // Outermost to target (for nested types).
    public IReadOnlyList<string> ContainingTypePath { get; set; } = System.Array.Empty<string>();

    // Keyword (class/struct/record/record struct) for each entry in ContainingTypePath, same
    // length and ordering. The last element equals ContainingTypeKeyword.
    public IReadOnlyList<string> ContainingTypeKeywords { get; set; } = System.Array.Empty<string>();

    public string ContainingTypeKeyword { get; set; } = "class";

    // Chosen at interpret time to avoid collisions with user-declared members.
    public string BackingFieldName { get; set; } = "";

    // Ternary shape only; chosen at interpret time to avoid collisions.
    public string HasValueFlagName { get; set; } = "";
}
