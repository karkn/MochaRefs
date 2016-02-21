using Mocha.Common.Exceptions;
using Mocha.Refs.Web.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Filters
{
    public class GlobalHandleErrorAttribute: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            /// filterContext.ExceptionHandled == trueでもログは取っておく
            LogUtil.LogControllerError(filterContext);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                /// Application_Errorは呼ばれない
                HandleAjaxRequestException(filterContext);
            }
            else
            {
                /// custom errorが有効でなければ
                /// ExceptionHandledがtrueにされないので
                /// Application_Errorも呼ばれることに注意
                base.OnException(filterContext);
            }
        }

        private void HandleAjaxRequestException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.Result = new JsonResult
            {
                Data = CreateData(filterContext.Exception),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        private object CreateData(Exception ex)
        {
            if (ex is BusinessException)
            {
                return new
                {
                    Message = ex.Message,
                    IsBusinessError = true,
                };
            }
            else
            {
                return new
                {
#if DEBUG
                    Message = ex.ToString(),
#else
                        Message = "", // Release時はクライアントで使わないので。
#endif
                    IsBusinessError = false,
                };
            }

        }

    }
}
