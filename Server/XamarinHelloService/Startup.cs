using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(XamarinAzureMobileAppTestService.Startup))]

namespace XamarinAzureMobileAppTestService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}