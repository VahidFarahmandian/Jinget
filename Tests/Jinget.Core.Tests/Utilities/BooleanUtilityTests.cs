using Jinget.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Jinget.Core.Tests.Utilities;

[TestClass()]
public class BooleanUtilityTests
{
    [TestMethod()]
    public void should_return_expression_contains_true_condition()
    {
        Expression<Func<_BaseData.TestClass, bool>> expectedResult = x => 1 == 1;

        Expression<Func<_BaseData.TestClass, bool>> result = BooleanUtility.TrueCondition<_BaseData.TestClass>();

        Assert.AreEqual(expectedResult.Compile()(new _BaseData.TestClass()), result.Compile()(new _BaseData.TestClass()));
    }

    [TestMethod()]
    public void should_return_expression_contains_false_condition()
    {
        Expression<Func<_BaseData.TestClass, bool>> expectedResult = x => 1 == 0;

        Expression<Func<_BaseData.TestClass, bool>> result = BooleanUtility.FalseCondition<_BaseData.TestClass>();

        Assert.AreEqual(expectedResult.Compile()(new _BaseData.TestClass()), result.Compile()(new _BaseData.TestClass()));
    }
}