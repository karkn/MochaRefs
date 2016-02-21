using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;

namespace Mocha.Refs.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            DisplayModeProvider.Instance.Modes.Clear();
            DisplayModeProvider.Instance.Modes.Add(new DefaultDisplayMode());

            MvcHandler.DisableMvcResponseHeader = true;

            AntiForgeryConfig.CookieName = WebConsts.RequestVerificationTokenCookieName;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ChannelConfig.Config();
            MapConfig.CreateMaps();

            Mocha.Refs.Bootstrapper.Bootstrapper.InitContainer(() => new Microsoft.Practices.Unity.PerRequestLifetimeManager());

            Mocha.Refs.Bootstrapper.Bootstrapper.InitDatabase();

            //ScheduleConfig.Config();

#if !DEBUG
            ScheduleConfig.Config();
#endif
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            /// Controller外で起きた例外処理
            if (Server != null)
            {
                var ex = Server.GetLastError();
                if (ex != null)
                {
                    if (ex is HttpException && ((HttpException)ex).GetHttpCode() == (int)HttpStatusCode.NotFound)
                    {
                        /// NotFoundを相手にするとログが大変になるので無視
                        return;
                    }
                    
                    /// CustomErrorが無効な場合(=ローカルアクセス時≒開発時)は
                    /// Controller内でおきた例外がここでもログ出力されてしまうことに注意
                    /// CustomErrorが有効な場合(=リモートアクセス時≒運用時)は
                    /// Controller外でおきた例外のみここでログ出力される
                    try
                    {
                        LogUtil.LogError(ex, HttpContext.Current);
                    }
                    catch (Exception)
                    {
                        LogUtil.LogErrorSimple(ex);
                    }
                }
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
        }
    }
}
