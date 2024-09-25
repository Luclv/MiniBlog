using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiniBlog.Startup))]
namespace MiniBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
