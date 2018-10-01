using CSI.Core;
using CSI.Web.Mvc;
using CSI.Web.Mvc.KendoUI;
using CSI.Web.Mvc.KendoUI.Extensions;
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
using Kendo.Mvc.Extensions;
using CSI.ComponentModel;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class CardRegistrationController : AppControllerBase
    {
        protected readonly IAccessControlService service;

        public CardRegistrationController(IAccessControlService serivce)
        {
            this.service = serivce;
        }
        
        [ApplicationAuthorize("ACS080", PermissionNames.View)]
        [SiteMapPageTitle("ACS080")]
        public ActionResult Index()
        {
            return View();
        }

        #region Caching Methods
        private T GetDataFromCache<T>(string tempDataId, out bool cached)
           where T : class, new()
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new T();
            }
            return cache[key] as T;
        }

        private void SaveDataIntoCache<T>(string tempDataId, T data)
        {
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
            cache.Add(key, data, policy);
        }

        private void RemoveDataFromCache(string tempDataId)
        {
            var key = tempDataId.ToLowerInvariant();
            var cache = MemoryCache.Default;
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }
        #endregion

        #region Visitor Card Registration
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ListTempVisitorCardRegistration([DataSourceRequest]DataSourceRequest request, string tempDataId, VisitorCardRegistrationSearchCriteria criteria)
        {
            if (criteria.ClearCache)
            {
                RemoveDataFromCache(tempDataId);
            }
            criteria.ApplyDataSourceRequest(request);
            bool cached;
            var dataItems = GetDataFromCache<List<VisitorCardRegistrationViewModel>>(tempDataId, out cached);
            if (!cached)
            {
                dataItems = service.GetVisitorCardRegistrationsByCriteria(criteria).Select(t=>t.ToViewModel()).ToList();
                SaveDataIntoCache(tempDataId, dataItems);
            }
            return JsonNet(dataItems.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateTempVisitorCardRegistration(string tempDataId, VisitorCardRegistrationViewModel model)
        {
            bool cached = false;
            var data = GetDataFromCache<List<VisitorCardRegistrationViewModel>>(tempDataId, out cached);

            var transaction = data.FirstOrDefault(t => t.TranID == model.TranID);
            if (transaction == null)
            {
                return InternalServerError(MessageHelper.DataNotFoundFormat("Access Control Transaction data not found from select card."));
            }

            if (!String.IsNullOrEmpty(model.CardID))
            {
                //2.1 Validate Duplicate Card No.
                var findItemCard = data.FirstOrDefault((t => t.CardID == model.CardID && t.TranID != model.TranID));
                if (findItemCard != null)
                {
                    return InternalServerError(MessageHelper.DuplicateInList($"Card No. {findItemCard.CardNo}"));
                }
              
                var card = service.GetCard(model.CardID);
                if (card == null)
                {
                    return InternalServerError(MessageHelper.DataNotFoundFormat("Card ID: {0} data not found.", model.CardID));
                }

                if (model.VerifyTypeID > 0)
                {
                    var verifyType = service.GetMisc(model.VerifyTypeID);
                    if (verifyType == null)
                    {
                        return InternalServerError(MessageHelper.DataNotFoundFormat("VerifyType ID: {0} data not found.", model.VerifyTypeID));
                    }
                }
                transaction.CardID = card.CardID;
                transaction.CardNo = card.CardNo;
            }
            else {
                transaction.CardID = String.Empty;
                transaction.CardNo = String.Empty;
            }
            transaction.VerifyTypeID = model.VerifyTypeID;
            transaction.VerifyNo = model.VerifyTypeID == 0 ? "" : model.VerifyNo;
            SaveDataIntoCache(tempDataId, data);
            return JsonNet(model, JsonRequestBehavior.AllowGet);
        }
       

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApplicationSuspend]
        public ActionResult SaveVisitorCardRegistration(List<VisitorCardRegistrationViewModel> models,DateTime entryDate)
        {
            var results = new ObjectResults<VisitorCardRegistrationViewModel>();
            var cardToAdds = new List<VisitorCardRegistrationViewModel>();
            var cardToCancels = new List<VisitorCardRegistrationViewModel>();
            foreach (var model in models)
            {
                if (model.CardID == model.OriginalCardID) { continue; }

                if (model.TranID == null)
                {
                    results.AddResult(model, new Exception("Invalid data. Transaction ID is null."));
                    continue;
                }

                try
                {
                    //8.1 Validate Overlap CardNo
                    var result = service.CheckOverlapCardNo(entryDate, model.CardNo);
                    if (!result.IsSucceed)
                    {
                        results.AddResult(model, result.Error);
                        continue;
                    }                   

                    if (!String.IsNullOrEmpty(model.OriginalCardID))
                    {
                        cardToCancels.Add(model);
                    }

                    if (!String.IsNullOrEmpty(model.CardID))
                    {
                        cardToAdds.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    results.AddResult(model, ex);
                }
            }

            // [Cancel] Transaction from Change Register Card ID
            if (cardToCancels.Count > 0)
            {
                if (DateTime.Today.CompareTo(entryDate.Date) >= 0)
                {
                    var options = new ExportInterfaceFileToAccessControlTaskOptions()
                    {
                        ExportInterfaceFileOptions = ApplicationContext.Setting.Task.ExportFileOptions,
                        TaskOptions = new ExportToAccessControlOptions()
                        {
                            ExportModes = Tasks.ExportToAccessControlModes.Cancel,
                            EffectiveDate = entryDate,
                            Transactions = cardToCancels.Select(t=>t.TranID).ToArray()
                        }
                    };
                    var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                    if (!taskResult.IsSucceed)
                    {
                        results.AddResult(null, taskResult.Error);
                    }
                }
                else {
                    // Update Acs Transaction to Cancel
                    cardToCancels.Select(t=>t.TranID).Each((Guid transactionId) =>
                    {
                        var acs = service.GetAcsTransaction(transactionId);
                        if (acs != null && acs.Status != (byte)TransactionStatus.SendCardToCancel)
                        {
                            acs.Status = (byte)TransactionStatus.SendCardToCancel;
                            acs.CancelAcsDate = DateTime.Now;
                            acs.UpdateBy = User.Identity.Name;
                            acs.UpdateDate = DateTime.Now;
                            service.UpdateAcsTransactions(acs);
                        }
                    });
                }
            }
            // Update Visitor Card Registration for Cancel
            foreach (var model in cardToCancels)
            {
                var entity = model.ToEntity();
                entity.UserName = User.Identity.Name;
                var updateResult = service.UpdateVisitorCardRegistrationView(entity);
                if (!updateResult.IsSucceed)
                {
                    results.AddResult(model, updateResult.Error);
                }
            }
            // Update Visitor Card Registration for Add
            foreach (var model in cardToAdds)
            {
                var entity = model.ToEntity();
                entity.UserName = User.Identity.Name;
                var updateResult = service.UpdateVisitorCardRegistrationView(entity);
                if (!updateResult.IsSucceed)
                {
                    results.AddResult(model, updateResult.Error);
                }
            }

            // [Add] Transaction from Register Card ID (EMPTY => {CARDNO})
            if (cardToAdds.Count > 0)
            {
                if (DateTime.Today.CompareTo(entryDate.Date) >= 0)
                {
                    // Generate interface when Current Day >= EntryDate
                    var options = new ExportInterfaceFileToAccessControlTaskOptions()
                    {
                        ExportInterfaceFileOptions = ApplicationContext.Setting.Task.ExportFileOptions,
                        TaskOptions = new ExportToAccessControlOptions()
                        {
                            ExportModes = Tasks.ExportToAccessControlModes.Add,
                            EffectiveDate = entryDate,
                            Transactions = cardToAdds.Select(t=>t.TranID).ToArray()
                        }
                    };
                    var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                    if (!taskResult.IsSucceed)
                    {
                        results.AddResult(null, taskResult.Error);
                    }
                }
            }
            return JsonNet(results.ToViewResult(), JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetVisitorCardRegistration(string tempDataId)
        {
            RemoveDataFromCache(tempDataId);
            return Ok(MessageHelper.SaveCompleted());
        }
        #endregion

        #region Business Trip Card Registration
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ListTempBusinessTripCardRegistration([DataSourceRequest]DataSourceRequest request, string tempDataId, BusinessTripCardRegistrationSearchCriteria criteria)
        {
            if (criteria.ClearCache)
            {
                RemoveDataFromCache(tempDataId);
            }
            criteria.ApplyDataSourceRequest(request);
            bool cached;
            var dataItems = GetDataFromCache<List<BusinessTripCardRegistrationViewModel>>(tempDataId, out cached);
            if (!cached)
            {
                dataItems = service.GetBusinessTripCardRegistrationsByCriteria(criteria).Select(t => t.ToViewModel()).ToList();
                SaveDataIntoCache(tempDataId, dataItems);
            }
            return JsonNet(dataItems.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateTempBusinessTripCardRegistration(string tempDataId, BusinessTripCardRegistrationViewModel model)
        {
            bool cached = false;            
            var key = tempDataId.ToLowerInvariant();
            var data = GetDataFromCache<List<BusinessTripCardRegistrationViewModel>>(tempDataId, out cached);
            var results = new ObjectResults<BusinessTripCardRegistrationViewModel>();

            var transaction = data.FirstOrDefault(t => t.TranID == model.TranID);
            if (transaction == null)
            {
                return InternalServerError(MessageHelper.DataNotFoundFormat("Access Control Transaction data not found from select card."));
            }

            if (!String.IsNullOrEmpty(model.CardID))
            {
                // Select card from list => validate selected card that duplicate other card in cache.
                var findItemCard = data.FirstOrDefault(t => t.CardID == model.CardID && t.TranID != model.TranID);
                if (findItemCard != null)
                {
                    return InternalServerError(MessageHelper.DuplicateInList($"Card No. {findItemCard.CardNo}"));
                }

                var card = service.GetCard(model.CardID);
                if (card == null)
                {
                    return InternalServerError(MessageHelper.DataNotFound($" Card No: {model.CardNo} data not found."));
                }

                if(model.CardID != model.OriginalCardID)
                {
                    model.CardNo = card.CardNo;
                    var result = service.CheckOverlapPeriodCardNo(model.EntryDateFrom, model.EntryDateTo, model.CardNo);
                    if (!result.IsSucceed)
                    {
                        results.AddResult(model, result.Error);
                        var message = String.Join(", ", results.GetErrorMessages());
                        return InternalServerError(message);
                    }
                }
                
                transaction.CardID = card.CardID;
                transaction.CardNo = card.CardNo;
            }
            else {
                // Clear Card
                transaction.CardID = String.Empty;
                transaction.CardNo = String.Empty;
            }
            SaveDataIntoCache(tempDataId, data);
            return JsonNet(transaction, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApplicationSuspend]
        public ActionResult SaveBusinessTripCardRegistration(string tempDataId, List<BusinessTripCardRegistrationViewModel> models)
        {
            var results = new ObjectResults<BusinessTripCardRegistrationViewModel>();
            var cardToAdds = new List<BusinessTripCardRegistrationViewModel>();
            var cardToCancels = new List<BusinessTripCardRegistrationViewModel>();
            bool cached = false;
            var data = GetDataFromCache<List<BusinessTripCardRegistrationViewModel>>(tempDataId, out cached);

            //Loop check all card is overlap?.
            foreach (var model in models)
            {
                if (model.CardID == model.OriginalCardID) { continue; }
                //8.1 Validate Overlap CardNo
                var card = service.GetCard(model.CardID);
                if (card == null)
                {
                    return InternalServerError(MessageHelper.DataNotFound($" Card No: {model.CardNo} data not found."));
                }

                model.CardNo = card.CardNo;
                var result = service.CheckOverlapPeriodCardNo(model.EntryDateFrom, model.EntryDateTo, model.CardNo);
                if (!result.IsSucceed)
                {
                    results.AddResult(model, result.Error);
                    var message = String.Join(", ", results.GetErrorMessages());
                    return InternalServerError(message);
                }
            }

            foreach (var model in models)
            {
                if (!String.IsNullOrEmpty(model.CardID))
                { 
                    var findItemCard = data.FirstOrDefault(t => t.CardID == model.CardID && t.TranID != model.TranID);
                    if (findItemCard != null)
                    {
                        return InternalServerError(MessageHelper.DuplicateInList($"Card No. {findItemCard.CardNo}"));
                    }
                }

                if (model.CardID == model.OriginalCardID) { continue; }
                //6.1 Validate Card No. require feild
                //if (String.IsNullOrEmpty(model.CardNo))
                //{
                //    results.AddResult(model, new Exception(MessageHelper.RequiredDropDownList("Card No.")));
                //    continue;
                //}

                if (model.TranID == null)
                {
                    results.AddResult(model, new Exception("Invalid data. Transaction ID is null."));
                    continue;
                }

                try
                {
                    //8.1 Validate Overlap CardNo
                    //var card = service.GetCard(model.CardID);
                    //if (card == null)
                    //{
                    //    return InternalServerError(MessageHelper.DataNotFound($" Card No: {model.CardNo} data not found."));
                    //}

                    //if (model.CardID != model.OriginalCardID)
                    //{
                    //    model.CardNo = card.CardNo;
                    //    var result = service.CheckOverlapPeriodCardNo(model.EntryDateFrom, model.EntryDateTo, model.CardNo);
                    //    if (!result.IsSucceed)
                    //    {
                    //        results.AddResult(model, result.Error);
                    //        continue;
                    //    }
                    //}

                    if (!String.IsNullOrEmpty(model.OriginalCardID))
                    {
                        cardToCancels.Add(model);                        
                    }

                    if (!String.IsNullOrEmpty(model.CardID))
                    {
                        cardToAdds.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    results.AddResult(model, ex);
                }
            }

            // [Cancel] Transaction from Change Register Card ID
            if (cardToCancels.Count > 0)
            {
                var options = new ExportInterfaceFileToAccessControlTaskOptions()
                {
                    ExportInterfaceFileOptions = ApplicationContext.Setting.Task.ExportFileOptions,
                    TaskOptions = new ExportToAccessControlOptions() { ExportModes = Tasks.ExportToAccessControlModes.Cancel, Transactions = cardToCancels.Select(t=>t.TranID).ToArray() }
                };
                var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                if (!taskResult.IsSucceed)
                {
                    results.AddResult(null, taskResult.Error);
                }                
            }

            // Update Bussienss Trip Card Registration for Cancel
            foreach (var model in cardToCancels)
            {
                var entity = model.ToEntity();
                entity.UserName = User.Identity.Name;
                var updateResult = service.UpdateBusinessTripCardRegistrationView(entity);
                if (!updateResult.IsSucceed)
                {
                    results.AddResult(model, updateResult.Error);
                }
            }
            // Update Bussienss Trip Card Registration for Add
            foreach (var model in cardToAdds)
            {
                var entity = model.ToEntity();
                entity.UserName = User.Identity.Name;
                var updateResult = service.UpdateBusinessTripCardRegistrationView(entity);
                if (!updateResult.IsSucceed)
                {
                    results.AddResult(model, updateResult.Error);
                }
            }

            // [Add] Transaction from Register Card ID (EMPTY => {CARDNO})
            if (cardToAdds.Count > 0)
            {
                var options = new ExportInterfaceFileToAccessControlTaskOptions()
                {
                    ExportInterfaceFileOptions = ApplicationContext.Setting.Task.ExportFileOptions,
                    TaskOptions = new ExportToAccessControlOptions() { ExportModes = Tasks.ExportToAccessControlModes.Add, Transactions = cardToAdds.Select(t=>t.TranID).ToArray() }
                };
                var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                if (!taskResult.IsSucceed) {
                    results.AddResult(null, taskResult.Error);
                }               
            }
            return JsonNet(results.ToViewResult(), JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetBusinessTripCardRegistration(string tempDataId)
        {
            RemoveDataFromCache(tempDataId);
            return Ok(MessageHelper.SaveCompleted());
        }
        #endregion

        public ActionResult FindCards(FilterRequest filter, string cardType, int pageSize)
        {
            var criteria = new RegisterCardSearchCriteria()
            {
                PageSize = pageSize,
                CardType = cardType
            };

            if (filter.Filters.Count > 0)
            {
                criteria.CardID = filter.Filters[0].Value;
            }

            var cards = service.GetCardNoList(criteria);
            return JsonNet(cards, JsonRequestBehavior.AllowGet);
        }
    }
}