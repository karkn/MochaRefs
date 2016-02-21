using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web
{
    public static class WebConsts
    {
        public const int RefListsPageSize = 20;

        public const int MaxRefListTitleLength = 100;
        public const int MaxRefListCommentLength = 400;

        public const int MaxRefLinkUrlLength = 300;
        public const int MaxRefLinkTitleLength = 200;
        public const int MaxRefLinkCommentLength = 400;
        public const int MaxRefHeadingLength = 100;
        public const int MaxRefTextLength = 400;

        public const int MaxTagNameLength = 20;

        public const int MaxTitleSearchLength = 200;
        public const int MaxTextSearchLength = 200;

        public const string UnsetTitle = "(タイトルなし)";

        public const string RefsUserKey = "RefsUser";

        public const string AuthCookieName = "Auth";
        public const string RequestVerificationTokenCookieName = "Token";
        public const string ViewdPageCookieName = "VP";
        public const string ViewdPageCookieIdKeyFormat = "Id{0}";

    }
}