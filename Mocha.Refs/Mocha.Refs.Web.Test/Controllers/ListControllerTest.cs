using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Mocha.Refs.Core.Handlers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Mocha.Refs.Core.Transfer;
using Mocha.Common.Unity;
using Mocha.Refs.Core;
using Microsoft.Practices.Unity;
using Mocha.Refs.Web.Models.List;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Testing.Mock;

namespace Mocha.Refs.Web.Controllers
{
    [TestClass]
    public class ListControllerTest
    {
        [TestMethod]
        public async Task TestAddByBookmarklet_Get()
        {
            // arrange
            var title = "タイトル1";
            var url = "http://www.mochaware.jp/";
            var pagedRefLists = ObjectMother.GetPagedRefLists();
            var queryStrs = new NameValueCollection() {
                {"title", title},
            };

            MapConfig.CreateMaps();
            MochaContainer.RegisterTypes = ObjectMother.RegisterAuthenticatedUserContext;
            MochaContainer.GetContainer();

            var mocks = new ControllerContextMocks();
            mocks.UnvalidatedRequestValues.Setup(u => u.QueryString).Returns(queryStrs);
            mocks.Request.Setup(r => r.IsAuthenticated).Returns(true);

            var handlerMock = new Mock<IRefListHandler>();
            handlerMock.Setup(h => h.GetRefListsAsync(It.IsAny<GetRefListsRequest>())).ReturnsAsync(pagedRefLists);

            var controller = new ListController(null, null, handlerMock.Object, null, null);
            controller.ControllerContext = mocks.CreateControllerContext(controller);

            // act
            var result = await controller.AddByBookmarklet(url) as ViewResult;

            // assert
            var model = result.Model as AddByBookmarkletPageViewModel;
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(
                pagedRefLists.RefLists.FirstOrDefault().Title,
                model.RefLists.FirstOrDefault().Title
            );
        }

        [TestMethod]
        public async Task TestAddByBookmarklet_Post_IdZero()
        {
            // arrange
            var input = new AddByBookmarkletPageInputModel()
            {
                RefListId = 0
            };

            // act
            var controller = new ListController(null, null, null, null, null);
            var result = await controller.AddByBookmarklet(input) as RedirectToRouteResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Create", result.RouteValues["Action"]);
            Assert.IsNotNull(controller.TempData["_RefViewModelForCreate"]);
        }

        [TestMethod]
        public async Task TestAddByBookmarklet_Post_IdNotZero()
        {
            // arrange
            var input = new AddByBookmarkletPageInputModel()
            {
                RefListId = 1,
                Title = ObjectMother.GetRefListTitle(),
                Uri = ObjectMother.GetRefListUri(),
                Comment = ObjectMother.GetRefListComment(),
            };

            var handlerMock = new Mock<IRefListHandler>();
            handlerMock.Setup(h => h.AddRefWithoutRowVersionAsync(It.IsAny<long>(), It.IsAny<Ref>())).Returns(Task.FromResult(0));

            // act
            var controller = new ListController(null, null, handlerMock.Object, null, null);
            var result = await controller.AddByBookmarklet(input) as RedirectToRouteResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.RouteValues["Action"]);
            Assert.AreEqual(1L, result.RouteValues["id"]);
            handlerMock.Verify(h => h.AddRefWithoutRowVersionAsync(It.IsAny<long>(), It.IsAny<Ref>()), Times.Once());
        }
    }
}
