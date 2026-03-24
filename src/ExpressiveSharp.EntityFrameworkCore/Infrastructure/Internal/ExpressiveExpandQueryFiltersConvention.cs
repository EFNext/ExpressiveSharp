using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Convention that expands <see cref="ExpressiveAttribute"/> member calls within
/// global query filters at model-finalizing time.
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

                var expanded = filter.Expression.ExpandExpressives(_options) as LambdaExpression;

                if (filter.Key is not null)
                    entityType.SetQueryFilter(filter.Key, expanded);
                else
                    entityType.SetQueryFilter(expanded);
            }
#else
            var queryFilter = entityType.GetQueryFilter();
            if (queryFilter is not null)
            {
                entityType.SetQueryFilter(
                    queryFilter.ExpandExpressives(_options) as LambdaExpression);
            }
#endif
        }
    }
}
