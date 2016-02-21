using Mocha.Common.Data;
using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class Favorite: IAuditable
    {
        public long Id { get; set; }

        [Required]
        public FavoriteKind Kind { get; set; }

        public long OwnerId { get; set; }
        public User Owner { get; set; }

        public long? RefListId { get; set; }
        public RefList RefList { get; set; }

        public long? TagId { get; set; }
        public Tag Tag { get; set; }

        public long? UserId { get; set; }
        public User User { get; set; }

        /// IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
    }
}
