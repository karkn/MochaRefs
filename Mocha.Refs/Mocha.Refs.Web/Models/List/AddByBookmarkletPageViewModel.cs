using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class AddByBookmarkletPageViewModel
    {
        public string Uri { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public ICollection<RefListViewModel> RefLists { get; set; }
    }
}
