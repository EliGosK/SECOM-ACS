
using CSI.Web.Mvc;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/gates")]
    [RouteCulture]
    public class GateApiController : AppControllerBase
    {
        protected readonly IAccessControlService service;
        public GateApiController(IAccessControlService service)
        {
            this.service = service;
        }

        [Route("factory/{factoryCode}")]
        public ActionResult GetGatesByFactoryCode(string factoryCode)
        {
            var dataItem = service.GetGatesByFactoryCode(factoryCode);
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }

        [Route("{id}")]
        public ActionResult GetGate(string id)
        {
            var dataItem = service.GetGate(id);
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }
    }
}