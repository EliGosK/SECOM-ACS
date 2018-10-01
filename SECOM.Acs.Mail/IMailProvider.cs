using SECOM.ACS.Mail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail
{
    public interface IMailProvider
    {
        void SendMail(dynamic model,string templateName, MailAddressCollection mailAddresses);
        void SendMail<TModel>(TModel model, MailAddressCollection mailAddresses);
    }
}
