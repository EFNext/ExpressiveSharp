using System;
using System.Reflection;
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(ExpressiveSharp.Services.ExpressiveHotReloadHandler))]

namespace ExpressiveSharp.Services;

internal static class ExpressiveHotReloadHandler
{
    public static void ClearCache(Type[]? updatedTypes)
    {
        ResetGeneratedRegistries();
        ExpressiveResolver.ClearCachesForMetadataUpdate();
        ExpressiveReplacer.ClearCachesForMetadataUpdate();
    }

    public static void UpdateApplication(Type[]? updatedTypes) => ClearCache(updatedTypes);

    /// <summary>
    /// Finds every loaded assembly's generated <c>ExpressiveSharp.Generated.ExpressionRegistry</c>
    /// class and invokes its <c>ResetMap()</c> method so the next <c>TryGet</c> rebuilds
    /// <c>LambdaExpression</c> instances from the hot-reloaded factory IL.
    /// </summary>
    private static void ResetGeneratedRegistries()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
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
