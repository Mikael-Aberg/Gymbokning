using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gymbokning.Startup))]
namespace Gymbokning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
