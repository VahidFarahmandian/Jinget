namespace Jinget.Blazor.Components;

public abstract class JingetBaseComponent : ComponentBase
{
    [Parameter, EditorRequired] public required string Id { get; set; } = Guid.NewGuid().ToString("N");
    [Parameter] public string DisplayName { get; set; }
    [Parameter] public string CssClass { get; set; }
    [Parameter] public bool IsDisabled { get; set; }
    [Parameter] public bool IsReadOnly { get; set; }
    [Parameter] public string HelperText { get; set; }
    [Parameter] public bool IsRequired { get; set; } = false;
    [Parameter] public string RequiredError { get; set; } = "*";

    private object? _value;
    [Parameter]
    public object? Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<object?> ValueChanged { get; set; }
    [Parameter] public EventCallback<ChangeEventArgs> OnChange { get; set; }
}
