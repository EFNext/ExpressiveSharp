using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;
using ExpressiveSharp.Diagnostics;
using ExpressiveSharp.Services;
using ExpressiveSharp.Tests.TestFixtures;

namespace ExpressiveSharp.Tests.Diagnostics;

[TestClass]
public class ExpressiveDiagnosticsTests
{
    [TestInitialize]
    public void ResetCaches() => ExpressiveResolver.ResetAllCaches();

    [TestMethod]
    public void ExpandExpressives_WithActivityListener_RecordsActivityWithTags()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = src => src.Name == ExpressiveDiagnostics.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        Expression<Func<Product, double>> expr = p => p.Total;
        _ = expr.ExpandExpressives();

        Assert.AreEqual(1, stopped.Count);
        Assert.AreEqual("Expressive.Expand", stopped[0].OperationName);
        Assert.IsNotNull(stopped[0].GetTagItem("transformer.count"));
        Assert.IsNotNull(stopped[0].GetTagItem("expansion.node_count"));
        Assert.IsNotNull(stopped[0].GetTagItem("expansion.duration_ms"));
    }

    [TestMethod]
    public void FindGeneratedExpression_RepeatedLookup_RecordsHitAndMissCounters()
    {
        var hits = 0L;
        var misses = 0L;
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, ml) =>
        {
            if (instrument.Meter.Name != ExpressiveDiagnostics.SourceName) return;
            if (instrument.Name is "expressive.cache.hits" or "expressive.cache.misses")
                ml.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, _, _) =>
        {
            if (instrument.Name == "expressive.cache.hits") Interlocked.Add(ref hits, measurement);
            else if (instrument.Name == "expressive.cache.misses") Interlocked.Add(ref misses, measurement);
        });
        listener.Start();

        var resolver = new ExpressiveResolver();
        var member = typeof(Product).GetProperty(nameof(Product.Total))!;

        _ = resolver.FindGeneratedExpression(member);
        _ = resolver.FindGeneratedExpression(member);

        Assert.AreEqual(1, misses, "First lookup should be a cache miss");
        Assert.AreEqual(1, hits, "Second lookup should be a cache hit");
    }

    [TestMethod]
    public void FindGeneratedExpressionViaReflection_RecordsFallbackCounter()
    {
        var fallbackCount = 0L;
        var lastMemberTag = (string?)null;
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, ml) =>
        {
            if (instrument.Meter.Name == ExpressiveDiagnostics.SourceName
                && instrument.Name == "expressive.reflection_fallback.count")
                ml.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((_, measurement, tags, _) =>
        {
            Interlocked.Add(ref fallbackCount, measurement);
            foreach (var tag in tags)
            {
                if (tag.Key == "member") lastMemberTag = tag.Value as string;
            }
        });
        listener.Start();

        var member = typeof(Product).GetProperty(nameof(Product.Total))!;
        _ = ExpressiveResolver.FindGeneratedExpressionViaReflection(member);

        Assert.AreEqual(1, fallbackCount);
        Assert.IsNotNull(lastMemberTag);
        StringAssert.Contains(lastMemberTag!, nameof(Product.Total));
    }

    [TestMethod]
    public void EventSource_RegistryInitializationFailed_CapturedByListener()
    {
        using var listener = new CapturingEventListener();

        var ex = new TypeInitializationException(typeof(Product).FullName, new InvalidOperationException("boom"));
        ExpressiveEventSource.Log.RegistryInitializationFailed(typeof(Product).Assembly, ex);

        var evt = listener.Events.SingleOrDefault(e => e.EventName == nameof(ExpressiveEventSource.RegistryInitializationFailed));
        Assert.IsNotNull(evt);
        Assert.AreEqual(EventLevel.Error, evt.Level);
        Assert.AreEqual(typeof(Product).Assembly.GetName().Name, evt.Payload![0]);
    }

    [TestMethod]
    public void EventSource_HotReloadResetFailed_CapturedByListener()
    {
        using var listener = new CapturingEventListener();

        ExpressiveEventSource.Log.HotReloadResetFailed(typeof(Product).Assembly, new InvalidOperationException("oops"));

        var evt = listener.Events.SingleOrDefault(e => e.EventName == nameof(ExpressiveEventSource.HotReloadResetFailed));
        Assert.IsNotNull(evt);
        Assert.AreEqual(EventLevel.Warning, evt.Level);
    }

    [TestMethod]
    public void EventSource_MultipleExpressiveForMappings_CapturedByListener()
    {
        using var listener = new CapturingEventListener();

        var member = typeof(Product).GetProperty(nameof(Product.Total))!;
        ExpressiveEventSource.Log.MultipleExpressiveForMappings(member, typeof(Product).Assembly, typeof(string).Assembly);

        var evt = listener.Events.SingleOrDefault(e => e.EventName == nameof(ExpressiveEventSource.MultipleExpressiveForMappings));
        Assert.IsNotNull(evt);
        Assert.AreEqual(EventLevel.Error, evt.Level);
    }

    // Source name is hardcoded — EventListener's base constructor invokes OnEventSourceCreated
    // for already-existing sources before our derived ctor body runs, so any instance field would
    // still be null when the comparison happens.
    private sealed class CapturingEventListener : EventListener
    {
        public List<EventWrittenEventArgs> Events { get; } = new();

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name == ExpressiveDiagnostics.SourceName)
                EnableEvents(eventSource, EventLevel.Verbose);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventSource.Name == ExpressiveDiagnostics.SourceName)
                Events.Add(eventData);
        }
    }
}
