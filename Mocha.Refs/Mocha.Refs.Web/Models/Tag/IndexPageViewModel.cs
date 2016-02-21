using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.Tag
{
    [Serializable]
    public class IndexPageViewModel
    {
        public ICollection<TagViewModel> Tags { get; set; }
    }
}