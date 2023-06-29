using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Jinget.Core.ExtensionMethods.Expressions.Tests
{
    [TestClass()]
    public class BooleanExpressionExtensionsTests
    {
        [TestMethod()]
        public void should_negate_the_given_exoression()
        {
            Expression<Func<TestClass, bool>> expression = x => x.Property1 > 0;
            Expression<Func<TestClass, bool>> expectedExpression = x => !(x.Property1 > 0);

            var result = expression.Not();

            Assert.AreEqual(expectedExpression.Body.ToString(), result.Body.ToString());
        }

        [TestMethod()]
        public void should_combine_conditions_by_and()
        {
            Expression<Func<TestClass, bool>> expression = x => x.Property1 > 0;
            Expression<Func<TestClass, bool>> expectedExpression = x => x.Property1 > 0 && x.Property2 != string.Empty;

            var result = expression.AndAlso(x => x.Property2 != string.Empty, "x");

            Assert.AreEqual(expectedExpression.Body.ToString(), result.Body.ToString());
        }

        [TestMethod()]
        public void should_combine_conditions_by_or()
        {
            Expression<Func<TestClass, bool>> expression = x => x.Property1 > 0;
            Expression<Func<TestClass, bool>> expectedExpression = x => x.Property1 > 0 || x.Property2 != string.Empty;

            var result = expression.OrElse(x => x.Property2 != string.Empty, "x");

            Assert.AreEqual(expectedExpression.Body.ToString(), result.Body.ToString());
        }
    }
}