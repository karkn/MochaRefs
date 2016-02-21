using AutoMapper;
using Mocha.Refs.Core.Contracts;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Web.Filters;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Web.Models.TagUse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Controllers
{
    public class TagUseController : AbstractController
    {
        private ITagHandler _tagHandler;
        private IRefListHandler _refListHandler;

        public TagUseController(IUserHandler userHandler, ITagHandler tagHandler, IRefListHandler refListHandler)
            : base(userHandler)
        {
            _tagHandler = tagHandler;
            _refListHandler = refListHandler;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Manage()
        {
            var user = GetUser();
            var tagUses = await _tagHandler.GetAllTagUsesAsync(user.Id);
            var vm = new ManagePageViewModel()
            {
                User = Mapper.Map<UserViewModel>(user),
                TagUses = Mapper.Map<ICollection<TagUseViewModel>>(tagUses),
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public async Task<ActionResult> RenameTagUse(long? id, byte[] rowVersion, string oldName, string newName)
        {
            SystemContract.RequireNotNull(id, "id");
            SystemContract.RequireNotNullOrWhiteSpace(oldName, "oldName");
            SystemContract.RequireNotNullOrWhiteSpace(newName, "newName");

            var user = GetUser();
            var request = new RenameTagUseRequest()
            {
                TagUseIdentity = new EntityIdentity(id.Value, rowVersion),
                OwnerId = user.Id,
                OldName = oldName,
                NewName = newName,
            };
            var result = await _tagHandler.RenameTagUseAsync(request);
            var ret = Mapper.Map<TagUseViewModel>(result);

            return JsonNet(ret);
        }

        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public async Task<ActionResult> AddTagUse(string name)
        {
            SystemContract.RequireNotNullOrWhiteSpace(name, "name");

            var user = GetUser();
            var result = await _tagHandler.EnsureTagUseAsync(user.Id, name);

            var tagUses = await _tagHandler.GetAllTagUsesAsync(user.Id);
            var ret = Mapper.Map<ICollection<TagUseViewModel>>(tagUses);
            return JsonNet(ret);
        }

        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public async Task<ActionResult> RemoveTagUse(long? id, byte[] rowVersion)
        {
            SystemContract.RequireNotNull(id, "id");
            SystemContract.RequireNotNull(rowVersion, "rowVersion");

            await _tagHandler.RemoveTagUsesAsync(new EntityIdentity(id.Value, rowVersion));
            return JsonNet(true);
        }
    }
}