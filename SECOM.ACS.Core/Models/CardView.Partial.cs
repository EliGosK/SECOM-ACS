using CSI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class CardView
    {

    }

    public class VisitorCardRegistrationSearchCriteria :PagingOptions
    { 
        public DateTime EntryDate { get; set; }
        public string VisitorName { get; set; }
        public string Company { get; set; }
        public bool ClearCache { get; set; }
        public string CardStatus { get; set; }
    }

    public class BusinessTripCardRegistrationSearchCriteria : PagingOptions
    {
        public DateTime EntryDate { get; set; }
        public string BusinessTripEmployeeName { get; set; }
        public string RequestName { get; set; }
        public bool ClearCache { get; set; }
        public string CardStatus { get; set; }
    }

    public class CheckOverlapPeriodCardNoCriteria
    {
        public DateTime EntryDateFrom { get; set; }
        public DateTime EntryDateTo { get; set; }
        public string CardNo { get; set; }
    }
}
