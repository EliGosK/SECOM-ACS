using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(AcsTransactionViewModelResource))]
    public class AcsTransactionViewModel
    {
        public Guid TranID { get; set; }
        public string ReqNo { get; set; }
        public string ReqType { get; set; }
        public Guid DetailID { get; set; }
        public DateTime EntryDateFrom { get; set; }
        public TimeSpan EntryTimeFrom { get; set; }
        public DateTime EntryDateTo { get; set; }
        public TimeSpan EntryTimeTo { get; set; }
        public string CardID { get; set; }
        public Nullable<DateTime> CardReceiveTime { get; set; }
        public Nullable<DateTime> CardReturnTime { get; set; }
        public byte Status { get; set; }
        public Nullable<DateTime> SendAcsDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Nullable<DateTime> CancelAcsDate { get; set; }
    }
}