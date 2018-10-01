using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail.Models
{
    public abstract class RequestMailModeBase 
    {
        public string RequestNo { get; set; }
        public string DocumentTypeEN { get; set; }
        public string DocumentTypeTH { get; set; }
        public string LinkUrl { get; set; }
    }
}
