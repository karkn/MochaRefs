using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class SearchPageViewModel
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public string TextSearch { get; set; }
        public ICollection<RefListViewModel> RefLists { get; set; }
    }
}