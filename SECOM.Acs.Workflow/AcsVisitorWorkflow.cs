using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.Linq;

namespace SECOM.ACS.Workflow
{
    public class AcsVisitorWorkflow : AcsTransactionWorkflow
    {
        public AcsVisitorWorkflow(IAccessControlService service,IAcsTask<ExportInterfaceFileToAccessControlTaskOptions> task, MailManager mailManager) 
            : base(service,task, mailManager)
        {

        }

        protected override void DoUpdateRequestStatus(IAcsRequest request)
        {
            var acs = request as AcsVisitor;
            if (acs == null) { throw new ArgumentException("Invalid request data. request data is not AcsVisitor."); }
          
            var result = DataService.UpdateAcsVisitor(acs);
            if (!result.IsSucceed)
            {
                throw result.Error;
            }
        }

        protected override TransactionAcs[] CreateTransactionFromRequest(IAcsRequest request)
        {
            var acs = request as AcsVisitor;
            return acs.ToTransactions(request.UpdateBy);
        }
    }
}
