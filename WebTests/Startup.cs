using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SchoolSoul.Web.Startup))]
namespace SchoolSoul.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
