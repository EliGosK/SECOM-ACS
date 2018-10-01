using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class MailConfiguration
    {
        //public string MailFrom { get; set; } 
        //public string MailFromDisplay { get; set; }
        //public string Host { get; set; }

        public string MailTemplateFolder { get; set; } = "~/EmailTemplates";
        public List<MailParameter> CustomParameters { get; set; } = new List<MailParameter>();
    }

    public class MailParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
