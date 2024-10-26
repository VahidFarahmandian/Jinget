(function ($) {
    $.fn.jinget_select2tree = function (options) {
        var defaults = {
            matcher: matchCustom,
            templateResult: formTemplateResult,
            templateSelection: formTemplateSelection
        };
        var opts = $.extend(defaults, options);
        $(this).select2(opts)
            // When the component is opened, we need to render the items as a tree
            .off("select2:open").on("select2:open", options, onTreeOpened)
            //If the user clicks on the icon, nothing should be selected and the tree should only be expanded or collapsed
            .off("select2:selecting").on("select2:selecting", onTreeItemSelecting)
            //To prevent the component from closing when the expand or collapse icon is clicked
            .off("select2:closing").on("select2:closing", onTreeItemClosing);
        //$(document).on('keyup', 'span.select2-container--open input.select2-search__field', function (e) {
        //    if (e.which == 40 || e.which == 38) {

        //        var currentHighlightedIndex = $(this).closest('.select2-dropdown').find('li.select2-results__option--highlighted').index();
        //        var isCurrentHighlightedCollapsed = $(this).closest('.select2-dropdown').find('li.select2-results__option--highlighted span.fa-plus-square').length > 0;
        //        var currentHighlightedDataLevel = $(this).closest('.select2-dropdown').find('li.select2-results__option--highlighted div[data-parent]').attr('data-level');
        //        var currentHighlightedDataParent = $(this).closest('.select2-dropdown').find('li.select2-results__option--highlighted div[data-parent]').attr('data-parent');
        //        var totalCountSameLevelItems = $(this).closest('.select2-dropdown').find("li div[data-parent='" + currentHighlightedDataParent + "']").length;

        //        if (currentHighlightedIndex < totalCountSameLevelItems) {
        //            $(this).closest('.select2-dropdown').find("li div[data-parent='" + currentHighlightedDataParent + "']").parent().eq(currentHighlightedIndex).removeClass('select2-results__option--highlighted')
        //            $(this).closest('.select2-dropdown').find("li div[data-parent='" + currentHighlightedDataParent + "']").parent().eq(currentHighlightedIndex + 1).addClass('select2-results__option--highlighted')
        //        }
        //    }
        //})
    };

    function isIconClicked(e) {
        if (e == undefined)
            return false;
        return $(e.target).hasClass('fa-plus-square') || $(e.target).hasClass('fa-minus-square');
    }

    //When an item with a child is selected and the list is closed, it is not possible to collapse the item next time, 
    //and the following code has been written to solve this problem.
    function onTreeItemClosing(e) {
        if (isIconClicked(e.params.args.originalEvent)) {
            onTreeItemSelecting(e);
        }
    }
    function onTreeOpened(e) {
        $('input.select2-search__field').prop('placeholder', e.data.searchPlaceholderText);

        var $select = $(this);
        setTimeout(function () {
            moveOption($select);
        }, 0);
    }
    function onTreeItemSelecting(e) {
        var toBeOpen = $(e.params.args.originalEvent.target).hasClass('fa-plus-square');
        var $select = $(this);
        if (isIconClicked(e.params.args.originalEvent)) {
            switchAction($select, e.params.args.data.id, toBeOpen);
            e.preventDefault();
        }
        else {
            switchAction($select, e.params.args.data.id, toBeOpen);
        }
    }

    //When an item is selected, the return value from this method is displayed as the selected value in the component
    function formTemplateSelection(data) {
        return normalizeString(data.text);
    }
    //How to render the items in the tree are set in this method
    function formTemplateResult(data) {
        if (data.loading) {
            return normalizeString(data.text);
        }
        var $element = $(data.element);
        var $select = $element.parent();
        var $markup = $("<div><span></span><span></span></div>");
        var container = $markup[0];
        var $container = $(container);
        $container.attr('id', "container-" + data.id);
        //item value
        $container.attr('val', $element.val());
        //item's parent value
        $container.attr('data-parent', $element.data("parent"));
        //items nesting level
        $container.attr('data-level', $element.data("level"));

        //Child item calculation is done only for items other than the default item
        var hasChild = data.id != '---' && $select.find("option[data-parent='" + $element.val() + "']").length > 0;

        var $icon = $(container.firstChild);
        $icon.attr('id', "icon-" + data.id);
        $icon.attr('data-hasChild', hasChild);
        $icon.addClass("fa-solid");

        var isSearching = $(".select2-search__field").val().length > 0;

        if (isSearching) {
            //Using this attribute later in the search mode, it is possible to expand and collapse the tree
            $container.attr('status', 'searching');
        }
        //If it was searching and the search contains results, then display the tree in expand mode
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
        //Don't put any icon for the default item
        else if (data.id != '---' && !hasChild) {
            if (!$icon.hasClass("fa-minus"))
                $icon.addClass("fa-minus");
        }
        var $text = $(container.lastChild);
        $text.attr('id', "text-" + data.id);
        $text.html(normalizeString(data.text));

        var dir = $(data.element.parentNode)[0].dataset.bind.substr(5, 3);

        //No nesting is needed for the root node
        var padding = data.element.dataset.level <= 1 ? 0 : (data.element.dataset.level - 1) * 20;
        if (dir == 'ltr') {
            $container.css({
                "margin": "5px 0 0 " + padding + "px"
            });
            $icon.css({
                "margin-right": "5px"
            });
        }
        else {
            $container.css({
                "margin": "5px " + padding + "px 0 0"
            });
            $icon.css({
                "margin-left": "5px"
            });
        }
        $container.css({
            "cursor": "pointer"
        });
        return $markup;
    }

    //Used to render the tree items, it also makes the subcategory items not visible when the tree is opened
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

    //Collapse and expand nodes in the tree to display the subset
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
                //Does it have any children that are collapsed?
                if ($(".select2-results__options li[opened=true] div[data-parent='" + childId + "']").length > 0)
                    switchAction($select, childId, open);

                //Is it in search mode and we want to close the node?
                if ($(".select2-results__options div[status='searching'][data-parent='" + childId + "']").length > 0)
                    switchAction($select, childId, open);

                parent.removeClass("fa-minus-square").addClass("fa-plus-square");
                $(this).removeAttr('opened');
                $(this).removeAttr('status');
            });
            childs.slideUp("fast");
        }
    }

    //Search in tree
    function matchCustom(params, data) {
        //If nothing was searched. It means that only the tree is opened
        if (typeof params.term === 'undefined') {
            return data;
        }
        // If the search term was empty
        else if ($.trim(params.term) === '') {
            $('#' + $(data.element).parent()[0].id).select2('close').select2('open')
            return data;
        }
        // Do not display the item if there is no 'text' property
        if (typeof data.text === 'undefined') {
            return null;
        }

        var term = params.term.toLowerCase().replace('ي', 'ی').replace('ك', 'ک');
        var $element = $(data.element);
        var $select = $element.parent();
        var childMatched = checkForChildMatch($select, $element, term);

        //The default item should not participate in the search. The default item is an item that does not have an ID
        //Sample default item: <option value="" data-parent="">@DefaultText</option>
        if (data.id != '---' && (childMatched || data.text.toLowerCase().indexOf(term) >= 0)) {
            $("#" + data._resultId).css("display", "unset");
            return data;
        }
        // Return `null` if the term should not be displayed
        return null;
    }
    function normalizeString(input) {
        return input.replace('ي', 'ی').replace('ك', 'ک');
    }

    //To search the children of the current node
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