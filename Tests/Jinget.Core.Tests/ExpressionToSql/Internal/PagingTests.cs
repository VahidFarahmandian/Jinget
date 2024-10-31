namespace Jinget.Core.Tests.ExpressionToSql.Internal;

[TestClass]
public class PagingTests
{
    [TestMethod]
    public void should_return_paging_clause()
    {
        string expectedResult = "OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY";
        string result = new Paging { PageNumber = 1, PageSize = 5 }.ToString();
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void should_return_paging_clause_default_page_size()
    {
        string expectedResult = "OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";
        string result = new Paging { PageNumber = -1, PageSize = -1 }.ToString();
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void should_return_nothing()
    {
        string expectedResult = "";
        string result = new Paging { PageNumber = 0, PageSize = 0 }.ToString();
        Assert.AreEqual(expectedResult, result);
    }
}