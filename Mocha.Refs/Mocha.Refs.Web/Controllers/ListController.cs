using AutoMapper;
using Microsoft.AspNet.Identity;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Web.Extensions;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Web.Models.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Net.Http;
using Mocha.Refs.Web.Filters;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Core.DataTypes;
using Mocha.Common.Exceptions;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Contracts;
using Mocha.Common.Utils;
using Mocha.Common.Cache;
using Mocha.Refs.Web.Readers;
using Mocha.Refs.Web.Utils;
using Mocha.Refs.Integration.Pocket;

namespace Mocha.Refs.Web.Controllers
{
    public class ListController : AbstractController
    {
        private static RefListViewModel[] EmptyRefListViewModels = new RefListViewModel[0];

        private ITagHandler _tagHandler;
        private IFavoriteHandler _favoriteHandler;
        private IRefListHandler _refListHandler;
        private RefListViewModelReader _refListViewModelReader;

        public ListController(
            IUserHandler userHandler, ITagHandler tagHandler, IRefListHandler refListHandler, IFavoriteHandler favoriteHandler,
            RefListViewModelReader refListViewModelReader
        )
            : base(userHandler)
        {
            _tagHandler = tagHandler;
            _refListHandler = refListHandler;
            _favoriteHandler = favoriteHandler;
            _refListViewModelReader = refListViewModelReader;
        }

        /// <summary>
        /// すべてのリストを一覧表示。
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> Index(string userName = "", string titleSearch = "", string tag = "")
        {
            var req = new GetRefListsRequest(
                null, userName, null, tag, titleSearch, null, PublishingStatusConditionKind.PublishOnly,
                0, WebConsts.RefListsPageSize,
                RefListSortKind.PublishedDateDescending
            );
            var result = await _refListHandler.GetRefListsAsync(req);

            var vm = new IndexPageViewModel()
            {
                Tag = tag,
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };
            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> NarrowIndex(
            long? authorId, string titleSearch = "", string tag = "", int pageIndex = 1,
            RefListSortKind sort = RefListSortKind.PublishedDateDescending
        )
        {
            var req = new GetRefListsRequest(
                authorId, null, null, tag, titleSearch, null, PublishingStatusConditionKind.PublishOnly,
                pageIndex - 1, WebConsts.RefListsPageSize,
                sort
            );
            var result = await _refListHandler.GetRefListsAsync(req);

            var vm = new IndexPageViewModel()
            {
                Tag = tag,
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };

            return JsonNet(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 自分のリストを管理用に一覧表示。
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Manage(string titleSearch = "", string tag = null)
        {
            var user = GetUser();
            var vm = await PrepareManagePageViewModel(
                user.Id, null, titleSearch, tag, PublishingStatusConditionKind.All, 0, WebConsts.RefListsPageSize,
                RefListSortKind.UpdatedDateDescending
            );

            vm.Author = Mapper.Map<UserViewModel>(user);

            var tagUses = await _tagHandler.GetAllTagUsesAsync(user.Id);
            vm.OwnedTagUses = tagUses.Select(u => u.Name);

            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> NarrowManage(
            string titleSearch = "", string tag = null,
            PublishingStatusConditionKind publishingStatusCondition = PublishingStatusConditionKind.All,
            int pageIndex = 1,
            RefListSortKind sort = RefListSortKind.UpdatedDateDescending
        )
        {
            var user = GetUser();
            var vm = await PrepareManagePageViewModel(
                user.Id, null, titleSearch, tag, publishingStatusCondition,
                pageIndex - 1, WebConsts.RefListsPageSize,
                sort
            );
            return JsonNet(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Search(string q = "")
        {
            var vm = new SearchPageViewModel()
            {
                TextSearch = q,
                PageIndex = 1,
            };

            if (!string.IsNullOrWhiteSpace(q))
            {
                if (q.StartsWith("http://") || q.StartsWith("https://"))
                {
                    var url = Uri.EscapeUriString(q);
                    return RedirectToAction("FindByBookmarklet", new { url = url, title = q, });
                }
                else
                {
                    var result = await _refListHandler.SearchRefListsAsync(q, 0, 20);
                    vm.PageCount = IndexUtil.GetPageCount(result.WholeCount, 20);
                    vm.RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists);
                }
            } else {
                vm.PageCount = 1;
                vm.RefLists = EmptyRefListViewModels;
            }

            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> SearchRefList(string q, int pageIndex = 1)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                var vm = new SearchPageViewModel()
                {
                    TextSearch = q,
                    PageIndex = pageIndex,
                    PageCount = 1,
                    RefLists = EmptyRefListViewModels,
                };
                return JsonNet(vm, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var pageSize = 20;
                var start = (pageIndex - 1) * pageSize;

                var result = await _refListHandler.SearchRefListsAsync(q, start, 20);
                var vm = new SearchPageViewModel()
                {
                    TextSearch = q,
                    PageIndex = pageIndex,
                    PageCount = IndexUtil.GetPageCount(result.WholeCount, pageSize),
                    RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
                };
                return JsonNet(vm, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Detail(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "List");
            }

            var listViewModel = await _refListViewModelReader.ReadRefListViewModel(id.Value);
            if (listViewModel == null)
            {
                return HttpNotFound();
            }

            var canBrowse = listViewModel.PublishingStatus == PublishingStatusKind.Publish; /// 非公開設定の確認のため本人も見れないようにする。
            if (canBrowse)
            {
                var refViewModels = await _refListViewModelReader.ReadRefViewModels(id.Value);
                var isFavored = false;
                if (IsAuthenticated())
                {
                    var user = GetUser();
                    isFavored = await _favoriteHandler.ExistsFavoriteRefListAsync(user.Id, listViewModel.Id);
                }
                var vm = new DetailPageViewModel()
                {
                    IsEdit = false,
                    CanEdit = IsCurrentUserId(listViewModel.Author.Id),
                    IsFavored = isFavored,
                    RefList = listViewModel,
                    Refs = refViewModels,
                };

                await SetCookieAndIncrementViewCount(id.Value);
                return Request.IsGoogleCrawl() ? View("DetailStatic", vm) : View(vm);
            }
            else
            {
                return View("DetailNotBrowsible");
            }
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> GetRefs(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var listViewModel = await _refListViewModelReader.ReadRefListViewModel(id.Value);
            if (listViewModel == null)
            {
                return HttpNotFound();
            }

            var canBrowse = listViewModel.PublishingStatus == PublishingStatusKind.Publish; /// 非公開設定の確認のため本人も見れないようにする。
            if (canBrowse)
            {
                var refViewModels = await _refListViewModelReader.ReadRefViewModels(id.Value);
                await SetCookieAndIncrementViewCount(id.Value);
                return JsonNet(refViewModels, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpGet]
        public ActionResult DetailPart(long? id)
        {
            return PartialView("Detail/_ListDetail", new { Refs = string.Format("Refs['Id_{0}']", id) }.ToExpando());
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> OpenByBookmarklet(string url, string title)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return HttpNotFound();
            }

            var user = GetUser();
            var result = default(IEnumerable<RefList>);
            try
            {
                result = await _refListHandler.GetRefListsByContainingLink(user.Id, url);
            }
            catch (ArgumentException e)
            {
                throw new BusinessException(e, Errors.InvalidUrlFormat);
            }

            if (result.Count() == 1)
            {
                var found = result.First();
                return RedirectToAction("Edit", new { id = found.Id });
            }
            else
            {
                var vm = new OpenByBookmarkletPageViewModel()
                {
                    IsOwnOnly = true,
                    Uri = url,
                    Title = title,
                    RefLists = Mapper.Map<ICollection<RefListViewModel>>(result),
                };
                return View("OpenByBookmarklet", vm);
            }
        }

        [HttpGet]
        public async Task<ActionResult> FindByBookmarklet(string url, string title)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return HttpNotFound();
            }

            var result = default(IEnumerable<RefList>);
            try
            {
                result = await _refListHandler.GetAllRefListsByContainingLink(url);
            }
            catch (ArgumentException e)
            {
                throw new BusinessException(e, Errors.InvalidUrlFormat);
            }

            var vm = new OpenByBookmarkletPageViewModel()
            {
                IsOwnOnly = false,
                Uri = url,
                Title = title,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result),
            };
            return View("OpenByBookmarklet", vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUser();
            var listViewModel = await _refListViewModelReader.ReadRefListViewModel(id.Value);
            if (listViewModel == null)
            {
                return HttpNotFound();
            }

            var canBrowse = IsCurrentUserId(listViewModel.Author.Id) || listViewModel.PublishingStatus == PublishingStatusKind.Publish;
            if (canBrowse)
            {
                var tagUses = await _tagHandler.GetAllTagUsesAsync(user.Id);
                var ownedTagUses = tagUses.Select(u => u.Name);

                var isFavored = false;
                if (IsAuthenticated())
                {
                    isFavored = await _favoriteHandler.ExistsFavoriteRefListAsync(user.Id, listViewModel.Id);
                }

                var isEdit = IsCurrentUserId(listViewModel.Author.Id);
                var refViewModels = await _refListViewModelReader.ReadRefViewModels(id.Value);
                var vm = new DetailPageViewModel()
                {
                    IsEdit = isEdit,
                    CanEdit = isEdit, /// IsEdit==trueならCanEditの値は使われない
                    IsFavored = isFavored,
                    RefList = listViewModel,
                    Refs = refViewModels,
                    OwnedTagUses = ownedTagUses,
                };
                
                await SetCookieAndIncrementViewCount(id.Value);
                return View("Detail", vm);
            }
            else
            {
                return View("DetailNotBrowsible");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var refe = (RefViewModel)TempData["_RefViewModelForCreate"];

            var vm = new CreatePageViewModel();
            var tagUses = await _tagHandler.GetAllTagUsesAsync(GetUser().Id);
            vm.OwnedTagUses = tagUses.Select(u => u.Name);
            vm.Refs = refe == null ? new RefViewModel[0] : new[] { refe };
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreatePageViewModel vm)
        {
            var user = GetUser();
            var req = new CreateRefListRequest()
            {
                PublishingStatus = PublishingStatusKind.Draft,
            };
            Mapper.Map<CreatePageViewModel, CreateRefListRequest>(vm, req);

            var result = await _refListHandler.CreateRefListAsync(user.Id, req);

            return RedirectToAction("Edit", new { id = result.Id });
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> AddByBookmarklet(string url)
        {
            /// bindするとvalidateで「<」とか「>」とかがだめになる
            var title = Request.Unvalidated.QueryString["title"];

            SystemContract.RequireNotNullOrWhiteSpace(url, "url");
            BusinessContract.Require(url.Length <= WebConsts.MaxRefLinkUrlLength, Errors.UrlTooLong);

            if (!string.IsNullOrWhiteSpace(title) && title.Length > WebConsts.MaxRefLinkTitleLength)
            {
                title = title.Substring(0, WebConsts.MaxRefLinkTitleLength);
            }

            var model = new AddByBookmarkletPageViewModel()
            {
                Uri = url,
                Title = title,
            };

            var user = GetUser();
            var req = new GetRefListsRequest(
                user.Id, null, null, null, "", null, PublishingStatusConditionKind.All, 0, 500, RefListSortKind.UpdatedDateDescending
            );
            var resp = await _refListHandler.GetRefListsAsync(req);
            model.RefLists = Mapper.Map<ICollection<RefListViewModel>>(resp.RefLists);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddByBookmarklet(AddByBookmarkletPageInputModel model)
        {
            if (model.RefListId == 0)
            {
                var refe = new RefViewModel()
                {
                    Kind = RefKind.Link,
                    Uri = model.Uri,
                    Title = model.Title,
                    Comment = model.Comment,
                };
                TempData["_RefViewModelForCreate"] = refe;
                return RedirectToAction("Create");
            }
            else
            {
                /// 既存のリストに追加
                var refe = new Ref()
                {
                    Kind = RefKind.Link,
                    Uri = model.Uri,
                    Title = model.Title,
                    Comment = model.Comment,
                };
                await _refListHandler.AddRefWithoutRowVersionAsync(model.RefListId, refe);
                return RedirectToAction("Edit", new { id = model.RefListId });
                //return Redirect(model.Uri);
            }
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> MoveRef(EntityIdentity info, int oldIndex, int newIndex)
        {
            var result = await _refListHandler.MoveRefAsync(info, oldIndex, newIndex);
            return JsonNet(result);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> MoveRefTo(
            EntityIdentity sourceRefListIdentity, EntityIdentity targetRefListIdentity, int sourceRefIndex
        )
        {
            var result = await _refListHandler.MoveRefToAsync(sourceRefListIdentity, targetRefListIdentity, sourceRefIndex);
            return JsonNet(result);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> EditListHeader(UpdateRefListRequest req)
        {
            var result = await _refListHandler.UpdateRefListAsync(req);
            var vm = new EntityIdentity()
            {
                Id = result.Id,
                RowVersion = result.RowVersion,
            };

            return JsonNet(vm);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> Remove(EntityIdentity refListIdentity)
        {
            await _refListHandler.RemoveRefListAsync(refListIdentity);
            return JsonNet(refListIdentity);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> AddRef(EntityIdentity info, RefViewModel refe, int refIndex)
        {
            var refeDto = Mapper.Map<Ref>(refe);
            var result = await _refListHandler.AddRefAsync(info, refIndex, refeDto);

            return JsonNet(result);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> EditRef(EntityIdentity info, RefViewModel refe)
        {
            var refeDto = Mapper.Map<Ref>(refe);
            var result = await _refListHandler.UpdateRefAsync(info, refeDto);

            return JsonNet(result);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> RemoveRef(EntityIdentity info, int refIndex)
        {
            var result = await _refListHandler.RemoveRefAsync(info, refIndex);
            return JsonNet(result);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> AddFavoriteRefList(long refListId)
        {
            var user = GetUser();
            await _favoriteHandler.AddFavoriteRefListAsync(user.Id, refListId);
            return JsonNet(true);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> RemoveFavoriteRefList(long refListId)
        {
            var user = GetUser();
            await _favoriteHandler.RemoveFavoriteRefListAsync(user.Id, refListId);
            return JsonNet(true);
        }

        [HttpGet]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> GetRefLists()
        {
            var user = GetUser();
            var req = new GetRefListsRequest(
                user.Id, null, null, null, null, null, PublishingStatusConditionKind.All,
                0, 500, RefListSortKind.UpdatedDateDescending
            );
            var result = await _refListHandler.GetRefListsAsync(req);
            var lists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists);
            return JsonNet(lists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> AuthPocket()
        {
            var pocket = new PocketAccount();
            pocket.CallbackUri = Url.AbsoluteUrl("~/List/AuthPocket");
            var reqCode = await pocket.GetRequestCode();

            var callbackUri = Url.AbsoluteUrl("~/List/AuthPocketResult?reqCode=") + reqCode;
            var uri = pocket.Auth(callbackUri);
            return Redirect(uri.ToString());
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> AuthPocketResult(string reqCode)
        {
            var pocket = new PocketAccount();
            var pocketUser = await pocket.GetUser(reqCode);
            if (pocketUser == null)
            {
                ViewBag.Message = "認証に失敗しました。";
                return View();
            }
            else
            {
                var user = GetUser();
                await _UserHandler.AddUserDataAsync(user.Id, CoreConsts.PocketAccessCodeUserDataKey, pocketUser.AccessCode);
                ViewBag.Message = "認証に成功しました。";
                return View();
            }
        }

        [HttpGet]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> GetPocketItems()
        {
            var user = GetUser();
            var pocketAccessCode = await _UserHandler.GetUserDataAsync(user.Id, CoreConsts.PocketAccessCodeUserDataKey);

            if (pocketAccessCode != null)
            {
                try
                {
                    var pocket = new PocketAccount(pocketAccessCode.Value);
                    var items = await pocket.GetItems();
                    return JsonNet(items, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return JsonNet(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return JsonNet(false, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<ManagePageViewModel> PrepareManagePageViewModel(
            long? authorId, string userName, string titleSearch, string tag, PublishingStatusConditionKind publishingStatusCondition,
            int pageIndex, int pageSize, RefListSortKind sort
        )
        {
            var req = new GetRefListsRequest(
                authorId, userName, null, tag, titleSearch, null, publishingStatusCondition, pageIndex, pageSize, sort
            );

            var resp = await _refListHandler.GetRefListsAsync(req);

            var vm = Mapper.Map<ManagePageViewModel>(resp);
            ++vm.PageIndex;
            vm.TitleSearch = titleSearch;
            vm.TagUse = tag;

            return vm;
        }

        private async Task SetCookieAndIncrementViewCount(long refListId)
        {
            if (Request.Cookies[WebConsts.ViewdPageCookieName] == null)
            {
                var cookie = new HttpCookie(WebConsts.ViewdPageCookieName);
                await SetCookieAndIncrementViewCountCore(cookie, refListId);
            }
            else
            {
                if (Request.Cookies[WebConsts.ViewdPageCookieName][string.Format(WebConsts.ViewdPageCookieIdKeyFormat, refListId)] == null)
                {
                    var cookie = (HttpCookie)Request.Cookies[WebConsts.ViewdPageCookieName];
                    await SetCookieAndIncrementViewCountCore(cookie, refListId);
                }
            }
        }

        private async Task SetCookieAndIncrementViewCountCore(HttpCookie cookie, long refListId)
        {
            cookie[string.Format(WebConsts.ViewdPageCookieIdKeyFormat, refListId)] = "1";
            cookie.Expires = DateTime.Now.AddDays(1);
            cookie.Path = "/";
            Response.Cookies.Add(cookie);
            await _refListHandler.IncrementViewCountAsync(refListId);
        }
    }
}