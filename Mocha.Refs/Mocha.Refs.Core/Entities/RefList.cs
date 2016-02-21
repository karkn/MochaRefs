using Mocha.Common.Data;
using Mocha.Refs.Core.DataTypes;
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
    public class RefList : IAuditable
    {
        public RefList()
        {
            Refs = new List<Ref>();
            TagUses = new List<TagUse>();
        }

        public long Id { get; set; }

        //public RefListKind Kind { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public PublishingStatusKind PublishingStatus { get; set; }

        public DateTime? PublishedDate { get; set; }

        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public long AuthorId { get; set; }
        public virtual User Author { get; set; }

        public virtual ICollection<Ref> Refs { get; set; }

        public virtual ICollection<TagUse> TagUses { get; set; }

        public virtual ICollection<Favorite> FavoringFavorites { get; set; }

        public virtual RefListStatistics Statistics { get; set; }

        /// IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
    }
}
