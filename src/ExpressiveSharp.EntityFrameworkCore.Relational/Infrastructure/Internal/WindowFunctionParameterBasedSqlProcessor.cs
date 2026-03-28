using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

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
        var processor = new WindowFunctionSqlNullabilityProcessor(Dependencies, Parameters);
        return processor.Process(queryExpression, parametersDecorator);
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
        var processor = new WindowFunctionSqlNullabilityProcessor(Dependencies, UseRelationalNulls);
        return processor.Process(queryExpression, parametersValues, out canCache);
    }
#endif
}
