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
    
    public partial class ItemDataView
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemDisplayEN { get; set; }
        public string ItemDisplayTH { get; set; }
        public bool IsItemOut { get; set; }
        public bool IsItemIn { get; set; }
        public bool IsPhoto { get; set; }
        public bool IsConfdt { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int ItemTypeID { get; set; }
        public string MiscName { get; set; }
        public string MiscDisplayTH { get; set; }
        public string MiscDisplayEN { get; set; }
    }
}
