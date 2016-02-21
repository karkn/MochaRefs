using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class PageCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PageCondition(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
