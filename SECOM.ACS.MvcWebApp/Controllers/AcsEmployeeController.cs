using CSI.ComponentModel;
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
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class AcsEmployeeController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public AcsEmployeeController(IAccessControlService service)
        {
            this.service = service;
        }
       

        [ApplicationAuthorize("ACS020", PermissionNames.Add)]
        [SiteMapPageTitle("ACS020")]
        public ActionResult Create()
        {
            var defaultEntryTime = service.GetDefaultEntryTime();
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };

            var model = new AcsEmployeeViewModel()
            {
                ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Employee),
                RequestFor = RequestFors.Employee,
                CreateBy = User.Identity.Name,
                CreateDate = DateTime.Now,
                EntryDateFrom = DateTime.Now.AddDays(1),
                EntryTimeFrom = defaultEntryTime.EntryTimeFromTotalMinutes,
                EntryDateTo = DateTime.Now.AddDays(1),
                EntryTimeTo = defaultEntryTime.EntryTimeToTotalMinutes,
                Status = RequestStatus.Requesting,
                RequestEmployee = employee.ToViewModel()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [IgnoreModelErrors("ReqNo,CreateDate,UpdateDate,Status")]
        public ActionResult Create(AcsEmployeeViewModel viewModel)
        {
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };

            if (!ModelState.IsValid)
            {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
                viewModel.RequestEmployee = employee.ToViewModel();
                return View(viewModel);
            }

            viewModel.CreateBy = User.Identity.Name;
            viewModel.UpdateBy = User.Identity.Name;
            viewModel.Status = RequestStatus.Requesting;
            var entity = viewModel.ToEntity();

            var result = service.CreateAcsEmployee(entity);
            if (result.IsSucceed)
            {
                var dataItem = result.DataState as AcsEmployee;
                if (dataItem != null)
                {
                    var message = String.Format("(Request No. {0})", dataItem.ReqNo);
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted(message));
                }
                else {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                }

                var workflowResult = WorkflowManager.RunForCreateRequest(entity);
                if (!workflowResult.IsSucceed)
                {
                    FlashMessage.Warning(MessageHelper.WorkflowError(workflowResult.Error));
                }
                return RedirectToAction("Detail",new { id = dataItem.ReqNo });
            }
            else
            {
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
                // Return to Create Action
                viewModel.RequestEmployee = employee.ToViewModel();
                return View(viewModel);
            }
        }

        [ApplicationAuthorize("ACS020", PermissionNames.View)]
        [SiteMapPageTitle("ACS020")]
        public ActionResult Detail(string id)
        {
            var request = service.GetAcsEmployee(id, LoadAcsEmployeeOptions.Header | LoadAcsEmployeeOptions.Approval);
            if (request == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", id)));
            }

            var viewModel = request.ToViewModel();
            return View(viewModel);
        }

        [ApplicationAuthorize("ACS020", PermissionNames.Edit)]
        [SiteMapPageTitle("ACS020")]
        public ActionResult Edit(string id)
        {
            var acs = service.GetAcsEmployee(id, LoadAcsEmployeeOptions.All);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", id)));
            }

            if (acs.CreateBy == User.Identity.Name && acs.Status == RequestStatus.Requesting)
            {
                // Author and request status is "Requesting"
                var model = acs.ToViewModel();
                SaveEmployeeDataIntoCache(model.TempDataId, model.AcsEmployeeDetails);
                return View("EditForAuthor",model);
            }
            else {
                var model = acs.ToViewModel();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("CreateDate,UpdateDate,RequestFor,Areas,SuperiorApproveUserName")]
        [ApplicationSuspend]
        public ActionResult Edit(AcsEmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsEmployee(viewModel.ReqNo, LoadAcsEmployeeOptions.All);
                if (acs == null)
                {
                    return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", viewModel.ReqNo)));
                }

                // Modified data
                var entity = viewModel.ToEntity();
                acs.Note = entity.Note;
                acs.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsEmployeeForAuthor(acs);
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

            var employee = service.GetEmployeeInformation(viewModel.CreateBy) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("CreateDate,UpdateDate,RequestFor,Areas,SuperiorApproveUserName")]
        [ApplicationSuspend]
        public ActionResult EditForAuthor(AcsEmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsEmployee(viewModel.ReqNo, LoadAcsEmployeeOptions.All);
                if (acs == null)
                {
                    return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", viewModel.ReqNo)));
                }

                if (acs.Status != RequestStatus.Requesting)
                {
                    return View("_Message", MessageViewModel.Error("Request could not modified. Current Status: {0}", AcsModelHelper.GetRequestStatusName(acs.Status)));
                }

                // Modified data
                var entity = viewModel.ToEntity();
                entity.UpdateBy = User.Identity.Name;
                //acs.EntryDateFrom = entity.EntryDateFrom;
                //acs.EntryTimeFrom = entity.EntryTimeFrom;
                //acs.EntryDateTo = entity.EntryDateTo;
                //acs.EntryTimeTo = entity.EntryTimeTo;
                //acs.Note = entity.Note;
                //acs.PurposeCodeID = entity.PurposeCodeID;
                //acs.OtherPurpose = entity.OtherPurpose;
                //acs.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsEmployeeForAuthor(entity);
                if (result.IsSucceed)
                {
                    FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                    return RedirectToAction("Detail", new { id = viewModel.ReqNo });
                }
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
            else {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
            }

            var employee = service.GetEmployeeInformation(viewModel.CreateBy) ?? new EmployeeInformation() { };
            viewModel.RequestEmployee = employee.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult CancelRequest(string requestNo)
        {
            var acs = service.GetAcsEmployee(requestNo, LoadAcsEmployeeOptions.None);
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
        public ActionResult CancelRequest(AcsEmployeeViewModel model)
        {
            var acs = service.GetAcsEmployee(model.ReqNo, LoadAcsEmployeeOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }
          
            return DoCancelRequest(acs, model.Note);
        }

        private ActionResult DoCancelRequest(AcsEmployee acs, string note)
        {
            var viewModel = acs.ToViewModel();
            if (viewModel.AllowCancelRequest(User))
            {
                acs.Note = note;
                acs.UpdateBy = User.Identity.Name;
                acs.Status = RequestStatus.Cancel;
                var result = service.UpdateAcsEmployee(acs);
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
            var acs = service.GetAcsEmployee(data.RequestNo, LoadAcsEmployeeOptions.Approval | LoadAcsEmployeeOptions.EmployeeDetail);
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
            approval.RejectReason = data.ApprovalCode == ApprovalCode.Approve?"": data.RejectReason;
            
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

        [ApplicationAuthorize("ACS020", PermissionNames.Add)]
        [SiteMapPageTitle("ACS020")]
        public ActionResult Duplicate(string id)
        {

            var acs = service.GetAcsEmployee(id, LoadAcsEmployeeOptions.All);
            if (acs == null) {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFound()));
            }

            var defaultEntryTime = service.GetDefaultEntryTime();
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            acs.ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Employee);
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
            SaveEmployeeDataIntoCache(model.TempDataId, model.AcsEmployeeDetails);
            model.ReferenceReqNo = id;
            return View("Create",model);
        }

        #region Temp Employee
        [OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        public ActionResult GetTempEmployee([DataSourceRequest]DataSourceRequest request, string tempDataId)
        {
            bool cached = false;
            var data = GetEmployeeDataFromCache(tempDataId, out cached)
                .OrderBy(t => t.Seq)
                .ToList();
            if (!cached && !String.IsNullOrEmpty(tempDataId))
            {
                try
                {
                    data = new List<AcsEmployeeDetailViewModel>();
                    SaveEmployeeDataIntoCache(tempDataId, data);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            var result = data.ToDataSourceResult(request);
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        private List<AcsEmployeeDetailViewModel> GetEmployeeDataFromCache(string tempDataId, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<AcsEmployeeDetailViewModel>();
            }
            return cache[key] as List<AcsEmployeeDetailViewModel>;
        }

        private void SaveEmployeeDataIntoCache(string tempDataId, IList<AcsEmployeeDetailViewModel> model)
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
        public ActionResult CreateTempEmployee(string tempDataId, AcsEmployeeDetailViewModel model)
        {
            bool cached = false;
            var data = GetEmployeeDataFromCache(tempDataId, out cached);
            var maxSeq = data.Count() == 0 ? 0 : data.Max(t => t.Seq);

            if (String.IsNullOrEmpty(model.EmployeeID))
            {
                // From business Trip
                if (data.Any(t => String.Compare(t.EmployeeName, model.EmployeeName, true) == 0))
                {
                    return InternalServerError(MessageHelper.DuplicateField("Employee Name", model.EmployeeName));
                }
                
                model.EmployeeName = model.EmployeeName;
                model.DepartmentName = model.DepartmentName;
                model.Seq = maxSeq + 1;
                data.Add(model);
                SaveEmployeeDataIntoCache(tempDataId, data);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            else
            {
                // From employee
                var findEmployee = service.GetEmployeeInformation(model.EmployeeID);
                if (findEmployee == null)
                {
                    return InternalServerError(MessageHelper.DataNotFound("Employee data not found."));
                }

                var findItem = data.Find(t => String.Compare(t.EmployeeID, model.EmployeeID, true) == 0);
                if (findItem != null)
                {
                    return InternalServerError(MessageHelper.DuplicateField("Employee Name", findItem.EmployeeName));
                }

                var dataView = findEmployee.ToViewModel();
                model.EmployeeName = dataView.EmployeeName;
                model.DepartmentName = dataView.Department;
                model.Seq = maxSeq + 1;
                data.Add(model);
                SaveEmployeeDataIntoCache(tempDataId, data);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateTempEmployee(string tempDataId, AcsEmployeeDetailViewModel model)
        {
            bool cached = false;
            var dataList = GetEmployeeDataFromCache(tempDataId, out cached);
            if (String.IsNullOrEmpty(model.EmployeeID))
            {
                // Bussiness Trip
                var dataToUpdated = dataList.FirstOrDefault(t => t.DetailID == model.DetailID);
                if (dataToUpdated != null)
                {
                    dataToUpdated.EmployeeName = model.EmployeeName;
                    dataToUpdated.DepartmentName = model.DepartmentName;
                    SaveEmployeeDataIntoCache(tempDataId, dataList);
                    return JsonNet(model, JsonRequestBehavior.AllowGet);
                }
                return InternalServerError(MessageHelper.DataNotFound());
            }
            else {
                // Employee
                var findDuplicate = dataList.FirstOrDefault(t => t.EmployeeID == model.EmployeeID && t.DetailID != model.DetailID);
                if (findDuplicate != null)
                {
                    return InternalServerError(MessageHelper.DuplicateField("Employee Name", findDuplicate.EmployeeID));
                }

                var dataToUpdated = dataList.FirstOrDefault(t => t.DetailID == model.DetailID);
                if (dataToUpdated != null)
                {
                    var employee = service.GetEmployeeInformation(model.EmployeeID);
                    if (employee == null) { return InternalServerError(MessageHelper.DataNotFoundFormat($"Employee ID: {model.EmployeeID} data not found.")); }
                    var employeeViewModel = employee.ToViewModel();
                    dataToUpdated.EmployeeID = model.EmployeeID;
                    dataToUpdated.EmployeeName = employeeViewModel.EmployeeName;
                    dataToUpdated.DepartmentName = employeeViewModel.Department;
                    SaveEmployeeDataIntoCache(tempDataId, dataList);
                    return JsonNet(model, JsonRequestBehavior.AllowGet);
                }
                return InternalServerError(MessageHelper.DataNotFound());
            }            
        }

        [HttpPost]
        public ActionResult DeleteTempEmployee(string tempDataId, AcsEmployeeDetailViewModel model)
        {
            bool cached = false;
            var dataList = GetEmployeeDataFromCache(tempDataId, out cached);
            var dataToDeleted = dataList.FirstOrDefault(t => t.DetailID == model.DetailID);
            if (dataToDeleted != null)
            {
                dataList.Remove(dataToDeleted);
                SaveEmployeeDataIntoCache(tempDataId, dataList);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ClearTempEmployee(string tempDataId)
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

        #region Import Employee
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files,string v)
        {
            var dataItems = new List<AcsEmployeeImportData>();
            var errors = new ObjectResults<string[]>();
            foreach (var f in files)
            {
                using (var fs = new StreamReader(f.InputStream))
                {
                    using (var reader = new CsvHelper.CsvReader(fs))
                    {
                        switch (v)
                        {
                            case RequestFors.Employee:
                                reader.Configuration.RegisterClassMap<AcsEmployeeImportDataMap>();
                                break;
                            default:
                                reader.Configuration.RegisterClassMap<AcsBusinessTripImportDataMap>();
                                break;
                        }
                        
                        reader.Configuration.HasHeaderRecord = true;
                        reader.Configuration.Delimiter = "|";
                        while (reader.Read())
                        {
                            try
                            {
                                var data = reader.GetRecord<AcsEmployeeImportData>();
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

            switch (v)
            {
                case RequestFors.Employee:
                    return JsonNet(new { Data = dataItems.Select(t=>new { EmployeeID = t.EmployeeID }), Errors = errors.ToListResult() }, JsonRequestBehavior.AllowGet);
                default:
                    return JsonNet(new { Data = dataItems.Select(t=>new { EmployeeName = t.EmployeeName, DepartmentName = t.DepartmentName }), Errors = errors.ToListResult() }, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(IList<AcsEmployeeImportData> data, string tempDataId, string requestFor)
        {
            var results = new ObjectResults<AcsEmployeeImportData>();
            bool cached = false;
            var dataItems = GetEmployeeDataFromCache(tempDataId, out cached);
            var maxSeq = dataItems.Count() == 0 ? 0 : dataItems.Max(t => t.Seq);
            switch (requestFor)
            {
                case RequestFors.Employee:
                    foreach (var d in data)
                    {
                        if (dataItems.Any(t => t.EmployeeID == d.EmployeeID))
                        {
                            // Duplicate employee id.
                            results.AddResult(d, new Exception(String.Format("Duplicate employee id. (Employee ID: {0})", d.EmployeeID)));
                            continue;
                        }

                        var findEmployee = service.GetEmployeeDataView(d.EmployeeID);
                        if (findEmployee == null)
                        {
                            // Employee  data not found in employee master data.
                            results.AddResult(d, new Exception("Employee data not found."));
                        }
                        else
                        {
                            results.AddResult(d);
                            var dataView = findEmployee.ToViewModel();
                            dataItems.Add(new AcsEmployeeDetailViewModel() { Seq = ++maxSeq, EmployeeID = d.EmployeeID, EmployeeName = dataView.EmployeeName, DepartmentName = dataView.DepartmentName });
                        }
                    }
                    SaveEmployeeDataIntoCache(tempDataId, dataItems);
                    break;
                case RequestFors.BusinessTrip:
                    foreach (var d in data)
                    {
                        if (dataItems.Any(t => String.Compare(t.EmployeeName, d.EmployeeName, true) == 0))
                        {
                            // Duplicate employee name.
                            results.AddResult(d, new Exception(String.Format("Duplicate employee name. (Employee Name: {0})", d.EmployeeName)));
                            continue;
                        }
                        results.AddResult(d);
                        dataItems.Add(new AcsEmployeeDetailViewModel() { Seq = ++maxSeq, EmployeeName = d.EmployeeName, DepartmentName = d.DepartmentName });
                    }
                    SaveEmployeeDataIntoCache(tempDataId, dataItems);
                    break;
            }
            return JsonNet(results.ToResult(), JsonRequestBehavior.AllowGet);
        }


        #endregion

        [HttpPost]
        public ActionResult ListEmployeeDetail([DataSourceRequest]DataSourceRequest request, string requestNo)
        {
            var dataViews = service.GetAcsEmployeeDetailDataViews(requestNo);
            var result = dataViews.ToDataSourceResult(request, (AcsEmployeeDetailDataView d) => d.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }
    }
}