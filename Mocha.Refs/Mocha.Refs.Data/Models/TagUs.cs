using System;
using System.Collections.Generic;

namespace Mocha.Refs.Data.Models
{
    public partial class TagUs
    {
        public TagUs()
        {
            this.RefLists = new List<RefList>();
        }

        public long Id { get; set; }
        public byte[] RowVersion { get; set; }
        public long TagId { get; set; }
        public long OwnerId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<RefList> RefLists { get; set; }
    }
}
