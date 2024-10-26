import {JingetDpBaseJalaliCalendar} from "./jinget.dp.base.jalali.calendar";
import {JingetDpCommonProps} from "./jinget.dp.base.common.props";
import {JingetDateModel, JingetDateTimeModel, JingetDateTimePickerSetting} from "./jinget.dp.base.models";

export class JingetDpCommon {
    private constructor() {
    }

    public static newGuid(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
            let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    public static extend(...args: any[]): any {
        for (let i = 1; i < args.length; i++)
            for (let key in args[i])
                if (args[i].hasOwnProperty(key))
                    args[0][key] = args[i][key];
        return args[0];
    }

    public static cloneDate(dateTime: Date): Date {
        if (dateTime === undefined)
            dateTime = new Date();
        return new Date(dateTime.getTime());
    }

    public static getDateTimeByDate(dateTime: Date): JingetDateTimeModel {
        return {
            year: dateTime.getFullYear(),
            month: dateTime.getMonth() + 1,
            day: dateTime.getDate(),
            hour: dateTime.getHours(),
            minute: dateTime.getMinutes(),
            second: dateTime.getSeconds(),
            millisecond: dateTime.getMilliseconds(),
            dayOfWeek: dateTime.getDay()
        };
    }

    // public static getDateTimeByNumber(dateNumber: number): JingetDateTimeModel {
    //     return {
    //         year: Math.floor(dateNumber / 10000),
    //         month: Math.floor(dateNumber / 100) % 100,
    //         day: dateNumber % 100,
    //         hour: 0,
    //         minute: 0,
    //         second: 0,
    //         millisecond: 0,
    //         dayOfWeek: -1
    //     };
    // }

    public static getDateTimeJsonPersian1(dateTime: Date): JingetDateTimeModel {
        if (dateTime === undefined)
            dateTime = new Date();

        let persianDate = JingetDpBaseJalaliCalendar.toJalali(dateTime.getFullYear(), dateTime.getMonth() + 1, dateTime.getDate());
        return {
            year: persianDate.year,
            month: persianDate.month,
            day: persianDate.day,
            hour: dateTime.getHours(),
            minute: dateTime.getMinutes(),
            second: dateTime.getSeconds(),
            millisecond: dateTime.getMilliseconds(),
            dayOfWeek: dateTime.getDay()
        };
    }

    public static getDateTimeJsonPersian2(yearPersian: number, monthPersian: number, dayPersian: number, hour: number, minute: number, second: number): JingetDateTimeModel {
        if (!this.isNumber(hour)) hour = 0;
        if (!this.isNumber(minute)) minute = 0;
        if (!this.isNumber(second)) second = 0;
        let gregorian = JingetDpBaseJalaliCalendar.toGregorian(yearPersian, monthPersian, dayPersian);
        return this.getDateTimeJsonPersian1(
            new Date(gregorian.year, gregorian.month - 1, gregorian.day, hour, minute, second));
    }

    private static isLeapYear(persianYear: number): boolean {
        return JingetDpBaseJalaliCalendar.isLeapJalaliYear(persianYear);
    }

    public static getDaysInMonthPersian(year: number, month: number): number {
        let numberOfDaysInMonth = 31;
        if (month > 6 && month < 12)
            numberOfDaysInMonth = 30;
        else if (month == 12)
            numberOfDaysInMonth = this.isLeapYear(year) ? 30 : 29;
        return numberOfDaysInMonth;
    }

    public static getDaysInMonth(year: number, month: number): number {
        return new Date(year, month + 1, 0).getDate();
    }

    public static getLastDayDateOfPreviousMonth(dateTime: Date, isGregorian: boolean): Date {
        if (dateTime === undefined)
            dateTime = new Date();
        let dateTimeLocal = this.cloneDate(dateTime);
        if (isGregorian) {
            let previousMonth = new Date(dateTimeLocal.getFullYear(), dateTimeLocal.getMonth() - 1, 1),
                daysInMonth = this.getDaysInMonth(previousMonth.getFullYear(), previousMonth.getMonth());
            return new Date(previousMonth.getFullYear(), previousMonth.getMonth(), daysInMonth);
        }
        let dateTimeJsonPersian = this.getDateTimeJsonPersian1(dateTimeLocal);
        dateTimeJsonPersian.month += -1;
        if (dateTimeJsonPersian.month <= 0) {
            dateTimeJsonPersian.month = 12;
            dateTimeJsonPersian.year--;
        } else if (dateTimeJsonPersian.month > 12) {
            dateTimeJsonPersian.year++;
            dateTimeJsonPersian.month = 1;
        }
        return this.getDateTime1(dateTimeJsonPersian.year, dateTimeJsonPersian.month, this.getDaysInMonthPersian(dateTimeJsonPersian.year, dateTimeJsonPersian.month));
    }

    public static getFirstDayDateOfNextMonth(dateTime: Date, isGregorian: boolean): Date {
        if (dateTime === undefined)
            dateTime = new Date();
        let dateTimeLocal = this.cloneDate(dateTime);
        if (isGregorian) {
            let nextMonth = new Date(dateTimeLocal.getFullYear(), dateTimeLocal.getMonth() + 1, 1);
            return new Date(nextMonth.getFullYear(), nextMonth.getMonth(), 1);
        }
        let dateTimeJsonPersian = this.getDateTimeJsonPersian1(dateTimeLocal);
        dateTimeJsonPersian.month += 1;
        if (dateTimeJsonPersian.month <= 0) {
            dateTimeJsonPersian.month = 12;
            dateTimeJsonPersian.year--;
        }
        if (dateTimeJsonPersian.month > 12) {
            dateTimeJsonPersian.year++;
            dateTimeJsonPersian.month = 1;
        }
        return this.getDateTime1(dateTimeJsonPersian.year, dateTimeJsonPersian.month, 1);
    }

    public static getDateTime1(yearPersian: number, monthPersian: number, dayPersian: number, hour?: number, minute?: number, second?: number): Date {
        if (!this.isNumber(hour)) hour = 0;
        if (!this.isNumber(minute)) minute = 0;
        if (!this.isNumber(second)) second = 0;
        let gregorian = JingetDpBaseJalaliCalendar.toGregorian(yearPersian, monthPersian, dayPersian);
        return new Date(gregorian.year, gregorian.month - 1, gregorian.day, hour, minute, second);
    }

    private static getDateTime2(dateTimeJsonPersian: JingetDateTimeModel): Date {
        if (!dateTimeJsonPersian.hour) dateTimeJsonPersian.hour = 0;
        if (!dateTimeJsonPersian.minute) dateTimeJsonPersian.minute = 0;
        if (!dateTimeJsonPersian.second) dateTimeJsonPersian.second = 0;
        let gregorian = JingetDpBaseJalaliCalendar.toGregorian(dateTimeJsonPersian.year, dateTimeJsonPersian.month, dateTimeJsonPersian.day);
        return new Date(gregorian.year, gregorian.month - 1, gregorian.day, dateTimeJsonPersian.hour, dateTimeJsonPersian.minute, dateTimeJsonPersian.second);
    }

    private static getDateTime3(dateTimeJson: JingetDateTimeModel): Date {
        return new Date(dateTimeJson.year, dateTimeJson.month - 1, dateTimeJson.day, dateTimeJson.hour, dateTimeJson.minute, dateTimeJson.second);
    }

    public static getDateTime4(dateNumber: number, dateTime: Date, isGregorian: boolean): Date {
        if (dateTime === undefined)
            dateTime = new Date();
        let dateTimeJson = this.getDateTimeJson2(dateNumber);
        if (!isGregorian) {
            let dateTimeJsonPersian = this.getDateTimeJsonPersian1(dateTime);
            dateTimeJsonPersian.year = dateTimeJson.year;
            dateTimeJsonPersian.month = dateTimeJson.month;
            dateTimeJsonPersian.day = dateTimeJson.day;
            dateTime = this.getDateTime2(dateTimeJsonPersian);
        } else
            dateTime = new Date(dateTimeJson.year, dateTimeJson.month - 1, dateTimeJson.day,
                dateTime.getHours(), dateTime.getMinutes(), dateTime.getSeconds());
        return dateTime;
    }

    public static getDateTimeJson2(dateNumber: number): JingetDateTimeModel {
        return {
            year: Math.floor(dateNumber / 10000),
            month: Math.floor(dateNumber / 100) % 100,
            day: dateNumber % 100,
            hour: 0,
            minute: 0,
            second: 0,
            millisecond: 0,
            dayOfWeek: -1
        };
    }

    public static getLesserDisableBeforeDate(setting: JingetDateTimePickerSetting): JingetDateTimeModel | null {
        // دریافت تاریخ کوچکتر
        // از بین تاریخ های غیر فعال شده در گذشته
        let resultDate: Date | null = null;
        const dateNow = new Date();
        if (setting.disableBeforeToday && setting.disableBeforeDate) {
            if (setting.disableBeforeDate.getTime() <= dateNow.getTime())
                resultDate = this.cloneDate(setting.disableBeforeDate);
            else
                resultDate = dateNow;
        } else if (setting.disableBeforeDate)
            resultDate = this.cloneDate(setting.disableBeforeDate);
        else if (setting.disableBeforeToday)
            resultDate = dateNow;
        if (resultDate == null)
            return null;
        if (setting.isGregorian)
            return this.getDateTimeJson1(resultDate);
        return this.getDateTimeJsonPersian1(resultDate);
    }

    public static getBiggerDisableAfterDate(setting: JingetDateTimePickerSetting): JingetDateTimeModel | null {
        // دریافت تاریخ بزرگتر
        // از بین تاریخ های غیر فعال شده در آینده
        let resultDate: Date | null = null;
        const dateNow = new Date();
        if (setting.disableAfterDate && setting.disableAfterToday) {
            if (setting.disableAfterDate.getTime() >= dateNow.getTime())
                resultDate = this.cloneDate(setting.disableAfterDate);
            else
                resultDate = dateNow;
        } else if (setting.disableAfterDate)
            resultDate = this.cloneDate(setting.disableAfterDate);
        else if (setting.disableAfterToday)
            resultDate = dateNow;
        if (resultDate == null)
            return null;
        if (setting.isGregorian)
            return this.getDateTimeJson1(resultDate);
        return this.getDateTimeJsonPersian1(resultDate);
    }

    // private static addMonthToDateTimeJson(dateTimeJson: JingetDateTimeModel, addedMonth: number, isGregorian: boolean): JingetDateTimeModel {
    //     // وقتی نیاز هست تا ماه یا روز به تاریخی اضافه کنم
    //     // پس از اضافه کردن ماه یا روز این متد را استفاده میکنم تا سال و ماه
    //     // با مقادیر جدید تصحیح و برگشت داده شوند
    //     const dateTimeJson1 = Object.assign({}, dateTimeJson);
    //     dateTimeJson1.day = 1;
    //     dateTimeJson1.month += addedMonth;
    //     if (!isGregorian) {
    //         if (dateTimeJson1.month <= 0) {
    //             dateTimeJson1.month = 12;
    //             dateTimeJson1.year--;
    //         }
    //         if (dateTimeJson1.month > 12) {
    //             dateTimeJson1.year++;
    //             dateTimeJson1.month = 1;
    //         }
    //         return dateTimeJson1;
    //     }
    //     return JingetDateTimePicker.getDateTimeJson1(this.getDateTime3(dateTimeJson1));
    // }
    public static getDateTimeJson1(dateTime: Date): JingetDateTimeModel {
        if (dateTime === undefined)
            dateTime = new Date();
        return {
            year: dateTime.getFullYear(),
            month: dateTime.getMonth() + 1,
            day: dateTime.getDate(),
            hour: dateTime.getHours(),
            minute: dateTime.getMinutes(),
            second: dateTime.getSeconds(),
            millisecond: dateTime.getMilliseconds(),
            dayOfWeek: dateTime.getDay()
        };
    }

    public static convertToNumber1(dateTimeJson: JingetDateTimeModel): number {
        return Number(
            this.zeroPad(dateTimeJson.year) +
            this.zeroPad(dateTimeJson.month) +
            this.zeroPad(dateTimeJson.day));
    }

    public static convertToNumber2(year: number, month: number, day: number): number {
        return Number(
            this.zeroPad(year) +
            this.zeroPad(month) +
            this.zeroPad(day));
    }

    public static convertToNumber3(dateTime: Date): number {
        if (dateTime === undefined)
            dateTime = new Date();
        return this.convertToNumber1(this.getDateTimeJson1(dateTime));
    }

    // private static convertToNumber4(dateTime: Date): number {
    //   return Number(JingetDateTimePicker.zeroPad(dateTime.getFullYear()) + JingetDateTimePicker.zeroPad(dateTime.getMonth()) + JingetDateTimePicker.zeroPad(dateTime.getDate()));
    // }

    public static getDateTimeString(dateTimeJson: JingetDateTimeModel, format: string, isGregorian: boolean, persianNumber: boolean): string {

        /// فرمت های که پشتیبانی می شوند
        /// <para />
        /// yyyy: سال چهار رقمی
        /// <para />
        /// yy: سال دو رقمی
        /// <para />
        /// MMMM: نام ماه
        /// <para />
        /// MM: عدد دو رقمی ماه
        /// <para />
        /// M: عدد یک رقمی ماه
        /// <para />
        /// dddd: نام روز هفته
        /// <para />
        /// dd: عدد دو رقمی روز ماه
        /// <para />
        /// d: عدد یک رقمی روز ماه
        /// <para />
        /// HH: ساعت دو رقمی با فرمت 00 تا 24
        /// <para />
        /// H: ساعت یک رقمی با فرمت 0 تا 24
        /// <para />
        /// hh: ساعت دو رقمی با فرمت 00 تا 12
        /// <para />
        /// h: ساعت یک رقمی با فرمت 0 تا 12
        /// <para />
        /// mm: عدد دو رقمی دقیقه
        /// <para />
        /// m: عدد یک رقمی دقیقه
        /// <para />
        /// ss: ثانیه دو رقمی
        /// <para />
        /// s: ثانیه یک رقمی
        /// <para />
        /// fff: میلی ثانیه 3 رقمی
        /// <para />
        /// ff: میلی ثانیه 2 رقمی
        /// <para />
        /// f: میلی ثانیه یک رقمی
        /// <para />
        /// tt: ب.ظ یا ق.ظ
        /// <para />
        /// t: حرف اول از ب.ظ یا ق.ظ

        format = format.replace(/yyyy/mg, dateTimeJson.year.toString());
        format = format.replace(/yy/mg, (dateTimeJson.year % 100).toString());
        format = format.replace(/MMMM/mg, this.getMonthName(dateTimeJson.month - 1, isGregorian));
        format = format.replace(/MM/mg, JingetDpCommon.zeroPad(dateTimeJson.month));
        format = format.replace(/M/mg, dateTimeJson.month.toString());
        format = format.replace(/dddd/mg, this.getWeekDayName(dateTimeJson.dayOfWeek, isGregorian));
        format = format.replace(/dd/mg, JingetDpCommon.zeroPad(dateTimeJson.day));
        format = format.replace(/d/mg, dateTimeJson.day.toString());
        format = format.replace(/HH/mg, JingetDpCommon.zeroPad(dateTimeJson.hour));
        format = format.replace(/H/mg, dateTimeJson.hour.toString());
        format = format.replace(/hh/mg, JingetDpCommon.zeroPad(JingetDpCommon.getShortHour(dateTimeJson.hour).toString()));
        format = format.replace(/h/mg, JingetDpCommon.zeroPad(dateTimeJson.hour));
        format = format.replace(/mm/mg, JingetDpCommon.zeroPad(dateTimeJson.minute));
        format = format.replace(/m/mg, dateTimeJson.minute.toString());
        format = format.replace(/ss/mg, JingetDpCommon.zeroPad(dateTimeJson.second));
        format = format.replace(/s/mg, dateTimeJson.second.toString());
        format = format.replace(/fff/mg, JingetDpCommon.zeroPad(dateTimeJson.millisecond, '000'));
        format = format.replace(/ff/mg, JingetDpCommon.zeroPad(dateTimeJson.millisecond / 10));
        format = format.replace(/f/mg, (dateTimeJson.millisecond / 100).toString());
        format = format.replace(/tt/mg, JingetDpCommon.getAmPm(dateTimeJson.hour, isGregorian));
        format = format.replace(/t/mg, JingetDpCommon.getAmPm(dateTimeJson.hour, isGregorian)[0]);

        if (persianNumber)
            format = JingetDpCommon.toPersianNumber(format);
        return format;
    }

    public static correctOptionValue(optionName: string, value: any): any {
        const setting = new JingetDateTimePickerSetting();
        Object.keys(setting).filter(key => key === optionName).forEach(key => {
            switch (typeof (<any>setting)[key]) {
                case 'number':
                    value = +value;
                    break;
                case 'string':
                    value = value.toString();
                    break;
                case 'boolean':
                    value = !!value;
                    break;
                case 'object':
                    if ((<any>setting)[key] instanceof Date) {
                        value = new Date(value);
                    } else if (Array.isArray((<any>setting)[key])) {
                        switch (optionName) {
                            case 'holidays':
                            case 'disabledDates':
                            case 'specialDates':
                            case 'selectedRangeDate':
                                value.forEach((item: any, i: number) => {
                                    value[i] = new Date(item);
                                });
                                break;
                            case 'disabledDays':
                            case 'rangeSelectorMonthsToShow':
                                value.forEach((item: any, i: number) => {
                                    value[i] = +item;
                                });
                                break;
                        }
                    }
                    break;
            }
        });
        return value;
    }

    public static getShortHour(hour: number): number {
        let shortHour;
        if (hour > 12)
            shortHour = hour - 12;
        else
            shortHour = hour;
        return shortHour;
    }

    public static getAmPm(hour: number, isGregorian: boolean): string {
        let amPm;
        if (hour > 12) {
            if (isGregorian)
                amPm = 'PM';
            else
                amPm = 'ب.ظ';
        } else if (isGregorian)
            amPm = 'AM';
        else
            amPm = 'ق.ظ';
        return amPm;
    }

    // private static addMonthToDateTime(dateTime: Date, addedMonth: number, isGregorian: boolean): Date {
    //     let dateTimeJson: JingetDateTimeModel;
    //     if (!isGregorian) {
    //         dateTimeJson = JingetDateTimePicker.getDateTimeJsonPersian1(dateTime);
    //         dateTimeJson = JingetDateTimePicker.addMonthToDateTimeJson(dateTimeJson, addedMonth, isGregorian);
    //         return this.getDateTime2(dateTimeJson);
    //     }
    //     dateTimeJson = JingetDateTimePicker.getDateTimeJson1(dateTime);
    //     dateTimeJson = JingetDateTimePicker.addMonthToDateTimeJson(dateTimeJson, addedMonth, isGregorian);
    //     return this.getDateTime3(dateTimeJson);
    // }

    private static isNumber(n: any): boolean {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }

    public static toPersianNumber(inputNumber1: number | string): string {
        /* ۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹ */
        if (!inputNumber1) return '';
        let str1 = inputNumber1.toString().trim();
        if (!str1) return '';
        str1 = str1.replace(/0/img, '۰');
        str1 = str1.replace(/1/img, '۱');
        str1 = str1.replace(/2/img, '۲');
        str1 = str1.replace(/3/img, '۳');
        str1 = str1.replace(/4/img, '۴');
        str1 = str1.replace(/5/img, '۵');
        str1 = str1.replace(/6/img, '۶');
        str1 = str1.replace(/7/img, '۷');
        str1 = str1.replace(/8/img, '۸');
        str1 = str1.replace(/9/img, '۹');
        return str1;
    }

    public static toEnglishNumber(inputNumber1: number | string): string {
        /* ۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹ */
        if (!inputNumber1) return '';
        let str1 = inputNumber1.toString().trim();
        if (!str1) return '';
        str1 = str1.replace(/۰/img, '0');
        str1 = str1.replace(/۱/img, '1');
        str1 = str1.replace(/۲/img, '2');
        str1 = str1.replace(/۳/img, '3');
        str1 = str1.replace(/۴/img, '4');
        str1 = str1.replace(/۵/img, '5');
        str1 = str1.replace(/۶/img, '6');
        str1 = str1.replace(/۷/img, '7');
        str1 = str1.replace(/۸/img, '8');
        str1 = str1.replace(/۹/img, '9');
        return str1;
    }

    public static zeroPad(nr: any, base?: string): string {
        if (nr == undefined || nr == '') return '00';
        if (base == undefined || base == '') base = '00';
        let len = (String(base).length - String(nr).length) + 1;
        return len > 0 ? new Array(len).join('0') + nr : nr;
    }

    public static getWeekDayName(englishWeekDayIndex: number, isGregorian: boolean): string {
        if (!isGregorian) return JingetDpCommonProps.weekDayNamesPersian[englishWeekDayIndex];
        return JingetDpCommonProps.weekDayNames[englishWeekDayIndex];
    }

    public static getMonthName(monthIndex: number, isGregorian: boolean): string {
        if (monthIndex < 0)
            monthIndex = 11;
        else if (monthIndex > 11)
            monthIndex = 0;
        if (!isGregorian) return JingetDpCommonProps.monthNamesPersian[monthIndex];
        return JingetDpCommonProps.monthNames[monthIndex];
    }

    public static getWeekDayShortName(englishWeekDayIndex: number, isGregorian: boolean): string {
        if (!isGregorian)
            return JingetDpCommonProps.shortDayNamesPersian[englishWeekDayIndex];
        return JingetDpCommonProps.shortDayNames[englishWeekDayIndex];
    }

    public static addMonthToDateTime(dateTime: Date, addedMonth: number, isGregorian: boolean) {
        if (dateTime === undefined)
            dateTime = new Date();
        let dateTimeJson;
        if (!isGregorian) {
            dateTimeJson = this.getDateTimeJsonPersian1(dateTime);
            dateTimeJson = this.addMonthToDateTimeJson(dateTimeJson, addedMonth, isGregorian);
            return this.getDateTime2(dateTimeJson);
        }
        dateTimeJson = this.getDateTimeJson1(dateTime);
        dateTimeJson = this.addMonthToDateTimeJson(dateTimeJson, addedMonth, isGregorian);
        return this.getDateTime3(dateTimeJson);
    }

    public static addMonthToDateTimeJson(dateTimeJson: JingetDateTimeModel, addedMonth: number, isGregorian: boolean) {
        // وقتی نیاز هست تا ماه یا روز به تاریخی اضافه کنم
        // پس از اضافه کردن ماه یا روز این متد را استفاده میکنم تا سال و ماه
        // با مقادیر جدید تصحیح و برگشت داده شوند
        const dateTimeJson1 = Object.assign({}, dateTimeJson);
        dateTimeJson1.day = 1;
        dateTimeJson1.month += addedMonth;
        if (!isGregorian) {
            if (dateTimeJson1.month <= 0) {
                dateTimeJson1.month = 12;
                dateTimeJson1.year--;
            }
            if (dateTimeJson1.month > 12) {
                dateTimeJson1.year++;
                dateTimeJson1.month = 1;
            }
            return dateTimeJson1;
        }
        return this.getDateTimeJson1(this.getDateTime3(dateTimeJson1));
    }

    /**
     * تبدیل آبجکت تاریخ به شمسی
     * @param date آبجکت تاریخ
     */
    static convertDateToJalali = (date: Date): JingetDateModel => {
        if (date === undefined)
            date = new Date();
        const dateTimeJson1 = JingetDpCommon.getDateTimeByDate(date);
        const jalaliJsonModel = JingetDpBaseJalaliCalendar.toJalali(dateTimeJson1.year, dateTimeJson1.month, dateTimeJson1.day);
        return {
            year: jalaliJsonModel.year,
            month: jalaliJsonModel.month,
            day: jalaliJsonModel.day,
        }
    };
    /**
     * تبدیل آبجکت تاریخ به رشته
     * @param date آبجکت تاریخ
     * @param isGregorian آیا تاریخ میلادی مد نظر است یا تبدیل به شمسی شود
     * @param format فرمت مورد نظر برای تبدیل تاریخ به رشته
     */
    static convertDateToString = (date: Date, isGregorian: boolean, format: string): string => {
        if (date === undefined)
            date = new Date();
        return JingetDpCommon.getDateTimeString(!isGregorian ? JingetDpCommon.getDateTimeJsonPersian1(date) : JingetDpCommon.getDateTimeByDate(date), format, isGregorian, !isGregorian);
    };

}