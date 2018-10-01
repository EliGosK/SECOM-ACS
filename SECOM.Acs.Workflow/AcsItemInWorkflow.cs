using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Tasks;
using CSI.ComponentModel;

namespace SECOM.ACS.Workflow
{
    public class AcsItemInWorkflow : AcsStateWorkflow
    {
        public AcsItemInWorkflow(IAccessControlService service, MailManager mailManager)
            : base(service, mailManager)
        {

        }

        protected override void DoUpdateRequestStatus(IAcsRequest request)
        {
            var acs = request as AcsItemIn;
            if (acs == null) { throw new ArgumentException("Invalid request data. request data is not AcsItemIn."); }

            var result = DataService.UpdateAcsItemIn(acs);
            if (!result.IsSucceed)
            {
                throw result.Error;
            }
        }


        protected override void DoApproveActionOnSecondStep(WorkflowDataState dataState, IEnumerable<ReqApproverList> approvers, ReqApproverList approver)
        {
            // Are Approver (Step 2) => APPROVED => Send RequestAcknowledge mail
            if (approver.Step == 2 && approver.ApprovalCode == ApprovalCode.Approve) {
                OnProgress(new MessageEventArgs($"Update Request Status to APPROVING"));
                // Set request Status = APPROVING.
                dataState.Request.Status = RequestStatus.Approving;
                DoUpdateRequestStatus(dataState.Request);

                var acs = DataService.GetAcsItemIn(dataState.Request.ReqNo, LoadAcsItemInOptions.None);
                var employee = DataService.GetEmployeeInformation(acs.AckBy);
                if (employee == null) { return; }
                SendAcknowledgeMail(employee, dataState.Request);
            }
        }
    }
}
