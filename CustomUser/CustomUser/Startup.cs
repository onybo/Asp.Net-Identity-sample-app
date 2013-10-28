using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CustomUser.Startup))]
namespace CustomUser
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
