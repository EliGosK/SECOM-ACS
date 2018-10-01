using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("EmpName", "EmpNameEN")]
    [LocalizeProperty("EmpName", "th", "EmpNameTH")]
    [LocalizeProperty("Area", "AreaEN")]
    [LocalizeProperty("Area", "th", "AreaTH")]
    public partial class ReceiveReturnBusinessTripCardDataView
    {
        public string UpdateBy { get; set; }
    }

    public class BusinessTripCardDataSearchCriteria
    {
        public DateTime EntryDate { get; set; }
        public string[] Factory { get; set; }
        public string BusinessEmployeeName { get; set; }
        public string RequesterName { get; set; }
    }
}
