using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Jinget.Core.Enumerations;
using Jinget.Core.Tests._BaseData;
using static Jinget.Core.Tests._BaseData.TestClass;

namespace Jinget.Core.ExtensionMethods.Tests
{
    [TestClass()]
    public class IQueryableExtensionsTests
    {
        [TestMethod()]
        public void Should_sort_the_list_based_on_given_property()
        {
            TestClass class1 = new() { Property1 = 1, Property2 = "C" };
            TestClass class2 = new() { Property1 = 2, Property2 = "A" };
            TestClass class3 = new() { Property1 = 3, Property2 = "B" };

            var input = new List<TestClass>() { class1, class2, class3 }.AsQueryable();
            var expected = new List<TestClass>() { class1, class3, class2 }.AsQueryable();

            var result = input.OrderByDynamic("Property2", OrderByDirection.Descending);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod()]
        public void Should_sort_the_nested_member_based_on_given_property()
        {
            TestClass parent1 = new() { Property1 = 1, Property2 = "C", InnerSingularProperty = new() { InnerProperty1 = 10, InnerProperty2 = "CC" } };
            TestClass parent2 = new() { Property1 = 2, Property2 = "A", InnerSingularProperty = new() { InnerProperty1 = 20, InnerProperty2 = "AA" } };
            TestClass parent3 = new() { Property1 = 3, Property2 = "B", InnerSingularProperty = new() { InnerProperty1 = 30, InnerProperty2 = "BB" } };

            var input = new List<TestClass>() { parent1, parent2, parent3 }.AsQueryable();
            var expected = new List<TestClass>() { parent3, parent2, parent1 }.AsQueryable();

            var result = input.OrderByDynamic("InnerSingularProperty.InnerProperty1", OrderByDirection.Descending);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod()]
        public void Should_sort_the_nested_list_based_on_given_property()
        {
            InnerClass child1 = new() { InnerProperty1 = 10, InnerProperty2 = "AA" };
            InnerClass child2 = new() { InnerProperty1 = 11, InnerProperty2 = "BB" };
            InnerClass child3 = new() { InnerProperty1 = 12, InnerProperty2 = "CC" };

            TestClass parent1 = new() { Property1 = 1, Property2 = "A", InnerProperty = new List<InnerClass> { child3, child2, child1 } };
            TestClass parent2 = new() { Property1 = 2, Property2 = "B", InnerProperty = new List<InnerClass> { child2, child1, child3 } };
            TestClass parent3 = new() { Property1 = 3, Property2 = "C", InnerProperty = new List<InnerClass> { child1, child2, child3 } };

            var input = new List<TestClass>() { parent1, parent2, parent3 }.AsQueryable();
            var expectedParents = new List<TestClass>() { parent3, parent2, parent1 }.AsQueryable();
            var expectedChildren = new List<InnerClass>() { child1, child2, child3 }.AsQueryable();

            var result = input.AsQueryable().OrderByDynamic("Property1", OrderByDirection.Descending)
                .Select(x => new TestClass
                {
                    Property1 = x.Property1,
                    Property2 = x.Property2,
                    InnerProperty = x.InnerProperty.AsQueryable().OrderByDynamic("InnerProperty1", OrderByDirection.Ascending).ToList()
                });

            Assert.IsTrue(expectedParents.Select(x => new { x.Property1, x.Property2 }).SequenceEqual(result.Select(x => new { x.Property1, x.Property2 })));
            foreach (var item in result)
            {
                Assert.IsTrue(expectedChildren.SequenceEqual(item.InnerProperty));
            }
        }

        [TestMethod()]
        public void Should_return_empty_collection_when_collection_is_empty()
        {
            var input = new List<TestClass>().AsQueryable();
            var expected = new List<TestClass>().AsQueryable();

            var result = input.OrderByDynamic("", OrderByDirection.Descending);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_throw_ArgumentException_when_property_name_is_invalid()
        {
            TestClass class1 = new() { Property1 = 1, Property2 = "C" };
            var input = new List<TestClass>() { class1 }.AsQueryable();
            input.OrderByDynamic("Property3", OrderByDirection.Descending);
        }

        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void Should_throw_NullReferenceException_when_collection_is_null()
        {
            IQueryable<TestClass> input = null;
            input.OrderByDynamic("", OrderByDirection.Descending);
        }
    }
}