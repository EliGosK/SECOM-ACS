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
    
    public partial class ReceiveReturnBusinessTripCardDataView
    {
        public System.Guid TranID { get; set; }
        public System.Guid DetailID { get; set; }
        public string EmpNameEN { get; set; }
        public string EmpNameTH { get; set; }
        public string AreaEN { get; set; }
        public string AreaTH { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public System.DateTime EntryDateTo { get; set; }
        public string CardID { get; set; }
        public string CardNo { get; set; }
        public Nullable<System.DateTime> TimeIn { get; set; }
        public Nullable<System.DateTime> TimeOut { get; set; }
        public string BusinessEmployee { get; set; }
    }
}
