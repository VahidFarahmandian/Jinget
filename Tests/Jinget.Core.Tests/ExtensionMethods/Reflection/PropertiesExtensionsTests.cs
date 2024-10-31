namespace Jinget.Core.Tests.ExtensionMethods.Reflection;

[TestClass]
public class PropertiesExtensionsTests
{
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