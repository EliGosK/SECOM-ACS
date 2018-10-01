using CSI.Exceptions;
using SECOM.ACS.Mail;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    [AcsTaskAttribute(CanEdit = true, CanRun = true)]
    public class UpdateDocumentStatusTask : AcsTaskBase<UpdateDocumentStatusTaskOptions>
    {
        private readonly IAccessControlService acsService;
        private readonly IDocumentService documentExpirationService;
        private readonly MailManager mailManager;

        private IList<SystemMisc> documentTypes;

        public UpdateDocumentStatusTask(IAccessControlService acsService, IDocumentService documentExpirationService, IMailProvider mailProvider) 
            : base("ACP020", "Update document status")
        {
            this.acsService = acsService;
            this.documentExpirationService = documentExpirationService;
            this.mailManager = new MailManager(mailProvider);
        }

        protected override object ExecuteTask(UpdateDocumentStatusTaskOptions options)
        {
            documentTypes = acsService.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType).ToList();
            DoUpdateDocumentStatus(options,() => documentExpirationService.UpdateDocumentStatus(options.User, (int)options.DocumentTypes));
            return null;
        }

        private void DoUpdateDocumentStatus(UpdateDocumentStatusTaskOptions options, Func<UpdateDocumentResult> predicate)
        {
            OnProgress(new TaskProgressEventArgs($"Invoke Document Status Service to update document."));
            var result = predicate.Invoke();
            if (result.IsSucceed)
            {
                OnProgress(new TaskProgressEventArgs($"Update Document Status is success. Total request set to expired {result.DataState.ExpireRequestNoList.Count()} records. Total request set to done {result.DataState.DoneRequestNoList.Count()} records."));

                if (options.EnabledNotification)
                {
                    if (result.DataState.ExpireRequestNoList != null && result.DataState.ExpireRequestNoList.Count() > 0)
                    {
                        foreach (var request in result.DataState.ExpireRequestNoList)
                        {
                            try
                            {
                                var employee = acsService.GetEmployeeInformation(request.Requester);
                                if (employee != null && !String.IsNullOrEmpty(employee.Email))
                                {
                                    var documentType = documentTypes.FirstOrDefault(t => t.SysMiscCode == request.DocumentType);
                                    // Send mail to requester
                                    var model = new RequestExpiredMailModel()
                                    {
                                        RequestNo = request.ReqNo,
                                        DocumentTypeEN = documentType == null ? "" : documentType.SysMiscValue1,
                                        DocumentTypeTH = documentType == null ? "" : documentType.SysMiscValue2
                                    };
                                    var mailAddresses = new MailAddressCollection();
                                    mailAddresses.Add(new MailAddress(employee.Email,employee.EmpNameEN));
                                    OnProgress(new TaskProgressEventArgs($"Sending request expire email to {employee.Email}. (Request No. {request.ReqNo}, Document Type: {model.DocumentTypeEN}, Requester: {request.Requester})"));
                                    mailManager.SendRequestExpired(model, mailAddresses);
                                }
                            }
                            catch (Exception ex)
                            {
                                OnProgress(new TaskProgressEventArgs($"Send email fail with error {ExceptionUtility.GetLastExceptionMessage(ex)}"));
                                OnError(new ErrorEventArgs(ex));
                            }
                        }
                       
                    }
                    else
                    {
                        OnProgress(new TaskProgressEventArgs($"Skip send mail notification. There are no request that update status to expire."));
                    }
                }
            }
            else
            {
                OnProgress(new TaskProgressEventArgs($"Update Document Status is fail. Error {result.GetErrorMessage()}"));
                OnError(new ErrorEventArgs(result.Error));
            }
        }
    }



}
