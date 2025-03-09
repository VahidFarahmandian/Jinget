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
        Assert.IsTrue(typeof(int).IsSimpleType());
        Assert.IsTrue(typeof(bool).IsSimpleType());
        Assert.IsTrue(typeof(char).IsSimpleType());
        Assert.IsTrue(typeof(double).IsSimpleType());
    }

    [TestMethod]
    public void IsSimpleType_DecimalStringGuidDateTimeByteArrayEnum_ReturnsTrue()
    {
        Assert.IsTrue(typeof(decimal).IsSimpleType());
        Assert.IsTrue(typeof(string).IsSimpleType());
        Assert.IsTrue(typeof(Guid).IsSimpleType());
        Assert.IsTrue(typeof(DateTime).IsSimpleType());
        Assert.IsTrue(typeof(byte[]).IsSimpleType());
        Assert.IsTrue(typeof(MyEnum).IsSimpleType());
    }

    [TestMethod]
    public void IsSimpleType_NullableTypes_ReturnsTrue()
    {
        Assert.IsTrue(typeof(int?).IsSimpleType());
        Assert.IsTrue(typeof(decimal?).IsSimpleType());
        Assert.IsTrue(typeof(Guid?).IsSimpleType());
        Assert.IsTrue(typeof(DateTime?).IsSimpleType());
        Assert.IsTrue(typeof(MyEnum?).IsSimpleType());
    }

    [TestMethod]
    public void IsSimpleType_NonSimpleTypes_ReturnsFalse()
    {
        Assert.IsFalse(typeof(object).IsSimpleType());
        Assert.IsFalse(typeof(MyClass).IsSimpleType());
        Assert.IsFalse(typeof(MyStruct).IsSimpleType());
        Assert.IsFalse(typeof(int[,]).IsSimpleType()); // multidimensional array
        Assert.IsFalse(typeof(int[]).IsSimpleType()); // one dimensional array that is not byte[]
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
        Assert.IsTrue(result.Count != 0);
        Assert.AreEqual(expectedResult.First(), result.First());
    }
}