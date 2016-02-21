using Mocha.Common.Exceptions;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Contracts;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Factories;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Core.Handlers.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public class TagHandler: ITagHandler
    {
        IRefsContext _refsContext;

        public TagHandler(IRefsContext refsContext)
        {
            _refsContext = refsContext;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(TagSortKind sort)
        {
            var query = _refsContext.Tags.Include("Statistics").AsNoTracking();
            query = QueryUtil.AppendQueryForTagSort(query, sort);
            var storedTags = await query.ToArrayAsync();
            return storedTags;
        }

        public async Task<Tag> GetTagAsync(string name)
        {
            SystemContract.RequireNotNullOrWhiteSpace(name, "name");

            var query = _refsContext.Tags.Include("Statistics").AsNoTracking().
                Where(t => t.Name == name);

            var storedTag = await query.SingleOrDefaultAsync();
            return storedTag;
        }

        public async Task<Tag> EnsureTagAsync(string name)
        {
            // todo: special charsを含んでいる場合もはじく
            SystemContract.RequireNotNullOrWhiteSpace(name, "name");

            var query = _refsContext.Tags.Include("Statistics").
                Where(t => t.Name == name);

            var storedTag = await query.SingleOrDefaultAsync();
            if (storedTag == null)
            {
                var tag = TagFactory.Create(name);
                _refsContext.Tags.Add(tag);
                await _refsContext.SaveChangesAsync();
                storedTag = tag;
            }

            return storedTag;
        }


        public async Task<IEnumerable<TagUse>> GetAllTagUsesAsync(long ownerId)
        {
            var query =
                from u in _refsContext.TagUses.Include("Statistics").AsNoTracking()
                where u.OwnerId == ownerId
                orderby u.Name
                select u;

            var storedTagUses = await query.ToArrayAsync();
            return storedTagUses;
        }

        public async Task<TagUse> GetTagUseAsync(long tagUseId)
        {
            var query = _refsContext.TagUses.Include("Statistics").AsNoTracking().
                Where(t => t.Id == tagUseId);
                    
            var storedTagUse = await query.SingleOrDefaultAsync();
            return storedTagUse;
        }

        public async Task<TagUse> GetTagUseAsync(long ownerId, string name)
        {
            SystemContract.RequireNotNullOrWhiteSpace(name, "name");

            var query = _refsContext.TagUses.Include("Statistics").AsNoTracking().
                Where(t => t.OwnerId == ownerId && t.Name == name);

            /// collationをcase insensitiveにしているので大文字小文字が異なるTagUseも取得される
            var storedTagUses = await query.ToArrayAsync();
            var storedTagUse = storedTagUses.SingleOrDefault(u => u.Name == name);
            return storedTagUse;
        }

        public async Task<TagUse> EnsureTagUseAsync(long ownerId, string name)
        {
            SystemContract.RequireNotNullOrWhiteSpace(name, "name");

            var query = _refsContext.TagUses.Include("Statistics").
                Where(t => t.OwnerId == ownerId && t.Name == name);

            /// collationをcase insensitiveにしているので大文字小文字が異なるTagUseも取得される
            var storedTagUses = await query.ToArrayAsync();
            var storedTagUse = storedTagUses.SingleOrDefault(u => u.Name == name);

            if (storedTagUse == null)
            {
                var tag = await EnsureTagAsync(name);
                var tagUse = TagUseFactory.Create(name, tag.Id, ownerId);
                _refsContext.TagUses.Add(tagUse);
                await _refsContext.SaveChangesAsync();
                storedTagUse = tagUse;
            }

            return storedTagUse;
        }

        public async Task<TagUse> RenameTagUseAsync(RenameTagUseRequest request)
        {
            SystemContract.RequireNotNullOrWhiteSpace(request.NewName, "request.NewName");
            SystemContract.RequireNotNullOrWhiteSpace(request.OldName, "request.OldName");

            /// NewNameの使用確認
            var newNameTagUseQuery = _refsContext.TagUses.Include("Owner").AsNoTracking().
                Where(u => u.OwnerId == request.OwnerId && u.Name == request.NewName);
            /// collationをcase insensitiveにしているので大文字小文字が異なるTagUseも取得される
            var newNameTagUses = await newNameTagUseQuery.ToArrayAsync();
            var isNewNameTagUseExists = newNameTagUses.Any(u => u.Name == request.NewName);
            BusinessContract.Require(!isNewNameTagUseExists, Errors.TagUseNameAlreadyExists, request.NewName);

            var oldNameTagUseQuery = _refsContext.TagUses.Include("Owner").
                Where(t => t.OwnerId == request.OwnerId && t.Name == request.OldName);
            /// collationをcase insensitiveにしているので大文字小文字が異なるTagUseも取得される
            var storedTagUses = await oldNameTagUseQuery.ToArrayAsync();
            var storedTagUse = storedTagUses.Single(u => u.Name == request.OldName);
            BusinessContract.ValidateWritePermission(request.OwnerId);
            BusinessContract.ValidateRowVersion(request.TagUseIdentity.RowVersion, storedTagUse.RowVersion);

            var tag = await EnsureTagAsync(request.NewName);
            storedTagUse.TagId = tag.Id;
            storedTagUse.Name = request.NewName;
            
            _refsContext.MarkAsModified(storedTagUse);
            await _refsContext.SaveChangesAsync();

            return storedTagUse;
        }

        public async Task RemoveTagUsesAsync(EntityIdentity identity)
        {
            SystemContract.RequireEntityIdentity(identity);

            var query = _refsContext.TagUses.Include("RefLists").
                Where(t => t.Id == identity.Id);

            var storedTagUse = await query.SingleAsync();
            BusinessContract.ValidateRowVersion(storedTagUse.RowVersion, identity.RowVersion);
            BusinessContract.ValidateWritePermission(storedTagUse.OwnerId);

            storedTagUse.RefLists.Clear();
            _refsContext.TagUses.Remove(storedTagUse);

            await _refsContext.SaveChangesAsync();
        }

        //public async Task<bool> SweepTagUsesAsync(long ownerId, string name)
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        throw new ArgumentException();
        //    }

        //    var query = _refsContext.TagUses.Include("Tag").Include("RefLists").
        //        Where(t => t.OwnerId == ownerId && t.Name == name && !t.RefLists.Any());

        //    var storedTagUse = await query.SingleOrDefaultAsync();
        //    if (storedTagUse != null)
        //    {
        //        _refsContext.TagUses.Remove(storedTagUse);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }
}
