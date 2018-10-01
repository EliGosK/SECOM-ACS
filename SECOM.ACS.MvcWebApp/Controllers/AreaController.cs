using CSI.ComponentModel;
using CSI.Web.Mvc;
using CSI.Web.Mvc.KendoUI;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class AreaController: AppControllerBase
    {
        protected readonly IAccessControlService service;
        public AreaController(IAccessControlService service)
        {
            this.service = service;
        }

        [ApplicationAuthorize("MAS030", PermissionNames.View)]
        [SiteMapPageTitle("MAS030")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NoCache]
        public ActionResult List([DataSourceRequest]DataSourceRequest request, AreaSearchCriteria criteria)
        {
            var dataItems =  service.GetAreaByCriteria(criteria,AreaLoadOptions.All);
            var result = dataItems.ToDataSourceResult(request, (Area area) => area.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ApplicationSuspend]
        public ActionResult Create(AreaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(viewModel.ApproverDepartment) || String.IsNullOrEmpty(viewModel.ApproverPosition))
                {
                    
                    return InternalServerError(MessageHelper.SaveFailed());
                }
                
                
                var entity = viewModel.ToEntity();
                entity.CreateBy = User.Identity.Name;
                entity.UpdateBy = User.Identity.Name;
                var result = service.CreateArea(entity);
                if (result.IsSucceed)
                    return Ok(MessageHelper.SaveCompleted());
                else
                {
                    if (result.Error.GetType() == typeof(DuplicateDataException))
                    {
                        var args = new string[]{
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(AreaViewModel), "AreaName").GetDisplayName(),
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(AreaViewModel), "FactoryCode").GetDisplayName(),
                            entity.AreaName,
                            entity.FactoryCode
                        };
                        return InternalServerError(MessageHelper.DuplicateData(args));
                    }
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [ApplicationSuspend]
        public ActionResult Update(AreaViewModel viewModel)
        {
            
            if (ModelState.IsValid)
            {
                var entity = viewModel.ToEntity();
                entity.UpdateBy = User.Identity.Name;
                

                var result = service.UpdateArea(entity);
                if (result.IsSucceed)
                    return Ok(MessageHelper.SaveCompleted());
                else
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            return BadRequest();
        }

        [ApplicationSuspend]
        [HttpPost]
        public ActionResult Delete(AreaViewModel viewModel)
        {
            //if (ModelState.IsValid)
            //{
                var area = service.GetArea(viewModel.AreaID);
                if (area == null)
                {
                    return InternalServerError("Delete failed. Area data not found or delete by other user.");
                }

                var entity = viewModel.ToEntity();
                var result = service.DeleteArea(entity);
                if (result.IsSucceed)
                    return Ok(MessageHelper.DeleteCompleted());
                else
                    return InternalServerError(MessageHelper.DeleteFailed(result.GetErrorMessage()));
          
        }
        
        
    }
}