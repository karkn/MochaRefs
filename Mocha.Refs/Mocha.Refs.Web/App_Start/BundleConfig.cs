using System.Web;
using System.Web.Optimization;

namespace Mocha.Refs.Web
{
    public class BundleConfig
    {
        // バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkId=301862 を参照してください
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備が
            // できたら、http://modernizr.com にあるビルド ツールを使用して、必要なテストのみを選択します。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            // site css
            bundles.Add(new StyleBundle("~/Content/sitecss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap-select.css"));
            /// パスとバンドル名を一致させないと画像が読み込まれない。
            /// ファイルパスから拡張子を取り除いたものにするのがよい。
            bundles.Add(new StyleBundle("~/Content/css/select2").Include(
                      "~/Content/css/select2.css",
                      "~/Content/select2-bootstrap/select2-bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/mochacss").Include(
                      "~/App/Styles/mocha.css"));

            // page css
            bundles.Add(new StyleBundle("~/Content/home-index").Include(
                      "~/App/Styles/home-index.css"));

            bundles.Add(new StyleBundle("~/Content/list-index").Include(
                      "~/App/Styles/list-index.css"));
            bundles.Add(new StyleBundle("~/Content/list-detail").Include(
                      "~/App/Styles/list-detail.css"));
            bundles.Add(new StyleBundle("~/Content/list-manage").Include(
                      "~/App/Styles/list-manage.css"));
            bundles.Add(new StyleBundle("~/Content/list-open-by-bookmarklet").Include(
                      "~/App/Styles/list-open-by-bookmarklet.css"));
            bundles.Add(new StyleBundle("~/Content/list-search").Include(
                      "~/App/Styles/list-search.css"));

            bundles.Add(new StyleBundle("~/Content/user-index").Include(
                      "~/App/Styles/user-index.css"));
            bundles.Add(new StyleBundle("~/Content/user-detail").Include(
                      "~/App/Styles/user-detail.css"));
            bundles.Add(new StyleBundle("~/Content/user-favorite-ref-list").Include(
                      "~/App/Styles/user-favorite-ref-list.css"));
            bundles.Add(new StyleBundle("~/Content/user-favorite-tag").Include(
                      "~/App/Styles/user-favorite-tag.css"));
            bundles.Add(new StyleBundle("~/Content/user-summary").Include(
                      "~/App/Styles/user-summary.css"));

            bundles.Add(new StyleBundle("~/Content/tag-index").Include(
                      "~/App/Styles/tag-index.css"));

            // javascript
            bundles.Add(new ScriptBundle("~/bundles/shim").Include(
                        "~/Scripts/es5-shim.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        //"~/Scripts/jquery-ui-{version}.js"));
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.ui.touch-punch.js"));

            bundles.Add(new ScriptBundle("~/bundles/xregexp").Include(
                        "~/Scripts/xregexp/xregexp.js",
                        "~/Scripts/xregexp/unicode-base.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-sanitize.js",
                        "~/Scripts/i18n/angular-locale_ja-jp.js"));
            bundles.Add(new ScriptBundle("~/bundles/ui-sortable").Include(
                        "~/Scripts/ui-sortable/sortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/select").Include(
                        "~/Scripts/bootstrap-select.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                        "~/Scripts/select2.js",
                        "~/Scripts/Select2-locales/select2_locale_ja.js"));

            bundles.Add(new ScriptBundle("~/bundles/mocha-head").Include(
                        "~/App/Scripts/mocha.error.js",
                        "~/App/Scripts/mocha.util.js",
                        "~/App/Scripts/mocha.angular.js"));

            bundles.Add(new ScriptBundle("~/bundles/mocha").Include(
                        "~/App/Scripts/mocha.jquery.js",
                        "~/App/Scripts/mocha.plugin.js",
                        "~/App/Scripts/mocha.initpage.js"));
            bundles.Add(new ScriptBundle("~/bundles/list-summary-base").Include(
                        "~/App/Scripts/list-summary-base.js"));

            bundles.Add(new ScriptBundle("~/bundles/list-detail-common").Include(
                        "~/App/Scripts/list-detail-common.js"));
            bundles.Add(new ScriptBundle("~/bundles/list-detail-view").Include(
                        "~/App/Scripts/list-detail-view.js"));
            bundles.Add(new ScriptBundle("~/bundles/list-detail-edit").Include(
                        "~/App/Scripts/list-detail-edit.js"));
        }
    }
}
