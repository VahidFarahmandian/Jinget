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
}