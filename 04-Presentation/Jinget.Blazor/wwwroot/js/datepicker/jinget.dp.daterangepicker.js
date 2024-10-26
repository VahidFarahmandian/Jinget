let dateRangePickerDefaults = {
    //click on the button to show the picker. 
    buttonId: '',
    //input host the picker.
    inputId: '',
    //dates are gregorian or jalali
    isGregorian: true,
    //number of month to show before current month in date range picker
    rangeSelectorMonthsToShowStart: 0,
    //number of month to show after current month in date range picker
    rangeSelectorMonthsToShowEnd: 1,
    //show persian numbers or english numbers
    persianNumber: !this.isGregorian,
    //the picker is disabled or not. if disabled the picker will not be shown
    disabled: false,
    //display picker in modal or not
    modalMode: false,
    //show/hide time picker
    enableTimePicker: false,
    //default is yyyy/MM/dd
    dateFormat: 'yyyy/MM/dd',
    //default is yyyy/MM/dd
    textFormat: 'yyyy/MM/dd',
    //number of years being show in year selected
    yearOffset: 15,
    //where to show the picker. values are auto, top, bottom, left, right 
    placement: 'auto',
    //which date are holidays? in gregorian format
    holidays: [],
    //which date are special dates? in gregorian format
    specialDates: [],
    //which dates are disabled? in gregorian format
    disabledDates: [],
    //selected date to bind to picker
    selectedDate: null,
    //selected start date in date range mode
    rangeSelectorStartDate: null,
    //selected end date in date range mode
    rangeSelectorEndDate: null,
    //selected date to show in picker input
    selectedDateToShow: new Date(),
    //disable dates before today
    disableBeforeToday: false,
    //disable dates after today
    disableAfterToday: false,
    //disable dates before this date
    disableBeforeDate: null,
    //disable dates after this date
    disableAfterDate: null
};

function initializeDateRangePicker(dotnet, params = {}) {

    params = $.extend({}, dateRangePickerDefaults, params);
    let element = document.getElementById(params.buttonId);
    let settings = {
        //selected date will be bind to this input
        targetTextSelector: '[data-name="' + params.inputId + '"]',
        //selected date in gregorian format will be bind to this hidden input
        targetDateSelector: '[data-name="hdn-' + params.inputId + '"]',

        isGregorian: params.isGregorian,
        rangeSelector: true,
        rangeSelectorMonthsToShowStart: params.rangeSelectorMonthsToShowStart,
        rangeSelectorMonthsToShowEnd: params.rangeSelectorMonthsToShowEnd,
        persianNumber: params.persianNumber,
        disabled: params.disabled,
        modalMode: params.modalMode,
        enableTimePicker: params.enableTimePicker,
        dateFormat: params.dateFormat,
        textFormat: params.textFormat,
        yearOffset: params.yearOffset,
        placement: params.placement,
        holidays: normalizeDates(params.holidays),
        specialDates: normalizeDates(params.specialDates),
        disabledDates: normalizeDates(params.disabledDates),
        selectedDate: normalizeDate(params.selectedDate),
        rangeSelectorStartDate: normalizeDate(params.rangeSelectorStartDate),
        rangeSelectorEndDate: normalizeDate(params.rangeSelectorEndDate),
        disableBeforeDate: normalizeDate(params.disableBeforeDate),
        disableAfterDate: normalizeDate(params.disableAfterDate),
        disableBeforeToday: params.disableBeforeToday,
        disableAfterToday: params.disableAfterToday,
        //this event will be raised whenever calendar view changes. 
        //for example when user click to go to next month or other year
        calendarViewOnChange: (a) => {
        },
        onDayClick: (a) => {
            dotnet.invokeMethodAsync('OnJSDatePickerSelectedDateChanged', a.selectedDate);
        }
    };
    new jinget_dp.JingetDateTimePicker(element, settings);
    //set custom selected date 
    window.setDate = (id, date) => {
        getPicker(id).setDate(new Date(date));
    }
    //clear selected date
    window.clearDate = (id) => {
        getPicker(id).clearDate();
    }

    //find picker element in page
    function getPicker(id) {
        let inputSelector = $('#' + id);
        let hasBtn = inputSelector[0].getAttribute('data-has-btn');
        let picker;
        if (hasBtn === "true") {
            let btnSelector = $('#btn-' + id);
            picker = jinget_dp.JingetDateTimePicker.getInstance(btnSelector[0]);
        } else {
            picker = jinget_dp.JingetDateTimePicker.getInstance(inputSelector[0]);
        }
        return picker;
    }

    //normalize date to js date
    function normalizeDate(date) {
        return date == null ? null : new Date(date)
    }

    //normalize an array of dates toan array of js dates
    function normalizeDates(dateArray) {
        if (dateArray == null)
            return dateArray
        else
            return dateArray.map(d => new Date(d))
    }
}