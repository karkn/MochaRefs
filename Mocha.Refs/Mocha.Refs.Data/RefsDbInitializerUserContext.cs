using Mocha.Refs.Core;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Data
{
    internal class RefsDbInitializerUserContext: IUserContext
    {
        private RefsContext _refsContext;

        public RefsDbInitializerUserContext(RefsContext refsContext)
        {
            _refsContext = refsContext;
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public User GetUser()
        {
            if (_refsContext == null)
            {
                throw new InvalidOperationException();
            }

            var user = (
                from u in _refsContext.Users
                where u.UserName == "mctest1"
                select u
            ).SingleOrDefault();

            return user;
        }
    }
}
