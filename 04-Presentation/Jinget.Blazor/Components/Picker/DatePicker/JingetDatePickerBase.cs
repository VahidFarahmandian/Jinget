using Jinget.Blazor.Enums;
using Jinget.Core.ExtensionMethods;

namespace Jinget.Blazor.Components.Picker.DatePicker;

public class JingetDatePickerBase : JingetDatePickerBaseComponent
{
    public DateTime? SelectedDate => HasValue() ? Convert.ToDateTime(Value.ToString()) : null;

    public string? SelectedDateJalali => $"{DateTimeUtility.ToSolarDate(SelectedDate)} {SelectedDate.Value.ToTimeOnly()}";

    protected override async Task InitializePicker()
    {
        await JS.InvokeVoidAsync("initializeDatePicker",
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
            await JS.InvokeVoidAsync("setDate", Id, Convert.ToDateTime(e));
        }

        await base.UpdateUIAsync(e);
    }
}