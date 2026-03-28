using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Interface for ExpressiveSharp plugins that register additional services into
/// the EF Core service provider. Plugins are discovered automatically via
/// <see cref="ExpressivePluginAttribute"/> on their containing assembly.
/// </summary>
public interface IExpressivePlugin
{
    /// <summary>
    /// Registers services into the EF Core internal service collection.
    /// Called during <c>UseExpressives()</c> initialization.
    /// </summary>
    void ApplyServices(IServiceCollection services);
}
