﻿namespace Jinget.Core.Tests.ExtensionMethods.Reflection;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    public void Should_return_true_for_anonymous_type()
    {
        var input = new { Name = "Vahid", Age = 32 };
        var result = input.GetType().IsAnonymousType();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_return_false_for_anonymous_type()
    {
        var input = new TestClass();
        var result = input.GetType().IsAnonymousType();
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void should_call_method_dynamically()
    {
        string expectedResult = "string is: vahid, integer is: 123, generic type is: SubType";

        var result = typeof(TestClass).Call(
            caller: new TestClass(),
            name: nameof(TestClass.GetInfo),
            null,
            parameterValues: ["vahid", 123],
            generics: typeof(SubType));

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void should_call_overloaded_static_method_dynamically()
    {
        int expectedResult = 250;

        var result = typeof(TestClass).Call(
            name: nameof(TestClass.Method1),
            parameterTypes: [typeof(int), typeof(int)],
            parameterValues: [100, 150],
            generics: null);

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void should_call_generic_overloaded_extension_method_dynamically()
    {
        string[] expectedResult = ["TestClass", "SoapSample"];

        var result = typeof(TestClassExtensions).Call(
            name: nameof(TestClassExtensions.Method1),
            parameterTypes: [typeof(TestClass)],
            parameterValues: [new TestClass()],
            typeof(TestClass), typeof(SoapSample));

        Assert.AreEqual(expectedResult.First(), ((string[])result).First());
        Assert.AreEqual(expectedResult.Last(), ((string[])result).Last());
    }

    [TestMethod]
    public void shuold_return_all_reference_type_properties_except_string_types()
    {
        var result = typeof(TestClass).GetReferenceTypeProperties();

        Assert.IsTrue(result.Any(x => x.Name == "InnerSingularProperty"));
    }

    [TestMethod]
    public void shuold_return_all_reference_type_properties_including_string_types()
    {
        var result = typeof(TestClass).GetReferenceTypeProperties(includeStringTypes: true);

        Assert.IsTrue(result.Any(x => x.Name == "Property2"));
    }

    [TestMethod]
    public void should_return_the_default_value_for_given_type()
    {
        var result = typeof(string).GetDefaultValue();
        Assert.IsNull(result);

        result = typeof(int).GetDefaultValue();
        Assert.AreEqual(0, result);

        result = typeof(TestClass).GetDefaultValue();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void IsCollectionType_ShouldReturnTrue_ForGenericList()
    {
        var type = typeof(List<int>);
        Assert.IsTrue(type.IsCollectionType());
    }

    [TestMethod]
    public void IsCollectionType_ShouldReturnTrue_ForGenericIEnumerable()
    {
        var type = typeof(IEnumerable<string>);
        Assert.IsTrue(type.IsCollectionType());
    }

    [TestMethod]
    public void IsCollectionType_ShouldReturnFalse_ForString()
    {
        var type = typeof(string);
        Assert.IsFalse(type.IsCollectionType());
    }

    [TestMethod]
    public void IsCollectionType_ShouldReturnFalse_ForInt()
    {
        var type = typeof(int);
        Assert.IsFalse(type.IsCollectionType());
    }

    [TestMethod]
    public void IsCollectionType_ShouldReturnFalse_ForNonGenericCollection()
    {
        var type = typeof(System.Collections.ArrayList);
        Assert.IsFalse(type.IsCollectionType());
    }

    [TestMethod]
    public void IsCollectionType_ShouldReturnTrue_ForCustomGenericCollection()
    {
        var type = typeof(MyCustomList<double>);
        Assert.IsTrue(type.IsCollectionType());
    }

    class MyCustomList<T> : List<T> { }
}