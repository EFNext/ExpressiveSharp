using System.Linq.Expressions;
using ExpressiveSharp.FunctionalTests.Infrastructure;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class TranslationTestBase
{
    private IExpressionConsumer _consumer = null!;

    protected abstract IExpressionConsumer CreateConsumer();

    [TestInitialize]
    public void SetUp() => _consumer = CreateConsumer();

    [TestCleanup]
    public void TearDown() => _consumer.Dispose();

    protected Task AssertWhereTranslates<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
        => _consumer.AssertWhereTranslatesAsync(predicate);

    protected Task AssertSelectTranslates<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class
        => _consumer.AssertSelectTranslatesAsync(selector);

    protected Task AssertOrderByTranslates<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
        => _consumer.AssertOrderByTranslatesAsync(keySelector);
}
