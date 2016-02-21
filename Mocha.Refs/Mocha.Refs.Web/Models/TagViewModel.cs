using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class TagViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int RefListCount { get; set; }
        public int FavoriteCount { get; set; }
    }
}