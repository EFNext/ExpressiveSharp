using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// Decorates the existing <see cref="IRelationalParameterBasedSqlProcessorFactory"/> to produce
/// processors that handle <see cref="WindowFunctionSqlExpression"/> during nullability optimization.
/// The original factory (provider-specific) is captured in the service registration so it can be
/// restored if this plugin is removed.
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for custom SQL expression nullability processing")]
internal sealed class WindowFunctionParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
{
    private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;

    public WindowFunctionParameterBasedSqlProcessorFactory(
        IRelationalParameterBasedSqlProcessorFactory inner,
        RelationalParameterBasedSqlProcessorDependencies dependencies)
    {
        _dependencies = dependencies;
    }

#if NET10_0_OR_GREATER
    public RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters) =>
        new WindowFunctionParameterBasedSqlProcessor(_dependencies, parameters);
#else
    public RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls) =>
        new WindowFunctionParameterBasedSqlProcessor(_dependencies, useRelationalNulls);
#endif
}
