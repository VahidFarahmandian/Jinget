using Jinget.Blazor.Enums;

namespace Jinget.Blazor.Components.Picker;

public abstract class JingetDatePickerBaseComponent : JingetBaseComponent
{
    /// <summary>
    /// text to be shown as placeholder inside input
    /// </summary>
    [Parameter] public string Label { get; set; } = "تاریخ";

    /// <summary>
    /// Show a button next to date picker or not.
    /// By clicking on this button the picker will be shown.
    /// If HasButton=false, then in orde to show  the date picker, input box should be clicked
    /// </summary>
    [Parameter]
    public bool ShowPickerButton { get; set; }

    /// <summary>
    /// Is the calendar Gregorian or Jalali
    /// </summary>
    [Parameter]
    public JingetCalendarType CalendarType { get; set; } = JingetCalendarType.Gregorian;

    /// <summary>
    /// Whether to show the time picker alongside the date picker or not
    /// </summary>
    [Parameter]
    public bool ShowTimePicker { get; set; }

    /// <summary>
    /// whether to show the picker in own modal or not
    /// </summary>
    [Parameter]
    public bool ShowInModal { get; set; }

    /// <summary>
    /// Format used to show the selected date in input box
    /// </summary>
    [Parameter]
    public string TextFormat { get; set; } = "yyyy/MM/dd";

    /// <summary>
    /// Format used to store the selected date
    /// </summary>
    [Parameter]
    public string DateFormat { get; set; } = "yyyy/MM/dd";

    /// <summary>
    /// dates which should be marked as holidays
    /// </summary>
    [Parameter]
    public DateOnly[] Holidays { get; set; } = [];

    /// <summary>
    /// dates which should be marked as special dates
    /// </summary>
    [Parameter]
    public DateOnly[] SpecialDates { get; set; } = [];

    /// <summary>
    /// dates which should be disabled
    /// </summary>
    [Parameter]
    public DateOnly[] DisabledDates { get; set; } = [];

    /// <summary>
    /// Disable dates before today date
    /// </summary>
    [Parameter]
    public bool DisableBeforeToday { get; set; }

    /// <summary>
    /// Disable dates after today date
    /// </summary>
    [Parameter]
    public bool DisableAfterToday { get; set; }

    /// <summary>
    /// Disable dates before this date
    /// </summary>
    [Parameter]
    public DateOnly? DisableBeforeDate { get; set; }

    /// <summary>
    /// Disable dates after this date
    /// </summary>
    [Parameter]
    public DateOnly? DisableAfterDate { get; set; }

    protected string buttonId => ShowPickerButton ? "btn-" + Id : Id;
    protected string inputId => "hdn-" + Id;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializePicker();
        }
    }

    protected abstract Task InitializePicker();

    /// <summary>
    /// This method is being invoked by jinget.datepicker.js whenever selected date changed
    /// </summary>
    [JSInvokable]
    public abstract void OnJSDatePickerSelectedDateChanged(object? selectedDate);

    /// <summary>
    /// Set selected date
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetSelectedDateAsync(object? e)
    {
        await SetAsync(e);
        await UpdateUIAsync(e);
    }

    protected async Task SetAsync(object? e)
    {
        Value = e;
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = e });
    }

    protected virtual async Task UpdateUIAsync(object? e)
    {
        if (e == null)
            await JS.InvokeVoidAsync("clearDate", Id);
    }
}