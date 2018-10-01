using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail.Models
{
    public class SendMailViewModel
    {
        public string[] MailTo { get; set; }
        public string RequestNo { get; set; }
        public string DocumentCode { get; set; }
        public string MailName { get; set; }
        public string ItemName { get; set; }

        public string RejectReason { get; set; }
        public string AreaNameEN { get; set; }
        public string AreaNameTH { get; set; }

        public string EmployeeNameEN { get; set; }
        public string EmployeeNameTH { get; set; }
        public string NewPassword { get; set; }
    }
}
