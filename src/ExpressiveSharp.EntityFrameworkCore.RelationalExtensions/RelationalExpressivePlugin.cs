using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Transformers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions;

/// <summary>
/// Plugin that registers window function services into the EF Core service provider.
/// Activated via <c>.UseExpressives(o => o.UseRelationalExtensions())</c>.
/// </summary>
public sealed class RelationalExpressivePlugin : IExpressivePlugin
{
    // Stable hash — this plugin is stateless, so all instances are equivalent.
    // Used by ExpressiveOptionsExtension to compute the EF Core service provider cache key.
    public override int GetHashCode() => typeof(RelationalExpressivePlugin).GetHashCode();

    public override bool Equals(object? obj) => obj is RelationalExpressivePlugin;

    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to decorate EF Core services")]
    public void ApplyServices(IServiceCollection services)
    {
        // Register method call translator plugin (scoped — matches EF Core's service lifetimes)
        services.AddScoped<IMethodCallTranslatorPlugin, WindowFunctionTranslatorPlugin>();

        // Register member translator plugin for WindowFrameBound property getters
        // (UnboundedPreceding, CurrentRow, UnboundedFollowing)
        services.AddScoped<IMemberTranslatorPlugin, WindowFunctionMemberTranslatorPlugin>();

        // Register evaluatable expression filter
        services.AddSingleton<IEvaluatableExpressionFilterPlugin, WindowFunctionEvaluatableExpressionFilter>();

        // Decorate the SQL nullability processor factory to handle WindowFunctionSqlExpression.
        // Uses the decorator pattern to preserve the provider's existing factory.
        DecorateParameterBasedSqlProcessorFactory(services);
    }

    public IExpressionTreeTransformer[] GetTransformers() =>
        [new RewriteIndexedSelectToRowNumber()];

    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to decorate EF Core services")]
    private static void DecorateParameterBasedSqlProcessorFactory(IServiceCollection services)
    {
        var targetDescriptor = services.FirstOrDefault(
            x => x.ServiceType == typeof(IRelationalParameterBasedSqlProcessorFactory));

        if (targetDescriptor is not null)
        {
            // Provider registered first — decorate immediately
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
        else
        {
            // UseExpressives() called before provider — deferred decoration.
            // Pre-register so the provider's TryAdd becomes a no-op.
            // At resolution time, create the default factory and wrap it.
            services.AddScoped<IRelationalParameterBasedSqlProcessorFactory>(sp =>
            {
                var inner = ActivatorUtilities.CreateInstance<RelationalParameterBasedSqlProcessorFactory>(sp);
                var dependencies = sp.GetRequiredService<RelationalParameterBasedSqlProcessorDependencies>();
                return new WindowFunctionParameterBasedSqlProcessorFactory(inner, dependencies);
            });
        }
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
