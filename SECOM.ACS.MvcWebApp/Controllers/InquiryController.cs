using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Models;
using System.Web.Hosting;
using SECOM.ACS.Json;
using SECOM.ACS.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class InquiryController : AppControllerBase
    {
        // GET: Inquiry
        public ActionResult Index()
        {
            return View();
        }

    }
}