using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using ExpressiveSharp.Generator.Registry;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Generator.Infrastructure;

/// <summary>A generated source file as value-equatable data, safe to cache in the pipeline.</summary>
internal readonly record struct GeneratedSource(string HintName, string Text);

/// <summary>
/// One member's bind+emit output. <see cref="Sources"/> and <see cref="RegistryEntry"/> are value-data
/// (gated by their projections); <see cref="Diagnostics"/> hold live syntax-tree locations (required for
/// <c>#pragma warning</c>/<c>.editorconfig</c> suppression) so they flow uncached and are re-reported each run.
/// </summary>
internal sealed class MemberComputation(
    ImmutableArray<GeneratedSource> sources,
    ImmutableArray<Diagnostic> diagnostics,
    ExpressionRegistryEntry? registryEntry)
{
    public ImmutableArray<GeneratedSource> Sources { get; } = sources;
    public ImmutableArray<Diagnostic> Diagnostics { get; } = diagnostics;
    public ExpressionRegistryEntry? RegistryEntry { get; } = registryEntry;
}

/// <summary>
/// Collects <c>ReportDiagnostic</c>/<c>AddSource</c> so interpretation/emission can run inside an
/// incremental <c>Select</c> (where no <see cref="SourceProductionContext"/> exists). Mirrors the
/// members the generator uses, so call sites are unchanged.
/// </summary>
internal sealed class GeneratorOutputContext
{
    private readonly ImmutableArray<Diagnostic>.Builder _diagnostics =
        ImmutableArray.CreateBuilder<Diagnostic>();
    private readonly ImmutableArray<GeneratedSource>.Builder _sources =
        ImmutableArray.CreateBuilder<GeneratedSource>();

    public GeneratorOutputContext(CancellationToken cancellationToken = default)
        => CancellationToken = cancellationToken;

    public CancellationToken CancellationToken { get; }

    public void ReportDiagnostic(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    public void AddSource(string hintName, SourceText sourceText)
        => _sources.Add(new GeneratedSource(hintName, sourceText.ToString()));

    public void AddSource(string hintName, string source)
        => _sources.Add(new GeneratedSource(hintName, source));

    public ImmutableArray<Diagnostic> Diagnostics => _diagnostics.ToImmutable();
    public ImmutableArray<GeneratedSource> Sources => _sources.ToImmutable();

    /// <summary>Replays everything collected to a real context (for pipelines that run in a source-output).</summary>
    public void FlushTo(SourceProductionContext context)
    {
        foreach (var diagnostic in _diagnostics)
            context.ReportDiagnostic(diagnostic);
        foreach (var source in _sources)
            context.AddSource(source.HintName, SourceText.From(source.Text, System.Text.Encoding.UTF8));
    }
}

/// <summary><see cref="ImmutableArray{T}"/> with element-wise value equality (the default is by reference).</summary>
internal readonly struct EquatableArray<T>(ImmutableArray<T> array) : IEquatable<EquatableArray<T>>
{
    private readonly ImmutableArray<T> _array = array;

    public ImmutableArray<T> AsImmutableArray => _array.IsDefault ? ImmutableArray<T>.Empty : _array;

    public int Length => AsImmutableArray.Length;

    public bool Equals(EquatableArray<T> other)
    {
        var self = AsImmutableArray;
        var that = other.AsImmutableArray;
        if (self.Length != that.Length)
            return false;

        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < self.Length; i++)
        {
            if (!comparer.Equals(self[i], that[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            var comparer = EqualityComparer<T>.Default;
            foreach (var item in AsImmutableArray)
                hash = hash * 31 + (item is null ? 0 : comparer.GetHashCode(item));
            return hash;
        }
    }

    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);
}
