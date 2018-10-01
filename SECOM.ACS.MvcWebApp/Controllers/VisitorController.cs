using CSI.Localization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class VisitorController : AppControllerBase
    {
        // GET: Visitor
        private readonly IAccessControlService service;
        public VisitorController(IAccessControlService service)
        {
            this.service = service;
        }

        [ApplicationAuthorize("ACS081", PermissionNames.View)]
        [SiteMapPageTitle("ACS081")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListVisitorCard([DataSourceRequest]DataSourceRequest request, VisitorCardDataSearchCriteria criteria)
        {
            var dataItems = service.GetReceiveReturnVisitorCardDataViews(criteria);
            var result = dataItems.ToDataSourceResult(request, (ReceiveReturnVisitorCardDataView data) =>data.ToViewModel());

            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ListBusinessCard([DataSourceRequest]DataSourceRequest request, BusinessTripCardDataSearchCriteria criteria)
        {
            var dataItems = service.GetReceiveReturnBusinessTripCardDataViews(criteria);
            var result = dataItems.ToDataSourceResult(request, (ReceiveReturnVisitorCardDataView data) => data.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ReturnBusinessCard(System.Guid tranID)
        {
            if(tranID == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            var transaction = service.GetAcsTransaction(tranID);
            if (transaction.CardReturnTime.HasValue) {
                throw new Exception("Card was already return by other user.");
            }

            var dataView = new ReceiveReturnBusinessTripCardDataView();
            dataView.TranID = tranID;
            dataView.UpdateBy = User.Identity.Name;
            var result = service.ReturnBusinessCard(dataView);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));

        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ReceiveBusinessCard(System.Guid TranID )
        {
            if (TranID == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }
            var dataView = new ReceiveReturnBusinessTripCardDataView();
            dataView.TranID = TranID;
            dataView.UpdateBy = User.Identity.Name;
             var result = service.ReceiveBusinessCard(dataView);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ReturnVisitorCard(System.Guid TranID,int VerifyTypeID, string VerifyNo )
        {
            if (TranID == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }
            var datas = new ReceiveReturnBusinessTripCardDataView();
            datas.VerifyTypeID = VerifyTypeID;
            datas.VerifyNo = VerifyNo;
            datas.UpdateBy = User.Identity.Name;
            var result = service.ReturnVisitorCard(datas);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ReceiveVisitorCard(System.Guid TranID, int VerifyTypeID, string VerifyNo)
        {
            if (TranID == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }
            var datas = new ReturnRetrieveVisitorCardData();
            datas.VerifyTypeID = VerifyTypeID;
            datas.VerifyNo = VerifyNo;
            datas.UpdateBy = User.Identity.Name;
            var result = service.UpdateReceiveVisitorCard(datas);
            if (result.IsSucceed)
                return Ok(MessageHelper.SaveCompleted());
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
        }

        #region Temp Visitor Card
        [OutputCache(NoStore = true, Duration = 0)]
        public JsonResult GetTempVisitorCard([DataSourceRequest]DataSourceRequest request, string tempDataId, VisitorCardDataSearchCriteria criteria)
        {
            bool cached = false;
            //if (!criteria.LoadFromCache)
            //{
            //    var dataItems = service.GetDataReturnRetrieveVisitorCard(criteria).Select(t => t.ToViewModel()).ToList();
            //    SaveVisitorCardDataIntoCache(tempDataId, dataItems);
            //    var result = dataItems.ToDataSourceResult(request);
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var dataItems = GetVisitorCardDataFromCache(tempDataId, out cached);
            //    if (!cached)
            //    {
            //        dataItems = service.GetDataReturnRetrieveVisitorCard(criteria).Select(t => t.ToViewModel()).ToList();
            //        SaveVisitorCardDataIntoCache(tempDataId, dataItems);
            //    }
            //    var result = dataItems.ToDataSourceResult(request);
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}
            var data = GetVisitorCardDataFromCache(tempDataId, out cached).OrderBy(t => t.TranID).ToList();
            if (!cached && !String.IsNullOrEmpty(tempDataId) || data == null)
            {
                try
                {
                    data = service.GetReceiveReturnVisitorCardDataViews(criteria).Select(t => t.ToViewModel()).ToList();
                    if (data.Count > 0)
                    {

                        SaveVisitorCardDataIntoCache(tempDataId, data);
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<ReceiveReturnVisitorCardDataViewModel> GetVisitorCardDataFromCache(string tempDataId, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<ReceiveReturnVisitorCardDataViewModel>();
            }
            return cache[key] as List<ReceiveReturnVisitorCardDataViewModel>;
        }

        private void SaveVisitorCardDataIntoCache(string tempDataId, IList<ReceiveReturnVisitorCardDataViewModel> model)
        {
            model.OrderBy(t => t.TranID).ToList();
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(60),
                Priority = CacheItemPriority.Default
            };
            cache.Add(key, model, policy);
        }

        public ActionResult UpdateTempVisitorCard(string tempDataId, ReceiveReturnVisitorCardDataViewModel model)
        {
            bool cached = false;
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            var data = GetVisitorCardDataFromCache(tempDataId, out cached);
            //2.1 Validate Duplicate Card No.
            var findItemCard = data.FirstOrDefault((t => t.CardID == model.CardID && t.TranID != model.TranID));
            if (findItemCard != null)
            {
                return InternalServerError(MessageHelper.DuplicateInList("Card No."));
            }
            var findItem = data.FirstOrDefault(t => t.TranID == model.TranID);
            if (findItem != null)
            {
                //var card = service.GetCard(model.CardID);
                //if (card == null)
                //{
                //    return InternalServerError(MessageHelper.DataNotFoundFormat("Card ID: {0} data not found.", model.CardID));
                //}
                var VerifyType = service.GetMisc(model.VerifyTypeID);
                if (VerifyType == null)
                {
                    return InternalServerError(MessageHelper.DataNotFoundFormat("VerifyType ID: {0} data not found.", model.VerifyTypeID));
                }
                findItem.VerifyTypeID = model.VerifyTypeID;
                findItem.VerifyNo = model.VerifyNo;
                findItem.VerifyType = ModelLocalizeManager.GetValue(VerifyType, "MiscDisplay");
                SaveVisitorCardDataIntoCache(tempDataId, data);
                return JsonNet(findItem, JsonRequestBehavior.AllowGet);
            }
            return JsonNet(model, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ClearTempVisitorCard(string tempDataId)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
            return Ok("Clear Temporary  is successfully");
        }
        #endregion

    }
}