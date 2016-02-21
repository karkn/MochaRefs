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
    public class User
    {
        public User()
        {
            UserLogins = new List<UserLogin>();
            UserData = new List<UserData>();

            AuthoredRefLists = new List<RefList>();
            OwnedTagUses = new List<TagUse>();
            OwnedFavorites = new List<Favorite>();
        }

        public long Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string SecurityStamp { get; set; }
        //public Guid MochaUserId { get; set; }

        public virtual ICollection<UserLogin> UserLogins { get; set; }
        public virtual ICollection<UserData> UserData { get; set; }

        public virtual ICollection<RefList> AuthoredRefLists { get; set; }

        public virtual ICollection<TagUse> OwnedTagUses { get; set; }

        /// <summary>
        /// このユーザーが登録したお気に入り。
        /// </summary>
        public virtual ICollection<Favorite> OwnedFavorites { get; set; }

        /// <summary>
        /// このユーザーに対するお気に入り。
        /// </summary>
        public virtual ICollection<Favorite> FavoringFavorites { get; set; }

    }
}
