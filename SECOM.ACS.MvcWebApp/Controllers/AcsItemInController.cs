using CSI.Localization;
using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Json;
using SECOM.ACS.Mail.Models;
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
using System.Web.Hosting;
using System.Web.Mvc;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class AcsItemInController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public AcsItemInController(IAccessControlService service)
        {
            this.service = service;
        }

        [SiteMapPageTitle("ACS050")]
        [ApplicationAuthorize("ACS050", PermissionNames.Add)]
        public ActionResult Create()
        {
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            var model = new AcsItemInViewModel()
            {
                CreateBy = User.Identity.Name,
                CreateDate = DateTime.Now,
                TakeInDate = DateTime.Now.AddDays(1),
                Status = RequestStatus.Requesting,
                ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.ItemIn),
                RequestEmployee = employee.ToViewModel()
            };          
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [IgnoreModelErrors("ReqNo,CreateDate,UpdateDate")]
        public ActionResult Create(AcsItemInViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.CreateBy = User.Identity.Name;
                viewModel.UpdateBy = User.Identity.Name;
                var entity = viewModel.ToEntity();                
                entity.Status = RequestStatus.Requesting;
                var result = service.CreateAcsItemIn(entity);
                if (result.IsSucceed)
                {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                    var workflowResult = WorkflowManager.RunForCreateRequest(entity);
                    if (!workflowResult.IsSucceed)
                    {
                        FlashMessage.Warning(MessageHelper.WorkflowError(workflowResult.Error));
                    }
                    return RedirectToAction("Detail", new { id = entity.ReqNo });
                }
                else
                {
                    FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
            }
            else {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
            }
            // Return to Create Action
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult CancelRequest(string reqNo)
        {
            var acs = service.GetAcsItemIn(reqNo, LoadAcsItemInOptions.None);
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
        public ActionResult CancelRequest(AcsItemInViewModel model)
        {
            var acs = service.GetAcsItemIn(model.ReqNo, LoadAcsItemInOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }
            return DoCancelRequest(acs, model.Note);
        }

        private ActionResult DoCancelRequest(AcsItemIn acs, string note)
        {
            var viewModel = acs.ToViewModel();
            if (viewModel.AllowCancelRequest(User))
            {
                acs.Note = note;
                acs.UpdateBy = User.Identity.Name;
                acs.Status = RequestStatus.Cancel;
                var result = service.UpdateAcsItemIn(acs);
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
        public ActionResult ForceDone(string id)
        {
            var acs = service.GetAcsItemIn(id, LoadAcsItemInOptions.None);
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
        public ActionResult ForceDone(AcsItemInViewModel model)
        {
            var acs = service.GetAcsItemIn(model.ReqNo, LoadAcsItemInOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound($"Request No: {model.ReqNo}"));
            }
            return DoForceDone(acs, model.Note);
        }

        private ActionResult DoForceDone(AcsItemIn acs, string note)
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

            var result = service.UpdateAcsItemIn(acs);
            if (result.IsSucceed)
            {
                return Ok(MessageHelper.SaveCompleted());
            }
            else
            {
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
        }

        [SiteMapPageTitle("ACS050")]
        [ApplicationAuthorize("ACS050", PermissionNames.View)]
        public ActionResult Detail(string id)
        {
            var request = service.GetAcsItemIn(id,LoadAcsItemInOptions.Header);
      
            if (request == null)
            {
                return View("_Message", MessageViewModel.Error("Request Item-In data not found."));
            }

            var model = request.ToViewModel();
            return View(model);
        }

        [SiteMapPageTitle("ACS050")]
        [ApplicationAuthorize("ACS050", PermissionNames.Edit)]
        public ActionResult Edit(string id)
        {
            var request = service.GetAcsItemIn(id, LoadAcsItemInOptions.All);
            if (request == null)
            {
                return View("_Message", MessageViewModel.Error("Request Item-In data not found."));
            }

            if(request.CreateBy == User.Identity.Name && request.Status == RequestStatus.Requesting )
            {
               
                var model = request.ToViewModel();
                var data = service.GetAcsItemInDetailDataViews(model.ReqNo).Select(t=>t.ToViewModel()).ToList();
                SaveDataIntoCache(model.TempDataId, data);
                return View("EditForAuthor",model);
            }
            else
            {
                var model = request.ToViewModel();
                var data = service.GetAcsItemInDetailDataViews(model.ReqNo).Select(t => t.ToViewModel()).ToList();
                SaveDataIntoCache(model.TempDataId, data);
                return View("Edit", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [IgnoreModelErrors("SuperiorApproveUserName,AreaApproveUserName,AcknowledgeUserName")]
        public ActionResult Edit(AcsItemInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
                return RedirectToAction("Edit", new { id = viewModel.ReqNo });
            }

            var acs = service.GetAcsItemIn(viewModel.ReqNo, LoadAcsItemInOptions.None);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error("Request Item-In data not found."));
            }
            viewModel.UpdateBy = User.Identity.Name;
            viewModel.UpdateDate = DateTime.Now;
            var entity = viewModel.ToEntity();
            acs.Note = entity.Note;
            acs.PurposeCodeID = entity.PurposeCodeID;
            acs.OtherPurpose = entity.OtherPurpose;
            acs.TakeInName = entity.TakeInName;
            acs.TakeInDate = entity.TakeInDate;
            acs.Company = entity.Company;
            acs.DeptName = entity.DeptName;
            acs.UpdateBy = User.Identity.Name;
           
            foreach(var data in entity.AcsItemInDetails.ToList())
            {
            acs.AcsItemInDetails.Add(data);
            }
            var result = service.UpdateAcsItemInForAuthor(acs);
            if (!result.IsSucceed)
            {
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else {
                FlashMessage.Confirmation(MessageHelper.SaveCompleted());
            }            
            return RedirectToAction("Detail", new { id = viewModel.ReqNo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [IgnoreModelErrors("SuperiorApproveUserName,AreaApproveUserName,AcknowledgeUserName")]
        public ActionResult EditForOther(AcsItemInViewModel viewModel)
        {
           

            var acs = service.GetAcsItemIn(viewModel.ReqNo, LoadAcsItemInOptions.None);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error("Request Item-In data not found."));
            }
            viewModel.ReqNo = acs.ReqNo;
            viewModel.UpdateBy = User.Identity.Name;
            viewModel.UpdateDate = DateTime.Now;
            var entity = viewModel.ToEntity();
            if (acs.Status == RequestStatus.Approved)
            {
         
                var result1 = service.UpdateAcsItemInDetail(entity.AcsItemInDetails.ToList());
                if (!result1.IsSucceed)
                {
                    FlashMessage.Danger(MessageHelper.SaveFailed(result1.GetErrorMessage()));
                }
                //else
                //{
                //    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                //}
            }
            
               
                acs.Note = entity.Note;
                acs.UpdateBy = User.Identity.Name;
                acs.UpdateDate = DateTime.Now;
                var result = service.UpdateAcsItemIn(acs);
                if (!result.IsSucceed)
                {
                    FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
                else
                {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                }

       
           
            return RedirectToAction("Detail", new { id = viewModel.ReqNo });
        }

        [ApplicationAuthorize("ACS050", PermissionNames.Add)]
        [SiteMapPageTitle("ACS050")]
        public ActionResult Duplicate(string id)
        {

            var acs = service.GetAcsItemIn(id, LoadAcsItemInOptions.All);
            if (acs == null)
            {
                var model = MessageViewModel.Error(MessageHelper.DataNotFound());
                return View("_Message", model);
            }

            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            acs.ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Employee);
            acs.CreateBy = User.Identity.Name;
            acs.CreateDate = DateTime.Now;
            acs.Status = RequestStatus.Requesting;
            acs.RequestEmployee = employee;
           
            var viewModel  = acs.ToViewModel();
            var data = service.GetAcsItemInDetailDataViews(id).Select(t=>t.ToViewModel()).ToList();
            foreach(var item in data)
            {
                item.ActualTakeInQty = null;
                item.ActualReturnDate = null;
                item.PlanReturnDate = null;
                item.ActualReturnDate = null;
            }
            SaveDataIntoCache(viewModel.TempDataId, data);
            return View("Create", viewModel);
        }

        [HttpPost]
        public ActionResult ListItemInDetails([DataSourceRequest]DataSourceRequest request, string reqNo)
        {
            var data = service.GetAcsItemInDetailDataViews(reqNo);
            var result = data.ToDataSourceResult(request, (AcsItemInDetailDataView dataView) => dataView.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult Approval(ApprovalRequest data)
        {
            var acs = service.GetAcsItemIn(data.RequestNo, LoadAcsItemInOptions.Approval);
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
                var workflowResult = WorkflowManager.RunForApprovalRequest(new WorkflowDataState(approval.ApprovalID, acs), options);
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
        public ActionResult Acknowledge(string id)
        {
            var acs = service.GetAcsItemIn(id, LoadAcsItemInOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            if (acs.Status != RequestStatus.Approving)
            {
                var message = MessageHelper.InvalidRequestStatusForGA();
                return InternalServerError(MessageHelper.SaveFailed(message));
            }

            // Modified AcsPhoto for Acknowlege from GA.
            acs.AckBy = User.Identity.Name;
            acs.AckDate = DateTime.Now;
            acs.UpdateBy = User.Identity.Name;
            acs.UpdateDate = DateTime.Now;
            acs.Status = RequestStatus.Approved;
            var result = service.UpdateAcsItemIn(acs);
            if (result.IsSucceed)
            {
                var employee = service.GetEmployeeInformation(acs.CreateBy);
                SendRequestApprovedMail(employee, acs);
                return Ok(MessageHelper.SaveCompleted());
            }
            else
            {
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
        }

        #region Temp ItemIn
        [HttpPost]
        public ActionResult ListTempItemInDetails([DataSourceRequest]DataSourceRequest request, string tmpDataID)
        {
            bool cached = false;
            var data = GetDataFromCache(tmpDataID, out cached)
                .OrderBy(t => t.Seq)
                .ToList();
            if (!cached)
            {
                try
                {
                    data = new List<AcsItemInDetailViewModel>();
                    SaveDataIntoCache(tmpDataID, data);
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
        public ActionResult CreateTempItemList(string tmpDataID, AcsItemInDetailViewModel model)
        {
            bool cached = false;
            var data = GetDataFromCache(tmpDataID, out cached);
            var checkDuplicate = data.FirstOrDefault(t => t.ItemID == model.ItemID);
            if (checkDuplicate == null)
            {
                var item = service.GetItem(model.ItemID);
                if (item == null) { return InternalServerError(MessageHelper.DataNotFound($"Item ID {model.ItemID} not found.")); }

                var misc = service.GetMisc(model.ItemTypeID);
                if (misc == null) { return InternalServerError(MessageHelper.DataNotFound($"Item Type ID {model.ItemTypeID} not found.")); }
                model.ItemTypeName = ModelLocalizeManager.GetValue(misc, "MiscDisplay");
                model.ItemName = ModelLocalizeManager.GetValue(item, "ItemDisplay");
                model.Seq = (Int16)(data.Count() == 0 ? 1 : data.Max(t => t.Seq) + 1);
                model.UpdateBy = User.Identity.Name;
                data.Add(model);
                SaveDataIntoCache(tmpDataID, data);
                return JsonNet(model, JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DuplicateInList(checkDuplicate.ItemName));
        }

        private List<AcsItemInDetailViewModel> GetDataFromCache(string tmpDataID, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tmpDataID.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<AcsItemInDetailViewModel>();
            }
            return cache[key] as List<AcsItemInDetailViewModel>;
        }

        private void SaveDataIntoCache(string tempDataId, List<AcsItemInDetailViewModel> model)
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
        public ActionResult UpdateTempItemList(string tmpDataID,AcsItemInDetailViewModel model)
        {
            bool cached = false;
            var dataList = GetDataFromCache(tmpDataID, out cached);
            var dataToUpdate = dataList.FirstOrDefault(t => t.DetailID == model.DetailID);
           
            if (dataToUpdate != null)
            {
                var item = service.GetItem(model.ItemID);
                if (item == null) { return InternalServerError(MessageHelper.DataNotFound($"Item Id {model.ItemID} not found.")); }
                var misc = service.GetMisc(model.ItemTypeID);
                if (misc == null) { return InternalServerError(MessageHelper.DataNotFound($"Item Type Id {model.ItemTypeID} not found.")); }

                dataToUpdate.ItemTypeID = model.ItemTypeID;
                dataToUpdate.ItemTypeName = ModelLocalizeManager.GetValue(misc, "MiscDisplay");
                dataToUpdate.ItemName = ModelLocalizeManager.GetValue(item, "ItemDisplay");
                dataToUpdate.Confidential = model.Confidential;
                dataToUpdate.Description = model.Description;
                dataToUpdate.RequestItemQty = model.RequestItemQty;
                dataToUpdate.PlanReturnDate = model.PlanReturnDate;
                dataToUpdate.ActualReturnDate = model.ActualReturnDate;
                dataToUpdate.ActualReturnQty = model.ActualReturnQty;
                dataToUpdate.ActualTakeInQty = model.ActualTakeInQty;
                dataToUpdate.UpdateBy = User.Identity.Name;
                SaveDataIntoCache(tmpDataID, dataList);
                return JsonNet(model, JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

        [HttpPost]
        public ActionResult DeleteTempItemList(string tmpDataID, AcsItemInDetailViewModel model)
        {
            bool cached = false;
            var dataList = GetDataFromCache(tmpDataID, out cached);
            var dataToDeleted = dataList.FirstOrDefault(t => t.ItemID == model.ItemID);
            if (dataToDeleted != null)
            {
                dataList.Remove(dataToDeleted);
                SaveDataIntoCache(tmpDataID, dataList);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }
        #endregion

        protected void SendRequestApprovedMail(EmployeeInformation employee, IAcsRequest request)
        {
            var documentType = service.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var model = new RequestApprovedMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            this.MailManager.SendRequestApproved(model, new string[] { employee.Email });
        }
    }
}