using CSI.Core;
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.Models
{
    public interface IEmployee
    {
        string EmpID { get;  }
    }

    [LocalizeProperty("EmpName","EmpNameEN")]
    [LocalizeProperty("EmpName","th", "EmpNameTH")]
    public partial class Employee : IEmployee
    {
        public int[] UserRoles { get; set; }
        public IList<AreaMapping> AreaMappings { get; set; } = new List<AreaMapping>();
        public bool LendingFlag { get; set; }


        public void MergeArea(int[] areaList)
        {
            foreach (var areaId in areaList)
            {
                if (this.AreaMappings.Any(t => t.AreaID == areaId)) { continue; }
                this.AreaMappings.Add(new AreaMapping() { CardID = this.CardID, AreaID = areaId, IsMainArea = this.AreaMappings.Count == 0, EmpID = this.EmpID });
            }
        }
    }

    public class EmployeeSearchCriteria : PagingOptions
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public EmployeeFilterTypes Filter { get; set; } = EmployeeFilterTypes.All;
    }

    [Flags]
    public enum EmployeeFilterTypes :int
    {
        EmployeeID = 1,
        EmployeeName = 2,
        All = EmployeeID | EmployeeName
    }

    [Flags]
    public enum LoadEmployeeOptions : int
    {
        None = 0,
        UserGroups = 1,
        Area = 2,
        All = UserGroups | Area
    }

    [LocalizeProperty("EmpName", "EmpNameEN")]
    [LocalizeProperty("EmpName", "th", "EmpNameTH")]
    [LocalizeProperty("Position", "PositionEN")]
    [LocalizeProperty("Position", "th", "PositionTH")]
    public partial class EmployeeApprovalDataView : IEmployee
    {

    }

    [LocalizeProperty("EmpName", "EmpNameEN")]
    [LocalizeProperty("EmpName", "th", "EmpNameTH")]
    [LocalizeProperty("Department", "DepartmentEN")]
    [LocalizeProperty("Department", "th", "DepartmentTH")]
    [LocalizeProperty("Position", "PositionEN")]
    [LocalizeProperty("Position", "th", "PositionTH")]
    public partial class EmployeeInformation : IEmployee
    {
        public int[] Roles { get; set; }
    }

    [LocalizeProperty("EmpName", "EmpNameEN")]
    [LocalizeProperty("EmpName", "th", "EmpNameTH")]
    [LocalizeProperty("Department", "DepartmentEN")]
    [LocalizeProperty("Department", "th", "DepartmentTH")]
    [LocalizeProperty("Position", "PositionEN")]
    [LocalizeProperty("Position", "th", "PositionTH")]
    public partial class EmployeeDataView : IEmployee
    {

    }
}