using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public interface IFavoriteHandler
    {
        Task<PagedRefLists> GetAllFavoriteRefListsAsync(long ownerId, PageCondition pageCondition, RefListSortKind sort);

        Task<PagedRefLists> GetFavoriteRefListsAsync(long ownerId, PageCondition pageCondition, FavoriteRefListSortKind sort);
        Task<IEnumerable<Tag>> GetFavoriteTagsAsync(long ownerId);
        Task<IEnumerable<User>> GetFavoriteUsersAsync(long ownerId);
        Task<bool> ExistsFavoriteRefListAsync(long ownerId, long refListId);
        Task<bool> ExistsFavoriteTagAsync(long ownerId, long tagId);
        Task<bool> ExistsFavoriteUserAsync(long ownerId, long userId);

        Task AddFavoriteRefListAsync(long ownerId, long refListId);
        Task RemoveFavoriteRefListAsync(long ownerId, long refListId);
        Task AddFavoriteTagAsync(long ownerId, long tagId);
        Task RemoveFavoriteTagAsync(long ownerId, long tagId);
        Task AddFavoriteUserAsync(long ownerId, long userId);
        Task RemoveFavoriteUserAsync(long ownerId, long userId);
    }
}
