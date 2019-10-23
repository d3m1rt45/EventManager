using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KonneyTM.Startup))]
namespace KonneyTM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
