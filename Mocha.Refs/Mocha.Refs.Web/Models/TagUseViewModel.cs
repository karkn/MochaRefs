using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class TagUseViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
//        public TagViewModel Tag { get; set; }
        public byte[] RowVersion { get; set; }
        public int RefListCount { get; set; }
        public int PublishedRefListCount { get; set; }
    }
}