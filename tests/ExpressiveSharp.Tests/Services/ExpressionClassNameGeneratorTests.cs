using ExpressiveSharp.Services;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressionClassNameGeneratorTests
{
    [TestMethod]
    public void GenerateName_SimpleNamespaceAndMember()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Foo.Bar", null, "MyProp");
        Assert.AreEqual("Foo_Bar_MyProp", result);
    }

    [TestMethod]
    public void GenerateName_NullNamespace()
    {
        var result = ExpressionClassNameGenerator.GenerateName(null, null, "MyProp");
        Assert.AreEqual("_MyProp", result);
    }

    [TestMethod]
    public void GenerateName_NestedClassHierarchy()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Ns", ["Outer", "Inner"], "Prop");
        Assert.AreEqual("Ns_Outer_Inner_Prop", result);
    }

    [TestMethod]
    public void GenerateName_GenericClassWithArity()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Ns", ["MyClass`1"], "Prop");
        Assert.AreEqual("Ns_MyClass_Prop`1", result);
    }

    [TestMethod]
    public void GenerateName_WithParameterTypes()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Ns", null, "Method", ["int", "string"]);
        Assert.AreEqual("Ns_Method_P0_int_P1_string", result);
    }

    [TestMethod]
    public void GenerateName_ExplicitInterfaceMember()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Ns", null, "IFoo.Bar");
        Assert.AreEqual("Ns_IFoo__Bar", result);
    }

    [TestMethod]
    public void GenerateName_ParameterTypeWithGlobalPrefix()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Ns", null, "M", ["global::System.Int32"]);
        Assert.AreEqual("Ns_M_P0_System_Int32", result);
    }

    [TestMethod]
    public void GenerateName_ParameterTypeWithInvalidChars()
    {
        var result = ExpressionClassNameGenerator.GenerateName("Ns", null, "M", ["List<int>"]);
        Assert.AreEqual("Ns_M_P0_List_int_", result);
    }

    [TestMethod]
    public void GenerateFullName_PrependsNamespace()
    {
        var result = ExpressionClassNameGenerator.GenerateFullName("Foo.Bar", null, "MyProp");
        Assert.AreEqual("ExpressiveSharp.Generated.Foo_Bar_MyProp", result);
    }

    [TestMethod]
    public void Namespace_ConstantIsCorrect()
    {
        var actual = ExpressionClassNameGenerator.Namespace;
        Assert.AreEqual("ExpressiveSharp.Generated", actual);
    }

    [TestMethod]
    public void GenerateClassName_SimpleNamespaceAndClass()
    {
        var result = ExpressionClassNameGenerator.GenerateClassName("Foo", ["C"]);
        Assert.AreEqual("Foo_C", result);
    }

    [TestMethod]
    public void GenerateClassName_NullNamespace()
    {
        var result = ExpressionClassNameGenerator.GenerateClassName(null, ["C"]);
        Assert.AreEqual("_C", result);
    }

    [TestMethod]
    public void GenerateClassName_NestedClasses()
    {
        var result = ExpressionClassNameGenerator.GenerateClassName("Ns", ["Outer", "Inner"]);
        Assert.AreEqual("Ns_Outer_Inner", result);
    }

    [TestMethod]
    public void GenerateClassName_GenericClassWithArity()
    {
        var result = ExpressionClassNameGenerator.GenerateClassName("Ns", ["MyClass`1"]);
        Assert.AreEqual("Ns_MyClass`1", result);
    }

    [TestMethod]
    public void GenerateClassName_DottedNamespace()
    {
        var result = ExpressionClassNameGenerator.GenerateClassName("Foo.Bar", ["C"]);
        Assert.AreEqual("Foo_Bar_C", result);
    }

    [TestMethod]
    public void GenerateClassFullName_PrependsNamespace()
    {
        var result = ExpressionClassNameGenerator.GenerateClassFullName("Foo", ["C"]);
        Assert.AreEqual("ExpressiveSharp.Generated.Foo_C", result);
    }

    [TestMethod]
    public void GenerateMethodSuffix_SimpleMember()
    {
        var result = ExpressionClassNameGenerator.GenerateMethodSuffix("IdPlus1", null);
        Assert.AreEqual("IdPlus1", result);
    }

    [TestMethod]
    public void GenerateMethodSuffix_WithParameters()
    {
        var result = ExpressionClassNameGenerator.GenerateMethodSuffix("Add", ["int"]);
        Assert.AreEqual("Add_P0_int", result);
    }

    [TestMethod]
    public void GenerateMethodSuffix_MultipleParameters()
    {
        var result = ExpressionClassNameGenerator.GenerateMethodSuffix("Method", ["int", "string"]);
        Assert.AreEqual("Method_P0_int_P1_string", result);
    }

    [TestMethod]
    public void GenerateMethodSuffix_ExplicitInterface()
    {
        var result = ExpressionClassNameGenerator.GenerateMethodSuffix("IFoo.Bar", null);
        Assert.AreEqual("IFoo__Bar", result);
    }

    [TestMethod]
    public void GenerateMethodSuffix_GlobalPrefixStripped()
    {
        var result = ExpressionClassNameGenerator.GenerateMethodSuffix("M", ["global::System.Int32"]);
        Assert.AreEqual("M_P0_System_Int32", result);
    }
}
