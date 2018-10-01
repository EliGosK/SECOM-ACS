using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SECOM.ACS.Mail;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Models;

namespace SECOM.ACS.Tests
{
    [TestClass]
    public class SendMailUnitTest
    {
        private MailManager mailManager;
        private IAccessControlService service;

        [TestInitialize]
        public void Initialize()
        {
            mailManager = new MailManager(new RazorMailProvider(new RazorMailOptions() { BaseTemplateFolder = "EmailTemplates" }));
            service = new AccessControlService();
        }

        private dynamic GetDocumentType(string documentType)
        {
            var documentTypes = service.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType).ToList();
            var dataItem = documentTypes.FirstOrDefault(t => t.SysMiscCode == documentType);
            return new
            {
                DocumentCode = documentType,
                DocumentTypeEN = dataItem.SysMiscValue1,
                DocumentTypeTH = dataItem.SysMiscValue2
            };
        }

        [TestMethod]
        public void SendRequestApprovedMailTestMethod()
        {
            
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var model = new RequestApprovedMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH
            };
            mailManager.SendRequestApproved(model,new string[] { "sittichok@csithai.com" });
        }
              
        [TestMethod]
        public void SendRequestCancelledMailTestMethod()
        {
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var model = new RequestCancelledMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH
            };
            mailManager.SendRequestCancelled(model, new string[] { "sittichok@csithai.com" });
        }

        [TestMethod]
        public void SendRequestExpiredMailTestMethod()
        {
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var model = new RequestExpiredMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH
            };
            mailManager.SendRequestExpired(model, new string[] { "sittichok@csithai.com" });
        }

        [TestMethod]
        public void SendRequestRejectedMailTestMethod()
        {
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var model = new RequestRejectedMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH
            };
            mailManager.SendRequestRejected(model, new string[] { "sittichok@csithai.com" });
        }

        [TestMethod]
        public void SendRequestRejectedByAreaMailTestMethod()
        {
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var area = service.GetArea(1);
            var model = new RequestRejectedByAreaMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH,
                AreaNameEN = area.AreaDisplayEN,
                AreaNameTH = area.AreaDisplayTH                
            };
            mailManager.SendRequestRejectedByArea(model, new string[] { "sittichok@csithai.com" });
        }

        [TestMethod]
        public void SendRequestWaitForApprovalMailTestMethod()
        {
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var model = new RequestWaitForApprovalMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH
            };
            mailManager.SendRequestWaitForApproval(model, new string[] { "sittichok@csithai.com" });
        }

        [TestMethod]
        public void SendRequestWithnessMailTestMethod()
        {
            var request = service.GetAcsEmployee("E20171002-004", LoadAcsEmployeeOptions.None);
            var doc = GetDocumentType(request.DocumentType);
            var model = new RequestWithnesMailModel()
            {
                RequestNo = request.ReqNo,
                DocumentTypeEN = doc.DocumentTypeEN,
                DocumentTypeTH = doc.DocumentTypeTH
            };
            mailManager.SendRequestWithness(model, new string[] { "sittichok@csithai.com" });
        }
    }
}
