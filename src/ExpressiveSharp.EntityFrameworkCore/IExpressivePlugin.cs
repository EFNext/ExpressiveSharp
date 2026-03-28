using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Interface for ExpressiveSharp plugins that register additional services into
/// the EF Core service provider. Plugins are registered via
/// <see cref="ExpressiveOptionsBuilder.AddPlugin"/>.
/// </summary>
public interface IExpressivePlugin
{
    /// <summary>
    /// Registers services into the EF Core internal service collection.
    /// Called during <c>UseExpressives()</c> initialization.
    /// </summary>
    void ApplyServices(IServiceCollection services);

    /// <summary>
    /// Returns expression tree transformers to add to the EF Core transformer pipeline.
    /// Called when building <see cref="Services.ExpressiveOptions"/>.
    /// Default implementation returns an empty array.
    /// </summary>
    IExpressionTreeTransformer[] GetTransformers() => [];
}
