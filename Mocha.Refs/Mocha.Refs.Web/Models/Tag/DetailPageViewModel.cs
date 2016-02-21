using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.Tag
{
    [Serializable]
    public class DetailPageViewModel: PagedRefListsViewModel
    {
        public TagViewModel Tag { get; set; }

        public bool IsFavored { get; set; }

        public string TitleSearch { get; set; }
    }
}