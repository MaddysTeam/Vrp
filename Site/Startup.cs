using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Res.Startup))]
namespace Res
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
