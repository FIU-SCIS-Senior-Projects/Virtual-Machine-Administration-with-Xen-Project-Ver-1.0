using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CTAM_VM_MGT.Startup))]
namespace CTAM_VM_MGT
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
