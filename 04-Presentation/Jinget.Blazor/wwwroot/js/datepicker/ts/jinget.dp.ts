import {Modal, Popover} from "bootstrap";
import {JingetDpCommon} from "./core/jinget.dp.base.common";
import {JingetDateTimePickerBase} from "./core/jinget.dp.base";
import {JingetDpBaseJalaliCalendar} from "./core/jinget.dp.base.jalali.calendar";
import {JingetDpDataMapper} from "./core/jinget.dp.base.data.mapper";
import {JingetDateTimeModel, JingetDateTimePickerSetting} from "./core/jinget.dp.base.models";

export class JingetDateTimePicker extends JingetDateTimePickerBase {
    bsPopover: Popover | null = null;
    bsModal: Modal | null = null;

    /**
     * دریافت اینستنس پاپ آور بوت استرپ
     */
    getBsPopoverInstance = (): Popover | null => this.bsPopover;

    /**
     * دریافت اینستنس مدال بوت استرپ
     * در صورتی که آپشن modalMode را صحیح کرده باشید
     */
    getBsModalInstance = (): Modal | null => this.bsModal;

    constructor(element: Element, setting: JingetDateTimePickerSetting) {
        super(element, setting);
        if (element != undefined && setting != undefined) {
            JingetDpDataMapper.set(this.guid, this);
            this.initializeBsPopover(setting);
        }
    }

    private initializeBsPopover(setting: JingetDateTimePickerSetting): void {

        // Validation
        if (setting.rangeSelector && (setting.toDate || setting.fromDate)) throw new Error(`JingetDateTimePicker => You can not set true 'toDate' or 'fromDate' and 'rangeSelector' together`);
        if (setting.toDate && setting.fromDate) throw new Error(`JingetDateTimePicker => You can not set true 'toDate' and 'fromDate' together`);
        if (!setting.groupId && (setting.toDate || setting.fromDate)) throw new Error(`JingetDateTimePicker => When you set 'toDate' or 'fromDate' true, you have to set 'groupId'`);

        // آپشن هایی که باید همان لحظه تغییر اعمال شوند

        if (setting.disabled) {
            this.element.setAttribute("disabled", '');
        } else {
            this.element.removeAttribute("disabled");
        }
        if (setting.toDate || setting.fromDate) {
            setting.rangeSelector = false;
            this.element.setAttribute("data-jinget-dtp-group", setting.groupId);
            if (setting.toDate)
                this.element.setAttribute("data-to-date", 'true');
            else if (setting.fromDate)
                this.element.setAttribute("data-from-date", 'true');
        }
        if (!setting.rangeSelector) {
            setting.rangeSelectorMonthsToShow = [0, 0];
        }

        setTimeout(() => {
            this.dispose();
            const title = this.getPopoverHeaderTitle(setting);
            let datePickerBodyHtml = this.getDateTimePickerBodyHtml(setting);
            let tempDiv = document.createElement('div');
            tempDiv.innerHTML = datePickerBodyHtml;
            const dropDowns = tempDiv.querySelectorAll('.dropdown>button');
            dropDowns.forEach(e => {
                if (setting.disabled) {
                    e.setAttribute('disabled', '');
                    e.classList.add('disabled');
                } else {
                    e.removeAttribute('disabled');
                    e.classList.remove('disabled');
                }
            });
            datePickerBodyHtml = tempDiv.innerHTML;
            if (setting.modalMode) {
                this.setModalHtml(title, datePickerBodyHtml, setting);
                this.bsPopover = null;
                setTimeout(() => {
                    const el = this.getModal();
                    if (el != null) {
                        this.bsModal = new Modal(el);
                        this.enableMainEvents();
                    }
                }, 200);
            } else if (setting.inLine) {
                this.bsPopover = null;
                this.element.innerHTML = datePickerBodyHtml;
                this.enableInLineEvents();
            } else {
                this.bsPopover = new Popover(this.element, {
                    container: 'body',
                    content: datePickerBodyHtml,
                    title: title,
                    html: true,
                    placement: setting.placement,
                    trigger: 'manual',
                    template: JingetDateTimePickerBase.popoverHtmlTemplate,
                    sanitize: false,
                });
                this.enableMainEvents();
            }
            JingetDateTimePicker.setSelectedData(setting);
            this.tempTitleString = title;
        }, setting.inLine ? 10 : 100);
    }

    private static getSelectedDateTimeTextFormatted(setting: JingetDateTimePickerSetting): string {
        if (setting.selectedDate == undefined) return '';
        if (!setting.enableTimePicker) {
            setting.selectedDate.setHours(0);
            setting.selectedDate.setMinutes(0);
            setting.selectedDate.setSeconds(0);
        }
        if (setting.rangeSelector && setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate != undefined)
            return JingetDpCommon.getDateTimeString(!setting.isGregorian ? JingetDpCommon.getDateTimeJsonPersian1(setting.rangeSelectorStartDate) : JingetDpCommon.getDateTimeByDate(setting.rangeSelectorStartDate), setting.textFormat, setting.isGregorian, setting.persianNumber) + ' - ' +
                JingetDpCommon.getDateTimeString(!setting.isGregorian ? JingetDpCommon.getDateTimeJsonPersian1(setting.rangeSelectorEndDate) : JingetDpCommon.getDateTimeByDate(setting.rangeSelectorEndDate), setting.textFormat, setting.isGregorian, setting.persianNumber);
        return JingetDpCommon.getDateTimeString(!setting.isGregorian ? JingetDpCommon.getDateTimeJsonPersian1(setting.selectedDate) : JingetDpCommon.getDateTimeByDate(setting.selectedDate), setting.textFormat, setting.isGregorian, setting.persianNumber);
    }

    // دریافت رشته تاریخ انتخاب شده
    private static getSelectedDateFormatted(setting: JingetDateTimePickerSetting): string {
        if ((!setting.rangeSelector && !setting.selectedDate) ||
            (setting.rangeSelector && !setting.rangeSelectorStartDate && !setting.rangeSelectorEndDate))
            return '';
        if (setting.rangeSelector)
            return JingetDpCommon.getDateTimeString(JingetDpCommon.getDateTimeByDate(setting.rangeSelectorStartDate!), setting.dateFormat, true, setting.persianNumber) + ' - ' +
                JingetDpCommon.getDateTimeString(JingetDpCommon.getDateTimeByDate(setting.rangeSelectorEndDate!), setting.dateFormat, true, setting.persianNumber);
        return JingetDpCommon.getDateTimeString(JingetDpCommon.getDateTimeByDate(setting.selectedDate!), setting.dateFormat, true, setting.persianNumber);
    }


    private static setSelectedData(setting: JingetDateTimePickerSetting): void {
        const targetTextElement = setting.targetTextSelector ? document.querySelector(setting.targetTextSelector) : undefined;
        const targetDateElement = setting.targetDateSelector ? document.querySelector(setting.targetDateSelector) : undefined;
        const changeEvent = new Event('change');
        if (targetTextElement != undefined) {
            const dateTimeTextFormat = this.getSelectedDateTimeTextFormatted(setting);
            switch (targetTextElement.tagName.toLowerCase()) {
                case 'input':
                    (<any>targetTextElement).value = dateTimeTextFormat;
                    break;
                default:
                    targetTextElement.innerHTML = dateTimeTextFormat;
                    break;
            }
            targetTextElement.dispatchEvent(changeEvent);
        }
        if (targetDateElement != undefined) {
            const dateTimeFormat = JingetDpCommon.toEnglishNumber(this.getSelectedDateFormatted(setting));
            switch (targetDateElement.tagName.toLowerCase()) {
                case 'input':
                    (<any>targetDateElement).value = dateTimeFormat;
                    break;
                default:
                    targetDateElement.innerHTML = dateTimeFormat;
                    break;
            }
            targetDateElement.dispatchEvent(changeEvent);
        }
    }

    private changeMonth = (element: Element): void => {
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) {
            return;
        }
        if (instance.setting.disabled) return;
        const dateNumber = Number(element.getAttribute('data-number'));
        const setting = instance.setting;
        let selectedDateToShow = JingetDpCommon.cloneDate(setting.selectedDateToShow);
        selectedDateToShow = JingetDpCommon.getDateTime4(dateNumber, selectedDateToShow, setting.isGregorian);
        setting.selectedDateToShow = JingetDpCommon.cloneDate(selectedDateToShow);
        JingetDpDataMapper.set(instance.guid, instance);
        this.updateCalendarBodyHtml(element, setting);
        if (setting.calendarViewOnChange != undefined)
            setting.calendarViewOnChange(selectedDateToShow);
    }
    // کلیک روی روزها
    // انتخاب روز
    private selectDay = (element: Element): void => {
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) return;
        if (instance.setting.disabled || element.getAttribute('disabled') != undefined)
            return;
        let dateNumber = Number(element.getAttribute('data-number'));
        const setting = instance.setting;
        const disabled = element.getAttribute('disabled') != undefined;
        if (setting.selectedDate != undefined && !setting.enableTimePicker) {
            setting.selectedDate.setHours(0);
            setting.selectedDate.setMinutes(0);
            setting.selectedDate.setSeconds(0);
        }
        let selectedDateJson = !setting.selectedDate ? null : JingetDpCommon.getDateTimeByDate(setting.selectedDate);
        let selectedDateToShow = !setting.selectedDateToShow ? new Date() : JingetDpCommon.cloneDate(setting.selectedDateToShow);
        let selectedDateToShowJson = JingetDpCommon.getDateTimeByDate(selectedDateToShow);
        if (disabled) {
            if (setting.onDayClick != undefined) setting.onDayClick(setting);
            return;
        }
        selectedDateToShow = JingetDpCommon.getDateTime4(dateNumber, selectedDateToShow, setting.isGregorian);
        if (setting.rangeSelector) { // اگر رنج سلکتور فعال بود
            if (setting.rangeSelectorStartDate != null && setting.rangeSelectorEndDate != null) {
                setting.selectedRangeDate = [];
                setting.rangeSelectorStartDate = null;
                setting.rangeSelectorEndDate = null;
                let closestSelector = '[data-jinget-dtp]';
                if (setting.inLine)
                    closestSelector = '[data-jinget-dtp-guid]';
                element.closest(closestSelector)?.querySelectorAll('td.selected-range-days-start-end,td.selected-range-days')
                    .forEach(e => {
                        e.classList.remove('selected-range-days');
                        e.classList.remove('selected-range-days-start-end');
                    });
            }
            if (setting.rangeSelectorStartDate == undefined) {
                element.classList.add('selected-range-days-start-end');
                setting.rangeSelectorStartDate = JingetDpCommon.cloneDate(selectedDateToShow);
                setting.selectedDate = JingetDpCommon.cloneDate(selectedDateToShow);
                setting.selectedDateToShow = JingetDpCommon.cloneDate(selectedDateToShow);
            } else if (setting.rangeSelectorEndDate == undefined) {
                if (setting.rangeSelectorStartDate.getTime() >= selectedDateToShow.getTime())
                    return;
                element.classList.add('selected-range-days-start-end');
                setting.rangeSelectorEndDate = JingetDpCommon.cloneDate(selectedDateToShow);
                JingetDateTimePicker.setSelectedData(setting);
            }
            JingetDpDataMapper.set(instance.guid, instance);
            if (setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate != undefined) {
                setting.selectedRangeDate = [JingetDpCommon.cloneDate(setting.rangeSelectorStartDate), JingetDpCommon.cloneDate(setting.rangeSelectorEndDate)];
                if (!setting.inLine)
                    instance.hide();
                else
                    this.updateCalendarBodyHtml(element, setting);
            }
            return;
        }
        setting.selectedDate = JingetDpCommon.cloneDate(selectedDateToShow);
        if (setting.selectedDate != undefined && !setting.enableTimePicker) {
            setting.selectedDate.setHours(0);
            setting.selectedDate.setMinutes(0);
            setting.selectedDate.setSeconds(0);
        }
        setting.selectedDateToShow = JingetDpCommon.cloneDate(selectedDateToShow);
        if (selectedDateJson != undefined) {
            if (setting.enableTimePicker) {
                selectedDateJson.hour = selectedDateToShowJson.hour;
                selectedDateJson.minute = selectedDateToShowJson.minute;
                selectedDateJson.second = selectedDateToShowJson.second;
            } else {
                selectedDateJson.hour = 0;
                selectedDateJson.minute = 0;
                selectedDateJson.second = 0;
            }
            setting.selectedDate.setHours(selectedDateJson.hour);
            setting.selectedDate.setMinutes(selectedDateJson.minute);
            setting.selectedDate.setSeconds(selectedDateJson.second);
        }
        JingetDpDataMapper.set(instance.guid, instance);
        JingetDateTimePicker.setSelectedData(setting);
        element.setAttribute('data-jinget-dtp-selected-day', '');
        if (setting.toDate || setting.fromDate) {
            // وقتی روی روز یکی از تقویم ها کلیک می شود
            // باید تقویم دیگر نیز تغییر کند و روزهایی از آن غیر فعال شود
            const toDateElement = document.querySelector(`[data-jinget-dtp-group="${setting.groupId}"][data-to-date]`);
            const fromDateElement = document.querySelector(`[data-jinget-dtp-group="${setting.groupId}"][data-from-date]`);
            if (setting.fromDate && toDateElement != undefined) {
                const instance = JingetDateTimePicker.getInstance(toDateElement);
                if (instance != null) {
                    if (setting.inLine)
                        this.updateCalendarBodyHtml(toDateElement, instance.setting);
                    else
                        instance.initializeBsPopover(instance.setting);
                }
            } else if (setting.toDate && fromDateElement != undefined) {
                const instance = JingetDateTimePicker.getInstance(fromDateElement);
                if (instance != null) {
                    if (setting.inLine)
                        this.updateCalendarBodyHtml(fromDateElement, instance.setting);
                    else
                        instance.initializeBsPopover(instance.setting);
                }
            } else
                this.updateCalendarBodyHtml(element, setting);
        } else {
            this.updateCalendarBodyHtml(element, setting, true);
        }
        if (setting.onDayClick != undefined)
            setting.onDayClick(setting);
        if (!setting.inLine) {
            instance.hide();
        } else {
            // حذف روزهای انتخاب شده در تقویم این لاین
            element.closest(`[data-jinget-dtp-guid="${this.guid}"]`)!
                .querySelectorAll('[data-day]')
                .forEach(e => e.removeAttribute('data-jinget-dtp-selected-day'));
        }
    }

    // هاور روی روزها
    private hoverOnDays = (e: Event): void => {
        const element = <Element>e.target;
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) return;
        const setting = instance.setting;

        if (element.getAttribute('disabled') != undefined || !setting.rangeSelector ||
            (setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate != undefined)) return;

        const dateNumber = Number(element.getAttribute('data-number'));
        const allDayElements: Element[] = [].slice.call(document.querySelectorAll('td[data-day]'));
        allDayElements.forEach(e => {
            e.classList.remove('selected-range-days');
            e.classList.remove('selected-range-days-nm');
        });

        const allNextOrPrevMonthDayElements: Element[] = [].slice.call(document.querySelectorAll('td[data-nm]'));
        allNextOrPrevMonthDayElements.forEach(e => {
            e.classList.remove('selected-range-days');
            e.classList.remove('selected-range-days-nm');
        });

        const rangeSelectorStartDate = !setting.rangeSelectorStartDate ? undefined : JingetDpCommon.cloneDate(setting.rangeSelectorStartDate);
        const rangeSelectorEndDate = !setting.rangeSelectorEndDate ? undefined : JingetDpCommon.cloneDate(setting.rangeSelectorEndDate);
        let rangeSelectorStartDateNumber;
        let rangeSelectorEndDateNumber;

        if (setting.isGregorian) {
            rangeSelectorStartDateNumber = !rangeSelectorStartDate ? 0 : JingetDpCommon.convertToNumber3(rangeSelectorStartDate);
            rangeSelectorEndDateNumber = !rangeSelectorEndDate ? 0 : JingetDpCommon.convertToNumber3(rangeSelectorEndDate);
        } else {
            rangeSelectorStartDateNumber = !rangeSelectorStartDate ? 0 : JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(rangeSelectorStartDate));
            rangeSelectorEndDateNumber = !rangeSelectorEndDate ? 0 : JingetDpCommon.convertToNumber1(JingetDpCommon.getDateTimeJsonPersian1(rangeSelectorEndDate));
        }

        if (rangeSelectorStartDateNumber > 0 && dateNumber > rangeSelectorStartDateNumber) {
            for (let i1 = rangeSelectorStartDateNumber; i1 <= dateNumber; i1++) {
                allDayElements.filter(e => e.getAttribute('data-number') == i1.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
                    .forEach(e => e.classList.add('selected-range-days'));
                allNextOrPrevMonthDayElements.filter(e => e.getAttribute('data-number') == i1.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
                    .forEach(e => e.classList.add('selected-range-days-nm'));
            }
        } else if (rangeSelectorEndDateNumber > 0 && dateNumber < rangeSelectorEndDateNumber) {
            for (let i2 = dateNumber; i2 <= rangeSelectorEndDateNumber; i2++) {
                allDayElements.filter(e => e.getAttribute('data-number') == i2.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
                    .forEach(e => e.classList.add('selected-range-days'));
                allNextOrPrevMonthDayElements.filter(e => e.getAttribute('data-number') == i2.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
                    .forEach(e => e.classList.add('selected-range-days-nm'));
            }
        }

    }

    //برو به امروز
    private goToday = (e: Event): void => {
        const element = <Element>e.target;
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) return;
        const setting = instance.setting;
        setting.selectedDateToShow = new Date();
        JingetDpDataMapper.set(instance.guid, instance);
        this.updateCalendarBodyHtml(element, setting);
    }
    // عوض کردن ساعت
    private timeChanged = (e: Event): void => {
        const element = <Element>e.target;
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) return;
        const setting = instance.setting;
        const value: string = (<any>element).value;
        if (!setting.enableTimePicker) return;
        if (setting.selectedDateToShow == undefined)
            setting.selectedDateToShow = new Date();
        let hour = Number(value.substring(0, 2));
        let minute = Number(value.substring(3, 2));
        setting.selectedDateToShow = new Date(setting.selectedDateToShow.setHours(hour));
        setting.selectedDateToShow = new Date(setting.selectedDateToShow.setMinutes(minute));
        if (setting.selectedDate == undefined)
            setting.selectedDate = new Date();
        setting.selectedDate = new Date(setting.selectedDate.setHours(hour));
        setting.selectedDate = new Date(setting.selectedDate.setMinutes(minute));
        JingetDpDataMapper.set(instance.guid, instance);
        JingetDateTimePicker.setSelectedData(setting);
    }

    private enableMainEvents(): void {
        if (this.setting.inLine) return;
        if (this.bsPopover != null) {
            this.element.addEventListener('shown.bs.popover', this.popoverOrModalShownEvent);
            this.element.addEventListener('hidden.bs.popover', this.popoverOrModalHiddenEvent);
            this.element.addEventListener('inserted.bs.popover', this.popoverInsertedEvent);
            this.element.addEventListener('click', this.showPopoverEvent, true);
        } else if (this.bsModal != null) {
            const modalElement = this.getModal();
            if (modalElement == null) {
                console.error("jinget.dp: `modalElement` not found!");
                return;
            }
            modalElement.addEventListener('shown.bs.modal', this.popoverOrModalShownEvent);
            modalElement.addEventListener('hidden.bs.modal', this.popoverOrModalHiddenEvent);
        }
    }

    private popoverInsertedEvent = (e: Event): void => {
        const element = <Element>e.target;
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) return;
        const setting = instance.setting;
        this.hideYearsBox(element, setting);
    }
    private popoverOrModalShownEvent = (): void => {
        this.enableEvents();
    }
    private popoverOrModalHiddenEvent = (): void => {
        this.disableEvents();
    }

    private enableInLineEvents(): void {
        if (!this.setting.inLine) return;
        setTimeout(() => {
            const dtp = document.querySelector(`[data-jinget-dtp-guid="${this.guid}"]`);
            if (dtp != null) {
                dtp.querySelector('[data-jinget-dtp-time]')?.addEventListener('change', this.timeChanged, false);
                dtp.addEventListener('click', this.selectCorrectClickEvent);
                dtp.querySelectorAll('[data-day]').forEach(e => e.addEventListener('mouseenter', this.hoverOnDays, true));
            }
        }, 100);
    }

    private updateCalendarBodyHtml = (element: Element, setting: JingetDateTimePickerSetting, updatePopoverContent = false): void => {
        const calendarHtml = this.getDateTimePickerBodyHtml(setting);
        const dtpInlineHeader = calendarHtml.match(/<th jinget-dtp-inline-header\b[^>]*>(.*?)<\/th>/img)![0];
        this.tempTitleString = dtpInlineHeader;
        if (!setting.inLine && updatePopoverContent && !setting.modalMode) {
            const popover = this.getBsPopoverInstance();
            if (!popover) {
                console.error(" jinget.dp: `BsPopoverInstance` is null!");
                return;
            }
            setTimeout(() => {
                popover.setContent({
                    '.popover-header': dtpInlineHeader,
                    '.popover-body': calendarHtml
                });
            }, 100);
            return;
        }
        let containerElement = element.closest('[data-name= "jinget-dtp-body"]');
        if (containerElement == null) {
            containerElement = element.closest('[data-jinget-dtp-guid]');
            if (containerElement == null) {
                console.error(" jinget.dp: `data-jinget-dtp-guid` element not found !")
                return;
            }
            if (setting.modalMode)
                containerElement = containerElement.querySelector('[data-name= "jinget-dtp-body"]');
        }
        if (containerElement == null) {
            console.error(" jinget.dp: `data-jinget-dtp-guid` element not found!")
            return;
        }
        this.setPopoverHeaderHtml(element, setting, dtpInlineHeader.trim());
        containerElement.innerHTML = calendarHtml;
        this.hideYearsBox(element, setting);
        this.enableEvents();
        this.enableInLineEvents();
    }

    private enableEvents(): void {
        if (this.setting.inLine) return;
        setTimeout(() => {
            document.addEventListener('click', this.selectCorrectClickEvent, false);
            document.querySelector('html')!.addEventListener('click', this.hidePopoverEvent, true);
            document.querySelectorAll('[data-jinget-dtp-time]').forEach(e => e.addEventListener('change', this.timeChanged, false));
            document.querySelectorAll('[data-jinget-dtp] [data-day]').forEach(e => e.addEventListener('mouseenter', this.hoverOnDays, true));
        }, 500);
    }

    private disableEvents(): void {
        document.removeEventListener('click', this.selectCorrectClickEvent);
        document.querySelector('html')!.removeEventListener('click', this.hidePopoverEvent);
        document.querySelectorAll('[data-jinget-dtp-time]')?.forEach(e => e.removeEventListener('change', this.timeChanged));
        document.querySelectorAll('[data-jinget-dtp] [data-day]').forEach(e => e.removeEventListener('mouseenter', this.hoverOnDays));
        const dtp = document.querySelector(`[data-jinget-dtp-guid="${this.guid}"]`);
        if (dtp != null) {
            dtp.removeEventListener('click', this.selectCorrectClickEvent, false);
            dtp.querySelectorAll('[data-day]')?.forEach(e => e.removeEventListener('mouseenter', this.hoverOnDays, true));
        }
    }

    private selectCorrectClickEvent = (e: Event): void => {
        const element = <Element>e.target;
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) return;
        if ((instance.setting.disabled || instance.element.getAttribute('disabled') != undefined))
            return;
        if (element.getAttribute('jinget-pdtp-select-year-button') != null) {
            this.showYearsBox(element);
        } else if (element.getAttribute('data-jinget-dtp-go-today') != null) {
            this.goToday(e);
        } else if (element.getAttribute('data-day') != null) {
            this.selectDay(element);
        } else if (element.getAttribute('data-jinget-hide-year-list-box')) {
            this.hideYearsBox(element, instance.setting);
        } else if (element.getAttribute('data-change-date-button')) {
            this.changeMonth(element);
        } else if (element.getAttribute('data-year-range-button-change') != null && element.getAttribute('disabled') == null) {
            this.changeYearList(element);
        }
    }
    private showPopoverEvent = (e: Event): void => {
        JingetDpDataMapper.getAll().forEach(i => i.hide());
        const element = <Element>e.target;
        const instance = JingetDateTimePicker.getInstance(element);
        if (instance == null || instance.setting.disabled) return;
        instance.show();
    }
    private hidePopoverEvent = (e: Event): void => {
        const element = <Element>e.target;
        if (element.tagName == 'HTML') {
            JingetDpDataMapper.getAll().forEach(i => !i.setting.modalMode ? i.hide() : () => {
            });
            return;
        }
        const isWithinDatePicker = element.closest('[data-jinget-dtp]') != null || element.getAttribute('data-jinget-dtp-guid') != null || element.getAttribute('data-jinget-dtp-go-today') != null;
        if (!isWithinDatePicker) {
            JingetDpDataMapper.getAll().forEach(i => i.hide());
        }
    }

    //نمایش تقویم
    show(): void {
        this.bsModal?.show();
        this.bsPopover?.show();
    }

    //مخفی کردن تقویم
    hide(): void {
        this.bsModal?.hide();
        this.bsPopover?.hide();
    }

    //مخفی یا نمایش تقویم
    toggle(): void {
        if (this.bsPopover == null) return;
        this.bsPopover.toggle();
    }

    //فعال کردن تقویم
    enable(): void {
        this.setting.disabled = false;
        this.element.removeAttribute("disabled");
        JingetDpDataMapper.set(this.guid, this);
        if (this.bsPopover != null)
            this.bsPopover.enable();
    }

    //غیرفعال کردن تقویم
    disable(): void {
        this.setting.disabled = true;
        this.element.setAttribute("disabled", '');
        JingetDpDataMapper.set(this.guid, this);
        if (this.bsPopover != null)
            this.bsPopover.disable();
    }

    //بروز کردن محل قرار گرفتن تقویم
    updatePosition(): void {
        this.bsPopover?.update();
        this.bsModal?.handleUpdate();
    }

    /**
     * به روز کردن متن نمایش تاریخ روز انتخاب شده
     */
    updateSelectedDateText(): void {
        JingetDateTimePicker.setSelectedData(this.setting);
    }

    /**
     * از بین بردن تقویم
     */
    dispose(): void {
        if (this.bsPopover != null)
            this.bsPopover.dispose();
        if (this.bsModal != null)
            this.bsModal.dispose();
        this.element.removeEventListener('click', this.showPopoverEvent);
        this.bsPopover = null;
        this.bsModal = null;
    }

    /**
     * دریافت متن تاریخ انتخاب شده
     */
    getText(): string {
        return JingetDateTimePicker.getSelectedDateFormatted(this.setting);
    }

    /**
     * دریافت آبجکت تاریخ انتخاب شده
     */
    getSelectedDate(): Date | null {
        return this.setting.selectedDate;
    }

    /**
     * دریافت آبجکت های تاریخ های انتخاب شده در مد رنج سلکتور
     */
    getSelectedDateRange(): Date[] {
        return this.setting.selectedRangeDate;
    }

    /**
     * بروز کردن تاریخ انتخاب شده
     */
    setDate(date: Date): void {
        this.updateOptions({
            selectedDate: date,
            selectedDateToShow: date
        });
    }

    /**
     * بروز کردن تاریخ انتخاب شده با استفاده از تاریخ شمسی
     */
    setDatePersian(yearPersian: number, monthPersian: number, dayPersian: number): void {
        const gregorianDateJson = JingetDpBaseJalaliCalendar.toGregorian(yearPersian, monthPersian, dayPersian);
        console.log(gregorianDateJson);
        const date = new Date(gregorianDateJson.year, gregorianDateJson.month - 1, gregorianDateJson.day);
        this.updateOptions({
            selectedDate: date,
            selectedDateToShow: date
        });
    }

    /**
     * بروز کردن رنج تاریخی انتخاب شده
     */
    setDateRange(startDate: Date, endDate: Date): void {
        this.updateOptions({
            selectedDate: startDate,
            selectedDateToShow: startDate,
            selectedRangeDate: [startDate, endDate]
        });
    }

    /**
     * حذف تاریخ انتخاب شده
     */
    clearDate(): void {
        this.updateOptions({
            selectedDate: null,
            selectedDateToShow: new Date(),
        });
    }

    /**
     * بروز کردن تنظیمات تقویم
     * @param optionName نام آپشن مورد نظر
     * @param value مقدار
     */
    updateOption(optionName: string, value: any): void {
        if (!optionName) return;
        value = JingetDpCommon.correctOptionValue(optionName, value);
        (<any>this.setting)[optionName] = value;
        JingetDpDataMapper.set(this.guid, this);
        this.initializeBsPopover(this.setting);
    }

    /**
     * بروز کردن تنظیمات تقویم
     * @param options تنظیمات مورد نظر
     */
    updateOptions(options: any): void {
        Object.keys(options).forEach((key) => {
            (<any>this.setting)[key] = JingetDpCommon.correctOptionValue(key, (<any>options)[key]);
        });
        JingetDpDataMapper.set(this.guid, this);
        this.initializeBsPopover(this.setting);
    }

    private setPopoverHeaderHtml = (element: Element, setting: JingetDateTimePickerSetting, htmlString: string): void => {
        // element = المانی که روی آن فعالیتی انجام شده و باید عنوان تقویم آن عوض شود    
        if (this.bsPopover != null) {
            const popoverElement = this.getPopover(element);
            if (popoverElement == null) return;
            popoverElement.querySelector('[data-jinget-dtp-title]')!.innerHTML = htmlString;
        } else if (setting.inLine) {
            let inlineTitleBox = element.closest('[data-jinget-dtp-guid]')!.querySelector('[data-name="dtp-years-container"]')!;
            inlineTitleBox.innerHTML = htmlString;
            inlineTitleBox.classList.remove('w-0');
        } else if (setting.modalMode) {
            let inlineTitleBox = element.closest('[data-jinget-dtp-guid]')!.querySelector('[data-jinget-dtp-title] .modal-title')!;
            inlineTitleBox.innerHTML = htmlString;
        }
    }

    private getPopover(element: Element): Element | null {
        let popoverId = element.getAttribute('aria-describedby');
        if (popoverId == undefined || popoverId == '')
            return element.closest('[data-jinget-dtp]');
        return document.getElementById(popoverId.toString());
    }


    protected override getDisabledDateObject(setting: JingetDateTimePickerSetting): [JingetDateTimeModel | null, JingetDateTimeModel | null] {
        let disableBeforeDateTimeJson = JingetDpCommon.getLesserDisableBeforeDate(setting);
        let disableAfterDateTimeJson = JingetDpCommon.getBiggerDisableAfterDate(setting);
        // بررسی پراپرتی های از تاریخ، تا تاریخ
        if ((setting.fromDate || setting.toDate) && setting.groupId) {
            const toDateElement = document.querySelector(`[data-jinget-dtp-group="${setting.groupId}"][data-to-date]`);
            const fromDateElement = document.querySelector(`[data-jinget-dtp-group="${setting.groupId}"][data-from-date]`);
            if (toDateElement != null && setting.fromDate) {
                const toDateSetting = JingetDateTimePicker.getInstance(toDateElement)?.setting;
                const toDateSelectedDate = !toDateSetting ? null : toDateSetting.selectedDate;
                disableAfterDateTimeJson = !toDateSelectedDate ? null : setting.isGregorian ? JingetDpCommon.getDateTimeByDate(toDateSelectedDate) : JingetDpCommon.getDateTimeJsonPersian1(toDateSelectedDate);
            } else if (fromDateElement != null && setting.toDate) {
                const fromDateSetting = JingetDateTimePicker.getInstance(fromDateElement)?.setting;
                const fromDateSelectedDate = !fromDateSetting ? null : fromDateSetting.selectedDate;
                disableBeforeDateTimeJson = !fromDateSelectedDate ? null : setting.isGregorian ? JingetDpCommon.getDateTimeByDate(fromDateSelectedDate) : JingetDpCommon.getDateTimeJsonPersian1(fromDateSelectedDate);
            }
        }
        return [disableBeforeDateTimeJson, disableAfterDateTimeJson];
    }

    private changeYearList = (element: Element): void => {
        // کلیک روی دکمه های عوض کردن رنج سال انتخابی
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) {
            return;
        }
        const setting = instance.setting;
        const isNext = element.getAttribute('data-year-range-button-change') == '1';
        const yearStart = Number(element.getAttribute('data-year'));
        const yearsToSelectObject = this.getYearsBoxBodyHtml(setting, isNext ? yearStart : yearStart - setting.yearOffset * 2);
        if (setting.inLine)
            element.closest('[data-jinget-dtp-guid]')!.querySelector('[data-jinget-dtp-year-list-box]')!.innerHTML = yearsToSelectObject.html;
        else
            element.closest('[data-jinget-dtp]')!.querySelector('[data-jinget-dtp-year-list-box]')!.innerHTML = yearsToSelectObject.html;
        this.setPopoverHeaderHtml(element, setting, this.getYearsBoxHeaderHtml(setting, yearsToSelectObject.yearStart, yearsToSelectObject.yearEnd));
    }
    private showYearsBox = (element: Element): void => {
        const instance = JingetDateTimePicker.getInstance(element);
        if (!instance) {
            return;
        }
        const setting = instance.setting;
        const mdDatePickerContainer = setting.inLine ? element.closest('[data-jinget-dtp-guid]') : element.closest('[data-jinget-dtp]');
        if (mdDatePickerContainer == null) return;
        this.tempTitleString = setting.inLine
            ? mdDatePickerContainer.querySelector(' [jinget-dtp-inline-header]')!.textContent!.trim()
            : mdDatePickerContainer.querySelector('[data-jinget-dtp-title]')!.textContent!.trim();
        const yearsToSelectObject = this.getYearsBoxBodyHtml(setting, 0);
        const dateTimePickerYearsToSelectHtml = yearsToSelectObject.html;
        const dateTimePickerYearsToSelectContainer = mdDatePickerContainer.querySelector('[data-jinget-dtp-year-list-box]');
        this.setPopoverHeaderHtml(element, setting, this.getYearsBoxHeaderHtml(setting, yearsToSelectObject.yearStart, yearsToSelectObject.yearEnd));
        dateTimePickerYearsToSelectContainer!.innerHTML = dateTimePickerYearsToSelectHtml;
        dateTimePickerYearsToSelectContainer!.classList.remove('w-0');
        if (setting.inLine) {
            mdDatePickerContainer.classList.add('overflow-hidden')
            dateTimePickerYearsToSelectContainer!.classList.add('inline');
        } else if (setting.modalMode) {
            mdDatePickerContainer.querySelector('[data-name= "jinget-dtp-body"]')!.setAttribute('hidden', '');
        } else {
            dateTimePickerYearsToSelectContainer!.classList.remove('inline');
        }
    }
    private hideYearsBox = (element: Element, setting: JingetDateTimePickerSetting): void => {
        if (setting.inLine) {
            const dtpInLine = element.closest('[data-jinget-dtp-guid]');
            if (dtpInLine == null) return;
            const dtpInlineHeaderElement = dtpInLine.querySelector(' [jinget-dtp-inline-header]');
            if (this.tempTitleString && dtpInlineHeaderElement != null)
                dtpInlineHeaderElement.innerHTML = this.tempTitleString;
            const yearListBoxElement = dtpInLine.querySelector('[data-jinget-dtp-year-list-box]');
            if (yearListBoxElement != null) {
                yearListBoxElement.classList.add('w-0');
                yearListBoxElement.innerHTML = '';
            }
            const inlineYearsContainerElement = dtpInLine.querySelector('[data-name="dtp-years-container"]');
            if (inlineYearsContainerElement != null) {
                inlineYearsContainerElement.classList.add('w-0');
                inlineYearsContainerElement.innerHTML = '';
            }
            dtpInLine.classList.remove('overflow-hidden');
        } else {
            const popoverOrModalElement = setting.modalMode ? this.getModal() : this.getPopover(element);
            if (popoverOrModalElement == null) return;
            if (this.tempTitleString) {
                if (setting.modalMode)
                    popoverOrModalElement.querySelector('[data-jinget-dtp-title] .modal-title')!.innerHTML = this.tempTitleString;
                else {
                    popoverOrModalElement.querySelector('[data-jinget-dtp-title]')!.innerHTML = this.tempTitleString;
                }
                popoverOrModalElement.querySelector('[data-name= "jinget-dtp-body"]')!.removeAttribute('hidden');
            }
            const yearListBox = popoverOrModalElement.querySelector('[data-jinget-dtp-year-list-box]');
            yearListBox!.classList.add('w-0');
            yearListBox!.innerHTML = '';
        }
    }


    /**
     * دریافت اینستنس تقویم از روی المانی که تقویم روی آن فعال شده است
     * @param element المانی که تقویم روی آن فعال شده
     * @returns اینستنس تقویم
     */
    public static getInstance(element: Element): any {
        let elementGuid = element.getAttribute('data-jinget-dtp-guid');
        if (!elementGuid) {
            elementGuid = element.closest('[data-jinget-dtp-guid]')?.getAttribute('data-jinget-dtp-guid') ?? null;
            if (!elementGuid) {
                const id = element.closest('[data-jinget-dtp]')?.getAttribute('id');
                if (!id)
                    return null;
                elementGuid = document.querySelector('[aria-describedby="' + id + '"]')?.getAttribute('data-jinget-dtp-guid') ?? null;
                if (!elementGuid)
                    return null;
            }
        }
        return JingetDpDataMapper.get(elementGuid);
    }
}