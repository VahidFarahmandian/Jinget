export interface JingetDateTimeModel {
    year: number,
    month: number,
    day: number,
    hour: number,
    minute: number,
    second: number,
    millisecond: number,
    dayOfWeek: number
}

export interface JingetJalaliCalendarModel {
    leap: number,
    gy: number,
    march: number
}

export interface JingetDateTimePickerYearToSelectModel {
    yearStart: number,
    yearEnd: number,
    html: string
}

export interface JingetDateModel {
    year: number,
    month: number,
    day: number,
}

type PopoverPlacement = 'auto' | 'top' | 'bottom' | 'left' | 'right';

export class JingetDateTimePickerSetting {
    /**
     * محل قرار گرفتن تقویم
     */
    placement: PopoverPlacement | (() => PopoverPlacement) | undefined = 'bottom';
    /**
     * فعال بودن تایم پیکر
     */
    enableTimePicker = false;
    /**
     * سلکتور نمایش روز انتخاب شده
     */
    targetTextSelector = '';
    /**
     * سلکتور ذخیره تاریخ میلادی، برای روز انتخاب شده
     */
    targetDateSelector = '';
    /**
     * آیا تقویم برای کنترل روز پایانی تاریخ است
     */
    toDate = false;
    /**
     * آیا تقویم برای کنترل روز شروع تاریخ است
     */
    fromDate = false;
    /**
     * شناسه گروه در حالتی که از
     * toDate
     * و
     * fromDate
     * استفاده شده است
     */
    groupId = '';
    /**
     * آیا تقویم غیر فعال است؟
     */
    disabled = false;
    /**
     * فرمت نمایش روز انتخاب شده تقویم
     */
    textFormat = '';
    /**
     * فرمت ذخیره تاریخ میلادی انتخاب شده
     */
    dateFormat = '';
    /**
     * آیا تقویم میلادی استفاده شود؟
     */
    isGregorian = false;
    /**
     * آیا تقویم به صورت این لاین نمایش داده شود؟
     */
    inLine = false;
    /**
     * تاریخ انتخاب شده
     */
    selectedDate: Date | null = null;
    /**
     * تاریخی که نمایش تقویم از آن شروع می شود
     */
    selectedDateToShow = new Date();
    /**
     * تعداد سال های قابل نمایش در لیست سال های قابل انتخاب
     */
    yearOffset = 15;
    /**
     * تاریخ میلادی روزهای تعطیل
     */
    holidays: Date[] = [];
    /**
     * تاریخ میلادی روزهای غیر فعال
     */
    disabledDates: Date[] = [];
    /**
     * عدد روزهایی از هفته که غیر فعال هستند
     */
    disabledDays: number[] = [];
    /**
     * تاریخ میلادی روزهای خاص
     */
    specialDates: Date[] = [];
    /**
     * آیا روزهای قبل از امروز غیر فعال شوند؟
     */
    disableBeforeToday = false;
    /**
     * آیا روزهای بعد از امروز غیر فعال شوند؟
     */
    disableAfterToday = false;
    /**
     * روزهای قبل از این تاریخ غیر فعال شود
     */
    disableBeforeDate: Date | null = null;
    /**
     * روزهای بعد از این تاریخ غیر فعال شود
     */
    disableAfterDate: Date | null = null;
    /**
     * آیا تقویم به صورت انتخاب بازه نمایش داده شود؟
     */
    rangeSelector = false;
    /**
     * تاریخ شروع تقویم در مد انتخاب بازه تاریخی برای نمایش
     */
    rangeSelectorStartDate: Date | null = null;
    /**
     * تاریخ پایان تقویم در مد انتخاب بازه تاریخی برای نمایش
     */
    rangeSelectorEndDate: Date | null = null;
    /**
     * تعداد ماه های قابل نمایش در قابلیت انتخاب بازه تاریخی
     */
    rangeSelectorMonthsToShow: number[] = [0, 0];
    /**
     * تاریخ های انتخاب شده در مد بازه انتخابی
     */
    selectedRangeDate: Date[] = [];
    /**
     * آیا تقویم به صورت مدال نمایش داده شود
     */
    modalMode = false;
    /**
     * تبدیل اعداد به فارسی
     */
    persianNumber = false;
    /**
     * رویداد عوض شدن ماه و تاریخ در دیت پیکر
     * @param _ تاریخ ماه انتخابی
     */
    calendarViewOnChange = (_: Date) => {
    };
    /**
     * رویداد انتخاب روز در دیت پیکر
     * @param _ تمامی تنظیمات دیت پیکر
     */
    onDayClick = (_: JingetDateTimePickerSetting) => {
    }
}

