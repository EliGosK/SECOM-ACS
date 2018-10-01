using CSI.Localization;
using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/area")]
    [RouteCulture]
    public class AreaApiController : AppControllerBase
    {
        protected readonly IAccessControlService service;
        public AreaApiController(IAccessControlService service)
        {
            this.service = service;
        }

        [Route("")]
        public ActionResult GetAllArea()
        {
            var dataItems = service.GetAllArea().Select(t => new
            {
                AreaID = t.AreaID,
                FactoryCode = t.FactoryCode,
                AreaName = t.AreaName,
                AreaDisplay = ModelLocalizeManager.GetValue(t,"AreaDisplay"),
                IsActive = t.IsActive,
                ConfdtLevel = t.ConfdtLevel
            }).ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("{id:int}")]
        public ActionResult GetArea(int id)
        {
            var dataItem = service.GetArea(id);
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// List all area
        /// </summary>
        /// <returns></returns>
        [Route("list")]
        public ActionResult GetAreaDataViews()
        {
            var dataItem = service.GetAreaDataViews(null,true)
                .Select(t => new AreaDataViewModel
                {
                    AreaID = t.AreaID,
                    FactoryCode = t.FactoryCode,
                    AreaDisplay = ModelLocalizeManager.GetValue(t, "AreaDisplay")
                });
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("listnotmapping")]
        public ActionResult GetAreaDataViewsNotMapping()
        {
            var dataItem = service.GetAreaDataViews(User.Identity.Name,false)
                .Select(t => new AreaDataViewModel
                {
                    AreaID = t.AreaID,
                    FactoryCode = t.FactoryCode,
                    AreaDisplay = ModelLocalizeManager.GetValue(t, "AreaDisplay")                    
                });
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("list/{user}/{mapping:bool}")]
        public ActionResult GetAreaDataViewsForUser(string user,bool mapping)
        {
            var dataItem = service.GetAreaDataViews(user, mapping)
                .Select(t => new AreaDataViewModel
                {
                    AreaID = t.AreaID,
                    FactoryCode = t.FactoryCode,
                    AreaDisplay = ModelLocalizeManager.GetValue(t, "AreaDisplay")
                });
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);

        }

        [Route("~/api/positions/areaapprover/list")]
        public ActionResult GetPositionsAreaApprover()
        {
            var dataItems = service.GetPositionsForAreaApprover().Select(t => new { Name = ModelLocalizeManager.GetValue(t, "PositionName"), Value = t.PositionID });
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/departments/areaapprover/list")]
        public ActionResult GetDepartmentsAreaApprover()
        {
            var dataItems = service.GetDepartmentForAreaApprover().Select(t => new { Name = ModelLocalizeManager.GetValue(t, "DepartmentName"), Value = t.DepartmentID });
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }
    }
}