namespace Jinget.Blazor.Components.Picker;

public abstract class JingetDatePickerBase : JingetBaseComponent
{
    public string DateFormat { get; set; } = "yyyy/MM/dd";

    [Parameter] public string Culture { get; set; } = "fa-IR";
    [Parameter] public bool Editable { get; set; } = true;
    [Parameter] public bool Clearable { get; set; } = true;
    [Parameter] public DateTime? MinDate { get; set; }
    [Parameter] public DateTime? MaxDate { get; set; }
    [Parameter] public bool EnglishNumber { get; set; }
    [Parameter] public abstract string Label { get; set; }
    [Parameter] public Func<DateTime, bool>? DisabledDateFunc { get; set; }
    [Parameter] public Func<DateTime, string> CustomDateStyleFunc { get; set; } = (DateTime dt) => dt.DayOfWeek == DayOfWeek.Friday ? "red-text text-accent-4" : "";

    protected override async Task OnInitializedAsync()
    {
        if (Culture.ToLower() == "fa-ir" || Culture.ToLower() == "ar-sa")
            Editable = false;
        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (EnglishNumber)
            await JS.InvokeVoidAsync("toEnglishNumber", Id);
    }
    protected async Task OnOpenAsync()
    {
        await JS.InvokeVoidAsync("gotoDate", Id);
    }
}
