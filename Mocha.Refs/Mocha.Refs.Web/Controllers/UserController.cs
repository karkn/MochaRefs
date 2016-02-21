using AutoMapper;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Contracts;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Web.Filters;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Controllers
{
    public class UserController : AbstractController
    {
        private ITagHandler _tagHandler;
        private IRefListHandler _refListHandler;
        private IFavoriteHandler _favoriteHandler;

        public UserController(
            IUserHandler userHandler, ITagHandler tagHandler, IRefListHandler refListHandler, IFavoriteHandler favoriteHandler
        )
            : base(userHandler)
        {
            _tagHandler = tagHandler;
            _refListHandler = refListHandler;
            _favoriteHandler = favoriteHandler;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var users = await _UserHandler.GetAllUsersAsync();
            var vm = new IndexPageViewModel()
            {
                Users = Mapper.Map<ICollection<UserViewModel>>(users),
            };
            return View(vm);
        }

        /// <summary>
        /// 自分も含めて名前を指定したユーザーのリストを表示。
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> Detail(string userName, string titleSearch = "", string tag = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return RedirectToAction("Index", "User");
            }

            var author = await _UserHandler.GetUserByUserNameAsync(userName);
            BusinessContract.Require(author != null, Errors.UserNotFound, userName);

            /// 非公開設定の確認のため本人でもPublishしか見れないようにする。
            var req = new GetRefListsRequest(
                author.Id, null, null, tag, titleSearch, null, PublishingStatusConditionKind.PublishOnly, 0, WebConsts.RefListsPageSize,
                RefListSortKind.PublishedDateDescending
            );
            var result = await _refListHandler.GetRefListsAsync(req);
            var tagUses = await _tagHandler.GetAllTagUsesAsync(author.Id);

            var vm = new DetailPageViewModel()
            {
                Author = Mapper.Map<UserViewModel>(author),
                OwnedTagUses = Mapper.Map<ICollection<TagUseViewModel>>(tagUses),
                TitleSearch = titleSearch,
                TagUse = tag,
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };
            
            var isFavored = false;
            if (IsAuthenticated())
            {
                var user = GetUser();
                isFavored = await _favoriteHandler.ExistsFavoriteUserAsync(user.Id, author.Id);
            }
            vm.IsFavored = isFavored;

            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> NarrowDetail(
            long? id, string titleSearch = "", string tag = null, int pageIndex = 1,
            RefListSortKind sort = RefListSortKind.PublishedDateDescending
        )
        {
            SystemContract.RequireNotNull(id, "id");

            //var isPublishOnly = !IsCurrentUserId(id.Value); /// Author以外のユーザーの場合、Publishなリストだけを対象にする
            /// 非公開設定の確認のため本人でもPublishしか見れないようにする。
            var req = new GetRefListsRequest(
                id.Value, null, null, tag, titleSearch, null, PublishingStatusConditionKind.PublishOnly,
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

        //[HttpGet]
        //[Authorize]
        //public async Task<ActionResult> Summary()
        //{
        //    var vm = new SummaryPageViewModel();
        //    //var users = await _UserHandler.GetAllUsersAsync();
        //    //var vm = new IndexPageViewModel()
        //    //{
        //    //    Users = Mapper.Map<ICollection<UserViewModel>>(users),
        //    //};
        //    return View(vm);
        //}

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> FavoriteRefList()
        {
            var user = GetUser();
            var result = await _favoriteHandler.GetFavoriteRefListsAsync(
                user.Id,
                new PageCondition(0, WebConsts.RefListsPageSize),
                FavoriteRefListSortKind.FavoriteCreatedDescending
            );
            var vm = new FavoriteRefListPageViewModel()
            {
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                AllRefListCount = result.AllRefListCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };
            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> NarrowFavoriteRefList(
            int pageIndex = 1,
            FavoriteRefListSortKind sort = FavoriteRefListSortKind.FavoriteCreatedDescending
        )
        {
            var user = GetUser();
            var result = await _favoriteHandler.GetFavoriteRefListsAsync(
                user.Id,
                new PageCondition(pageIndex - 1, WebConsts.RefListsPageSize),
                sort
            );
            var vm = new FavoriteRefListPageViewModel()
            {
                PageIndex = result.PageIndex + 1,
                PageCount = result.PageCount,
                AllRefListCount = result.AllRefListCount,
                RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists),
            };
            return JsonNet(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> FavoriteUser()
        {
            var user = GetUser();
            var result = await _favoriteHandler.GetFavoriteUsersAsync(user.Id);
            var vm = new FavoriteUserPageViewModel()
            {
                Users = Mapper.Map<ICollection<UserViewModel>>(result),
            };
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> FavoriteTag()
        {
            var user = GetUser();
            var result = await _favoriteHandler.GetFavoriteTagsAsync(user.Id);
            var vm = new FavoriteTagPageViewModel()
            {
                Tags = Mapper.Map<ICollection<TagViewModel>>(result),
            };
            return View(vm);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> AddFavoriteUser(long userId)
        {
            var owner = GetUser();
            await _favoriteHandler.AddFavoriteUserAsync(owner.Id, userId);
            return JsonNet(true);
        }

        [HttpPost]
        [AjaxOnly]
        [Authorize]
        public async Task<ActionResult> RemoveFavoriteUser(long userId)
        {
            var owner = GetUser();
            await _favoriteHandler.RemoveFavoriteUserAsync(owner.Id, userId);
            return JsonNet(true);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "*")] /// 10分
        public ActionResult ProfileImage(string userName)
        {
            var path = Server.MapPath("~/");
            path = Path.Combine(path, ConfigurationManager.AppSettings["ProfileImagesPath"]);
            path = Path.GetFullPath(path);
            path = Path.Combine(path, userName.Substring(0, 2));
            var imagePath = Path.Combine(path, userName + ".png");
            return File(imagePath, "image/png");
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "*")] /// 10分
        public ActionResult SmallProfileImage(string userName)
        {
            var path = Server.MapPath("~/");
            path = Path.Combine(path, ConfigurationManager.AppSettings["ProfileImagesPath"]);
            path = Path.GetFullPath(path);
            path = Path.Combine(path, userName.Substring(0, 2));
            var imagePath = Path.Combine(path, userName + "_small.png");
            return File(imagePath, "image/png");
        }

    }
}