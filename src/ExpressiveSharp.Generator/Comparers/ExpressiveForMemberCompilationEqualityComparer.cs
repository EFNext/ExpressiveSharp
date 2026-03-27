using System.Runtime.CompilerServices;
using ExpressiveSharp.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Comparers;

/// <summary>
/// Equality comparer for [ExpressiveFor] pipeline tuples,
/// mirroring <see cref="MemberDeclarationSyntaxAndCompilationEqualityComparer"/> for the standard pipeline.
/// </summary>
internal class ExpressiveForMemberCompilationEqualityComparer
    : IEqualityComparer<((MethodDeclarationSyntax Method, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation)>
{
    private readonly static MemberDeclarationSyntaxEqualityComparer _memberComparer = new();

    public bool Equals(
        ((MethodDeclarationSyntax Method, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) x,
        ((MethodDeclarationSyntax Method, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) y)
    {
        var (xLeft, xCompilation) = x;
        var (yLeft, yCompilation) = y;

        if (ReferenceEquals(xLeft.Method, yLeft.Method) &&
            ReferenceEquals(xCompilation, yCompilation) &&
            xLeft.GlobalOptions == yLeft.GlobalOptions)
        {
            return true;
        }

        if (!ReferenceEquals(xLeft.Method.SyntaxTree, yLeft.Method.SyntaxTree))
        {
            return false;
        }

        if (xLeft.Attribute != yLeft.Attribute)
        {
            return false;
        }

        if (xLeft.GlobalOptions != yLeft.GlobalOptions)
        {
            return false;
        }

        if (!_memberComparer.Equals(xLeft.Method, yLeft.Method))
        {
            return false;
        }

        return xCompilation.ExternalReferences.SequenceEqual(yCompilation.ExternalReferences);
    }

    public int GetHashCode(((MethodDeclarationSyntax Method, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) obj)
    {
        var (left, compilation) = obj;
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + _memberComparer.GetHashCode(left.Method);
            hash = hash * 31 + RuntimeHelpers.GetHashCode(left.Method.SyntaxTree);
            hash = hash * 31 + left.Attribute.GetHashCode();
            hash = hash * 31 + left.GlobalOptions.GetHashCode();

            var references = compilation.ExternalReferences;
            var referencesHash = 17;
            referencesHash = referencesHash * 31 + references.Length;
            foreach (var reference in references)
            {
                referencesHash = referencesHash * 31 + RuntimeHelpers.GetHashCode(reference);
            }
            hash = hash * 31 + referencesHash;

            return hash;
        }
    }
}
