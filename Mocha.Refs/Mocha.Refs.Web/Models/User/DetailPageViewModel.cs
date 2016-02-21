using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.User
{
    [Serializable]
    public class DetailPageViewModel: PagedRefListsViewModel
    {
        public UserViewModel Author { get; set; }

        public bool IsFavored { get; set; }

        public string TitleSearch { get; set; }
        public string TagUse { get; set; }

        public ICollection<TagUseViewModel> OwnedTagUses { get; set; }
    }
}