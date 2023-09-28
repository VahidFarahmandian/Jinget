using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jinget.Core.Tests._BaseData;
using static Jinget.Core.ExtensionMethods.ObjectExtensions;

namespace Jinget.Core.ExtensionMethods.Tests
{
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

            TestClass obj = new TestClass();
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
                InnerListProperty = new List<TestClass.InnerClass>
                {
                    new TestClass.InnerClass
                    {
                        InnerProperty1=789
                    }
                }
            };
            var result = obj.ToDictionary(new Options
            {
                IgnoreNull = false
            });

            Assert.IsTrue(result.Keys.Any(x => x == "InnerSingularProperty"));
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
    }
}