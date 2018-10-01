using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("Error")]
    public class ErrorController : Controller
    {
        [Route("{statusCode:int}")]
        public ActionResult Index(int statusCode)
        {
            Response.TrySkipIisCustomErrors = true;
            return View(statusCode.ToString());
        }
    }
}