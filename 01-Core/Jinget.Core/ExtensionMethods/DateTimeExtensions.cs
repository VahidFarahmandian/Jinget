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
}