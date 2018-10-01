using CSI.Web.Mvc;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;
using CSI.Localization;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    [RoutePrefix("api/employees")]
    [RouteCulture]
    public class EmployeeApiController : AppControllerBase
    {
        private readonly IAccessControlService service;
        public EmployeeApiController(IAccessControlService service)
        {
            this.service = service;
        }

        [Route("")]
        public ActionResult GetAllEmployee()
        {
            var dataItems = service.GetAllEmployee();
            var result = dataItems.Select(t => new { EmployeeID = t.EmpID, EmployeeNameEN = t.EmpNameEN, EmployeeNameTH = t.EmpNameTH });
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [Route("info/{user}")]
        public ActionResult GetEmployeeInformation(string user)
        {
            var dataItem = service.GetEmployeeInformation(user);
            if (dataItem != null)
            {
                return JsonNet(dataItem.ToViewModel(), JsonRequestBehavior.AllowGet);
            }
            return JsonNet(new { }, JsonRequestBehavior.AllowGet);
        }

        [Route("{employeeNo}")]
        public ActionResult GetEmployee(string employeeNo)
        {
            var dataItem = service.GetEmployee(employeeNo);
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/positions/list")]
        public ActionResult GetPosition()
        {
            var dataItems = service.GetAllPosition().Select(t => new { Name = ModelLocalizeManager.GetValue(t, "PositionName"), Value = t.PositionID });
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/departments/list")]
        public ActionResult GetDepartment()
        {
            var dataItems = service.GetAllDepartment().Select(t => new { Name = ModelLocalizeManager.GetValue(t, "DepartmentName"), Value = t.DepartmentID });
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }
        
    }
}