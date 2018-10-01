using CSI.Localization;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [LocalizeProperty("EmpName", "EmpNameEN")]
    [LocalizeProperty("EmpName","th", "EmpNameTH")]
    public class EmployeeViewModel
    {
        public string EmployeeID { get; set; }
        public string UserName { get; set; }
        public string CardID { get; set; }
        public string Email { get; set; }
        public string DepartmentID { get; set; }
        public string PositionID { get; set; }
        public string Level { get; set; }
        public bool LendingFlag { get; set; }
        public bool IsHRLink { get; set; }
        public bool IsShow { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string EmpNameTH { get; set; }
        public string EmpNameEN { get; set; }

        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
    }
}