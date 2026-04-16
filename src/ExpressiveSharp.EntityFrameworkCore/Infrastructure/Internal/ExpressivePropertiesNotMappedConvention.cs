using System.Reflection;
using ExpressiveSharp.Mapping;
using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Convention that marks properties as unmapped in the EF Core model when they have no backing
/// database column:
/// <list type="bullet">
///   <item>decorated with <see cref="ExpressiveAttribute"/>,</item>
///   <item>decorated with <see cref="ExpressiveForAttribute"/> (the property is a stub itself), or</item>
///   <item>the target of an <see cref="ExpressiveForAttribute"/> stub elsewhere in the solution.</item>
/// </list>
/// </summary>
public class ExpressivePropertiesNotMappedConvention : IEntityTypeAddedConvention
{
    private readonly IExpressiveResolver _resolver;

    public ExpressivePropertiesNotMappedConvention(IExpressiveResolver resolver)
    {
        _resolver = resolver;
    }

    public void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        if (entityTypeBuilder.Metadata.ClrType is null)
            return;

        foreach (var property in entityTypeBuilder.Metadata.ClrType.GetRuntimeProperties())
        {
            if (property.GetCustomAttribute<ExpressiveAttribute>() is not null
                || property.GetCustomAttribute<ExpressiveForAttribute>() is not null
                || _resolver.FindExternalExpression(property) is not null)
            {
                entityTypeBuilder.Ignore(property.Name);
            }
        }
    }
}
