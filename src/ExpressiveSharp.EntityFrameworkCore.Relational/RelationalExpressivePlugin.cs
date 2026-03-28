using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;
using ExpressiveSharp.EntityFrameworkCore.Relational.Transformers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExpressiveSharp.EntityFrameworkCore.Relational;

/// <summary>
/// Plugin that registers window function services into the EF Core service provider.
/// Auto-discovered via <see cref="ExpressivePluginAttribute"/> when the assembly is loaded.
/// </summary>
/// <summary>
/// Plugin that registers window function services into the EF Core service provider.
/// Activated via <c>.UseExpressives(o => o.UseRelationalExtensions())</c>.
/// </summary>
public sealed class RelationalExpressivePlugin : IExpressivePlugin
{
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to decorate EF Core services")]
    public void ApplyServices(IServiceCollection services)
    {
        // Register method call translator plugin (scoped — matches EF Core's service lifetimes)
        services.AddScoped<IMethodCallTranslatorPlugin, WindowFunctionTranslatorPlugin>();

        // Register evaluatable expression filter
        services.AddSingleton<IEvaluatableExpressionFilterPlugin, WindowFunctionEvaluatableExpressionFilter>();

        // Replace the SQL nullability processor factory to handle WindowFunctionSqlExpression
        services.Replace(ServiceDescriptor.Scoped<IRelationalParameterBasedSqlProcessorFactory,
            WindowFunctionParameterBasedSqlProcessorFactory>());

        // Replace the SQL generator factory to render WindowFunctionSqlExpression
        DecorateQuerySqlGeneratorFactory(services);
    }

    public IExpressionTreeTransformer[] GetTransformers() =>
        [new RewriteIndexedSelectToRowNumber()];

    private static void DecorateQuerySqlGeneratorFactory(IServiceCollection services)
    {
        var targetDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IQuerySqlGeneratorFactory));
        if (targetDescriptor is null) return;

        services.Replace(ServiceDescriptor.Describe(
            typeof(IQuerySqlGeneratorFactory),
            sp =>
            {
                IQuerySqlGeneratorFactory inner;
                if (targetDescriptor.ImplementationInstance is IQuerySqlGeneratorFactory inst)
                    inner = inst;
                else if (targetDescriptor.ImplementationFactory is not null)
                    inner = (IQuerySqlGeneratorFactory)targetDescriptor.ImplementationFactory(sp);
                else
                {
                    Debug.Assert(targetDescriptor.ImplementationType is not null);
                    inner = (IQuerySqlGeneratorFactory)ActivatorUtilities.CreateInstance(
                        sp, targetDescriptor.ImplementationType!);
                }

                var dependencies = sp.GetRequiredService<QuerySqlGeneratorDependencies>();
                return new ExpressiveRelationalQuerySqlGeneratorFactory(inner, dependencies);
            },
            targetDescriptor.Lifetime));
    }
}
