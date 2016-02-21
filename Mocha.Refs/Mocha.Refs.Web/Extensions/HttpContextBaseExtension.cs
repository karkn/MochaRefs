using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Extensions
{
    public static class HttpContextBaseExtension
    {
        public static IEnumerable<AuthenticationDescription> GetExternalAuthenticationTypes(this HttpContextBase context)
        {
            return context.GetOwinContext().Authentication.GetExternalAuthenticationTypes();
        }
    }
}