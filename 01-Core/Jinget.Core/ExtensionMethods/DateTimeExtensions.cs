using Jinget.Core.Types.JingetDateTime;

namespace Jinget.Core.ExtensionMethods;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts <see cref="DateTime"/> to <see cref="DateOnly"/>
    /// </summary>
    public static DateOnly ToDateOnly(this DateTime input) => DateOnly.FromDateTime(input);

    /// <summary>
    /// return the time part of the datetime object.
    /// see also: <seealso cref="TimeOnly"/>, <seealso cref="DateTime"/>
    /// </summary>
    public static TimeOnly ToTimeOnly(this DateTime input) => TimeOnly.FromDateTime(input);

    /// <summary>
    /// Set time for <see cref="DateTime"/> object
    /// see also: <seealso cref="TimeOnly"/>, <seealso cref="DateTime"/>
    /// </summary>
    public static DateTime SetTime(this DateTime input, TimeOnly time) => input.Add(time.ToTimeSpan());

    public static bool HasStarted(this DateTime? restriction, DateTime dt)
    {
        return dt >= restriction;
    }

    public static bool HasEnded(this DateTime? restriction, DateTime dt)
    {
        return dt <= restriction;
    }


    public static bool IsWithinDateRange(this DateRange range, DateTime dt)
    {
        return dt >= range.Start && dt <= range.End;
    }

    public static bool IsWithinTimeOfDayRange(this TimeRange range, TimeOnly time)
    {
        return time >= range.Start && time <= range.End;
    }

    public static bool IsWithinSpecificDayTimeRanges(this List<DayTimeRange> ranges, DateTime dt)
    {
        return ranges.Any(restriction =>
        {
            var dayOfWeek = dt.DayOfWeek;
            if ((int)dayOfWeek == (int)restriction.DayOfWeek)
            {
                var currentTime = TimeOnly.FromDateTime(dt);
                return currentTime >= restriction.StartTime && currentTime <= restriction.EndTime;
            }
            return false;
        });
    }
}