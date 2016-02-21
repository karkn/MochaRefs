using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class ManagePageViewModel: PagedRefListsViewModel
    {
        public UserViewModel Author { get; set; }

        public string TitleSearch { get; set; }

        public string TagUse { get; set; }
        public IEnumerable<string> OwnedTagUses { get; set; }
    }
}