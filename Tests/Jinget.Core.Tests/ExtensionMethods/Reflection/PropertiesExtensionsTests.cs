namespace Jinget.Core.Tests.ExtensionMethods.Reflection;

[TestClass]
public class PropertiesExtensionsTests
{
    enum MyEnum { Value1, Value2 }
    class MyClass { }
    struct MyStruct { public int Value; }

    [TestMethod]
    public void IsSimpleType_PrimitiveTypes_ReturnsTrue()
    {
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(int)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(bool)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(char)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(double)));
    }

    [TestMethod]
    public void IsSimpleType_DecimalStringGuidDateTimeByteArrayEnum_ReturnsTrue()
    {
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(decimal)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(string)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(Guid)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(DateTime)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(byte[])));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(MyEnum)));
    }

    [TestMethod]
    public void IsSimpleType_NullableTypes_ReturnsTrue()
    {
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(int?)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(decimal?)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(Guid?)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(DateTime?)));
        Assert.IsTrue(PropertiesExtensions.IsSimpleType(typeof(MyEnum?)));
    }

    [TestMethod]
    public void IsSimpleType_NonSimpleTypes_ReturnsFalse()
    {
        Assert.IsFalse(PropertiesExtensions.IsSimpleType(typeof(object)));
        Assert.IsFalse(PropertiesExtensions.IsSimpleType(typeof(MyClass)));
        Assert.IsFalse(PropertiesExtensions.IsSimpleType(typeof(MyStruct)));
        Assert.IsFalse(PropertiesExtensions.IsSimpleType(typeof(int[,]))); // multidimensional array
        Assert.IsFalse(PropertiesExtensions.IsSimpleType(typeof(int[]))); // one dimensional array that is not byte[]
    }

    [TestMethod]
    public void IsSimpleType_NullType_ReturnsFalse()
    {
        Assert.IsFalse(PropertiesExtensions.IsSimpleType(null));
    }

    [TestMethod]
    public void Should_return_true_for_nullable_type()
    {
        var input = typeof(int?);
        var result = input.IsNullable();
        Assert.IsTrue(result);

        input = typeof(bool?);
        result = input.IsNullable();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_return_all_writable_primitive_properties()
    {
        Dictionary<string, PropertyInfo> expectedResult = new()
        {
            {"Property1", typeof(TestClass).GetProperty("Property1") },
            {"Property2", typeof(TestClass).GetProperty("Property2") }
        };
        var result = typeof(TestClass).GetWritableProperties();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.AreEqual(expectedResult.First(), result.First());
    }
}