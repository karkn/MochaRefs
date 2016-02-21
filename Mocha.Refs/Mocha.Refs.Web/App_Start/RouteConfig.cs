using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mocha.Refs.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Sitemap",
                url: "sitemap.xml",
                defaults: new { controller = "Home", action = "Sitemap", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AddByBookmarklet",
                url: "add",
                defaults: new { controller = "List", action = "AddByBookmarklet" }
            );

            routes.MapRoute(
                name: "OpenByBookmarklet",
                url: "open",
                defaults: new { controller = "List", action = "OpenByBookmarklet" }
            );

            routes.MapRoute(
                name: "FindByBookmarklet",
                url: "find",
                defaults: new { controller = "List", action = "FindByBookmarklet" }
            );

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "List", action = "Search" }
            );

            routes.MapRoute(
                name: "ListDetail",
                url: "lists/{id}",
                defaults: new { controller = "List", action = "Detail" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "ListIndex",
                url: "lists",
                defaults: new { controller = "List", action = "Index" }
            );

            routes.MapRoute(
                name: "UserDetail",
                url: "users/{userName}",
                defaults: new { controller = "User", action = "Detail" },
                constraints: new { userName = @"[a-zA-Z0-9]+" }
            );

            routes.MapRoute(
                name: "UserIndex",
                url: "users",
                defaults: new { controller = "User", action = "Index" }
            );

            routes.MapRoute(
                name: "TagDetail",
                url: "tags/{tag}",
                defaults: new { controller = "Tag", action = "Detail" }
            );

            routes.MapRoute(
                name: "TagIndex",
                url: "tags",
                defaults: new { controller = "Tag", action = "Index" }
            );

            routes.MapRoute(
                name: "ProfileImage",
                url: "profimage/{userName}",
                defaults: new { controller = "User", action = "ProfileImage" }
            );
            routes.MapRoute(
                name: "SmallProfileImage",
                url: "smprofimage/{userName}",
                defaults: new { controller = "User", action = "SmallProfileImage" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
