using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// Decorates the provider's <see cref="IQuerySqlGeneratorFactory"/> to produce
/// <see cref="ExpressiveRelationalQuerySqlGenerator"/> instances that can render
/// <see cref="WindowFunctionSqlExpression"/> nodes.
/// </summary>
internal sealed class ExpressiveRelationalQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
    private readonly IQuerySqlGeneratorFactory _inner;
    private readonly QuerySqlGeneratorDependencies _dependencies;

    public ExpressiveRelationalQuerySqlGeneratorFactory(
        IQuerySqlGeneratorFactory inner,
        QuerySqlGeneratorDependencies dependencies)
    {
        _inner = inner;
        _dependencies = dependencies;
    }

    public QuerySqlGenerator Create() =>
        new ExpressiveRelationalQuerySqlGenerator(_dependencies);
}
