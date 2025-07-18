using Jinget.Core.Attributes.ValidationAttributes;

namespace Jinget.Core.Types.JingetDateTime;
public class TimeRange
{
    [TimeGreaterThan("End", ErrorMessage = "Start time must be before end time.")]
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}