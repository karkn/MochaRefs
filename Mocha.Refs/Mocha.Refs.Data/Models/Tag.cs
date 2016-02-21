using System;
using System.Collections.Generic;

namespace Mocha.Refs.Data.Models
{
    public partial class Tag
    {
        public Tag()
        {
            this.TagUses = new List<TagUs>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
        public virtual ICollection<TagUs> TagUses { get; set; }
    }
}
