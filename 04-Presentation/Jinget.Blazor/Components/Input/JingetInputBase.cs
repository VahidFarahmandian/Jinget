using Jinget.Core.ExtensionMethods;

namespace Jinget.Blazor.Components.Input;

public abstract class JingetInputBase : JingetBaseComponent
{
    [Parameter] public int Rows { get; set; }
    [Parameter] public Enums.InputType InputType { get; set; }
    [Parameter] public string? PlaceHolder { get; set; }

    private IDictionary<string, object>? _dataAttributes;

    [Parameter(CaptureUnmatchedValues = true)]

    public IDictionary<string, object> DataAttributes
    {
        get => _dataAttributes == null
            ? []
            : _dataAttributes.Where(x => x.Key.StartsWith("data-")).ToDictionary(x => x.Key, x => x.Value);
        set => _dataAttributes = value;
    }


    protected internal async Task OnChangedAsync(ChangeEventArgs? e)
    {
        Value = e?.Value;
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = Value });
    }
}