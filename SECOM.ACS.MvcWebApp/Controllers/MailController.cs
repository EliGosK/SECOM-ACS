using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using SECOM.ACS.Mail;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Services;
using SECOM.ACS.MvcWebApp.Extensions;
using System.Net.Mail;
using SECOM.ACS.Models;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class MailController : AppControllerBase
    {
        private readonly IAccessControlService service;
        public MailController(IAccessControlService service)
        {
            this.service = service;
        }
      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendMail(SendMailViewModel model)
        {

            var misc = service.GetSystemMiscsByMiscType(SystemMiscTypes.DocumentType)
                .FirstOrDefault(t => t.SysMiscCode == model.DocumentCode);
            var prefix = model.RequestNo.Substring(0, 1).ToUpperInvariant();
            var parameterName = $"{prefix}LinkUrl";
            var p = ApplicationContext.Setting.Mail.CustomParameters.Where(t => String.Compare(parameterName, t.Name, true) == 0).FirstOrDefault();

            var linkUrl = Url.Action("Detail", "AcsEmployee", new { id = model.RequestNo });
            if (p != null) {
                linkUrl = String.Format(p.Value, model.RequestNo);
            }
            try
            {
                if (misc != null)
                {
                    var mailModel = new {
                        DocumentCode = model.DocumentCode,
                        DocumentTypeEN = misc.SysMiscValue1,
                        DocumentTypeTH = misc.SysMiscValue2,
                        RequestNo = model.RequestNo,
                        ItemName = model.ItemName,
                        LinkUrl = linkUrl,
                        AreaNameTH = model.AreaNameTH,
                        AreaNameEN = model.AreaNameEN,
                        RejectReason = model.RejectReason,
                        EmployeeNameEN = model.EmployeeNameEN,
                        EmployeeNameTH = model.EmployeeNameTH,
                        NewPassword = model.NewPassword
                    };
                
                    MailManager.SendMail(model.MailName, mailModel, model.MailTo);
                    return Ok("Mail is send.");
                }
                return BadRequest(String.Format("Could not found Document Type: {0}", model.DocumentCode));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public ActionResult SendPasswordResetMail(SendMailViewModel model)
        {
            try
            {
                var mailModel = new
                {
                    EmployeeNameEN = model.EmployeeNameEN,
                    EmployeeNameTH = model.EmployeeNameTH,
                    NewPassword = model.NewPassword
                };

                MailManager.SendMail(model.MailName, mailModel, model.MailTo);
                return Ok("Mail is send.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

    
}