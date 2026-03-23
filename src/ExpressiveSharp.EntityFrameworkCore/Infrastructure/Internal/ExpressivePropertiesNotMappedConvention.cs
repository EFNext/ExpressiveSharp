using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Convention that marks properties decorated with <see cref="ExpressiveAttribute"/>
/// as unmapped in the EF Core model, since they have no backing database column.
/// </summary>
public class ExpressivePropertiesNotMappedConvention : IEntityTypeAddedConvention
{
    public void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        if (entityTypeBuilder.Metadata.ClrType is null)
            return;

        foreach (var property in entityTypeBuilder.Metadata.ClrType.GetRuntimeProperties())
        {
            if (property.GetCustomAttribute<ExpressiveAttribute>() is not null)
            {
                entityTypeBuilder.Ignore(property.Name);
            }
        }
    }
}
