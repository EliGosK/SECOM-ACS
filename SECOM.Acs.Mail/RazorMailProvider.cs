using CSI.Utilities;
using RazorEmail;
using System;
using System.IO;
using System.Net.Mail;
using System.Web.Hosting;

namespace SECOM.ACS.Mail
{
    public class RazorMailProvider : IMailProvider
    {
        public RazorMailOptions Options { get; private set; }

        public RazorMailProvider(RazorMailOptions options)
        {
            this.Options = options;
        }

        private string GetTemplateFolder()
        {
            var baseDir = Options.BaseTemplateFolder;
            if (baseDir.Contains("~"))
                baseDir = HostingEnvironment.MapPath(baseDir);

            if (!Path.IsPathRooted(baseDir))
                baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir);
            return baseDir;
        }

        private MailMessage CreateMailMessage(RazorMailer mailer, object model, string templateName, MailAddress mailAddress)
        {
            var mail = mailer.Create(templateName, model, mailAddress.Address, mailAddress.DisplayName)
                   .ToMailMessage((Email email) => {
                       foreach (var v in email.Views)
                       {
                           v.EncodingText = this.Options.Encoding.BodyName;
                       }
                   });
            mail.IsBodyHtml = true;
            mail.BodyEncoding = this.Options.Encoding;
            mail.SubjectEncoding = this.Options.Encoding;
            return mail;
        }

        public void SendMail<TModel>(TModel model, MailAddressCollection mailAddresses)
        {
            var attr = AttributeHelper.GetAttribute<MailTemplateAttribute>(typeof(TModel), true);
            if (attr == null) { throw new Exception("Mail template not found."); }
            SendMail(model, attr.TemplateName, mailAddresses);
        }

        public void SendMail(object model,string templateName, MailAddressCollection mailAddresses)
        {
            var mailer = new RazorMailer(GetTemplateFolder());
            foreach (var mailAddress in mailAddresses)
            {
                var mailMessage = CreateMailMessage(mailer, model, templateName, mailAddress);
                mailMessage.Send();
            }
        }

        public void SendMailAsync<TModel>(TModel model, MailAddressCollection mailAddresses)
        {
            var attr = AttributeHelper.GetAttribute<MailTemplateAttribute>(typeof(TModel), true);
            if (attr == null) { throw new Exception("Mail template not found."); }
            SendMailAsync(model, attr.TemplateName, mailAddresses);
        }

        public void SendMailAsync(object model, string templateName, MailAddressCollection mailAddresses)
        {
            var mailer = new RazorMailer(GetTemplateFolder());
            foreach (var mailAddress in mailAddresses)
            {
                var mailMessage = CreateMailMessage(mailer, model, templateName, mailAddress);
                mailMessage.SendAsync();
            }
        }
    }
}
