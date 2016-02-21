using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class CreatePageViewModel
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public IEnumerable<string> OwnedTagUses { get; set; }
        public IEnumerable<string> TagUses { get; set; }
        public IEnumerable<RefViewModel> Refs { get; set; }
    }
}