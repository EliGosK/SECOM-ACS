using CSI.Localization;
using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(EmployeeDataViewModelResource))]
    public class EmployeeDataViewModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int AreaID { get; set; }
    }
}