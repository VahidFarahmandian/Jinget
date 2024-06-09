namespace Jinget.Blazor.Attributes.Picker;


public abstract class JingetBaseDatePickerElement : JingetFormElement
{
    public string? Culture { get; set; }
    public bool EnglishNumber { get; set; }
}