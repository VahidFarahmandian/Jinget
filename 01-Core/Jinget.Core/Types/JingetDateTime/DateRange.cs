using Jinget.Core.Attributes.ValidationAttributes;

namespace Jinget.Core.Types.JingetDateTime;
public struct DateRange
{
    [DateGreaterThan("End", ErrorMessage = "Start date must be before end date.")]
    public System.DateTime? Start { get; set; }
    public System.DateTime? End { get; set; }
}