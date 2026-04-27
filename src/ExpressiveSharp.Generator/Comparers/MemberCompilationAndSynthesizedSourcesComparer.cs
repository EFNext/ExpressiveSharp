using System.Collections.Immutable;
using ExpressiveSharp.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Comparers;

// Wraps MemberDeclarationSyntaxAndCompilationEqualityComparer with the synthesized-source array
// so caching invalidates correctly when an [ExpressiveProperty] attribute changes anywhere —
// the augmented compilation used for binding depends on it.
internal sealed class MemberCompilationAndSynthesizedSourcesComparer
    : IEqualityComparer<(((MemberDeclarationSyntax Member, ExpressiveAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right)>
{
    public readonly static MemberCompilationAndSynthesizedSourcesComparer Instance = new();

    private static readonly MemberDeclarationSyntaxAndCompilationEqualityComparer _inner = new();

    private MemberCompilationAndSynthesizedSourcesComparer() { }

    public bool Equals(
        (((MemberDeclarationSyntax Member, ExpressiveAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right) x,
        (((MemberDeclarationSyntax Member, ExpressiveAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right) y)
        => _inner.Equals(x.Left, y.Left)
            && SynthesizedSourceArrayComparer.Instance.Equals(x.Right, y.Right);

    public int GetHashCode(
        (((MemberDeclarationSyntax Member, ExpressiveAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right) obj)
    {
        unchecked
        {
            return _inner.GetHashCode(obj.Left) * 31
                + SynthesizedSourceArrayComparer.Instance.GetHashCode(obj.Right);
        }
    }
}
