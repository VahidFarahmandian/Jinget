using Jinget.Core.Tests._BaseData;
using Jinget.Core.Utilities.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Jinget.Core.Tests.ExtensionMethods.Expressions
{
    [TestClass]
    public class ReplaceExpressionVisitorTests
    {
        [TestMethod]
        public void should_replace_parameter_in_expression_tree()
        {
            Expression<Func<TestClass, bool>> expression = x => x.Property1 > 0;
            Expression<Func<TestClass, bool>> expectedResult = y => y.Property1 > 0;
            ReplaceExpressionVisitor visitor = new(expression.Parameters[0], Expression.Parameter(typeof(TestClass), "y"));
            var result = visitor.Visit(expression);

            Assert.AreEqual(expectedResult.ToString(), result.ToString());
        }
    }
}
