using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(ReceiveReturnVisitorCardDataViewModelResource))]
    public class ReceiveReturnVisitorCardDataViewModel
    {
        public System.Guid TranID { get; set; }
        public System.Guid DetailID { get; set; }
        public string VisitorName { get; set; }
        public string Company { get; set; }
        public string AreaName { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public Nullable<System.DateTime> TimeIn { get; set; }
        public System.DateTime EntryDateTo { get; set; }
        public Nullable<System.DateTime> TimeOut { get; set; }
        [UIHint("VerifyType")]
        public int VerifyTypeID { get; set; }
        public string VerifyTypeName { get; set; }
        public string VerifyNo { get; set; }
        public string CardID { get; set; }
        public string CardNo { get; set; }
        public string UpdateBy { get; set; }

        public bool IsInEntryDateRange(DateTime entryDate)
        {
#if DEBUG
            return true;
#else
             return DateTime.Compare(entryDate.Date, this.EntryDateFrom) >= 0 && DateTime.Compare(this.EntryDateTo, entryDate.Date) >= 0;
#endif
        }

        public bool AllowReceive {
            get {
                return !this.TimeIn.HasValue && IsInEntryDateRange(DateTime.Now);
            }
        }

        public bool AllowReturn
        {
            get {
                return this.TimeIn.HasValue && !this.TimeOut.HasValue && IsInEntryDateRange(DateTime.Now);
            }
        }
    }
}