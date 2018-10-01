using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(RequestDH08DataViewModelResource))]
    public class RequestDH08DataViewModel
    {
        public string ObjectID { get; set; }
        public string DocumentType { get; set; }
        public string ReqNo { get; set; }
        public string ReqStatus { get; set; }
        public string ReqStatusName { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public System.TimeSpan EntryTimeFrom { get; set; }
        public System.DateTime EntryDateTo { get; set; }
        public System.TimeSpan EntryTimeTo { get; set; }
        public string RequestBy { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string Area { get; set; }
        public string EquipName { get; set; }
        public string EntryTimeFromString
        {
            get { return  $"{this.EntryTimeFrom.Hours}:{this.EntryTimeFrom.Minutes:00}" ; }
        }

        public string EntryTimeToString
        {
            get { return  $"{this.EntryTimeTo.Hours}:{this.EntryTimeTo.Minutes:00}" ; }

        }
    }
}