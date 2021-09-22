using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;
using System;

namespace Jinget.Core.ExtensionMethods.Tests
{
    [TestClass()]
    public class TypeExtensionsTests
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
        public void Should_return_true_for_nullable_type()
        {
            var input = typeof(int?);
            var result = input.IsNullable();
            Assert.IsTrue(result);

            input = typeof(Nullable<bool>);
            result = input.IsNullable();
            Assert.IsTrue(result);
        }

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
        public void Should_invoke_default_constructor()
        {
            var obj = typeof(SubType).InvokeDefaultConstructor<SubType>();
            var expectedType = typeof(SubType);
            
            Assert.IsNotNull(obj);
            Assert.AreEqual(expectedType, obj.GetType());
        }
    }
}