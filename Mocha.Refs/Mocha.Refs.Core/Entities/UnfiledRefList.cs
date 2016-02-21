using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class UnfiledRefList
    {
        [Key, ForeignKey("Owner")]
        public long OwnerId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual User Owner { get; set; }

        //[InverseProperty("RefList")]
        public virtual ICollection<Ref> Refs { get; set; }

        // IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
    }
}
