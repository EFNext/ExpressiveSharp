using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.Relational.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// Translates <see cref="WindowFunction"/> static methods into SQL window function expressions.
/// ROW_NUMBER uses the built-in <see cref="RowNumberExpression"/>;
/// RANK, DENSE_RANK, and NTILE use <see cref="WindowFunctionSqlExpression"/>.
/// </summary>
internal sealed class WindowFunctionMethodCallTranslator : IMethodCallTranslator
{
    private readonly ISqlExpressionFactory _sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource _typeMappingSource;

    public WindowFunctionMethodCallTranslator(
        ISqlExpressionFactory sqlExpressionFactory,
        IRelationalTypeMappingSource typeMappingSource)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _typeMappingSource = typeMappingSource;
    }

    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method.DeclaringType != typeof(WindowFunction))
            return null;

        var longTypeMapping = _typeMappingSource.FindMapping(typeof(long))!;

        return method.Name switch
        {
            nameof(WindowFunction.RowNumber) when arguments.Count == 1 && arguments[0] is WindowSpecSqlExpression spec
                => new RowNumberExpression(spec.Partitions, spec.Orderings, longTypeMapping),

            nameof(WindowFunction.RowNumber) when arguments.Count == 0
                => new RowNumberExpression(
                    [],
                    [new OrderingExpression(
                        _sqlExpressionFactory.Fragment("(SELECT NULL)"),
                        ascending: true)],
                    longTypeMapping),

            nameof(WindowFunction.Rank) when arguments.Count >= 1 && arguments[0] is WindowSpecSqlExpression spec
                => new WindowFunctionSqlExpression("RANK", [], spec.Partitions, spec.Orderings, typeof(long), longTypeMapping),

            nameof(WindowFunction.DenseRank) when arguments.Count >= 1 && arguments[0] is WindowSpecSqlExpression spec
                => new WindowFunctionSqlExpression("DENSE_RANK", [], spec.Partitions, spec.Orderings, typeof(long), longTypeMapping),

            nameof(WindowFunction.Ntile) when arguments.Count >= 2 && arguments[1] is WindowSpecSqlExpression spec
                => new WindowFunctionSqlExpression("NTILE",
                    [_sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0])],
                    spec.Partitions, spec.Orderings, typeof(long), longTypeMapping),

            _ => null
        };
    }
}
