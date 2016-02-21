using Mocha.Common.Data;
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
    public class TagUse: IAuditable
    {
        public TagUse()
        {
            RefLists = new List<RefList>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public long TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public long OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public virtual ICollection<RefList> RefLists { get; set; }

        public virtual TagUseStatistics Statistics { get; set; }

        /// IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
    }
}
