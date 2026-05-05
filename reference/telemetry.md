---
url: 'https://efnext.github.io/ExpressiveSharp/reference/telemetry.md'
---
# Telemetry

ExpressiveSharp emits runtime telemetry through the standard .NET diagnostics primitives — `ActivitySource` for distributed traces, `Meter` for metrics, and `EventSource` for previously-silent failure paths. All three share the source name `ExpressiveSharp` (also exposed as the constant `ExpressiveDiagnostics.SourceName`).

Every instrument is **zero-cost when no listener is attached**: counters and activities short-circuit on the no-listener path, and the more expensive observations (node-counting, `MemberInfo.ToString()` for tags) are guarded explicitly.

## Activity (Distributed Tracing)

A single span is emitted from the `ExpressiveSharp` `ActivitySource` (the source's name is also exposed as the constant `ExpressiveDiagnostics.SourceName`):

| Span | Emitted from | Tags |
|------|--------------|------|
| `Expressive.Expand` | `expression.ExpandExpressives()` (and the EF Core / MongoDB query compiler decorators that call into it) | `transformer.count`, `expansion.node_count`, `expansion.duration_ms` |

Because the span runs synchronously inside the EF Core query pipeline, it nests cleanly under EF Core's `Microsoft.EntityFrameworkCore` activity. Trace viewers will show:

```
EF query  →  Expressive.Expand  →  SQL execution
```

### OpenTelemetry

```csharp
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("ExpressiveSharp")
    .AddSource("Microsoft.EntityFrameworkCore") // optional — nests Expressive.Expand under EF activities
    .AddOtlpExporter()
    .Build();
```

::: tip
The `expansion.duration_ms` tag is recorded redundantly with the activity's intrinsic duration so consumers that only run a metrics pipeline (no tracer) still get the timing.
:::

## Meter (Metrics)

Five instruments on `Meter("ExpressiveSharp")`:

| Instrument | Type | Unit | Tags | What it tells you |
|------------|------|------|------|-------------------|
| `expressive.cache.hits` | Counter\<long> | — | — | The runtime resolver cache served the lookup without computing it again. |
| `expressive.cache.misses` | Counter\<long> | — | — | The resolver had to compute the lambda for a member it hadn't seen before. |
| `expressive.reflection_fallback.count` | Counter\<long> | — | `member` | The slow reflection-based fallback ran. Used for open-generic class members and generic methods not in the static registry. **Sustained activity here is a perf bug.** |
| `expressive.expansion.node_count` | Histogram\<int> | nodes | — | Total expression-tree node count after expansion + transformers. Surfaces members that explode into massive trees and bloat SQL. |
| `expressive.expansion.duration_ms` | Histogram\<double> | ms | — | Wall-clock cost of one `ExpandExpressives()` call. |

### Live monitoring with `dotnet-counters`

```sh
dotnet-counters monitor -n MyApp ExpressiveSharp
```

### OpenTelemetry

```csharp
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter("ExpressiveSharp")
    .AddOtlpExporter()
    .Build();
```

::: info
A healthy steady-state has **`cache.hits` ≫ `cache.misses`** after warmup, and **`reflection_fallback.count` should be flat** unless your code uses open-generic `[Expressive]` members. A non-trivial slope on `reflection_fallback.count` means the runtime is paying reflection costs that the static registry could have served — please file an issue with a reproducer.
:::

## EventSource (Failure Diagnostics)

Three events on `EventSource("ExpressiveSharp")`. They surface failure paths that ExpressiveSharp deliberately recovers from but that historically left users debugging by rubber duck.

| Event ID | Name | Level | Payload | When it fires |
|----------|------|-------|---------|---------------|
| `1` | `RegistryInitializationFailed` | Error | `assemblyName`, `exceptionType`, `message` | An assembly's generated `ExpressionRegistry` static constructor threw. The registry is marked inert (no `[ExpressiveFor]` lookups will succeed against it) but the process keeps running. |
| `2` | `HotReloadResetFailed` | Warning | `assemblyName`, `exceptionType`, `message` | A hot-reload edit-and-continue update arrived but invalidating the assembly's expression registry threw. The stale registry stays cached, so the next lookup may return pre-edit lambdas. |
| `3` | `MultipleExpressiveForMappings` | Error | `memberInfoString`, `firstAssembly`, `secondAssembly` | Two different assemblies both registered an `[ExpressiveFor]` mapping for the same member. An `InvalidOperationException` is thrown immediately after; the event fires first so it survives even if the exception is later caught. |

### Collecting with `dotnet-trace`

The most common path. No code changes, no app restart:

```sh
dotnet-trace collect -n MyApp --providers ExpressiveSharp::Verbose
```

Open the resulting `.nettrace` file in [PerfView](https://github.com/microsoft/perfview) or Visual Studio's diagnostic tools to inspect the events.

## Stability

The source name `ExpressiveSharp`, the instrument names, and the EventSource event IDs are part of the public API surface and follow the same stability rules as the rest of ExpressiveSharp. Tags carried on activities and counters may be extended (new tags added) but existing tags will not be renamed within a major version.
