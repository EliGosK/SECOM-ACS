using System.Web;
using System.Web.Optimization;

namespace SECOM.ACS.MvcWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/assets/js/jquery.js",
                "~/assets/js/jquery.confirm.min.js",
                "~/assets/js/jquery.scrollUp.min.js",
                "~/assets/js/jquery.easing.js",
                "~/assets/js/jquery.tablehover.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/assets/js/app.ui.js",
                "~/assets/js/fastclick.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app/th").Include(
                "~/assets/js/app.ui.js",
                "~/assets/js/app.messages.th.js",
                "~/assets/js/fastclick.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo-ui").Include(
                "~/assets/libs/kendo/js/kendo.all.min.js",
                "~/assets/libs/kendo/js/kendo.aspnetmvc.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/assets/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/assets/js/bootstrap.js",
                      "~/assets/js/bootstrap-hover-dropdown.js",
                      "~/assets/js/bootstrap-notify.js",
                      "~/assets/js/respond.js"));


            bundles.Add(new StyleBundle("~/assets/libs/kendo/styles/kendo-ui").Include(
              "~/assets/libs/kendo/styles/kendo.common-bootstrap.min.css",
              "~/assets/libs/kendo/styles/kendo.bootstrap.min.css",
              "~/assets/libs/kendo/styles/kendo.bootstrap.mobile.min.css"));


            bundles.Add(new StyleBundle("~/assets/css/app").Include(
                "~/assets/css/bootstrap.css",
                "~/assets/css/bootstrap-dropdownhover.min.css",
                "~/assets/css/bootstrap-formhelpers.min.css",
                "~/assets/css/font-awesome.css",
                "~/assets/css/jquery-confirm.min.css",
                "~/assets/css/animate.min.css",
                "~/assets/css/site.css",
                "~/assets/css/site-vendor.css"));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
