window.toggleModal = (params = {id, show} = {}) => {
    if (params.show)
        $('#' + params.id).modal('show');
    else
        $('#' + params.id).modal('hide');
}


/*jinget.json.visualizer START*/
window.toJsonVisualizer = (params = {
    id,
    data,
    collapsed = false,
    rootCollapsable = true,
    withQuotes = false,
    withLinks = true,
    bigNumbers = false
} = {}) => {
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
    dotnet, id, isSearchable = false, isRtl = true, noResultText = 'Nothing to display!',
    searchPlaceholderText = '', parentElementId = ''
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
            dropdownParent: params.parentElementId === '' ? null : $('#' + params.parentElementId),

            minimumResultsForSearch: params.isSearchable ? 0 : Infinity,
            language: {
                noResults: function () {
                    return "<div class='select2-no-result-text'>" + params.noResultText + "</div>";
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
    noResultText = 'Nothing to display!',
    searchPlaceholderText = '', parentElementId = ''
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
            dropdownParent: params.parentElementId === '' ? null : $('#' + params.parentElementId),

            minimumResultsForSearch: params.isSearchable ? 0 : Infinity,
            searchPlaceholderText: params.searchPlaceholderText,
            language: {
                noResults: function () {
                    return "<div class='select2-no-result-text'>" + params.noResultText + "</div>";
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

function gotoDate(id, refresh) {
    let done = false;
    const targetNode = document.querySelector("body");
    const config = {attributes: true, childList: true, subtree: true};
    let observer = new MutationObserver(() => {
        let CONTROL_INTERVAL = setInterval(function () {
            if (done === false) {
                hideHeader(id);
                let startSelector = $('#' + id + ' .mud-button-date')[0];
                if (startSelector.textContent === '') {
                    done = true;
                    return;
                }
                if (refresh === false)
                    return;
                $('#' + id + ' div.jinget-picker svg.mud-range-input-separator').css('visibility', 'visible');
                selectDate(startSelector);
                done = true;
            }
            clearInterval(CONTROL_INTERVAL);
            observer.disconnect();
        }, 100);
    });
    observer.observe(targetNode, config);
}

function hideHeader(id) {
    let startSelector = $('#' + id + ' .mud-button-date')[0];
    if (startSelector.textContent === '' || startSelector.textContent === undefined)
        $('#' + id + ' .mud-picker-datepicker-toolbar').css('display', 'none');
    else
        $('#' + id + ' .mud-picker-container button.mud-button-year').css('display', 'none');
}

function selectDate(startSelector) {
    let year = parseInt(startSelector.textContent.substring(0, 4));
    let month = parseInt(startSelector.textContent.substring(5, 7));
    //transit to month view
    $('button.mud-picker-slide-transition')[0].click();
    setTimeout(function () {
        //transit to year view
        $('button.mud-picker-slide-transition')[0].click();
        setTimeout(function () {
            let yearElement = $('#pickerYears .mud-picker-year *:contains(' + year + ')')[0];
            yearElement.click()
            setTimeout(function () {
                $('.mud-picker-month-container .mud-picker-month')[month - 1].click()
            }, 20);
        }, 20);
    }, 20);
}

function reStylePicker(id) {

    $('input[id=' + id + ']').parent().removeClass();
    $('input[id=' + id + ']').parent().addClass('form-control jinget-picker');
}

/*Picker END*/