using CSI.ComponentModel;
using CSI.Web.Mvc;
using SECOM.ACS.Identity;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/authorizes")]
    [RouteCulture]
    public class AuthrozieApiController : AppControllerBase
    {
        protected readonly ISecurityService securityService;

        public AuthrozieApiController(ISecurityService service)
        {
            securityService = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("save")]
        public JsonResult Save(PermissionViewModel viewModel)
        {
            var result = ObjectResult.Succeed();
            if (viewModel.AuthorizeRules.Count == 0)
            {
                // Delete all permission in role
                result = securityService.DeletesPermissionRecord(viewModel.RoleId);
            }
            else
            {
                result = securityService.UpdatePermission(viewModel.ToEntity(User.Identity.Name));
            }

            if (result.IsSucceed)
            {
                // Refresh Permission
                var roles = RoleManager.Roles.Where(t => t.IsActive).ToList();
                var userRoles = UserManager.ToUserRoleMapping();
                var permissions = securityService.GetPermissionRecords();
                ApplicationContext.SecurityContext.AddData(roles, userRoles, permissions);
                return Ok("Permission data was update successfully.");
            }
            return InternalServerError(result.Error);
        }

        [Route("role")]
        public ActionResult GetPermissionsByRole(int role)
        {
            try
            {
                var dataView = securityService.GetPermission(role);
                return JsonNet(dataView.ToViewModel(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("refresh")]
        public JsonResult RefreshSecurityContext()
        {
            var roles = RoleManager.Roles.Where(t=>t.IsActive).ToList();
            var userRoles = UserManager.ToUserRoleMapping();
            var permissions = securityService.GetPermissionRecords();
            ApplicationContext.SecurityContext.AddData(roles, userRoles, permissions);
            return Ok("Refresh user group authorize caching data is completed.");
        }
    }
}