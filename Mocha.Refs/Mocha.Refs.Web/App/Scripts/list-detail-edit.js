var mc = mc || {};

getModule().
    directive("mcSelect", mc.angular.SelectDirective).
    directive("mcTagInput", ["$parse", mc.angular.TagInputDirective]);

var _editing = false;
var _lastMenuOpenItem = null;
var _lastMouseEnterItem = null;

function setEditing(value) {
    "use strict";

    _editing = value;
    if (_editing) {
        $(".sidebar button").attr("disabled", "disabled");

    } else {
        $(".sidebar button").removeAttr("disabled");
    }
}

function PageController($scope) {
    "use strict";

    $scope.publishingStatusOpts = [
        { Value: "0", Text: "公開" },
        { Value: "1", Text: "非公開" },
        { Value: "2", Text: "下書き" }
    ];
    $scope.Model = mc.page.model;
    $scope._input = {};
    $scope._submitted = false;

    PageControllerCommon($scope);

    function beginEditLink(elem) {
        setEditing(true);
        elem.find(".item-action").hide();
        elem.find(".link-view").hide();
        elem.removeClass("item-hovered");
        elem.addClass("item-highlight");

        $("#link-edit").insertBefore($(".item-link hr", elem)).fadeIn().focusFirstInput();
        elem.scrollIntoView();
    }

    function endEditLink(elem) {
        elem.find(".link-view").fadeIn();
        elem.removeClass("item-highlight");
        $("#link-edit").hide().appendTo($("#formContainer"));
        setEditing(false);
    }

    function beginEditHeading(elem) {
        setEditing(true);
        elem.find(".item-action").hide();
        elem.find(".heading-view").hide();
        elem.removeClass("item-hovered");
        elem.addClass("item-highlight");

        $("#heading-edit").insertBefore($(".item-heading hr", elem)).fadeIn();
        elem.scrollIntoView();
        elem.focusFirstInput();
    }

    function endEditHeading(elem) {
        elem.find(".heading-view").fadeIn();
        elem.removeClass("item-highlight");

        $("#heading-edit").hide().appendTo($("#formContainer"));
        setEditing(false);
    }

    function beginEditText(elem) {
        setEditing(true);
        elem.find(".item-action").hide();
        elem.find(".text-view").hide();
        elem.removeClass("item-hovered");
        elem.addClass("item-highlight");
        $("#text-edit").insertBefore($(".item-text hr", elem)).fadeIn();
        elem.scrollIntoView();
        elem.focusFirstInput();
    }

    function endEditText(elem) {
        elem.find(".text-view").fadeIn();
        elem.removeClass("item-highlight");
        $("#text-edit").hide().appendTo($("#formContainer"));
        setEditing(false);
    }

    function beginEditListHeader(elem) {
        setEditing(true);
        elem.find(".list-header-action").hide();
        elem.find(".list-header-view").hide();
        elem.find(".list-header-edit").fadeIn();
        elem.removeClass("item-hovered");
        elem.scrollIntoView();
        elem.focusFirstInput();
    }

    function endEditListHeader(elem) {
        elem.find(".list-header-edit").hide();
        elem.find(".list-header-view").fadeIn();
        setEditing(false);
    }

    // ----------------------------------------
    // list-header
    // ----------------------------------------
    $scope.showListHeaderEdit = function (event) {
        $scope._submitted = false;
        $scope._input = {
            Title: $scope.Model.RefList.Title,
            Comment: $scope.Model.RefList.Comment,
            TagUses: $scope.Model.RefList.TagUses
        };
        var elem = $(event.target).closest(".list-header");
        beginEditListHeader(elem);
    };

    $scope.saveListHeaderEdit = function (event) {
        $scope._submitted = true;
        if ($scope.form.$invalid) {
            return;
        }

        $scope.Model.RefList.Title = $scope._input.Title;
        $scope.Model.RefList.Comment = $scope._input.Comment;
        $scope.Model.RefList.TagUses = $scope._input.TagUses;
        var data = {
            Id: $scope.Model.RefList.Id,
            RowVersion: $scope.Model.RefList.RowVersion,
            PublishingStatus: $scope.Model.RefList.PublishingStatus,
            Title: $scope._input.Title,
            TagUses: $scope._input.TagUses,
            Comment: $scope._input.Comment
        };
        $scope._input = {};

        var elem = $(event.target).closest(".list-header");
        endEditListHeader(elem);
        elem.blink();

        mc.util.post({
            url: mc.page.actions.editListHeader,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RowVersion;
            });
        });
    };

    $scope.cancelListHeaderEdit = function (event) {
        $scope._input = {};

        var elem = $(event.target).closest(".list-header");
        endEditListHeader(elem);
    };

    // ----------------------------------------
    // link
    // ----------------------------------------
    $scope.showEditLink = function (event, item) {
        $scope._submitted = false;
        $scope._input = {
            Uri: item.Uri,
            Title: item.Title,
            Comment: item.Comment
        };

        $("#link-edit").data("item", item);
        var elem = $(event.target).closest(".item");
        beginEditLink(elem);
    };

    $scope.saveEditLink = function (event, valid) {
        $scope._submitted = true;
        if (!valid) {
            return;
        }

        var item = $("#link-edit").data("item");
        item.Uri = $scope._input.Uri;
        item.Title = $scope._input.Title;
        item.Comment = $scope._input.Comment;

        var data = {
            info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            refe: item
        };

        $scope._input = {};
        $("#link-edit").removeData("item");

        var elem = $(event.target).closest(".item");
        endEditLink(elem);
        elem.blink();
        $scope.refreshPluginContent(item);

        mc.util.post({
            url: mc.page.actions.editRef,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RowVersion;
            });
        });

    };

    $scope.cancelEditLink = function (event) {
        $scope._input = {};
        $("#link-edit").removeData("item");

        var elem = $(event.target).closest(".item");
        endEditLink(elem);
    };

    $scope.moveLink = function (event, item) {
        mc.util.get({
            url: mc.page.actions.getRefLists
        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.RefLists = data;
                $scope.MoveTargetRefList = $scope.RefLists[0];
            });
        });

        var modal = $("#select-ref-list-modal");
        var ok = $(".btn-primary", modal);
        ok.off("click");
        ok.on("click", function (e) {
            if ($scope.Model.RefList.Id === $scope.MoveTargetRefList.Id) {
                return;
            }

            var index = item == null ? -1 : $scope.Model.Refs.indexOf(item);
            if (index < 0) {
                return;
            }

            $scope.Model.Refs.splice(index, 1);

            var data = {
                sourceRefListIdentity: {
                    Id: $scope.Model.RefList.Id,
                    RowVersion: $scope.Model.RefList.RowVersion
                },
                targetRefListIdentity: {
                    Id: $scope.MoveTargetRefList.Id,
                    RowVersion: $scope.MoveTargetRefList.RowVersion
                },
                sourceRefIndex: index
            };
            mc.util.post({
                url: mc.page.actions.moveRefTo,
                data: JSON.stringify(data)

            }).done(function (result) {
                $scope.$apply(function () {
                    var data = JSON.parse(result);
                    $scope.Model.RefList.RowVersion = data.RowVersion;
                });
            });
        });
        modal.modal("show");
    };

    $scope.onUrlBlur = function () {
        var url = $scope._input.Uri;
        if (url)
        {
            if (!mc.util.startsWith(url, "http://") &&
                !mc.util.startsWith(url, "https://")) {
                $scope._input.Uri = "http://" + url;
            }
        }
    }

    $scope.getTitle = function (valid) {
        if (!valid) {
            return;
        }

        var data = { uri: $scope._input.Uri };
        mc.util.get({
            url: mc.page.actions.getTitle,
            data: data,
            dataType: 'jsonp',
            cache: true,
            error: null // 例外時に集約ハンドラの処理を実行しない
        }).done(function (data) {
            if (data != null) {
                $scope.$apply(function () {
                    $scope._input.Title = data.trim();
                });
            }
        });
    };

    // ----------------------------------------
    // heading
    // ----------------------------------------
    $scope.showEditHeading = function (event, item) {
        $scope._submitted = false;
        $scope._input = {
            Title: item.Title,
        };

        $("#heading-edit").data("item", item);

        var elem = $(event.target).closest(".item");
        beginEditHeading(elem);
    };

    $scope.saveEditHeading = function (event, valid) {
        $scope._submitted = true;
        if (!valid) {
            return;
        }

        var item = $("#heading-edit").data("item");
        item.Title = $scope._input.Title;

        var data = {
            info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            refe: item
        };

        $scope._input = {};
        $("#heading-edit").removeData("item");

        var elem = $(event.target).closest(".item");
        endEditHeading(elem);
        elem.blink();

        mc.util.post({
            url: mc.page.actions.editRef,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RowVersion;
            });
        });
    };

    $scope.cancelEditHeading = function (event) {
        var self = $(event.target);
        var elem = self.closest(".item");

        $("#heading-edit").removeData("item");
        $scope._input = {};
        endEditHeading(elem);
    };

    // ----------------------------------------
    // text
    // ----------------------------------------
    $scope.showEditText = function (event, item) {
        $scope._submitted = false;
        $scope._input = {
            Comment: item.Comment,
        };

        $("#text-edit").data("item", item);

        var elem = $(event.target).closest(".item");
        beginEditText(elem);
    };

    $scope.saveEditText = function (event, valid) {
        $scope._submitted = true;
        if (!valid) {
            return;
        }

        var item = $("#text-edit").data("item");
        item.Comment = $scope._input.Comment;

        var data = {
            info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            refe: item
        };

        $scope._input = {};
        $("#text-edit").removeData("item");

        var elem = $(event.target).closest(".item");
        endEditText(elem);
        elem.blink();

        mc.util.post({
            url: mc.page.actions.editRef,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RowVersion;
            });
        });
    };

    $scope.cancelEditText = function (event, item) {
        var self = $(event.target);
        var elem = self.closest(".item");

        $scope._input = {};
        $("#text-edit").removeData("item");

        endEditText(elem);
    };

    // ----------------------------------------
    // add item
    // ----------------------------------------
    $scope.showAddItem = function (event, item, target) {
        var elem = $(event.target).closest(".item");
        var itemAdd = $("#item-add");

        setEditing(true);
        elem.find(".item-action").hide();
        elem.removeClass("item-hovered");

        itemAdd.data("item", item);
        $scope._submitted = false;
        $scope._input = {
            Uri: "",
            Title: "",
            Comment: ""
        };

        itemAdd.find(".link-add").hide();
        itemAdd.find(".heading-add").hide();
        itemAdd.find(".text-add").hide();

        itemAdd.insertAfter($(".item-add-anchor", elem)).fadeIn();
        if (target === "link") {
            itemAdd.find(".link-add").show().focusFirstInput();
        } else if (target == "heading") {
            itemAdd.find(".heading-add").show().focusFirstInput();
        } else if (target == "text") {
            itemAdd.find(".text-add").show().focusFirstInput();
        }
        itemAdd.scrollIntoView();
    };

    $scope.showAddItemTop = function (event, target) {
        var itemAdd = $("#item-add-top");

        setEditing(true);

        $scope._submitted = false;
        $scope._input = {
            Uri: "",
            Title: "",
            Comment: ""
        };

        $(".list-header-action").hide();

        itemAdd.find(".link-add").hide();
        itemAdd.find(".heading-add").hide();
        itemAdd.find(".text-add").hide();

        itemAdd.fadeIn();
        if (target === "link") {
            itemAdd.find(".link-add").show().focusFirstInput();
        } else if (target == "heading") {
            itemAdd.find(".heading-add").show().focusFirstInput();
        } else if (target == "text") {
            itemAdd.find(".text-add").show().focusFirstInput();
        }
        itemAdd.scrollIntoView();
    };

    $scope.showAddLink = function (event, item) {
        $scope._submitted = false;
        $scope._input = {
            Uri: "",
            Title: "",
            Comment: ""
        };
        $("#item-add").data("item", item);

        setEditing(true);
        var itemAdd = $(event.target).closest(".item-add");
        itemAdd.find(".heading-add").hide();
        itemAdd.find(".text-add").hide();
        itemAdd.find(".link-add").fadeIn().focusFirstInput();
        itemAdd.scrollIntoView();
    };

    $scope.showAddHeading = function (event, item) {
        $scope._submitted = false;
        $scope._input = {
            Title: ""
        };
        $("#item-add").data("item", item);

        setEditing(true);
        var itemAdd = $(event.target).closest(".item-add");
        itemAdd.find(".link-add").hide();
        itemAdd.find(".text-add").hide();
        itemAdd.find(".heading-add").fadeIn().focusFirstInput();
        itemAdd.scrollIntoView();
    };

    $scope.showAddText = function (event, item) {
        $scope._submitted = false;
        $scope._input = {
            Comment: ""
        };
        $("#item-add").data("item", item);

        setEditing(true);
        var itemAdd = $(event.target).closest(".item-add");
        itemAdd.find(".link-add").hide();
        itemAdd.find(".heading-add").hide();
        itemAdd.find(".text-add").fadeIn().focusFirstInput();
        itemAdd.scrollIntoView();
    };

    $scope.hideAddItem = function (event, item) {
        $scope._input = {};
        setEditing(false);

        $("#item-add").removeData("item");
        $("#item-add").hide().appendTo($("#formContainer"));

        var isTop = $(event.target).closest("#item-add-top").length !== 0;
        if (isTop) {
            $("#item-add-top").hide();
        }
    };

    $scope.saveAddLink = function (event, valid) {
        $scope._submitted = true;
        if (!valid) {
            return;
        }

        var item = $("#item-add").data("item"); // #item-add-topならnull

        var newItem = {
            Kind: 0,
            Uri: $scope._input.Uri,
            Title: $scope._input.Title,
            Comment: $scope._input.Comment,
        };
        var index = item == null ? -1 : $scope.Model.Refs.indexOf(item);
        $scope.Model.Refs.splice(index + 1, 0, newItem);

        var data = {
            info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            refe: newItem,
            refIndex: index + 1
        };

        $scope._input = {};
        $("#item-add").removeData("item");
        $("#item-add").hide().appendTo($("#formContainer"));
        setEditing(false);

        var isTop = $(event.target).closest("#item-add-top").length !== 0;
        if (isTop) {
            $("#item-add-top").hide();
        }

        mc.util.post({
            url: mc.page.actions.addRef,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RefListIdentity.RowVersion;
                newItem.Id = data.RefId;
            });
            var elem = $(".list-items .item").eq(index + 1);
            elem.blink();
            elem.scrollIntoView();
            $scope.refreshPluginContent(newItem);

        });
    };

    $scope.saveAddHeading = function (event, valid) {
        $scope._submitted = true;
        if (!valid) {
            return;
        }

        var item = $("#item-add").data("item"); // #item-add-topならnull

        var newItem = {
            Kind: 10,
            Title: $scope._input.Title
        };
        var index = item == null ? -1 : $scope.Model.Refs.indexOf(item);
        $scope.Model.Refs.splice(index + 1, 0, newItem);

        var data = {
            info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            refe: newItem,
            refIndex: index + 1
        };

        $scope._input = {};
        $("#item-add").removeData("item");
        $("#item-add").hide().appendTo($("#formContainer"));
        setEditing(false);

        var isTop = $(event.target).closest("#item-add-top").length !== 0;
        if (isTop) {
            $("#item-add-top").hide();
        }

        mc.util.post({
            url: mc.page.actions.addRef,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RefListIdentity.RowVersion;
                newItem.Id = data.RefId;
            });
            var elem = $(".list-items .item").eq(index + 1);
            elem.blink();
            elem.scrollIntoView();
        });
    };

    $scope.saveAddText = function (event, valid) {
        $scope._submitted = true;
        if (!valid) {
            return;
        }

        var item = $("#item-add").data("item"); // #item-add-topならnull

        var newItem = {
            Kind: 11,
            Comment: $scope._input.Comment
        };
        var index = item == null ? -1 : $scope.Model.Refs.indexOf(item);
        $scope.Model.Refs.splice(index + 1, 0, newItem);

        var data = {
            info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            refe: newItem,
            refIndex: index + 1
        };

        $scope._input = {};
        $("#item-add").removeData("item");
        $("#item-add").hide().appendTo($("#formContainer"));
        setEditing(false);

        var isTop = $(event.target).closest("#item-add-top").length !== 0;
        if (isTop) {
            $("#item-add-top").hide();
        }

        mc.util.post({
            url: mc.page.actions.addRef,
            data: JSON.stringify(data)

        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RefListIdentity.RowVersion;
                newItem.Id = data.RefId;
            });

            var elem = $(".list-items .item").eq(index + 1);
            elem.blink();
            elem.scrollIntoView();
        });
    };

    // ----------------------------------------
    // remove item
    // ----------------------------------------
    $scope.showRemoveItemModal = function (event, item) {
        var modal = $("#remove-item-modal");
        var ok = $(".btn-primary", modal);
        ok.off("click");
        ok.on("click", function (e) {
            var index = item == null ? -1 : $scope.Model.Refs.indexOf(item);
            if (index < 0) {
                return;
            }

            $scope.Model.Refs.splice(index, 1);

            var data = {
                info: {
                    Id: $scope.Model.RefList.Id,
                    RowVersion: $scope.Model.RefList.RowVersion
                },
                refIndex: index
            };

            mc.util.post({
                url: mc.page.actions.removeRef,
                data: JSON.stringify(data)

            }).done(function (result) {
                $scope.$apply(function () {
                    var data = JSON.parse(result);
                    $scope.Model.RefList.RowVersion = data.RowVersion;
                });

            });

        });
        modal.modal("show");
    };

    // ----------------------------------------
    // open link
    // ----------------------------------------
    $scope.openAllLinks = function (event) {
        var refs = $scope.Model.Refs;
        for (var i = 0; i < refs.length; ++i) {
            var refe = refs[i];
            if (refe.Kind === 0) {
                var win = window.open(refe.Uri, "_blank");
            }
        }
    };

    // ----------------------------------------
    // alert
    // ----------------------------------------
    $scope.isBookmarkletInfoAlertHidden = function () {
        return $.cookie(mc.env.bookmarkletInfoAlertHiddenCookieKey) !== "1";
    };

    $scope.hideBookmarkletInfoAlert = function () {
        $.cookie(mc.env.bookmarkletInfoAlertHiddenCookieKey, "1", { path: "/", expires: 365 });
    };

    $scope.isAddByDnDInfoAlertHidden = function () {
        return $.cookie(mc.env.addByDnDInfoAlertHiddenCookieKey) !== "1";
    };

    $scope.hideAddByDnDInfoAlert = function () {
        $.cookie(mc.env.addByDnDInfoAlertHiddenCookieKey, "1", { path: "/", expires: 365 });
    };

    $scope.isAddFromPocketInfoAlertHidden = function () {
        return $.cookie(mc.env.addFromPocketInfoAlertHiddenCookieKey) !== "1";
    };

    $scope.hideAddFromPocketInfoAlert = function () {
        $.cookie(mc.env.addFromPocketInfoAlertHiddenCookieKey, "1", { path: "/", expires: 365 });
    };

    // ----------------------------------------
    // import
    // ----------------------------------------
    $scope.updatePocketItems= function () {
        mc.util.get({
            url: mc.page.actions.getPocketItems,
            error: null // 例外時に集約ハンドラの処理を実行しない
        }).done(function (result) {
            var data = JSON.parse(result);
            if (data) {
                $scope.$apply(function () {
                    $scope.PocketItems = data;
                });
            } else {
                $scope.$apply(function () {
                    $scope.PocketItems = "unconfigured";
                });
            }
        });
    };

    $scope.showImportSidebarContent = function () {
        $("#default-sidebar-content").hide();
        $("#import-sidebar-content").fadeIn();
        $scope.updatePocketItems();
    };

    $scope.hideImportSidebarContent = function () {
        $("#import-sidebar-content").hide();
        $("#default-sidebar-content").fadeIn();
    };

    $scope.showPocketAuthModal = function () {
        $('#pocket-auth-modal').on('show.bs.modal', function () {
            $("iframe").attr("src", mc.page.actions.authPocket);
        });
        $('#pocket-auth-modal').on('hide.bs.modal', function () {
            $scope.updatePocketItems();
            $("iframe").attr("src", "");
        });
        $('#pocket-auth-modal').modal({ show: true })
    };

    // ----------------------------------------
    // move
    // ----------------------------------------
    $scope.moveItemTo = function (item, newIndex) {
        var index = item == null ? -1 : $scope.Model.Refs.indexOf(item);
        if (index < 0) {
            return;
        }

        $scope.Model.Refs.splice(index, 1);
        $scope.Model.Refs.splice(newIndex, 0, item);

        var data = {
            Info: {
                Id: $scope.Model.RefList.Id,
                RowVersion: $scope.Model.RefList.RowVersion
            },
            OldIndex: index,
            NewIndex: newIndex
        };
        mc.util.post({
            url: mc.page.actions.moveRef,
            data: JSON.stringify(data)
        }).done(function (result) {
            $scope.$apply(function () {
                var data = JSON.parse(result);
                $scope.Model.RefList.RowVersion = data.RowVersion;
            });
            $scope.gotoItem(item);
        });
    }

    $scope.moveItemToTop = function (item) {
        $scope.moveItemTo(item, 0);
    };

    $scope.moveItemToBottom = function (item) {
        $scope.moveItemTo(item, $scope.Model.Refs.length - 1);
    };

    $scope.moveItemToFiveAhead = function (item) {
        var index = $scope.Model.Refs.indexOf(item);
        index = Math.max(index - 5, 0);
        $scope.moveItemTo(item, index);
    };

    $scope.moveItemToFiveBehind = function (item) {
        var index = $scope.Model.Refs.indexOf(item);
        index = Math.min(index + 5, $scope.Model.Refs.length - 1);
        $scope.moveItemTo(item, index);
    };

    // ----------------------------------------
    // sortable
    // ----------------------------------------
    $scope.sortableOptions = {
        tolerance: "pointer",
        handle: ".sortable-handle",
        axis: "y",
        //placeholder: "ui-sortable-placeholder",
        start: function (event, ui) {
            //ui.placeholder.height(ui.item.height());
            //ui.placeholder.width(ui.item.width());
            ui.item.data('start', ui.item.index());
        },
        stop: function (event, ui) {
            var start = ui.item.data("start"),
                end = ui.item.index();
            ui.item.data("start", null);

            var data = {
                Info: {
                    Id: $scope.Model.RefList.Id,
                    RowVersion: $scope.Model.RefList.RowVersion
                },
                OldIndex: start,
                NewIndex: end
            };
            mc.util.post({
                url: mc.page.actions.moveRef,
                data: JSON.stringify(data)

            }).done(function (result) {
                $scope.$apply(function () {
                    var data = JSON.parse(result);
                    $scope.Model.RefList.RowVersion = data.RowVersion;
                });

            });
        }
    };
}

function insertLink(index, newItem) {
    var scope = getPageScope();
    var data = {
        info: {
            Id: scope.Model.RefList.Id,
            RowVersion: scope.Model.RefList.RowVersion
        },
        refe: newItem,
        refIndex: index
    };
    mc.util.post({
        url: mc.page.actions.addRef,
        data: JSON.stringify(data)
    }).done(function (result) {
        scope.$apply(function () {
            var data = JSON.parse(result);
            scope.Model.RefList.RowVersion = data.RefListIdentity.RowVersion;
            newItem.Id = data.RefId;
        });
        scope.refreshPluginContent(newItem);
    });
}

function handleDrop(e, index) {
    e.preventDefault();
    e.stopPropagation();

    var dt = e.originalEvent.dataTransfer;

    var text = null;
    var types = dt.types;
    if ($.inArray("text/x-moz-url", types) !== -1) {
        text = dt.getData("text/x-moz-url");
    } else {
        text = dt.getData("text");
    }

    if (text != null) {
        var lines = text.split(/\r\n|\r|\n/);
        if (lines.length < 1) {
            return false;
        }
        var uri = lines[0];
        var title = lines.length >= 2 ? lines[1] : null;

        if (!mc.util.startsWith(uri, "http://") && !mc.util.startsWith(uri, "https://")) {
            return false;
        }

        if (index > -1) {
            var newItem = {
                Kind: 0,
                Uri: uri,
                Title: title === null ? uri : title,
                Comment: ""
            };

            var scope = getPageScope();
            scope.$apply(function () {
                scope.Model.Refs.splice(index, 0, newItem);
            });

            if (title != null) {
                insertLink(index, newItem);
            } else {
                // titleがない場合は取得を試みる。だめならuriを使う。
                mc.util.get({
                    url: mc.page.actions.getTitle,
                    data: { uri: uri },
                    dataType: "jsonp",
                    cache: true,
                    error: null // 例外時に集約ハンドラの処理を実行しない
                }).done(function (result) {
                    if (result != null && result.trim() != "") {
                        scope.$apply(function () {
                            newItem.Title = result.trim();
                        });
                    }
                    insertLink(index, newItem);
                });
            }
        }
    }
    return false;
}

$(function () {
    if (mc.env.isMobile) {
        // set up list header click/tap
        $(document).on({
            click: function (e) {
                if (e.target.tagName.toLowerCase() === "a") {
                    return true;
                }
                if (_editing) {
                    return false;
                }
                $(".item-action").hide();
                $(".item").removeClass("item-hovered");
                $(".list-header-action").hide();

                $(".list-header-action").show();
                return false;
            }
        }, ".list-header");

        // set up item click/tap
        $(document).on({
            click: function (e) {
                if (e.target.tagName.toLowerCase() === "a") {
                    return true;
                }
                if (_editing) {
                    return false;
                }
                $(".item-action").hide();
                $(".item").removeClass("item-hovered");
                $(".list-header-action").hide();

                var item = $(this);
                item.find(".item-action").show();
                item.addClass("item-hovered");
                return false;
            }
        }, ".item");

    } else {
        // set up list header hover, dnd
        $(document).on({
            mouseenter: function () {
                if (_editing) {
                    return false;
                }
                $(".list-header-action").show();
                return false;
            },
            mouseleave: function () {
                if (_editing) {
                    return false;
                }
                $(".list-header-action").hide();
                return false;
            }
        }, ".list-header");

        // set up item hover, dnd
        $(document).on({
            dragover: function (e) {
                if (_editing || mc.util.isInAjaxProcess()) {
                    return true;
                }
                e.preventDefault();
                e.stopPropagation();
                return false;
            },
            drop: function (e) {
                if (_editing || mc.util.isInAjaxProcess()) {
                    return true;
                }
                var count = $(".list-items .item").length;
                return handleDrop(e, count);
            }
        }, "body");

        $(document).on({
            // hover
            mouseenter: function (e) {
                var item = $(this);

                // メニューに対してもmouseenterが起こってしまうのを防ぐ
                if (!$(e.target).hasClass("dropdown-menu")) {
                    _lastMouseEnterItem = item;
                }

                if (_editing || _lastMenuOpenItem !== null) {
                    return false;
                }
                item.find(".item-action").show();
                item.addClass("item-hovered");
                return false;
            },
            mouseleave: function () {
                var item = $(this);
                _lastMouseEnterItem = null;
                if (_editing || _lastMenuOpenItem !== null) {
                    return false;
                }
                item.find(".item-action").hide();
                item.removeClass("item-hovered");
                return false;
            },
            // dnd
            dragenter: function (e) {
                if (_editing || mc.util.isInAjaxProcess()) {
                    return true;
                }
                e.preventDefault();
                e.stopPropagation();
                return false;
            },
            dragleave: function (e) {
                if (_editing || mc.util.isInAjaxProcess()) {
                    return true;
                }
                var item = $(this);
                item.removeClass("item-hovered");
                e.preventDefault();
                e.stopPropagation();
                return false;
            },
            dragover: function (e) {
                if (_editing || mc.util.isInAjaxProcess()) {
                    return true;
                }
                var item = $(this);
                item.addClass("item-hovered");
                e.preventDefault();
                e.stopPropagation();
                return false;
            },
            drop: function (e) {
                if (_editing || mc.util.isInAjaxProcess()) {
                    return true;
                }
                var elem = $(this);
                var index = $(".list-items .item").index(elem);
                return handleDrop(e, index);
            }
        }, ".item");

        $(".list-items").on("show.bs.dropdown", function (e) {
            _lastMouseEnterItem = null;
            _lastMenuOpenItem = $(e.target).parents(".item");
        });
        $(".list-items").on("hidden.bs.dropdown", function (e) {
            if (_lastMenuOpenItem !== null) {
                _lastMenuOpenItem.find(".item-action").hide();
                _lastMenuOpenItem.removeClass("item-hovered");
                _lastMenuOpenItem = null;
            }
            if (_lastMouseEnterItem !== null) {
                _lastMouseEnterItem.find(".item-action").show();
                _lastMouseEnterItem.addClass("item-hovered");
            }
        });

        $(document).on({
            dragstart: function (e) {
                var uri = e.target.href;
                var title = e.target.text;

                var dt = e.originalEvent.dataTransfer;
                dt.effectAllowed = 'copy';
                dt.setData('text', uri + "\r" + title);
            }

        }, ".imported-item");
    }

    // 公開状態の変更
    $("#publishing-status").on("modelChanged", function () {
        var scope = getPageScope();
        var data = {
            Id: scope.Model.RefList.Id,
            RowVersion: scope.Model.RefList.RowVersion,
            PublishingStatus: scope.Model.RefList.PublishingStatus,
            Title: scope.Model.RefList.Title,
            Comment: scope.Model.RefList.Comment,
            TagUses: scope.Model.RefList.TagUses
        };
        mc.util.post({
            url: mc.page.actions.editListHeader,
            data: JSON.stringify(data)

        }).done(function (result) {
            scope.$apply(function () {
                var data = JSON.parse(result);
                scope.Model.RefList.RowVersion = data.RowVersion;
            });

        });
    });

});  // $(function () {
