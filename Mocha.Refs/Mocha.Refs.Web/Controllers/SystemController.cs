using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Web.Filters;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Controllers
{
    public class SystemController : AbstractController
    {
        private static Logger _logger = LogManager.GetLogger("ClientError");

        public SystemController(IUserHandler userHandler)
            : base(userHandler)
        {
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult LogClientError(string kind, string message, string url, string lineNumber, string stackTrace)
        {
            var req = HttpContext.Request;

            var user = HttpContext.User;
            var userName = user != null && user.Identity != null ? user.Identity.Name : "";

            var info = LogEventInfo.Create(LogLevel.Error, "", message);
            info.Properties["user"] = userName;
            info.Properties["userHost"] = req.UserHostAddress;
            info.Properties["userAgent"] = req.UserAgent;

            info.Properties["kind"] = kind;
            info.Properties["url"] = url;
            info.Properties["lineNumber"] = lineNumber;
            info.Properties["stackTrace"] = stackTrace;

            _logger.Log(info);

            return JsonNet(true);
        }

        [HttpGet]
        public ActionResult Ping()
        {
            return Content(DateTime.Now.ToString());
        }
    }
}