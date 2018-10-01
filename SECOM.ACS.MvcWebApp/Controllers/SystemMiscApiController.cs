using CSI.Localization;
using CSI.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.Services;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using RS = SECOM.ACS.MvcWebApp.Properties;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/systemmiscs")]
    [RouteCulture]
    public class SystemMiscApiController : AppControllerBase
    {
        private readonly IAccessControlService masterService;

        public SystemMiscApiController(IAccessControlService service)
        {
            masterService = service;
        }

        [Route("types/{name}")]
        public ActionResult GetByMiscType(string name)
        {
            var dataItems = masterService.GetSystemMiscsByMiscType(name)
                .OrderBy(t => t.SysMiscSortNo)
                .Where(t => t.IsActive)
                .ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("list/{type}")]
        public ActionResult ListByMiscType(string type)
        {
            var dataItems = masterService.GetSystemMiscsByMiscType(type)
                .OrderBy(t => t.SysMiscSortNo)
                .Where(t=>t.IsActive)
                .Select(t => new { Name = ModelLocalizeManager.GetValue(t, "SysMisc"), Value = t.SysMiscCode })                
                .ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("factory")]
        public ActionResult GetFactory()
        {
            var dataItems = masterService.GetSystemMiscsByMiscType("factory")
            .OrderBy(t => t.SysMiscSortNo)
            .Where(t => t.IsActive)
             .Select(t => new { Name = ModelLocalizeManager.GetValue(t, "SysMisc"), Value = t.SysMiscCode })
            .ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("misctype")]
        public ActionResult GetMiscType()
        {
            return GetByMiscType("MiscType");
        }

        [Route("requestfor")]
        public ActionResult GetRequestFor()
        {
            return GetByMiscType("ReqFor");
        }

        [Route("requesttype")]
        public ActionResult GetRequestType()
        {
            return GetByMiscType("RequestType");
        }
        [Route("ReceiveCardType")]
        public ActionResult GetReceiveCardType()
        {
            return GetByMiscType("ReceiveCardType");
        }

        [Route("doctype")]
        public ActionResult GetDocumentType()
        {
            var dataItems = masterService.GetSystemMiscsByMiscType("DocType")
                .OrderBy(t => t.SysMiscSortNo)
            .Where(t => t.IsActive)
             .Select(t => new { Name = ModelLocalizeManager.GetValue(t, "SysMisc"), Value = t.SysMiscCode })
             .ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("Status")]
        public ActionResult GetStatus()
        {
            var dataItems = masterService.GetSystemMiscsByMiscType("Status")
                .OrderBy(t => t.SysMiscSortNo)
            .Where(t => t.IsActive)
             .Select(t => new { Name = ModelLocalizeManager.GetValue(t, "SysMisc"), Value = t.SysMiscCode })
             .ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("entryTime")]
        public ActionResult GetEntryTime()
        {
            var dataItem = masterService.GetDefaultEntryTime();
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }


    }
}