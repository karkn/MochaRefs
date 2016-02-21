using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class RefViewModel
    {
        public long Id { get; set; }

        public RefKind Kind { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public int DisplayOrder { get; set; }
    }
}