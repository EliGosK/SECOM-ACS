using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MailTemplateAttribute : Attribute
    {
        public MailTemplateAttribute(string templateName)
        {
            this.TemplateName = templateName;
        }

        public string TemplateName { get; private set; }
    }
}
