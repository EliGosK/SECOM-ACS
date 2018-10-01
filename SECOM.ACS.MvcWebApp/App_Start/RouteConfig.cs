using CSI.Web.Mvc;
using CSI.Web.Mvc.Localization;
using SECOM.ACS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SECOM.ACS.MvcWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;

            var supportCultures = ApplicationContext.Setting.Cultures.SupportCultures.Select(c=>c.ToCultureInfo()).ToArray();
            routes.MapLocalizedMvcAttributeRoutes(
                urlPrefix: "{culture}/",
                defaults: new { culture = "en" },
                constraints: new { culture = "[a-zA-Z]{2}" }
            );

            routes.MapLocalizeRoute("Default",
               url: "{culture}/{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               constraints: new { culture = "[a-zA-Z]{2}" },
               cultures: supportCultures
            );

            routes.MapRouteToLocalizeRedirect("RedirectToLocalize",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //  name: "DefaultWithCulture",
            //  url: "{culture}/{controller}/{action}/{id}",
            //  defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //  constraints: new { culture = new CultureConstraint(defaultCulture: "en", pattern: "[a-z]{2}") }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { culture = "en", controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
