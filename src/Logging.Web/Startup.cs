using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Takenet.Library.Logging.Web.Startup))]
namespace Takenet.Library.Logging.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
