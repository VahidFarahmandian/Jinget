using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Jinget.Core.Utilities.Tests
{
    [TestClass()]
    public class BooleanUtilityTests
    {
        [TestMethod()]
        public void should_return_expression_contains_true_condition()
        {
            Expression<Func<Core.Tests._BaseData.TestClass, bool>> expectedResult = x => 1 == 1;

            Expression<Func<Core.Tests._BaseData.TestClass, bool>> result = BooleanUtility.TrueCondition<Core.Tests._BaseData.TestClass>();

            Assert.AreEqual(expectedResult.Compile()(new Core.Tests._BaseData.TestClass()), result.Compile()(new Core.Tests._BaseData.TestClass()));
        }

        [TestMethod()]
        public void should_return_expression_contains_false_condition()
        {
            Expression<Func<Core.Tests._BaseData.TestClass, bool>> expectedResult = x => 1 == 0;

            Expression<Func<Core.Tests._BaseData.TestClass, bool>> result = BooleanUtility.FalseCondition<Core.Tests._BaseData.TestClass>();

            Assert.AreEqual(expectedResult.Compile()(new Core.Tests._BaseData.TestClass()), result.Compile()(new Core.Tests._BaseData.TestClass()));
        }
    }
}