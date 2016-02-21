using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.User
{
    [Serializable]
    public class FavoriteUserPageViewModel
    {
        public ICollection<UserViewModel> Users { get; set; }
    }
}