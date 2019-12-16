using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ELearning.Startup))]
namespace ELearning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
