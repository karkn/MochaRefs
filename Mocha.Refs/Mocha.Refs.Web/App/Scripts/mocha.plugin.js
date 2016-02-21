/*!
 * Mocha plugin
 */
var mc = mc || {};

(function (plugin) {
    "use strict";

    plugin.loadIsPluginEnabled = function (id) {
        return $.cookie(mc.env.linkPluginEnabledCookieKeyPrefix + id) === "1";
    };

    plugin.saveIsPluginEnabled = function (id, value) {
        var storedValue = value ? "1" : "0";
        $.cookie(mc.env.linkPluginEnabledCookieKeyPrefix + id, storedValue, { path: "/", expires: 365 });
    };

})(mc.plugin = mc.plugin || {});
