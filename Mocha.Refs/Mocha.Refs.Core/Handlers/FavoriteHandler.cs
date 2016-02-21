using Mocha.Common.Utils;
using Mocha.Refs.Core.Contracts;
using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Factories;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Core.Handlers.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public class FavoriteHandler: IFavoriteHandler
    {
        IRefsContext _refsContext;

        public FavoriteHandler(IRefsContext refsContext)
        {
            _refsContext = refsContext;
        }

        public async Task<PagedRefLists> GetFavoriteRefListsAsync(
            long ownerId, PageCondition pageCondition, FavoriteRefListSortKind sort
        )
        {
            var query = _refsContext.Favorites.
                Include("RefList").Include("RefList.Author").Include("RefList.TagUses.Tag").Include("RefList.Statistics").AsNoTracking().
                Where(f => f.OwnerId == ownerId && f.Kind == FavoriteKind.RefList);

            var count = await query.CountAsync();

            query = QueryUtil.AppendQueryForFavoriteRefListSort(query, sort);
            query = query.Skip(pageCondition.PageIndex * pageCondition.PageSize);
            query = query.Take(pageCondition.PageSize);

            var storedFavs = await query.ToArrayAsync();

            return new PagedRefLists()
            {
                PageIndex = pageCondition.PageIndex,
                PageCount = IndexUtil.GetPageCount(count, pageCondition.PageSize),
                RefLists = storedFavs.Select(f => f.RefList),
                AllRefListCount = count,
            };
        }

        public async Task<IEnumerable<Tag>> GetFavoriteTagsAsync(long ownerId)
        {
            var query = _refsContext.Favorites.Include("Tag").AsNoTracking().
                Where(f => f.OwnerId == ownerId && f.Kind == FavoriteKind.Tag);
            query = query.OrderByDescending(f => f.CreatedDate);

            var storedFavs = await query.ToArrayAsync();

            return storedFavs.Select(f => f.Tag);
        }

        public async Task<IEnumerable<User>> GetFavoriteUsersAsync(long ownerId)
        {
            var query = _refsContext.Favorites.Include("User").AsNoTracking().
                Where(f => f.OwnerId == ownerId && f.Kind == FavoriteKind.User);
            query = query.OrderByDescending(f => f.CreatedDate);

            var storedFavs = await query.ToArrayAsync();

            return storedFavs.Select(f => f.User);
        }

        public async Task<PagedRefLists> GetAllFavoriteRefListsAsync(long ownerId, PageCondition pageCondition, RefListSortKind sort)
        {
            var refListIdsQuery = _refsContext.Favorites.
                Where(f => f.OwnerId == ownerId && f.Kind == FavoriteKind.RefList).
                Select(f => f.RefListId.Value);
            var refListIds = await refListIdsQuery.ToArrayAsync();

            var tagIdsQuery = _refsContext.Favorites.
                Where(f => f.OwnerId == ownerId && f.Kind == FavoriteKind.Tag).
                Select(f => f.TagId.Value);
            var tagIds = await tagIdsQuery.ToArrayAsync();

            var userIdsQuery = _refsContext.Favorites.
                Where(f => f.OwnerId == ownerId && f.Kind == FavoriteKind.User).
                Select(f => f.UserId.Value);
            var userIds = await userIdsQuery.ToArrayAsync();

            var query = _refsContext.RefLists.AsNoTracking().
                Where(l =>
                    l.PublishingStatus == PublishingStatusKind.Publish &&
                    (
                        refListIds.Contains(l.Id) ||
                        tagIds.Intersect(l.TagUses.Select(u => u.TagId)).Any() ||
                        userIds.Contains(l.AuthorId)
                    )
                );

            var refListCount = await query.CountAsync();

            query = QueryUtil.AppendQueryForRefListSort(query, sort);

            query = query.Skip(pageCondition.PageIndex * pageCondition.PageSize);
            query = query.Take(pageCondition.PageSize);

            var storedRefList = await query.ToArrayAsync();
            return new PagedRefLists()
            {
                PageIndex = pageCondition.PageIndex,
                PageCount = IndexUtil.GetPageCount(refListCount, pageCondition.PageSize),
                RefLists = storedRefList,
                AllRefListCount = refListCount,
            };
        }

        public async Task<bool> ExistsFavoriteRefListAsync(long ownerId, long refListId)
        {
            var query = _refsContext.Favorites.AsNoTracking();
            var result = await query.AnyAsync(
                f => f.OwnerId == ownerId && f.RefListId == refListId && f.Kind == FavoriteKind.RefList
            );
            return result;
        }

        public async Task<bool> ExistsFavoriteTagAsync(long ownerId, long tagId)
        {
            var query = _refsContext.Favorites.AsNoTracking();
            var result = await query.AnyAsync(
                f => f.OwnerId == ownerId && f.TagId == tagId && f.Kind == FavoriteKind.Tag
            );
            return result;
        }

        public async Task<bool> ExistsFavoriteUserAsync(long ownerId, long userId)
        {
            var query = _refsContext.Favorites.AsNoTracking();
            var result = await query.AnyAsync(
                f => f.OwnerId == ownerId && f.UserId == userId && f.Kind == FavoriteKind.User
            );
            return result;
        }


        public async Task AddFavoriteRefListAsync(long ownerId, long refListId)
        {
            var exists = await ExistsFavoriteRefListAsync(ownerId, refListId);
            SystemContract.Require(
                !exists,
                "登録済みのお気に入りリスト, ownerId: {0}, refListId: {1}", ownerId.ToString(), refListId.ToString()
            );

            /// update statistics data
            {
                var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == refListId);
                var storedRefListStatistics = await statisticsQuery.SingleAsync();

                var favCountQuery = _refsContext.Favorites.AsNoTracking();
                var favCount = await favCountQuery.CountAsync(s => s.RefListId == refListId);
                storedRefListStatistics.FavoriteCount = favCount + 1;
            }

            var fav = FavoriteFactory.CreateForRefList(ownerId, refListId);
            _refsContext.Favorites.Add(fav);

            await _refsContext.SaveChangesAsync();
        }

        public async Task RemoveFavoriteRefListAsync(long ownerId, long refListId)
        {
            var query = _refsContext.Favorites.Where(
                f => f.OwnerId == ownerId && f.RefListId == refListId && f.Kind == FavoriteKind.RefList
            );
            var storedFav = query.SingleOrDefault();

            SystemContract.Require(
                storedFav != null,
                "未登録のお気に入りリスト, ownerId: {0}, refListId: {1}", ownerId.ToString(), refListId.ToString()
            );

            /// update statistics data
            {
                var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == refListId);
                var storedRefListStatistics = await statisticsQuery.SingleAsync();

                var favCountQuery = _refsContext.Favorites.AsNoTracking();
                var favCount = await favCountQuery.CountAsync(s => s.RefListId == refListId);
                storedRefListStatistics.FavoriteCount = favCount - 1;
            }

            _refsContext.Favorites.Remove(storedFav);

            await _refsContext.SaveChangesAsync();
        }

        public async Task AddFavoriteTagAsync(long ownerId, long tagId)
        {
            var exists = await ExistsFavoriteTagAsync(ownerId, tagId);
            SystemContract.Require(
                !exists,
                "登録済みのお気に入りタグ, ownerId: {0}, tagId: {1}", ownerId.ToString(), tagId.ToString()
            );

            /// update statistics data
            //{
            //    var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == userId);
            //    var storedRefListStatistics = await statisticsQuery.SingleAsync();

            //    var favCountQuery = _refsContext.Favorites.AsNoTracking();
            //    var favCount = await favCountQuery.CountAsync(s => s.RefListId == userId);
            //    storedRefListStatistics.FavoriteCount = favCount + 1;
            //}

            var fav = FavoriteFactory.CreateForTag(ownerId, tagId);
            _refsContext.Favorites.Add(fav);

            await _refsContext.SaveChangesAsync();
        }

        public async Task RemoveFavoriteTagAsync(long ownerId, long tagId)
        {
            var query = _refsContext.Favorites.Where(
                f => f.OwnerId == ownerId && f.TagId == tagId && f.Kind == FavoriteKind.Tag
            );
            var storedFav = query.SingleOrDefault();

            SystemContract.Require(
                storedFav != null,
                "未登録のお気に入りタグ, ownerId: {0}, tagId: {1}", ownerId.ToString(), tagId.ToString()
            );

            /// update statistics data
            //{
            //    var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == userId);
            //    var storedRefListStatistics = await statisticsQuery.SingleAsync();

            //    var favCountQuery = _refsContext.Favorites.AsNoTracking();
            //    var favCount = await favCountQuery.CountAsync(s => s.RefListId == userId);
            //    storedRefListStatistics.FavoriteCount = favCount - 1;
            //}

            _refsContext.Favorites.Remove(storedFav);

            await _refsContext.SaveChangesAsync();
        }

        public async Task AddFavoriteUserAsync(long ownerId, long userId)
        {
            var exists = await ExistsFavoriteUserAsync(ownerId, userId);
            SystemContract.Require(
                !exists,
                "登録済みのお気に入りユーザ, ownerId: {0}, userId: {1}", ownerId.ToString(), userId.ToString()
            );

            /// update statistics data
            //{
            //    var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == userId);
            //    var storedRefListStatistics = await statisticsQuery.SingleAsync();

            //    var favCountQuery = _refsContext.Favorites.AsNoTracking();
            //    var favCount = await favCountQuery.CountAsync(s => s.RefListId == userId);
            //    storedRefListStatistics.FavoriteCount = favCount + 1;
            //}

            var fav = FavoriteFactory.CreateForUser(ownerId, userId);
            _refsContext.Favorites.Add(fav);

            await _refsContext.SaveChangesAsync();
        }

        public async Task RemoveFavoriteUserAsync(long ownerId, long userId)
        {
            var query = _refsContext.Favorites.Where(
                f => f.OwnerId == ownerId && f.UserId == userId && f.Kind == FavoriteKind.User
            );
            var storedFav = query.SingleOrDefault();

            SystemContract.Require(
                storedFav != null,
                "未登録のお気に入りユーザ, ownerId: {0}, userId: {1}", ownerId.ToString(), userId.ToString()
            );

            /// update statistics data
            //{
            //    var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == userId);
            //    var storedRefListStatistics = await statisticsQuery.SingleAsync();

            //    var favCountQuery = _refsContext.Favorites.AsNoTracking();
            //    var favCount = await favCountQuery.CountAsync(s => s.RefListId == userId);
            //    storedRefListStatistics.FavoriteCount = favCount - 1;
            //}

            _refsContext.Favorites.Remove(storedFav);

            await _refsContext.SaveChangesAsync();
        }

    }
}
