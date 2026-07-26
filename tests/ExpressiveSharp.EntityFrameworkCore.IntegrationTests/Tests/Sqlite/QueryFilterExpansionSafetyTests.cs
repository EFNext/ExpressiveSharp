using System.Linq.Expressions;
using ExpressiveSharp.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class QueryFilterExpansionSafetyTests
{
    private sealed class RootUnwrappingTransformer : IExpressionTreeTransformer
    {
        public Expression Transform(Expression expression)
            => expression is LambdaExpression lambda ? lambda.Body : expression;
    }

    private sealed class RootUnwrappingPlugin : IExpressivePlugin
    {
        public void ApplyServices(IServiceCollection services)
        {
        }

        public IExpressionTreeTransformer[] GetTransformers() => [new RootUnwrappingTransformer()];
    }

    [TestMethod]
    public async Task QueryFilter_TransformerReturningNonLambda_FailsModelBuildingLoudly()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        try
        {
            var options = new DbContextOptionsBuilder<QueryFilterTestDbContext>()
                .UseSqlite(connection)
                .UseExpressives(o => o.AddPlugin(new RootUnwrappingPlugin()))
                .Options;

            await using var context = new QueryFilterTestDbContext(options);

            var ex = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => context.Database.EnsureCreatedAsync());

            StringAssert.Contains(ex.Message, "query filter");
            StringAssert.Contains(ex.Message, "IExpressionTreeTransformer");
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
}
