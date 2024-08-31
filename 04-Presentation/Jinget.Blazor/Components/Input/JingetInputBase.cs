using Jinget.Core.ExtensionMethods;

namespace Jinget.Blazor.Components.Input;

public abstract class JingetInputBase : JingetBaseComponent
{
    [Parameter] public int Rows { get; set; }
    [Parameter] public Enums.InputType InputType { get; set; }
    [Parameter] public string? PlaceHolder { get; set; }

    protected internal async Task OnChangedAsync(ChangeEventArgs? e)
    {
        Value = e?.Value;
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = Value });
    }

    protected internal bool HasValue() =>
        Value != null &&
        !string.IsNullOrWhiteSpace(Value?.ToString()) &&
        !Value.HasDefaultValue();
}
