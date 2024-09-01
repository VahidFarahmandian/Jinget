using Jinget.Blazor.Components.BI;

namespace Jinget.Blazor.Components.BI.Gauge;

public abstract class JingetGaugeBase : JingetBIBaseComponent
{
    public override object? Value { get => base.Value ?? 0; set => base.Value = value; }

    /// <summary>
    /// If set to true the the gauge data-value will be shown as a text below the gauge
    /// </summary>
    [Parameter] public bool ShowValueAsText { get; set; }

    /// <summary>
    /// Width of gauge.
    /// </summary>
    [Parameter] public int? Width { get; set; } = 200;

    /// <summary>
    /// Choose the starting hue for the active color (for value 0)
    /// </summary>
    [Parameter] public int? HueLow { get; set; } = 1;

    /// <summary>
    /// Choose the ending hue for the active color (for value 100)
    /// </summary>
    [Parameter] public int? HueHigh { get; set; } = 128;

    /// <summary>
    /// Saturation for active color. Saturation should be represented as a percentage number, such as 100%
    /// </summary>
    [Parameter] public string? Saturation { get; set; } = "100%";

    /// <summary>
    /// Lightness for active color. Lightness should be represented as a percentage number, such as 100%
    /// </summary>
    [Parameter] public string? Lightness { get; set; } = "50%";

    /// <summary>
    /// Background color of Gauge
    /// </summary>
    [Parameter] public string? GaugeBackGroundColor { get; set; } = "#1b1b1f";

    /// <summary>
    /// This color should match the parent div of the gauge (or beyond)
    /// </summary>
    [Parameter] public string? GaugeParentElementGroundColor { get; set; } = "#323138";

    /// <summary>
    /// Set new value to gauge
    /// </summary>
    /// <param name="value"></param>
    public async Task SetValueAsync(int value)
    {
        Value = value;
        await JS.InvokeVoidAsync("setJingetGaugeValue",
            new
            {
                dotnet = DotNetObjectReference.Create(this),
                Id,
                Value,
                Width,
                HueLow,
                HueHigh,
                Saturation,
                Lightness,
                GaugeBackGroundColor,
                GaugeParentElementGroundColor
            });
        StateHasChanged();
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = value });
    }
}
