using Microsoft.AspNet.Identity;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Auth
{
    public class IdentityUser: IUser
    {
        public User User { get; private set; }

        public IdentityUser(User user)
        {
            User = user;
        }

        #region IUser メンバー

        public string Id
        {
            get { return User.Id.ToString(); }
        }

        public string UserName
        {
            get { return User.UserName; }
            set { User.UserName = value; }
        }

        #endregion
    }
}
