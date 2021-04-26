using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SecurityAssetManager.Startup))]
namespace SecurityAssetManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
