using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

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

    /// <summary>
    /// When the runtime tells us which types changed, use their assemblies directly.
    /// Fall back to a full scan only when <paramref name="updatedTypes"/> is null or empty,
    /// which the runtime may do for large/unknown change sets.
    /// </summary>
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

    /// <summary>
    /// Invokes <c>ResetMap()</c> on each assembly's generated <c>ExpressionRegistry</c> class
    /// (when present) so the next <c>TryGet</c> rebuilds <c>LambdaExpression</c> instances
    /// from the hot-reloaded factory IL.
    /// </summary>
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

            try { reset.Invoke(null, null); }
            catch { /* best-effort; stale registry stays stale */ }
        }
    }
}
