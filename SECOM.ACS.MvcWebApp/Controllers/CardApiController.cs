using CSI.Localization;
using CSI.Web.Mvc;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/cards")]
    [RouteCulture]
    public class CardApiController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public CardApiController(IAccessControlService service)
        {
            this.service = service;
        }
        // GET: CardApi
        [Route("type/add")]
        public ActionResult GetCardTypeForAdd()
        {
            var dataItems = service.GetCardTypeForCreate();
            return JsonNet(dataItems.Select(t => new { Name = t.SysMiscValue1, Value = t.SysMiscCode }), JsonRequestBehavior.AllowGet);
        }
        [Route("type/Search")]
        public ActionResult GetCardTypeForSearch()
        {
            var dataItems = service.GetCardTypeForSearch();
            return JsonNet(dataItems.Select(t => new { Name = t.SysMiscValue1, Value = t.SysMiscCode }), JsonRequestBehavior.AllowGet);
        }
    }
}