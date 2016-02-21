using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mocha.Refs.Web.Startup))]
namespace Mocha.Refs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
