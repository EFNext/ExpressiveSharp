using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

/// <summary>
/// Issue #60: C# 14 <c>extension(T) { ... }</c> members must produce well-formed lambdas
/// — the body's reference to the extension parameter must be the same
/// <see cref="ParameterExpression"/> the lambda declares.
/// </summary>
[TestClass]
public class ExtensionMemberTests
{
    [TestMethod]
    public void ExtensionMethod_NoArgs_Compiles_AndEvaluates()
    {
        var entity = new ExtEntity { Id = 4 };
        Expression<Func<ExtEntity, int>> expr = e => e.TripleId();
        var expanded = (Expression<Func<ExtEntity, int>>)expr.ExpandExpressives();

        var result = expanded.Compile()(entity);

        Assert.AreEqual(12, result);
    }

    [TestMethod]
    public void ExtensionMethod_WithExtraParameters_Compiles_AndEvaluates()
    {
        var entity = new ExtEntity { Id = 5 };
        Expression<Func<ExtEntity, int, int>> expr = (e, f) => e.Multiply(f);
        var expanded = (Expression<Func<ExtEntity, int, int>>)expr.ExpandExpressives();

        var result = expanded.Compile()(entity, 6);

        Assert.AreEqual(30, result);
    }

    [TestMethod]
    public void ExtensionMethod_OnPrimitive_Compiles_AndEvaluates()
    {
        Expression<Func<int, int>> expr = i => i.Squared();
        var expanded = (Expression<Func<int, int>>)expr.ExpandExpressives();

        var result = expanded.Compile()(9);

        Assert.AreEqual(81, result);
    }

    [TestMethod]
    public void ExtensionMethod_WithSwitchExpression_Compiles_AndEvaluates()
    {
        var entity = new ExtEntity { Id = 85 };
        Expression<Func<ExtEntity, string>> expr = e => e.GetGrade();
        var expanded = (Expression<Func<ExtEntity, string>>)expr.ExpandExpressives();

        var result = expanded.Compile()(entity);

        Assert.AreEqual("B", result);
    }

    [TestMethod]
    public void ExtensionMethod_WithBlockBody_Compiles_AndEvaluates()
    {
        var entity = new ExtEntity { Id = 5, IsActive = true };
        Expression<Func<ExtEntity, string>> expr = e => e.GetStatus();
        var expanded = (Expression<Func<ExtEntity, string>>)expr.ExpandExpressives();

        var result = expanded.Compile()(entity);

        Assert.AreEqual("Active", result);
    }

    [TestMethod]
    public void ExtensionMethod_WithMultipleReceiverReferences_Compiles_AndEvaluates()
    {
        var entity = new ExtEntity { Id = 1, Name = "foo" };
        Expression<Func<ExtEntity, string>> expr = e => e.IdAndName();
        var expanded = (Expression<Func<ExtEntity, string>>)expr.ExpandExpressives();

        var result = expanded.Compile()(entity);

        Assert.AreEqual("1: foo", result);
    }

    [TestMethod]
    public void ExtensionMethod_OnInterface_Compiles_AndEvaluates()
    {
        IExtEntity entity = new ExtEntity { Id = 3, Name = "abc" };
        Expression<Func<IExtEntity, string>> expr = e => e.GetLabel();
        var expanded = (Expression<Func<IExtEntity, string>>)expr.ExpandExpressives();

        var result = expanded.Compile()(entity);

        Assert.AreEqual("3: abc", result);
    }

    [TestMethod]
    public void StaticExtensionMethod_Compiles_AndEvaluates()
    {
        Expression<Func<int, int>> expr = x => ExtEntity.DoubleOf(x);
        var expanded = (Expression<Func<int, int>>)expr.ExpandExpressives();

        var result = expanded.Compile()(11);

        Assert.AreEqual(22, result);
    }
}

public interface IExtEntity
{
    int Id { get; }
    string? Name { get; }
}

public class ExtEntity : IExtEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
}

public static class ExtEntityExtensions
{
    extension(ExtEntity e)
    {
        [Expressive]
        public int DoubleId => e.Id * 2;

        [Expressive]
        public int TripleId() => e.Id * 3;

        [Expressive]
        public int Multiply(int factor) => e.Id * factor;

        [Expressive]
        public string IdAndName() => e.Id + ": " + e.Name;

        [Expressive]
        public string GetGrade() => e.Id switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            _ => "F",
        };

        [Expressive(AllowBlockBody = true)]
        public string GetStatus()
        {
            if (e.IsActive && e.Id > 0)
            {
                return "Active";
            }
            return "Inactive";
        }
    }

    extension(IExtEntity e)
    {
        [Expressive]
        public string GetLabel() => e.Id + ": " + e.Name;
    }

    extension(ExtEntity)
    {
        [Expressive]
        public static int DoubleOf(int x) => x * 2;
    }

    extension(int i)
    {
        [Expressive]
        public int Squared() => i * i;
    }
}
