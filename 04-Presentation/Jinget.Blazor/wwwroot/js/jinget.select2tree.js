(function ($) {
    $.fn.select2tree = function (options) {
        var defaults = {
            matcher: matchCustom,
            templateSelection: templateSelectionCustom,
            templateResult: templateResultCustom
        };
        var opts = $.extend(defaults, options);
        var $this = $(this);
        $(this).select2(opts).on("select2:open", function () {
            open($this);
        });
    };

    function templateResultCustom(data, container, opts) {

        if (data.element) {
            //insert span element and add 'parent' property
            var $wrapper = $("<span></span><span>" + data.text + "</span>");
            var $switchSpn = $wrapper.first();
            var $element = $(data.element);
            var $select = $element.parent();
            var $container = $(container);

            $container.attr("val", $element.val());
            $container.attr("data-parent", $element.data("parent"));

            //برای آیتم پیش فرض نیازی به فلش نیست. آیتم پیش فرض آیتمی است که فاقد ای دی میباشد
            //یعنی تگ روبرو: <option value="" data-parent="">@DefaultText</option>
            var isData = false;
            if (data.id != "---")
                isData = true;

            var hasChilds = isData && $select.find("option[data-parent='" + $element.val() + "']").length > 0;
            if (hasChilds) {
                $switchSpn.addClass("switch-tree fa-solid");

                if ($switchSpn.hasClass("fa-plus-square"))
                    $switchSpn.removeClass("fa-plus-square").addClass("fa-minus-square");
                else
                    $switchSpn.removeClass("fa-minus-square").addClass("fa-plus-square");
            }
            $switchSpn.css({
                "padding": "0 10px 0 10px",
                "cursor": "pointer"
            });

            //اگر دارای پدر بود
            if ($element.data("parent") !== '') {
                var dir = $select.attr('data-bind').split(':')[1].trim();

                var padding = getTreeLevel($select, $element.val()) * 2;
                if (!hasChilds)
                    padding++;
                $container.css("margin-" + (dir == 'ltr' ? "left" : "right"), padding + "em");
            }

            return $wrapper;
        } else {
            return data.text;
        }
    };

    function getTreeLevel($select, id) {
        var level = 0;
        while ($select.find("option[data-parent!=''][value='" + id + "']").length > 0) {
            id = $select.find("option[value='" + id + "']").data("parent");
            level++;
        }
        return level;
    }

    function moveOption($select, id) {
        if (id) {
            $select.find(".select2-results__options li[data-parent='" + id + "']").insertAfter(".select2-results__options li[val=" + id + "]");
            $select.find(".select2-results__options li[data-parent='" + id + "']").each(function () {
                moveOption($select, $(this).attr("val"));
            });
        } else {

            $(".select2-results__options li[data-parent!='']").css("display", "none");
            $(".select2-results__options li[data-parent='']").appendTo(".select2-results__options ul");
            $(".select2-results__options li[data-parent='']").each(function () {
                moveOption($select, $(this).attr("val"));
            });
        }
    }
    function switchAction($select, id, open) {

        var parent = $(".select2-results__options li[val=" + id + "] span[class]:eq(0)");
        var childs = $(".select2-results__options li[data-parent='" + id + "']");
        if (open) {
            parent.removeClass("fa-plus-square").addClass("fa-minus-square");
            childs.each(function () { $(this).attr('opened', true); });
            childs.slideDown();
        } else {
            childs.each(function () {
                var childId = $(this).attr('val');
                //آیا فرزندی دارد که باز باشد؟
                if ($(".select2-results__options li[data-parent='" + childId + "'][opened=true]").length > 0)
                    switchAction($select, childId, open);

                parent.removeClass("fa-minus-square").addClass("fa-plus-square");
                $(this).removeAttr('opened');
            });
            childs.slideUp("fast");
        }
    }

    function open($select) {
        setTimeout(function () {

            moveOption($select);
            //override mousedown for collapse/expand 
            $(".switch-tree").mousedown(function () {
                switchAction(
                    $select,
                    $(this).parent().attr("val"),
                    $(this).hasClass("fa-plus-square"));
                event.stopPropagation();
            });
            //override mouseup to nothing
            $(".switch-tree").mouseup(() => false);
        }, 0);
    }

    function matchCustom(params, data) {
        if ($.trim(params.term) === '') {
            return data;
        }
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
        return null;
    }

    function checkForChildMatch($select, $element, term) {
        //if (term == '')
        //    open($select);
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

    function templateSelectionCustom(item) {
        return item.text;
    }

    function getParentText($select, $element) {
        var text = '';
        var parentVal = $element.data('parent');
        if (parentVal == '') return text;

        var parent = $select.find('option[value=' + parentVal + ']');

        if (parent) {
            text = getParentText($select, parent);
            if (text != '') text += ' - ';
            text += parent.text();
        }
        return text;
    }
})(jQuery);