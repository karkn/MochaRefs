using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class UserViewModel
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool HasProfileImage { get; set; }
    }
}