/// <reference path="qunit.js" />
/// <reference path="../../../Mocha.Refs.Web/App/Scripts/mocha.util.js" />

test("test format", function () {
    equal("hoge, 1", mc.util.format("{0}, {1}", "hoge", 1));
});
