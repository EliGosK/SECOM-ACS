//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SECOM.ACS.Models
{
    using System;
    
    public partial class RequestDH03DataView
    {
        public string ObjectID { get; set; }
        public string DocumentTypeEN { get; set; }
        public string DocumentTypeTH { get; set; }
        public string ReqNo { get; set; }
        public string ReqStatusEN { get; set; }
        public string ReqStatusTH { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public Nullable<System.TimeSpan> EntryTimeFrom { get; set; }
        public Nullable<System.DateTime> EntryDateTo { get; set; }
        public Nullable<System.TimeSpan> EntryTimeTo { get; set; }
        public string RequestByEN { get; set; }
        public string RequestByTH { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string AreaDisplayEN { get; set; }
        public string ReqStatus { get; set; }
        public string AreaDisplayTH { get; set; }
    }
}