using CSI.Web.Mvc;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using SECOM.ACS.Workflow;
using System;
using System.Linq;
using System.Web.Mvc;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class AcsPhotoController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public AcsPhotoController(IAccessControlService service)
        {
            this.service = service;
        }

        public ActionResult Index()
        {
            return View();
        }

        [SiteMapPageTitle("ACS060")]
        [ApplicationAuthorize("ACS060",PermissionNames.Add)]
        public ActionResult Create()
        {
            var defaultEntryTime = service.GetDefaultEntryTime();
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            
            var model = new AcsPhotoViewModel()
            {
                ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Photographing),
                CreateBy = User.Identity.Name,
                CreateDate = DateTime.Now,
                PhotoByType = PhotoByTypes.Employee,
                PhotoEmpID = UserData.EmployeeID,
                TakePhotoDateFrom = DateTime.Now.AddDays(1),
                TakePhotoTimeFrom = defaultEntryTime.EntryTimeFromTotalMinutes,
                TakePhotoDateTo = DateTime.Now.AddDays(1),
                TakePhotoTimeTo = defaultEntryTime.EntryTimeToTotalMinutes,
                Status = RequestStatus.Requesting,
                RequestEmployee = employee.ToViewModel() 
            };  
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("ReqNo,CreateDate,UpdateDate,Status")]
        [ApplicationSuspend]
        public ActionResult Create(AcsPhotoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.CreateBy = User.Identity.Name;
                viewModel.UpdateBy = User.Identity.Name;
                viewModel.Status = RequestStatus.Requesting;
                var entity = viewModel.ToEntity();
                var result = service.CreateAcsPhoto(entity);
                if (result.IsSucceed)
                {
                    var dataItem = result.DataState as AcsPhoto;
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

        [SiteMapPageTitle("ACS060")]
        [ApplicationAuthorize("ACS060", PermissionNames.View)]
        public ActionResult Detail(string id)
        {
            var request = service.GetAcsPhoto(id, LoadAcsPhotoOptions.All);
            if (request == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}",id)));
            }

            var viewModel = request.ToViewModel();
            return View(viewModel);
        }

        [SiteMapPageTitle("ACS060")]
        [ApplicationAuthorize("ACS060", PermissionNames.Edit)]
        public ActionResult Edit(string id)
        {
            var acs = service.GetAcsPhoto(id,LoadAcsPhotoOptions.All);
            if (acs == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}",id))); 
            }

            if (acs.CreateBy == User.Identity.Name && acs.Status == RequestStatus.Requesting)
            {
                var model = acs.ToViewModel();
                return View("EditForAuthor",model);
            }
            else {
                var model = acs.ToViewModel();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [IgnoreModelErrors("PhotoByType,TargetItem,SuperiorApproveUserName,AreaApproveUserName,AcknowledgeUserName")]
        [ApplicationSuspend]
        public ActionResult Edit(AcsPhotoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsPhoto(viewModel.ReqNo, LoadAcsPhotoOptions.All);
                if (acs == null)
                {
                    return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFoundFormat("Request No: {0}", viewModel.ReqNo)));
                }

                //if (acs.Status != RequestStatus.Requesting)
                //{
                //    return View("_Message", MessageViewModel.Error("Request could not modified. Current Status: {0}", AcsModelHelper.GetRequestStatusName(acs.Status)));
                //}
                // Modified data
                var entityToUpdated = viewModel.ToEntity();
                acs.Note = entityToUpdated.Note;
                acs.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsPhoto(acs);
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
        [IgnoreModelErrors("SuperiorApproveUserName,AreaApproveUserName,AcknowledgeUserName,CreateDate,UpdateDate")]
        [ApplicationSuspend]
        public ActionResult EditForAuthor(AcsPhotoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var acs = service.GetAcsPhoto(viewModel.ReqNo, LoadAcsPhotoOptions.All);
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
                acs.TakePhotoDateFrom = entityToUpdated.TakePhotoDateFrom;
                acs.TakePhotoTimeFrom = entityToUpdated.TakePhotoTimeFrom;
                acs.TakePhotoDateTo = entityToUpdated.TakePhotoDateTo;
                acs.TakePhotoTimeTo = entityToUpdated.TakePhotoTimeTo;
                acs.TargetItem = entityToUpdated.TargetItem;
                acs.Note = entityToUpdated.Note;
                acs.PurposeCodeID = entityToUpdated.PurposeCodeID;
                acs.OtherPurpose = entityToUpdated.OtherPurpose;
                acs.EquipItemID = entityToUpdated.EquipItemID;
                acs.OtherEquip = entityToUpdated.OtherEquip;
                acs.IsLending = entityToUpdated.IsLending;
                acs.UpdateBy = User.Identity.Name;
                var result = service.UpdateAcsPhoto(acs);
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
        [ApplicationSuspend]
        public ActionResult CancelRequest(string requestNo)
        {
            var acs = service.GetAcsPhoto(requestNo, LoadAcsPhotoOptions.None);
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
        public ActionResult CancelRequest(AcsPhotoViewModel model)
        {
            var acs = service.GetAcsPhoto(model.ReqNo, LoadAcsPhotoOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }
            return DoCancelRequest(acs, model.Note);
        }

        private ActionResult DoCancelRequest(AcsPhoto acs,string note)
        {
            var viewModel = acs.ToViewModel();
            if (viewModel.AllowCancelRequest(User))
            {
                acs.Note = note;
                acs.UpdateBy = User.Identity.Name;
                acs.Status = RequestStatus.Cancel;
                var result = service.UpdateAcsPhoto(acs);
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
        public ActionResult Acknowledge(string id)
        {
            var acs = service.GetAcsPhoto(id, LoadAcsPhotoOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            if (acs.Status != RequestStatus.Approving) {
                var message = MessageHelper.InvalidRequestStatusForGA();
                return InternalServerError(MessageHelper.SaveFailed(message));
            }

            // Modified AcsPhoto for Acknowlege from GA.
            acs.AckBy = User.Identity.Name;
            acs.AckDate = DateTime.Now;
            acs.UpdateBy = User.Identity.Name;
            acs.UpdateDate = DateTime.Now;
            acs.Status = RequestStatus.Approved;
            var result = service.UpdateAcsPhoto(acs);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult UpdateGA(AcsPhotoViewModel model)
        {
            var acs = service.GetAcsPhoto(model.ReqNo, LoadAcsPhotoOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            if (acs.Status != RequestStatus.Approved)
            {
                var message = MessageHelper.InvalidRequestForGAReturnAsset();
                return InternalServerError(MessageHelper.SaveFailed(message));
            }

            // Modified AcsPhoto from GA.
            acs.AssetCode = model.AssetCode;
            acs.ActReturnDate = model.ActReturnDate;
            acs.UpdateBy = User.Identity.Name;

            var result = service.UpdateAcsPhoto(acs);
            if (result.IsSucceed)
            {
                return Ok(MessageHelper.SaveCompleted());
            }
            else
            {
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        public ActionResult Approval(ApprovalRequest data)
        {
            var acs = service.GetAcsPhoto(data.RequestNo,LoadAcsPhotoOptions.Approval);
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
            var acs = service.GetAcsPhoto(id, LoadAcsPhotoOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound($"Request No: {id}"));
            }
            return DoForceDone(acs,acs.Note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApplicationSuspend]
        [ActionName("ForceDoneFromEdit")]
        public ActionResult ForceDone(AcsPhotoViewModel model)
        {
            var acs = service.GetAcsPhoto(model.ReqNo, LoadAcsPhotoOptions.None);
            if (acs == null)
            {
                return InternalServerError(MessageHelper.DataNotFound($"Request No: {model.ReqNo}"));
            }
            return DoForceDone(acs, model.Note);
        }

        private ActionResult DoForceDone(AcsPhoto acs, string note)
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

            var result = service.UpdateAcsPhoto(acs);
            if (result.IsSucceed)
            {
                return Ok(MessageHelper.SaveCompleted());
            }
            else
            {
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            }
        }

        [ApplicationAuthorize("ACS060", PermissionNames.Add)]
        [SiteMapPageTitle("ACS060")]
        public ActionResult Duplicate(string id)
        {

            var acs = service.GetAcsPhoto(id, LoadAcsPhotoOptions.All);
            if (acs == null)
            {
                var model = MessageViewModel.Error(MessageHelper.DataNotFound());
                return View("_Message", model);
            }

            var defaultEntryTime = service.GetDefaultEntryTime();
            var employee = service.GetEmployeeInformation(User.Identity.Name) ?? new EmployeeInformation() { };
            acs.ReqNo = AcsModelHelper.GenerateEmptyRequestNo(AcsRequestPrefixCharacters.Employee);
            acs.TakePhotoDateFrom = DateTime.Now.AddDays(1);
            acs.TakePhotoTimeFrom = defaultEntryTime.EntryTimeFromTimespan;
            acs.TakePhotoDateTo = DateTime.Now.AddDays(1);
            acs.TakePhotoTimeTo = defaultEntryTime.EntryTimeToTimespan;
            acs.CreateBy = User.Identity.Name;
            acs.CreateDate = DateTime.Now;
            acs.Status = RequestStatus.Requesting;
            acs.RequestEmployee = employee;            
            return View("Create", acs.ToViewModel());
        }

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