namespace Jinget.Blazor.Components.Input;

public abstract class JingetInputBase : JingetBaseComponent
{
    [Parameter] public int Rows { get; set; }
    [Parameter] public InputType InputType { get; set; }


    internal Converter<object?> StringConverter = new()
    {
        SetFunc = value => value?.ToString(),
        GetFunc = text => text?.ToString(),
    };
    internal async void OnTextChanged(object? e)
    {
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = e });
    }
}
