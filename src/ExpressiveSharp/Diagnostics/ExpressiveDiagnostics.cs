using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ExpressiveSharp.Diagnostics;

public static class ExpressiveDiagnostics
{
    public const string SourceName = "ExpressiveSharp";

    internal static readonly ActivitySource ActivitySource = new(SourceName);
    internal static readonly Meter Meter = new(SourceName);

    internal static readonly Counter<long> CacheHits =
        Meter.CreateCounter<long>("expressive.cache.hits");

    internal static readonly Counter<long> CacheMisses =
        Meter.CreateCounter<long>("expressive.cache.misses");

    internal static readonly Counter<long> ReflectionFallback =
        Meter.CreateCounter<long>("expressive.reflection_fallback.count");

    internal static readonly Histogram<int> ExpansionNodeCount =
        Meter.CreateHistogram<int>("expressive.expansion.node_count");

    internal static readonly Histogram<double> ExpansionDurationMs =
        Meter.CreateHistogram<double>("expressive.expansion.duration_ms", unit: "ms");
}
