using Mocha.Refs.Core.Entities;
using Mocha.Refs.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Mocha.Refs.Web.Utils
{
    public static class SitemapUtil
    {
        public static string GetXmlString(IEnumerable<RefList> refLists, UrlHelper urlHelper)
        {
            var xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            var root = new XElement(xmlns + "urlset");

            var urls = new List<string>();

            urls.Add(urlHelper.AbsoluteUrl("~/"));

            var baseUrl = urlHelper.AbsoluteUrl("~/lists/");
            var refListUrls = refLists.Select(l => baseUrl + l.Id);
            urls.AddRange(refListUrls);

            foreach (var url in urls)
            {
                var elem = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", url),
                    new XElement(xmlns + "changefreq", "daily")
                );
                root.Add(elem);
            }

            using (var ms = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    root.Save(writer);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}