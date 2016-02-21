using HtmlAgilityPack;
using Mocha.Common.Cache;
using Mocha.Util.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Util.Web.Controllers
{
    public class UtilController : Controller
    {
        private static Regex MetaHttpEquivContentTypeRegex = new Regex(
            @"<meta\s+http-equiv\s*=\s*""Content-Type""\s+content\s*=\s*""(text/html;\s*charset=.*;{0,1})\s*""\s*/{0,1}>",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
        );
        private static Regex MetaCharsetRegex = new Regex(
            @"<meta\s*charset=""(.*)""\s*>",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
        );
        private static Regex LineStartWhitespaceRegex = new Regex(
            @"^\s+",
            RegexOptions.Compiled | RegexOptions.Multiline
        );
        private static Regex GooglePlusCountRegex = new Regex(
            @"\[2,([0-9.]+),\[",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant
        );

        private IObjectCache<string> _htmlCache;
        private IObjectCache<Size> _imageSizeCache;


        public UtilController()
        {
            _htmlCache = new ObjectCache<string>("html");
            _imageSizeCache = new ObjectCache<Size>("imageSize");
        }

        private ActionResult Jsonp(string callback, object obj)
        {
            return JavaScript(string.Format("{0}({1})", callback, obj.ToJson()));
        }

        [HttpGet]
        [OutputCache(Duration = 86400, VaryByParam = "*")] /// 24時間
        public async Task<ActionResult> GetTitle(string uri, string callback)
        {
            try
            {
                var html = await GetHtml(uri);
                if (string.IsNullOrWhiteSpace(html))
                {
                    return Jsonp(callback, null);
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                {
                    var title = GetOgTitleContent(doc, "//meta[@property='og:title']");
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        return Jsonp(callback, title);
                    }
                    title = GetOgTitleContent(doc, "//meta[@name='og:title']");
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        return Jsonp(callback, title);
                    }
                }

                {
                    var node = doc.DocumentNode.SelectNodes("//title");
                    if (node == null)
                    {
                        return null;
                    }
                    var title = node.First().InnerText;
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        title = HttpUtility.HtmlDecode(title);
                        return Jsonp(callback, title.Trim());
                    }
                }

                return Jsonp(callback, null);
            }
            catch (Exception)
            {
                return Jsonp(callback, null);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 86400, VaryByParam = "*")] /// 24時間
        public async Task<ActionResult> GetDescription(string uri, string callback)
        {
            try
            {
                var html = await GetHtml(uri);
                if (string.IsNullOrWhiteSpace(html))
                {
                    return Jsonp(callback, null);
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                {
                    var desc = GetDescriptionContent(doc, "//meta[@name='description']");
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        return Jsonp(callback, desc);
                    }
                    desc = GetDescriptionContent(doc, "//meta[@property='og:description']");
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        return Jsonp(callback, desc);
                    }
                    desc = GetDescriptionContent(doc, "//meta[@name='og:description']");
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        return Jsonp(callback, desc);
                    }
                }

                {
                    var text = ConvertHtml(doc);
                    var len = Math.Min(200, text.Length);
                    var desc = text.Length > 200 ? text.Substring(0, len).Trim() + "..." : text.Substring(0, len).Trim();
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        desc = LineStartWhitespaceRegex.Replace(desc, "");
                        desc = HttpUtility.HtmlEncode(desc);
                        return Jsonp(callback, desc);
                    }
                }
                
                return Jsonp(callback, null);
            }
            catch (Exception)
            {
                return Jsonp(callback, null);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 86400, VaryByParam = "*")] /// 24時間
        public async Task<ActionResult> GetImage(string uri, string callback)
        {
            try
            {
                var html = await GetHtml(uri);
                if (string.IsNullOrWhiteSpace(html))
                {
                    return Jsonp(callback, null);
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                {
                    var image = GetOgImageContent(doc, "//meta[@property='og:image']", uri);
                    if (!string.IsNullOrWhiteSpace(image))
                    {
                        return Jsonp(callback, image);
                    }
                    image = GetOgImageContent(doc, "//meta[@name='og:image']", uri);
                    if (!string.IsNullOrWhiteSpace(image))
                    {
                        return Jsonp(callback, image);
                    }

                    image = GetAppleTouchIconHref(doc, uri);
                    if (!string.IsNullOrWhiteSpace(image))
                    {
                        return Jsonp(callback, image);
                    }

                    image = await GetImageSrc(doc, uri);
                    if (!string.IsNullOrWhiteSpace(image))
                    {
                        return Jsonp(callback, image);
                    }
                }

                return Jsonp(callback, null);
            }
            catch (Exception)
            {
                return Jsonp(callback, null);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 86400, VaryByParam = "*")] /// 24時間
        public async Task<ActionResult> GetGooglePlusCount(string uri, string callback)
        {
            try
            {
                var gpUrl = "https://apis.google.com/_/+1/fastbutton?url=" + uri;
                var html = await GetHtml(gpUrl);
                if (string.IsNullOrWhiteSpace(html))
                {
                    return Jsonp(callback, null);
                }

                var match = GooglePlusCountRegex.Match(html);
                if (match.Success)
                {
                    var count = match.Groups[1].Value;
                    if (!string.IsNullOrWhiteSpace(count))
                    {
                        return Jsonp(callback, count.Trim());
                    }
                }

                return Jsonp(callback, null);
            }
            catch (Exception)
            {
                return Jsonp(callback, null);
            }
        }

        /// <summary>
        /// HttpClientではhttp://www.yomiuri.co.jp/で文字化けが起こった。(Shift_JISだから?)
        /// </summary>
        private async Task<string> GetHtml(string url)
        {
            if (_htmlCache.Exists(url))
            {
                return _htmlCache.Get(url);
            }

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var result = default(Uri);
            if (!Uri.TryCreate(url, UriKind.Absolute, out result))
            {
                throw new ArgumentException(string.Format("Illegal url format: {0}", url));
            }

            var client = new WebClient();

            var data = await client.DownloadDataTaskAsync(url);

            var html = "";
            var headerContentTypeValue = client.ResponseHeaders[HttpResponseHeader.ContentType];
            if (!string.IsNullOrWhiteSpace(headerContentTypeValue))
            {
                var contentType = new ContentType(headerContentTypeValue);
                if (contentType.CharSet != null)
                {
                    html = Encoding.GetEncoding(contentType.CharSet).GetString(data);
                }
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                var ascii = Encoding.ASCII.GetString(data);
                var match = MetaCharsetRegex.Match(ascii);
                var metaCharsetValue = match.Success ? match.Groups[1].Value : "";
                if (!string.IsNullOrWhiteSpace(metaCharsetValue))
                {
                    try
                    {
                        var enc = Encoding.GetEncoding(metaCharsetValue);
                        html = enc.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        /// Encoding.GetEncoding(metaCharsetValue)で例外が起きた場合、例外処理せずに次の処理に行く
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                var ascii = Encoding.ASCII.GetString(data);
                var match = MetaHttpEquivContentTypeRegex.Match(ascii);
                var metaContentTypeValue = match.Success ? match.Groups[1].Value : "";
                if (!string.IsNullOrWhiteSpace(metaContentTypeValue))
                {
                    var contentType = new ContentType(metaContentTypeValue);
                    if (contentType.CharSet != null)
                    {
                        html = Encoding.GetEncoding(contentType.CharSet).GetString(data);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                html = Encoding.UTF8.GetString(data);
            }

            if (!string.IsNullOrWhiteSpace(html))
            {
                _htmlCache.Add(url, html, 1440); // 24時間
            }

            return html;
        }

        private string GetOgTitleContent(HtmlDocument doc, string xpath)
        {
            var node = doc.DocumentNode.SelectNodes(xpath);
            if (node == null)
            {
                return null;
            }
            var title = node.First().GetAttributeValue("content", string.Empty);
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }
            title = HttpUtility.HtmlDecode(title);
            return title.Trim();
        }

        private string GetOgImageContent(HtmlDocument doc, string xpath, string linkUriString)
        {
            var node = doc.DocumentNode.SelectNodes(xpath);
            if (node == null)
            {
                return null;
            }
            var image = node.First().GetAttributeValue("content", string.Empty);
            if (string.IsNullOrWhiteSpace(image))
            {
                return null;
            }
            return EnsureAbsolute(image, linkUriString);
        }

        private string GetAppleTouchIconHref(HtmlDocument doc, string linkUriString)
        {
            var node = doc.DocumentNode.SelectNodes("//link[@rel='apple-touch-icon']");
            if (node == null)
            {
                return null;
            }
            var href = node.First().GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href))
            {
                return null;
            }
            return EnsureAbsolute(href, linkUriString);
        }

        private async Task<string> GetImageSrc(HtmlDocument doc, string linkUriString)
        {
            var srcs = GetImageSrcs(doc, linkUriString);
            if (srcs == null)
            {
                return null;
            }

            try
            {
                using (var client = new WebClient())
                {
                    // 最初の5個だけ対象とする
                    var sizes = await Task.WhenAll(srcs.Take(5).Select((src, i) => GetImageSize(client, src, i)));

                    var firstMatched = sizes.
                        Where(s => s != null).
                        OrderBy(r => r.Item1).
                        FirstOrDefault(r => IsCandidateImageSize(r.Item2));

                    return firstMatched.Item3;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsCandidateImageSize(Size size)
        {
            return
                (size.Width > 40 || size.Height > 40) &&
                (size.Width > size.Height ? size.Width / size.Height < 4 : size.Height / size.Width < 4);
        }

        private async Task<Tuple<int, Size, string>> GetImageSize(WebClient client, string uri, int index)
        {
            if (_imageSizeCache.Exists(uri))
            {
                var cached = _imageSizeCache.Get(uri);
                return cached == Size.Empty ? null : Tuple.Create(index, cached, uri);
            }

            var size = Size.Empty;

            try
            {
                using (var stream = await client.OpenReadTaskAsync(uri))
                {
                    var image = Image.FromStream(stream);
                    size = IsAnimationGif(image) ? Size.Empty : image.Size;
                }
            }
            catch (Exception)
            {
                size = Size.Empty;
            }

            _imageSizeCache.Add(uri, size, 1440); // 24時間

            return size == Size.Empty ? null : Tuple.Create(index, size, uri);
        }

        private bool IsAnimationGif(Image image)
        {
            try
            {
                var frameDimensions = new System.Drawing.Imaging.FrameDimension(image.FrameDimensionsList[0]);
                var frames = image.GetFrameCount(frameDimensions);
                return frames > 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string[] GetImageSrcs(HtmlDocument doc, string linkUriString)
        {
            var node = doc.DocumentNode.SelectNodes("//img");
            if (node == null)
            {
                return null;
            }
            var srcs = node.Select(n => n.GetAttributeValue("src", null)).Where(src => !string.IsNullOrWhiteSpace(src)).ToArray();
            if (srcs == null || srcs.Length == 0)
            {
                return null;
            }
            return srcs.Select(src => EnsureAbsolute(src, linkUriString)).Where(src => !string.IsNullOrWhiteSpace(src)).ToArray();
        }

        private string EnsureAbsolute(string uriString, string linkUriString)
        {
            var uri = default(Uri);
            if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
            {
                return uriString.Trim();
            }
            else
            {
                return RelativeToAbsolute(uriString, linkUriString);
            }
        }

        private Uri GetBaseUri(string uriString)
        {
            var uri = new Uri(uriString);
            return new Uri(uri.GetLeftPart(UriPartial.Authority));
        }

        private string RelativeToAbsolute(string href, string linkUriString)
        {
            var baseUri = GetBaseUri(linkUriString);
            var ret = default(Uri);
            return Uri.TryCreate(baseUri, href, out ret) ? ret.ToString() : null;
        }

        private string GetDescriptionContent(HtmlDocument doc, string xpath)
        {
            var node = doc.DocumentNode.SelectNodes(xpath);
            if (node == null)
            {
                return null;
            }
            var desc = node.First().GetAttributeValue("content", string.Empty);
            if (string.IsNullOrWhiteSpace(desc))
            {
                return null;
            }
            desc = HttpUtility.HtmlDecode(desc);
            desc = LineStartWhitespaceRegex.Replace(desc, "");
            return desc.Trim();
        }

        private string ConvertHtml(HtmlDocument doc)
        {
            var sw = new StringBuilder();
            ConvertTo(doc.DocumentNode, sw);
            return sw.ToString();
        }

        private void ConvertContentTo(HtmlNode node, StringBuilder outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        private void ConvertTo(HtmlNode node, StringBuilder outText)
        {
            var max = 200;
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Append(HtmlEntity.DeEntitize(html));
                        if (outText.Length > max)
                        {
                            return;
                        }
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.AppendLine();
                            if (outText.Length > max)
                            {
                                return;
                            }
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }
    }
}