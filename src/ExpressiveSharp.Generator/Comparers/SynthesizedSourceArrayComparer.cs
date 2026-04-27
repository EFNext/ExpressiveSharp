using System.Collections.Immutable;

namespace ExpressiveSharp.Generator.Comparers;

/// <summary>
/// Sequence equality on the synthesized (HintName, Source) array. ImmutableArray's default
/// equality is reference, which would invalidate every downstream consumer on every edit.
/// </summary>
internal sealed class SynthesizedSourceArrayComparer
    : IEqualityComparer<ImmutableArray<(string HintName, string Source)>>
{
    public readonly static SynthesizedSourceArrayComparer Instance = new();

    private SynthesizedSourceArrayComparer() { }

    public bool Equals(
        ImmutableArray<(string HintName, string Source)> x,
        ImmutableArray<(string HintName, string Source)> y)
    {
        if (x.IsDefault != y.IsDefault) return false;
        if (x.IsDefault) return true;
        if (x.Length != y.Length) return false;
        for (var i = 0; i < x.Length; i++)
        {
            if (x[i].HintName != y[i].HintName) return false;
            if (x[i].Source != y[i].Source) return false;
        }
        return true;
    }

    public int GetHashCode(ImmutableArray<(string HintName, string Source)> obj)
    {
        if (obj.IsDefault) return 0;
        unchecked
        {
            var hash = 17;
            for (var i = 0; i < obj.Length; i++)
            {
                hash = hash * 31 + (obj[i].HintName?.GetHashCode() ?? 0);
                hash = hash * 31 + (obj[i].Source?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
