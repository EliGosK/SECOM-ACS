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
    public class AcsEmployeeWorkflow : AcsTransactionWorkflow
    {
        public AcsEmployeeWorkflow(IAccessControlService service, IAcsTask<ExportInterfaceFileToAccessControlTaskOptions> task, MailManager mailManager)
            : base(service, task, mailManager)
        {

        }
        
        protected override void DoUpdateRequestStatus(IAcsRequest request)
        {
            var acs = request as AcsEmployee;
            if (acs == null) { throw new ArgumentException("Invalid request data. request data is not AcsEmployee."); }
          
            var result = DataService.UpdateAcsEmployee(acs);
            if (!result.IsSucceed)
            {
                throw result.Error;
            }
        }

        protected override TransactionAcs[] CreateTransactionFromRequest(IAcsRequest request)
        {
            var acs = request as AcsEmployee;            
            var tranasctions = acs.ToTransactions(request.UpdateBy);
            // Find Card ID
            foreach (var detail in acs.AcsEmployeeDetails)
            {
                var employee = DataService.GetEmployee(detail.EmpID);
                if (employee == null) { continue; }

                var transaction = tranasctions.FirstOrDefault(t => t.DetailID == detail.DetailID);
                if (transaction == null) { continue; }

                transaction.CardID = employee.CardID;
            }
            return tranasctions;
        }
    }
}
