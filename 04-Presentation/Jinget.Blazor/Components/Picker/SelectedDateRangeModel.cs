namespace Jinget.Blazor.Components.Picker;

public class SelectedDateRangeModel
{
    public DateRange? DateRange { get; set; }
    public string? StartDateJalali { get; set; }
    public string? EndDateJalali { get; set; }

    /// <summary>
    /// set date range by gregorian start and end date
    /// </summary>
    public void Set(DateTime start, DateTime end)
    {
        if (end < start)
            throw new Exception($"{nameof(end)} should be greater than {nameof(start)}");
        DateRange = new DateRange(start, end);
        StartDateJalali = DateTimeUtility.ToSolarDate(start);
        EndDateJalali = DateTimeUtility.ToSolarDate(end);
    }

    /// <summary>
    /// set date range by solar start and end date
    /// </summary>
    public void Set(string start, string end)
    {
        var gregorianStart = DateTimeUtility.ToGregorianDate(start);
        var gregorianEnd = DateTimeUtility.ToGregorianDate(end);
        if (gregorianEnd < gregorianStart)
            throw new Exception($"{nameof(end)} should be greater than {nameof(start)}");

        DateRange = new DateRange(gregorianStart, gregorianEnd);
        StartDateJalali = start;
        EndDateJalali = end;
    }
}
