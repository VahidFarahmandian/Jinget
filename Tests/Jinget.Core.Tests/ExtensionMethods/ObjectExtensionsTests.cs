using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Jinget.Core.Tests._BaseData;
using static Jinget.Core.ExtensionMethods.ObjectExtensions;
using System;
using Jinget.Core.ExtensionMethods;

namespace Jinget.Core.Tests.ExtensionMethods;

[TestClass()]
public class ObjectExtensionsTests
{
    [TestMethod()]
    public void should_check_if_given_type_is_a_numeric_type_or_not()
    {
        int x = 0;
        var result = x.IsNumericType();
        Assert.IsTrue(result);

        string name = "vahid";
        result = name.IsNumericType();
        Assert.IsFalse(result);

        TestClass obj = new();
        result = obj.IsNumericType();
        Assert.IsFalse(result);
    }

    [TestMethod()]
    public void should_convert_given_type_to_dictionary()
    {
        TestClass obj = new()
        {
            Property1 = 123,
            Property2 = "vahid",
            InnerSingularProperty = new TestClass.InnerClass
            {
                InnerProperty1 = 456
            },
            InnerListProperty =
            [
                new() {
                    InnerProperty1=789
                }
            ]
        };
        var result = obj.ToDictionary();
        Assert.IsTrue(result.Keys.Count == 5);
        Assert.IsTrue(result.Keys.Any(x => x == "InnerSingularProperty"));
    }

    [TestMethod()]
    public void should_convert_given_type_to_dictionary_custom_options()
    {
        TestClass obj = new()
        {
            Property1 = 123,
            Property2 = "vahid",
            InnerSingularProperty = new TestClass.InnerClass
            {
                InnerProperty1 = 456
            },
            InnerListProperty =
            [
                new() {
                    InnerProperty1=789
                }
            ]
        };
        var result = obj.ToDictionary(new Options
        {
            IgnoreNull = false,
            IgnoreExpr2SQLOrderBys = false,
            IgnoreExpr2SQLPagings = false,
            IgnoreExpressions = false
        });

        Assert.IsTrue(result.Keys.Count == 7);
        Assert.IsTrue(result.Keys.Any(x => x == "InnerSingularProperty"));
    }

    [TestMethod()]
    public void should_return_null_when_type_is_null_while_gettting_the_given_property_value()
    {
        TestClass obj = null;
        var result = obj.GetValue(nameof(TestClass.Property2));
        Assert.IsNull(result);
    }

    [TestMethod()]
    public void should_get_the_value_of_the_given_property()
    {
        TestClass obj = new()
        {
            Property1 = 123,
            Property2 = "vahid"
        };
        string expectedResult = "vahid";
        var result = obj.GetValue(nameof(TestClass.Property2));

        Assert.AreEqual(expectedResult, result);
    }


    [TestMethod()]
    public void should_convert_object_to_given_type()
    {
        Type1 obj = new()
        {
            Id = 123,
            Name = "vahid"
        };

        var result = obj.ToType<Type2>(suppressError: true);

        Assert.AreEqual(obj.Id, result.Id);
        Assert.AreEqual(obj.Name, result.Name);
        Assert.IsTrue(result.SurName == null);
    }

    [TestMethod()]
    public void should_return_true_for_two_objects_with_same_structure_and_value()
    {
        Type1 objSource = new()
        {
            Id = 123,
            Name = "vahid"
        };
        Type2 objTarget = new()
        {
            Id = 123,
            Name = "vahid"
        };
        var result = objSource.HasSameValuesAs(objTarget);

        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_true_for_guid_default_value()
    {
        Guid input = default;
        var result = input.HasDefaultValue();
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_true_for_int_default_value()
    {
        int input = default;
        var result = input.HasDefaultValue();
        Assert.IsTrue(result);
    }

    [TestMethod()]
    public void should_return_false_for_nullable_int_default_value()
    {
        int? input = null;
        var result = input.HasDefaultValue();
        Assert.IsFalse(result);
    }

    [TestMethod()]
    public void should_return_false_for_reference_type_default_value()
    {
        TestClass input = new();
        var result = input.HasDefaultValue();
        Assert.IsFalse(result);
    }
}