using System.Collections.Generic;
using System.Linq;
using ExpressiveSharp.Generator.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class OrderedChainBindingTests : GeneratorTestBase
{
    private const string IncludableStubs = """
        namespace ExpressiveSharp.EntityFrameworkCore
        {
            public interface IIncludableExpressiveQueryable<TEntity, TProperty>
                : ExpressiveSharp.IExpressiveQueryable<TEntity>
                where TEntity : class
            {
            }

            public static class IncludeStubs
            {
                public static IIncludableExpressiveQueryable<TEntity, TProperty> Include<TEntity, TProperty>(
                    this ExpressiveSharp.IExpressiveQueryable<TEntity> source,
                    System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> navigationPropertyPath)
                    where TEntity : class
                    => null!;
            }
        }
        """;

    private static string Query(string chain) => $$"""
        using System.Linq;
        using ExpressiveSharp;
        using ExpressiveSharp.EntityFrameworkCore;

        namespace TestNs
        {
            public class Order
            {
                public int Id { get; set; }
                public string Name { get; set; } = "";
                public Customer Customer { get; set; } = new();
            }

            public class Customer
            {
                public int Id { get; set; }
            }

            public class TestClass
            {
                public void Run(IQueryable<Order> query)
                {
                    query.AsExpressive(){{chain}}.ToList();
                }
            }
        }
        """;

    private static List<Diagnostic> Errors(Compilation compilation) =>
        compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

    [TestMethod]
    public void OrderBy_ThenBy_Compiles()
    {
        var compilation = CreateCompilation([Query(".OrderBy(o => o.Id).ThenBy(o => o.Name)"), IncludableStubs]);

        Assert.AreEqual(0, Errors(compilation).Count);
    }

    [TestMethod]
    public void ThenBy_WithoutOrderBy_DoesNotCompile()
    {
        var compilation = CreateCompilation([Query(".Where(o => o.Id > 0).ThenBy(o => o.Name)"), IncludableStubs]);

        var errors = Errors(compilation);

        Assert.IsTrue(errors.Count > 0);
        Assert.IsTrue(errors.All(e => e.GetMessage().Contains("ThenBy")));
    }

    [TestMethod]
    public void ThenBy_AfterChainContinuityPassthrough_DoesNotCompile()
    {
        var compilation = CreateCompilation([Query(".OrderBy(o => o.Id).Take(5).ThenBy(o => o.Name)"), IncludableStubs]);

        var errors = Errors(compilation);

        Assert.IsTrue(errors.Count > 0);
        Assert.IsTrue(errors.All(e => e.GetMessage().Contains("ThenBy")));
    }

    [TestMethod]
    public void ThenBy_AfterIncludeFollowingOrderBy_DoesNotCompile()
    {
        var compilation = CreateCompilation(
            [Query(".OrderBy(o => o.Id).Include(o => o.Customer).ThenBy(o => o.Name)"), IncludableStubs]);

        var errors = Errors(compilation);

        Assert.IsTrue(errors.Count > 0);
        Assert.IsTrue(errors.All(e => e.GetMessage().Contains("ThenBy")));
    }

    [TestMethod]
    public void Include_OrderBy_ThenBy_Compiles()
    {
        var compilation = CreateCompilation(
            [Query(".Include(o => o.Customer).OrderBy(o => o.Id).ThenBy(o => o.Name)"), IncludableStubs]);

        Assert.AreEqual(0, Errors(compilation).Count);
    }
}
