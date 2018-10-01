using CSI.ComponentModel;
using SECOM.ACS.Mail;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public abstract class AcsStateWorkflow : AcsWorkflowBase
    {
        public AcsStateWorkflow(IAccessControlService service, MailManager mailManager) 
            : base(service,mailManager)
        {
           
        }

        public override void StartForApprovalRequest(WorkflowDataState dataState, ExportInterfaceFileOptions exportFileOptions)
        {
            OnProgress(new MessageEventArgs("Workflow run for approval request is invoke."));
            var approvalId = dataState.DataId;
            var approver = DataService.GetReqApproverList(approvalId);
            if (approver == null) { throw new Exception("Approval data not found"); }

            var approvers = this.DataService.GetReqApproverListByRequestNo(approver.ReqNo);
            OnProgress(new MessageEventArgs($"Get Request Approver List (Request No. {approver.ReqNo}, Approval Code {approver.ApprovalCode}, Step {approver.Step})."));
            if (approver.ApprovalCode == ApprovalCode.Approve)
            {
                if (approver.Step == 1)
                {
                    OnProgress(new MessageEventArgs($"Get next step request approver list (Step: {(approver.Step + 1)})."));
                    // First approval flow is APPROVED => Send mail notification to next approver.
                    var approverNextSteps = approvers.Where(t => t.Step == approver.Step + 1).ToList();
                    SendRequestWaitForApprovalMail(dataState.Request, approverNextSteps);
                    // First Approved update request status from Requesting => Approving
                    if (dataState.Request.Status == RequestStatus.Requesting)
                    {
                        OnProgress(new MessageEventArgs($"Update request status from REQUESTING to APPROVING."));
                        dataState.Request.Status = RequestStatus.Approving;
                        DoUpdateRequestStatus(dataState.Request);
                    }
                }
                else
                {
                    DoApproveActionOnSecondStep(dataState, approvers, approver);
                }
            }

            if (approver.ApprovalCode == ApprovalCode.Reject)
            {
                if (approver.Step == 1)
                {
                    // Superior REJECT (Step = 1)
                    // Update Request Status to Reject
                    dataState.Request.Status = RequestStatus.Rejected;
                    DoUpdateRequestStatus(dataState.Request);

                    // Send Request Reject mail to requester
                    var employee = DataService.GetEmployeeInformation(dataState.Request.Requester);
                    SendRequestRejectedMail(employee, dataState.Request,approver);
                }
                else
                {
                    // Area Approval = REJECT (Step > 1)
                    var currentStepApprovers = approvers.Where(t => t.Step == approver.Step).ToList();
                    if (currentStepApprovers.Count(t => t.ApprovalCode == ApprovalCode.Reject) > 0)
                    {
                        OnProgress(new MessageEventArgs($"All document approval flow at step {approver.Step} are REJECTED. Update Request Status to REJECT"));

                        // Update Request Status to Reject
                        dataState.Request.Status = RequestStatus.Rejected;
                        DoUpdateRequestStatus(dataState.Request);

                        // Send Reject to requester
                        var employee = DataService.GetEmployeeInformation(dataState.Request.Requester);
                        SendRequestRejectedMail(employee, dataState.Request, approver);
                    }
                    else
                    {
                        if (approver.AreaID.HasValue)
                        {
                            // Send Reject by area to requester
                            var employee = DataService.GetEmployeeInformation(dataState.Request.Requester);
                            SendRequestRejectedByAreaMail(employee, dataState.Request, approver);
                        }
                    }
                }
            }
        }

        public override void StartForCancelRequest(IAcsRequest request, ExportInterfaceFileOptions exportFileOptions)
        {
            OnProgress(new MessageEventArgs("Workflow run for cancel request is invoked."));
            var approvers = DataService.GetReqApproverListByRequestNo(request.ReqNo).ToList();

            var approverToNotifications = approvers.Where(t => t.ApprovalCode == ApprovalCode.Approve).ToList();
            if (approverToNotifications.Count() > 0)
            {
                SendRequestCancelledMail(request, approverToNotifications);
            }

            // Update Request Status to CANCEL
            request.Status = RequestStatus.Cancel;
            DoUpdateRequestStatus(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataState"></param>
        /// <param name="approvers"></param>
        /// <param name="approver"></param>
        protected virtual void DoApproveActionOnSecondStep(WorkflowDataState dataState, IEnumerable<ReqApproverList> approvers, ReqApproverList approver)
        {
            var currentStepApprovers = approvers.Where(t => t.Step == approver.Step).ToList();
            if (currentStepApprovers.Count() == currentStepApprovers.Count(t => t.ApprovalCode == ApprovalCode.Approve))
            {
                OnProgress(new MessageEventArgs($"All document approval flow at step {approver.Step} are APPROVED. Update Request Status to APPROVED"));
                // All area approval flow are APPROVED => Set request Status = APPROVED.
                dataState.Request.Status = RequestStatus.Approved;
                DoUpdateRequestStatus(dataState.Request);

                var employee = DataService.GetEmployeeInformation(dataState.Request.Requester);
                SendRequestApprovedMail(employee, dataState.Request);
            }
        }
       
    }
}
