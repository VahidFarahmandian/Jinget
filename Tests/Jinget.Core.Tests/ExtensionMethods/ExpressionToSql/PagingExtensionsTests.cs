using System.Collections.Generic;
using Jinget.Core.Exceptions;
using Jinget.Core.ExpressionToSql.Internal;
using Jinget.Core.ExtensionMethods.ExpressionToSql;
using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Tests.ExtensionMethods.ExpressionToSql;

[TestClass()]
public class PagingExtensionsTests
{
    [TestMethod()]
    public void should_return_empty_string()
    {
        Paging paging = null;
        List<OrderBy> lstOrderBy = null;

        var result = paging.GetPaging(lstOrderBy);

        Assert.IsTrue(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod()]
    [ExpectedException(typeof(JingetException), AllowDerivedTypes = false)]
    public void should_throw_jinget_exception()
    {
        Paging paging = new()
        {
            PageNumber = 1,
            PageSize = 10
        };
        List<OrderBy> lstOrderBy = null;

        paging.GetPaging(lstOrderBy);
    }

    [TestMethod()]
    public void should_return_stringfied_order_by_clause()
    {
        Paging paging = null;
        List<OrderBy> lstOrderBy =
        [
            new OrderBy
            {
                Name = x=>((TestClass)x).Property1,
                Direction = Enumerations.OrderByDirection.Descending
            }
        ];
        string expectedResult = "ORDER BY [Property1] DESC ";

        var result = paging.GetPaging(lstOrderBy);

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void should_return_stringfied_paging_clause()
    {
        Paging paging = new()
        {
            PageNumber = 1,
            PageSize = 10
        };
        List<OrderBy> lstOrderBy =
        [
            new OrderBy
            {
                Name = x=>((TestClass)x).Property1,
                Direction = Enumerations.OrderByDirection.Descending
            },
            new OrderBy
            {
                Name = x=>((TestClass)x).Property2,
                Direction = Enumerations.OrderByDirection.Ascending
            }
        ];

        string expectedResult = "ORDER BY [Property1] DESC,[Property2] ASC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

        var result = paging.GetPaging(lstOrderBy);

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void should_return_stringfied_paging_clause_generic_orderby()
    {
        Paging paging = new()
        {
            PageNumber = 1,
            PageSize = 10
        };
        List<OrderBy<TestClass>> lstOrderBy =
        [
            new OrderBy<TestClass>
            {
                Name = x => x.Property1,
                Direction = Enumerations.OrderByDirection.Descending
            },
            new OrderBy<TestClass>
            {
                Name = x => x.Property2,
                Direction = Enumerations.OrderByDirection.Ascending
            }
        ];

        string expectedResult = "ORDER BY [Property1] DESC,[Property2] ASC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

        var result = paging.GetPaging(lstOrderBy);

        Assert.AreEqual(expectedResult, result);
    }
}
