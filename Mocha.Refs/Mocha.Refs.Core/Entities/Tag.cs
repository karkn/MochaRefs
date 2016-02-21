using Mocha.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class Tag: IAuditable
    {
        public Tag()
        {
            TagUses = new List<TagUse>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<TagUse> TagUses { get; set; }

        public virtual ICollection<Favorite> FavoringFavorites { get; set; }

        public virtual TagStatistics Statistics { get; set; }

        /// IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
    }
}
