using CSI.Web.Mvc;
using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MappingConfig.Register();

            ModelMetadataProviders.Current = new ConventionalModelMetadataProvider(false, typeof(ModelMetadataResource));
            // Change Default ModelBinder
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableDateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());
            ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
            FlashMessage.Transport = new FlashMessageSessionTransport();

            //ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            //ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());
        }
    }
}
