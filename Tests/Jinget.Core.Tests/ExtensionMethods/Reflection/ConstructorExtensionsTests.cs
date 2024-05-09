using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.ExtensionMethods.Reflection.Tests;

[TestClass()]
public class ConstructorExtensionsTests
{
    [TestMethod()]
    public void Should_return_public_default_constructor()
    {
        var ctor = typeof(TestClass).GetDefaultConstructor();

        Assert.IsNotNull(ctor);
        Assert.IsTrue(ctor.IsConstructor);
        Assert.IsTrue(ctor.IsPublic);
    }

    [TestMethod()]
    public void Should_return_private_default_constructor()
    {
        var ctor = typeof(SubType).GetDefaultConstructor();

        Assert.IsNotNull(ctor);
        Assert.IsTrue(ctor.IsConstructor);
        Assert.IsFalse(ctor.IsPublic);
    }

    [TestMethod()]
    public void Should_invoke_default_constructor()
    {
        var obj = typeof(SubType).InvokeDefaultConstructor<SubType>();
        var expectedType = typeof(SubType);

        Assert.IsNotNull(obj);
        Assert.AreEqual(expectedType, obj.GetType());
    }
}