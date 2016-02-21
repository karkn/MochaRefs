using AutoMapper;
using Mocha.Common.Exceptions;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Web.Extensions;
using Mocha.Refs.Web.Filters;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Web.Models.Home;
using Mocha.Refs.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Controllers
{
    public class HomeController : AbstractController
    {
        private IRefListHandler _refListHandler;
        private IFavoriteHandler _favoriteHandler;

        public HomeController(
            IUserHandler userHandler, IRefListHandler refListHandler, IFavoriteHandler favoriteHandler
        ): base(userHandler)
        {
            _refListHandler = refListHandler;
            _favoriteHandler = favoriteHandler;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var vm = new IndexPageViewModel();
            if (Request.IsAuthenticated)
            {
                var user = GetUser();
                var cond = new PageCondition(0, WebConsts.RefListsPageSize);
                var result = await _favoriteHandler.GetAllFavoriteRefListsAsync(user.Id, cond, RefListSortKind.PublishedDateDescending);
                Mapper.Map<PagedRefLists, PagedRefListsViewModel>(result, vm);
                return View(vm);
            }
            else
            {
                if (Request.IsGoogleCrawl())
                {
                    var req = new GetRefListsRequest(
                        null, null, null, null, null, null, PublishingStatusConditionKind.PublishOnly, 0, WebConsts.RefListsPageSize,
                        RefListSortKind.PublishedDateDescending
                    );
                    var result = await _refListHandler.GetRefListsAsync(req);
                    vm.RefLists = Mapper.Map<ICollection<RefListViewModel>>(result.RefLists);
                    return View("IndexStatic", vm);
                }
                else
                {
                    return View(vm);
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Narrow(
            int pageIndex,
            int pageSize = WebConsts.RefListsPageSize,
            RefListSortKind sort = RefListSortKind.PublishedDateDescending
        )
        {
            var user = GetUser();
            var cond = new PageCondition(pageIndex - 1, pageSize);
            var result = await _favoriteHandler.GetAllFavoriteRefListsAsync(user.Id, cond, sort);
            var vm = Mapper.Map<PagedRefListsViewModel>(result);
            return JsonNet(vm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

#if DEBUG
        public ActionResult Test()
        {
            return View();
        }
        public ActionResult Exception()
        {
            throw new Exception("エラーが発生しました");
        }
#endif

        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> Search(string searchText)
        {
            var result = await _refListHandler.SearchRefListsAsync(searchText, 0, 20);
            var ret = Mapper.Map<IEnumerable<RefListViewModel>>(result);
            return JsonNet(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Sitemap()
        {
            var req = new GetRefListsRequest(
                null, null, null, null, null, null, PublishingStatusConditionKind.PublishOnly, 0, 500,
                RefListSortKind.UpdatedDateDescending
            );
            var result = await _refListHandler.GetRefListsAsync(req);

            var xml = SitemapUtil.GetXmlString(result.RefLists, Url);
            return Content(xml, "text/xml", Encoding.UTF8);
        }
    }
}