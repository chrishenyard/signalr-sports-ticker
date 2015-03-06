using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SportsTicker {
	public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
