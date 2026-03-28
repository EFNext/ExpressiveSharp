using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Wraps the provider's <see cref="RelationalParameterBasedSqlProcessor"/> to temporarily
/// hide <see cref="WindowFunctionSqlExpression"/> nodes during nullability processing.
/// The provider's own processor (with all its provider-specific customizations) handles
/// the actual work — we only intercept the public entry points to wrap/unwrap.
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for custom SQL expression nullability processing")]
internal sealed class WindowFunctionParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
{
    private readonly RelationalParameterBasedSqlProcessor _inner;

#if NET10_0_OR_GREATER
    public WindowFunctionParameterBasedSqlProcessor(
        RelationalParameterBasedSqlProcessor inner,
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
        _inner = inner;
    }

    public override Expression Process(
        Expression queryExpression,
        ParametersCacheDecorator parametersDecorator)
    {
        var wrapped = WindowFunctionSqlExpressionWrapper.WrapAll(queryExpression, out var stash);
        var processed = _inner.Process(wrapped, parametersDecorator);
        return WindowFunctionSqlExpressionWrapper.UnwrapAll(processed, stash);
    }
#else
    public WindowFunctionParameterBasedSqlProcessor(
        RelationalParameterBasedSqlProcessor inner,
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
        _inner = inner;
    }
#endif

#if !NET10_0_OR_GREATER
    public override Expression Optimize(
        Expression queryExpression,
        IReadOnlyDictionary<string, object?> parametersValues,
        out bool canCache)
    {
        var wrapped = WindowFunctionSqlExpressionWrapper.WrapAll(queryExpression, out var stash);
        var processed = _inner.Optimize(wrapped, parametersValues, out canCache);
        return WindowFunctionSqlExpressionWrapper.UnwrapAll(processed, stash);
    }
#endif
}
