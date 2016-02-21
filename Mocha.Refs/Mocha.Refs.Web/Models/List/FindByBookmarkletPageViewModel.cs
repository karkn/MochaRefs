using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class OpenByBookmarkletPageViewModel
    {
        public bool IsOwnOnly { get; set; }
        public string Uri { get; set; }
        public string Title { get; set; }
        public ICollection<RefListViewModel> RefLists { get; set; }
    }
}