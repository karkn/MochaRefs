using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mocha.Util.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GetTitle",
                url: "title",
                defaults: new { controller = "Util", action = "GetTitle" }
            );
            routes.MapRoute(
                name: "GetImage",
                url: "image",
                defaults: new { controller = "Util", action = "GetImage" }
            );
            routes.MapRoute(
                name: "GetDescription",
                url: "description",
                defaults: new { controller = "Util", action = "GetDescription" }
            );
            routes.MapRoute(
                name: "GetGooglePlusCount",
                url: "gpluscount",
                defaults: new { controller = "Util", action = "GetGooglePlusCount" }
            );
            //routes.MapRoute(
            //    name: "ProfileImage",
            //    url: "profimage/{userName}",
            //    defaults: new { controller = "User", action = "ProfileImage" }
            //);
            //routes.MapRoute(
            //    name: "SmallProfileImage",
            //    url: "smprofimage/{userName}",
            //    defaults: new { controller = "User", action = "SmallProfileImage" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
