using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MongoUser.Startup))]
namespace MongoUser
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
