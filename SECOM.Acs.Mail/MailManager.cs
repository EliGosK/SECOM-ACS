using RazorEmail;
using SECOM.ACS.Mail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail
{
    public class MailManager : IDisposable
    {
        private readonly IMailProvider mailProvider;
        public Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

        public MailManager(IMailProvider provider)
        {
            mailProvider = provider;
        }

        private MailAddressCollection ToMailAddress(string[] mailAddresses)
        {
            var mailTo = new MailAddressCollection();
            foreach (var mailAddress in mailAddresses)
            {
                mailTo.Add(mailAddress);
            }
            return mailTo;
        }

        public void AddParameter(string name,string value)
        {
            if (this.Parameters.ContainsKey(name))
                this.Parameters[name] = value;
            else
                this.Parameters.Add(name, value);
        }

        public void SendMail(string templateName, dynamic model, string[] mailAddresses)
        {            
            mailProvider.SendMail(model, templateName, ToMailAddress(mailAddresses));
        }

        protected void InitializeMailModel(RequestMailModeBase model)
        {
            var key = String.Concat(model.RequestNo.Substring(0, 1), "LinkUrl");
            model.LinkUrl = this.Parameters.ContainsKey(key) ? String.Format(this.Parameters[key], model.RequestNo) : "#";
        }

        #region Send Request Approve
        public void SendRequestApproved(RequestApprovedMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestApprovedMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestApproved(RequestApprovedMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestApprovedMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Request Cancelled
        public void SendRequestCancelled(RequestCancelledMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestCancelledMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestCancelled(RequestCancelledMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestCancelledMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Request Expired
        public void SendRequestExpired(RequestExpiredMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestExpiredMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestExpired(RequestExpiredMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestExpiredMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Request Rejected
        public void SendRequestRejected(RequestRejectedMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestRejectedMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestRejected(RequestRejectedMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestRejectedMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Request Rejected By Area
        public void SendRequestRejectedByArea(RequestRejectedByAreaMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestRejectedByAreaMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestRejectedByArea(RequestRejectedByAreaMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestRejectedByAreaMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Request Wait For Approval
        public void SendRequestWaitForApproval(RequestWaitForApprovalMailModel model,string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestWaitForApprovalMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestWaitForApproval(RequestWaitForApprovalMailModel model,MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestWaitForApprovalMailModel>(model,mailAddresses);
        }
        #endregion

        #region Send Request Wait For Approval
        public void SendRequestWithness(RequestWithnesMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestWithnesMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestWithnes(RequestWithnesMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestWithnesMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Request Acknowledge
        public void SendRequestAcknowledge(RequestAcknowledgeMailModel model, string[] mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestAcknowledgeMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendRequestAcknowledge(RequestAcknowledgeMailModel model, MailAddressCollection mailAddresses)
        {
            InitializeMailModel(model);
            mailProvider.SendMail<RequestAcknowledgeMailModel>(model, mailAddresses);
        }
        #endregion

        #region Send Password Reset
        public void SendPasswordReset(PasswordResetMailModel model, string[] mailAddresses)
        {
            mailProvider.SendMail<PasswordResetMailModel>(model, ToMailAddress(mailAddresses));
        }

        public void SendPasswordReset(PasswordResetMailModel model, MailAddressCollection mailAddresses)
        {
            mailProvider.SendMail<PasswordResetMailModel>(model, mailAddresses);
        }
        #endregion

        public void Dispose()
        {
            
        }
    }
}
