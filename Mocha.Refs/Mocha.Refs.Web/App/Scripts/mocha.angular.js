/*!
 * Mocha angular
 *
 * 必須変数:
 *   $mc.env.requestUri
 */
var mc = mc || {};

(function ($mc) {
    "use strict";

    $mc.angular = {
        DefaultExceptionHandler: defaultExceptionHandler,

        PublishingStatusFilter: publishingStatusFilter,
        UrlEncodeFilter: urlEncodeFilter,

        SelectDirective: selectDirective,
        TagInputDirective: tagInputDirective,
        PaginationDirective: getPaginationDirective
    }

    function defaultExceptionHandler() {
        return function (exception, cause) {
            $mc.error.sendClientErrorLog(
                "angular.$exceptionHandler",
                exception.message,
                $mc.env.requestUri,
                null,
                exception.stack
            );
        };
    }

    function publishingStatusFilter() {
        return function(value) {
            return value == 0 ? "公開" : value == 1 ? "非公開" : "下書き";
        }
    }

    function urlEncodeFilter() {
        return function (value) {
            return encodeURIComponent(value);
        }
    }

    function selectDirective() {

        function createOptions(arr, valueProp, textProp) {
            var html = "";
            for (var i = 0; i < arr.length; ++i) {
                var item = arr[i];
                var value = valueProp ? item[valueProp] : item;
                var text = textProp ? item[textProp] : item;
                value = value !== null ? value.toString() : "";
                text = text !== null ? text.toString() : "";
                html += "<option value='" + $mc.util.htmlEncode(value) + "'>" + $mc.util.htmlEncode(text) + "</option>";
            }
            return html;
        }

        return {
            restrict: 'A',
            require: '?ngModel',
            scope: {
                "mcArray": "=",
                "mcValue": "=",
                "mcText": "="
            },
            link: function (scope, elem, attrs, ngModel) {
                $(function () {
                    var html = createOptions(scope.mcArray, attrs.mcValue, attrs.mcText);
                    elem.append(html);

                    elem.find("option:empty").remove(); /// ngModelがバインド時に勝手に作ったoptionを消す

                    var select = elem.selectpicker();
                    select.on("change", function () {
                        ngModel.$setViewValue(elem.val());
                        elem.trigger("modelChanged");
                    });

                    elem.val(ngModel.$viewValue).selectpicker('render');
                });

                //選択状態の監視
                //scope.$watch(attrs.ngModel, function (newValue) {
                //    elem.val(newValue).selectpicker('render');
                //    elem.trigger("modelChanged");
                //});
            }
        }
    }

    function tagInputDirective($parse) {
        function dataToArray(data) {
            if (!data) {
                return [];
            }
            return $.map(data, function (elem, i) { return elem.text; });
        }

        function arrayToData(array) {
            if (!array) {
                return [];
            }
            var ret = [];
            for (var i = 0; i < array.length; ++i) {
                var value = array[i];
                ret.push({
                    id: value,
                    text: value
                });
            }
            return ret;
        }

        return {
            restrict: "A",
            require: ["ngModel"],
            template: '<input type="text" />',
            link: function (scope, element, attr) {
                //選択候補
                var tags = [];

                var input = $(element.children("input"));
                $(function () {
                    //Select2の作成対象
                    //選択状態
                    var result = $parse(attr.ngModel);

                    //idはタグを新規作成するときにどのプロパティと比較するかというものなので
                    //textにしておかないと、選択候補と同じタグを新たに作成できてしまう
                    input.select2(
                    {
                        tags: function () { return tags; },
                        id: function (i) { return i.text; },
                        maximumInputLength: 20,
                        maximumSelectionSize: 10,
                        tokenSeparators: [",", " "],
                        placeholder: attr.mcPlaceholder,
                        formatNoMatches: function () { return "候補はありません"; },
                        createSearchChoice: function (term) {
                            var s = $.trim(term);
                            var regexp = XRegExp("[^\\p{L}#_\\-+*.:]", "gi");
                            s = XRegExp.replace(s, regexp, '');
                            // 普通の文字(\p{L})といくつかの記号はOK。それ以外は除去しておく。
                            if (s) {
                                return {
                                    id: s,
                                    text: s
                                };
                            } else {
                                return null;
                            }
                        }
                    });
                    input.on("change", function () {
                        //選択結果を渡す
                        var changed = dataToArray(input.select2('data'));
                        result.assign(scope, changed);
                        //変更結果を適用する
                        if (scope.$$phase) {
                            scope.$eval(attr.ngModel);
                        }
                        else {
                            scope.$apply(attr.ngModel);
                        }
                    });

                });
                //選択候補の監視
                scope.$watch(attr.mcTagInput, function (newValue) {
                    tags = newValue;
                }, true);

                //選択状態の監視
                scope.$watch(attr.ngModel, function (newValue) {
                    var changed = arrayToData(newValue);
                    input.select2("data", changed);
                }, true);
            }
        };

    }

    function getPaginationDirective(template) {
        return function () {
            return {
                restrict: "A",
                scope: {
                    mcCurrent: "=",
                    mcCount: "=",
                    mcFunc: "&"
                },
                templateUrl: template, //"@Url.Content("~/Content/templates/pagination.html")",
                link: function (scope, element, attrs) {
                    function update() {
                        var count = scope.mcCount;
                        var current = scope.mcCurrent;
                        var pages = [];
                        if (count < 10) {
                            pages = $mc.util.range(1, scope.mcCount);
                            pages[current - 1].className = "active";
                        } else {
                            if (current < 6) {
                                // 先頭、真ん中
                                pages = $mc.util.range(1, 7);
                                pages[current - 1].className = "active";
                                // 末尾
                                pages.push({ label: "...", index: 8 });
                                pages.push({ label: count, index: count });
                            } else if (current > count - 5) {
                                // 真ん中、末尾
                                pages = $mc.util.range(count - 6, count);
                                pages[6 - (count - current)].className = "active";
                                // 先頭
                                pages.splice(0, 0, { label: "...", index: count - 7 });
                                pages.splice(0, 0, { label: 1, index: 1 });
                            } else {
                                // 真ん中
                                pages = $mc.util.range(current - 2, current + 2);
                                pages[2].className = "active";
                                // 先頭
                                pages.splice(0, 0, { label: "...", index: current - 3 });
                                pages.splice(0, 0, { label: 1, index: 1 });
                                // 末尾
                                pages.push({ label: "...", index: current + 3 });
                                pages.push({ label: count, index: count });
                            }
                        }
                        // 前のページ
                        pages.splice(0, 0, { label: "<", index: current - 1 });
                        if (current == 1) {
                            pages[0].className = "disabled";
                        }
                        // 次のページ
                        pages.push({ label: ">", index: current + 1 });
                        if (current == count) {
                            pages[pages.length - 1].className = "disabled";
                        }

                        scope._pages = pages;
                    }

                    scope.$watch("mcCurrent", function (value) {
                        update();
                    });
                    scope.$watch("mcCount", function (value) {
                        update();
                    });

                    var handler = scope.mcFunc();
                    scope.click = function (index) {
                        if (index != scope.mcCurrent && index > 0 && index < scope.mcCount + 1) {
                            handler(index);
                        }
                    };

                    update();
                }
            };
        }
    }

})(mc);
