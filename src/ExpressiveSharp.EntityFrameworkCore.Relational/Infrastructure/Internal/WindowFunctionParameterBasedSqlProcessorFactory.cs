using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for custom SQL expression nullability processing")]
internal sealed class WindowFunctionParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
{
    private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;

    public WindowFunctionParameterBasedSqlProcessorFactory(
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
