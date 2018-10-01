using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Practices.Unity;
using SECOM.ACS.Services;
using SECOM.ACS.Identity;
using SECOM.ACS.Infrastructure;

namespace SECOM.ACS.Web.Mvc
{
    /// <summary>
    /// Ensure initialize application security context to perform authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class AppSecurityContextAttribute : ActionFilterAttribute
    {
        protected  ISecurityService SecurityService { get; set; }

        public AppSecurityContextAttribute(ISecurityService securityService)
        {
            this.SecurityService = securityService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest()) { return; }
            if (ApplicationContext.SecurityContext.IsExpired)
            {
                var userManager = filterContext.HttpContext.GetOwinContext().Get<ApplicationUserManager>();
                var roleManager = filterContext.HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                var roles = roleManager.Roles.Where(d => d.IsActive).ToList();
                var userRoles = userManager.ToUserRoleMapping();
                var permissions = SecurityService.GetPermissionRecords();
                ApplicationContext.SecurityContext.AddData(roles, userRoles, permissions);
            }
        }
        
    }
}