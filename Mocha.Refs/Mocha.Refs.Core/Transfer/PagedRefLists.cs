using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class PagedRefLists
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int AllRefListCount { get; set; }
        public IEnumerable<RefList> RefLists { get; set; }
    }
}
