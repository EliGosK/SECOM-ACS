using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using CSI.ComponentModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.Models;
using System.Web.Hosting;
using SECOM.ACS.Json;
using SECOM.ACS.Web.Mvc;
using SECOM.ACS.Services;
using CSI.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class ItemController : AppControllerBase
    {
        protected readonly IAccessControlService serivce;
        public ItemController(IAccessControlService serivce)
        {
            this.serivce = serivce;
        }

        [ApplicationAuthorize("MAS020", PermissionNames.View)]
        [SiteMapPageTitle("MAS020")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NoCache]
        public ActionResult List([DataSourceRequest]DataSourceRequest request, ItemSearchCriteria criteria)
        {
            var dataItems = serivce.GetItemViewsByCriteria(criteria);
            var result = dataItems.ToDataSourceResult(request, (ItemDataView item) => item.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [IgnoreModelErrors("CreateDate, UpdateDate")]
        [ApplicationSuspend]
        public ActionResult Create(ItemDataViewModel model)
        {
            if (ModelState.IsValid)
            {

            var entity = model.ToEntity();
            entity.CreateBy = User.Identity.Name;
            entity.UpdateBy = User.Identity.Name;
            var result = serivce.CreateItem(entity);
            if (result.IsSucceed)
            {
                return Ok(MessageHelper.SaveCompleted());
            }
            else
            {
                    if (result.Error.GetType() == typeof(DuplicateDataException))
                    {
                        var args = new string[]{
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(ItemDataViewModel), "ItemTypeID").GetDisplayName(),
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(ItemDataViewModel), "ItemName").GetDisplayName(),
                            model.MiscDisplay,
                            entity.ItemName
                        };
                        return InternalServerError(MessageHelper.DuplicateField(args));
                    }
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }

            }
            return BadRequest();
        }

        [IgnoreModelErrors("CreateDate, UpdateDate")]
        [ApplicationSuspend]
        public ActionResult Update(ItemDataViewModel model)
        {
            if (ModelState.IsValid)
            {

            var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            entity.CreateBy = User.Identity.Name;
            var result = serivce.UpdateItem(entity);
            if (result.IsSucceed)
                return Ok((MessageHelper.SaveCompleted()));
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            return BadRequest();
        }
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        [ApplicationSuspend]
        public ActionResult Delete(ItemDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = serivce.GetItem(model.ItemID);
                if(item == null)
                {
                    return InternalServerError("Delete failed. Item data not found or delete by other user.");
                }
                var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            var result = serivce.DeleteItem(entity);
            if (result.IsSucceed)
                return Ok(MessageHelper.DeleteCompleted());
            else
                return InternalServerError(MessageHelper.DeleteFailed(result.GetErrorMessage()));
            }
            return BadRequest();
        }
    }
}