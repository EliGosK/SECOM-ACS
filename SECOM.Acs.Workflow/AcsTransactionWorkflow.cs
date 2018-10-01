using CSI.ComponentModel;
using SECOM.ACS.Mail;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public abstract class AcsTransactionWorkflow : AcsWorkflowBase
    {
        public IAcsTask<ExportInterfaceFileToAccessControlTaskOptions> ExportInterfaceFileTask { get; private set; }

        public AcsTransactionWorkflow(IAccessControlService service, IAcsTask<ExportInterfaceFileToAccessControlTaskOptions> task, MailManager mailManager)
            : base(service, mailManager)
        {
            this.ExportInterfaceFileTask = task;
        }

        protected virtual bool IsCreateInterfaceFileOnAreaApproved(IEnumerable<TransactionAcs> trans)
        {
            return trans.Where(t => !String.IsNullOrEmpty(t.CardID)).Any();
        }

        public override void StartForApprovalRequest(WorkflowDataState dataState, ExportInterfaceFileOptions exportInterfaceFileOptions)
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
                    if (!approver.ReferenceApprovalID.HasValue)
                    {
                        OnProgress(new MessageEventArgs($"Get next step request approver list (Step: {(approver.Step+1)})."));
                        // First approval flow is APPROVED => Send mail notification to next approver.
                        var approverNextSteps = approvers.Where(t => t.Step == approver.Step + 1).ToList();
                        SendRequestWaitForApprovalMail(dataState.Request, approverNextSteps);
                        // First Approved update request status from Requesting => Approving
                        if (dataState.Request.Status == RequestStatus.Requesting) {
                            OnProgress(new MessageEventArgs($"Update request status from REQUESTING to APPROVING."));
                            dataState.Request.Status = RequestStatus.Approving;
                            DoUpdateRequestStatus(dataState.Request);
                        }
                    }
                    else
                    {
                        OnProgress(new MessageEventArgs($"Get Request Approver List from reference approval id {approver.ReferenceApprovalID.Value}."));
                        // Approval flow is APPROVED refer from area approval flow is REJECT.
                        //var approvalToCreates = DataService.GetReqApproverList(approver.ReferenceApprovalID.Value);
                        // Create Area Req Approver List from Superior Approved (Find Area Req Approver Step 2 and Approval Code = REJECTED)

                        var areaApprovers = approvers.Where(t => t.Step == 2).ToList();
                        var latest = areaApprovers.GroupBy(t => new
                        {
                            AreaID = t.AreaID,
                            t.ApproveUserName
                        })
                        .Select(g => new
                        {
                            Key = g.Key,
                            UpdateDate = g.Max(row => row.UpdateDate)
                        });

                        var q = from l in latest
                                join c in areaApprovers
                                on new { AreaID = l.Key.AreaID, l.UpdateDate } equals new { c.AreaID, c.UpdateDate }
                                select c;

                        var approvalToCreates = q.Where(t => t.ApprovalCode == ApprovalCode.Reject).ToList();
                        DoCreateReferReqApproverList(dataState, approver, approvalToCreates.ToArray());
                    }
                }
                else
                {

                    // Second Approval flow is APPROVED (Step > 1)
                    // Invoke export interface file task to create interface file to Access Control System.
                    OnProgress(new MessageEventArgs($"Getting Acs transaction from request no. {dataState.Request.ReqNo}"));
                    var trans = DataService.GetAcsTransactionsByRequestNo(dataState.Request.ReqNo);
                    if (trans.Count() == 0)
                    {
                        OnProgress(new MessageEventArgs($"Creating Acs transaction from request no. {dataState.Request.ReqNo}"));
                        var transToCreate = CreateTransactionFromRequest(dataState.Request);
                       
                        var insertTransResult = DataService.CreateAcsTransactions(transToCreate);
                        if (!insertTransResult.IsSucceed)
                        {
                            throw insertTransResult.Error;
                        }
                        OnProgress(new MessageEventArgs($"Create Acs transaction from request no. {dataState.Request.ReqNo} is successfully."));
                        // Get Transaction ID for Create interface files.
                        trans = DataService.GetAcsTransactionsByRequestNo(dataState.Request.ReqNo);
                    }

                    if (IsCreateInterfaceFileOnAreaApproved(trans))
                    {
                        // Invoke Export to ACS task (ACP030) to create interface file.
                        var transToExportFiles = trans.Select(t => t.TranID).ToArray();
                        //OnProgress(new MessageEventArgs($"Creating interface file from transacitons {String.Join(",", transToExportFiles)}."));
                        DoExportInterfaceFile(transToExportFiles, exportInterfaceFileOptions, ExportToAccessControlModes.Add);
                    }

                    var currentStepApprovers = approvers.Where(t => t.Step == approver.Step).ToList();
                    var latest = currentStepApprovers.GroupBy(t => new
                    {
                        AreaID = t.AreaID.HasValue? t.AreaID.Value:0,
                        t.ApproveUserName
                    })
                    .Select(g => new
                    {
                        Key = g.Key,
                        UpdateDate = g.Max(row => row.UpdateDate)
                    });

                    var q = from l in latest
                            join c in currentStepApprovers
                            on new { AreaID = l.Key.AreaID, l.UpdateDate } equals new { AreaID= c.AreaID.HasValue ? c.AreaID.Value : 0, c.UpdateDate }
                            select c;

                    if (q.Count() == q.Count(t => t.ApprovalCode == ApprovalCode.Approve))
                    {
                        OnProgress(new MessageEventArgs($"All document approval flow at step {approver.Step} are APPROVED. Update Request Status to APPROVED"));
                        // All area approval flow are APPROVED => Set request Status = APPROVED.
                        dataState.Request.Status = RequestStatus.Approved;
                        DoUpdateRequestStatus(dataState.Request);

                        var employee = DataService.GetEmployeeInformation(dataState.Request.Requester);
                        SendRequestApprovedMail(employee, dataState.Request);
                        return;
                    }
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
                    
                    // Call Export to ACS task (ACP030) to cancel card id that approved by another area.
                    var effectiveDate = DateTime.Now.Date;
                    var trans = DataService.GetAcsTransactionsByRequestNo(dataState.Request.ReqNo);
                    var transToExportFiles = trans.Where(t => t.Status == (byte)TransactionStatus.SendCardToACS && DateTime.Compare(effectiveDate, t.EntryDateFrom.Date) >= 0 && DateTime.Compare(effectiveDate, t.EntryDateTo.Date) <= 0)
                        .Select(t => t.TranID)
                        .ToArray();
                    DoExportInterfaceFile(transToExportFiles, exportInterfaceFileOptions, ExportToAccessControlModes.Cancel);
                }
                else
                {
                    // Area Approval = REJECT (Step > 1)
                    if (approver.ReferenceApprovalID.HasValue)
                    {
                        var referApprover = DataService.GetReqApproverList(approver.ReferenceApprovalID.Value);
                        if (referApprover == null) { throw new Exception("Approval data not found."); }

                        DoCreateReferReqApproverList(dataState, approver, referApprover);
                    }
                    else
                    {
                        var referApprovers = approvers.Where(t => t.Step == 1).ToArray();
                        if (referApprovers.Count() == 0)
                        {
                            // Approval flow without superior approver.
                            var currentStepApprovers = approvers.Where(t => t.Step == approver.Step).ToList();

                            var latest = currentStepApprovers.GroupBy(t => new
                            {
                                AreaID = t.AreaID.HasValue ? t.AreaID.Value : 0,
                                t.ApproveUserName
                            })
                            .Select(g => new
                            {
                                Key = g.Key,
                                UpdateDate = g.Max(row => row.UpdateDate)
                            });

                            var q = from l in latest
                                    join c in currentStepApprovers
                                    on new { AreaID = l.Key.AreaID, l.UpdateDate } equals new { AreaID = c.AreaID.HasValue ? c.AreaID.Value : 0, c.UpdateDate }
                                    select c;

                            if (q.Count() == q.Count(t => t.ApprovalCode == ApprovalCode.Reject))
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
                        else
                        {
                            var latestStep1 = referApprovers.OrderByDescending(t => t.CreateDate).FirstOrDefault();
                            if (latestStep1 != null)
                            {
                                if (!String.IsNullOrEmpty(latestStep1.ApprovalCode))
                                {
                                    // Create Refere approver list and send mail to superior
                                    DoCreateReferReqApproverList(dataState, approver, referApprovers);
                                }
                                else
                                {
                                    if (approver.AreaID.HasValue)
                                    {
                                        // Send RequestRejected By Area Mail
                                        var employee = DataService.GetEmployeeInformation(latestStep1.ApproveUserName);
                                        if (employee == null)
                                        {
                                            OnProgress(new MessageEventArgs($"Could not send request reject by area mail. Employee data not found from User: {latestStep1.ApproveUserName}"));
                                        }
                                        SendRequestRejectedByAreaMail(employee, dataState.Request, approver);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public override void StartForCancelRequest(IAcsRequest request, ExportInterfaceFileOptions exportInterfaceFileOptions)
        {
            OnProgress(new MessageEventArgs("Workflow run for cancel request is invoked."));
            var approvers = DataService.GetReqApproverListByRequestNo(request.ReqNo).ToList();
            var user = request.UpdateBy;
            var approverToNotifications = approvers.Where(t => t.ApprovalCode == ApprovalCode.Approve).ToList();

            // Update Request Status to CANCEL
            request.Status = RequestStatus.Cancel;
            DoUpdateRequestStatus(request);

            if (approverToNotifications.Count() > 0)
            {
                if (approverToNotifications.Where(t => t.Step == 2).Count() > 0)
                {
                    // Has area approval is APPROVED
                    var trans = DataService.GetAcsTransactionsByRequestNo(request.ReqNo);
                    // Update transaction that not yet assigned card id to access control. 
                    var transToCancel = trans.Where(t => t.Status == (byte)TransactionStatus.ActiveWithoutAssignedCardID);
                    if (transToCancel.Count() > 0)
                    {
                        foreach (var t in transToCancel)
                        {
                            t.Status = (byte)TransactionStatus.CancelTransaction;
                            t.UpdateBy = user;
                            t.UpdateDate = DateTime.Now;
                        }
                        OnProgress(new MessageEventArgs($"Cancelling transaction {String.Join(",", transToCancel.Select(t => t.TranID.ToString()))}. Change transaction status from {TransactionStatus.ActiveWithoutAssignedCardID} to {TransactionStatus.CancelTransaction}."));
                        var cancelResult = DataService.UpdateAcsTransactions(transToCancel.ToArray());
                        if (!cancelResult.IsSucceed)
                        {
                            throw cancelResult.Error;
                        }
                        OnProgress(new MessageEventArgs($"Cancel transaction is successfully"));
                    }

                    // Export interface to cancel card access control 
                    OnProgress(new MessageEventArgs($"Create interface to cancel effective transaction (Status: {TransactionStatus.SendCardToACS})"));
                    var effectiveDate = DateTime.Now.Date;
                    var transToExportFiles = trans.Where(t => t.Status == (byte)TransactionStatus.SendCardToACS && DateTime.Compare(effectiveDate, t.EntryDateFrom.Date) >= 0 && DateTime.Compare(effectiveDate, t.EntryDateTo.Date) <= 0)
                         .Select(t => t.TranID)
                         .ToArray();
                    DoExportInterfaceFile(transToExportFiles, exportInterfaceFileOptions, ExportToAccessControlModes.Cancel);
                }

                // Send Cancel Mail to approver who approved request.
                SendRequestCancelledMail(request, approverToNotifications);
            }

                   
        }

        protected void DoExportInterfaceFile(Guid[] transactions, ExportInterfaceFileOptions exportInterfaceFileOptions, ExportToAccessControlModes mode)
        {
            if (transactions == null || transactions.Length == 0) { return; }
            var options = new ExportInterfaceFileToAccessControlTaskOptions()
            {
                ExportInterfaceFileOptions = exportInterfaceFileOptions,
                TaskOptions = new ExportToAccessControlOptions()
                {
                    ExportModes = mode,
                    Transactions = transactions
                }
            };
            OnProgress(new MessageEventArgs($"Creating interface file with transactions {String.Join(",", transactions.Select(t => t.ToString()))}, mode {mode}."));
            var exportResult = ExportInterfaceFileTask.Execute(options);
            if (exportResult.IsSucceed)
            {
                if (exportResult.DataState != null)
                {
                    OnProgress(new MessageEventArgs($"Create interface file is successfully. Output file: {exportResult.DataState}"));
                }
                else {
                    OnProgress(new MessageEventArgs($"*** No transaction to create interface file."));
                }
            }
            else {
                OnError(new System.IO.ErrorEventArgs(exportResult.Error));
            }
        }

        protected abstract TransactionAcs[] CreateTransactionFromRequest(IAcsRequest request);


    }
}
