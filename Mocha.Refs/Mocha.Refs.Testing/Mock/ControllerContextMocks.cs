using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mocha.Refs.Testing.Mock
{
    public class ControllerContextMocks
    {
        public Mock<HttpContextBase> HttpContext { get; private set; }
        public Mock<HttpRequestBase> Request { get; private set; }
        public Mock<HttpResponseBase> Response { get; private set; }
        public Mock<HttpSessionStateBase> SessionState { get; private set; }
        public Mock<HttpServerUtilityBase> ServerUtility { get; private set; }

        public Mock<UnvalidatedRequestValuesBase> UnvalidatedRequestValues { get; private set; }

        public RouteData RouteData { get; private set; }

        public ControllerContextMocks()
        {
            HttpContext = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            SessionState = new Mock<HttpSessionStateBase>();
            ServerUtility = new Mock<HttpServerUtilityBase>();

            HttpContext.Setup(c => c.Request).Returns(Request.Object);
            HttpContext.Setup(c => c.Response).Returns(Response.Object);
            HttpContext.Setup(c => c.Session).Returns(SessionState.Object);
            HttpContext.Setup(c => c.Server).Returns(ServerUtility.Object);

            UnvalidatedRequestValues = new Mock<UnvalidatedRequestValuesBase>();
            Request.Setup(c => c.Unvalidated).Returns(UnvalidatedRequestValues.Object);

            RouteData = new RouteData();
        }

        public ControllerContext CreateControllerContext(ControllerBase controller)
        {
            return new ControllerContext(HttpContext.Object, RouteData, controller);
        }
    }
}
