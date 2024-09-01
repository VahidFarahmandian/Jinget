using System.Collections.Generic;
using Jinget.Core.ExpressionToSql.Internal;
using Jinget.Core.ExtensionMethods.ExpressionToSql;
using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Tests.ExtensionMethods.ExpressionToSql;

[TestClass()]
public class OrderByExtensionsTests
{
    [TestMethod()]
    public void should_return_empty_string()
    {
        List<OrderBy> lstOrderBy = null;

        var result = lstOrderBy.GetSorting();

        Assert.IsTrue(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod()]
    public void should_return_stringfied_order_by_clause()
    {
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

        string expectedResult = "ORDER BY [Property1] DESC,[Property2] ASC";

        var result = lstOrderBy.GetSorting();

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void should_return_stringfied_order_by_clause_complex_type()
    {
        List<OrderBy> lstOrderBy =
        [
            new OrderBy
            {
                Name = x=>((TestClass)x).InnerSingularProperty.InnerProperty1,
                Direction = Enumerations.OrderByDirection.Descending
            }
        ];

        string expectedResult = "ORDER BY [InnerSingularProperty.InnerProperty1] DESC";

        var result = lstOrderBy.GetSorting();

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void should_return_stringfied_order_by_clause_complex_type_generic()
    {
        List<OrderBy<TestClass>> lstOrderBy =
        [
            new OrderBy<TestClass>
            {
                Name = x=>x.InnerSingularProperty.InnerProperty1,
                Direction = Enumerations.OrderByDirection.Descending
            }
        ];

        string expectedResult = "ORDER BY [InnerSingularProperty.InnerProperty1] DESC";

        var result = lstOrderBy.GetSorting();

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void should_return_stringfied_order_by_clause_stringfied_type_name()
    {
        List<OrderBy> lstOrderBy =
        [
            new OrderBy
            {
                Name = x=>"Property3",
                Direction = Enumerations.OrderByDirection.Descending
            }
        ];

        string expectedResult = "ORDER BY [Property3] DESC";

        var result = lstOrderBy.GetSorting();

        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod()]
    public void should_return_stringfied_order_by_clause_using_string_name()
    {
        List<OrderBy> lstOrderBy =
        [
            new OrderBy("Property3")
            {
                Direction = Enumerations.OrderByDirection.Descending
            }
        ];

        string expectedResult = "ORDER BY [Property3] DESC";

        var result = lstOrderBy.GetSorting();

        Assert.AreEqual(expectedResult, result);
    }
}
