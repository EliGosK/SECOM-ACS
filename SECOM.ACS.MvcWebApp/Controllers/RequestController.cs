using CSI.Localization;
using CSI.Text;
using CSI.Web.Mvc.KendoUI.Extensions;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Reporting;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class RequestController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public RequestController(IAccessControlService service)
        {
            this.service = service;
        }

        [SiteMapPageTitle("ACS010")]
        public ActionResult Dashboard()
        {
            var dataItems = service.GetPermissionDashboard(User.Identity.Name);
            var permission = new PrivilegeViewDSHData();
            var userData = User.Identity.GetUserData();
            foreach (var dataItem in dataItems)
            {
                permission.ViewDSH01 |= dataItem.ViewDSH01;
                permission.ViewDSH02 |= dataItem.ViewDSH02;
                permission.ViewDSH03 |= dataItem.ViewDSH03;
                permission.ViewDSH04 |= dataItem.ViewDSH04;
                permission.ViewDSH05 |= dataItem.ViewDSH05;
                permission.ViewDSH06 |= dataItem.ViewDSH06;
                permission.ViewDSH07 |= dataItem.ViewDSH07;
                permission.ViewDSH08 |= dataItem.ViewDSH08;
                permission.IsVerifyItemIn |= (dataItem.IsVerifyItemIn && userData.IsVerifyItemIn);
                permission.IsVerifyItemOut |= (dataItem.IsVerifyItemOut && userData.IsVerifyItemOut);
                permission.IsLending |= (dataItem.ViewDSH07 && userData.IsLending);
            }
            return View(permission);
        }

        [SiteMapPageTitle("ACS070")]
        public ActionResult Inquiry()
        {

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq01([DataSourceRequest]DataSourceRequest request,DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForRequestInProgress(criteria);
            var result = dataItems.ToDataSourceResult(request,(RequestDH01DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds01", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq02([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForRequestWaitToApprover(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH02DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds02", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq03([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForRequestWaitToAcknowledge(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH03DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds03", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq04([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForSecurityRoom(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH04DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds04", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq05([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForItemOut(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH05DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds05", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq06([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForItemIn(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH06DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds06", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DashboardReq07([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForLending(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH07DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds07", result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult DashboardReq08([DataSourceRequest]DataSourceRequest request, DashboardSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            criteria.User = User.Identity.Name;
            var dataItems = service.GetDashboardForWitness(criteria);
            var result = dataItems.ToDataSourceResult(request, (RequestDH08DataView data) => data.ToViewModel());
            return JsonNet(DashboardDataSourceResult.ToViewResult("ds08", result), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ListRequestInquiry([DataSourceRequest]DataSourceRequest request, RequestInquriySearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            var dataItem = service.GetRequestDataBySearchCriteria(criteria);
            var result = dataItem.ToDataSourceResult(request, (RequestDataView data) => data.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportRequestInquiryResultAsync(RequestInquriySearchCriteria criteria)
        {
            try
            {
                var dataItems = service.GetRequestDataBySearchCriteria(criteria).ToList();
                if(dataItems.Count == 0)
                {
                    return InternalServerError(MessageHelper.DataNotFound());
                }
                var reportBuilder = new RequestAccessReportBuilder();
                var ms = new MemoryStream();
                var reportData = new RequestAccessReportData()
                {
                    Data = dataItems,
                    HeaderNames = AcsModelHelper.GetModelDisplayNames<RequestInquriyDataViewModel>()
                };


                reportBuilder.CreateReport(ms, reportData);
                ms.Position = 0;
                string fileDownloadName = "RequestInquiry.xlsx";
                var fileKey = TextGenerator.Generate(32).ToLowerInvariant();
                TempData[fileKey] = ms;
                return JsonNet(new { id = fileKey, filename = fileDownloadName, message = MessageHelper.GenerateReportSuccess("Request Inquiry") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        public ActionResult LoadPartial(string name)
        {
            return PartialView(name);
        }

        public ActionResult ListDocumentTypeForRequestAcknowledge()
        {
            var codes = new string[] { "ACS050", "ACS060" };
            var dataItems = service.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                .OrderBy(t => t.SysMiscSortNo)
                .Where(t => t.IsActive && codes.Contains(t.SysMiscCode))
                .Select(t => new { Name = ModelLocalizeManager.GetValue(t, "SysMisc"), Value = t.SysMiscCode })
                .ToList();

            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
                
        }
    }
}