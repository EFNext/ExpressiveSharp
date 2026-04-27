using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Plugin that registers additional services and/or expression tree transformers
/// into the EF Core service provider, registered via <see cref="ExpressiveOptionsBuilder.AddPlugin"/>.
/// </summary>
public interface IExpressivePlugin
{
    void ApplyServices(IServiceCollection services);

    IExpressionTreeTransformer[] GetTransformers() => [];
}
