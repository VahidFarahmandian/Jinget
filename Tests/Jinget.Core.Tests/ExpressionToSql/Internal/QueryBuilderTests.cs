using Jinget.Core.ExpressionToSql.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Jinget.Core.Tests.ExpressionToSql.Internal;

[TestClass()]
public class QueryBuilderTests
{
    private QueryBuilder _qb;

    [TestInitialize]
    public void Initialize() => _qb = new QueryBuilder(new StringBuilder());

    [TestMethod()]
    public void should_return_top_expression()
    {
        string expectedResult = "SELECT TOP 10";
        var result = _qb.Take(10).ToString();
        Assert.AreEqual(expectedResult, result.ToString());
    }

    [TestMethod()]
    public void should_return_columns()
    {
        string expectedResult = "SELECT c.[Col1], c.[Col2]";
        var result = _qb.AddAttribute("Col1", "c").AddSeparator().AddAttribute("[Col2]", "c");
        Assert.AreEqual(expectedResult, result.ToString());
    }

    [TestMethod()]
    public void should_return_table_name()
    {
        string expectedResult = "SELECT FROM [Sch].[MyTable] AS T";
        var result = _qb.AddTable(new Table
        {
            Name = "MyTable",
            Schema = "Sch"
        }, "T");
        Assert.AreEqual(expectedResult, result.ToString());
    }

    [TestMethod()]
    public void should_return_function_name()
    {
        string expectedResult = "SELECT FROM [Sch].[Function]() AS F";
        var result = _qb.AddTable(new Table
        {
            Name = "Function()",
            Schema = "Sch"
        }, "F");
        Assert.AreEqual(expectedResult, result.ToString());
    }
}