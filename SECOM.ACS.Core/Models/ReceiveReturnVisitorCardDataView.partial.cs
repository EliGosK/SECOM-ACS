using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("Area","AreaEN")]
    [LocalizeProperty("Area", "th", "AreaTH")]
    public partial class ReceiveReturnVisitorCardDataView
    {
        public string UpdateBy { get; set; }
        public Misc VerifyType { get; set; }

    }

    public class VisitorCardDataSearchCriteria
    {
        public DateTime EntryDate { get; set; }
        public string[] Factory { get; set; }
        public string VisitorName { get; set; }
        public string Company { get; set; }
        public bool LoadFromCache { get; set; }
    }
}
