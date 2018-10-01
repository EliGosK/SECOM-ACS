using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/roles")]
    [ApplicationAuthorize]
    [RouteCulture]
    public class RoleApiController : AppControllerBase
    {
        [HttpGet]
        [Route("")]
        public ActionResult GetAllRoles()
        {
            var values = RoleManager.Roles.OrderBy(t=>t.Name).ToList();
            return JsonNet(values, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("list")]
        public ActionResult ListRoles()
        {
            var values = RoleManager.Roles.ToList().Select(t => t.ToViewModel());
            return JsonNet(values, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("findbyname")]
        [Route("findbyname/{name}")]
        public ActionResult FindByName(string name)
        {
            var values = RoleManager.Roles.AsQueryable()
                .Where(t => t.Name.Contains(name))
                .Select(t => t.Name).ToArray();
            return JsonNet(values, JsonRequestBehavior.AllowGet);
        }



    }
}