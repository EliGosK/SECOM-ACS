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

namespace SECOM.ACS.Workflow
{
    public class AcsItemOutWorkflow : AcsStateWorkflow
    {
        public AcsItemOutWorkflow(IAccessControlService service, MailManager mailManager) 
            : base(service,mailManager)
        {

        }

        protected override void DoUpdateRequestStatus(IAcsRequest request)
        {
            var acs = request as AcsItemOut;
            if (acs == null) { throw new ArgumentException("Invalid request data. request data is not AcsEmployee."); }

            var result = DataService.UpdateAcsItemOut(acs);
            if (!result.IsSucceed)
            {
                throw result.Error;
            }
        }
    }
}
