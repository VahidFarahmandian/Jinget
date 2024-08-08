namespace Jinget.Blazor.Components.Popup;

public abstract class JingetPopupBase : ComponentBase
{
    [Parameter] public string Id { get; set; } = Guid.NewGuid().ToString("N");
    [Parameter] public bool Rtl { get; set; } = true;

    [Parameter] public string? CloseButtonText { get; set; } = "بستن";

    [Parameter] public EventCallback OnOpen { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
}
