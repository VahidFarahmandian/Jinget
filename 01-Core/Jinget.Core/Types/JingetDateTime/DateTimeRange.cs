using Jinget.Core.Attributes.ValidationAttributes;

namespace Jinget.Core.Types.JingetDateTime;
public class DateTimeRange
{
    [DateGreaterThan("End", ErrorMessage = "Start date must be before end date.")]
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}