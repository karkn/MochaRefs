using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Extensions
{
    public static class HttpRequestBaseExtension
    {
        public static bool IsGoogleCrawl(this HttpRequestBase request)
        {
            return request.QueryString.AllKeys.Contains("_escaped_fragment_");
        }

        public static bool IsHome(this HttpRequestBase request, UrlHelper urlHelper)
        {
            return request.Url.AbsolutePath == urlHelper.Content("~/");
        }
    }
}