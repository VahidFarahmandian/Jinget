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
        if (restriction == null)
            return false;
        return dt >= DateTime.SpecifyKind(restriction.Value, dt.Kind);
    }


    public static bool HasEnded(this DateTime? restriction, DateTime dt)
    {
        if (restriction == null)
            return false;
        return dt <= DateTime.SpecifyKind(restriction.Value, dt.Kind);
    }

    /// <summary>
    /// Date range check with UTC support
    /// </summary>
    public static bool IsWithinDateRange(this DateRange range, DateTime dt)
    {
        if (range.Start == null || range.End == null) return false;

        var utcDt = dt.Kind == DateTimeKind.Utc ? dt : dt.ToUniversalTime();
        var startUtc = range.Start.Value.Kind == DateTimeKind.Utc
            ? range.Start.Value
            : range.Start.Value.ToUniversalTime();
        var endUtc = range.End.Value.Kind == DateTimeKind.Utc
            ? range.End.Value
            : range.End.Value.ToUniversalTime();

        return utcDt >= startUtc && utcDt <= endUtc;
    }

    /// <summary>
    /// Time-of-day handling (local time expected)
    /// </summary>
    public static bool IsWithinTimeOfDayRange(this TimeRange range, TimeOnly time)
    {
        if (range.Start == null || range.End == null) return false;
        return time >= range.Start && time <= range.End;
    }

    /// <summary>
    /// Day/time ranges (local time expected)
    /// </summary>
    public static bool IsWithinSpecificDayTimeRanges(this List<DayTimeRange> ranges, DateTime dt)
    {
        // Convert to local time if input is UTC
        var localDt = dt.Kind == DateTimeKind.Utc ? dt.ToLocalTime() : dt;
        var timeOnly = TimeOnly.FromDateTime(localDt);

        return ranges.Any(r =>
            localDt.DayOfWeek == r.DayOfWeek &&
            timeOnly >= r.StartTime &&
            timeOnly <= r.EndTime);
    }
}