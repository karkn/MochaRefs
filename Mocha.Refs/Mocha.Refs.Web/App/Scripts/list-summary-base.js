$(function () {
    $(document).on({
        mouseenter: function () {
            var item = $(this);
            item.find(".list-actions").show();
        },
        mouseleave: function () {
            var item = $(this);
            item.find(".list-actions").hide();
        }
    }, ".list-summary");
});

function getToggleListDetailFunc($scope) {
    function getKey(list) {
        return "Id_" + list.Id.toString();
    }

    return function (event, list) {
        $target = $(event.target);
        if (event.target.tagName.toLowerCase() === "a" || $target.parents("a").length > 0 || $target.parents(".list-detail").length > 0) {
            return;
        }

        if ($scope.DetailPartView[list.Id]) {
            $scope.DetailPartView[list.Id] = null;

        } else {
            if (!$scope.Refs[getKey(list)]) {
                mc.util.get({
                    url: "/List/GetRefs",
                    data: { id: list.Id }
                }).done(function (result) {
                    var data = JSON.parse(result);
                    $scope.$apply(function () {
                        $scope.Refs[getKey(list)] = data;
                    });
                    console.log(data);
                });
            }

            $scope.DetailPartView[list.Id] = "/List/DetailPart/" + list.Id.toString();
        }
    };
}
