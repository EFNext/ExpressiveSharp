using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ExpressiveSharp.Services;
using ExpressiveSharp.Transformers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Registers ExpressiveSharp services into the EF Core service provider:
/// conventions for model building and a query compiler decorator for automatic expansion.
/// </summary>
public class ExpressiveOptionsExtension : IDbContextOptionsExtension
{
    private readonly IReadOnlyList<IExpressivePlugin> _plugins;
    private readonly bool _preserveThrowExpressions;
    private readonly int _pluginHash;

    public ExpressiveOptionsExtension(IReadOnlyList<IExpressivePlugin> plugins, bool preserveThrowExpressions = false)
    {
        _plugins = plugins;
        _preserveThrowExpressions = preserveThrowExpressions;

        var hash = new HashCode();
        foreach (var plugin in plugins)
        {
            hash.Add(plugin.GetType().FullName);
            hash.Add(plugin.GetHashCode());
        }
        hash.Add(preserveThrowExpressions);
        _pluginHash = hash.ToHashCode();

        Info = new ExtensionInfo(this);
    }

    public DbContextOptionsExtensionInfo Info { get; }

    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to decorate query compiler")]
    public void ApplyServices(IServiceCollection services)
    {
        // Register conventions
        services.AddScoped<IConventionSetPlugin, ExpressiveDbSetDiscoveryConventionPlugin>();
        services.AddScoped<IConventionSetPlugin, ExpressivePropertiesNotMappedConventionPlugin>();
        services.AddScoped<IConventionSetPlugin, ExpressiveExpandQueryFiltersConventionPlugin>();

        // Decorate IQueryCompiler with ExpressiveQueryCompiler
        var targetDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IQueryCompiler));

        var decoratorFactory = ActivatorUtilities.CreateFactory(
            typeof(ExpressiveQueryCompiler), [typeof(IQueryCompiler)]);

        if (targetDescriptor is not null)
        {
            // Provider registered first (e.g. UseSqlite().UseExpressives()) — decorate immediately
            services.Replace(ServiceDescriptor.Describe(
                targetDescriptor.ServiceType,
                serviceProvider => decoratorFactory(serviceProvider, [CreateTargetInstance(serviceProvider, targetDescriptor)]),
                targetDescriptor.Lifetime));
        }
        else
        {
            // UseExpressives() called before provider (e.g. AddSqlite optionsAction) — deferred decoration.
            // Pre-register IQueryCompiler so the provider's TryAdd becomes a no-op.
            // At resolution time, all provider services (IDatabase, IModel, etc.) are available.
            services.AddScoped<IQueryCompiler>(sp =>
                (IQueryCompiler)decoratorFactory(sp, [ActivatorUtilities.CreateInstance<QueryCompiler>(sp)]));
        }

        // Apply plugin services
        foreach (var plugin in _plugins)
            plugin.ApplyServices(services);

        // Collect plugin transformers (captured by value in the closure)
        var extraTransformers = _plugins
            .SelectMany(p => p.GetTransformers())
            .ToArray();

        // Register a dedicated ExpressiveOptions instance with EF Core transformers
        // plus any transformers contributed by plugins
        var preserveThrow = _preserveThrowExpressions;
        services.AddSingleton(sp =>
        {
            var options = new ExpressiveOptions();
            if (!preserveThrow)
                options.AddTransformers(new ReplaceThrowWithDefault());
            options.AddTransformers(
                new ConvertLoopsToLinq(),
                new RemoveNullConditionalPatterns(),
                new FlattenTupleComparisons(),
                new FlattenConcatArrayCalls(),
                new FlattenBlockExpressions());
            if (extraTransformers.Length > 0)
                options.AddTransformers(extraTransformers);
            return options;
        });
    }

    public void Validate(IDbContextOptions options)
    {
    }

    private static object CreateTargetInstance(IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance is not null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory is not null)
            return descriptor.ImplementationFactory(services);

        Debug.Assert(descriptor.ImplementationType is not null);
        return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType!);
    }

    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension) { }

        public override bool IsDatabaseProvider => false;
        public override string LogFragment => "UseExpressives ";

        public override int GetServiceProviderHashCode()
            => ((ExpressiveOptionsExtension)Extension)._pluginHash;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo otherInfo
               && ((ExpressiveOptionsExtension)otherInfo.Extension)._pluginHash
                  == ((ExpressiveOptionsExtension)Extension)._pluginHash;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo["Expressives:Enabled"] = "true";
    }
}
