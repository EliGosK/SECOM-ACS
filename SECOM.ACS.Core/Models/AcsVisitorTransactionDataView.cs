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
    
    public partial class AcsVisitorTransactionDataView
    {
        public System.Guid TranID { get; set; }
        public System.Guid DetailID { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public System.TimeSpan EntryTimeFrom { get; set; }
        public System.DateTime EntryDateTo { get; set; }
        public System.TimeSpan EntryTimeTo { get; set; }
        public Nullable<int> VerifyTypeID { get; set; }
        public string VerifyTypeEN { get; set; }
        public string VerifyTypeTH { get; set; }
        public string VerifyNo { get; set; }
        public string CardID { get; set; }
        public string CardNo { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string DeptName { get; set; }
    }
}
