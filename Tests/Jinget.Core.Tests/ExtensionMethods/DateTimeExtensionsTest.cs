using Jinget.Core.Types.JingetDateTime;

namespace Jinget.Core.Tests.ExtensionMethods;

[TestClass]
public class DateTimeExtensionsTest
{
    [TestMethod]
    public void should_cast_valid_datetime_to_dateonly()
    {
        var expectedDateOnly = new DateOnly(2024, 10, 15);
        var dateTime = new DateTime(2024, 10, 15);
        var dateOnly = dateTime.ToDateOnly();
        Assert.AreEqual(expectedDateOnly, dateOnly);
    }

    [TestMethod]
    public void should_get_time_from_datetime()
    {
        var expectedDateOnly = new TimeOnly(10, 20, 30);
        var dateTime = new DateTime(2024, 10, 15, 10, 20, 30);
        var dateOnly = dateTime.ToTimeOnly();
        Assert.AreEqual(expectedDateOnly, dateOnly);
    }

    [TestMethod]
    public void should_set_time_for_datetime()
    {
        var expectedDateOnly = new DateTime(2024, 10, 15, 10, 20, 30);
        var dateTime = new DateTime(2024, 10, 15);
        var dateOnly = dateTime.SetTime(new TimeOnly(10, 20, 30));
        Assert.AreEqual(expectedDateOnly, dateOnly);
    }

    [TestMethod]
    public void HasStarted_Started_ReturnsTrue()
    {
        DateTime? restriction = new DateTime(2024, 1, 1);
        DateTime dt = new DateTime(2024, 1, 1);
        Assert.IsTrue(restriction.HasStarted(dt));
    }

    [TestMethod]
    public void HasStarted_NotStarted_ReturnsFalse()
    {
        DateTime? restriction = new DateTime(2024, 1, 2);
        DateTime dt = new DateTime(2024, 1, 1);
        Assert.IsFalse(restriction.HasStarted(dt));
    }

    [TestMethod]
    public void HasEnded_Ended_ReturnsTrue()
    {
        DateTime? restriction = new DateTime(2024, 1, 1);
        DateTime dt = new DateTime(2024, 1, 1);
        Assert.IsTrue(restriction.HasEnded(dt));
    }

    [TestMethod]
    public void HasEnded_NotEnded_ReturnsFalse()
    {
        DateTime? restriction = new DateTime(2024, 1, 1);
        DateTime dt = new DateTime(2024, 1, 2);
        Assert.IsFalse(restriction.HasEnded(dt));
    }

    [TestMethod]
    public void IsWithinDateRange_WithinRange_ReturnsTrue()
    {
        DateRange range = new DateRange { Start = new DateTime(2024, 1, 1), End = new DateTime(2024, 1, 31) };
        DateTime dt = new DateTime(2024, 1, 15);
        Assert.IsTrue(range.IsWithinDateRange(dt));
    }

    [TestMethod]
    public void IsWithinDateRange_OutsideRange_ReturnsFalse()
    {
        DateRange range = new DateRange { Start = new DateTime(2024, 1, 1), End = new DateTime(2024, 1, 31) };
        DateTime dt = new DateTime(2024, 2, 1);
        Assert.IsFalse(range.IsWithinDateRange(dt));
    }

    [TestMethod]
    public void IsWithinTimeOfDayRange_WithinRange_ReturnsTrue()
    {
        TimeRange range = new TimeRange { Start = new TimeOnly(8, 0), End = new TimeOnly(17, 0) };
        TimeOnly time = new TimeOnly(12, 0);
        Assert.IsTrue(range.IsWithinTimeOfDayRange(time));
    }

    [TestMethod]
    public void IsWithinTimeOfDayRange_OutsideRange_ReturnsFalse()
    {
        TimeRange range = new TimeRange { Start = new TimeOnly(8, 0), End = new TimeOnly(17, 0) };
        TimeOnly time = new TimeOnly(18, 0);
        Assert.IsFalse(range.IsWithinTimeOfDayRange(time));
    }

    [TestMethod]
    public void IsWithinSpecificDayTimeRanges_WithinRange_ReturnsTrue()
    {
        List<DayTimeRange> ranges = new List<DayTimeRange>
            {
                new DayTimeRange { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) }
            };
        DateTime dt = new DateTime(2024, 7, 29, 12, 0, 0); // Monday
        Assert.IsTrue(ranges.IsWithinSpecificDayTimeRanges(dt));
    }

    [TestMethod]
    public void IsWithinSpecificDayTimeRanges_OutsideRange_ReturnsFalse()
    {
        List<DayTimeRange> ranges = new List<DayTimeRange>
            {
                new DayTimeRange { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) }
            };
        DateTime dt = new DateTime(2024, 7, 29, 18, 0, 0); // Monday, outside time range
        Assert.IsFalse(ranges.IsWithinSpecificDayTimeRanges(dt));

        dt = new DateTime(2024, 7, 30, 12, 0, 0); // Tuesday, different day
        Assert.IsFalse(ranges.IsWithinSpecificDayTimeRanges(dt));
    }

    [TestMethod]
    public void IsWithinSpecificDayTimeRanges_EmptyList_ReturnsFalse()
    {
        List<DayTimeRange> ranges = new List<DayTimeRange>(); // Empty list
        DateTime dt = DateTime.Now;
        Assert.IsFalse(ranges.IsWithinSpecificDayTimeRanges(dt));
    }
}