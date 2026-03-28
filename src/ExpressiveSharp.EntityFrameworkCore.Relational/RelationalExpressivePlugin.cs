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

        // Decorate the SQL nullability processor factory to handle WindowFunctionSqlExpression.
        // Uses the decorator pattern to preserve the provider's existing factory.
        DecorateParameterBasedSqlProcessorFactory(services);
    }

    public IExpressionTreeTransformer[] GetTransformers() =>
        [new RewriteIndexedSelectToRowNumber()];

    private static void DecorateParameterBasedSqlProcessorFactory(IServiceCollection services)
    {
        var targetDescriptor = services.FirstOrDefault(
            x => x.ServiceType == typeof(IRelationalParameterBasedSqlProcessorFactory));
        if (targetDescriptor is null) return;

        services.Replace(ServiceDescriptor.Describe(
            typeof(IRelationalParameterBasedSqlProcessorFactory),
            sp =>
            {
                var inner = CreateTargetInstance<IRelationalParameterBasedSqlProcessorFactory>(sp, targetDescriptor);
                var dependencies = sp.GetRequiredService<RelationalParameterBasedSqlProcessorDependencies>();
                return new WindowFunctionParameterBasedSqlProcessorFactory(inner, dependencies);
            },
            targetDescriptor.Lifetime));
    }

    private static T CreateTargetInstance<T>(IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance is T instance)
            return instance;

        if (descriptor.ImplementationFactory is not null)
            return (T)descriptor.ImplementationFactory(services);

        Debug.Assert(descriptor.ImplementationType is not null);
        return (T)ActivatorUtilities.CreateInstance(services, descriptor.ImplementationType!);
    }
}
