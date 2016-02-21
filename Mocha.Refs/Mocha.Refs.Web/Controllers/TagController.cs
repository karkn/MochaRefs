using AutoMapper;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Contracts;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Web.Filters;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Web.Models.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Controllers
{
    public class TagController : AbstractController
    {
        private ITagHandler _tagHandler;
        private IFavoriteHandler _favoriteHandler;
        private IRefListHandler _refListHandler;

        public TagController(
            IUserHandler userHandler, ITagHandler tagHandler, IFavoriteHandler favoriteHandler, IRefListHandler refListHandler
        )
            : base(userHandler)
        {
            _tagHandler = tagHandler;
            _favoriteHandler = favoriteHandler;
            _refListHandler = refListHandler;
        }

        [HttpGet]
        [OutputCache(Duration = 600)] /// 10分
        public async Task<ActionResult> Index()
        {
            var tags = await _tagHandler.GetAllTagsAsync(TagSortKind.RefListCountDescending);
            var vm = new IndexPageViewModel()
            {
                Tags = Mapper.Map<ICollection<TagViewModel>>(tags),
            };
            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> NarrowIndex(TagSortKind sort = TagSortKind.RefListCountDescending)
        {
            var tags = await _tagHandler.GetAllTagsAsync(sort);
            var vm = new IndexPageViewModel()
            {
                Tags = Mapper.Map<ICollection<TagViewModel>>(tags),
            };
            return JsonNet(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(string tag, string titleSearch = "")
        {
            if (string.IsNullOrWhiteSpace(tag) || tag == CoreConsts.UnsetTagName)
            {
                return RedirectToAction("Index");
            }

            var tagEntity = await _tagHandler.GetTagAsync(tag);
            if (tagEntity == null)
            {
                return HttpNotFound();
            }
            var tagVm = Mapper.Map<TagViewModel>(tagEntity);

            /// 非公開設定の確認のため本人でもPublishしか見れないようにする。
            var req = new GetRefListsRequest(
                null, null, null, tag, titleSearch, null, PublishingStatusConditionKind.PublishOnly, 0, WebConsts.RefListsPageSize,
                RefListSortKind.PublishedDateDescending
            );
            var result = await _refListHandler.GetRefListsAsync(req);

            var vm = new DetailPageViewModel()
            {
                Tag = tagVm,
                TitleSearch = titleSearch,
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };

            var isFavored = false;
            if (IsAuthenticated())
            {
                var user = GetUser();
                isFavored = await _favoriteHandler.ExistsFavoriteTagAsync(user.Id, tagVm.Id);
            }
            vm.IsFavored = isFavored;

            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> NarrowDetail(
            string tag, string titleSearch = "", int pageIndex = 1, RefListSortKind sort = RefListSortKind.PublishedDateDescending
        )
        {
            SystemContract.RequireNotNull(tag, "tag");
            SystemContract.Require(tag != CoreConsts.UnsetTagName, "$unsetが指定されました");

            /// 非公開設定の確認のため本人でもPublishしか見れないようにする。
            var req = new GetRefListsRequest(
                null, null, null, tag, titleSearch, null, PublishingStatusConditionKind.PublishOnly,
                pageIndex - 1, WebConsts.RefListsPageSize, sort
            );
            var result = await _refListHandler.GetRefListsAsync(req);

            var vm = new PagedRefListsViewModel()
            {
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };

            return JsonNet(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> AddFavoriteTag(long tagId)
        {
            var owner = GetUser();
            await _favoriteHandler.AddFavoriteTagAsync(owner.Id, tagId);
            return JsonNet(true);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> RemoveFavoriteTag(long tagId)
        {
            var owner = GetUser();
            await _favoriteHandler.RemoveFavoriteTagAsync(owner.Id, tagId);
            return JsonNet(true);
        }
    }
}