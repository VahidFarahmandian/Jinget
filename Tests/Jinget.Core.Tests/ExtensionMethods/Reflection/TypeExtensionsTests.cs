using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.ExtensionMethods.Reflection.Tests
{
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
    }
}