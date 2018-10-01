using CSI.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using SECOM.ACS.MvcWebApp.App_Start;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System.Linq;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            var container = UnityConfig.GetConfiguredContainer();
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            filters.Add(new ApplicationHandleErrorAttribute() { LoggingName = ApplicationLogType.Applicaiton });            
            filters.Add(new AppSecurityContextAttribute(container.Resolve<ISecurityService>()));
        }
    }
}
