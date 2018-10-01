using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(RequestDH05DataViewModelResource))]
    public class RequestDH05DataViewModel
    {
        public string ObjectID { get; set; }
        public string DocumentType { get; set; }
        public string ReqNo { get; set; }
        public string ReqStatus { get; set; }
        public string ReqStatusName { get; set; }
        public System.DateTime TakeOutDate { get; set; }
        public Nullable<int> EntryTimeFrom { get; set; }
        public Nullable<int> EntryDateTo { get; set; }
        public Nullable<int> EntryTimeTo { get; set; }
        public string RequestBy { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string SourcePlace { get; set; }
        public string TakeOutCompany { get; set; }
        public string TakeOutDepartment { get; set; }
        public string TakeOutName { get; set; }
       
    }
}