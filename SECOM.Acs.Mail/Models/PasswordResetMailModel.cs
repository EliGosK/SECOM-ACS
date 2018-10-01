using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail.Models
{
    [MailTemplate("PasswordReset")]
    public class PasswordResetMailModel 
    {
        public string EmployeeID { get; set; }
        public string EmployeeNameEN { get; set; }
        public string EmployeeNameTH { get; set; }
        public string NewPassword { get; set; }
    }
}
