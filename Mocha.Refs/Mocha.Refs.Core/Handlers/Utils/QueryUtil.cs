using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers.Utils
{
    internal static class QueryUtil
    {
        public static IQueryable<RefList> AppendQueryForRefListSort(IQueryable<RefList> query, RefListSortKind sort)
        {
            switch (sort)
            {
                case RefListSortKind.UpdatedDateDescending:
                    query = query.OrderByDescending(l => l.UpdatedDate);
                    break;
                case RefListSortKind.UpdatedDateAscending:
                    query = query.OrderBy(l => l.UpdatedDate);
                    break;
                case RefListSortKind.CreatedDateDescending:
                    query = query.OrderByDescending(l => l.CreatedDate);
                    break;
                case RefListSortKind.CreatedDateAscending:
                    query = query.OrderBy(l => l.CreatedDate);
                    break;
                case RefListSortKind.PublishedDateDescending:
                    query = query.OrderByDescending(l => l.PublishedDate);
                    break;
                case RefListSortKind.FavoriteCountDescending:
                    query = query.
                        OrderByDescending(l => l.Statistics.FavoriteCount).ThenByDescending(l => l.PublishedDate);
                    break;
                case RefListSortKind.ViewCountDescending:
                    query = query.
                        OrderByDescending(l => l.Statistics.ViewCount).ThenByDescending(l => l.PublishedDate);
                    break;
            }
            return query;
        }

        public static IQueryable<Favorite> AppendQueryForFavoriteRefListSort(IQueryable<Favorite> query, FavoriteRefListSortKind sort)
        {
            switch (sort)
            {
                case FavoriteRefListSortKind.RefListUpdatedDateDescending:
                    query = query.OrderByDescending(f => f.RefList.UpdatedDate);
                    break;
                case FavoriteRefListSortKind.RefListUpdatedDateAscending:
                    query = query.OrderBy(f => f.RefList.UpdatedDate);
                    break;
                case FavoriteRefListSortKind.RefListCreatedDateDescending:
                    query = query.OrderByDescending(f => f.RefList.CreatedDate);
                    break;
                case FavoriteRefListSortKind.RefListCreatedDateAscending:
                    query = query.OrderBy(f => f.RefList.CreatedDate);
                    break;
                case FavoriteRefListSortKind.RefListPublishedDateDescending:
                    query = query.OrderByDescending(f => f.RefList.PublishedDate);
                    break;
                case FavoriteRefListSortKind.FavoriteCountDescending:
                    query = query.
                        OrderByDescending(f => f.RefList.Statistics.FavoriteCount).ThenByDescending(l => l.CreatedDate);
                    break;
                case FavoriteRefListSortKind.FavoriteCreatedDescending:
                    query = query.
                        OrderByDescending(f => f.CreatedDate);
                    break;
            }
            return query;
        }

        public static IQueryable<Tag> AppendQueryForTagSort(IQueryable<Tag> query, TagSortKind sort)
        {
            switch (sort)
            {
                case TagSortKind.RefListCountDescending:
                    query = query.OrderByDescending(t => t.Statistics.RefListCount).ThenBy(t => t.Name);
                    break;
                case TagSortKind.FavoriteCountDescending:
                    query = query.OrderByDescending(t => t.Statistics.FavoriteCount).ThenBy(t => t.Name);
                    break;
                case TagSortKind.NameAscending:
                    query = query.OrderBy(t => t.Name);
                    break;
            }
            return query;
        }
    }
}
