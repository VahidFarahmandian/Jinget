using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Jinget.Core.Tests._BaseData;

namespace Jinget.Core.ExtensionMethods.Expressions.Tests
{
    [TestClass()]
    public class ExpressionUtilityTests
    {
        [TestMethod()]
        public void should_create_a_member_init_expression()
        {
            string[] inputs = new string[] { "Property1", "Property2" };
            string parameterName = "x";
            Expression<Func<TestClass, TestClass>> expectedExpression = x => new TestClass { Property1 = x.Property1, Property2 = x.Property2 };

            Expression<Func<TestClass, TestClass>> result = Utilities.Expressions.ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

            Assert.AreEqual(expectedExpression.Type, result.Type);
        }

        [TestMethod()]
        public void should_create_a_member_access_expression()
        {
            string[] inputs = new string[] { "Property1", "Property2" };
            string parameterName = "x";
            Expression<Func<TestClass, TestClass>> expectedExpression = x => new TestClass { Property1 = x.Property1, Property2 = x.Property2 };

            Expression<Func<TestClass, TestClass>> result = Utilities.Expressions.ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

            Assert.AreEqual(expectedExpression.Type, result.Type);
        }
    }
}