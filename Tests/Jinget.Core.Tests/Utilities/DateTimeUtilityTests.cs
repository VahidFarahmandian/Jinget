using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Jinget.Core.Utilities.Tests
{
    [TestClass()]
    public class DateTimeUtilityTests
    {
        [TestMethod()]
        public void should_return_solar_date()
        {
            string expectedResult = "1399/07/21";

            DateTime input = new(2020, 10, 12);

            var result = DateTimeUtility.ToSolarDate(input);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void should_return_gregorian_date()
        {
            DateTime expectedResult = new(2020, 10, 12);

            string input = "1399/07/21";

            var result = DateTimeUtility.ToGregorianDate(input);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void should_return_timespan()
        {
            TimeSpan expectedResult = new(2, 30, 0);

            int input = 150;

            var result = DateTimeUtility.ParseToTime(input);

            Assert.AreEqual(expectedResult, result);
        }
    }
}