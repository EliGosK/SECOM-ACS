using MvcSiteMapProvider;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
            
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangeLanguage(string lang, string returnUrl)
        {
            var langCookie = new HttpCookie("locale", lang) { HttpOnly = true };
            Response.AppendCookie(langCookie);
            return Redirect(HttpUtility.UrlDecode(returnUrl));
        }
    }
}
