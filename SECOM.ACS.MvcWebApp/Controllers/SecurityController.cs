using CSI.Core;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using SECOM.ACS.Identity;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Web.Mvc;
using CSI.Web.Mvc.KendoUI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.Services;
using System.Threading.Tasks;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class SecurityController : AppControllerBase
    {
        protected readonly ISecurityService service;

        public SecurityController(ISecurityService service)
        {
            this.service = service;
        }

        #region User Role        
        [ApplicationAuthorize("SYS020", PermissionNames.View)]
        [SiteMapPageTitle("SYS020")]
        public ActionResult UserRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListRole([DataSourceRequest]DataSourceRequest request, SearchRoleCriteria criteria)
        {
            var dataItems = RoleManager.Roles.Where(t => String.IsNullOrEmpty(criteria.Name) || t.Name.Contains(criteria.Name)).ToList();
            var result = dataItems.ToDataSourceResult(request, (Role r) => r.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRole(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(viewModel.RoleID);
                if (role == null)
                {
                    return InternalServerError("Update role fail. Role data not found.");
                }
                role.Name = viewModel.Name;
                role.IsActive = viewModel.IsActive;
                role.Description = viewModel.Description;
                role.UpdateBy = User.Identity.Name;
                role.UpdateDate = DateTime.Now;
                var result = await RoleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    var message = result.Errors.First();
                    return InternalServerError(message);
                }
                return Ok(MessageHelper.SaveCompleted());
            }
            return InvalidRequest();
        }

        [HttpPost]
        [ApplicationSuspend]
        public async Task<ActionResult> DeleteRole(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(viewModel.RoleID);
                if (role == null)
                {
                    return NotFound(MessageHelper.DataNotFound());
                }

                var result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    var message = result.Errors.First();
                    return InternalServerError(MessageHelper.DeleteFailed(message));
                }
                else
                {
                    service.DeletesPermission(viewModel.RoleID);
                }
                return Ok(MessageHelper.DeleteCompleted());
            }
            return InvalidRequest();
        }
        
        [HttpPost]
        [ApplicationSuspend]
        public async Task<ActionResult> CreateRole(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var role = viewModel.ToEntity();
                role.IsSystemRole = false;
                role.CreateBy = User.Identity.Name;
                role.UpdateBy = User.Identity.Name;
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    var message = string.Join(".", roleresult.Errors);
                    return InternalServerError(message);
                }
                return Ok(MessageHelper.SaveCompleted());
            }
            return InvalidRequest("Create Role failed. Invalid data.");
        }
        #endregion

        #region User Role Authorization
        [ApplicationAuthorize("SYS030", PermissionNames.View)]
        [SiteMapPageTitle("SYS030")]
        public ActionResult UserRoleAuthorize()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult ListAuthorize([DataSourceRequest]DataSourceRequest request)
        //{
        //    var dataItems = JsonDataContext.GetDataFromJsonFile<AuthorizeRule>(HostingEnvironment.MapPath("~/content/json/security/roles.json"));
        //    var result = dataItems.ToDataSourceResult(request);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        #endregion

       
    }

}