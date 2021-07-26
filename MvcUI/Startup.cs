using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mercoplano.Simplex.Server.MvcUI.Startup))]
namespace Mercoplano.Simplex.Server.MvcUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
