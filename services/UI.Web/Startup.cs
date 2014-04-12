using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UI.Web.Startup))]
namespace UI.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
