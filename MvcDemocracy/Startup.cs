using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcDemocracy.Startup))]
namespace MvcDemocracy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
