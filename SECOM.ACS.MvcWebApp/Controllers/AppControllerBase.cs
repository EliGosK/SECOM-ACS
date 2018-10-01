using CSI.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using SECOM.ACS.Identity;
using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.App_Start;
using SECOM.ACS.Tasks;
using SECOM.ACS.Workflow;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class AppControllerBase : Controller
    {
        #region Properties
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public UserData UserData
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var identity = User.Identity as ClaimsIdentity;
                    if (identity == null) { return UserData.Empty(); }
                    var c = identity.Claims.FirstOrDefault(t => t.Type == ClaimTypes.UserData);
                    if (c==null) { return UserData.Empty(); }
                    return JsonConvert.DeserializeObject<UserData>(c.Value);
                }
                return UserData.Empty();
            }
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        protected MailManager MailManager
        {
            get
            {
                var manager = HttpContext.GetOwinContext().Get<MailManager>();               
                return manager;
            }
        }

        protected WorkflowManager WorkflowManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<WorkflowManager>();
            }
        }

        protected UpdateEmployeeInfoTask UpdateEmployeeTask
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UpdateEmployeeInfoTask>();
            }
        }


        protected ExportInterfaceFileToAccessControlTask ExportInterfaceFileToAccessControl
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ExportInterfaceFileToAccessControlTask>();
            }
        }

        protected string CurrentCulture
        {
            get {
                var cookieLocale = HttpContext.Request.Cookies["locale"];
                if (cookieLocale != null)
                {
                    return cookieLocale.Value;
                }
                else
                {
                    var uiCulture = CultureInfo.CurrentUICulture;
                    return uiCulture.Name;
                }
            }
        }
        #endregion

        #region Internal Server Error
        protected JsonResult InternalServerError(Exception error)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Json(new { message = error.Message }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult InternalServerError(string message)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult InternalServerError(string format,params object[] args)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Json(new { message = String.Format(format, args) }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        protected JsonResult BadRequest(string message = "Invalid Request")
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult InvalidRequest(string message = "Invalid Request")
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult Ok(string message = "success")
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult OkFormat(string format, params object[] args)
        {
            return Ok(String.Format(format, args));
        }

        protected JsonResult NotFound(string message = "Data not found")
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult Created(string message = "created")
        {
            Response.StatusCode = (int)HttpStatusCode.Created;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JsonNet(object data, JsonRequestBehavior behavior)
        {
            return new JsonNetResult(data);
        }

        public ActionResult JsonNet(object data, JsonSerializerSettings settings, JsonRequestBehavior behavior)
        {
            return new JsonNetResult(data, settings);
        }

    }
}