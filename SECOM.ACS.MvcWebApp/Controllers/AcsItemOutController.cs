using CSI.Localization;
using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using SECOM.ACS.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class AcsItemOutController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public AcsItemOutController (IAccessControlService service)
        {
            this.service = service;
        }

        [SiteMapPageTitle("ACS040")]
        [ApplicationAuthorize("ACS040", PermissionNames.Add)]
        public ActionResult Create()
        {
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            var model = new AcsItemOutViewModel();
            model.RequestNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.ItemOut);
            model.CreateBy = User.Identity.Name;
            model.CreateDate = DateTime.Now;
            model.Status = RequestStatus.Requesting;
            model.RequestEmployee = employee.ToViewModel();
            return View(model);
        }

        [HttpPost]
        [ApplicationSuspend]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AcsItemOutViewModel viewModel)
        {
            viewModel.CreateBy = User.Identity.Name;
            var entity = viewModel.ToEntity();

            //Create AcsItemOut. AcsItemOutDetail, ApproveList
            var result = service.CreateAcsItemOut(entity);
            if (result.IsSucceed)
            {
                FlashMessage.Confirmation(MessageHelper.SaveCompleted());

                var workflowResult = WorkflowManager.RunForCreateRequest(entity);
                if (!workflowResult.IsSucceed)
                {
                    FlashMessage.Warning(MessageHelper.WorkflowError(workflowResult.Error));
                }
                //return RedirectToAction("Create");
                return RedirectToAction(entity.ReqNo, "acsitemout/detail");
            }
            else
            {
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }

            // Return to Create Action
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ListItem([DataSourceRequest]DataSourceRequest request,string tmpDataID)
        {
            bool cached = false;
            var data = GetDataFromCache(tmpDataID, out cached)
                .OrderBy(t => t.Seq)
                .ToList();
            if (!cached && !String.IsNullOrEmpty(tmpDataID))
            {
                try
                {
                    data = new List<AcsItemOutItemViewModel>();
                    SaveItemDataIntoCache(tmpDataID, data);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            var result = data.ToDataSourceResult(request);
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateItemList(string tmpDataID, AcsItemOutItemViewModel model)
        {
            bool cached = false;
            var data = GetDataFromCache(tmpDataID, out cached);
            if (!cached && !String.IsNullOrEmpty(tmpDataID))
            {
                data = new List<AcsItemOutItemViewModel>();
            }

            var findItemDup = data.FirstOrDefault((t => t.ItemID == model.ItemID));
            if (findItemDup != null)
            {
                return InternalServerError(MessageHelper.DuplicateInList($"Item Name {findItemDup.ItemName}"));
            }

            var item = service.GetItem(model.ItemID);
            if (item == null) { return InternalServerError(MessageHelper.DataNotFound($"Item Id {model.ItemID} not found.")); }

            var misc = service.GetMisc(model.ItemTypeID);
            if (misc == null) { return InternalServerError(MessageHelper.DataNotFound($"Item Type Id {model.ItemTypeID} not found.")); }

            model.ItemTypeName = ModelLocalizeManager.GetValue(misc, "MiscDisplay");
            model.ItemName = ModelLocalizeManager.GetValue(item, "ItemDisplay");
            model.Seq = (Int16)(data.Count()==0?1: data.Max(t => t.Seq) + 1);
            data.Add(model);
            SaveItemDataIntoCache(tmpDataID, data);
            return JsonNet(model, JsonRequestBehavior.AllowGet);
        }

        private List<AcsItemOutItemViewModel> GetDataFromCache(string tmpDataID, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tmpDataID.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<AcsItemOutItemViewModel>();
            }
            return cache[key] as List<AcsItemOutItemViewModel>;
        }

        private void SaveItemDataIntoCache(string tempDataId, List<AcsItemOutItemViewModel> model)
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
            cache.Add(key, model, policy);
        }

        [HttpPost]
        public ActionResult UpdateItemList(string tmpDataID, AcsItemOutItemViewModel model)
        {
            bool cached = false;
            var dataList = GetDataFromCache(tmpDataID, out cached);
            var dataToUpdate = dataList.FirstOrDefault(t => t.Seq == model.Seq);
            if (dataToUpdate != null)
            {
                dataToUpdate.ItemTypeID = model.ItemTypeID;
                dataToUpdate.ItemName = model.ItemName;
                dataToUpdate.Confidential = model.Confidential;
                dataToUpdate.Description = model.Description;
                dataToUpdate.RequestItemQty = model.RequestItemQty;
                dataToUpdate.PlanReturnDate = model.PlanReturnDate;
                dataToUpdate.ActualReturn = model.ActualReturn;
                dataToUpdate.ActualReturnQty = model.ActualReturnQty;
                dataToUpdate.ActualTakeOutQty = model.ActualTakeOutQty;

                if(model.ActualReturn != null || model.ActualReturnQty != null || model.ActualTakeOutQty != null)
                {
                    dataToUpdate.UpdateBy = User.Identity.Name;
                }
                else
                {
                    dataToUpdate.UpdateBy = model.UpdateBy;
                }

                SaveItemDataIntoCache(tmpDataID, dataList);
                return JsonNet(model, JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

        [HttpPost]
        public ActionResult DeleteItemList(string tmpDataID, AcsItemOutItemViewModel model)
        {
            bool cached = false;
            var dataList = GetDataFromCache(tmpDataID, out cached);
            var dataToDeleted = dataList.FirstOrDefault(t => t.Seq == model.Seq);
            if (dataToDeleted != null)
            {
                dataList.Remove(dataToDeleted);
                SaveItemDataIntoCache(tmpDataID, dataList);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

        [HttpPost]
        public ActionResult InsertAcsItemOut(AcsItemOutItemViewModel model)
        {
            return Ok(MessageHelper.SaveCompleted());
        }

        [SiteMapPageTitle("ACS040")]
        [ApplicationAuthorize("ACS040", PermissionNames.View)]
        public ActionResult Detail(string id)
        {
            var request = service.GetAcsItemOut(id, LoadAcsItemOutOptions.All);
            if (request == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", id)));
            }

            var viewModel = request.ToViewModel();
            viewModel.SuperiorEmployee = service.GetEmployeeInformation(viewModel.SuperiorApproveUserName).ToViewModel();
            viewModel.GAEmployee = service.GetEmployeeInformation(viewModel.GAApproveUserName).ToViewModel();
            if(viewModel.ForceDoneBy != null)
            {
                var result = service.GetEmployeeInformation(viewModel.ForceDoneBy);

                if(result != null)
                    viewModel.ForceDoneEmployee = result.ToViewModel();
            }
            
            return View(viewModel);
        }

        [SiteMapPageTitle("ACS040")]
        [ApplicationAuthorize("ACS040", PermissionNames.Edit)]
        public ActionResult Edit(string id)
        {
            var acs = service.GetAcsItemOut(id, LoadAcsItemOutOptions.All);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", id)));
            }

            var model = acs.ToViewModel();
            model.SuperiorEmployee = service.GetEmployeeInformation(model.SuperiorApproveUserName).ToViewModel();
            model.GAEmployee = service.GetEmployeeInformation(model.GAApproveUserName).ToViewModel();

            if(model.ForceDoneBy != null)
                model.ForceDoneEmployee = service.GetEmployeeInformation(model.ForceDoneBy).ToViewModel();

            var tmpData = new List<AcsItemOutItemViewModel>();
            var dataViews = service.GetAcsItemOutDetailDataViews(model.RequestNo);
            foreach (var dataView in dataViews)
            {
                dataView.Item = service.GetItem(dataView.ItemID);
                dataView.ItemType = service.GetMisc(dataView.Item.ItemTypeID);

                tmpData.Add(dataView.ToViewModel());
            }
            SaveItemDataIntoCache(model.TempDataId, tmpData);

            if (acs.CreateBy == User.Identity.Name && acs.Status == RequestStatus.Requesting)
            {
                // Author and request status is "Requesting"
                return View("EditForAuthor", model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [ApplicationSuspend]
        [ValidateAntiForgeryToken]
        public ActionResult EditAuthor(AcsItemOutViewModel viewModel)
        {
            viewModel.UpdateBy = User.Identity.Name;
            var entity = viewModel.ToEntity();

            //Edit update AcsItemOut. AcsItemOutDetail
            var result = service.UpdateAcsItemOutForAuthor(entity);
            if (!result.IsSucceed)
            {
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else
            {
                FlashMessage.Confirmation(MessageHelper.SaveCompleted());
            }

            return RedirectToAction("Detail", new { id = viewModel.RequestNo });
        }

        [HttpPost]
        [ApplicationSuspend]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("GAApproveUserName,CreateBy,TakeOutDate,Company,Department,TakeOutName")]
        public ActionResult Edit(AcsItemOutViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsItemOut(viewModel.RequestNo, LoadAcsItemOutOptions.All);
                if (acs == null)
                {
                    return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", viewModel.RequestNo)));
                }
                var entityToUpdated = viewModel.ToEntity();
                acs.Note = entityToUpdated.Note;
                acs.UpdateBy = User.Identity.Name;
                acs.AcsItemOutDetail = entityToUpdated.AcsItemOutDetail;

                var result = service.UpdateAcsItemOut(acs);
                if (result.IsSucceed)
                {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                    return RedirectToAction("Detail", new { id = viewModel.RequestNo });
                }
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState());
            }

            // Return to Edit View
            var employee = service.GetEmployeeInformation(viewModel.CreateBy) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ListAcsItemOutDetail([DataSourceRequest]DataSourceRequest request, string tmpDataID, string requestNo)
        {
            var tmpData = new List<AcsItemOutItemViewModel>();
            var dataViews = service.GetAcsItemOutDetailDataViews(requestNo) ;
            foreach( var dataView in dataViews)
            {
                dataView.Item = service.GetItem(dataView.ItemID);
                dataView.ItemType = service.GetMisc(dataView.Item.ItemTypeID);

                tmpData.Add(dataView.ToViewModel());
            }
            var result = dataViews.ToDataSourceResult(request, (AcsItemOutDetailDataView dataView) => dataView.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ListTempItemOutDetails([DataSourceRequest]DataSourceRequest request, string tmpDataID)
        {
            bool cached = false;
            var data = GetDataFromCache(tmpDataID, out cached)
                .OrderBy(t => t.Seq)
                .ToList();
            if (!cached)
            {
                try
                {
                    data = new List<AcsItemOutItemViewModel>();
                    SaveItemDataIntoCache(tmpDataID, data);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            var result = data.ToDataSourceResult(request);
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult CancelRequest(string id)
        {
            var acs = service.GetAcsItemOut(id, LoadAcsItemOutOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            return DoCancelRequest(acs, acs.Note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [ActionName("CancelRequestFromEdit")]
        public ActionResult CancelRequest(string id, string note)
        {
            var acs = service.GetAcsItemOut(id, LoadAcsItemOutOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            return DoCancelRequest(acs, note);
        }

        private ActionResult DoCancelRequest(AcsItemOut acs, string note)
        {
            var viewModel = acs.ToViewModel();
            if (viewModel.AllowCancelRequest(User))
            {
                acs.Note = note;
                acs.UpdateBy = User.Identity.Name;
                acs.Status = RequestStatus.Cancel;
                var result = service.UpdateAcsItemOut(acs);
                if (result.IsSucceed)
                {
                    var options = ApplicationContext.Setting.Task.ExportFileOptions;
                    var workflowResult = WorkflowManager.RunForCancelRequest(acs, options);
                    if (workflowResult.IsSucceed)
                        return Ok(MessageHelper.SaveCompleted());
                    else
                        return InternalServerError(result.GetErrorMessage());
                }
                else
                {
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
                };
            }
            // **** This message will revise later
            var message = MessageHelper.InvalidRequestForCancelRequest();
            return InternalServerError(MessageHelper.SaveFailed(message));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult Approval(ApprovalRequest data)
        {
            var acs = service.GetAcsItemOut(data.RequestNo, LoadAcsItemOutOptions.Approval);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            if (RequestStatus.IsCompleted(acs.Status))
            {
                return InternalServerError(MessageHelper.RequestAlreadyCompleted());
            }

            var approval = acs.ReqApproverList.FirstOrDefault(t => t.ApprovalID.ToString() == data.ApprovalID);
            if (approval == null)
            {
                return InternalServerError(MessageHelper.InvalidReqApprovalRequest());
            }

            // Modified Req Approval from request.
            approval.ApprovalCode = data.ApprovalCode;
            approval.ApprovalDate = DateTime.Now;
            approval.RejectReason = data.RejectReason;

            var result = service.UpdateReqApproverList(approval);
            if (result.IsSucceed)
            {
                // Invokie workflow
                var options = ApplicationContext.Setting.Task.ExportFileOptions;
                var workflowResult = WorkflowManager.RunForApprovalRequest(new WorkflowDataState(approval.ApprovalID,acs), options);
                if (workflowResult.IsSucceed)
                    return Ok(MessageHelper.SaveCompleted());
                else
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else
            {
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult ForceDone(string id)
        {
            var acs = service.GetAcsItemOut(id, LoadAcsItemOutOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound($"Request No: {id}"));
            }

            return DoForceDone(acs, acs.Note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [ActionName("ForceDoneFromEdit")]
        public ActionResult ForceDone(AcsPhotoViewModel model)
        {
            var acs = service.GetAcsItemOut(model.ReqNo, LoadAcsItemOutOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound($"Request No: {model.ReqNo}"));
            }

            var entity = model.ToEntity();
            return DoForceDone(acs, model.Note);
        }

        private ActionResult DoForceDone(AcsItemOut acs, string note)
        {
            var viewModel = acs.ToViewModel();
            if (!viewModel.AllowForceDone(User))
            {
                return InternalServerError(MessageHelper.InvalidRequestForForceDone());
            }
            acs.Note = note;
            acs.ForceDoneBy = User.Identity.Name;
            acs.ForceDoneDate = DateTime.Now;
            acs.Status = RequestStatus.Done;
            acs.UpdateBy = User.Identity.Name;

            var result = service.UpdateAcsItemOut(acs);
            if (result.IsSucceed)
            {
                return Ok(MessageHelper.SaveCompleted());
            }
            else
            {
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
        }

        [ApplicationAuthorize("ACS040", PermissionNames.Add)]
        [SiteMapPageTitle("ACS040")]
        public ActionResult Duplicate(string id)
        {

            var acs = service.GetAcsItemOut(id, LoadAcsItemOutOptions.All);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFound()));
            }

            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            acs.ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.ItemOut);
            acs.CreateBy = User.Identity.Name;
            acs.CreateDate = DateTime.Now;
            acs.Status = RequestStatus.Requesting;
            acs.RequestEmployee = employee;
            var model = acs.ToViewModel();

            var tmpData = new List<AcsItemOutItemViewModel>();
            var dataItemDetail = service.GetAcsItemOutDetailDataViews(id);
            foreach (var itemDetail in dataItemDetail)
            {
                itemDetail.Item = service.GetItem(itemDetail.ItemID);
                itemDetail.ItemType = service.GetMisc(itemDetail.Item.ItemTypeID);
                itemDetail.PlanReturnDate = null;
                itemDetail.ActOutQty = null;
                itemDetail.ActReturnDate = null;
                itemDetail.ActReturnQty = null;

                tmpData.Add(itemDetail.ToViewModel());
            }
            model.AcsItemOutDetails = tmpData;

            // Save item detail into cache for view in create action.
            SaveItemDataIntoCache(model.TempDataId, (model.AcsItemOutDetails).ToList());
            //model.ReferenceReqNo = id;
            return View("Create", model);
        }
    }
}