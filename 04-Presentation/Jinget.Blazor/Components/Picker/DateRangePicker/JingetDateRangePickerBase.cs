using Jinget.Blazor.Enums;

namespace Jinget.Blazor.Components.Picker.DateRangePicker;

public class JingetDateRangePickerBase : JingetDatePickerBaseComponent
{
    public DateRangeModel? SelectedDateRange => HasValue()
        ? new DateRangeModel(
            Convert.ToDateTime(Value.ToString()), Convert.ToDateTime(Value.ToString()))
        : null;

    public string? SelectedDateRangeJalali
        =>
            $"{DateTimeUtility.ToSolarDate(SelectedDateRange.StartDate)} {SelectedDateRange.StartTime}";

    protected override async Task InitializePicker()
    {
        await JS.InvokeVoidAsync("initializeDateRangePicker",
            DotNetObjectReference.Create(this),
            new
            {
                buttonId = ShowPickerButton ? buttonId : Id,
                inputId = Id,
                isGregorian = CalendarType == JingetCalendarType.Gregorian,
                persianNumber = CalendarType == JingetCalendarType.Jalali,
                disabled = IsDisabled,
                modalMode = ShowInModal,
                enableTimePicker = ShowTimePicker,
                textFormat = TextFormat,
                dateFormat = DateFormat,
                holidays = Holidays,
                specialDates = SpecialDates,
                disabledDates = DisabledDates,
                selectedDate = Value,
                selectedDateToShow = GetSelectedDateToShow(),
                disableBeforeToday = DisableBeforeToday,
                //disable dates after today
                disableAfterToday = DisableAfterToday,
                //disable dates before this date
                disableBeforeDate = DisableBeforeDate,
                //disable dates after this date
                disableAfterDate = DisableAfterDate
            }
        );
    }

    private DateOnly GetSelectedDateToShow()
    {
        if (HasValue())
        {
            return DateOnly.FromDateTime(CalendarType == JingetCalendarType.Jalali
                ? DateTimeUtility.ToGregorianDate(Value!.ToString()!)!.Value
                : Convert.ToDateTime(Value));
        }

        return DateOnly.FromDateTime(DateTime.Now);
    }

    /// <summary>
    /// This method is being invoked by jinget.datepicker.js whenever selected date changed
    /// </summary>
    [JSInvokable]
    public override void OnJSDatePickerSelectedDateChanged(object? selectedDate)
        => _ = SetAsync(Convert.ToDateTime(selectedDate?.ToString()));

    protected override async Task UpdateUIAsync(object? e)
    {
        if (e != null)
        {
            await JS.InvokeVoidAsync("setDateRange", Id, SelectedDateRange.StartDate, SelectedDateRange.EndDate);
        }

        await base.UpdateUIAsync(e);
    }
}