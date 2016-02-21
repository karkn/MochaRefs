/*!
 * Mocha Init Page
 * ページ表示時に必ず呼ばれる処理。
 */
var mc = mc || {};

(function () {
    "use strict";

    $(function () {
        // ajax
        $.ajaxSetup({
            cache: false,
            timeout: 0,
            error: mc.error.handleAjaxError
        });


        // affix
        //$(".affix").affix({
        //    offset: {
        //        bottom: function () {
        //            this.bottom = $("footer").outerHeight(true) + 40; // body.padding-bottom(20) + hr.margin-bottom(20) = 40
        //            return this.bottom;
        //        }
        //    }
        //});

        /*
        * Clamped-width. 
        * Usage:
        *  <div data-clampedwidth=".myParent">This long content will force clamped width</div>
        *
        * http://stackoverflow.com/questions/12536354/bootstrap-affix-plugin-with-fluid-layout
        */
        //$('[data-clampedwidth]').each(function () {
        //    var elem = $(this);
        //    var parentPanel = elem.data('clampedwidth');
        //    var resizeFn = function () {
        //        var sideBarNavWidth =
        //            $(parentPanel).width() -
        //            parseInt(elem.css('paddingLeft'), 10) -
        //            parseInt(elem.css('paddingRight'), 10) -
        //            parseInt(elem.css('marginLeft'), 10) -
        //            parseInt(elem.css('marginRight'), 10) -
        //            parseInt(elem.css('borderLeftWidth'), 10) -
        //            parseInt(elem.css('borderRightWidth'), 10);
        //        elem.css('width', sideBarNavWidth);
        //    };

        //    resizeFn();
        //    $(window).resize(resizeFn);
        //});

    });
})();
