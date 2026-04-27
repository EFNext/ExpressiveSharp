using System.Reflection;
using ExpressiveSharp.Mapping;
using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Marks properties as unmapped when they have no backing column: decorated with
/// <see cref="ExpressiveAttribute"/> or <see cref="ExpressiveForAttribute"/>, or the target
/// of an <see cref="ExpressiveForAttribute"/> stub elsewhere in the solution.
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
