using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class PagedRefListsViewModel
    {
        /// <summary>
        /// 1始まりの数字。
        /// </summary>
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int AllRefListCount { get; set; }
        public ICollection<RefListViewModel> RefLists { get; set; }
    }
}