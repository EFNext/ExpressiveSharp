using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Convention that discovers <see cref="ExpressiveDbSet{TEntity}"/> properties on the DbContext
/// and registers their entity types in the model with the property name as the table name.
/// Mirrors EF Core's built-in <c>DbSetFindingConvention</c> + <c>TableNameFromDbSetConvention</c>
/// for <see cref="DbSet{TEntity}"/> subclasses.
/// </summary>
public class ExpressiveDbSetDiscoveryConvention : IModelInitializedConvention, IEntityTypeAddedConvention
{
    private readonly Dictionary<Type, string> _sets;

    public ExpressiveDbSetDiscoveryConvention(Type contextType)
    {
        _sets = new Dictionary<Type, string>();

        foreach (var property in contextType.GetRuntimeProperties())
        {
            if (property.GetMethod?.IsStatic == true)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            if (property.DeclaringType == typeof(DbContext))
                continue;

            var propertyType = property.PropertyType;
            if (!propertyType.IsGenericType)
                continue;
            // Skip exact DbSet<> — already handled by EF Core's DbSetFindingConvention
            if (propertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                continue;

            var dbSetBase = FindDbSetBase(propertyType);
            if (dbSetBase is null)
                continue;

            var entityType = dbSetBase.GenericTypeArguments[0];
            _sets.TryAdd(entityType, property.Name);
        }
    }

    public void ProcessModelInitialized(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        // Mirror DbSetFindingConvention: shouldBeOwned = false, fromDataAnnotation = true
        foreach (var entityType in _sets.Keys)
            modelBuilder.Entity(entityType, shouldBeOwned: false, fromDataAnnotation: true);
    }

    public void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        // Set the table name via the relational annotation (same key that ToTable() uses).
        // This avoids a hard dependency on Microsoft.EntityFrameworkCore.Relational while
        // still giving relational providers the correct table name.
        // Only set if not already mapped by a regular DbSet<T> property.
        var clrType = entityTypeBuilder.Metadata.ClrType;
        if (_sets.TryGetValue(clrType, out var tableName)
            && entityTypeBuilder.Metadata.FindAnnotation("Relational:TableName") is null)
        {
            entityTypeBuilder.HasAnnotation("Relational:TableName", tableName);
        }
    }

    private static Type? FindDbSetBase(Type type)
    {
        var current = type.BaseType;
        while (current is not null)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(DbSet<>))
                return current;
            current = current.BaseType;
        }
        return null;
    }
}

public class ExpressiveDbSetDiscoveryConventionPlugin : IConventionSetPlugin
{
    private readonly ProviderConventionSetBuilderDependencies _dependencies;

    public ExpressiveDbSetDiscoveryConventionPlugin(ProviderConventionSetBuilderDependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        var convention = new ExpressiveDbSetDiscoveryConvention(_dependencies.ContextType);
        conventionSet.ModelInitializedConventions.Add(convention);
        conventionSet.EntityTypeAddedConventions.Add(convention);
        return conventionSet;
    }
}
