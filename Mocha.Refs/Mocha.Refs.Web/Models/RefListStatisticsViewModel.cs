using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class RefListStatisticsViewModel
    {
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public int LinkCount { get; set; }
    }
}