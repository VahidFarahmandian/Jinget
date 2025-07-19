using Jinget.Core.Attributes.ValidationAttributes;

namespace Jinget.Core.Types.JingetDateTime;

public class DayTimeRange
{
    public DayOfWeek? DayOfWeek { get; set; }

    [TimeGreaterThan("EndTime", ErrorMessage = "Start time must be before end time.")]
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}