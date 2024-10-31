namespace Jinget.Core.Tests.Utilities.Enum
{
    [TestClass]
    public class OrderByDirectionUtilityTests
    {
        [TestMethod]
        public void should_get_order_by_direction_using_asc_string()
        {
            var expectedResult = OrderByDirection.Descending;
            var result = OrderByDirectionUtility.Get("desc");
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void should_get_order_by_direction_using_ascending_string()
        {
            var expectedResult = OrderByDirection.Ascending;
            var result = OrderByDirectionUtility.Get("Ascending");
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_throw_exception_for_empty_direction() => OrderByDirectionUtility.Get("");
    }
}