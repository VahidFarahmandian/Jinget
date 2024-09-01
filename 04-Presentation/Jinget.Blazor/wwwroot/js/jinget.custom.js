window.toggleModal = (params = { id, show } = {}) => {
    if (params.show)
        $('#' + params.id).modal('show');
    else
        $('#' + params.id).modal('hide');
}


/*jinget.json.visualizer START*/
window.toJsonVisualizer = (params = { id, data, collapsed=false, rootCollapsable=true, withQuotes=false, withLinks=true, bigNumbers=false } = {}) => {
    $('#' + params.id).jsonVisualizer(
        params.data, {

        //all nodes are collapsed at html generation
        collapsed: params.collapsed,

        //allow root element to be collasped
        rootCollapsable: params.rootCollapsable,

        //all JSON keys are surrounded with double quotation marks
        withQuotes: params.withQuotes,

        //all values that are valid links will be clickable
        withLinks: params.withLinks,

        //support different libraries for big numbers,
        //if true display the real number only, false shows object containing big number with all fields instead of number only.
        bigNumbers: params.bigNumbers
    });
}

/*jinget.json.visualizer END*/

/*select2 START*/
window.initJingetDropDownList = (params = {
    dotnet, id, isSearchable = false, isRtl = true, noResultText='Nothing to display!',
    searchPlaceholderText='', parentElementId=''
} = {}) => {
    var element = $('#' + params.id).select2(
        {
            dir: params.isRtl ? 'rtl' : 'ltr',
            closeOnSelect: true,
            theme: 'outlined',
            width: 'resolve',
            dropdownPosition: 'auto',

            //example: Bootstrap modals tend to steal focus from other elements outside of the modal.
            //Since by default, Select2 attaches the dropdown menu to the <body> element, it is considered "outside of the modal".
            //To avoid this problem, you may attach the dropdown to the modal itself with the dropdownParent setting
            dropdownParent: params.parentElementId == '' ? null : $('#' + params.parentElementId),

            minimumResultsForSearch: params.isSearchable ? 0 : Infinity,
            language: {
                noResults: function () {
                    return "<div class='select2-no-result-text'>" + params.noResultText; +"</div>";
                }
            },
            escapeMarkup: function (markup) {
                return markup;
            }
        }).off('select2:select').on('select2:select', function (e) {
            params.dotnet.invokeMethodAsync('OnJSDropDownListSelectedItemChanged', e.params.data.id);
        }).off('select2:open').on('select2:open', function (e) {
            $('input.select2-search__field').prop('placeholder', params.searchPlaceholderText);
        });
};
window.jinget_blazor_dropdownlist_selectItem = (id, value) => {
    $('#' + id).val(value).trigger("change");
};
window.jinget_blazor_dropdownlist_clear = (id) => {
    $('#' + id).val(null).trigger("change");
};
/*select2 END*/

/*jinget_select2tree START*/

window.initJingetDropDownListTree = (params = {
    dotnet, id, isSearchable = false, isRtl = true,
    noResultText='Nothing to display!',
    searchPlaceholderText='', parentElementId=''
} = {}) => {
    var element = $('#' + params.id).jinget_select2tree(
        {
            id: params.id,
            dir: params.isRtl ? 'rtl' : 'ltr',
            closeOnSelect: true,
            theme: 'outlined',
            width: 'resolve',
            dropdownPosition: 'below',

            //example: Bootstrap modals tend to steal focus from other elements outside of the modal.
            //Since by default, Select2 attaches the dropdown menu to the <body> element, it is considered "outside of the modal".
            //To avoid this problem, you may attach the dropdown to the modal itself with the dropdownParent setting
            dropdownParent: params.parentElementId == '' ? null : $('#' + params.parentElementId),

            minimumResultsForSearch: params.isSearchable ? 0 : Infinity,
            searchPlaceholderText: params.searchPlaceholderText,
            language: {
                noResults: function () {
                    return "<div class='select2-no-result-text'>" + params.noResultText; +"</div>";
                }
            },
            escapeMarkup: function (markup) {
                return markup;
            }
        });
    $('#' + params.id).off('select2:select').on('select2:select', function (e) {
        params.dotnet.invokeMethodAsync('OnJSDropDownListSelectedItemChanged', e.params.data.id);
        jinget_blazor_dropdownlist_tree_selectItem(e.target.id, e.params.data.id);
    });
};
window.jinget_blazor_dropdownlist_tree_selectItem = (id, value) => {
    $('#' + id).val(value).trigger("change");
};
window.jinget_blazor_dropdownlist_tree_clear = (id) => {
    $('#' + id).val('---').trigger("change");
};

/*jinget_select2tree END*/

/*localStorage/sessionStorage START*/

window.removeAll_localStorageKeys = () => {
    localStorage.clear();
}

window.removeAll_sessionStorageKeys = () => {
    sessionStorage.clear();
}

window.getAll_localStorageKeys = () => {
    return JSON.stringify(localStorage);
}

window.getAll_sessionStorageKeys = () => {
    return JSON.stringify(sessionStorage);
}

/*localStorage/sessionStorage END*/

/*DateRange/Date Picker START*/
function gotoDate(id) {

    var done = false;
    const targetNode = document.querySelector("body");
    const config = { attributes: true, childList: true, subtree: true };
    var observer = new MutationObserver(() => {
        var CONTROL_INTERVAL = setInterval(function () {
            if (done == false) {

                var startSelector = document.querySelectorAll("[id='" + id + "'] .mud-button-date")[0];
                if (startSelector == undefined)
                    return;
                //    startSelector = document.querySelectorAll("[id='" + id + "']")[0];

                var selectedRangeStartYear = parseInt(startSelector.textContent.substring(0, 4));
                var selectedRangeStartMonth = parseInt(startSelector.textContent.substring(5, 7));

                var currentDateButton = document.getElementsByClassName('mud-picker-calendar-header-switch')[0].querySelectorAll('.mud-picker-slide-transition')[0];
                var currentYear = parseInt(currentDateButton.textContent.substring(0, 4));
                var currentMonth = parseInt(GetMonthNumber(currentDateButton.textContent.substring(5).trim()));

                var yearDistance = selectedRangeStartYear - currentYear;
                yearDistance = isNaN(yearDistance) ? 0 : yearDistance;
                var monthDistance = selectedRangeStartMonth - currentMonth;
                monthDistance = isNaN(monthDistance) ? 0 : monthDistance;

                if (yearDistance != 0 || monthDistance != 0) {

                    document.getElementsByClassName('mud-picker-content')[0].style = 'visibility:hidden';
                    currentDateButton.click();

                    setTimeout(() => {
                        var yearChangerButtons = document.getElementsByClassName('mud-picker-calendar-header-switch')[0].querySelectorAll('.mud-flip-x-rtl');
                        if (yearDistance < 0) {
                            while (yearDistance != 0) {
                                var GotoPrevYear = yearChangerButtons[0];
                                if (GotoPrevYear != undefined) {
                                    GotoPrevYear.click();
                                    yearDistance++;
                                }
                            }
                        }
                        else if (yearDistance > 0) {
                            while (yearDistance != 0) {
                                var GotoNextYear = yearChangerButtons[1];
                                if (GotoNextYear != undefined) {
                                    GotoNextYear.click();
                                    yearDistance--;
                                }
                            }
                        }
                        setTimeout(() => {
                            var monthSelector = document.getElementsByClassName('mud-picker-month-container')[0].querySelectorAll('.mud-picker-month');
                            monthSelector[selectedRangeStartMonth - 1].click();
                            document.getElementsByClassName('mud-picker-content')[0].style = 'visibility:visible';
                        }, 50);
                    }, 100);
                }
                done = true;
            }
            clearInterval(CONTROL_INTERVAL);
            observer.disconnect();
        }, 100);
    });
    observer.observe(targetNode, config);
}

function GetMonthNumber(monthName) {
    switch (monthName) {

        case 'فروردین': return 1;
        case 'اردیبهشت': return 2;
        case 'خرداد': return 3;
        case 'تیر': return 4;
        case 'مرداد': return 5;
        case 'شهریور': return 6;
        case 'مهر': return 7;
        case 'آبان': return 8;
        case 'آذر': return 9;
        case 'دی': return 10;
        case 'بهمن': return 11;
        case 'اسفند': return 12;
    }
}
function refreshDatePicker() {

    var done = false;
    const targetNode = document.querySelector("body");
    const config = { attributes: true, childList: true, subtree: true };
    var observer = new MutationObserver(() => {
        var CONTROL_INTERVAL = setInterval(function () {
            if (document.querySelectorAll('.mud-picker-calendar-day:not(.mud-hidden)').length > 0) {
                //if month days are not start from 1
                if (document.querySelectorAll('.mud-picker-calendar-day:not(.mud-hidden)')[0].textContent != '1') {
                    if (done == false) {
                        document.getElementsByClassName('mud-picker-nav-button-next')[0].click();
                        done = true;
                    }
                    clearInterval(CONTROL_INTERVAL);
                    observer.disconnect();
                }
                else {
                    clearInterval(CONTROL_INTERVAL);
                    observer.disconnect();
                }
            }
        }, 100);
    });
    observer.observe(targetNode, config);
}

function toEnglishNumber(id) {
    const targetNode = document.querySelector("body");
    const config = { attributes: true, childList: true, subtree: true };
    var observer = new MutationObserver(() => {
        var CONTROL_INTERVAL = setInterval(function () {
            if (document.getElementById(id) != undefined) {// && container != undefined) {
                [...document.querySelectorAll("[id='" + id + "'] *")].forEach((el) => {
                    el.setAttribute('style', 'font-family:sans-serif !important;');
                });
                [...document.querySelectorAll("[id='" + id + "'] .mud-picker-calendar-transition *")].forEach((el) => {
                    el.setAttribute('style', 'font-family:sans-serif !important;');
                });
                clearInterval(CONTROL_INTERVAL);
                observer.disconnect();
            }
        }, 100);
    });
    observer.observe(targetNode, config);
}

/*Picker END*/