using System;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace ExpressiveSharp.Diagnostics;

[EventSource(Name = ExpressiveDiagnostics.SourceName)]
internal sealed class ExpressiveEventSource : EventSource
{
    public static readonly ExpressiveEventSource Log = new();

    private ExpressiveEventSource() { }

    [Event(1, Level = EventLevel.Error,
        Message = "ExpressiveSharp registry initialization failed for assembly '{0}': {1}: {2}")]
    public void RegistryInitializationFailed(string assemblyName, string exceptionType, string message)
        => WriteEvent(1, assemblyName, exceptionType, message);

    [NonEvent]
    public void RegistryInitializationFailed(Assembly assembly, Exception exception)
    {
        if (!IsEnabled()) return;
        RegistryInitializationFailed(
            assembly.GetName().Name ?? "<unknown>",
            exception.GetType().FullName ?? exception.GetType().Name,
            exception.Message);
    }

    [Event(2, Level = EventLevel.Warning,
        Message = "ExpressiveSharp hot-reload registry reset failed for assembly '{0}': {1}: {2}")]
    public void HotReloadResetFailed(string assemblyName, string exceptionType, string message)
        => WriteEvent(2, assemblyName, exceptionType, message);

    [NonEvent]
    public void HotReloadResetFailed(Assembly assembly, Exception exception)
    {
        if (!IsEnabled()) return;
        HotReloadResetFailed(
            assembly.GetName().Name ?? "<unknown>",
            exception.GetType().FullName ?? exception.GetType().Name,
            exception.Message);
    }

    [Event(3, Level = EventLevel.Error,
        Message = "Multiple [ExpressiveFor] mappings found for '{0}' in assemblies '{1}' and '{2}'")]
    public void MultipleExpressiveForMappings(string memberInfoString, string firstAssembly, string secondAssembly)
        => WriteEvent(3, memberInfoString, firstAssembly, secondAssembly);

    [NonEvent]
    public void MultipleExpressiveForMappings(MemberInfo memberInfo, Assembly first, Assembly second)
    {
        if (!IsEnabled()) return;
        MultipleExpressiveForMappings(
            memberInfo.ToString() ?? memberInfo.Name,
            first.GetName().Name ?? "<unknown>",
            second.GetName().Name ?? "<unknown>");
    }
}
