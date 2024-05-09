using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;
using System.Linq;

namespace Jinget.Core.ExtensionMethods.Reflection.Tests;

[TestClass()]
public class TypeExtensionsTests
{
    [TestMethod()]
    public void Should_return_true_for_anonymous_type()
    {
        var input = new { Name = "Vahid", Age = 32 };
        var result = input.GetType().IsAnonymousType();
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void Should_return_false_for_anonymous_type()
    {
        var input = new TestClass();
        var result = input.GetType().IsAnonymousType();
        Assert.IsFalse(result);
    }

    [TestMethod()]
    public void should_call_method_dynamically()
    {
        string expectedResult = "string is: vahid, integer is: 123, generic type is: SubType";

        var result = typeof(TestClass).Call(
            caller: new TestClass(),
            name: nameof(TestClass.GetInfo),
            parameters: ["vahid", 123],
            generics: typeof(SubType));

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void shuold_return_all_reference_type_properties_except_string_types()
    {
        var result = typeof(TestClass).GetReferenceTypeProperties();

        Assert.IsTrue(result.Any(x => x.Name == "InnerSingularProperty"));
    }

    [TestMethod()]
    public void shuold_return_all_reference_type_properties_including_string_types()
    {
        var result = typeof(TestClass).GetReferenceTypeProperties(includeStringTypes: true);

        Assert.IsTrue(result.Any(x => x.Name == "Property2"));
    }

    [TestMethod()]
    public void should_return_the_default_value_for_given_type()
    {
        var result = typeof(string).GetDefaultValue();
        Assert.IsNull(result);

        result = typeof(int).GetDefaultValue();
        Assert.AreEqual(0, result);

        result = typeof(TestClass).GetDefaultValue();
        Assert.IsNull(result);
    }
}