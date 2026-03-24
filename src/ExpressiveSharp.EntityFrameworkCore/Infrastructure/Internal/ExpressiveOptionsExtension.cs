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
    public ExpressiveOptionsExtension()
    {
        Info = new ExtensionInfo(this);
    }

    public DbContextOptionsExtensionInfo Info { get; }

    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to decorate query compiler")]
    public void ApplyServices(IServiceCollection services)
    {
        // Register conventions
        services.AddScoped<IConventionSetPlugin, ExpressivePropertiesNotMappedConventionPlugin>();
        services.AddScoped<IConventionSetPlugin, ExpressiveExpandQueryFiltersConventionPlugin>();

        // Decorate IQueryCompiler with ExpressiveQueryCompiler
        var targetDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IQueryCompiler))
            ?? throw new InvalidOperationException(
                "No QueryCompiler is configured. Ensure a database provider is configured before calling UseExpressives().");

        var decoratorFactory = ActivatorUtilities.CreateFactory(
            typeof(ExpressiveQueryCompiler), [targetDescriptor.ServiceType]);

        services.Replace(ServiceDescriptor.Describe(
            targetDescriptor.ServiceType,
            serviceProvider => decoratorFactory(serviceProvider, [CreateTargetInstance(serviceProvider, targetDescriptor)]),
            targetDescriptor.Lifetime));

        // Register a dedicated ExpressiveOptions instance with EF Core transformers
        services.AddSingleton(sp =>
        {
            var options = new ExpressiveOptions();
            options.AddTransformers(
                new ConvertLoopsToLinq(),
                new RemoveNullConditionalPatterns(),
                new FlattenBlockExpressions());
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

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo["Expressives:Enabled"] = "true";
    }
}
