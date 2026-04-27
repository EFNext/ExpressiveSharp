using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Comparers;

internal class MemberDeclarationSyntaxEqualityComparer : IEqualityComparer<MemberDeclarationSyntax>
{
    public bool Equals(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        // Roslyn reuses SyntaxTree objects for unchanged files, so a new SyntaxTree
        // means the file was edited even if this specific node text looks the same.
        if (!ReferenceEquals(x.SyntaxTree, y.SyntaxTree))
        {
            return false;
        }

        if (x.RawKind != y.RawKind)
        {
            return false;
        }

        if (x.FullSpan.Length != y.FullSpan.Length)
        {
            return false;
        }

        return x.IsEquivalentTo(y);
    }

    public int GetHashCode(MemberDeclarationSyntax obj)
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + RuntimeHelpers.GetHashCode(obj.SyntaxTree);
            hash = hash * 31 + obj.RawKind;
            hash = hash * 31 + obj.FullSpan.Length;
            return hash;
        }
    }
}
