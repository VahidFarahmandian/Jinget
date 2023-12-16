namespace Jinget.Blazor.Components.Picker
{
    public abstract class JingetDatePickerBase : ComponentBase
    {
        protected string id = Guid.NewGuid().ToString("N");


        public string DateFormat { get; set; } = "yyyy/MM/dd";

        [Parameter] public string Culture { get; set; } = "fa-IR";
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Editable { get; set; } = true;
        [Parameter] public bool Clearable { get; set; } = true;
        [Parameter] public DateTime? MinDate { get; set; }
        [Parameter] public DateTime? MaxDate { get; set; }
        [Parameter] public bool Required { get; set; } = false;
        [Parameter] public string RequiredError { get; set; } = "الزامی";
        [Parameter] public bool EnglishNumber { get; set; }
        [Parameter] public abstract string Label { get; set; }
        [Parameter] public Func<DateTime, bool> DisabledDateFunc { get; set; }
        [Parameter] public Func<DateTime, string> CustomDateStyleFunc { get; set; } = (DateTime dt) => dt.DayOfWeek == DayOfWeek.Friday ? "red-text text-accent-4" : "";

        protected override async Task OnInitializedAsync()
        {
            if (Culture.ToLower() == "fa-ir" || Culture.ToLower() == "ar-sa")
                Editable = false;
            await base.OnInitializedAsync();
        }
    }
}
