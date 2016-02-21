using Mocha.Refs.Core;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Filters
{
    public class HttpContextUserContext: IUserContext
    {
        public HttpContextUserContext()
        {
        }

        public bool IsAuthenticated
        {
            get { return GetUser() != null; }
        }

        public User GetUser()
        {
            return (User)HttpContext.Current.Items[WebConsts.RefsUserKey];
        }
    }
}