using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Helpers
{
    public static class ProfileImageHelper
    {
        public static string GetProfileImageUrl(string id, string userName)
        {
            return string.Format("/profimage/{0}", userName);
        }

        public static string GetSmallProfileImageUrl(string id, string userName)
        {
            return string.Format("/smprofimage/{0}", userName);
        }

        public static bool ExistsSmallProfileImage(string id, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }
            var server = HttpContext.Current.Server;
            var path = server.MapPath("~/App_Data/ProfileImages/" + userName.Substring(0, 2) + "/" + userName + "_small.png");
            return File.Exists(path);
        }

//        private static string UtilUrlBase()
//        {
//#if DEBUG
//            return "http://localhost:63966";
//#else
//            return "http://util.mochaware.jp";
//#endif
//        }
    }
}