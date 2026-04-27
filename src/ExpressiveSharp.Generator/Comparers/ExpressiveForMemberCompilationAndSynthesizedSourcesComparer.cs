using System.Collections.Immutable;
using ExpressiveSharp.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Comparers;

// Wraps ExpressiveForMemberCompilationEqualityComparer with the synthesized-source array.
internal sealed class ExpressiveForMemberCompilationAndSynthesizedSourcesComparer
    : IEqualityComparer<(((MemberDeclarationSyntax Member, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right)>
{
    public readonly static ExpressiveForMemberCompilationAndSynthesizedSourcesComparer Instance = new();

    private static readonly ExpressiveForMemberCompilationEqualityComparer _inner = new();

    private ExpressiveForMemberCompilationAndSynthesizedSourcesComparer() { }

    public bool Equals(
        (((MemberDeclarationSyntax Member, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right) x,
        (((MemberDeclarationSyntax Member, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right) y)
        => _inner.Equals(x.Left, y.Left)
            && SynthesizedSourceArrayComparer.Instance.Equals(x.Right, y.Right);

    public int GetHashCode(
        (((MemberDeclarationSyntax Member, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) Left, ImmutableArray<(string HintName, string Source)> Right) obj)
    {
        unchecked
        {
            return _inner.GetHashCode(obj.Left) * 31
                + SynthesizedSourceArrayComparer.Instance.GetHashCode(obj.Right);
        }
    }
}
