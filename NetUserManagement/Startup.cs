using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetUserManagement.Startup))]
namespace NetUserManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
