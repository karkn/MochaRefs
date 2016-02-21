using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class TagStatistics
    {
        public int RefListCount { get; set; }
        public int FavoriteCount { get; set; }

        public long TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
