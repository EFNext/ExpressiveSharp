using System.Diagnostics.CodeAnalysis;
using ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;
using ExpressiveSharp.EntityFrameworkCore.Relational.Transformers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExpressiveSharp.EntityFrameworkCore.Relational;

/// <summary>
/// Plugin that registers window function services into the EF Core service provider.
/// Activated via <c>.UseExpressives(o => o.UseRelationalExtensions())</c>.
/// </summary>
public sealed class RelationalExpressivePlugin : IExpressivePlugin
{
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to replace EF Core services")]
    public void ApplyServices(IServiceCollection services)
    {
        // Register method call translator plugin (scoped — matches EF Core's service lifetimes)
        services.AddScoped<IMethodCallTranslatorPlugin, WindowFunctionTranslatorPlugin>();

        // Register evaluatable expression filter
        services.AddSingleton<IEvaluatableExpressionFilterPlugin, WindowFunctionEvaluatableExpressionFilter>();

        // Replace the SQL nullability processor factory to handle WindowFunctionSqlExpression
        services.Replace(ServiceDescriptor.Scoped<IRelationalParameterBasedSqlProcessorFactory,
            WindowFunctionParameterBasedSqlProcessorFactory>());
    }

    public IExpressionTreeTransformer[] GetTransformers() =>
        [new RewriteIndexedSelectToRowNumber()];
}
