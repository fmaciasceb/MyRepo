using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyWebRecipe.Startup))]
namespace MyWebRecipe
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
