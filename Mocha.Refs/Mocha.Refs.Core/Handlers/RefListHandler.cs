using Microsoft.Practices.Unity;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Contracts;
using Mocha.Common.Utils;
using Mocha.Common.Cache;
using Mocha.Common.Channel;
using Mocha.Common.Extensions;
using Mocha.Refs.Core.Search;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Handlers.Messages;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Handlers.Utils;
using Mocha.Refs.Core.Repositories;

namespace Mocha.Refs.Core.Handlers
{
    public class RefListHandler: IRefListHandler
    {
        IRefsContext _refsContext;
        ISearchEngine _searchEngine;
        IUserHandler _userHandler;
        ITagHandler _tagHandler;

        IChannel _channel;

        public RefListHandler(
            IRefsContext refsContext, ISearchEngine searchEngine, IUserHandler userHandler, ITagHandler tagHandler,
            IChannel channel
        )
        {
            _refsContext = refsContext;
            _searchEngine = searchEngine;
            _userHandler = userHandler;
            _tagHandler = tagHandler;

            _channel = channel;
        }

        /// <summary>
        /// createdUserIdが作成したRefListを返します。
        /// </summary>
        public async Task<PagedRefLists> GetRefListsAsync(GetRefListsRequest req)
        {
            var query = _refsContext.RefLists.Include("Author").Include("TagUses.Tag").Include("Statistics").AsNoTracking();
            query = AppendQueryForAuthorId(query, req.AuthorId);
            query = AppendQueryForAuthorUserName(query, req.AuthorUserName);
            query = AppendQueryForTagUseId(query, req.TagUseId);
            query = AppendQueryForTagName(query, req.TagName, req.TagUseId);
            query = AppendQueryForFromDate(query, req.FromDate);
            query = AppendQueryForSearchText(query, req.TitleSearch);
            query = AppendQueryForPublishingStatus(query, req.PublishingStatusCondition);

            var storedRefListCount = await query.CountAsync();

            query = QueryUtil.AppendQueryForRefListSort(query, req.Sort);

            query = query.Skip(req.PageIndex * req.PageSize);
            query = query.Take(req.PageSize);

            var storedLists = await query.ToArrayAsync();

            var ret = new PagedRefLists()
            {
                PageIndex = req.PageIndex,
                PageCount = IndexUtil.GetPageCount(storedRefListCount, req.PageSize),
                AllRefListCount = storedRefListCount,
                RefLists = storedLists,
            };
            return ret;
        }

        #region 条件の追加用メソッド
        private IQueryable<RefList> AppendQueryForAuthorId(IQueryable<RefList> query, long? authorId)
        {
            if (authorId != null)
            {
                query = query.Where(l => l.AuthorId == authorId);
            }
            return query;
        }

        private IQueryable<RefList> AppendQueryForAuthorUserName(IQueryable<RefList> query, string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(l => l.Author.UserName == userName);
            }
            return query;
        }

        private IQueryable<RefList> AppendQueryForTagUseId(IQueryable<RefList> query, long? tagUseId)
        {
            if (tagUseId != null)
            {
                if (tagUseId == CoreConsts.UnsetTagId)
                {
                    query = query.Where(l => !l.TagUses.Any());
                }
                else
                {
                    query = query.Where(l => l.TagUses.Any(u => u.Id == tagUseId));
                }
            }
            return query;
        }

        private IQueryable<RefList> AppendQueryForTagName(IQueryable<RefList> query, string tagName, long? tagUseId)
        {
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                if (tagName == CoreConsts.UnsetTagName)
                {
                    query = query.Where(l => !l.TagUses.Any());
                }
                else
                {
                    query = query.Where(l => l.TagUses.Any(u => u.Name == tagName));
                }
            }
            return query;
        }

        private IQueryable<RefList> AppendQueryForSearchText(IQueryable<RefList> query, string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(l => !string.IsNullOrEmpty(l.Title) && l.Title.Contains(searchText));
            }
            return query;
        }

        private IQueryable<RefList> AppendQueryForFromDate(IQueryable<RefList> query, DateTime? fromDate)
        {
            if (fromDate != null)
            {
                query = query.Where(l => l.PublishedDate >= fromDate);
            }
            return query;
        }

        private IQueryable<RefList> AppendQueryForPublishingStatus
            (IQueryable<RefList> query,
            PublishingStatusConditionKind publishingStatusCondition
        )
        {
            switch (publishingStatusCondition)
            {
                case PublishingStatusConditionKind.PublishOnly:
                    query = query.Where(l => l.PublishingStatus == PublishingStatusKind.Publish);
                    break;
                case PublishingStatusConditionKind.PrivateOnly:
                    query = query.Where(l => l.PublishingStatus == PublishingStatusKind.Private);
                    break;
                case PublishingStatusConditionKind.DraftOnly:
                    query = query.Where(l => l.PublishingStatus == PublishingStatusKind.Draft);
                    break;
            }
            return query;
        }
        #endregion

        public async Task<IEnumerable<RefList>> GetRefListsByContainingLink(long authorId, string url)
        {
            return await GetRefListsByContainingLinkCore(authorId, url);
        }

        public async Task<IEnumerable<RefList>> GetAllRefListsByContainingLink(string url)
        {
            return await GetRefListsByContainingLinkCore(null, url);
        }

        private string GetBaseUrlForQuery(Uri uri, int distance)
        {
            var left = uri.GetLeftPart(UriPartial.Authority);
            var segments = uri.Segments; /// 最初の要素は"/"
            if (segments.Length < distance + 2)
            {
                return left;
            }
            else
            {
                /// "/a/b/c/d"(length=5)でdistance=2なら"/a/b/"(length=3)が返る
                return left + string.Join("", segments.Take(segments.Length - distance));
            }
        }

        /// <summary>
        /// authorId != nullのとき、指定されたauthorIdを持つすべてのRefListからの検索になる。
        /// authorId == nullのとき、PublishingStatusKind.PublishなすべてのRefListからの検索になる。
        /// 最大100件までにする。
        /// </summary>
        private async Task<IEnumerable<RefList>> GetRefListsByContainingLinkCore(
            long? authorId, string url, int maxCount = 100
        )
        {
            var urlObj = default(Uri);
            if (!Uri.TryCreate(url, UriKind.Absolute, out urlObj))
            {
                throw new ArgumentException("URLの形式が正しくありません: " + url);
            }

            var includeNotPublish = authorId != null;
            var distance = includeNotPublish ? 2 : 3;

            var baseUrlForQuery = GetBaseUrlForQuery(urlObj, distance);

            /// まずはbaseUrlと前方一致するリンクを持つリストに絞り込む
            /// 前方一致ならLIKEで比較的高速なので
            var query = _refsContext.RefLists.Include("Author").Include("Refs").Include("TagUses.Tag").Include("Statistics").AsNoTracking();
            query = includeNotPublish ?
                query.Where(l =>
                    l.Author.Id == authorId &&
                    l.Refs.Any(r => r.Kind == RefKind.Link && r.Uri.StartsWith(baseUrlForQuery))
                ) :
                query.Where(l =>
                    l.PublishingStatus == PublishingStatusKind.Publish &&
                    l.Refs.Any(r => r.Kind == RefKind.Link && r.Uri.StartsWith(baseUrlForQuery))
                );

            query.Take(maxCount);

            query = query.OrderByDescending(l => l.UpdatedDate);
            var storedRefLists = await query.ToArrayAsync();

            /// distanceを計算
            var baseUrlObj = new Uri(urlObj.GetLeftPart(UriPartial.Path));
            var refListsAndDistance = storedRefLists.Select(l =>
            {
                return new
                {
                    RefList = l,
                    Distance = l.Refs.Min(r => {
                        if (r.Kind != RefKind.Link)
                        {
                            return int.MaxValue; /// Link以外は結果に入らないようにしておく
                        }
                        var refUriObj = default(Uri);
                        if (!Uri.TryCreate(r.Uri, UriKind.Absolute, out refUriObj))
                        {
                            return int.MaxValue; /// おかしなURIは結果に入らないようにしておく
                        }

                        /// /a/b/c, /a/d/e/fの距離はb/cと戻ってd/e/fと進むので5、となるようにする。
                        var dist = 0;
                        var urlSegs = urlObj.Segments;
                        var refUrlSegs = refUriObj.Segments;
                        var minLen = Math.Min(urlSegs.Length, refUrlSegs.Length);
                        for (int i = 0; i < minLen; ++i)
                        {
                            /// 最後に'/'があってもなくても同じとみなす。
                            var seg1 = urlSegs[i];
                            seg1 = seg1[seg1.Length - 1] == '/' ? seg1.Remove(seg1.Length - 1) : seg1;
                            var seg2 = refUrlSegs[i];
                            seg2 = seg2[seg2.Length - 1] == '/' ? seg2.Remove(seg2.Length - 1) : seg2;
                            
                            if (seg1 != seg2)
                            {
                                dist += (minLen - i) * 2;
                                break;
                            }
                        }
                        dist += (urlSegs.Length - minLen) + (refUrlSegs.Length - minLen);

                        /// 完全一致は-1にしておく
                        if (dist == 0)
                        {
                            var strictMatched = (0 == Uri.Compare(
                                refUriObj, baseUrlObj, UriComponents.Path, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase
                            ));
                            if (strictMatched)
                            {
                                return -1;
                            }
                        }
                        return dist;
                    }),
                };
            });

            /// Path部分が一致するリンクを含むリストだけに絞り込む
            var strictMatchRefListsAndDistance = refListsAndDistance.Where(pair => pair.Distance == -1);

            var resultRefLists = default(IEnumerable<RefList>);
            if (includeNotPublish && strictMatchRefListsAndDistance.Count() == 1)
            {
                /// 自分のリストだけが対象でかつ、完全に一致するリストが一つだけあった場合は、それだけを返す
                resultRefLists = strictMatchRefListsAndDistance.Select(pair => pair.RefList);
            }
            else
            {
                /// Path部分がdistanceの範囲内になるリンクを含むリストだけに絞り込む
                var looseMatchRefListsAndDistance = refListsAndDistance.Where(pair => pair.Distance <= distance);
                resultRefLists = looseMatchRefListsAndDistance.OrderBy(pair => pair.Distance).Select(pair => pair.RefList);
            }

            return resultRefLists;
        }

        /// <summary>
        /// refListIdのRefListを返します。
        /// </summary>
        public async Task<RefList> GetRefListAsync(long refListId)
        {
            var query =
                from l in _refsContext.RefLists.
                    Include("Author").Include("Refs").Include("TagUses.Tag").Include("Statistics").AsNoTracking()
                where l.Id == refListId
                select l;

            var storedRefLists = await query.SingleOrDefaultAsync();
            if (storedRefLists != null)
            {
                storedRefLists.Refs = storedRefLists.Refs.OrderBy(r => r.DisplayOrder).ToArray();
            }
            return storedRefLists;
        }

        //public async Task<RefList> GetUnfiledRefListAsync(long authorId)
        //{
        //    var query =
        //        from l in _refsContext.RefLists.Include("Author").Include("Refs").AsNoTracking()
        //        where l.Kind == RefListKind.Unfiled && l.Author.Id == authorId
        //        select l;

        //    var storedRefLists = await query.SingleAsync();
        //    storedRefLists.Refs = storedRefLists.Refs.OrderBy(r => r.DisplayOrder).ToArray();
        //    return storedRefLists;
        //}

        public async Task<SearchRefListsResponse> SearchRefListsAsync(string searchText, int start = 0, int count = 20)
        {
            SystemContract.RequireNotNullOrWhiteSpace(searchText, "searchText");

            var max = 100;
            count = count > max ? max : count;

            var searchResult = _searchEngine.SearchRefList(searchText, max);
            var ids = searchResult.Skip(start).Take(count).ToArray();

            var query = _refsContext.RefLists.Include("Author").Include("TagUses.Tag").Include("Statistics").AsNoTracking().
                Where(l => ids.Contains(l.Id));

            var storedRefLists = await query.ToArrayAsync();

            var ret = new SearchRefListsResponse()
            {
                WholeCount = searchResult.Count(),
                RefLists = storedRefLists,
            };
            return ret;
        }

        /// <summary>
        /// refListを作成します。
        /// </summary>
        public async Task<RefList> CreateRefListAsync(long userId, CreateRefListRequest request)
        {
            /// DisplayOrderの設定
            if (request.Refs != null)
            {
                var i = 0;
                foreach (var r in request.Refs)
                {
                    r.DisplayOrder = i;
                    ++i;
                }
            }

            var user = await _userHandler.GetUserAsync(userId);
            _refsContext.Users.Attach(user);

            var linkCount = request.Refs == null ? 0 : request.Refs.Count(r => r.Kind == RefKind.Link);
            var refList = new RefList()
            {
                Author = user,
                Title = request.Title,
                Comment = request.Comment,
                PublishingStatus = request.PublishingStatus,
                PublishedDate = request.PublishingStatus == PublishingStatusKind.Draft ? (DateTime?)null : DateTime.Now,
                Refs = request.Refs,
                Statistics = new RefListStatistics() 
                { 
                    LinkCount = linkCount,
                },
            };
            _refsContext.RefLists.Add(refList);
            await _refsContext.SaveChangesAsync();

            await SetTagUsesAsync(refList, request.TagUses, false);

            refList.Refs = refList.Refs.OrderBy(r => r.DisplayOrder).ToArray();
            return refList;
        }

        //public async Task<RefList> CreateUnfiledRefListAsync(long userId)
        //{
        //    var user = await _userHandler.GetUserAsync(userId);
        //    _refsContext.Users.Attach(user);

        //    var refList = new RefList()
        //    {
        //        Kind = RefListKind.Unfiled,
        //        Author = user,
        //        Title = "unfiled",
        //        Comment = "",
        //        PublishingStatus = PublishingStatusKind.Private,
        //        Refs = new Ref[0],
        //        Statistics = null,
        //    };
        //    _refsContext.RefLists.Add(refList);
        //    await _refsContext.SaveChangesAsync();

        //    return refList;
        //}

        public async Task RemoveRefListAsync(EntityIdentity refListIdentity)
        {
            var query = _refsContext.RefLists.Include("TagUses").Where(l => l.Id == refListIdentity.Id);
            var storedRefList = await query.SingleAsync();

            BusinessContract.ValidateWritePermission(storedRefList.AuthorId);
            BusinessContract.ValidateRowVersion(storedRefList.RowVersion, refListIdentity.RowVersion);

            storedRefList.TagUses.Clear();
            foreach (var fav in storedRefList.FavoringFavorites.ToArray())
            {
                _refsContext.Favorites.Remove(fav);
            }
            _refsContext.RefListStatistics.Remove(storedRefList.Statistics);
            _refsContext.RefLists.Remove(storedRefList);

            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(refListIdentity.Id);
        }

        public async Task<EntityIdentity> UpdateRefListAsync(UpdateRefListRequest request)
        {
            var query =
                from l in _refsContext.RefLists.Include("TagUses.Tag").Include("TagUses.Statistics")
                where l.Id == request.Id
                select l;

            var storedRefList = await query.SingleOrDefaultAsync();

            BusinessContract.ValidateWritePermission(storedRefList.AuthorId);
            BusinessContract.ValidateRowVersion(storedRefList.RowVersion, request.RowVersion);

            var isPublishingStatusChanged = storedRefList.PublishingStatus != request.PublishingStatus;
            var isPublishedInThisTime =
                storedRefList.PublishingStatus != PublishingStatusKind.Publish &&
                request.PublishingStatus == PublishingStatusKind.Publish;

            storedRefList.Title = request.Title;
            storedRefList.Comment = request.Comment;
            storedRefList.PublishingStatus = request.PublishingStatus;
            if (isPublishedInThisTime)
            {
                storedRefList.PublishedDate = DateTime.Now;
            }
            await SetTagUsesAsync(storedRefList, request.TagUses, isPublishingStatusChanged);

            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(request.Id);

            return new EntityIdentity()
            {
                Id = request.Id,
                RowVersion = storedRefList.RowVersion,
            };
        }
        
        /// <summary>
        /// refListIdのRefListにrefeを追加します。
        /// bookmarkletから呼ばれ、呼出し後はリダイレクトされるのでRowVersionのことは考えなくてよい。
        /// </summary>
        public async Task AddRefWithoutRowVersionAsync(long refListId, Ref refe)
        {
            var query =
                from l in _refsContext.RefLists.Include("Refs")
                where l.Id == refListId
                select l;

            var storedRefList = await query.SingleAsync();

            BusinessContract.ValidateWritePermission(storedRefList.AuthorId);

            refe.DisplayOrder = storedRefList.Refs.Count;
            refe.RefListId = 0;
            refe.RefList = null;
            storedRefList.Refs.Add(refe);

            /// update statistics
            if (refe.Kind == RefKind.Link)
            {
                var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == refListId);
                var storedStatistics = await statisticsQuery.SingleAsync();
                storedStatistics.LinkCount = storedRefList.Refs.Count(r => r.Kind == RefKind.Link);
            }

            _refsContext.MarkAsModified(storedRefList);
            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(refListId);
        }

        public async Task<AddRefResponse> AddRefAsync(EntityIdentity refListIdentity, int refIndex, Ref refe)
        {
            var query =
                from l in _refsContext.RefLists.Include("Refs")
                where l.Id == refListIdentity.Id
                select l;

            var storedRefList = await query.SingleAsync();

            BusinessContract.ValidateWritePermission(storedRefList.AuthorId);
            BusinessContract.ValidateRowVersion(storedRefList.RowVersion, refListIdentity.RowVersion);

            refe.RefListId = 0;
            refe.RefList = null;

            var refs = storedRefList.Refs.OrderBy(r => r.DisplayOrder).ToList();
            refs.Insert(refIndex, refe);
            for (int i = 0; i < refs.Count; ++i)
            {
                refs[i].DisplayOrder = i;
            }

            storedRefList.Refs.Add(refe);

            /// update statistics
            if (refe.Kind == RefKind.Link)
            {
                var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == refListIdentity.Id);
                var storedStatistics = await statisticsQuery.SingleAsync();
                storedStatistics.LinkCount = storedRefList.Refs.Count(r => r.Kind == RefKind.Link);
            }

            _refsContext.MarkAsModified(storedRefList);
            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(refListIdentity.Id);

            var ret = new AddRefResponse()
            {
                RefListIdentity = new EntityIdentity(storedRefList.Id, storedRefList.RowVersion),
                RefId = refe.Id,
            };
            return ret;
        }

        public async Task<EntityIdentity> UpdateRefAsync(EntityIdentity identity, Ref updatedRef)
        {
            var query =
                from r in _refsContext.Refs.Include("RefList")
                where r.Id == updatedRef.Id
                select r;

            var storedRef = await query.SingleAsync();

            if (storedRef.RefListId != identity.Id)
            {
                throw new ArgumentException("リストのIdが正しくありません");
            }
            BusinessContract.ValidateWritePermission(storedRef.RefList.AuthorId);
            BusinessContract.ValidateRowVersion(storedRef.RefList.RowVersion, identity.RowVersion);

            switch (updatedRef.Kind)
            {
                case RefKind.Link:
                    storedRef.Uri = updatedRef.Uri;
                    storedRef.Title = updatedRef.Title;
                    storedRef.Comment = updatedRef.Comment;
                    break;
                case RefKind.Heading:
                    storedRef.Title = updatedRef.Title;
                    break;
                case RefKind.Text:
                    storedRef.Comment = updatedRef.Comment;
                    break;
            }

            if (_refsContext.IsModified(storedRef))
            {
                _refsContext.MarkAsModified(storedRef.RefList);
            }
            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(identity.Id);

            identity.RowVersion = storedRef.RefList.RowVersion;
            return identity;
        }

        public async Task<EntityIdentity> RemoveRefAsync(EntityIdentity refListIdentity, int refIndex)
        {
            var query =
                from l in _refsContext.RefLists.Include("Refs")
                where l.Id == refListIdentity.Id
                select l;

            var storedRefList = await query.SingleOrDefaultAsync();

            BusinessContract.ValidateWritePermission(storedRefList.AuthorId);
            BusinessContract.ValidateRowVersion(storedRefList.RowVersion, refListIdentity.RowVersion);

            var refs = storedRefList.Refs.OrderBy(r => r.DisplayOrder).ToList();
            var removing = refs[refIndex];
            refs.RemoveAt(refIndex);

            for (int i = 0; i < refs.Count; ++i)
            {
                refs[i].DisplayOrder = i;
            }

            _refsContext.Refs.Remove(removing);

            /// update statistics
            if (removing.Kind == RefKind.Link)
            {
                var statisticsQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == refListIdentity.Id);
                var storedStatistics = await statisticsQuery.SingleAsync();
                storedStatistics.LinkCount = storedRefList.Refs.Count(r => r.Kind == RefKind.Link);
            }

            _refsContext.MarkAsModified(storedRefList);
            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(refListIdentity.Id);

            refListIdentity.RowVersion = storedRefList.RowVersion;
            return refListIdentity;
        }

        public async Task<EntityIdentity> MoveRefToAsync(
            EntityIdentity fromListIdentity, EntityIdentity toListIdentity, int fromRefIndex, int toRefIndex = -1
        )
        {
            using (var tran = _refsContext.BeginTransaction())
            {
                try
                {
                    SystemContract.Require(
                        fromRefIndex > -1,
                        () => new ArgumentOutOfRangeException("fromRefIndex: " + fromRefIndex)
                    );

                    var fromQuery = _refsContext.RefLists.Include("Refs").
                        Where(l => l.Id == fromListIdentity.Id);
                    var fromStoredRefList = await fromQuery.SingleAsync();
                    SystemContract.Require(
                        fromRefIndex < fromStoredRefList.Refs.Count,
                        () => new ArgumentOutOfRangeException("fromRefIndex: " + fromRefIndex)
                    );
                    BusinessContract.ValidateWritePermission(fromStoredRefList.AuthorId);
                    BusinessContract.ValidateRowVersion(fromStoredRefList.RowVersion, fromListIdentity.RowVersion);

                    var toQuery = _refsContext.RefLists.Include("Refs").
                        Where(l => l.Id == toListIdentity.Id);
                    var toStoredRefList = await toQuery.SingleAsync();
                    BusinessContract.ValidateWritePermission(toStoredRefList.AuthorId);
                    BusinessContract.ValidateRowVersion(toStoredRefList.RowVersion, toListIdentity.RowVersion);

                    /// fromから削除
                    var fromRefs = fromStoredRefList.Refs.OrderBy(r => r.DisplayOrder).ToList();
                    var moving = fromRefs[fromRefIndex];
                    fromRefs.RemoveAt(fromRefIndex);
                    for (int i = 0; i < fromRefs.Count; ++i)
                    {
                        fromRefs[i].DisplayOrder = i;
                    }
                    fromStoredRefList.Refs.Remove(moving);

                    /// toに追加
                    var toRefs = toStoredRefList.Refs.OrderBy(r => r.DisplayOrder).ToList();
                    if (toRefIndex < 0)
                    {
                        toRefs.Add(moving);
                    }
                    else
                    {
                        toRefs.Insert(toRefIndex, moving);
                    }
                    for (int i = 0; i < toRefs.Count; ++i)
                    {
                        toRefs[i].DisplayOrder = i;
                    }
                    toStoredRefList.Refs.Add(moving);

                    _refsContext.MarkAsModified(fromStoredRefList);
                    await _refsContext.SaveChangesAsync(); // toのUpdatedDateをfromより後にするため早めに書き込む

                    /// update statistics
                    if (moving.Kind == RefKind.Link)
                    {
                        var fromStatQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == fromListIdentity.Id);
                        var storedFromStat = await fromStatQuery.SingleAsync();
                        storedFromStat.LinkCount = fromStoredRefList.Refs.Count(r => r.Kind == RefKind.Link);

                        var toStatQuery = _refsContext.RefListStatistics.Where(s => s.RefListId == toListIdentity.Id);
                        var storedToStat = await toStatQuery.SingleAsync();
                        storedToStat.LinkCount = toStoredRefList.Refs.Count(r => r.Kind == RefKind.Link);
                    }

                    _refsContext.MarkAsModified(toStoredRefList);
                    await _refsContext.SaveChangesAsync(); // toのUpdatedDateをfromより後にするため遅めに書き込む

                    SendRefListUpdated(fromListIdentity.Id);
                    SendRefListUpdated(toListIdentity.Id);

                    fromListIdentity.RowVersion = fromStoredRefList.RowVersion;

                    tran.Commit();
                    return fromListIdentity;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public async Task<EntityIdentity> MoveRefAsync(EntityIdentity identity, int oldIndex, int newIndex)
        {
            if (newIndex == oldIndex)
            {
                return identity;
            }

            var query =
                from l in _refsContext.RefLists.Include("Refs")
                where l.Id == identity.Id
                select l;

            var storedRefList = await query.SingleOrDefaultAsync();

            BusinessContract.ValidateWritePermission(storedRefList.AuthorId);
            BusinessContract.ValidateRowVersion(storedRefList.RowVersion, identity.RowVersion);

            var refs = storedRefList.Refs.OrderBy(r => r.DisplayOrder).ToList();
            var moving = refs[oldIndex];
            refs.RemoveAt(oldIndex);
            refs.Insert(newIndex, moving);
            for (int i = 0; i < refs.Count; ++i)
            {
                refs[i].DisplayOrder = i;
            }

            _refsContext.MarkAsModified(storedRefList);
            await _refsContext.SaveChangesAsync();
            SendRefListUpdated(identity.Id);

            identity.RowVersion = storedRefList.RowVersion;
            return identity;
        }

        public async Task IncrementViewCountAsync(long refListId)
        {
            var query = "UPDATE [Refs].[dbo].[RefListStatistics] SET ViewCount = ViewCount + 1 Where RefListId = {0}";
            await _refsContext.ExecuteSqlCommandAsync(query, refListId);
            await _refsContext.SaveChangesAsync();
        }

        private async Task<EntityIdentity> SetTagUsesAsync(RefList refList, IEnumerable<string> tagNames, bool isPublishingStatusChanged)
        {
            tagNames = tagNames ?? new string[0];
            if (tagNames.Any(n => string.IsNullOrWhiteSpace(n)))
            {
                throw new ArgumentException();
            }

            refList.TagUses = refList.TagUses ?? new List<TagUse>();
            var storedTagNames = refList.TagUses.Select(u => u.Name).ToArray();

            /// どちらにも含まれているタグに対してやることはない
            ///var asIsTagUses = storedTagNames.Intersect(tagNames);

            /// 削除分
            var removedTagUseNames = storedTagNames.Except(tagNames);
            var removedTagUses = removedTagUseNames.Select(n => refList.TagUses.Single(u => u.Name == n)).ToArray();
            foreach (var removed in removedTagUses)
            {
                refList.TagUses.Remove(removed);
            }

            /// 追加分
            var addedTagUseNames = tagNames.Except(storedTagNames);
            var addedTagUses = new List<TagUse>();
            foreach (var name in addedTagUseNames)
            {
                var tagUse = await _tagHandler.EnsureTagUseAsync(refList.AuthorId, name);
                addedTagUses.Add(tagUse);
            }
            /// 下の書き方ではEnsureTagUseAsync()が同時に複数回呼ばれるため?例外が発生する。
            /// ちゃんとawaitで待って同時には一回しか呼ばれないようにする。
            ///var addedTagUses = (await addedTagUseNames.Select(
            ///    async n => await _tagHandler.EnsureTagUseAsync(refList.AuthorId, n)
            ///).WhenAll()).ToArray();

            foreach (var added in addedTagUses)
            {
                refList.TagUses.Add(added);
            }

            /// リストの更新
            if (removedTagUseNames.Any() || addedTagUseNames.Any())
            {
                _refsContext.MarkAsModified(refList);
                SendRefListUpdated(refList.Id);
            }
            await _refsContext.SaveChangesAsync();

            /// update statistics
            {
                if (isPublishingStatusChanged)
                {
                    var asIsTagUseNames = storedTagNames.Intersect(tagNames);
                    var asIsTagUses = asIsTagUseNames.Select(n => refList.TagUses.Single(u => u.Name == n)).ToArray();
                    foreach (var asIs in asIsTagUses)
                    {
                        asIs.Statistics.RefListCount = asIs.RefLists.Count();
                        asIs.Statistics.PublishedRefListCount =
                            asIs.RefLists.Where(l => l.PublishingStatus == PublishingStatusKind.Publish).Count();
                    }
                }

                foreach (var removed in removedTagUses)
                {
                    removed.Statistics.RefListCount = removed.RefLists.Count();
                    removed.Statistics.PublishedRefListCount =
                        removed.RefLists.Where(l => l.PublishingStatus == PublishingStatusKind.Publish).Count();
                }
                foreach (var added in addedTagUses)
                {
                    added.Statistics.RefListCount = added.RefLists.Count();
                    added.Statistics.PublishedRefListCount =
                        added.RefLists.Where(l => l.PublishingStatus == PublishingStatusKind.Publish).Count();
                }
            }
            await _refsContext.SaveChangesAsync();

            return new EntityIdentity()
            {
                Id = refList.Id,
                RowVersion = refList.RowVersion,
            };
        }

        private void SendRefListUpdated(long id)
        {
            _channel.Send(new RefListUpdatedMessage(id));
        }


        /// <summary>
        /// IdによるRefの比較をするComparerです。
        /// </summary>
        //private class RefIdComparer : EqualityComparer<Ref>
        //{
        //    public override bool Equals(Ref x, Ref y)
        //    {
        //        return x.Id != 0 && y.Id != 0 && x.Id == y.Id;
        //    }

        //    public override int GetHashCode(Ref obj)
        //    {
        //        return obj.Id.GetHashCode();
        //    }
        //}

    }
}
