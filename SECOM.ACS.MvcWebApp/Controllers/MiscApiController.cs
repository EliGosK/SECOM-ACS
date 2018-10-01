using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.MvcWebApp.Resources;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using CSI.Localization;
using System;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    [RoutePrefix("api/miscs")]
    [RouteCulture]
    public class MiscApiController : AppControllerBase
    {
        protected readonly IAccessControlService serivce;

        public MiscApiController(IAccessControlService service)
        {
            this.serivce = service;
        }

        [Route("{type}")]
        public ActionResult GetMiscsByMiscType(string type)
        {
            var dataItems = serivce.GetMiscsByType(type);                
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }
   
        [Route("list/{type}")]
        public ActionResult GetListMiscsByMiscType(string type)
        {
            var dataItems = serivce.GetMiscsByType(type)
                .Select(t=> new {
                    Name = ModelLocalizeManager.GetValue(t, "MiscDisplay"),
                    Key = t.MiscName,
                    Value = t.MiscID
                })
              .OrderBy(t => t.Name, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true));
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }
    }
}
