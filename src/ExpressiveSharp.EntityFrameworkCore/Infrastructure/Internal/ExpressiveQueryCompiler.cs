using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// Decorates the EF Core <see cref="IQueryCompiler"/> to automatically expand
/// <see cref="ExpressiveAttribute"/> member references before query compilation.
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required to intercept query compilation")]
public sealed class ExpressiveQueryCompiler : QueryCompiler
{
    private readonly IQueryCompiler _decoratedQueryCompiler;
    private readonly ExpressiveOptions _options;

    public ExpressiveQueryCompiler(
        IQueryCompiler decoratedQueryCompiler,
        ExpressiveOptions options,
        IQueryContextFactory queryContextFactory,
        ICompiledQueryCache compiledQueryCache,
        ICompiledQueryCacheKeyGenerator compiledQueryCacheKeyGenerator,
        IDatabase database,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger,
        ICurrentDbContext currentContext,
        IEvaluatableExpressionFilter evaluatableExpressionFilter,
        IModel model)
        : base(
            queryContextFactory,
            compiledQueryCache,
            compiledQueryCacheKeyGenerator,
            database,
            logger,
            currentContext,
            evaluatableExpressionFilter,
            model)
    {
        _decoratedQueryCompiler = decoratedQueryCompiler;
        _options = options;
    }

    public override Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
        => _decoratedQueryCompiler.CreateCompiledAsyncQuery<TResult>(Expand(query));

    public override Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
        => _decoratedQueryCompiler.CreateCompiledQuery<TResult>(Expand(query));

    public override TResult Execute<TResult>(Expression query)
        => _decoratedQueryCompiler.Execute<TResult>(Expand(query));

    public override TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        => _decoratedQueryCompiler.ExecuteAsync<TResult>(Expand(query), cancellationToken);

    private Expression Expand(Expression expression)
        => expression.ExpandExpressives(_options);
}
