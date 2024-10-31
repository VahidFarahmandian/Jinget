namespace Jinget.Core.Tests.Utilities.Expressions;

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
