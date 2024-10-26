using Jinget.Blazor.Enums;

namespace Jinget.Blazor.Attributes.Picker;

public abstract class JingetBaseDatePickerElement : JingetFormElement
{
    public JingetCalendarType CalendarType { get; set; }
    public bool ShowPickerButton { get; set; }
}