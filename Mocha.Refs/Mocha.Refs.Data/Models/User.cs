using System;
using System.Collections.Generic;

namespace Mocha.Refs.Data.Models
{
    public partial class User
    {
        public User()
        {
            this.RefLists = new List<RefList>();
            this.TagUses = new List<TagUs>();
        }

        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public System.Guid MochaUserId { get; set; }
        public string DisplayName { get; set; }
        public virtual ICollection<RefList> RefLists { get; set; }
        public virtual ICollection<TagUs> TagUses { get; set; }
    }
}
