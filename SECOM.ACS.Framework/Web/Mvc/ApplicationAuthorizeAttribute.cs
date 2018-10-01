using Microsoft.AspNet.Identity.Owin;
using Microsoft.Practices.Unity;
using SECOM.ACS.Identity;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Services;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SECOM.ACS.Properties;

namespace SECOM.ACS.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        [Dependency]
        public ISecurityService SecurityService { get; set; }

        public ApplicationAuthorizeAttribute()
        {

        }

        public ApplicationAuthorizeAttribute(string objectId,string permissionName)
        {
            this.ObjectId = objectId;
            this.PermissionName = permissionName;
        }

        public string ObjectId { get; private set; }
        public string PermissionName { get; set; }
        
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                if (string.IsNullOrEmpty(this.ObjectId) && string.IsNullOrEmpty(this.PermissionName))
                {
                    return true;
                }

                if (ApplicationContext.SecurityContext.IsExpired)
                {
                    var userManager = httpContext.GetOwinContext().Get<ApplicationUserManager>();
                    var roleManager = httpContext.GetOwinContext().Get<ApplicationRoleManager>();
                    var roles = roleManager.Roles.Where(d => d.IsActive).ToList();
                    var userRoles = userManager.ToUserRoleMapping();
                    var permissions = SecurityService.GetPermissionRecords();
                    ApplicationContext.SecurityContext.AddData(roles, userRoles, permissions);
                }
                return ApplicationContext.SecurityContext.IsUserAuthorize(httpContext.User.Identity.Name, this.ObjectId, this.PermissionName);
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    filterContext.Result = new JsonResult() { Data = new { message = Resources.RequestDenied }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", statusCode = (int)HttpStatusCode.Unauthorized }));
                }
            }
            else {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    filterContext.Result = new JsonResult() { Data = new { message = Resources.SessionTimeout }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }
    }
}
