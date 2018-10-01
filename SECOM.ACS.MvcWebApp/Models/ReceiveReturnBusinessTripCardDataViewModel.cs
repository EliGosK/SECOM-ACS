using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(ReceiveReturnBusinessTripCardDataViewModelResource))]
    public class ReceiveReturnBusinessTripCardDataViewModel
    {
        public System.Guid TranID { get; set; }
        public System.Guid DetailID { get; set; }
        public string BusinessEmployeeName { get; set; }
        public string EmployeeName { get; set; }
        public string AreaName { get; set; }       
        public string CardID { get; set; }
        public string CardNo { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public Nullable<System.DateTime> TimeIn { get; set; }
        public System.DateTime EntryDateTo { get; set; }
        public Nullable<System.DateTime> TimeOut { get; set; }

        public bool IsInEntryDateRange(DateTime entryDate)
        {
#if DEBUG
            return true;
#else
             return DateTime.Compare(entryDate.Date, this.EntryDateFrom) >= 0 && DateTime.Compare(this.EntryDateTo, entryDate.Date) >= 0;
#endif
        }

        public bool AllowReceive
        {
            get
            {
                return !this.TimeIn.HasValue && IsInEntryDateRange(DateTime.Now);
            }
        }

        public bool AllowReturn
        {
            get
            {
                return this.TimeIn.HasValue && !this.TimeOut.HasValue && IsInEntryDateRange(DateTime.Now);
            }
        }
    }
}