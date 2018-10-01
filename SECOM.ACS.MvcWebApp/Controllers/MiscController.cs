using CSI.ComponentModel;
using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.MvcWebApp.Resources;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class MiscController : AppControllerBase
    {
        protected readonly IAccessControlService service;

        public MiscController(IAccessControlService service)
        {
            this.service = service;
        }

        [ApplicationAuthorize("MAS010", PermissionNames.View)]
        [SiteMapPageTitle("MAS010")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NoCache]
        public ActionResult List([DataSourceRequest]DataSourceRequest request, MiscSearchCriteria criteria)
        {
            var dataItems = service.GetMiscsByCriteria(criteria);
            var result = dataItems.ToDataSourceResult(request, (Misc item) => item.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [ApplicationSuspend]
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        public ActionResult Create(MiscViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.CreateBy = User.Identity.Name;
                entity.UpdateBy = User.Identity.Name;
                entity.DeleteAble = true;
                var result = service.CreateMisc(entity);
                if (result.IsSucceed)
                    return Ok(MessageHelper.SaveCompleted());
                else {
                    if (result.Error.GetType() == typeof(DuplicateDataException))
                    {
                        var args = new string[]{
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(MiscViewModel), "MiscType").GetDisplayName(),
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(MiscViewModel), "MiscName").GetDisplayName(),
                            entity.MiscType,
                            entity.MiscName
                        };
                        return InternalServerError(MessageHelper.DuplicateField(args));
                    }                    
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
            }
            return BadRequest();
        }

        [ApplicationSuspend]
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        public ActionResult Update(MiscViewModel model)
        {
            if (ModelState.IsValid)
            {
                var misc = service.GetMisc(model.MiscID);
                if (misc == null)
                {
                    return InternalServerError("Delete failed. Misc data not found or delete by other user.");
                }
                
                var entity = model.ToEntity();
                entity.UpdateBy = User.Identity.Name;
                var result = service.UpdateMisc(entity);
                if (result.IsSucceed)
                    return Ok(MessageHelper.SaveCompleted());
                else {
                    if (result.Error.GetType() == typeof(DuplicateDataException))
                    {
                        var args = new string[]{
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(MiscViewModel), "MiscType").GetDisplayName(),
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(MiscViewModel), "MiscName").GetDisplayName(),
                            entity.MiscType,
                            entity.MiscName
                        };
                        return InternalServerError(MessageHelper.DuplicateField(args));
                    }
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
            }
            return BadRequest();
        }

        [ApplicationSuspend]
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        public ActionResult Delete(MiscViewModel model)
        {
            if (ModelState.IsValid)
            {
                var misc = service.GetMisc(model.MiscID);
                if (misc == null)
                {
                    return InternalServerError("Delete failed. Misc data not found or delete by other user.");
                }

                var entity = model.ToEntity();
                entity.UpdateBy = User.Identity.Name;
                var result = service.DeleteMisc(entity);
                if (result.IsSucceed)
                    return Ok(MessageHelper.DeleteCompleted());
                else
                {
                    if (result.Error.GetType() == typeof(MiscInUseException))
                    {
                        return InternalServerError(MessageHelper.DeleteFailed(MessageResource.MSG0040));
                    }
                    return InternalServerError(MessageHelper.DeleteFailed(result.GetErrorMessage()));
                }
                   
            }
            return BadRequest();
        }


    }
}