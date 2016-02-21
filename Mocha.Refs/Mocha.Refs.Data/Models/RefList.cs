using System;
using System.Collections.Generic;

namespace Mocha.Refs.Data.Models
{
    public partial class RefList
    {
        public RefList()
        {
            this.Refs = new List<Ref>();
            this.TagUses = new List<TagUs>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public byte PublishingStatus { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] RowVersion { get; set; }
        public long AuthorId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
        public virtual User User { get; set; }
        public virtual RefListStatistic RefListStatistic { get; set; }
        public virtual ICollection<Ref> Refs { get; set; }
        public virtual ICollection<TagUs> TagUses { get; set; }
    }
}
