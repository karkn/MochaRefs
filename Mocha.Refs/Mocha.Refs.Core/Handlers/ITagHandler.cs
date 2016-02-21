using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public interface ITagHandler
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync(TagSortKind sort);
        Task<Tag> GetTagAsync(string name);
        Task<Tag> EnsureTagAsync(string name);

        Task<TagUse> EnsureTagUseAsync(long ownerId, string name);
        Task<IEnumerable<TagUse>> GetAllTagUsesAsync(long ownerId);
        Task<TagUse> GetTagUseAsync(long tagUseId);
        Task<TagUse> GetTagUseAsync(long ownerId, string name);
        Task<TagUse> RenameTagUseAsync(RenameTagUseRequest request);
        Task RemoveTagUsesAsync(EntityIdentity identity);
        //Task<bool> SweepTagUsesAsync(long ownerId, string name);
    }
}
