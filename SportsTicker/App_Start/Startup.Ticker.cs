using Microsoft.AspNet.SignalR;
using Owin;

namespace SportsTicker {
	public partial class Startup {
		public void ConfigureTicker(IAppBuilder app) {
			var hubConfiguration = new HubConfiguration {
				EnableDetailedErrors = true
			};

			app.MapSignalR(hubConfiguration);
		}
	}
}