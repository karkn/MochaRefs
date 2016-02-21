using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Extensions
{
    public static class HtmlHelperExtension
    {
        private static readonly Regex MobileUserAgentRegex = new Regex(
            "Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini",
            RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public static MvcHtmlString IsDebug(this HtmlHelper helper)
        {
#if DEBUG
            return new MvcHtmlString("true");
#else
            return new MvcHtmlString("false");
#endif
        }

        public static MvcHtmlString IsMobileDevice(this HtmlHelper helper, HttpRequestBase request)
        {
            var agent = request.UserAgent;
            var ret = agent != null && MobileUserAgentRegex.IsMatch(agent) ? "true" : "false";
            return new MvcHtmlString(ret);
        }

        public static MvcHtmlString JsMinify(this HtmlHelper helper, Func<object, object> markup)
        {
            string notMinifiedJs =
                (markup.DynamicInvoke(helper.ViewContext) ?? "").ToString();

            var minifier = new Minifier();
            var minifiedJs = minifier.MinifyJavaScript(
                notMinifiedJs,
                new CodeSettings
            {
                LocalRenaming = Microsoft.Ajax.Utilities.LocalRenaming.KeepAll,
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            });
            return new MvcHtmlString(minifiedJs);
        }
    }
}