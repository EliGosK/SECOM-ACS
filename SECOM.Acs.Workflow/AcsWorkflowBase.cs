using CSI.ComponentModel;
using Newtonsoft.Json;
using SECOM.ACS.Mail;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public abstract class AcsWorkflowBase : IAcsWorkflow
    {
        public IAccessControlService DataService { get; private set; }
        public MailManager MailManager { get; private set; }

        public AcsWorkflowBase(IAccessControlService service, MailManager mailManager)
        {
            this.DataService = service;
            this.MailManager = mailManager;
        }

        private EventHandlerList handlers = new EventHandlerList();
        private object progressEventKey = new object();
        private object errorEventKey = new object();

        public Guid InstanceId { get; private set; } = Guid.NewGuid();

        public event MessageEventHandler Progress
        {
            add { this.handlers.AddHandler(progressEventKey, value); }
            remove { this.handlers.RemoveHandler(progressEventKey, value); }
        }

        public event ErrorEventHandler Error
        {
            add { this.handlers.AddHandler(errorEventKey, value); }
            remove { this.handlers.RemoveHandler(errorEventKey, value); }
        }

        public virtual void StartForCreateRequest(IAcsRequest request)
        {
            OnProgress(new MessageEventArgs($"Run for create request is started."));
            // Send Mail Notification to approver (Step 1)
            var approvers = this.DataService.GetReqApproverListByRequestNo(request.ReqNo);
            var minStep = approvers.Count()==0?(byte)0: approvers.Min(t => t.Step);
            var approverToSendMails = approvers.Where(t => t.Step == minStep).ToList();
            if (approverToSendMails != null && approverToSendMails.Count>0)
            {
                SendRequestWaitForApprovalMail(request, approverToSendMails);
            }
        }

        public abstract void StartForApprovalRequest(WorkflowDataState dataState, ExportInterfaceFileOptions exportFileOptions);
        public abstract void StartForCancelRequest(IAcsRequest request, ExportInterfaceFileOptions exportFileOptions);
        protected abstract void DoUpdateRequestStatus(IAcsRequest request);

        #region Send Mail Notification
        /// <summary>
        /// Send request wait for approval mail from request approver list.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="approvers"></param>
        protected void SendRequestWaitForApprovalMail(IAcsRequest request, IList<ReqApproverList> approvers)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            foreach (var approver in approvers)
            {
                var employee = DataService.GetEmployeeInformation(approver.ApproveUserName);
                if (employee == null)
                {
                    OnProgress(new MessageEventArgs($"Could not send request wait for approval mail by area mail. Employee data not found from User: {approver.ApproveUserName}"));
                    continue;
                }
                if(employee.Email != null)
                    SendRequestWaitForApprovalMail(employee, request);
            }
        }

        /// <summary>
        /// Send request wait for approval mail.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="request"></param>
        protected void SendRequestWaitForApprovalMail(EmployeeInformation employee, IAcsRequest request)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var model = new RequestWaitForApprovalMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            OnProgress(new MessageEventArgs($"Sending Request Wait For Approval mail to {employee.Email} from request no {request.ReqNo}."));
            if(employee.Email != null)
                this.MailManager.SendRequestWaitForApproval(model, new string[] { employee.Email });
        }

        /// <summary>
        /// Send request wait for approval mail from request approver list.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="approvers"></param>
        protected void SendRequestCancelledMail(IAcsRequest request, IList<ReqApproverList> approvers)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            foreach (var approver in approvers)
            {
                var employee = DataService.GetEmployeeInformation(approver.ApproveUserName);
                if (employee == null)
                {
                    OnProgress(new MessageEventArgs($"Could not send request wait for approval mail by area mail. Employee data not found from User: {approver.ApproveUserName}"));
                    continue;
                }
                if(employee.Email != null)
                    SendRequestCancelledMail(employee, request);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="request"></param>
        protected void SendRequestCancelledMail(EmployeeInformation employee,IAcsRequest request)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                  .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var model = new RequestCancelledMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            OnProgress(new MessageEventArgs($"Sending Request Wait For Approval mail to {employee.Email} from request no {request.ReqNo}."));
            if(employee.Email != null)
                this.MailManager.SendRequestCancelled(model, new string[] { employee.Email });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="request"></param>
        /// <param name="areaId"></param>
        protected void SendRequestRejectedByAreaMail(EmployeeInformation employee, IAcsRequest request, ReqApproverList requestApproverList)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var area = DataService.GetArea(requestApproverList.AreaID.Value);
            var model = new RequestRejectedByAreaMailModel()
            {
                RequestNo = request.ReqNo,
                RejectReason = requestApproverList.RejectReason,
                AreaNameEN = area == null ? "" : area.AreaDisplayEN,
                AreaNameTH = area == null ? "" : area.AreaDisplayTH,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            OnProgress(new MessageEventArgs($"Sending Request Rejected by Area to {employee.Email} from request no {request.ReqNo}."));
            if (employee.Email != null)
                this.MailManager.SendRequestRejectedByArea(model, new string[] { employee.Email });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="request"></param>
        protected void SendRequestRejectedMail(EmployeeInformation employee, IAcsRequest request,ReqApproverList requestApproverList)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var model = new RequestRejectedMailModel()
            {
                RequestNo = request.ReqNo,
                RejectReason = requestApproverList.RejectReason,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            OnProgress(new MessageEventArgs($"Sending Request Rejected mail to {employee.Email} from request no {request.ReqNo}."));
            if (employee.Email != null)
                this.MailManager.SendRequestRejected(model, new string[] { employee.Email });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="request"></param>
        protected void SendRequestApprovedMail(EmployeeInformation employee, IAcsRequest request)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var model = new RequestApprovedMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            OnProgress(new MessageEventArgs($"Sending Request Approved mail to {employee.Email} from request no {request.ReqNo}."));
            if (employee.Email != null)
                this.MailManager.SendRequestApproved(model, new string[] { employee.Email });
        }

        protected void SendAcknowledgeMail(EmployeeInformation employee,IAcsRequest request)
        {
            var documentType = DataService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                   .FirstOrDefault(t => t.SysMiscCode == request.DocumentType);

            var model = new RequestAcknowledgeMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
            };
            OnProgress(new MessageEventArgs($"Sending Request Acknowlege mail to {employee.Email} from request no {request.ReqNo}."));
            if (employee.Email != null)
                this.MailManager.SendRequestAcknowledge(model, new string[] { employee.Email });
        }
        #endregion

        #region Methods

        protected void DoCreateReferReqApproverList(WorkflowDataState dataState, ReqApproverList approver, params ReqApproverList[] referApprovers)
        {
            foreach (var referApprover in referApprovers)
            {
                var approverToCreate = new ReqApproverList()
                {
                    ApprovalID = Guid.NewGuid(),
                    ReqNo = referApprover.ReqNo,
                    Step = referApprover.Step,
                    ApproveUserName = referApprover.ApproveUserName,
                    AreaID = referApprover.AreaID,
                    CreateBy = dataState.Request.UpdateBy,
                    CreateDate = DateTime.Now,
                    UpdateBy = dataState.Request.UpdateBy,
                    UpdateDate = DateTime.Now,
                    ReferenceApprovalID = approver.ApprovalID
                };
                OnProgress(new MessageEventArgs($"Creating Req Approver List with data {JsonConvert.SerializeObject(approverToCreate)}"));
                var result = DataService.CreateReqApproverList(approverToCreate);
                if (!result.IsSucceed)
                {
                    throw result.Error;
                }

                OnProgress(new MessageEventArgs($"Created Req Approver List is successfully."));
                if (approver.AreaID.HasValue)
                {
                    // Send RequestRejected By Area Mail to Superior
                    var employee = DataService.GetEmployeeInformation(approverToCreate.ApproveUserName);
                    if (employee == null)
                    {
                        OnProgress(new MessageEventArgs($"Could not send request reject by area mail. Employee data not found from User: {approverToCreate.ApproveUserName}"));
                        continue;
                    }
                    SendRequestRejectedByAreaMail(employee, dataState.Request, approver);
                }

                if (approver.ApprovalCode == ApprovalCode.Approve && approver.Step == 1)
                {
                    // Superior Approve and re-send mail to Area that rejected in previous request.
                    var employee = DataService.GetEmployeeInformation(approverToCreate.ApproveUserName);
                    if (employee == null)
                    {
                        OnProgress(new MessageEventArgs($"Could not send request approval mail. Employee data not found from User: {approverToCreate.ApproveUserName}"));
                        continue;
                    }
                    SendRequestApprovedMail(employee, dataState.Request);
                }
            }
        }

        
        #endregion


        protected void OnProgress(MessageEventArgs e)
        {
            var handler = handlers[progressEventKey] as MessageEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnError(ErrorEventArgs e)
        {
            var handler = handlers[errorEventKey] as ErrorEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
