using CSI.ComponentModel;
using CSI.Exceptions;
using CSI.Localization;
using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using SECOM.ACS.Workflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class AcsVisitorController : AppControllerBase
    {
        private readonly IAccessControlService service;
        public AcsVisitorController(IAccessControlService service)
        {
            this.service = service;
        }

        [SiteMapPageTitle("ACS030")]
        [ApplicationAuthorize("ACS030",PermissionNames.Add)]
        public ActionResult Create()
        {
            var defaultEntryTime = service.GetDefaultEntryTime();
            var Visitor = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };

            var model = new AcsVisitorViewModel()
            {
                ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Visitor),
                CreateBy = User.Identity.Name,
                CreateDate = DateTime.Now,
                EntryDateFrom = DateTime.Now.AddDays(1),
                EntryTimeFrom = defaultEntryTime.EntryTimeFromTotalMinutes,
                EntryDateTo = DateTime.Now.AddDays(1),
                EntryTimeTo = defaultEntryTime.EntryTimeToTotalMinutes,
                Status = RequestStatus.Requesting,
                RequestEmployee = Visitor.ToViewModel()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("ReqNo,CreateDate,UpdateDate,Status")]
        [ApplicationSuspend]
        public ActionResult Create(AcsVisitorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.CreateBy = User.Identity.Name;
                viewModel.UpdateBy = User.Identity.Name;
                viewModel.Status = RequestStatus.Requesting;
                var entity = viewModel.ToEntity();
                var result = service.CreateAcsVisitor(entity);
                if (result.IsSucceed)
                {
                    var dataItem = result.DataState as AcsVisitor;
                    if (dataItem != null)
                    {
                        var message = String.Format("(Request No. {0})", dataItem.ReqNo);
                        FlashMessage.Confirmation(MessageHelper.SaveCompleted(message));
                    }
                    else
                    {
                        FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                    }

                    var workflowResult = WorkflowManager.RunForCreateRequest(entity);
                    if (!workflowResult.IsSucceed)
                    {
                        FlashMessage.Warning(MessageHelper.WorkflowError(workflowResult.Error));
                    }
                    return RedirectToAction("Detail", new { id = dataItem.ReqNo });
                }
                else
                {
                    FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
            }
            else
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
            }

            // Return to Create Action
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [SiteMapPageTitle("ACS030")]
        [ApplicationAuthorize("ACS030", PermissionNames.Edit)]
        public ActionResult Edit(string id)
        {
            var acs = service.GetAcsVisitor(id, LoadAcsVisitorOptions.All);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", id)));
            }

            if (acs.CreateBy == User.Identity.Name && acs.Status == RequestStatus.Requesting)
            {
                var model = acs.ToViewModel();
                SaveVisitorDataIntoCache(model.TempDataId, model.AcsVisitorDetails);
                return View("EditForAuthor", model);
            }
            else
            {
                var model = acs.ToViewModel();
                return View(model);
            }
        }

        [SiteMapPageTitle("ACS030")]
        [ApplicationAuthorize("ACS030", PermissionNames.View)]
        public ActionResult Detail(string id)
        {
            var request = service.GetAcsVisitor(id, LoadAcsVisitorOptions.All);
            if (request == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", id)));
            }

            var viewModel = request.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("AreaApprovalVisitorID,CreateDate,UpdateDate,Areas,PurposeCodeID,SuperiorApproveUserName")]
        [ApplicationSuspend]
        public ActionResult Edit(AcsVisitorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsVisitor(viewModel.ReqNo, LoadAcsVisitorOptions.All);
                if (acs == null)
                {
                    return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", viewModel.ReqNo)));
                }

                // Modified data
                var entityToUpdated = viewModel.ToEntity();
                acs.Note = entityToUpdated.Note;
                acs.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsVisitor(acs);
                if (result.IsSucceed)
                {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                    return RedirectToAction("Detail", new { id = viewModel.ReqNo });
                }
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
            }

            // Return to Edit View
            var employee = service.GetEmployeeInformation(viewModel.CreateBy) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("CreateDate,UpdateDate,Areas,SuperiorApproveUserName")]
        [ApplicationSuspend]
        public ActionResult EditForAuthor(AcsVisitorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsVisitor(viewModel.ReqNo, LoadAcsVisitorOptions.All);
                if (acs == null)
                {
                    return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", viewModel.ReqNo)));
                }

                if (acs.Status != RequestStatus.Requesting)
                {
                    return View("_Message", MessageViewModel.Error("Request could not modified. Current Status: {0}", AcsModelHelper.GetRequestStatusName(acs.Status)));
                }
                // Modified data
                var entityToUpdated = viewModel.ToEntity();
                entityToUpdated.UpdateBy = User.Identity.Name;
                //acs.EntryDateFrom = entityToUpdated.EntryDateFrom;
                //acs.EntryTimeFrom = entityToUpdated.EntryTimeFrom;
                //acs.EntryDateTo = entityToUpdated.EntryDateTo;
                //acs.EntryTimeTo = entityToUpdated.EntryTimeTo;
                //acs.Note = entityToUpdated.Note;
                //acs.PurposeCodeID = entityToUpdated.PurposeCodeID;
                //acs.OtherPurpose = entityToUpdated.OtherPurpose;
                //acs.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsVisitorForAuthor(entityToUpdated);
                if (result.IsSucceed)
                {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                    return RedirectToAction("Detail", new { id = viewModel.ReqNo });
                }
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
            }

            // Return to Edit View
            var Visitor = service.GetEmployeeInformation(viewModel.CreateBy) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = Visitor.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult CancelRequest(string requestNo)
        {
            var acs = service.GetAcsVisitor(requestNo, LoadAcsVisitorOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            return DoCancelRequest(acs, acs.Note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [ActionName("CancelRequestForEdit")]
        public ActionResult CancelRequest(AcsVisitorViewModel model)
        {
            var acs = service.GetAcsVisitor(model.ReqNo, LoadAcsVisitorOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            return DoCancelRequest(acs, model.Note);
        }

        private ActionResult DoCancelRequest(AcsVisitor acs, string note)
        {
            var viewModel = acs.ToViewModel();
            if (viewModel.AllowCancelRequest(User))
            {
                acs.Note = note;
                acs.UpdateBy = User.Identity.Name;
                acs.Status = RequestStatus.Cancel;
                var result = service.UpdateAcsVisitor(acs);
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

        [SiteMapPageTitle("ACS030")]
        [ApplicationAuthorize("ACS030", PermissionNames.Add)]
        public ActionResult Duplicate(string id)
        {

            var acs = service.GetAcsVisitor(id, LoadAcsVisitorOptions.All);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFound()));
            }

            var defaultEntryTime = service.GetDefaultEntryTime();
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            acs.ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Visitor);
            acs.EntryDateFrom = DateTime.Now.AddDays(1);
            acs.EntryTimeFrom = defaultEntryTime.EntryTimeFromTimespan;
            acs.EntryDateTo = DateTime.Now.AddDays(1);
            acs.EntryTimeTo = defaultEntryTime.EntryTimeToTimespan;
            acs.CreateBy = User.Identity.Name;
            acs.CreateDate = DateTime.Now;
            acs.Status = RequestStatus.Requesting;
            acs.RequestEmployee = employee;
            var model = acs.ToViewModel();
            // Save employee detail into cache for view in create action.
            SaveVisitorDataIntoCache(model.TempDataId, model.AcsVisitorDetails);
            model.ReferenceReqNo = id;
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult Approval(ApprovalRequest data)
        {
            var acs = service.GetAcsVisitor(data.RequestNo, LoadAcsVisitorOptions.Approval | LoadAcsVisitorOptions.VisitorDetail);
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
            approval.RejectReason = data.ApprovalCode == ApprovalCode.Approve ? "" : data.RejectReason;

            var result = service.UpdateReqApproverList(approval);
            if (result.IsSucceed)
            {
                // Invokie workflow
                var options = ApplicationContext.Setting.Task.ExportFileOptions;
                var workflowResult = WorkflowManager.RunForApprovalRequest(new WorkflowDataState(approval.ApprovalID,acs),options);
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

       

        #region Visitor
        [OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        public ActionResult ListTempVisitor([DataSourceRequest]DataSourceRequest request, string tempDataId)
        {
            bool cached = false;
            var miscs = service.GetMiscsByType(MiscTypes.VerifyType);
            var data = GetVisitorDataFromCache(tempDataId, out cached)
                .OrderBy(t => t.Seq)               
                .ToList();

            data.Each((AcsVisitorDetailViewModel viewModel) =>
            {
                var misc = miscs.FirstOrDefault(t => t.MiscID == viewModel.VerifyTypeID);
                viewModel.VerifyType = misc==null? "": ModelLocalizeManager.GetValue(misc, "MiscDisplay");
            });

            if (!cached && !String.IsNullOrEmpty(tempDataId))
            {
                try
                {
                    data = new List<AcsVisitorDetailViewModel>();
                    SaveVisitorDataIntoCache(tempDataId, data);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }


            var result = data.ToDataSourceResult(request);            
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        private List<AcsVisitorDetailViewModel> GetVisitorDataFromCache(string tempDataId, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<AcsVisitorDetailViewModel>();
            }
            return cache[key] as List<AcsVisitorDetailViewModel>;
        }

        private void SaveVisitorDataIntoCache(string tempDataId, IList<AcsVisitorDetailViewModel> model)
        {
            model.OrderBy(t => t.Seq).ToList();
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
        public ActionResult CreateTempVisitor(string tempDataId, AcsVisitorDetailViewModel model)
        {
            bool cached = false;
            var data = GetVisitorDataFromCache(tempDataId, out cached);
            var maxSeq = data.Count() == 0 ? 0 : data.Max(t => t.Seq);
            // Validate data
            model.DetailID = model.DetailID == Guid.Empty ? Guid.NewGuid() : model.DetailID;
            model.Seq = maxSeq + 1;
            data.Add(model);
            SaveVisitorDataIntoCache(tempDataId, data);
            return JsonNet(model,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateTempVisitor(string tempDataId, AcsVisitorDetailViewModel model)
        {
            bool cached = false;
            var data = GetVisitorDataFromCache(tempDataId, out cached);
            var dataToUpdated = data.FirstOrDefault(t => t.DetailID == model.DetailID);
            if (dataToUpdated == null) {
                return InternalServerError("Update visitor fail. Data not found.");
            }

            // Modifield
            dataToUpdated.VerifyTypeID = model.VerifyTypeID;
            dataToUpdated.VerifyNo = model.VerifyNo;
            dataToUpdated.VisitorName = model.VisitorName;
            dataToUpdated.Company = model.Company;
            dataToUpdated.DepartmentName = model.DepartmentName;
            dataToUpdated.Photographing = model.Photographing;
            dataToUpdated.ItemInOut = model.ItemInOut;
            dataToUpdated.Description = model.Description;
            SaveVisitorDataIntoCache(tempDataId, data);
            return JsonNet(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteTempVisitor(string tempDataId, AcsVisitorDetailViewModel model)
        {
            bool cached = false;
            var dataList = GetVisitorDataFromCache(tempDataId, out cached);
            var dataToDeleted = dataList.FirstOrDefault(t => t.DetailID == model.DetailID);
            if (dataToDeleted != null)
            {
                dataList.Remove(dataToDeleted);
                SaveVisitorDataIntoCache(tempDataId, dataList);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ClearTempVisitor(string tempDataId)
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

        #region Import Visitor
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            var dataItems = new List<AcsVisitorImportData>();
            var errors = new ObjectResults<string[]>();
            var miscs = service.GetMiscsByType(MiscTypes.VerifyType).ToList();

            foreach (var f in files)
            {
                using (var fs = new StreamReader(f.InputStream))
                {
                    using (var reader = new CsvHelper.CsvReader(fs))
                    {
                        reader.Configuration.RegisterClassMap<AcsVisitorImportDataMap>();
                        reader.Configuration.HasHeaderRecord = true;
                        reader.Configuration.Delimiter = "|";
                        while (reader.Read())
                        {
                            try
                            {
                                var data = reader.GetRecord<AcsVisitorImportData>();
                                dataItems.Add(data);
                            }
                            catch (Exception ex)
                            {
                                errors.AddResult(reader.CurrentRecord, ex);
                            }

                        }
                    }
                }
            }
            // Set Verify Type
            dataItems.Each((AcsVisitorImportData data) =>
            {
                var misc = miscs.Find(t => t.MiscID == data.VerifyTypeID);
                data.VerifyType = misc == null ? String.Empty : misc.MiscDisplayEN;
            });
            return JsonNet(new { Data = dataItems, Errors = errors.ToListResult() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(IList<AcsVisitorImportData> data, string tempDataId, string requestFor)
        {
            var results = new ObjectResults<AcsVisitorImportData>();
            bool cached = false;
            var dataItems = GetVisitorDataFromCache(tempDataId, out cached);
            var maxSeq = dataItems.Count() == 0 ? 0 : dataItems.Max(t => t.Seq);
           
            foreach (var d in data)
            {
                results.AddResult(d);
                var viewModel = d.ToViewModel();
                viewModel.Seq = ++maxSeq;
                dataItems.Add(viewModel);
            }
            SaveVisitorDataIntoCache(tempDataId, dataItems);
            return JsonNet(results.ToResult(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public ActionResult ListVisitorDetail([DataSourceRequest]DataSourceRequest request, string requestNo)
        {
            var dataViews = service.GetAcsVisitorDetailDataViews(requestNo);
            var result = dataViews.ToDataSourceResult(request, (AcsVisitorDetailDataView d) => d.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        #region Transaction
        [HttpPost]
        public ActionResult ListTransaction([DataSourceRequest]DataSourceRequest request, string requestNo)
        {
            var dataItems = service.GetAcsVisitorTransactionDataViews(requestNo);
            var result = dataItems.ToDataSourceResult(request,(AcsVisitorTransactionDataView entity)=> entity.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}