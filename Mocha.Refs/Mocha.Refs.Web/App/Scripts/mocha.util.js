/*!
 * Mocha Utility
 */
var mc = mc || {};

(function ($mc) {
    "use strict";

    var _inAjaxProcess = false;

    var _showInProcessAlertCount = 0;

    var _charEntityMap = {
        "&": "&amp;",
        "'": "&#39;",
        '"': "&quot;",
        "<": "&lt;",
        ">": "&gt;"
    };

    function replaceCharEntity(chr) {
        return _charEntityMap[chr];
    }

    function addOnScroll() {
        var timer = null;
        $(window).on("scroll.mc.util", function () {
            adjustInProcessAlertPosition();
        });
    }

    function removeOnScroll() {
        $(window).off("scroll.mc.util");
    }

    function adjustInProcessAlertPosition() {
        var alert = $("#in-process-alert");
        var body = $("body, html");
        alert.css("top", (body.scrollTop() + 50));
        alert.css("left", (body.width() - alert.width()) / 2);
    }

    function showInProcessAlert() {
        if (_showInProcessAlertCount === 0) {
            addOnScroll();
            adjustInProcessAlertPosition();
            $("#in-process-alert").fadeIn(200);
        }
        ++_showInProcessAlertCount;
    }

    function hideInProcessAlert() {
        --_showInProcessAlertCount;
        if (_showInProcessAlertCount === 0) {
            $("#in-process-alert").fadeOut(200, function () {
                removeOnScroll();
            });
        }
    }

    $mc.util = {
        // fmt内の{0}、{1}、、、を二番目以降の引数に置換した文字列を返します。
        format: function (fmt) {
            for (var i = 1; i < arguments.length; ++i) {
                var reg = new RegExp("\\{" + (i - 1) + "\\}", "g");
                fmt = fmt.replace(reg, arguments[i]);
            }
            return fmt;
        },

        startsWith: function (s, prefix) {
            return s.indexOf(prefix) == 0;
        },

        sleep: function (msec) {
            var d1 = new Date().getTime();
            var d2 = new Date().getTime();
            while (d2 < d1 + msec) {
                d2 = new Date().getTime();
            }
            return;
        },

        range: function (start, end) {
            var ret = [];
            for (var i = start; i < end + 1; ++i) {
                ret.push({ index: i, label: i });
            }
            return ret;
        },

        htmlEncode: function (value) {
            return value.replace(/[&"'<>]/g, replaceCharEntity);
            //return String(str)
            //        .replace(/&/g, '&amp;')
            //        .replace(/"/g, '&quot;')
            //        .replace(/'/g, '&#39;')
            //        .replace(/</g, '&lt;')
            //        .replace(/>/g, '&gt;');
        },

        htmlDecode: function (value) {
            return String(value)
                .replace(/&quot;/g, '"')
                .replace(/&#39;/g, "'")
                .replace(/&lt;/g, '<')
                .replace(/&gt;/g, '>')
                .replace(/&amp;/g, '&');
        },

        cutString: function (s, max) {
            if (s !== null && s.length > max) {
                return s.substr(0, max);
            }
            return s;
        },

        getShortUrl: function (url, maxLength) {
            if (!url) {
                return url;
            }
            var ret = url.replace("http://", "");
            if (ret.length > maxLength) {
                ret = ret.substring(0, maxLength) + "...";
            }
            return ret;
        },

        isInAjaxProcess: function() {
            return _inAjaxProcess;
        },

        post: function (args) {
            if (_inAjaxProcess) {
                return;
            }
            _inAjaxProcess = true;
            var buttons = $("button,input[type=button],a.btn", $(".body-content"));
            setTimeout(function () {
                if (_inAjaxProcess) {
                    buttons.attr("disabled", "disabled");
                }
            }, 500);

            var defaultArgs = {
                type: "POST",
                contentType: "application/json"
            };
            var data = $.extend(true, args, defaultArgs);
            showInProcessAlert();
            return $.ajax(data).always(function () {
                _inAjaxProcess = false;
                buttons.removeAttr("disabled");
                hideInProcessAlert();
            });
        },

        get: function (args) {
            var noAlert = args && args.hasOwnProperty("noAlert") && args.noAlert;

            var defaultArgs = {
                type: "GET"
            };
            var data = $.extend(true, args, defaultArgs);

            if (noAlert) {
                return $.ajax(data);
            } else {
                showInProcessAlert();
                return $.ajax(data).always(function () {
                    hideInProcessAlert();
                });
            }
        },

        findBootstrapEnvironment: function () {
            var envs = ['xs', 'sm', 'md', 'lg'];

            var $el = $('<div>');
            $el.appendTo($('body'));

            for (var i = envs.length - 1; i >= 0; i--) {
                var env = envs[i];

                $el.addClass('hidden-' + env);
                if ($el.is(':hidden')) {
                    $el.remove();
                    return env;
                }
            }
        }
    };
})(mc);
