using CSI.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class AcsVIPController : AppControllerBase
    {
        protected readonly IAccessControlService service;

        public AcsVIPController(IAccessControlService serivce)
        {
            this.service = serivce;
        }

        [ApplicationAuthorize("ACS090", PermissionNames.View)]
        [SiteMapPageTitle("ACS090")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NoCache]
        public ActionResult ListVIPCardRegistration([DataSourceRequest]DataSourceRequest request, VIPCardRegistrationSearchCriteria criteria)
        {
            criteria.EntryDate = DateTime.Now.Date;
            var dataItems = service.GetVIPCardRegistrationViews(criteria);
            var result = dataItems.ToDataSourceResult(request, (VIPCardRegistrationView item) => item.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ApplicationSuspend]
        [ValidateAntiForgeryToken]
        public ActionResult SaveVIPCardRegistration(VIPCardRegistrationViewModel model)
        {
            if (model.Status == VIPCardStatus.Available)
            {
                // Create Card 
                var entity = model.ToEntity();
                entity.Status = VIPCardStatus.Unavailable;
                entity.CreateBy = User.Identity.Name;
                var result = service.CreateAcsVIP(entity);
                if (result.IsSucceed)
                {
                    // Invoke Export Interface file.
                    var options = ApplicationContext.Setting.Task.ToExportToAccessControlTaskOptions();
                    options.TaskOptions.Transactions = entity.TransactionAcs.Select(t => t.TranID).ToArray();
                    options.TaskOptions.ExportModes = ExportToAccessControlModes.Add;
                    var exportResult = ExportInterfaceFileToAccessControl.Execute(options);
                    if (exportResult.IsSucceed)
                        return Ok(MessageHelper.SaveCompleted());
                    else
                    {
                        // Rollback data when error occured while export interface file.
                        service.DeleteAcsVIP(entity);
                        return InternalServerError(exportResult.GetErrorMessage());
                    }
                }
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else
            {
                // Return Card
                var entity = model.ToEntity();
                entity.Status = VIPCardStatus.Available;
                entity.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsVIP(entity);
                if (result.IsSucceed)
                {
                    return Ok(MessageHelper.SaveCompleted());
                }
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }

        }
    }
}