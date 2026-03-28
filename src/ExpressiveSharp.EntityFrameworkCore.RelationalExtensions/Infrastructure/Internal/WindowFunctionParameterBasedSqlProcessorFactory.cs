using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Decorates the existing <see cref="IRelationalParameterBasedSqlProcessorFactory"/> to wrap
/// the provider's processor with <see cref="WindowFunctionSqlExpression"/> handling.
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for custom SQL expression nullability processing")]
internal sealed class WindowFunctionParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
{
    private readonly IRelationalParameterBasedSqlProcessorFactory _inner;
    private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;

    public WindowFunctionParameterBasedSqlProcessorFactory(
        IRelationalParameterBasedSqlProcessorFactory inner,
        RelationalParameterBasedSqlProcessorDependencies dependencies)
    {
        _inner = inner;
        _dependencies = dependencies;
    }

#if NET10_0_OR_GREATER
    public RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters) =>
        new WindowFunctionParameterBasedSqlProcessor(_inner.Create(parameters), _dependencies, parameters);
#else
    public RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls) =>
        new WindowFunctionParameterBasedSqlProcessor(_inner.Create(useRelationalNulls), _dependencies, useRelationalNulls);
#endif
}
