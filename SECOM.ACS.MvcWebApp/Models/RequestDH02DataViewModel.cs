using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(RequestDH02DataViewModelResource))]
    public class RequestDH02DataViewModel
    {
        public string ObjectID { get; set; }
        public string DocumentType { get; set; }
        public string ReqNo { get; set; }
        public string ReqStatus { get; set; }
        public string ReqStatusName { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public Nullable<System.TimeSpan> EntryTimeFrom { get; set; }
        public Nullable<System.DateTime> EntryDateTo { get; set; }
        public Nullable<System.TimeSpan> EntryTimeTo { get; set; }
        public string RequestBy { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string Area { get; set; }

        public string EntryTimeFromString { get { return EntryTimeFrom.HasValue ? String.Format("{0}:{1:00}",this.EntryTimeFrom.Value.Hours,this.EntryTimeFrom.Value.Minutes) : ""; } }
        public string EntryTimeToString { get { return EntryTimeTo.HasValue? $"{this.EntryTimeTo.Value.Hours}:{this.EntryTimeTo.Value.Minutes:00}": ""; }
}
    }
}