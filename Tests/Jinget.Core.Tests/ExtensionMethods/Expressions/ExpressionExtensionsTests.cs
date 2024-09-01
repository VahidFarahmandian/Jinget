using Jinget.Core.ExtensionMethods.Expressions;
using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Jinget.Core.Tests.ExtensionMethods.Expressions;

[TestClass()]
public class ExpressionExtensionsTests
{
    [TestMethod()]
    public void transform_anonymouse_new_expression_to_nonanonymouse_expression()
    {
        Expression<Func<TestClass, object>> expression = x => new { Property1 = 1 };

        Expression<Func<TestClass, object>> expectedExpression = x => new TestClass { Property1 = 1 };

        var result = expression.Transform();

        Assert.IsTrue(result.ToString() == "x => new TestClass() {Property1 = 1}");
    }

    [TestMethod()]
    public void transform_anonymouse_collection_expression_to_nonanonymouse_expression()
    {
        Expression<Func<ParentType, object>> expression = x => new
        {
            x.Id,
            Sub = new
            {
                x.Sub.Id,
                ColSubs = x.Sub.ColSubs.Select(u => new
                {
                    u.Id
                }).ToList()
            }
        };

        var expectedExpression = "x => new ParentType() {Id = x.Id, Sub = new SubType() {Id = x.Sub.Id, ColSubs = x.Sub.ColSubs.Select(u => new ColSubType() {Id = u.Id}).ToList()}}";

        var result = expression.Transform();

        Assert.IsTrue(result.ToString() == expectedExpression);
    }
}