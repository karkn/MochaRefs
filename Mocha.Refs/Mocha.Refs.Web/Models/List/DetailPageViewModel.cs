using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class DetailPageViewModel
    {
        public bool IsEdit { get; set; }

        public bool CanEdit { get; set; }

        public bool IsFavored { get; set; }

        public RefListViewModel RefList { get; set; }

        public ICollection<RefViewModel> Refs { get; set; }

        public IEnumerable<string> OwnedTagUses { get; set; }

        //public ICollection<RefViewModel> UnfiledRefs { get; set; }
    }
}