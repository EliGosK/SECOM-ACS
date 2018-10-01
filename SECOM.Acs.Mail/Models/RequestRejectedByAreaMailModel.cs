using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail.Models
{
    [MailTemplate("RequestRejectedByArea")]
    public class RequestRejectedByAreaMailModel : RequestMailModeBase
    {
        public string AreaNameEN { get; set; }
        public string AreaNameTH { get; set; }
        public string RejectReason { get; set; }
    }
}
