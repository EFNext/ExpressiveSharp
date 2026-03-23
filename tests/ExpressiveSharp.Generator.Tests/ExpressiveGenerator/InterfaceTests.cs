using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class InterfaceTests : GeneratorTestBase
{
    [TestMethod]
    public Task ExplicitInterfaceMember()
    {
        var compilation = CreateCompilation(
            """
            public interface IBase
            {
                int ComputedProperty { get; }
            }

            public class Concrete : IBase
            {
                public int Id { get; }

                [Expressive]
                int IBase.ComputedProperty => Id + 1;
            }
            """);

        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task DefaultInterfaceMember()
    {
        var compilation = CreateCompilation(
            """
            public interface IBase
            {
                int Id { get; }
                int ComputedProperty { get; }
                int ComputedMethod();
            }

            public interface IDefaultBase : IBase
            {
                [Expressive]
                int Default => ComputedProperty * 2;
            }
            """);

        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExplicitInterfaceImplementation()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public interface IStringId
                {
                    string Id { get; }
                }

                public class Item : IStringId
                {
                    public int Id { get; set; }

                    string IStringId.Id => Id.ToString();

                    [Expressive]
                    public string FormattedId => ((IStringId)this).Id;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
