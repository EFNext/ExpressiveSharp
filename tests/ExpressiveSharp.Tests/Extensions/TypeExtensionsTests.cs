using System.Reflection;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.Tests.TestFixtures;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    public void GetNestedTypePath_NonNestedType_ReturnsSingleElement()
    {
        var path = typeof(Animal).GetNestedTypePath();

        Assert.AreEqual(1, path.Length);
        Assert.AreEqual(typeof(Animal), path[0]);
    }

    [TestMethod]
    public void GetNestedTypePath_SingleLevelNesting_ReturnsTwoElements()
    {
        var path = typeof(Outer.Middle).GetNestedTypePath();

        Assert.AreEqual(2, path.Length);
        Assert.AreEqual(typeof(Outer), path[0]);
        Assert.AreEqual(typeof(Outer.Middle), path[1]);
    }

    [TestMethod]
    public void GetNestedTypePath_DeepNesting_ReturnsFullPath()
    {
        var path = typeof(Outer.Middle.Inner).GetNestedTypePath();

        Assert.AreEqual(3, path.Length);
        Assert.AreEqual(typeof(Outer), path[0]);
        Assert.AreEqual(typeof(Outer.Middle), path[1]);
        Assert.AreEqual(typeof(Outer.Middle.Inner), path[2]);
    }

    [TestMethod]
    public void GetConcreteMethod_VirtualOverride_ReturnsDerivedMethod()
    {
        var animalSound = typeof(Animal).GetMethod("Sound")!;
        var result = typeof(Dog).GetConcreteMethod(animalSound);

        Assert.AreEqual(typeof(Dog), result.DeclaringType);
        Assert.AreEqual("Sound", result.Name);
    }

    [TestMethod]
    public void GetConcreteMethod_SameDeclaringType_ReturnsOriginal()
    {
        var animalSound = typeof(Animal).GetMethod("Sound")!;
        var result = typeof(Animal).GetConcreteMethod(animalSound);

        Assert.AreEqual(animalSound, result);
    }

    [TestMethod]
    public void GetConcreteMethod_StaticMethod_ReturnsOriginal()
    {
        var staticMethod = typeof(Animal).GetMethod("StaticMethod")!;
        var result = typeof(Dog).GetConcreteMethod(staticMethod);

        Assert.AreEqual(staticMethod, result);
    }

    [TestMethod]
    public void GetConcreteMethod_NonVirtualMethod_ReturnsOriginal()
    {
        var nonVirtualMethod = typeof(Animal).GetMethod("NonVirtualMethod")!;
        var result = typeof(Dog).GetConcreteMethod(nonVirtualMethod);

        Assert.AreEqual(nonVirtualMethod, result);
    }

    [TestMethod]
    public void GetConcreteMethod_InterfaceMethod_ReturnsImplementation()
    {
        var interfaceDescribe = typeof(IIdentifiable).GetMethod("Describe")!;
        var result = typeof(Entity).GetConcreteMethod(interfaceDescribe);

        Assert.AreEqual(typeof(Entity), result.DeclaringType);
        Assert.AreEqual("Describe", result.Name);
    }

    [TestMethod]
    public void GetConcreteProperty_VirtualOverride_ReturnsDerivedProperty()
    {
        var animalName = typeof(Animal).GetProperty("Name")!;
        var result = typeof(Dog).GetConcreteProperty(animalName);

        Assert.AreEqual(typeof(Dog), result.DeclaringType);
        Assert.AreEqual("Name", result.Name);
    }

    [TestMethod]
    public void GetConcreteProperty_InterfaceProperty_ReturnsImplementation()
    {
        var interfaceId = typeof(IIdentifiable).GetProperty("Id")!;
        var result = typeof(Entity).GetConcreteProperty(interfaceId);

        Assert.AreEqual(typeof(Entity), result.DeclaringType);
        Assert.AreEqual("Id", result.Name);
    }

    [TestMethod]
    public void GetConcreteProperty_SameDeclaringType_ReturnsOriginal()
    {
        var animalName = typeof(Animal).GetProperty("Name")!;
        var result = typeof(Animal).GetConcreteProperty(animalName);

        Assert.AreEqual(animalName, result);
    }
}
