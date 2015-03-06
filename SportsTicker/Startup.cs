using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SportsTicker.Startup))]
namespace SportsTicker {
	public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			ConfigureTicker(app);
        }
    }
}
