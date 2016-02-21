using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.TagUse
{
    [Serializable]
    public class ManagePageViewModel
    {
        public UserViewModel User { get; set; } 
        public IEnumerable<TagUseViewModel> TagUses { get; set; }
    }
}