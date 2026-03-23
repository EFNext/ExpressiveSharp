using ExpressiveSharp.Generator.Emitter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Models;

internal class ExpressiveDescriptor
{
    public IEnumerable<UsingDirectiveSyntax>? UsingDirectives { get; set; }

    public string? ClassNamespace { get; set; }

    public IEnumerable<string>? NestedInClassNames { get; set; }

    public string? TargetClassNamespace { get; set; }

    public IEnumerable<string>? TargetNestedInClassNames { get; set; }

    public string? ClassName { get; set; }

    public TypeParameterListSyntax? ClassTypeParameterList { get; set; }

    public SyntaxList<TypeParameterConstraintClauseSyntax>? ClassConstraintClauses { get; set; }

    public string? MemberName { get; set; }

    public string? ReturnTypeName { get; set; }

    public ParameterListSyntax? ParametersList { get; set; }

    public IEnumerable<string>? ParameterTypeNames { get; set; }

    public TypeParameterListSyntax? TypeParameterList { get; set; }

    public SyntaxList<TypeParameterConstraintClauseSyntax>? ConstraintClauses { get; set; }

    /// <summary>
    /// Contains the imperative <c>Expression.*</c> factory code
    /// that builds the expression tree.
    /// </summary>
    public EmitResult? ExpressionTreeEmission { get; set; }

    /// <summary>
    /// Fully qualified type names of transformers to apply at runtime,
    /// declared via the [Expressive] attribute's built-in flags and Transformers property.
    /// </summary>
    public List<string> DeclaredTransformerTypeNames { get; } = new();
}
