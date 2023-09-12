using Jinget.Core.Utilities;
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

        [TestMethod()]
        public void should_return_true_for_valid_persian_date()
        {
            string input = "14010101";
            Assert.IsTrue(DateTimeUtility.IsValidPersianDate(input));

            input = "14100101";
            Assert.IsTrue(DateTimeUtility.IsValidPersianDate(input));

            input = "00010101";
            Assert.IsTrue(DateTimeUtility.IsValidPersianDate(input));
        }

        [TestMethod()]
        public void should_return_true_for_valid_persian_date_in_given_range()
        {
            string input = "14010101";
            string minDate = "13500101";
            string maxDate = "14030101";
            Assert.IsTrue(DateTimeUtility.IsValidPersianDate(input, minDate, maxDate));

            input = "14010101";
            Assert.IsTrue(DateTimeUtility.IsValidPersianDate(input, minDate));

            input = "14010101";
            Assert.IsTrue(DateTimeUtility.IsValidPersianDate(input, maxAcceptableDate: maxDate));
        }

        [TestMethod()]
        public void should_return_false_for_invalid_persian_date()
        {
            string input = "1401011";
            Assert.IsFalse(DateTimeUtility.IsValidPersianDate(input));

            input = "-14020101";
            Assert.IsFalse(DateTimeUtility.IsValidPersianDate(input));
        }

        [TestMethod()]
        public void should_return_false_for_persian_date_outside_given_range()
        {
            string input = "13820101";
            string minDate = "13900101";
            string maxDate = "14030101";
            Assert.IsFalse(DateTimeUtility.IsValidPersianDate(input, minDate, maxDate));

            input = "13891201";
            Assert.IsFalse(DateTimeUtility.IsValidPersianDate(input, minDate));

            input = "14040101";
            Assert.IsFalse(DateTimeUtility.IsValidPersianDate(input, maxAcceptableDate: maxDate));
        }
    }
}