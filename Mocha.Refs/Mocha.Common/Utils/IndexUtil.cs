using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Utils
{
    public static class IndexUtil
    {
        public static int GetPageCount(int elemCount, int pageSize)
        {
            if (elemCount < 0)
            {
                throw new ArgumentOutOfRangeException("elemCount");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }
            return ((elemCount - 1) / pageSize) + 1;
        }
    }
}
