using CSI.Web.Mvc;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/approvals")]
    [RouteCulture]
    public class ApprovalApiController : AppControllerBase
    {
        private readonly IAccessControlService service;
        public ApprovalApiController(IAccessControlService service)
        {
            this.service = service;
        }

        [Route("superior/{user?}")]
        public ActionResult Superior(string user)
        {
            var employee = user ?? User.Identity.Name;
            var dataItems = service.GetSuperiorApprover(employee).Select(t => t.ToViewModel()).ToList();                
            return JsonNet(dataItems.OrderBy(t=>t.EmployeeName, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true)), JsonRequestBehavior.AllowGet);
        }
       
        [Route("area/{areaId:int=0}")]
        public ActionResult Area(int areaId)
        {
            var dataItems = service.GetAreaApprover(areaId).Select(t => t.ToViewModel());
            return JsonNet(dataItems.OrderBy(t => t.EmployeeName, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true)), JsonRequestBehavior.AllowGet);
        }

        [Route("acknowledge/{name?}")]
        public ActionResult Acknowledge(string name)
        {
            var dataItems = service.GetAcknowledgeApprover(name).Select(t => t.ToViewModel());
            return JsonNet(dataItems.OrderBy(t => t.EmployeeName, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true)), JsonRequestBehavior.AllowGet);
        }

        [Route("ga")]
        public ActionResult GA()
        {
            var dataItems = service.GetGAApprover().Select(t => t.ToViewModel());
            return JsonNet(dataItems.OrderBy(t => t.EmployeeName, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true)), JsonRequestBehavior.AllowGet);
        }
    }
}