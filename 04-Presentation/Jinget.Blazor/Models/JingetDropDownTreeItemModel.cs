namespace Jinget.Blazor.Models;

public class JingetDropDownTreeItemModel(object? value, object? parentValue, string? text) : JingetDropDownItemModelBase(value, text)
{
    public object? ParentValue { get; set; } = parentValue;
    internal object? Padding { get; set; }
}
