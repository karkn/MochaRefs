/*!
 * Mocha jQuery
 * jQueryプラグイン。
 */
; (function ($) {
    "use strict";

    $.fn.scrollIntoView = function () {
        var elem = this;
        var windowHeight = $(window).height();
        var navbarHeight = $(".navbar-fixed-top").outerHeight();
        var body = $("html, body");
        var bodyTop = body.scrollTop() + navbarHeight;
        var bodyBottom = body.scrollTop() + windowHeight;
        var elemTop = elem.offset().top;
        var elemBottom = elemTop + elem.outerHeight();
        if (elemTop < bodyTop) {
            body.animate({
                scrollTop: elemTop - navbarHeight
            }, 300);
        } else if (elemBottom > bodyBottom) {
            body.animate({
                scrollTop: elemBottom - windowHeight
            }, 300);
        }
    };

    $.fn.scrollTo = function (complete) {
        var $elem = this;
        var $body = $("html, body");
        var navbarHeight = $(".navbar-fixed-top").innerHeight();
        var completedCount = 0;
        $body.animate({
            scrollTop: $elem.offset().top - navbarHeight
        }, 300, "linear", function () {
            ++completedCount;
            if (completedCount === 2) {
                // html, bodyで2回
                complete();
            }
        });
    };

    $.fn.blink = function (color) {
        if (!color) {
            color = "#fff9c0";
        }
        
        var elem = this;
        var bc = elem.css("background-color");

        elem.animate(
            { backgroundColor: color },
            100,
            function () {
                elem.animate(
                    { backgroundColor: bc },
                    1000,
                    function () { elem.css("background-color", ""); });
            }
        );
    };

    // 表示されているかどうかを返します。
    $.fn.isVisible = function () {
        return $.expr.filters.visible(this[0]);
    };

    $.fn.focusFirstInput = function () {
        var self = $(this);
        var input = $("input[type=text]:first", self);
        if (input.length === 0) {
            input = $("textarea:first", self);
        }
        input.focus();
    };

})(jQuery);
