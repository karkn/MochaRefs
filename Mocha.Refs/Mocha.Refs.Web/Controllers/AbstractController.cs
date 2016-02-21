using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Web.Extensions;
using Mocha.Refs.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Mocha.Refs.Web.Controllers
{
    public abstract class AbstractController: Controller
    {
        private IUserHandler _userHandler;

        protected IUserHandler _UserHandler
        {
            get { return _userHandler; }
        }

        public AbstractController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public JsonResult JsonNet(object obj)
        {
            return new JsonResult()
            {
                Data = obj.ToJson(),
            };
        }

        public JsonResult JsonNet(object obj, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = obj.ToJson(),
                JsonRequestBehavior = behavior,
            };
        }

        public bool IsAuthenticated()
        {
            return Request.IsAuthenticated;
        }

        public bool IsCurrentUserId(long id)
        {
            var user = GetUser();
            return user == null ? false : user.Id == id;
        }

        public User GetUser()
        {
            if (Request.IsAuthenticated)
            {
                var container = MochaContainer.GetContainer();
                var userContext = container.Resolve<IUserContext>();
                return userContext.GetUser();
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [ChildActionOnly]
        [OutputCache(Duration = 600)] /// 10分
        public ActionResult NewlyArrivedRefLists()
        {
            var refListHandler = MochaContainer.Resolve<IRefListHandler>();
            var req = new GetRefListsRequest(
                null, null, null, null, null, null, PublishingStatusConditionKind.PublishOnly,
                0, 10, RefListSortKind.PublishedDateDescending
            );
            var task = Task.Run(() => refListHandler.GetRefListsAsync(req));
            var result = task.Result;
            var vm = Mapper.Map<IEnumerable<RefListViewModel>>(result.RefLists);
            return View("Action/_NewlyArrivedRefLists", vm);
        }

        [HttpGet]
        [ChildActionOnly]
        [OutputCache(Duration = 600)] /// 10分
        public ActionResult TopViewCountRefLists()
        {
            var from = DateTime.Today;
            from = from.AddMonths(-1);

            var refListHandler = MochaContainer.Resolve<IRefListHandler>();
            var req = new GetRefListsRequest(
                null, null, null, null, null, from, PublishingStatusConditionKind.PublishOnly,
                0, 10, RefListSortKind.ViewCountDescending
            );
            var task = Task.Run(() => refListHandler.GetRefListsAsync(req));
            var result = task.Result;
            var vm = Mapper.Map<IEnumerable<RefListViewModel>>(result.RefLists);
            return View("Action/_NewlyArrivedRefLists", vm);
        }
    }
}