using Jinget.Core.ExtensionMethods;

namespace Jinget.Blazor.Components.Picker;

public class DateRangeModel
{
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public TimeOnly? StartTime { get; private set; }
    public TimeOnly? EndTime { get; private set; }

    public string? StartDateJalali { get; private set; }
    public string? EndDateJalali { get; private set; }

    /// <summary>
    /// set date range by gregorian start and end date
    /// </summary>
    public DateRangeModel(DateTime? start, DateTime? end)
    {
        if (end < start)
            throw new Exception($"{nameof(end)} should be greater than {nameof(start)}");
        StartDate = start?.ToDateOnly();
        EndDate = end?.ToDateOnly();
        StartTime = start?.ToTimeOnly();
        EndTime = end?.ToTimeOnly();
        StartDateJalali = DateTimeUtility.ToSolarDate(start);
        EndDateJalali = DateTimeUtility.ToSolarDate(end);
    }

    /// <summary>
    /// set date range by solar start and end date
    /// </summary>
    public DateRangeModel(string start, string end) : this(DateTimeUtility.ToGregorianDate(start),
        DateTimeUtility.ToGregorianDate(end))
    {
    }
}