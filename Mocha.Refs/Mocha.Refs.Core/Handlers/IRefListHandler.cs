using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public interface IRefListHandler
    {
        Task<PagedRefLists> GetRefListsAsync(GetRefListsRequest req);
        Task<RefList> GetRefListAsync(long refListId);
        Task<IEnumerable<RefList>> GetRefListsByContainingLink(long authorId, string url);
        Task<IEnumerable<RefList>> GetAllRefListsByContainingLink(string url);

        Task<SearchRefListsResponse> SearchRefListsAsync(string searchText, int start, int count);

        Task<RefList> CreateRefListAsync(long userId, CreateRefListRequest refList);
        Task<EntityIdentity> UpdateRefListAsync(UpdateRefListRequest request);
        Task RemoveRefListAsync(EntityIdentity listIdentity);

        Task<AddRefResponse> AddRefAsync(EntityIdentity listidentity, int refIndex, Ref refe);
        Task<EntityIdentity> MoveRefAsync(EntityIdentity listIdentity, int oldIndex, int newIndex);
        Task<EntityIdentity> UpdateRefAsync(EntityIdentity listIdentity, Ref updatedRef);
        Task<EntityIdentity> RemoveRefAsync(EntityIdentity listIdentity, int refIndex);
        Task<EntityIdentity> MoveRefToAsync(
            EntityIdentity fromListIdentity, EntityIdentity toListIdentity, int fromRefIndex, int toRefIndex = -1
        );

        Task IncrementViewCountAsync(long refListId);
        Task AddRefWithoutRowVersionAsync(long refListId, Ref refe);
    }
}
