using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using ExpressiveSharp.Diagnostics;

[assembly: MetadataUpdateHandler(typeof(ExpressiveSharp.Services.ExpressiveHotReloadHandler))]

namespace ExpressiveSharp.Services;

internal static class ExpressiveHotReloadHandler
{
    public static void ClearCache(Type[]? updatedTypes)
    {
        ResetGeneratedRegistries(SelectAffectedAssemblies(updatedTypes));
        ExpressiveResolver.ClearCachesForMetadataUpdate();
        ExpressiveReplacer.ClearCachesForMetadataUpdate();
    }

    public static void UpdateApplication(Type[]? updatedTypes) => ClearCache(updatedTypes);

    // Falls back to scanning every loaded assembly when updatedTypes is null/empty,
    // which the runtime does for large or unknown change sets.
    private static IEnumerable<Assembly> SelectAffectedAssemblies(Type[]? updatedTypes)
    {
        if (updatedTypes is { Length: > 0 })
        {
            var set = new HashSet<Assembly>();
            foreach (var t in updatedTypes)
            {
                if (t is not null) set.Add(t.Assembly);
            }
            return set;
        }

        return AppDomain.CurrentDomain.GetAssemblies();
    }

    // Invokes ResetMap() on each assembly's generated ExpressionRegistry so the next
    // TryGet rebuilds LambdaExpression instances from the hot-reloaded factory IL.
    private static void ResetGeneratedRegistries(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            if (assembly.IsDynamic) continue;

            Type? registryType;
            try
            {
                registryType = assembly.GetType("ExpressiveSharp.Generated.ExpressionRegistry", throwOnError: false);
            }
            catch
            {
                continue;
            }

            var reset = registryType?.GetMethod("ResetMap", BindingFlags.Static | BindingFlags.NonPublic);
            if (reset is null) continue;

            try
            {
                reset.Invoke(null, null);
            }
            catch (Exception ex)
            {
                // Best-effort; stale registry stays stale. Surface via EventSource so
                // a hot-reload edit-and-continue failure is no longer silent.
                var inner = (ex as TargetInvocationException)?.InnerException ?? ex;
                ExpressiveEventSource.Log.HotReloadResetFailed(assembly, inner);
            }
        }
    }
}
