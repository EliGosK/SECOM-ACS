using MvcSiteMapProvider.Web.Html.Models;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class PermissionExtension
    {
        public static bool HasPermission(this ControllerBase controller, string objectId ,string permission)
        {
            return ApplicationContext.SecurityContext.IsUserAuthorize(controller.ControllerContext.HttpContext.User.Identity.Name, objectId, permission);            
        }

        public static bool HasPermission(this HttpContextBase httpContext, string objectId, string permission)
        {
            return ApplicationContext.SecurityContext.IsUserAuthorize(httpContext.User.Identity.Name, objectId, permission);
        }

        public static bool CheckChildMenuPermission(this ControllerBase controller, SiteMapNodeModelList nodeList)
        {
            foreach (SiteMapNodeModel node in nodeList)
            {
                if (node.Attributes.Where(d=> d.Key ==  "objectId").Count() > 0)
                {
                    var objectId = (node.Attributes["objectId"]??"").ToString();
                    var permission = node.Attributes.ContainsKey("permission")?  (string)node.Attributes["permission"] : PermissionNames.View;
                    if (ApplicationContext.SecurityContext.IsUserAuthorize(controller.ControllerContext.HttpContext.User.Identity.Name, objectId, permission))
                    {
                        return true;
                    }
                }
                    
            }
            return false;
        }
    }
}