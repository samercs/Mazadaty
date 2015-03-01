using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mzayad.Web.Startup))]
namespace Mzayad.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.MapSignalR();
        }
    }
}
