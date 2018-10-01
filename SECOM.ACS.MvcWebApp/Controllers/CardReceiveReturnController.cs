using CSI.Localization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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
using System.Runtime.Caching;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class CardReceiveReturnController : AppControllerBase
    {
        private readonly IAccessControlService service;
        public CardReceiveReturnController(IAccessControlService service)
        {
            this.service = service;
        }

        [ApplicationAuthorize("ACS081", PermissionNames.View)]
        [SiteMapPageTitle("ACS081")]
        public ActionResult Index()
        {
            return View();
        }

        

        #region Visitor
        [HttpPost]
        public ActionResult ListVisitorCard([DataSourceRequest]DataSourceRequest request, VisitorCardDataSearchCriteria criteria)
        {
            var dataItems = service.GetReceiveReturnVisitorCardDataViews(criteria).ToList();
            var result = dataItems.ToDataSourceResult(request, (ReceiveReturnVisitorCardDataView data) => data.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateVisitorCard(ReceiveReturnVisitorCardDataViewModel model)
        {
            var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            var result = service.UpdateVisitorCard(entity);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }

        public ActionResult ReceiveVisitorCard(ReceiveReturnVisitorCardDataViewModel model)
        {
            var transaction = service.GetAcsTransaction(model.TranID);
            if (transaction == null)
            {
                throw new Exception("Transaction data not found.");
            }
            
            if (transaction.CardReceiveTime.HasValue)
            {
                throw new Exception($"Card was receive on {transaction.CardReceiveTime.Value.ToString("d/M/yyyy H:mm")}.");
            }

            var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            var result = service.ReceiveVisitorCard(entity);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }

        public ActionResult ReturnVisitorCard(ReceiveReturnVisitorCardDataViewModel model)
        {
            var transaction = service.GetAcsTransaction(model.TranID);
            if (transaction == null)
            {
                throw new Exception("Transaction data not found.");
            }

            if (!transaction.CardReceiveTime.HasValue)
            {
                throw new Exception($"Card was not receive.");
            }

            if (transaction.CardReturnTime.HasValue)
            {
                throw new Exception($"Card was return on {transaction.CardReturnTime.Value.ToString("d/M/yyyy H:mm")}.");
            }

            var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            var result = service.ReturnVisitorCard(entity);
            if (result.IsSucceed) {
                var options = new ExportInterfaceFileToAccessControlTaskOptions()
                {
                    ExportInterfaceFileOptions = ApplicationContext.Setting.Task.ExportFileOptions,
                    TaskOptions = new ExportToAccessControlOptions()
                    {
                        ExportModes = Tasks.ExportToAccessControlModes.Cancel,
                        Transactions = new Guid[] { entity.TranID }
                    }
                };
                var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                if (!taskResult.IsSucceed)
                {
                    return InternalServerError(MessageHelper.SaveFailed(taskResult.GetErrorMessage()));
                }
                return Ok(MessageHelper.SaveCompleted());
            }
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }
        #endregion

        #region Business Trip
        [HttpPost]
        public ActionResult ListBusinessTripCard([DataSourceRequest]DataSourceRequest request, BusinessTripCardDataSearchCriteria criteria)
        {
            var dataItems = service.GetReceiveReturnBusinessTripCardDataViews(criteria).ToList();
            var result = dataItems.ToDataSourceResult(request, (ReceiveReturnBusinessTripCardDataView data) => data.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceiveBusinessTripCard(ReceiveReturnBusinessTripCardDataViewModel model)
        {
            var transaction = service.GetAcsTransaction(model.TranID);
            if (transaction == null) {
                throw new Exception("Transaction data not found.");
            }

            if (transaction.CardReceiveTime.HasValue)
            {
                throw new Exception($"Card was received on {transaction.CardReceiveTime.Value.ToString("d/M/yyyy H:mm")}.");
            }

            var dataItem = model.ToEntity();
            dataItem.UpdateBy = User.Identity.Name;

            var result = service.ReceiveBusinessCard(dataItem);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnBusinessTripCard(ReceiveReturnBusinessTripCardDataViewModel model)
        {
            var transaction = service.GetAcsTransaction(model.TranID);
            if (transaction == null)
            {
                throw new Exception("Transaction data not found.");
            }

            if (transaction.CardReturnTime.HasValue)
            {
                throw new Exception($"Card was return on {transaction.CardReturnTime.Value.ToString("d/M/yyyy H:mm")}.");
            }

            var dataItem = model.ToEntity();
            dataItem.UpdateBy = User.Identity.Name;
            var result = service.ReturnBusinessCard(dataItem);
            if (result.IsSucceed) {
                var options = new ExportInterfaceFileToAccessControlTaskOptions()
                {
                    ExportInterfaceFileOptions = ApplicationContext.Setting.Task.ExportFileOptions,
                    TaskOptions = new ExportToAccessControlOptions()
                    {
                        ExportModes = Tasks.ExportToAccessControlModes.Cancel,
                        Transactions = new Guid[] { dataItem.TranID }
                    }
                };
                var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                if (!taskResult.IsSucceed)
                {
                    return InternalServerError(MessageHelper.SaveFailed(taskResult.GetErrorMessage()));
                }
                return Ok(MessageHelper.SaveCompleted());
            }
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));

        }
        #endregion
        
    }
}