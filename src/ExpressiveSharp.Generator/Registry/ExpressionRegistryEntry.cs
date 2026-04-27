using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Generator.Registry;

// Primitive-only fields plus EquatableImmutableArray — safe for incremental generator caching.
sealed internal record ExpressionRegistryEntry(
    string DeclaringTypeFullName,
    ExpressionRegistryMemberType MemberKind,
    string MemberLookupName,
    string GeneratedClassFullName,
    string ExpressionMethodName,
    EquatableImmutableArray ParameterTypeNames,
    bool IsMetadataOnly = false,
    string? ClassTypeParameters = null,
    // Source location of the [ExpressiveFor] stub, or null for [Expressive] entries.
    SourceLocation? StubLocation = null
);

// Value-typed source location — safe for incremental generator caching.
readonly internal record struct SourceLocation(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);
}

// ImmutableArray<T> uses reference equality by default, which breaks Roslyn's incremental
// caching when the same logical array is produced by two different steps. Element-wise
// equality lets incremental steps be correctly cached and skipped.
readonly internal struct EquatableImmutableArray(ImmutableArray<string> array) : IEquatable<EquatableImmutableArray>
{
    private readonly ImmutableArray<string> _array = array;

    public bool Equals(EquatableImmutableArray other) => _array.SequenceEqual(other._array);

    public override bool Equals(object? obj) => obj is EquatableImmutableArray other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            foreach (var s in _array)
            {
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    public static implicit operator ImmutableArray<string>(EquatableImmutableArray e) => e._array;
    public static implicit operator EquatableImmutableArray(ImmutableArray<string> a) => new(a);
}
