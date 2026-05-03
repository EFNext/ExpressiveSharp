using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.RelationalExtensions;

[TestClass]
public class WindowFunctionStubInvocationTests
{
    [TestMethod]
    public void Stub_InvokedDirectly_MentionsUseRelationalExtensions()
    {
        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => WindowFunction.RowNumber());
        StringAssert.Contains(ex.Message, "WindowFunction.RowNumber");
        StringAssert.Contains(ex.Message, "UseRelationalExtensions");
    }

    [TestMethod]
    public void Stub_NamesMethodInMessage()
    {
        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => WindowFunction.Rank(null!));
        StringAssert.Contains(ex.Message, "WindowFunction.Rank");
    }
}
