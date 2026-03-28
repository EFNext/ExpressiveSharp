using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// Wraps <see cref="WindowFunctionSqlExpression"/> nodes in safe placeholders before
/// nullability processing, then restores them afterward. This lets the provider's own
/// <see cref="SqlNullabilityProcessor"/> handle all standard expressions normally.
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for custom SQL expression nullability processing")]
internal sealed class WindowFunctionParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
{
#if NET10_0_OR_GREATER
    public WindowFunctionParameterBasedSqlProcessor(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }

    protected override Expression ProcessSqlNullability(
        Expression queryExpression,
        ParametersCacheDecorator parametersDecorator)
    {
        var wrapped = WindowFunctionSqlExpressionWrapper.WrapAll(queryExpression, out var stash);
        var processed = base.ProcessSqlNullability(wrapped, parametersDecorator);
        return WindowFunctionSqlExpressionWrapper.UnwrapAll(processed, stash);
    }
#else
    public WindowFunctionParameterBasedSqlProcessor(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }

    protected override Expression ProcessSqlNullability(
        Expression queryExpression,
        IReadOnlyDictionary<string, object?> parametersValues,
        out bool canCache)
    {
        var wrapped = WindowFunctionSqlExpressionWrapper.WrapAll(queryExpression, out var stash);
        var processed = base.ProcessSqlNullability(wrapped, parametersValues, out canCache);
        return WindowFunctionSqlExpressionWrapper.UnwrapAll(processed, stash);
    }
#endif
}
