using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinget.Core.Tests._BaseData;
using Jinget.Core.Enumerations;
using Jinget.Core.ExpressionToSql.Internal;

namespace Jinget.Core.Tests.ExpressionToSql.Internal;

[TestClass()]
public class OrderByTests
{
    [TestMethod()]
    public void should_return_orderby_clause_simple_expression()
    {
        string expectedResult = "[Property1] ASC";
        string result = new OrderBy
        {
            Name = f => ((TestClass)f).Property1,
            Direction = OrderByDirection.Ascending
        }.ToString();
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void should_return_orderby_clause_constant_column()
    {
        string expectedResult = "[Property1] ASC";
        string result = new OrderBy
        {
            Name = f => "Property1",
            Direction = OrderByDirection.Ascending
        }.ToString();
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    /// Successful order by clause creation using reflection property discovery
    /// In this case string literal first should convert to lambda expression and then it should executed
    /// </summary>
    [TestMethod]
    public void should_return_orderby_clause_string_literal_reflection()
    {
        string expectedResult = "[Property1] ASC";
        string result = new OrderBy
        {
            Name = f => typeof(TestClass).GetProperty("Property1").Name,
            Direction = OrderByDirection.Ascending
        }.ToString();
        Assert.AreEqual(expectedResult, result);
    }
}