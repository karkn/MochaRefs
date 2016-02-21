using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Filters
{
    public class UserInfoFilter: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var container = MochaContainer.GetContainer();

            var user = filterContext.HttpContext.User;
            var identityUserId = user.Identity.GetUserId();

            var refsUser = default(User);
            if (identityUserId != null)
            {
                var handler = container.Resolve<IUserHandler>();
                refsUser = handler.GetUser(Convert.ToInt64(identityUserId));
            }
            filterContext.HttpContext.Items[WebConsts.RefsUserKey] = refsUser;

            filterContext.Controller.ViewBag.User = refsUser;
            filterContext.Controller.ViewBag.Id = refsUser == null ? "" : refsUser.Id.ToString();
            filterContext.Controller.ViewBag.UserName = refsUser == null ? "" : refsUser.UserName;
            filterContext.Controller.ViewBag.DisplayName = refsUser == null ? "" : refsUser.DisplayName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // do nothing
        }

    }
}