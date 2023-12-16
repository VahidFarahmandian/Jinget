function gotoDateRange(id) {

    var done = false;
    const targetNode = document.querySelector("body");
    const config = { attributes: true, childList: true, subtree: true };
    var observer = new MutationObserver(() => {
        var CONTROL_INTERVAL = setInterval(function () {
            if (done == false) {

                var startSelector = document.querySelectorAll("[id='" + id + "'] .mud-button-date")[0];
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

function loadScript(args) {

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = args.url;

    // Then bind the event to the callback function.
    // There are several events for cross browser compatibility.
    script.onreadystatechange = args.callback;
    script.onload = args.callback;

    var jinget = document.getElementById("jinget");

    //fire the loading
    jinget.after(script);
}

loadScript(
    {
        url: '../_content/MudBlazor/MudBlazor.min.js',
        callback: loadScript(
            {
                url: '_content/Jinget.Blazor/js/jinget.jalali.picker.date.js'
            })
    });