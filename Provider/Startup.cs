using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Provider.Startup))]
namespace Provider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}