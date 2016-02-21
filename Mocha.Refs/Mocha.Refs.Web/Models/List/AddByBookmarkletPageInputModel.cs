using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class AddByBookmarkletPageInputModel
    {
        public string Uri { get; set; }

        [AllowHtml]
        public string Title { get; set; }

        [AllowHtml]
        public string Comment { get; set; }

        public long RefListId { get; set; }
    }
}
