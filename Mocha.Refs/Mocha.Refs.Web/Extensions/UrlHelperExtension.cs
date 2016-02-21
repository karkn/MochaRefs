using Mocha.Refs.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Extensions
{
    public static class UrlHelperExtension
    {
        public static string AbsoluteUrl(this UrlHelper helper, string contentPath)
        {
            var req = helper.RequestContext.HttpContext.Request;
            var builder = new UriBuilder(req.Url.AbsoluteUri);
            builder.Path = helper.Content(contentPath);
            return builder.Uri.ToString();
        }

        public static MvcHtmlString Twitter(this UrlHelper helper, string id, string title)
        {
            var url = helper.AbsoluteUrl("~/lists") + "/" + id;
            return new MvcHtmlString(
                "https://twitter.com/intent/tweet?url=" + url + "&text=" + helper.Encode(title) + "&lang=ja"
            );
        }
            
        public static MvcHtmlString GooglePlus(this UrlHelper helper, string id)
        {
            var url = helper.AbsoluteUrl("~/lists") + "/" + id;
            return new MvcHtmlString(
                "https://plus.google.com/share?url=" + url + "&hl=ja"
            );
        }

        public static MvcHtmlString Facebook(this UrlHelper helper, string id)
        {
            var url = helper.AbsoluteUrl("~/lists") + "/" + id;
            return new MvcHtmlString(
                "http://www.facebook.com/share.php?u=" + url
            );
        }

        public static MvcHtmlString ProfileImage(this UrlHelper helper, string id, string userName)
        {
            return new MvcHtmlString(ProfileImageHelper.GetProfileImageUrl(id, userName));
        }

        public static MvcHtmlString SmallProfileImage(this UrlHelper helper, string id, string userName)
        {
            return new MvcHtmlString(ProfileImageHelper.GetSmallProfileImageUrl(id, userName));
        }

        public static bool ExistsSmallProfileImage(this UrlHelper helper, string id, string userName)
        {
            return ProfileImageHelper.ExistsSmallProfileImage(id, userName);
        }
    }
}
