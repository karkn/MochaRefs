using System;
using System.Collections.Generic;

namespace Mocha.Refs.Data.Models
{
    public partial class RefListStatistic
    {
        public long RefListId { get; set; }
        public int ViewCount { get; set; }
        public virtual RefList RefList { get; set; }
    }
}
