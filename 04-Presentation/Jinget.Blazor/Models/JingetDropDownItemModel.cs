namespace Jinget.Blazor.Models;

public abstract class JingetDropDownItemModelBase(object? value, string? text)
{
    public object? Value { get; set; } = value;
    public string? Text { get; set; } = text;
}
public class JingetDropDownItemModel(object? value, string? text) : JingetDropDownItemModelBase(value, text)
{
}
