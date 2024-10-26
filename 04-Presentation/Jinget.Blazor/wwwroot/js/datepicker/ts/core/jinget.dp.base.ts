import {JingetDpCommon} from "./jinget.dp.base.common";
import {JingetDpCommonProps} from "./jinget.dp.base.common.props";
import {
    JingetDateTimeModel,
    JingetDateTimePickerSetting,
    JingetDateTimePickerYearToSelectModel
} from "./jinget.dp.base.models";

export abstract class JingetDateTimePickerBase {

    protected tempTitleString = '';
    protected guid: string = '';
    protected setting: JingetDateTimePickerSetting = new JingetDateTimePickerSetting();
    protected element: Element;

    protected constructor(element: Element, setting: JingetDateTimePickerSetting) {
        setting = JingetDpCommon.extend(new JingetDateTimePickerSetting(), setting);
        if (!element) throw new Error(`Jinget DateTime Picker => element is null!`);
        if (setting.rangeSelector && (setting.toDate || setting.fromDate)) throw new Error(`Jinget DateTime Picker => You can not set true 'toDate' or 'fromDate' and 'rangeSelector' together`);
        if (setting.toDate && setting.fromDate) throw new Error(`Jinget DateTime Picker => You can not set true 'toDate' and 'fromDate' together`);
        if (!setting.groupId && (setting.toDate || setting.fromDate)) throw new Error(`Jinget DateTime Picker => When you set 'toDate' or 'fromDate' true, you have to set 'groupId'`);

        if (!setting.textFormat) {
            setting.textFormat = 'yyyy/MM/dd';
            if (setting.enableTimePicker)
                setting.textFormat += ' HH:mm';
        }
        if (!setting.dateFormat) {
            setting.dateFormat = 'yyyy/MM/dd';
            if (setting.enableTimePicker)
                setting.dateFormat += ' HH:mm';
        }
        if (setting.yearOffset > 15)
            setting.yearOffset = 15;

        setting.selectedDate = setting.selectedDate ? JingetDpCommon.cloneDate(setting.selectedDate) : null;
        setting.selectedDateToShow = JingetDpCommon.cloneDate(setting.selectedDateToShow) ?? new Date();
        this.setting = setting;

        this.guid = JingetDpCommon.newGuid();
        element.setAttribute("data-jinget-dtp-guid", this.guid);
        this.element = element;
    }

    private static modalHtmlTemplate =
        `<div data-jinget-dtp data-jinget-dtp-guid="{{guid}}" class="modal fade jinget-bs-persian-datetime-picker-modal" tabindex="-1" role="dialog" aria-hidden="true">
  <div class="modal-dialog">
	  <div class="modal-content">
      <div class="modal-header" data-jinget-dtp-title="true">
        <h5 class="modal-title">Modal title</h5>
      </div>
      <div class="modal-body">
        <div class="select-year-box w-0" data-jinget-dtp-year-list-box="true"></div>
        <div data-name="jinget-dtp-body"></div>
      </div>
    </div>
  </div>
</div>`;
    public static popoverHtmlTemplate = `<div class="popover jinget-bs-persian-datetime-picker-popover" role="tooltip" data-jinget-dtp>
<div class="popover-arrow"></div>
<h3 class="popover-header text-center p-1" data-jinget-dtp-title="true"></h3>
<div class="popover-body p-0" data-name="jinget-dtp-body"></div>
</div>`;
    private static popoverHeaderSelectYearHtmlTemplate = `<table class="table table-sm table-borderless text-center p-0 m-0 {{rtlCssClass}}" dir="{{dirAttrValue}}">
<tr>
<th>
<button type="button" class="btn btn-sm btn-light w-100" title="{{previousText}}" data-year="{{latestPreviousYear}}" data-year-range-button-change="-1" {{prevYearButtonAttr}}> &lt; </button>
</th>
<th class="pt-1">
{{yearsRangeText}}
</th>
<th>
<button type="button" class="btn btn-sm btn-light w-100" title="{{nextText}}" data-year="{{latestNextYear}}" data-year-range-button-change="1" {{nextYearButtonAttr}}> &gt; </button>
</th>
</tr>
</table>`;
    private static dateTimePickerYearsToSelectHtmlTemplate = `<table class="table table-sm text-center p-0 m-0">
<tbody>
{{yearsBoxHtml}}
<tr>
<td colspan="100" class="text-center">
<button class="btn btn-sm btn-light w-100" data-jinget-hide-year-list-box="true">{{cancelText}}</button>
</td>
</tr>
</tbody>
</table>`;

    private static dateTimePickerHtmlTemplate = `<div class="jinget-bs-dtp-container {{rtlCssClass}}" {{inlineAttr}}>
<div class="select-year-inline-box w-0" data-name="dtp-years-container">
</div>
<div class="select-year-box w-0" data-jinget-dtp-year-list-box="true"></div>
<table class="table table-sm text-center p-0 m-0">
<thead>
<tr {{selectedDateStringAttribute}}>
<th jinget-dtp-inline-header colspan="100">{{dtpInlineHeader}}</th>
</tr>
</thead>
<tbody>
<tr>
{{monthsTdHtml}}
</tr>
</tbody>
<tfoot>
<tr {{timePickerAttribute}}>
<td colspan="100" class="text-center border-0">
<input type="time" value="{{time}}" maxlength="2" data-jinget-dtp-time />
</td>
</tr>
<tr>
<td colspan="100">
<button type="button" class="btn btn-light" title="{{goTodayText}}" data-jinget-dtp-go-today>{{todayDateString}}</button>
</td>
</tr>
</tfoot>
</table>
</div>`;

    private static dateTimePickerMonthTableHtmlTemplate = `<td class="border-0" style="{{monthTdStyle}}" {{monthTdAttribute}} data-td-month>
<table class="table table-sm table-striped table-borderless">
<thead>
<tr {{monthNameAttribute}}>
<th colspan="100" class="border-0">
<table class="table table-sm table-borderless">
<thead>
<tr>
<th>
<button type="button" class="btn btn-light"> {{currentMonthInfo}} </button>
</th>
</tr>
</thead>
</table>
</th>
</tr>
<tr {{theadSelectDateButtonTrAttribute}}>
<td colspan="100" class="border-0">
<table class="table table-sm table-borderless">
<tr>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{previousYearText}}" data-change-date-button="true" data-number="{{previousYearButtonDateNumber}}" {{previousYearButtonDisabledAttribute}}> &lt;&lt; </button>
</th>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{previousMonthText}}" data-change-date-button="true" data-number="{{previousMonthButtonDateNumber}}" {{previousMonthButtonDisabledAttribute}}> &lt; </button>
</th>
<th style="width: 120px;">
<div class="dropdown">
<button type="button" class="btn btn-light btn-sm dropdown-toggle w-100" id="mdtp-month-selector-button-{{guid}}"
data-bs-toggle="dropdown" aria-expanded="false">
{{selectedMonthName}}
</button>
<div class="dropdown-menu" aria-labelledby="mdtp-month-selector-button-{{guid}}">
<a class="dropdown-item {{selectMonth1ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth1DateNumber}}">{{monthName1}}</a>
<a class="dropdown-item {{selectMonth2ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth2DateNumber}}">{{monthName2}}</a>
<a class="dropdown-item {{selectMonth3ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth3DateNumber}}">{{monthName3}}</a>
<div class="dropdown-divider"></div>
<a class="dropdown-item {{selectMonth4ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth4DateNumber}}">{{monthName4}}</a>
<a class="dropdown-item {{selectMonth5ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth5DateNumber}}">{{monthName5}}</a>
<a class="dropdown-item {{selectMonth6ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth6DateNumber}}">{{monthName6}}</a>
<div class="dropdown-divider"></div>
<a class="dropdown-item {{selectMonth7ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth7DateNumber}}">{{monthName7}}</a>
<a class="dropdown-item {{selectMonth8ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth8DateNumber}}">{{monthName8}}</a>
<a class="dropdown-item {{selectMonth9ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth9DateNumber}}">{{monthName9}}</a>
<div class="dropdown-divider"></div>
<a class="dropdown-item {{selectMonth10ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth10DateNumber}}">{{monthName10}}</a>
<a class="dropdown-item {{selectMonth11ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth11DateNumber}}">{{monthName11}}</a>
<a class="dropdown-item {{selectMonth12ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth12DateNumber}}">{{monthName12}}</a>
</div>
</div>
</th>
<th style="width: 50px;">
<button type="button" class="btn btn-light btn-sm w-100" jinget-pdtp-select-year-button {{selectYearButtonDisabledAttribute}}>{{selectedYear}}</button>
</th>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{nextMonthText}}" data-change-date-button="true" data-number="{{nextMonthButtonDateNumber}}" {{nextMonthButtonDisabledAttribute}}> &gt; </button>
</th>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{nextYearText}}" data-change-date-button="true" data-number="{{nextYearButtonDateNumber}}" {{nextYearButtonDisabledAttribute}}> &gt;&gt; </button>
</th>
</tr>
</table>
</td>
</tr>
</thead>
<tbody class="days">
<tr>
<td class="{{weekDayShortName1CssClass}}">{{weekDayShortName1}}</td>
<td>{{weekDayShortName2}}</td>
<td>{{weekDayShortName3}}</td>
<td>{{weekDayShortName4}}</td>
<td>{{weekDayShortName5}}</td>
<td>{{weekDayShortName6}}</td>
<td class="{{weekDayShortName7CssClass}}">{{weekDayShortName7}}</td>
</tr>
{{daysHtml}}
</tbody>
</table>
</td>`;

    protected getModal(): Element | null {
        return document.querySelector(`.modal[data-jinget-dtp-guid="${this.guid}"]`);
    }

    protected setModalHtml(title: string, datePickerBodyHtml: string, setting: JingetDateTimePickerSetting): void {
        const prevModalElement = this.getModal();
        if (prevModalElement == null) {
            let modalHtml = JingetDateTimePickerBase.modalHtmlTemplate;
            modalHtml = modalHtml.replace(/\{\{guid\}\}/img, this.guid);
            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = modalHtml;
            tempDiv.querySelector('[data-jinget-dtp-title] .modal-title')!.innerHTML = title;
            tempDiv.querySelector('[data-name="jinget-dtp-body"]')!.innerHTML = datePickerBodyHtml;
            document.querySelector('body')!.appendChild(tempDiv);
        } else {
            prevModalElement.querySelector('[data-jinget-dtp-title] .modal-title')!.innerHTML = title;
            prevModalElement.querySelector('[data-name="jinget-dtp-body"]')!.innerHTML = datePickerBodyHtml;
        }
        const modalDialogElement = document.querySelector(`[data-jinget-dtp-guid="${this.guid}"] .modal-dialog`);
        if (modalDialogElement != null) {
            if (setting.rangeSelector) {
                if (setting.rangeSelectorMonthsToShow[0] > 0 || setting.rangeSelectorMonthsToShow[1] > 0)
                    modalDialogElement.classList.add('modal-xl');
                else
                    modalDialogElement.classList.remove('modal-xl');
            } else {
                modalDialogElement.classList.remove('modal-xl');
            }
        } else {
            console.warn("jinget.bs.datetimepicker: element with `data-jinget-dtp-guid` selector not found !")
        }
    }

    protected getYearsBoxBodyHtml(setting: JingetDateTimePickerSetting, yearToStart: number): JingetDateTimePickerYearToSelectModel {
        // بدست آوردن اچ تی ام ال انتخاب سال
        // yearToStart سال شروع

        setting.yearOffset = Number(setting.yearOffset);

        const selectedDateToShow = JingetDpCommon.cloneDate(setting.selectedDateToShow);
        const disabledDateObj = this.getDisabledDateObject(setting);
        const disableBeforeDateTimeJson = disabledDateObj[0];
        const disableAfterDateTimeJson = disabledDateObj[1];
        let html = JingetDateTimePickerBase.dateTimePickerYearsToSelectHtmlTemplate;
        let yearsBoxHtml = '';
        let todayDateTimeJson: JingetDateTimeModel;
        let selectedDateTimeToShowJson: JingetDateTimeModel;
        let counter = 1;

        if (setting.isGregorian) {
            selectedDateTimeToShowJson = JingetDpCommon.getDateTimeJson1(selectedDateToShow);
            todayDateTimeJson = JingetDpCommon.getDateTimeJson1(new Date());
        } else {
            selectedDateTimeToShowJson = JingetDpCommon.getDateTimeJsonPersian1(selectedDateToShow);
            todayDateTimeJson = JingetDpCommon.getDateTimeJsonPersian1(new Date());
        }
        counter = 1;
        const yearStart = yearToStart ? yearToStart : todayDateTimeJson.year - setting.yearOffset;
        const yearEnd = yearToStart ? yearToStart + setting.yearOffset * 2 : todayDateTimeJson.year + setting.yearOffset;
        for (let i = yearStart; i < yearEnd; i++) {
            let disabledAttr = '';
            if (disableBeforeDateTimeJson != null) {
                disabledAttr = i < disableBeforeDateTimeJson.year ? 'disabled' : '';
            }
            if (!disabledAttr && disableAfterDateTimeJson != null) {
                disabledAttr = i > disableAfterDateTimeJson.year ? 'disabled' : '';
            }
            let currentYearDateTimeJson = JingetDpCommon.getDateTimeJson2(JingetDpCommon.convertToNumber2(i, selectedDateTimeToShowJson.month, JingetDpCommon.getDaysInMonthPersian(i, selectedDateTimeToShowJson.month)));
            let currentYearDisabledAttr = '';
            let yearText = setting.isGregorian ? i.toString() : JingetDpCommon.toPersianNumber(i);
            let yearDateNumber = JingetDpCommon.convertToNumber2(i, selectedDateTimeToShowJson.month, 1);
            let todayAttr = todayDateTimeJson.year == i ? 'data-current-year="true"' : ''
            let selectedYearAttr = selectedDateTimeToShowJson.year == i ? 'data-selected-year' : ''
            let selectedYearTitle = '';
            if (todayAttr)
                selectedYearTitle = setting.isGregorian ? JingetDpCommonProps.currentYearText : JingetDpCommonProps.currentYearTextPersian;
            if (disableBeforeDateTimeJson != undefined && disableBeforeDateTimeJson.year != undefined && currentYearDateTimeJson.year < disableBeforeDateTimeJson.year)
                currentYearDisabledAttr = 'disabled';
            if (disableAfterDateTimeJson != undefined && disableAfterDateTimeJson.year != undefined && currentYearDateTimeJson.year > disableAfterDateTimeJson.year)
                currentYearDisabledAttr = 'disabled';
            if (setting.disableBeforeToday && currentYearDateTimeJson.year < todayDateTimeJson.year)
                currentYearDisabledAttr = 'disabled';
            if (setting.disableAfterToday && currentYearDateTimeJson.year > todayDateTimeJson.year)
                currentYearDisabledAttr = 'disabled';
            if (counter == 1) yearsBoxHtml += '<tr>';
            yearsBoxHtml += `
<td class="text-center" title="${selectedYearTitle}" ${todayAttr} ${selectedYearAttr}>
  <button class="btn btn-sm btn-light w-100" type="button" data-change-date-button="true" data-number="${yearDateNumber}" ${currentYearDisabledAttr} ${disabledAttr}>${yearText}</button>
</td>
`;
            if (counter == 5) yearsBoxHtml += '</tr>';
            counter++;
            if (counter > 5) counter = 1;
        }
        html = html.replace(/\{\{yearsBoxHtml\}\}/img, yearsBoxHtml);
        html = html.replace(/\{\{cancelText\}\}/img, setting.isGregorian ? JingetDpCommonProps.cancelText : JingetDpCommonProps.cancelTextPersian);
        if (setting.inLine && setting.yearOffset > 15)
            html += '<div style="height: 30px;"></div>';
        return {
            yearStart,
            yearEnd,
            html
        };
    }

    protected getYearsBoxHeaderHtml(setting: JingetDateTimePickerSetting, yearStart: number, yearEnd: number): string {
        const yearsRangeText = ` ${yearStart} - ${yearEnd - 1} `;
        const disabledDateObj = this.getDisabledDateObject(setting);
        let html = JingetDateTimePickerBase.popoverHeaderSelectYearHtmlTemplate;
        html = html.replace(/\{{rtlCssClass\}\}/img, setting.isGregorian ? '' : 'rtl');
        html = html.replace(/\{{dirAttrValue\}\}/img, setting.isGregorian ? 'ltr' : 'rtl');
        html = html.replace(/\{\{yearsRangeText\}\}/img, setting.isGregorian ? yearsRangeText : JingetDpCommon.toPersianNumber(yearsRangeText));
        html = html.replace(/\{\{previousText\}\}/img, setting.isGregorian ? JingetDpCommonProps.previousText : JingetDpCommonProps.previousTextPersian);
        html = html.replace(/\{\{nextText\}\}/img, setting.isGregorian ? JingetDpCommonProps.nextText : JingetDpCommonProps.nextTextPersian);
        html = html.replace(/\{\{latestPreviousYear\}\}/img, yearStart > yearEnd ? yearEnd.toString() : yearStart.toString());
        html = html.replace(/\{\{latestNextYear\}\}/img, yearStart > yearEnd ? yearStart.toString() : yearEnd.toString());
        html = html.replace(/\{\{prevYearButtonAttr\}\}/img, disabledDateObj[0] != null && yearStart - 1 < disabledDateObj[0].year ? 'disabled' : '');
        html = html.replace(/\{\{nextYearButtonAttr\}\}/img, disabledDateObj[1] != null && yearEnd + 1 > disabledDateObj[1].year ? 'disabled' : '');
        return html;
    }

    private getDateTimePickerMonthHtml(setting: JingetDateTimePickerSetting, isNextMonth: boolean, isPrevMonth: boolean): string {
        let selectedDateToShow = JingetDpCommon.cloneDate(setting.selectedDateToShow);
        let selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
        let selectedDateTime = setting.selectedDate != undefined ? JingetDpCommon.cloneDate(setting.selectedDate) : undefined;
        let isNextOrPrevMonth = isNextMonth || isPrevMonth;
        let html = JingetDateTimePickerBase.dateTimePickerMonthTableHtmlTemplate;
        html = html.replace(/\{\{guid\}\}/img, this.guid);
        html = html.replace(/\{\{monthTdAttribute\}\}/img, isNextMonth ? 'data-next-month' : isPrevMonth ? 'data-prev-month' : '');
        html = html.replace(/\{\{monthNameAttribute\}\}/img, !isNextOrPrevMonth ? 'hidden' : '');
        html = html.replace(/\{\{theadSelectDateButtonTrAttribute\}\}/img, !isNextOrPrevMonth ? '' : 'hidden');
        html = html.replace(/\{\{weekDayShortName1CssClass\}\}/img, setting.isGregorian ? 'text-danger' : '');
        html = html.replace(/\{\{weekDayShortName7CssClass\}\}/img, !setting.isGregorian ? 'text-danger' : '');
        html = html.replace(/\{\{previousYearText\}\}/img, setting.isGregorian ? JingetDpCommonProps.previousYearText : JingetDpCommonProps.previousYearTextPersian);
        html = html.replace(/\{\{previousMonthText\}\}/img, setting.isGregorian ? JingetDpCommonProps.previousMonthText : JingetDpCommonProps.previousMonthTextPersian);
        html = html.replace(/\{\{nextMonthText\}\}/img, setting.isGregorian ? JingetDpCommonProps.nextMonthText : JingetDpCommonProps.nextMonthTextPersian);
        html = html.replace(/\{\{nextYearText\}\}/img, setting.isGregorian ? JingetDpCommonProps.nextYearText : JingetDpCommonProps.nextYearTextPersian);
        html = html.replace(/\{\{monthName1\}\}/img, JingetDpCommon.getMonthName(0, setting.isGregorian));
        html = html.replace(/\{\{monthName2\}\}/img, JingetDpCommon.getMonthName(1, setting.isGregorian));
        html = html.replace(/\{\{monthName3\}\}/img, JingetDpCommon.getMonthName(2, setting.isGregorian));
        html = html.replace(/\{\{monthName4\}\}/img, JingetDpCommon.getMonthName(3, setting.isGregorian));
        html = html.replace(/\{\{monthName5\}\}/img, JingetDpCommon.getMonthName(4, setting.isGregorian));
        html = html.replace(/\{\{monthName6\}\}/img, JingetDpCommon.getMonthName(5, setting.isGregorian));
        html = html.replace(/\{\{monthName7\}\}/img, JingetDpCommon.getMonthName(6, setting.isGregorian));
        html = html.replace(/\{\{monthName8\}\}/img, JingetDpCommon.getMonthName(7, setting.isGregorian));
        html = html.replace(/\{\{monthName9\}\}/img, JingetDpCommon.getMonthName(8, setting.isGregorian));
        html = html.replace(/\{\{monthName10\}\}/img, JingetDpCommon.getMonthName(9, setting.isGregorian));
        html = html.replace(/\{\{monthName11\}\}/img, JingetDpCommon.getMonthName(10, setting.isGregorian));
        html = html.replace(/\{\{monthName12\}\}/img, JingetDpCommon.getMonthName(11, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName1\}\}/img, JingetDpCommon.getWeekDayShortName(0, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName2\}\}/img, JingetDpCommon.getWeekDayShortName(1, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName3\}\}/img, JingetDpCommon.getWeekDayShortName(2, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName4\}\}/img, JingetDpCommon.getWeekDayShortName(3, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName5\}\}/img, JingetDpCommon.getWeekDayShortName(4, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName6\}\}/img, JingetDpCommon.getWeekDayShortName(5, setting.isGregorian));
        html = html.replace(/\{\{weekDayShortName7\}\}/img, JingetDpCommon.getWeekDayShortName(6, setting.isGregorian));
        const disabledDateObj = this.getDisabledDateObject(setting);
        let i = 0, j = 0, firstWeekDayNumber, cellNumber = 0, tdNumber = 0, selectedDateNumber = 0,
            selectedMonthName = '', todayDateTimeJson, // year, month, day, hour, minute, second
            dateTimeToShowJson, // year, month, day, hour, minute, second
            numberOfDaysInCurrentMonth = 0, numberOfDaysInPreviousMonth = 0, tr = document.createElement('TR'),
            td = document.createElement("TD"), daysHtml = '', currentDateNumber = 0, previousMonthDateNumber = 0,
            nextMonthDateNumber = 0, previousYearDateNumber = 0, nextYearDateNumber = 0,
            rangeSelectorStartDate = !setting.rangeSelector || setting.rangeSelectorStartDate == undefined ? undefined : JingetDpCommon.cloneDate(setting.rangeSelectorStartDate),
            rangeSelectorEndDate = !setting.rangeSelector || setting.rangeSelectorEndDate == undefined ? undefined : JingetDpCommon.cloneDate(setting.rangeSelectorEndDate),
            rangeSelectorStartDateNumber = 0, rangeSelectorEndDateNumber = 0, dayNumberInString = '0', dayOfWeek = '', // نام روز هفته
            monthsDateNumberAndAttr: any = {
                month1DateNumber: 0,
                month2DateNumber: 0,
                month3DateNumber: 0,
                month4DateNumber: 0,
                month5DateNumber: 0,
                month6DateNumber: 0,
                month7DateNumber: 0,
                month8DateNumber: 0,
                month9DateNumber: 0,
                month10DateNumber: 0,
                month11DateNumber: 0,
                month12DateNumber: 0,
                selectMonth1ButtonCssClass: '',
                selectMonth2ButtonCssClass: '',
                selectMonth3ButtonCssClass: '',
                selectMonth4ButtonCssClass: '',
                selectMonth5ButtonCssClass: '',
                selectMonth6ButtonCssClass: '',
                selectMonth7ButtonCssClass: '',
                selectMonth8ButtonCssClass: '',
                selectMonth9ButtonCssClass: '',
                selectMonth10ButtonCssClass: '',
                selectMonth11ButtonCssClass: '',
                selectMonth12ButtonCssClass: '',
            }, holidaysDateNumbers = [], disabledDatesNumber = [], specialDatesNumber = [],
            disableBeforeDateTimeJson = disabledDateObj[0], disableAfterDateTimeJson = disabledDateObj[1],
            previousYearButtonDisabledAttribute = '', previousMonthButtonDisabledAttribute = '',
            selectYearButtonDisabledAttribute = '', nextMonthButtonDisabledAttribute = '',
            nextYearButtonDisabledAttribute = '', isTrAppended = false;
        if (setting.isGregorian) {
            dateTimeToShowJson = JingetDpCommon.getDateTimeJson1(selectedDateToShowTemp);
            todayDateTimeJson = JingetDpCommon.getDateTimeJson1(new Date());
            firstWeekDayNumber = new Date(dateTimeToShowJson.year, dateTimeToShowJson.month - 1, 1).getDay();
            selectedDateNumber = !selectedDateTime ? 0 : JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(selectedDateTime));
            numberOfDaysInCurrentMonth = JingetDpCommon.getDaysInMonth(dateTimeToShowJson.year, dateTimeToShowJson.month - 1);
            numberOfDaysInPreviousMonth = JingetDpCommon.getDaysInMonth(dateTimeToShowJson.year, dateTimeToShowJson.month - 2);
            previousMonthDateNumber = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(JingetDpCommon.getLastDayDateOfPreviousMonth(selectedDateToShowTemp, true)));
            nextMonthDateNumber = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(JingetDpCommon.getFirstDayDateOfNextMonth(selectedDateToShowTemp, true)));
            selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            previousYearDateNumber = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(new Date(selectedDateToShowTemp.setFullYear(selectedDateToShowTemp.getFullYear() - 1))));
            selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            nextYearDateNumber = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(new Date(selectedDateToShowTemp.setFullYear(selectedDateToShowTemp.getFullYear() + 1))));
            selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            rangeSelectorStartDateNumber = !setting.rangeSelector || !rangeSelectorStartDate ? 0 : JingetDpCommon.convertToNumber3(rangeSelectorStartDate);
            rangeSelectorEndDateNumber = !setting.rangeSelector || !rangeSelectorEndDate ? 0 : JingetDpCommon.convertToNumber3(rangeSelectorEndDate);
            for (i = 1; i <= 12; i++) {
                monthsDateNumberAndAttr['month' + i.toString() + 'DateNumber'] = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(new Date(selectedDateToShowTemp.setMonth(i - 1))));
                selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            }
            for (i = 0; i < setting.holidays.length; i++) {
                holidaysDateNumbers.push(JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(setting.holidays[i])));
            }
            for (i = 0; i < setting.disabledDates.length; i++) {
                disabledDatesNumber.push(JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(setting.disabledDates[i])));
            }
            for (i = 0; i < setting.specialDates.length; i++) {
                specialDatesNumber.push(JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJson1(setting.specialDates[i])));
            }
        } else {
            dateTimeToShowJson = JingetDpCommon.getDateTimeJsonPersian1(selectedDateToShowTemp);
            todayDateTimeJson = JingetDpCommon.getDateTimeJsonPersian1(new Date());
            firstWeekDayNumber = JingetDpCommon.getDateTimeJsonPersian2(dateTimeToShowJson.year, dateTimeToShowJson.month, 1, 0, 0, 0).dayOfWeek;
            selectedDateNumber = !selectedDateTime ? 0 : JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(selectedDateTime));
            numberOfDaysInCurrentMonth = JingetDpCommon.getDaysInMonthPersian(dateTimeToShowJson.year, dateTimeToShowJson.month);
            numberOfDaysInPreviousMonth = JingetDpCommon.getDaysInMonthPersian(dateTimeToShowJson.year - 1, dateTimeToShowJson.month - 1);
            previousMonthDateNumber = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(JingetDpCommon.getLastDayDateOfPreviousMonth(selectedDateToShowTemp, false)));
            selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            nextMonthDateNumber = JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(JingetDpCommon.getFirstDayDateOfNextMonth(selectedDateToShowTemp, false)));
            selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            previousYearDateNumber = JingetDpCommon.convertToNumber2(dateTimeToShowJson.year - 1, dateTimeToShowJson.month, dateTimeToShowJson.day);
            nextYearDateNumber = JingetDpCommon.convertToNumber2(dateTimeToShowJson.year + 1, dateTimeToShowJson.month, dateTimeToShowJson.day);
            selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            rangeSelectorStartDateNumber = !setting.rangeSelector || !rangeSelectorStartDate ? 0 : JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(rangeSelectorStartDate));
            rangeSelectorEndDateNumber = !setting.rangeSelector || !rangeSelectorEndDate ? 0 : JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(rangeSelectorEndDate));
            for (i = 1; i <= 12; i++) {
                monthsDateNumberAndAttr['month' + i.toString() + 'DateNumber'] = JingetDpCommon.convertToNumber2(dateTimeToShowJson.year, i, JingetDpCommon.getDaysInMonthPersian(dateTimeToShowJson.year, i));
                selectedDateToShowTemp = JingetDpCommon.cloneDate(selectedDateToShow);
            }
            for (i = 0; i < setting.holidays.length; i++) {
                holidaysDateNumbers.push(JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(setting.holidays[i])));
            }
            for (i = 0; i < setting.disabledDates.length; i++) {
                disabledDatesNumber.push(JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(setting.disabledDates[i])));
            }
            for (i = 0; i < setting.specialDates.length; i++) {
                specialDatesNumber.push(JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(setting.specialDates[i])));
            }
        }
        let todayDateNumber = JingetDpCommon.convertToNumber1(todayDateTimeJson);
        let selectedYear = setting.isGregorian ? dateTimeToShowJson.year.toString() : JingetDpCommon.toPersianNumber(dateTimeToShowJson.year);
        let disableBeforeDateTimeNumber = !disableBeforeDateTimeJson ? undefined : JingetDpCommon.convertToNumber1(disableBeforeDateTimeJson);
        let disableAfterDateTimeNumber = !disableAfterDateTimeJson ? undefined : JingetDpCommon.convertToNumber1(disableAfterDateTimeJson);
        let currentMonthInfo = JingetDpCommon.getMonthName(dateTimeToShowJson.month - 1, setting.isGregorian) + ' ' + dateTimeToShowJson.year.toString();
        if (!setting.isGregorian)
            currentMonthInfo = JingetDpCommon.toPersianNumber(currentMonthInfo);
        selectedMonthName = JingetDpCommon.getMonthName(dateTimeToShowJson.month - 1, setting.isGregorian);
        if (setting.yearOffset <= 0) {
            previousYearButtonDisabledAttribute = 'disabled';
            nextYearButtonDisabledAttribute = 'disabled';
            selectYearButtonDisabledAttribute = 'disabled';
        }
        // روز های ماه قبل
        if (!setting.isGregorian && firstWeekDayNumber != 6 || setting.isGregorian && firstWeekDayNumber != 0) {
            if (setting.isGregorian)
                firstWeekDayNumber--;
            let previousMonthDateTimeJson = JingetDpCommon.addMonthToDateTimeJson(dateTimeToShowJson, -1, setting.isGregorian);
            for (i = numberOfDaysInPreviousMonth - firstWeekDayNumber; i <= numberOfDaysInPreviousMonth; i++) {
                currentDateNumber = JingetDpCommon.convertToNumber2(previousMonthDateTimeJson.year, previousMonthDateTimeJson.month, i);
                dayNumberInString = setting.isGregorian ? JingetDpCommon.zeroPad(i) : JingetDpCommon.toPersianNumber(JingetDpCommon.zeroPad(i));
                td = document.createElement('TD');
                td.setAttribute('data-nm', '');
                td.setAttribute('data-number', currentDateNumber.toString());
                td.innerHTML = dayNumberInString;
                if (setting.rangeSelector) {
                    if (currentDateNumber == rangeSelectorStartDateNumber || currentDateNumber == rangeSelectorEndDateNumber)
                        td.classList.add('selected-range-days-start-end');
                    else if (rangeSelectorStartDateNumber > 0 && rangeSelectorEndDateNumber > 0 && currentDateNumber > rangeSelectorStartDateNumber && currentDateNumber < rangeSelectorEndDateNumber)
                        td.classList.add('selected-range-days-nm');
                }
                // روز جمعه
                if (!setting.isGregorian && tdNumber == 6)
                    td.classList.add('text-danger');
                // روز یکشنبه
                else if (setting.isGregorian && tdNumber == 0)
                    td.classList.add('text-danger');
                tr.appendChild(td);
                cellNumber++;
                tdNumber++;
                if (tdNumber >= 7) {
                    tdNumber = 0;
                    daysHtml += tr.outerHTML;
                    isTrAppended = true;
                    tr = document.createElement('TR');
                }
            }
        }
        // روزهای ماه جاری
        for (i = 1; i <= numberOfDaysInCurrentMonth; i++) {
            if (tdNumber >= 7) {
                tdNumber = 0;
                daysHtml += tr.outerHTML;
                isTrAppended = true;
                tr = document.createElement('TR');
            }
            // عدد روز
            currentDateNumber = JingetDpCommon.convertToNumber2(dateTimeToShowJson.year, dateTimeToShowJson.month, i);
            dayNumberInString = setting.isGregorian ? JingetDpCommon.zeroPad(i) : JingetDpCommon.toPersianNumber(JingetDpCommon.zeroPad(i));
            td = document.createElement('TD');
            td.setAttribute('data-day', '');
            td.setAttribute('data-number', currentDateNumber.toString());
            td.innerHTML = dayNumberInString;
            // امروز
            if (currentDateNumber == todayDateNumber) {
                td.setAttribute('data-today', '');
                td.setAttribute('title', setting.isGregorian ? JingetDpCommonProps.todayText : JingetDpCommonProps.todayTextPersian);
                // اگر نام روز هفته انتخاب شده در تکس باکس قبل از تاریخ امروز باشد
                // نباید دیگر نام روز هفته تغییر کند
                if (!dayOfWeek)
                    dayOfWeek = JingetDpCommon.getWeekDayName(tdNumber - 1 < 0 ? 0 : tdNumber - 1, setting.isGregorian);
            }
            // روز از قبل انتخاب شده
            if (!setting.rangeSelector && selectedDateNumber == currentDateNumber) {
                td.setAttribute('data-jinget-dtp-selected-day', '');
                dayOfWeek = JingetDpCommon.getWeekDayName(tdNumber - 1 < 0 ? 0 : tdNumber - 1, setting.isGregorian);
            }
            // روزهای تعطیل
            for (j = 0; j < holidaysDateNumbers.length; j++) {
                if (holidaysDateNumbers[j] != currentDateNumber)
                    continue;
                td.classList.add('text-danger');
                break;
            }
            // روز جمعه شمسی
            if (!setting.isGregorian && tdNumber == 6) {
                td.classList.add('text-danger');
            }
            // روز یکشنبه میلادی
            else if (setting.isGregorian && tdNumber == 0) {
                td.classList.add('text-danger');
            }
            // روزهای غیر فعال شده
            if (setting.disableBeforeToday) {
                if (currentDateNumber < todayDateNumber)
                    td.setAttribute('disabled', '');
                if (nextMonthDateNumber < todayDateNumber)
                    nextMonthButtonDisabledAttribute = 'disabled';
                if (nextYearDateNumber < todayDateNumber)
                    nextYearButtonDisabledAttribute = 'disabled';
                if (previousMonthDateNumber < todayDateNumber)
                    previousMonthButtonDisabledAttribute = 'disabled';
                if (previousYearDateNumber < todayDateNumber)
                    previousYearButtonDisabledAttribute = 'disabled';
                for (j = 1; j <= 12; j++) {
                    if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] < todayDateNumber)
                        monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
                }
            }
            if (setting.disableAfterToday) {
                if (currentDateNumber > todayDateNumber)
                    td.setAttribute('disabled', '');
                if (nextMonthDateNumber > todayDateNumber)
                    nextMonthButtonDisabledAttribute = 'disabled';
                if (nextYearDateNumber > todayDateNumber)
                    nextYearButtonDisabledAttribute = 'disabled';
                if (previousMonthDateNumber > todayDateNumber)
                    previousMonthButtonDisabledAttribute = 'disabled';
                if (previousYearDateNumber > todayDateNumber)
                    previousYearButtonDisabledAttribute = 'disabled';
                for (j = 1; j <= 12; j++) {
                    if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] > todayDateNumber)
                        monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
                }
            }
            if (disableAfterDateTimeNumber) {
                if (currentDateNumber > disableAfterDateTimeNumber)
                    td.setAttribute('disabled', '');
                if (nextMonthDateNumber > disableAfterDateTimeNumber)
                    nextMonthButtonDisabledAttribute = 'disabled';
                if (nextYearDateNumber > disableAfterDateTimeNumber)
                    nextYearButtonDisabledAttribute = 'disabled';
                if (previousMonthDateNumber > disableAfterDateTimeNumber)
                    previousMonthButtonDisabledAttribute = 'disabled';
                if (previousYearDateNumber > disableAfterDateTimeNumber)
                    previousYearButtonDisabledAttribute = 'disabled';
                for (j = 1; j <= 12; j++) {
                    if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] > disableAfterDateTimeNumber)
                        monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
                }
            }
            if (disableBeforeDateTimeNumber) {
                if (currentDateNumber < disableBeforeDateTimeNumber)
                    td.setAttribute('disabled', '');
                if (nextMonthDateNumber < disableBeforeDateTimeNumber)
                    nextMonthButtonDisabledAttribute = 'disabled';
                if (nextYearDateNumber < disableBeforeDateTimeNumber)
                    nextYearButtonDisabledAttribute = 'disabled';
                if (previousMonthDateNumber < disableBeforeDateTimeNumber)
                    previousMonthButtonDisabledAttribute = 'disabled';
                if (previousYearDateNumber < disableBeforeDateTimeNumber)
                    previousYearButtonDisabledAttribute = 'disabled';
                for (j = 1; j <= 12; j++) {
                    if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] < disableBeforeDateTimeNumber)
                        monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
                }
            }
            for (j = 0; j < disabledDatesNumber.length; j++) {
                if (currentDateNumber == disabledDatesNumber[j])
                    td.setAttribute('disabled', '');
            }
            for (j = 0; j < specialDatesNumber.length; j++) {
                if (currentDateNumber == specialDatesNumber[j])
                    td.setAttribute('data-special-date', '');
            }
            if (setting.disabledDays != null && setting.disabledDays.length > 0 && setting.disabledDays.indexOf(tdNumber) >= 0) {
                td.setAttribute('disabled', '');
            }
            // \\
            if (setting.rangeSelector) {
                if (currentDateNumber == rangeSelectorStartDateNumber || currentDateNumber == rangeSelectorEndDateNumber)
                    td.classList.add('selected-range-days-start-end');
                else if (rangeSelectorStartDateNumber > 0 && rangeSelectorEndDateNumber > 0 && currentDateNumber > rangeSelectorStartDateNumber && currentDateNumber < rangeSelectorEndDateNumber)
                    td.classList.add('selected-range-days');
            }
            tr.appendChild(td);
            isTrAppended = false;
            tdNumber++;
            cellNumber++;
        }
        if (tdNumber >= 7) {
            tdNumber = 0;
            daysHtml += tr.outerHTML;
            isTrAppended = true;
            tr = document.createElement('TR');
        }
        // روزهای ماه بعد
        let nextMonthDateTimeJson = JingetDpCommon.addMonthToDateTimeJson(dateTimeToShowJson, 1, setting.isGregorian);
        for (i = 1; i <= 42 - cellNumber; i++) {
            dayNumberInString = setting.isGregorian ? JingetDpCommon.zeroPad(i) : JingetDpCommon.toPersianNumber(JingetDpCommon.zeroPad(i));
            currentDateNumber = JingetDpCommon.convertToNumber2(nextMonthDateTimeJson.year, nextMonthDateTimeJson.month, i);
            td = document.createElement('TD');
            td.setAttribute('data-nm', '');
            td.setAttribute('data-number', currentDateNumber.toString());
            td.innerHTML = dayNumberInString;
            if (setting.rangeSelector) {
                if (currentDateNumber == rangeSelectorStartDateNumber || currentDateNumber == rangeSelectorEndDateNumber)
                    td.classList.add('selected-range-days-start-end');
                else if (rangeSelectorStartDateNumber > 0 && rangeSelectorEndDateNumber > 0 && currentDateNumber > rangeSelectorStartDateNumber && currentDateNumber < rangeSelectorEndDateNumber)
                    td.classList.add('selected-range-days-nm');
            }
            // روز جمعه
            if (!setting.isGregorian && tdNumber == 6)
                td.classList.add('text-danger');
            // روز یکشنبه
            else if (setting.isGregorian && tdNumber == 0)
                td.classList.add('text-danger');
            tr.appendChild(td);
            tdNumber++;
            if (tdNumber >= 7) {
                tdNumber = 0;
                daysHtml += tr.outerHTML;
                isTrAppended = true;
                tr = document.createElement('TR');
            }
        }
        if (!isTrAppended) {
            daysHtml += tr.outerHTML;
            isTrAppended = true;
        }
        html = html.replace(/\{\{currentMonthInfo\}\}/img, currentMonthInfo);
        html = html.replace(/\{\{selectedYear\}\}/img, selectedYear);
        html = html.replace(/\{\{selectedMonthName\}\}/img, selectedMonthName);
        html = html.replace(/\{\{daysHtml\}\}/img, daysHtml);
        html = html.replace(/\{\{previousYearButtonDisabledAttribute\}\}/img, previousYearButtonDisabledAttribute);
        html = html.replace(/\{\{previousYearButtonDateNumber\}\}/img, previousYearDateNumber.toString());
        html = html.replace(/\{\{previousMonthButtonDisabledAttribute\}\}/img, previousMonthButtonDisabledAttribute);
        html = html.replace(/\{\{previousMonthButtonDateNumber\}\}/img, previousMonthDateNumber.toString());
        html = html.replace(/\{\{selectYearButtonDisabledAttribute\}\}/img, selectYearButtonDisabledAttribute);
        html = html.replace(/\{\{nextMonthButtonDisabledAttribute\}\}/img, nextMonthButtonDisabledAttribute);
        html = html.replace(/\{\{nextMonthButtonDateNumber\}\}/img, nextMonthDateNumber.toString());
        html = html.replace(/\{\{nextYearButtonDisabledAttribute\}\}/img, nextYearButtonDisabledAttribute);
        html = html.replace(/\{\{nextYearButtonDateNumber\}\}/img, nextYearDateNumber.toString());
        html = html.replace(/\{\{dropDownMenuMonth1DateNumber\}\}/img, monthsDateNumberAndAttr.month1DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth2DateNumber\}\}/img, monthsDateNumberAndAttr.month2DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth3DateNumber\}\}/img, monthsDateNumberAndAttr.month3DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth4DateNumber\}\}/img, monthsDateNumberAndAttr.month4DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth5DateNumber\}\}/img, monthsDateNumberAndAttr.month5DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth6DateNumber\}\}/img, monthsDateNumberAndAttr.month6DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth7DateNumber\}\}/img, monthsDateNumberAndAttr.month7DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth8DateNumber\}\}/img, monthsDateNumberAndAttr.month8DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth9DateNumber\}\}/img, monthsDateNumberAndAttr.month9DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth10DateNumber\}\}/img, monthsDateNumberAndAttr.month10DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth11DateNumber\}\}/img, monthsDateNumberAndAttr.month11DateNumber);
        html = html.replace(/\{\{dropDownMenuMonth12DateNumber\}\}/img, monthsDateNumberAndAttr.month12DateNumber);
        html = html.replace(/\{\{selectMonth1ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth1ButtonCssClass);
        html = html.replace(/\{\{selectMonth2ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth2ButtonCssClass);
        html = html.replace(/\{\{selectMonth3ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth3ButtonCssClass);
        html = html.replace(/\{\{selectMonth4ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth4ButtonCssClass);
        html = html.replace(/\{\{selectMonth5ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth5ButtonCssClass);
        html = html.replace(/\{\{selectMonth6ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth6ButtonCssClass);
        html = html.replace(/\{\{selectMonth7ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth7ButtonCssClass);
        html = html.replace(/\{\{selectMonth8ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth8ButtonCssClass);
        html = html.replace(/\{\{selectMonth9ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth9ButtonCssClass);
        html = html.replace(/\{\{selectMonth10ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth10ButtonCssClass);
        html = html.replace(/\{\{selectMonth11ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth11ButtonCssClass);
        html = html.replace(/\{\{selectMonth12ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth12ButtonCssClass);
        return html;
    }

    protected getDateTimePickerBodyHtml(setting: JingetDateTimePickerSetting): string {
        let selectedDateToShow = JingetDpCommon.cloneDate(setting.selectedDateToShow);
        let html = JingetDateTimePickerBase.dateTimePickerHtmlTemplate;

        html = html.replace(/\{\{inlineAttr\}\}/img, setting.inLine ? 'data-inline' : '');
        html = html.replace(/\{\{rtlCssClass\}\}/img, setting.isGregorian ? '' : 'rtl');
        html = html.replace(/\{\{selectedDateStringAttribute\}\}/img, setting.inLine ? '' : 'hidden');
        html = html.replace(/\{\{goTodayText\}\}/img, setting.isGregorian ? JingetDpCommonProps.goTodayText : JingetDpCommonProps.goTodayTextPersian);
        html = html.replace(/\{\{timePickerAttribute\}\}/img, setting.enableTimePicker ? '' : 'hidden');

        const disabledDays = this.getDisabledDateObject(setting);
        let title = '';
        let todayDateString = '';
        let todayDateTimeJson: JingetDateTimeModel;
        let selectedDateTimeToShowJson: JingetDateTimeModel;
        let disableBeforeDateTimeJson: JingetDateTimeModel | null = disabledDays[0];
        let disableAfterDateTimeJson: JingetDateTimeModel | null = disabledDays[1];

        if (setting.isGregorian) {
            selectedDateTimeToShowJson = JingetDpCommon.getDateTimeJson1(selectedDateToShow);
            todayDateTimeJson = JingetDpCommon.getDateTimeJson1(new Date());
        } else {
            selectedDateTimeToShowJson = JingetDpCommon.getDateTimeJsonPersian1(selectedDateToShow);
            todayDateTimeJson = JingetDpCommon.getDateTimeJsonPersian1(new Date());
        }

        title = this.getPopoverHeaderTitle(setting);
        todayDateString = `${setting.isGregorian ? 'Today,' : 'امروز،'} ${todayDateTimeJson.day} ${JingetDpCommon.getMonthName(todayDateTimeJson.month - 1, setting.isGregorian)} ${todayDateTimeJson.year}`;
        if (!setting.isGregorian) {
            todayDateString = JingetDpCommon.toPersianNumber(todayDateString);
        }

        if (disableAfterDateTimeJson != undefined && disableAfterDateTimeJson.year <= selectedDateTimeToShowJson.year && disableAfterDateTimeJson.month < selectedDateTimeToShowJson.month)
            selectedDateToShow = setting.isGregorian ? new Date(disableAfterDateTimeJson.year, disableAfterDateTimeJson.month - 1, 1) : JingetDpCommon.getDateTime1(disableAfterDateTimeJson.year, disableAfterDateTimeJson.month, disableAfterDateTimeJson.day);

        if (disableBeforeDateTimeJson != undefined && disableBeforeDateTimeJson.year >= selectedDateTimeToShowJson.year && disableBeforeDateTimeJson.month > selectedDateTimeToShowJson.month)
            selectedDateToShow = setting.isGregorian ? new Date(disableBeforeDateTimeJson.year, disableBeforeDateTimeJson.month - 1, 1) : JingetDpCommon.getDateTime1(disableBeforeDateTimeJson.year, disableBeforeDateTimeJson.month, disableBeforeDateTimeJson.day);

        let monthsTdHtml = '';
        // let tempSelectedDateToShow = JingetDpCommon.cloneDate(selectedDateToShow);
        let numberOfNextMonths = setting.rangeSelectorMonthsToShow[1] <= 0 ? 0 : setting.rangeSelectorMonthsToShow[1];
        let numberOfPrevMonths = setting.rangeSelectorMonthsToShow[0] <= 0 ? 0 : setting.rangeSelectorMonthsToShow[0];
        numberOfPrevMonths *= -1;
        for (let i1 = numberOfPrevMonths; i1 < 0; i1++) {
            setting.selectedDateToShow = JingetDpCommon.addMonthToDateTime(JingetDpCommon.cloneDate(selectedDateToShow), i1, setting.isGregorian);
            monthsTdHtml += this.getDateTimePickerMonthHtml(setting, false, true);
        }
        setting.selectedDateToShow = JingetDpCommon.cloneDate(selectedDateToShow);
        monthsTdHtml += this.getDateTimePickerMonthHtml(setting, false, false);
        for (let i2 = 1; i2 <= numberOfNextMonths; i2++) {
            setting.selectedDateToShow = JingetDpCommon.addMonthToDateTime(JingetDpCommon.cloneDate(selectedDateToShow), i2, setting.isGregorian);
            monthsTdHtml += this.getDateTimePickerMonthHtml(setting, true, false);
        }
        // setting.selectedDateToShow = JingetDpCommon.cloneDate(selectedDateToShow);

        let totalMonthNumberToShow = Math.abs(numberOfPrevMonths) + 1 + numberOfNextMonths;
        let monthTdStyle = totalMonthNumberToShow > 1 ? 'width: ' + (100 / totalMonthNumberToShow).toString() + '%;' : '';

        monthsTdHtml = monthsTdHtml.replace(/\{\{monthTdStyle\}\}/img, monthTdStyle);

        html = html.replace(/\{\{dtpInlineHeader\}\}/img, title);
        html = html.replace(/\{\{todayDateString\}\}/img, todayDateString);
        html = html.replace(/\{\{time\}\}/img, `${JingetDpCommon.zeroPad(selectedDateTimeToShowJson.hour)}:${JingetDpCommon.zeroPad(selectedDateTimeToShowJson.minute)}`);
        html = html.replace(/\{\{monthsTdHtml\}\}/img, monthsTdHtml);

        return html;
    }

    protected getPopoverHeaderTitle(setting: JingetDateTimePickerSetting): string {
        let selectedDateToShowJson: JingetDateTimeModel;
        let title;
        if (setting.isGregorian) {
            selectedDateToShowJson = JingetDpCommon.getDateTimeJson1(setting.selectedDateToShow);
        } else {
            selectedDateToShowJson = JingetDpCommon.getDateTimeJsonPersian1(setting.selectedDateToShow);
        }
        if (setting.rangeSelector) {
            const startDate = JingetDpCommon.addMonthToDateTime(setting.selectedDateToShow, -setting.rangeSelectorMonthsToShow[0], setting.isGregorian);
            const endDate = JingetDpCommon.addMonthToDateTime(setting.selectedDateToShow, setting.rangeSelectorMonthsToShow[1], setting.isGregorian);
            let statDateJson: JingetDateTimeModel;
            let endDateJson: JingetDateTimeModel;
            if (setting.isGregorian) {
                statDateJson = JingetDpCommon.getDateTimeJson1(startDate);
                endDateJson = JingetDpCommon.getDateTimeJson1(endDate);
            } else {
                statDateJson = JingetDpCommon.getDateTimeJsonPersian1(startDate);
                endDateJson = JingetDpCommon.getDateTimeJsonPersian1(endDate);
            }
            const startMonthName = JingetDpCommon.getMonthName(statDateJson.month - 1, setting.isGregorian);
            const endMonthName = JingetDpCommon.getMonthName(endDateJson.month - 1, setting.isGregorian);
            title = `${startMonthName} ${statDateJson.year} - ${endMonthName} ${endDateJson.year}`;
        } else
            title = `${JingetDpCommon.getMonthName(selectedDateToShowJson.month - 1, setting.isGregorian)} ${selectedDateToShowJson.year}`;
        if (!setting.isGregorian)
            title = JingetDpCommon.toPersianNumber(title);
        return title;
    }


    protected abstract getDisabledDateObject(setting: JingetDateTimePickerSetting): [JingetDateTimeModel | null, JingetDateTimeModel | null];
}