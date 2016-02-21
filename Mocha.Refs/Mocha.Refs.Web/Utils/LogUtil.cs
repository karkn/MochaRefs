using Mocha.Common.Exceptions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Utils
{
    public static class LogUtil
    {
        private static Logger _applicationlogger = LogManager.GetLogger("ApplicationError");
        private static Logger _controllerLogger = LogManager.GetLogger("ControllerError");


        public static void LogError(Exception ex, HttpContext context)
        {
            if (!(ex is BusinessException))
            {
                var req = context.Request;

                var user = context.User;
                var userName = user != null && user.Identity != null ? user.Identity.Name : "";

                var info = LogEventInfo.Create(LogLevel.Error, "", ex.Message);
                info.Properties["user"] = userName;
                info.Properties["userHost"] = req.UserHostAddress;
                info.Properties["userAgent"] = req.UserAgent;
                info.Properties["url"] = req.RawUrl;
                info.Properties["urlReferrer"] = req.UrlReferrer;
                info.Properties["exception"] = ex.ToString();

                _applicationlogger.Log(info);
            }
        }

        public static void LogErrorSimple(Exception ex)
        {
            if (!(ex is BusinessException))
            {
                var info = LogEventInfo.Create(LogLevel.Error, "", ex.Message);
                info.Properties["exception"] = ex.ToString();
                _applicationlogger.Log(info);
            }
        }

        public static void LogControllerError(ExceptionContext filterContext)
        {

            var ex = filterContext.Exception;
            if (!(ex is BusinessException))
            {
                var req = filterContext.HttpContext.Request;

                var user = filterContext.HttpContext.User;
                var userName = user != null && user.Identity != null ? user.Identity.Name : "";

                var info = LogEventInfo.Create(LogLevel.Error, "", ex.Message);
                info.Properties["user"] = userName;
                info.Properties["userHost"] = req.UserHostAddress;
                info.Properties["userAgent"] = req.UserAgent;
                info.Properties["url"] = req.RawUrl;
                info.Properties["urlReferrer"] = req.UrlReferrer;
                info.Properties["exception"] = ex.ToString();

                _controllerLogger.Log(info);
            }
        }


    }
}