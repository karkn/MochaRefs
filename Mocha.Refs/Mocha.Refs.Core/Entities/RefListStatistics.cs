using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class RefListStatistics
    {
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public int LinkCount { get; set; }

        public long RefListId { get; set; }

        public virtual RefList RefList { get; set; }
    }
}
