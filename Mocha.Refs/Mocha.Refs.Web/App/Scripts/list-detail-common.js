var mc = mc || {};

function getKeys(obj) {
    var keys = [];
    for (var key in obj) {
        if (obj.hasOwnProperty(key)) {
            keys.push(key);
        }
    }
    return keys;
}

function getContent(results) {
    if (!results || results.length === 0) {
        return "";
    }
    var keys = getKeys(results);
    keys = keys.sort();
    var htmls = [];
    for (var i = 0; i < keys.length; ++i) {
        htmls.push(results[keys[i]]);
    }
    return htmls.join(", ");
}

function PageControllerCommon($scope) {
    $scope.addAfterTitle = function (id, item, html) {
        item.plugin = item.plugin || {};
        item.plugin.afterTitle = item.plugin.afterTitle || {};
        item.plugin.afterTitle.results = item.plugin.afterTitle.results || {};
        item.plugin.afterTitle.results[id] = html;
        item.plugin.afterTitle.content = getContent(item.plugin.afterTitle.results);
    };
    $scope.removeAfterTitle = function (id, item) {
        if (!item.plugin || !item.plugin.afterTitle || !item.plugin.afterTitle.results) {
            return;
        }
        delete item.plugin.afterTitle.results[id];
        item.plugin.afterTitle.content = getContent(item.plugin.afterTitle.results);
    };

    $scope.plugins = {
        google: {
            enabled: mc.plugin.loadIsPluginEnabled("google"),
            enable: function (item) {
                var data = {
                    uri: item.Uri
                };
                mc.util.get({
                    dataType: 'jsonp',
                    url: mc.env.utilUrlBase + "/gpluscount",
                    data: data,
                    noAlert: true,
                    cache: true,
                    error: null // 例外時に集約ハンドラの処理を実行しない
                }).done(function (data) {
                    if (data != null) {
                        var html = mc.util.format("<span class='fa fa-google-plus'></span> {0}", data || 0);
                        $scope.$apply(function () {
                            $scope.addAfterTitle("0_google", item, html);
                        });
                    }
                });
            },
            disable: function (item) {
                $scope.removeAfterTitle("0_google", item);
            }
        },
        twitter: {
            enabled: mc.plugin.loadIsPluginEnabled("twitter"),
            enable: function (item) {
                var data = {
                    url: item.Uri
                };
                mc.util.get({
                    dataType: 'jsonp',
                    url: "http://urls.api.twitter.com/1/urls/count.json",
                    data: data,
                    noAlert: true,
                    cache: true,
                    error: null // 例外時に集約ハンドラの処理を実行しない
                }).done(function (data) {
                    if (data != null) {
                        var href = "https://twitter.com/search?q=" + item.Uri;
                        var html = mc.util.format("<a href='{0}'><span class='fa fa-twitter'></span> {1}</a>", href, data.count || 0);
                        $scope.$apply(function () {
                            $scope.addAfterTitle("1_twitter", item, html);
                        });
                    }
                });
            },
            disable: function (item) {
                $scope.removeAfterTitle("1_twitter", item);
            }
        },
        facebook: {
            enabled: mc.plugin.loadIsPluginEnabled("facebook"),
            enable: function (item) {
                var data = {
                    id: item.Uri
                };
                mc.util.get({
                    dataType: 'jsonp',
                    url: "https://graph.facebook.com/",
                    data: data,
                    noAlert: true,
                    cache: true,
                    error: null // 例外時に集約ハンドラの処理を実行しない
                }).done(function (data) {
                    if (data != null) {
                        var html = mc.util.format("<span class='fa fa-facebook'></span> {0}", data.shares || 0);
                        $scope.$apply(function () {
                            $scope.addAfterTitle("2_facebook", item, html);
                        });
                    }
                });
            },
            disable: function (item) {
                $scope.removeAfterTitle("2_facebook", item);
            }
        },
        //hatenaBookmark: {
        //    enabled: mc.plugin.loadIsPluginEnabled("hatenaBookmark"),
        //    enable: function (item) {
        //        var data = {
        //            url: item.Uri
        //        };
        //        mc.util.get({
        //            dataType: 'jsonp',
        //            url: "http://api.b.st-hatena.com/entry.count",
        //            data: data,
        //            noAlert: true,
        //            cache: true,
        //            error: null // 例外時に集約ハンドラの処理を実行しない
        //        }).done(function (data) {
        //            if (data != null) {
        //                var href = "http://b.hatena.ne.jp/entry/" + item.Uri;
        //                var html = mc.util.format("<a href='{0}'>{1} users</a>", href, JSON.parse(data));
        //                $scope.$apply(function () {
        //                    $scope.addAfterTitle("hatenaBookmark", item, html);
        //                });
        //            }
        //        });
        //    },
        //    disable: function (item) {
        //        $scope.removeAfterTitle("hatenaBookmark", item);
        //    }
        //},
        description: {
            enabled: mc.plugin.loadIsPluginEnabled("description"),
            enable: function (item) {
                var data = { uri: item.Uri };
                mc.util.get({
                    dataType: 'jsonp',
                    url: mc.page.actions.getDescription,
                    data: data,
                    noAlert: true,
                    cache: true,
                    error: null // 例外時に集約ハンドラの処理を実行しない
                }).done(function (data) {
                    var desc = data === null ? "説明がありません。" : data.trim();
                    $scope.$apply(function () {
                        item.plugin = item.plugin || {};
                        $.extend(item.plugin, {
                            append: mc.util.format("<blockquote>{0}</blockquote>", desc)
                        });
                    });
                });
            },
            disable: function (item) {
                if (item.plugin && item.plugin.append) {
                    item.plugin.append = null;
                }
            }
        },
        image: {
            enabled: mc.plugin.loadIsPluginEnabled("image"),
            enable: function (item) {
                item.plugin = item.plugin || {};
                $.extend(item.plugin, {
                    itemLeft: mc.util.format("<img class='favicon' src='{0}' width='16' height='16' />", "http://favicon.hatena.ne.jp/?url=" + item.Uri)
                });

                var data = { uri: item.Uri };
                mc.util.get({
                    dataType: 'jsonp',
                    url: mc.page.actions.getImage,
                    data: data,
                    noAlert: true,
                    cache: true,
                    error: null // 例外時に集約ハンドラの処理を実行しない
                }).done(function (data) {
                    $scope.$apply(function () {
                        if (data != null) {
                            item.plugin = item.plugin || {};
                            $.extend(item.plugin, {
                                itemLeft: mc.util.format("<img src='{0}' />", data.trim())
                            });
                        }
                    });
                });
            },
            disable: function (item) {
                if (item.plugin && item.plugin.itemLeft) {
                    item.plugin.itemLeft = null;
                }
            }
        }
    };

    // ----------------------------------------
    // plugin
    // ----------------------------------------
    $scope.togglePlugin = function (id) {
        var disabled = !$scope.plugins[id].enabled;
        if (disabled) {
            $scope.enablePlugin(id);
        } else {
            $scope.disablePlugin(id);
        }
    };

    $scope.getPlugin = function (id) {
        return $scope.plugins[id];
    };

    $scope.isPluginEnabled = function (id) {
        return $scope.plugins[id].enabled;
    };

    $scope.enablePlugin = function (id) {
        var plugin = $scope.getPlugin(id);
        $.each($scope.Model.Refs, function (index, elem) {
            if (elem.Kind == 0) {
                plugin.enable(elem);
            }
        });
        mc.plugin.saveIsPluginEnabled(id, true);
    };

    $scope.disablePlugin = function (id) {
        var plugin = $scope.getPlugin(id);
        $.each($scope.Model.Refs, function (index, elem) {
            if (elem.Kind == 0) {
                plugin.disable(elem);
            }
        });
        mc.plugin.saveIsPluginEnabled(id, false);
    };

    $scope.refreshPluginContents = function () {
        $.each($scope.plugins, function (index, plugin) {
            if (plugin.enabled) {
                $.each($scope.Model.Refs, function (index, elem) {
                    if (elem.Kind == 0) {
                        plugin.enable(elem);
                    }
                });
            }
        });
    };

    $scope.refreshPluginContent = function (item) {
        $.each($scope.plugins, function (index, plugin) {
            if (plugin.enabled) {
                if (item.Kind == 0) {
                    plugin.enable(item);
                }
            }
        });
    };

    $scope.refreshPluginContents();

    // ----------------------------------------
    // outline
    // ----------------------------------------
    $scope.existsHeading = function () {
        return $scope.getHeadings().length > 0;
    };

    $scope.getHeadings = function () {
        return $.grep($scope.Model.Refs, function (elem) {
            return elem.Kind === 10;
        });
    };

    $scope.gotoItem = function (item) {
        var selector = "[data-mc-ref-id=" + item.Id + "]";
        $(selector).scrollTo(function () {
            $(selector).blink();
        });
    };

    // ----------------------------------------
    // favorite
    // ----------------------------------------
    $scope.toggleFavorite = function () {
        var url = $scope.Model.IsFavored ?
            mc.page.actions.removeFavorite :
            mc.page.actions.addFavorite;

        var data = {
            refListId: $scope.Model.RefList.Id
        };
        mc.util.post({
            url: url,
            data: JSON.stringify(data)
        }).done(function () {
            $scope.$apply(function () {
                $scope.Model.IsFavored = !$scope.Model.IsFavored;
            });
        });
    };

    // ----------------------------------------
    // find list
    // ----------------------------------------
    $scope.findList = function (url, title) {
        window.location.href = mc.env.basePath + "find?title=" +
           encodeURIComponent(title) + '&url=' + encodeURIComponent(url);
    };
}

//$(function () {
//});
