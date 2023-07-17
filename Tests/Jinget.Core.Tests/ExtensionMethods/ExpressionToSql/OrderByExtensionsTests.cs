using System.Collections.Generic;
using Jinget.Core.ExpressionToSql.Internal;
using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.ExtensionMethods.ExpressionToSql
{
    [TestClass()]
    public class OrderByExtensionsTests
    {
        [TestMethod()]
        public void should_return_empty_string()
        {
            List<OrderBy> orderbys = null;

            var result = orderbys.GetSorting();

            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
        }
        [TestMethod()]
        public void should_return_stringfied_order_by_clause()
        {
            List<OrderBy> orderbys = new List<OrderBy>
            {
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
            };

            string expectedResult = "ORDER BY [Property1] DESC,[Property2] ASC";

            var result = orderbys.GetSorting();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
