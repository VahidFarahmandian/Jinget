namespace Jinget.Blazor.Components.Picker;

public abstract class JingetDatePickerBase : JingetBaseComponent
{
    [Parameter] public string DateFormat { get; set; } = "yyyy/MM/dd";

    [Parameter] public string Culture { get; set; } = "fa-IR";
    [Parameter] public bool Editable { get; set; } = true;
    [Parameter] public bool Clearable { get; set; } = true;
    [Parameter] public DateTime? MinDate { get; set; }

    [Parameter] public DateTime? MaxDate { get; set; }

    [Parameter] public abstract string Label { get; set; }
    [Parameter] public Func<DateTime, bool>? DisabledDateFunc { get; set; }

    [Parameter]
    public Func<DateTime, string> CustomDateStyleFunc { get; set; } = (DateTime dt) =>
        dt.DayOfWeek == DayOfWeek.Friday ? "red-text text-accent-4" : "";

    protected override async Task OnInitializedAsync()
    {
        if (Culture.ToLower() == "fa-ir" || Culture.ToLower().StartsWith("ar-"))
            Editable = false;
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("reStylePicker", Id);
        await base.OnAfterRenderAsync(firstRender);
    }
}