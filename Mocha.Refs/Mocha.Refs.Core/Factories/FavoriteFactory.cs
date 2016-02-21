using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Factories
{
    public static class FavoriteFactory
    {
        public static Favorite CreateForRefList(long ownerId, long refListId)
        {
            return new Favorite()
            {
                Kind = FavoriteKind.RefList,
                OwnerId = ownerId,
                RefListId = refListId,
            };
        }

        public static Favorite CreateForTag(long ownerId, long tagId)
        {
            return new Favorite()
            {
                Kind = FavoriteKind.Tag,
                OwnerId = ownerId,
                TagId = tagId,
            };
        }

        public static Favorite CreateForUser(long ownerId, long userId)
        {
            return new Favorite()
            {
                Kind = FavoriteKind.User,
                OwnerId = ownerId,
                UserId = userId,
            };
        }
    }
}
