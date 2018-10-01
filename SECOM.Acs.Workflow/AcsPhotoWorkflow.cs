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
    public class AcsPhotoWorkflow : AcsStateWorkflow
    {
        public AcsPhotoWorkflow(IAccessControlService service, MailManager mailManager) 
            : base(service, mailManager)
        {

        }

        protected override void DoUpdateRequestStatus(IAcsRequest request)
        {
            var acs = request as AcsPhoto;
            if (acs == null) { throw new ArgumentException("Invalid request data. request data is not AcsEmployee."); }

            var result = DataService.UpdateAcsPhoto(acs);
            if (!result.IsSucceed)
            {
                throw result.Error;
            }
        }

        protected override void DoApproveActionOnSecondStep(WorkflowDataState dataState, IEnumerable<ReqApproverList> approvers, ReqApproverList approver)
        {
            // Are Approver (Step 2) => APPROVED => Send RequestAcknowledge mail
            if (approver.Step == 2 && approver.ApprovalCode == ApprovalCode.Approve)
            {
                OnProgress(new MessageEventArgs($"Update Request Status to APPROVING"));
                // Set request Status = APPROVING.
                dataState.Request.Status = RequestStatus.Approving;
                DoUpdateRequestStatus(dataState.Request);

                var acs = DataService.GetAcsPhoto(dataState.Request.ReqNo, LoadAcsPhotoOptions.None);
                var employee = DataService.GetEmployeeInformation(acs.AckBy);
                if (employee == null) { return; }
                SendAcknowledgeMail(employee, dataState.Request);
            }
        }
    }
}
