var mc = mc || {};

function PageController($scope) {
    "use strict";

    $scope.Model = mc.page.model;

    PageControllerCommon($scope);
}

$(function () {
    // set up item hover, dnd
    $(document).on({
        mouseenter: function () {
            var item = $(this);
            item.find(".item-action").show();
            item.addClass("item-hovered");
        },
        mouseleave: function () {
            var item = $(this);
            item.find(".item-action").hide();
            item.removeClass("item-hovered");
        }
    }, ".item");
});
