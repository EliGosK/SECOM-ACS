using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class EmployeeApprovalDataViewModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string DepartmentID { get; set; }
        public int AreaID { get; set; }
        public string UserName { get; set; }
    }
}