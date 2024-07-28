(function ($) {
    $.fn.select2tree = function (options) {
        var defaults = {
            matcher: matchCustom,
            templateResult: formTemplateResult,
            templateSelection: formTemplateSelection
        };
        var opts = $.extend(defaults, options);
        //زمانیکه کامپوننت باز میشود نیاز است تا ایتم ها را بصورت درختی رندر کنیم
        $(this).select2(opts).on("select2:open", function (e) {
            var $select = $(this);
            setTimeout(function () {
                moveOption($select);
            }, 0);
        });
        //در صورتیکه کاربر روی ایکون کلیک کرد نباید چیزی انتخاب شود و صرفا باید درخت اکسپند یا کلپس شود
        $(this).select2(opts).on("select2:selecting", function (e, selector, data, handler) {
            var iconClicked = $(e.params.args.originalEvent.target).hasClass('fa-plus-square') || $(e.params.args.originalEvent.target).hasClass('fa-minus-square');
            var toBeOpen = $(e.params.args.originalEvent.target).hasClass('fa-plus-square');
            var $select = $(this);
            if (iconClicked) {
                switchAction($select, e.params.args.data.id, toBeOpen);
                e.preventDefault();
            }
            else {
                switchAction($select, e.params.args.data.id, toBeOpen);
            }
        });
    };
    //زمانیکه ایتمی انتخاب شد مقدار برگشتی از این متد بعنوان مقدار انتخابی در کامپوننت نمایش داده میشود
    function formTemplateSelection(data) {
        return data.text;
    }
    //نحوه رندر شدن ایتم ها در درخت در این متد تنظیم میشوند
    function formTemplateResult(data) {
        if (data.loading) {
            return data.text;
        }
        var $element = $(data.element);
        var $select = $element.parent();
        var $markup = $("<div><span></span><span></span></div>");
        var container = $markup[0];
        var $container = $(container);
        $container.attr('id', "container-" + data.id);
        //مقدار گره
        $container.attr('val', $element.val());
        //شناسه گره پدر
        $container.attr('data-parent', $element.data("parent"));
        //عمق تو رفتگی
        $container.attr('data-level', $element.data("level"));

        var hasChild = $select.find("option[data-parent='" + $element.val() + "']").length > 0;

        var $icon = $(container.firstChild);
        $icon.attr('id', "icon-" + data.id);
        $icon.attr('data-hasChild', hasChild);
        $icon.addClass("fa-solid");

        var isSearching = $(".select2-search__field").val().length > 0;
        if (isSearching) {
            //با استفاده از این صفت بعدا در حالت جستجو امکان اکپند و کلپس پیاده میشود
            $container.attr('status', 'searching');
        }
        //اگر در حال جستجو بود و جستجو حاوی نتیجه بود آنگاه درخت را در حالت اکسپند نمایش بده
        if (isSearching && hasChild) {
            if ($icon.hasClass("fa-plus-square"))
                $icon.removeClass("fa-plus-square")
            $icon.addClass("fa-minus-square");
            $container.attr('opened', true);
        }
        else if (hasChild) {
            if ($icon.hasClass("fa-plus-square"))
                $icon.removeClass("fa-plus-square").addClass("fa-minus-square");
            else
                $icon.removeClass("fa-minus-square").addClass("fa-plus-square");
        }
        var $text = $(container.lastChild);
        $text.attr('id', "text-" + data.id);
        $text.html(data.text);

        var dir = $(data.element.parentNode)[0].dataset.bind.substr(5, 3);
        var padding = data.element.dataset.level * 2;
        if (dir == 'ltr') {
            $container.css({
                "margin-left": padding + "em"
            });
            $icon.css({
                "margin-left": "1em"
            });
        }
        else {
            $container.css({
                "margin-right": padding + "em"
            });
            $icon.css({
                "margin-left": "1em"
            });
        }
        return $markup;
    }

    //برای رندر آیتم های درخت استفاده میشود
    //همچنین باعث میشود در هنگام باز شدن درخت ایتم های زیرمجموعه دیده نشوند
    function moveOption($select, id) {
        if (id) {
            $select.find(".select2-results__options div[data-parent='" + id + "']").insertAfter(".select2-results__options div[val=" + id + "]");
            $select.find(".select2-results__options div[data-parent='" + id + "']").each(function () {
                moveOption($select, $(this).attr("val"));
            });
        } else {
            $(".select2-results__options div[data-parent!='']").parent().css("display", "none");
            $(".select2-results__options div[data-parent='']").appendTo(".select2-results__options ul");
            $(".select2-results__options div[data-parent='']").each(function () {
                moveOption($select, $(this).attr("val"));
            });
        }
    }

    //باز و بسته کردن گره ها در درخت برای نمایش زیرمجموعه
    function switchAction($select, id, open) {

        var parent = $(".select2-results__options div[val=" + id + "] span[class]:eq(0)");

        //we need to get access to li element which is the parent of dov element.
        //this is why we call .parent() on selectors
        var childs = $(".select2-results__options div[data-parent='" + id + "']").parent();
        if (open) {
            parent.removeClass("fa-plus-square").addClass("fa-minus-square");
            childs.each(function () {
                $(this).attr('opened', true);
            });
            childs.slideDown("fast");
        } else {
            childs.each(function () {
                var childId = $(this).find('div').attr('val');
                //آیا فرزندی دارد که باز باشد؟
                if ($(".select2-results__options li[opened=true] div[data-parent='" + childId + "']").length > 0)
                    switchAction($select, childId, open);

                //آیا در حالت جستجو بوده و میخواهیم گره را ببندیم؟
                if ($(".select2-results__options div[status='searching'][data-parent='" + childId + "']").length > 0)
                    switchAction($select, childId, open);

                parent.removeClass("fa-minus-square").addClass("fa-plus-square");
                $(this).removeAttr('opened');
                $(this).removeAttr('status');
            });
            childs.slideUp("fast");
        }
    }

    //جستجو در درخت
    function matchCustom(params, data) {
        //اگر چیزی جستجو نشده بود. یعنی فقط درخت باز شده است
        if (typeof params.term === 'undefined') {
            return data;
        }
        // اگر عبارت جستجو خالی بود
        else if ($.trim(params.term) === '') {
            $('#' + $(data.element).parent()[0].id).select2('close').select2('open')
            return data;
        }
        // Do not display the item if there is no 'text' property
        if (typeof data.text === 'undefined') {
            return null;
        }
        var term = params.term.toLowerCase();
        var $element = $(data.element);
        var $select = $element.parent();
        var childMatched = checkForChildMatch($select, $element, term);

        //آیتم پیش فرض نباید در جستجو شرکت کند. آیتم پیش فرض آیتمی است که فاقد ای دی میباشد
        //یعنی تگ روبرو: <option value="" data-parent="">@DefaultText</option>
        var isData = false;
        if (data.id != "---")
            isData = true;

        if (isData && (childMatched || data.text.toLowerCase().indexOf(term) >= 0)) {
            $("#" + data._resultId).css("display", "unset");
            return data;
        }
        // Return `null` if the term should not be displayed
        return null;
    }

    //برای جستجو در فرزندان گره جاری
    function checkForChildMatch($select, $element, term) {
        var matched = false;
        var val = $element.val();
        var childs;
        if (!val) {
            childs = $select.find('option[data-parent]');
        }
        else {
            childs = $select.find('option[data-parent=' + $element.val() + ']');
        }
        var childMatchFilter = jQuery.makeArray(childs).some(s => s.text.toLowerCase().indexOf(term) >= 0)
        if (childMatchFilter) return true;

        childs.each(function () {
            var innerChild = checkForChildMatch($select, $(this), term);
            if (innerChild) matched = true;
        });

        return matched;
    }
})(jQuery);