using CSI.Core;
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("AreaName", "AreaEN")]
    [LocalizeProperty("AreaName", "th", "AreaTH")]
    public partial class VisitorCardRegistrationView
    {
        public Misc VerifyType { get; set; }
        public Card Card { get; set; }
        public string UserName { get; set; }
    }

    [LocalizeProperty("AreaName", "AreaEN")]
    [LocalizeProperty("AreaName", "th", "AreaTH")]
    [LocalizeProperty("EmpName", "EmpNameEN")]
    [LocalizeProperty("EmpName", "th", "EmpNameTH")]
    public partial class BusinessTripCardRegistrationView
    {
        public Card Card { get; set; }
        public string UserName { get; set; }
        public string AreaName { get; set; }
        public string EmpName { get; set; }
    }

    [LocalizeProperty("Status", "StatusEN")]
    [LocalizeProperty("Status", "th", "StatusTH")]
    public partial class VIPCardRegistrationView
    {
        public Misc PositionType { get; set; }
        public string UserName { get; set; }

        public AreaDataView[] Area { get; set; }
        public int[] DefaultArea { get; set; }
    }

    public class VIPCardRegistrationSearchCriteria
    {
        public DateTime EntryDate { get; set; }
        public string CardStatus { get; set; }
    }

    public class RegisterCardSearchCriteria : PagingOptions
    {
        public string CardID { get; set; }
        public string CardType { get; set; }
    }
}
