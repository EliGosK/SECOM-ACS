using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("DepartmentName", "NameEN")]
    [LocalizeProperty("DepartmentName", "th", "NameTH")]
    public partial class Department
    {

    }

	public class DepartmentSearchCriteria
    {
        public string DepartmentID { get; set; }
        public string Name { get; set; }
    }
}
