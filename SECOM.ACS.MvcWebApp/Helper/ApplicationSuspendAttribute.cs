using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Helper;
using SECOM.ACS.MvcWebApp.Models;
using System;
using System.IO;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace SECOM.ACS.MvcWebApp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited=true, AllowMultiple=false)]
    public class ApplicationSuspendAttribute : ActionFilterAttribute
    {
        private readonly string appSuspendFile;
       
        public ApplicationSuspendAttribute() 
        {
            this.appSuspendFile = GetDefaultAppSuspendFilePath();
        }

        public ApplicationSuspendAttribute(string file)
        {
            this.appSuspendFile = file;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var file = this.appSuspendFile;
            if (file.StartsWith("~"))
                file = HostingEnvironment.MapPath(file);
           
            if (File.Exists(file))
            {
                var data = OfflineSystemFileManager.LoadFromFile(file) ?? OfflineOnlineSystemData.Default();
                data.Calculate();
                var isSuspend = data.IsOfflineSystem;
                if (isSuspend)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        filterContext.Result = new JsonResult() { Data = new { message = MessageHelper.SystemSuspend() }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var routeValues = new RouteValueDictionary() { { "controller", "SystemStatus" }, { "action", "Offline" } };
                        filterContext.Result = new RedirectToRouteResult(routeValues);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        private string GetDefaultAppSuspendFilePath()
        {
            return HostingEnvironment.MapPath("~/app_suspend.json");
        }
    }
}
