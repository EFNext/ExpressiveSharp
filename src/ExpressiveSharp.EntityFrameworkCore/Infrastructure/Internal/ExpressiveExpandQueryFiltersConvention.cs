using System.Linq.Expressions;

using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Expands <see cref="ExpressiveAttribute"/> member calls within global query filters
/// at model-finalizing time.
/// </summary>
public class ExpressiveExpandQueryFiltersConvention : IModelFinalizingConvention
{
    private readonly ExpressiveOptions _options;

    public ExpressiveExpandQueryFiltersConvention(ExpressiveOptions options)
    {
        _options = options;
    }

    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
#if NET10_0_OR_GREATER
            var queryFilters = entityType.GetDeclaredQueryFilters();

            foreach (var filter in queryFilters)
            {
                if (filter.Expression is null)
                    continue;

                if (filter.Key is not null)
                    entityType.SetQueryFilter(filter.Key, Expand(filter.Expression, entityType));
                else
                    entityType.SetQueryFilter(Expand(filter.Expression, entityType));
            }
#else
            var queryFilter = entityType.GetQueryFilter();
            if (queryFilter is not null)
            {
                entityType.SetQueryFilter(Expand(queryFilter, entityType));
            }
#endif
        }
    }

    // SetQueryFilter(null) REMOVES a filter, so a non-lambda expansion result must never be
    // cast-and-forwarded: a silently dropped soft-delete/tenant filter widens data access.
    private LambdaExpression Expand(LambdaExpression filter, IConventionEntityType entityType)
    {
        var expanded = filter.ExpandExpressives(_options);
        if (expanded is not LambdaExpression lambda)
        {
            throw new InvalidOperationException(
                $"Expanding the query filter for entity type '{entityType.DisplayName()}' produced a " +
                $"{expanded.NodeType} expression instead of a lambda, which would silently remove the filter. " +
                "A registered IExpressionTreeTransformer must return a LambdaExpression when given one.");
        }

        return lambda;
    }
}
