using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to handle custom SQL expression types")]
internal sealed class WindowFunctionSqlNullabilityProcessor : SqlNullabilityProcessor
{
#if NET10_0_OR_GREATER
    public WindowFunctionSqlNullabilityProcessor(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }
#else
    public WindowFunctionSqlNullabilityProcessor(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }
#endif

    protected override SqlExpression VisitCustomSqlExpression(
        SqlExpression sqlExpression,
        bool allowOptimizedExpansion,
        out bool nullable)
    {
        if (sqlExpression is WindowFunctionSqlExpression)
        {
            // Window functions always return a non-null value
            nullable = false;
            return sqlExpression;
        }

        return base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable);
    }
}
