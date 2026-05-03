using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Translates <see cref="WindowFunction"/> static methods into SQL window function expressions.
/// Ranking functions (ROW_NUMBER, RANK, DENSE_RANK, NTILE) never emit a frame clause — the SQL
/// standard forbids it and SQL Server/PostgreSQL reject the syntax. Aggregate functions
/// (SUM, AVG, COUNT, MIN, MAX) propagate the frame from <see cref="WindowSpecSqlExpression"/>.
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

        ValidateArguments(method, arguments);

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

            nameof(WindowFunction.PercentRank) when arguments.Count >= 1 && arguments[0] is WindowSpecSqlExpression spec
                => new WindowFunctionSqlExpression("PERCENT_RANK", [], spec.Partitions, spec.Orderings,
                    typeof(double), _typeMappingSource.FindMapping(typeof(double))),

            nameof(WindowFunction.CumeDist) when arguments.Count >= 1 && arguments[0] is WindowSpecSqlExpression spec
                => new WindowFunctionSqlExpression("CUME_DIST", [], spec.Partitions, spec.Orderings,
                    typeof(double), _typeMappingSource.FindMapping(typeof(double))),

            nameof(WindowFunction.Sum) when ExtractAggregateArgs(arguments, out var expr, out var spec)
                => MakeAggregate("SUM", [expr], spec, method.ReturnType),

            nameof(WindowFunction.Average) when ExtractAggregateArgs(arguments, out var expr, out var spec)
                => MakeAggregate("AVG",
                    // When the C# return type differs from the expression type (int/long→double),
                    // cast the argument so SQL computes a floating-point AVG, not integer division.
                    [NeedsFloatCast(expr, method.ReturnType)
                        ? _sqlExpressionFactory.ApplyDefaultTypeMapping(
                            _sqlExpressionFactory.Convert(expr, method.ReturnType))
                        : expr],
                    spec, method.ReturnType),

            nameof(WindowFunction.Count) when arguments.Count == 1 && arguments[0] is WindowSpecSqlExpression spec
                => MakeAggregate("COUNT", [_sqlExpressionFactory.Fragment("*")], spec, typeof(int)),

            nameof(WindowFunction.Count) when ExtractAggregateArgs(arguments, out var expr, out var spec)
                => MakeAggregate("COUNT", [expr], spec, typeof(int)),

            nameof(WindowFunction.Min) when ExtractAggregateArgs(arguments, out var expr, out var spec)
                => MakeAggregate("MIN", [expr], spec, method.ReturnType),

            nameof(WindowFunction.Max) when ExtractAggregateArgs(arguments, out var expr, out var spec)
                => MakeAggregate("MAX", [expr], spec, method.ReturnType),

            nameof(WindowFunction.Lag) when ExtractNavigationArgs(arguments, out var lagArgs, out var lagSpec)
                => MakeNavigation("LAG", lagArgs, lagSpec, method.ReturnType),

            nameof(WindowFunction.Lead) when ExtractNavigationArgs(arguments, out var leadArgs, out var leadSpec)
                => MakeNavigation("LEAD", leadArgs, leadSpec, method.ReturnType),

            nameof(WindowFunction.FirstValue) when ExtractAggregateArgs(arguments, out var fvExpr, out var fvSpec)
                => MakeAggregate("FIRST_VALUE", [fvExpr], fvSpec, method.ReturnType),

            nameof(WindowFunction.LastValue) when ExtractAggregateArgs(arguments, out var lvExpr, out var lvSpec)
                => MakeAggregate("LAST_VALUE", [lvExpr], lvSpec, method.ReturnType),

            nameof(WindowFunction.NthValue) when arguments.Count >= 3 && arguments[^1] is WindowSpecSqlExpression nvSpec
                => MakeAggregate("NTH_VALUE",
                    [arguments[0], _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1])],
                    nvSpec, method.ReturnType),

            _ => null
        };
    }

    private static void ValidateArguments(MethodInfo method, IReadOnlyList<SqlExpression> arguments)
    {
        switch (method.Name)
        {
            case nameof(WindowFunction.Ntile):
                if (arguments.Count >= 1 && arguments[0] is SqlConstantExpression { Value: int n } && n <= 0)
                    throw new InvalidOperationException(
                        $"WindowFunction.Ntile requires a positive bucket count; got {n}.");
                break;

            case nameof(WindowFunction.PercentRank):
            case nameof(WindowFunction.CumeDist):
                if (arguments.Count >= 1 && arguments[0] is WindowSpecSqlExpression spec && spec.Orderings.Count == 0)
                    throw new InvalidOperationException(
                        $"WindowFunction.{method.Name} requires an ORDER BY clause on the window definition.");
                break;

            case nameof(WindowFunction.Lag):
            case nameof(WindowFunction.Lead):
                if (arguments.Count >= 3 && arguments[1] is SqlConstantExpression { Value: int offset } && offset < 0)
                    throw new InvalidOperationException(
                        $"WindowFunction.{method.Name} offset must be non-negative; got {offset}.");
                break;

            case nameof(WindowFunction.NthValue):
                if (arguments.Count >= 3 && arguments[1] is SqlConstantExpression { Value: int p } && p < 1)
                    throw new InvalidOperationException(
                        $"WindowFunction.NthValue position is 1-based; got {p}.");
                break;
        }
    }

    private static bool ExtractAggregateArgs(
        IReadOnlyList<SqlExpression> arguments,
        out SqlExpression expression,
        out WindowSpecSqlExpression spec)
    {
        if (arguments.Count >= 2 && arguments[^1] is WindowSpecSqlExpression s)
        {
            expression = arguments[0];
            spec = s;
            return true;
        }

        expression = null!;
        spec = null!;
        return false;
    }

    /// <summary>
    /// Extracts function args (all but last) and window spec (last) from a navigation call.
    /// Applies default type mapping to all function arguments.
    /// </summary>
    private bool ExtractNavigationArgs(
        IReadOnlyList<SqlExpression> arguments,
        out List<SqlExpression> funcArgs,
        out WindowSpecSqlExpression spec)
    {
        if (arguments.Count >= 2 && arguments[^1] is WindowSpecSqlExpression s)
        {
            funcArgs = [];
            for (var i = 0; i < arguments.Count - 1; i++)
                funcArgs.Add(_sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[i]));
            spec = s;
            return true;
        }

        funcArgs = null!;
        spec = null!;
        return false;
    }

    private WindowFunctionSqlExpression MakeNavigation(
        string functionName,
        IReadOnlyList<SqlExpression> funcArgs,
        WindowSpecSqlExpression spec,
        Type returnType)
    {
        var typeMapping = _typeMappingSource.FindMapping(returnType);
        return new WindowFunctionSqlExpression(
            functionName,
            funcArgs,
            spec.Partitions,
            spec.Orderings,
            returnType,
            typeMapping);
    }

    // SQL Server performs integer division for AVG(int) — cast the argument when the
    // CLR return type is floating-point but the expression is an integer type.
    private static bool NeedsFloatCast(SqlExpression expr, Type returnType) =>
        returnType == typeof(double)
        && expr.Type is var t
        && (t == typeof(int) || t == typeof(long) || t == typeof(int?) || t == typeof(long?));

    private WindowFunctionSqlExpression MakeAggregate(
        string functionName,
        IReadOnlyList<SqlExpression> funcArgs,
        WindowSpecSqlExpression spec,
        Type returnType)
    {
        var typeMapping = _typeMappingSource.FindMapping(returnType);
        return new WindowFunctionSqlExpression(
            functionName,
            funcArgs,
            spec.Partitions,
            spec.Orderings,
            returnType,
            typeMapping,
            spec.FrameType,
            spec.FrameStart,
            spec.FrameEnd);
    }
}
